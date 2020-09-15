using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Server.Tools;

namespace KF.Remoting
{
    // Token: 0x02000010 RID: 16
    public class CoupleArenaService
    {
        // Token: 0x0600006F RID: 111 RVA: 0x0000572C File Offset: 0x0000392C
        private CoupleArenaService()
        {
        }

        // Token: 0x06000070 RID: 112 RVA: 0x000057D4 File Offset: 0x000039D4
        public static CoupleArenaService getInstance()
        {
            return CoupleArenaService._Instance;
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00005838 File Offset: 0x00003A38
        public void StartUp()
        {
            try
            {
                int num;
                bool flag;
                IEnumerator<XElement> objA = XElement.Load(KuaFuServerManager.GetResourcePath(@"Config\CoupleWar.xml", KuaFuServerManager.ResourcePathTypes.GameRes)).Elements().GetEnumerator();
                try
                {
                    flag = objA.MoveNext();
                    if (flag)
                    {
                        string[] strArray = objA.Current.Attribute("TimePoints").Value.Split(new char[] { ',', '-', '|' });
                        num = 0;
                        while (true)
                        {
                            flag = num < strArray.Length;
                            if (!flag)
                            {
                                this._WarTimePointList.Sort((_l, _r) => _l.Weekday - _r.Weekday);
                                break;
                            }
                            _CoupleArenaWarTimePoint item = new _CoupleArenaWarTimePoint
                            {
                                Weekday = Convert.ToInt32(strArray[num])
                            };
                            if ((item.Weekday < 1) || (item.Weekday > 7))
                            {
                                throw new Exception("weekday error!");
                            }
                            item.DayStartTicks = DateTime.Parse(strArray[num + 1]).TimeOfDay.Ticks;
                            item.DayEndTicks = DateTime.Parse(strArray[num + 2]).TimeOfDay.Ticks;
                            this._WarTimePointList.Add(item);
                            num += 3;
                        }
                    }
                }
                finally
                {
                    if (!ReferenceEquals(objA, null))
                    {
                        objA.Dispose();
                    }
                }
                objA = XElement.Load(KuaFuServerManager.GetResourcePath(@"Config\CoupleDuanWei.xml", KuaFuServerManager.ResourcePathTypes.GameRes)).Elements().GetEnumerator();
                try
                {
                    while (true)
                    {
                        flag = objA.MoveNext();
                        if (!flag)
                        {
                            break;
                        }
                        XElement current = objA.Current;
                        _CoupleArenaDuanWeiCfg item = new _CoupleArenaDuanWeiCfg
                        {
                            NeedJiFen = Convert.ToInt32(current.Attribute("NeedCoupleDuanWeiJiFen").Value.ToString()),
                            DuanWeiType = Convert.ToInt32(current.Attribute("Type").Value.ToString()),
                            DuanWeiLevel = Convert.ToInt32(current.Attribute("Level").Value.ToString()),
                            WinJiFen = Convert.ToInt32(current.Attribute("WinJiFen").Value.ToString()),
                            LoseJiFen = Convert.ToInt32(current.Attribute("LoseJiFen").Value.ToString())
                        };
                        this._DuanWeiCfgList.Add(item);
                    }
                }
                finally
                {
                    if (!ReferenceEquals(objA, null))
                    {
                        objA.Dispose();
                    }
                }
                this._DuanWeiCfgList.Sort((_l, _r) => _l.NeedJiFen - _r.NeedJiFen);
                DateTime time = TimeUtil.NowDateTime();
                this.Persistence.CheckClearRank(this.CurrRankWeek(time));
                this.SyncData.RankList = this.Persistence.LoadRankFromDb();
                this.SyncData.BuildRoleDict();
                this.SyncData.ModifyTime = time;
                this.IsNeedSort = false;
                num = 1;
                while (true)
                {
                    flag = (num < this.SyncData.RankList.Count) && !this.IsNeedSort;
                    if (!flag)
                    {
                        this.CheckRebuildRank(time);
                        this.CheckFlushRank2Db();
                        break;
                    }
                    this.IsNeedSort |= this.SyncData.RankList[num].CompareTo(this.SyncData.RankList[num - 1]) < 0;
                    this.IsNeedSort |= this.SyncData.RankList[num].Rank != (this.SyncData.RankList[num - 1].Rank + 1);
                    num++;
                }
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogTypes.Error, "CoupleArenaService.InitConfig failed!", exception, true);
            }
        }

        // Token: 0x06000072 RID: 114 RVA: 0x00005C8C File Offset: 0x00003E8C
        public int CoupleArenaJoin(int roleId1, int roleId2, int serverId)
        {
            int result;
            lock (this.Mutex)
            {
                if (!this.IsValidCoupleIfExist(roleId1, roleId2))
                {
                    result = -11003;
                }
                else if (this.JoinDataUtil.GetJoinData(roleId1) != null || this.JoinDataUtil.GetJoinData(roleId2) != null)
                {
                    result = -12;
                }
                else
                {
                    CoupleArenaJoinData joinData = this.JoinDataUtil.Create();
                    joinData.ServerId = serverId;
                    joinData.RoleId1 = roleId1;
                    joinData.RoleId2 = roleId2;
                    joinData.StartTime = TimeUtil.NowDateTime();
                    CoupleArenaCoupleDataK coupleData;
                    if (this.SyncData.RoleDict.TryGetValue(roleId1, out coupleData))
                    {
                        joinData.DuanWeiLevel = coupleData.DuanWeiLevel;
                        joinData.DuanWeiType = coupleData.DuanWeiType;
                    }
                    else
                    {
                        joinData.DuanWeiLevel = this._DuanWeiCfgList[0].DuanWeiLevel;
                        joinData.DuanWeiType = this._DuanWeiCfgList[0].DuanWeiType;
                    }
                    this.JoinDataUtil.AddJoinData(joinData);
                    result = 1;
                }
            }
            return result;
        }

        // Token: 0x06000073 RID: 115 RVA: 0x00005DC0 File Offset: 0x00003FC0
        public int CoupleArenaQuit(int roleId1, int roleId2)
        {
            int result;
            lock (this.Mutex)
            {
                if (this.IsValidCoupleIfExist(roleId1, roleId2))
                {
                    this.JoinDataUtil.DelJoinData(this.JoinDataUtil.GetJoinData(roleId1));
                    this.JoinDataUtil.DelJoinData(this.JoinDataUtil.GetJoinData(roleId2));
                }
                result = 1;
            }
            return result;
        }

        // Token: 0x06000074 RID: 116 RVA: 0x00005E48 File Offset: 0x00004048
        public CoupleArenaSyncData CoupleArenaSync(DateTime lastSyncTime)
        {
            CoupleArenaSyncData result;
            lock (this.Mutex)
            {
                if (lastSyncTime == this.SyncData.ModifyTime)
                {
                    result = null;
                }
                else if (!TimeUtil.RandomDispatchTime(lastSyncTime, TimeUtil.NowDateTime(), 180, 60, 10))
                {
                    result = null;
                }
                else
                {
                    result = new CoupleArenaSyncData
                    {
                        RankList = new List<CoupleArenaCoupleDataK>(this.SyncData.RankList),
                        RoleDict = null,
                        ModifyTime = this.SyncData.ModifyTime
                    };
                }
            }
            return result;
        }

        // Token: 0x06000075 RID: 117 RVA: 0x00005F30 File Offset: 0x00004130
        public int CoupleArenaPreDivorce(int roleId1, int roleId2)
        {
            int result;
            lock (this.Mutex)
            {
                DateTime now = TimeUtil.NowDateTime();
                if (!this.IsValidCoupleIfExist(roleId1, roleId2))
                {
                    if (!this.IsInWeekRangeActTimes(now))
                    {
                        CoupleArenaCoupleDataK coupleData;
                        this.SyncData.RoleDict.TryGetValue(roleId1, out coupleData);
                        CoupleArenaCoupleDataK coupleData2;
                        this.SyncData.RoleDict.TryGetValue(roleId2, out coupleData2);
                        if (coupleData != null && coupleData.IsDivorced == 1)
                        {
                            return 1;
                        }
                        if (coupleData2 != null && coupleData2.IsDivorced == 1)
                        {
                            return 1;
                        }
                    }
                    result = -11003;
                }
                else
                {
                    this.CoupleArenaQuit(roleId1, roleId2);
                    CoupleArenaCoupleDataK data = null;
                    if (!this.IsInWeekRangeActTimes(now))
                    {
                        data = null;
                        if (this.SyncData.RoleDict.TryGetValue(roleId1, out data))
                        {
                            data.IsDivorced = 1;
                            this.Persistence.WriteCoupleData(data);
                            if (data.Rank == 1)
                            {
                                this.SyncData.ModifyTime = now;
                            }
                        }
                        result = 1;
                    }
                    else
                    {
                        data = null;
                        if (!this.SyncData.RoleDict.TryGetValue(roleId1, out data))
                        {
                            this.DivorceRecord.Add(roleId1, roleId2);
                            result = 1;
                        }
                        else if (data == null)
                        {
                            result = 1;
                        }
                        else if (!this.Persistence.ClearCoupleData(data.Db_CoupleId))
                        {
                            result = -15;
                        }
                        else
                        {
                            if (data.Rank - 1 >= 0 && data.Rank - 1 < this.SyncData.RankList.Count && this.SyncData.RankList[data.Rank - 1].Db_CoupleId == data.Db_CoupleId)
                            {
                                this.SyncData.RankList.RemoveAt(data.Rank - 1);
                            }
                            else
                            {
                                this.SyncData.RankList.RemoveAll((CoupleArenaCoupleDataK _r) => _r.Db_CoupleId == data.Db_CoupleId);
                            }
                            this.DivorceRecord.Add(roleId1, roleId2);
                            this.SyncData.BuildRoleDict();
                            this.SyncData.ModifyTime = now;
                            this.IsNeedSort = true;
                            result = 1;
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x06000076 RID: 118 RVA: 0x00006230 File Offset: 0x00004430
        public CoupleArenaFuBenData GetCoupleFuBenData(long gameId)
        {
            CoupleArenaFuBenData result;
            lock (this.Mutex)
            {
                CoupleArenaFuBenData data;
                if (!this.GameFuBenDict.TryGetValue(gameId, out data))
                {
                    data = null;
                }
                result = data;
            }
            return result;
        }

        // Token: 0x06000077 RID: 119 RVA: 0x00006290 File Offset: 0x00004490
        public CoupleArenaPkResultRsp CoupleArenaPkResult(CoupleArenaPkResultReq req)
        {
            CoupleArenaPkResultRsp result;
            if (req == null)
            {
                result = null;
            }
            else
            {
                DateTime now = TimeUtil.NowDateTime();
                lock (this.Mutex)
                {
                    CoupleArenaFuBenData fuben;
                    if (!this.IsValidCoupleIfExist(req.ManRole1, req.WifeRole1) || !this.IsValidCoupleIfExist(req.ManRole2, req.WifeRole2))
                    {
                        result = null;
                    }
                    else if (!this.GameFuBenDict.TryGetValue(req.GameId, out fuben))
                    {
                        result = null;
                    }
                    else
                    {
                        CoupleArenaPkResultRsp rsp = new CoupleArenaPkResultRsp();
                        if (req.winSide == 0)
                        {
                            rsp.Couple1RetData.Result = 0;
                            rsp.Couple2RetData.Result = 0;
                        }
                        else if (req.winSide == 1)
                        {
                            rsp.Couple1RetData.Result = (this.DivorceRecord.IsDivorce(req.ManRole1, req.WifeRole1) ? 0 : 1);
                            rsp.Couple2RetData.Result = (this.DivorceRecord.IsDivorce(req.ManRole2, req.WifeRole2) ? 0 : 2);
                        }
                        else
                        {
                            rsp.Couple1RetData.Result = (this.DivorceRecord.IsDivorce(req.ManRole1, req.WifeRole1) ? 0 : 2);
                            rsp.Couple2RetData.Result = (this.DivorceRecord.IsDivorce(req.ManRole2, req.WifeRole2) ? 0 : 1);
                        }
                        int duanweiType = this._DuanWeiCfgList[0].DuanWeiType;
                        int duanweiLevel = this._DuanWeiCfgList[0].DuanWeiLevel;
                        int duanweiType2 = this._DuanWeiCfgList[0].DuanWeiType;
                        int duanweiLevel2 = this._DuanWeiCfgList[0].DuanWeiLevel;
                        if (this.SyncData.RoleDict.ContainsKey(req.ManRole1))
                        {
                            duanweiType = this.SyncData.RoleDict[req.ManRole1].DuanWeiType;
                            duanweiLevel = this.SyncData.RoleDict[req.ManRole1].DuanWeiLevel;
                        }
                        if (this.SyncData.RoleDict.ContainsKey(req.ManRole2))
                        {
                            duanweiType2 = this.SyncData.RoleDict[req.ManRole2].DuanWeiType;
                            duanweiLevel2 = this.SyncData.RoleDict[req.ManRole2].DuanWeiLevel;
                        }
                        this.HandlePkResult(req.ManRole1, req.ManZoneId1, req.ManSelector1, req.WifeRole1, req.WifeZoneId1, req.WifeSelector1, duanweiType2, duanweiLevel2, rsp.Couple1RetData);
                        this.HandlePkResult(req.ManRole2, req.ManZoneId2, req.ManSelector2, req.WifeRole2, req.WifeZoneId2, req.WifeSelector2, duanweiType, duanweiLevel, rsp.Couple2RetData);
                        this.RemoveFuBen(req.GameId);
                        this.Persistence.AddPkLog(fuben.GameId, fuben.StartTime, TimeUtil.NowDateTime(), req.ManRole1, req.WifeRole1, rsp.Couple1RetData.Result, req.ManRole2, req.WifeRole2, rsp.Couple2RetData.Result);
                        result = rsp;
                    }
                }
            }
            return result;
        }

        // Token: 0x06000078 RID: 120 RVA: 0x0000668C File Offset: 0x0000488C
        private void HandlePkResult(int man, int manzone, byte[] mandata, int wife, int wifezone, byte[] wifedata, int pkDuanWeiType, int pkDuanWeiLevel, CoupleArenaPkResultItem retData)
        {
            CoupleArenaCoupleDataK coupleData = null;
            if (!this.SyncData.RoleDict.TryGetValue(man, out coupleData))
            {
                coupleData = new CoupleArenaCoupleDataK();
                coupleData.Db_CoupleId = this.Persistence.GetNextDbCoupleId();
                coupleData.ManRoleId = man;
                coupleData.ManZoneId = manzone;
                coupleData.ManSelectorData = mandata;
                coupleData.WifeRoleId = wife;
                coupleData.WifeZoneId = wifezone;
                coupleData.WifeSelectorData = wifedata;
                coupleData.DuanWeiLevel = this._DuanWeiCfgList[0].DuanWeiLevel;
                coupleData.DuanWeiType = this._DuanWeiCfgList[0].DuanWeiType;
                coupleData.Rank = this.SyncData.RankList.Count + 1;
                if (retData.Result != 0)
                {
                    this.SyncData.RankList.Add(coupleData);
                    this.SyncData.RoleDict[coupleData.ManRoleId] = coupleData;
                    this.SyncData.RoleDict[coupleData.WifeRoleId] = coupleData;
                }
            }
            else
            {
                coupleData.ManSelectorData = mandata;
                coupleData.WifeSelectorData = wifedata;
            }
            retData.OldDuanWeiType = coupleData.DuanWeiType;
            retData.OldDuanWeiLevel = coupleData.DuanWeiLevel;
            _CoupleArenaDuanWeiCfg duanweiCfgLose = this._DuanWeiCfgList.Find((_CoupleArenaDuanWeiCfg _d) => _d.DuanWeiLevel == coupleData.DuanWeiLevel && _d.DuanWeiType == coupleData.DuanWeiType);
            if (duanweiCfgLose == null)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("couplearena.HandlePkResult can't find duanwei cfg ,type={0}, level={1}", coupleData.DuanWeiType, coupleData.DuanWeiLevel), null, true);
            }
            else
            {
                _CoupleArenaDuanWeiCfg duanweiCfgWin = this._DuanWeiCfgList.Find((_CoupleArenaDuanWeiCfg _d) => _d.DuanWeiLevel == pkDuanWeiLevel && _d.DuanWeiType == pkDuanWeiType);
                if (duanweiCfgWin == null)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("couplearena.HandlePkResult can't find duanwei cfg ,type={0}, level={1}", pkDuanWeiType, pkDuanWeiLevel), null, true);
                }
                else
                {
                    if (retData.Result == 0)
                    {
                        retData.NewDuanWeiType = coupleData.DuanWeiType;
                        retData.NewDuanWeiLevel = coupleData.DuanWeiLevel;
                    }
                    else
                    {
                        coupleData.TotalFightTimes++;
                        if (retData.Result == 1)
                        {
                            coupleData.WinFightTimes++;
                            coupleData.LianShengTimes++;
                            coupleData.JiFen += duanweiCfgWin.WinJiFen;
                            retData.GetJiFen = duanweiCfgWin.WinJiFen;
                        }
                        else
                        {
                            coupleData.LianShengTimes = 0;
                            coupleData.JiFen += duanweiCfgLose.LoseJiFen;
                            coupleData.JiFen = Math.Max(coupleData.JiFen, 0);
                            retData.GetJiFen = duanweiCfgLose.LoseJiFen;
                        }
                        this.ParseDuanweiByJiFen(coupleData.JiFen, out coupleData.DuanWeiType, out coupleData.DuanWeiLevel);
                        this.SyncData.ModifyTime = TimeUtil.NowDateTime();
                        retData.NewDuanWeiLevel = coupleData.DuanWeiLevel;
                        retData.NewDuanWeiType = coupleData.DuanWeiType;
                        this.IsNeedSort = true;
                    }
                    if (retData.Result != 0)
                    {
                        this.Persistence.WriteCoupleData(coupleData);
                    }
                }
            }
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00006A78 File Offset: 0x00004C78
        private void ParseDuanweiByJiFen(int jifen, out int duanweiType, out int duanweiLevel)
        {
            duanweiLevel = this._DuanWeiCfgList[0].DuanWeiLevel;
            duanweiType = this._DuanWeiCfgList[0].DuanWeiType;
            for (int i = 0; i < this._DuanWeiCfgList.Count; i++)
            {
                if (jifen >= this._DuanWeiCfgList[i].NeedJiFen)
                {
                    if (i == this._DuanWeiCfgList.Count - 1 || jifen < this._DuanWeiCfgList[i + 1].NeedJiFen)
                    {
                        duanweiType = this._DuanWeiCfgList[i].DuanWeiType;
                        duanweiLevel = this._DuanWeiCfgList[i].DuanWeiLevel;
                    }
                }
            }
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00006B40 File Offset: 0x00004D40
        public void Update()
        {
            try
            {
                lock (this.Mutex)
                {
                    DateTime now = TimeUtil.NowDateTime();
                    if ((now - this.LastUpdateTime).TotalMilliseconds >= 1000.0)
                    {
                        this.UpdateFrameCount += 1U;
                        if (now.DayOfYear != this.LastUpdateTime.DayOfYear)
                        {
                            this.MatchTimeLimiter.Reset();
                            this.JoinDataUtil.Reset();
                            this.DivorceRecord.Reset();
                        }
                        this.CheckRoleMatch(now);
                        if (this.UpdateFrameCount % 30U == 0U)
                        {
                            this.CheckTimeOutFuBen(now);
                        }
                        if (this.LastUpdateTime.TimeOfDay.Ticks <= this._WarTimePointList.First<_CoupleArenaWarTimePoint>().DayStartTicks && now.TimeOfDay.Ticks >= this._WarTimePointList.First<_CoupleArenaWarTimePoint>().DayStartTicks)
                        {
                            if (this.Persistence.CheckClearRank(this.CurrRankWeek(now)))
                            {
                                lock (this.Mutex)
                                {
                                    this.SyncData.RankList.Clear();
                                    this.SyncData.BuildRoleDict();
                                    this.SyncData.ModifyTime = now;
                                }
                            }
                        }
                        if (this.UpdateFrameCount % 30U == 0U)
                        {
                            this.CheckRebuildRank(now);
                        }
                        if (this.UpdateFrameCount % 300U == 0U)
                        {
                            this.CheckFlushRank2Db();
                        }
                        this.LastUpdateTime = now;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteExceptionUseCache("CoupleArenaService.Update failed! " + ex.Message);
            }
        }

        // Token: 0x0600007B RID: 123 RVA: 0x00006D98 File Offset: 0x00004F98
        private void CheckRoleMatch(DateTime now)
        {
            lock (this.Mutex)
            {
                List<CoupleArenaJoinData> joinDatas = this.JoinDataUtil.GetJoinList();
                if (joinDatas != null && joinDatas.Count > 0)
                {
                    CoupleArenaJoinMatcher joinMatcher = new CoupleArenaJoinMatcher();
                    foreach (CoupleArenaJoinData joinData in joinDatas)
                    {
                        if ((now - joinData.StartTime).TotalSeconds >= 60.0 || joinData.ToKfServerId > 0)
                        {
                            this.JoinDataUtil.DelJoinData(joinData);
                        }
                        else if ((now - joinData.StartTime).TotalSeconds >= 30.0)
                        {
                            joinMatcher.AddGlobalJoinData(joinData);
                        }
                        else
                        {
                            joinMatcher.AddJoinData(joinData.DuanWeiType, joinData.DuanWeiLevel, joinData);
                        }
                    }
                    foreach (List<CoupleArenaJoinData> list in joinMatcher.GetAllMatch())
                    {
                        int i = 0;
                        while (i < list.Count - 1)
                        {
                            CoupleArenaJoinData one = list[i];
                            CoupleArenaJoinData two = list[i + 1];
                            if (this.MatchTimeLimiter.GetMatchTimes(one.RoleId1, one.RoleId2, two.RoleId1, two.RoleId2) >= TianTiPersistence.Instance.MaxRolePairFightCount)
                            {
                                i++;
                            }
                            else
                            {
                                CoupleArenaFuBenData fubenData = new CoupleArenaFuBenData();
                                fubenData.GameId = this.Persistence.GetNextGameId();
                                fubenData.StartTime = now;
                                fubenData.RoleList = new List<KuaFuFuBenRoleData>();
                                fubenData.RoleList.Add(new KuaFuFuBenRoleData
                                {
                                    ServerId = one.ServerId,
                                    RoleId = one.RoleId1,
                                    Side = 1
                                });
                                fubenData.RoleList.Add(new KuaFuFuBenRoleData
                                {
                                    ServerId = one.ServerId,
                                    RoleId = one.RoleId2,
                                    Side = 1
                                });
                                fubenData.RoleList.Add(new KuaFuFuBenRoleData
                                {
                                    ServerId = two.ServerId,
                                    RoleId = two.RoleId1,
                                    Side = 2
                                });
                                fubenData.RoleList.Add(new KuaFuFuBenRoleData
                                {
                                    ServerId = two.ServerId,
                                    RoleId = two.RoleId2,
                                    Side = 2
                                });
                                if (!ClientAgentManager.Instance().AssginKfFuben(this.GameType, fubenData.GameId, 4, out fubenData.KfServerId))
                                {
                                    LogManager.WriteLog(LogTypes.Error, "CoupleArena 没有跨服可以分配", null, true);
                                    return;
                                }
                                this.MatchTimeLimiter.AddMatchTimes(one.RoleId1, one.RoleId2, two.RoleId1, two.RoleId2, 1);
                                this.GameFuBenDict[fubenData.GameId] = fubenData;
                                i += 2;
                                one.ToKfServerId = fubenData.KfServerId;
                                two.ToKfServerId = fubenData.KfServerId;
                                CoupleArenaCanEnterData enterData = new CoupleArenaCanEnterData
                                {
                                    GameId = fubenData.GameId,
                                    KfServerId = fubenData.KfServerId,
                                    RoleId1 = one.RoleId1,
                                    RoleId2 = one.RoleId2
                                };
                                ClientAgentManager.Instance().PostAsyncEvent(one.ServerId, this.EvItemGameType, new AsyncDataItem(KuaFuEventTypes.CoupleArenaCanEnter, new object[]
                                {
                                    enterData
                                }));
                                CoupleArenaCanEnterData enterData2 = new CoupleArenaCanEnterData
                                {
                                    GameId = fubenData.GameId,
                                    KfServerId = fubenData.KfServerId,
                                    RoleId1 = two.RoleId1,
                                    RoleId2 = two.RoleId2
                                };
                                AsyncDataItem evItem2 = new AsyncDataItem(KuaFuEventTypes.CoupleArenaCanEnter, new object[]
                                {
                                    fubenData.GameId,
                                    fubenData.KfServerId,
                                    two.RoleId1,
                                    two.RoleId2
                                });
                                ClientAgentManager.Instance().PostAsyncEvent(two.ServerId, this.EvItemGameType, new AsyncDataItem(KuaFuEventTypes.CoupleArenaCanEnter, new object[]
                                {
                                    enterData2
                                }));
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0600007C RID: 124 RVA: 0x000072D0 File Offset: 0x000054D0
        private void CheckTimeOutFuBen(DateTime now)
        {
            lock (this.Mutex)
            {
                foreach (CoupleArenaFuBenData fuben in this.GameFuBenDict.Values.ToList<CoupleArenaFuBenData>())
                {
                    if ((now - fuben.StartTime).TotalMinutes > 5.0)
                    {
                        this.RemoveFuBen(fuben.GameId);
                    }
                }
            }
        }

        // Token: 0x0600007D RID: 125 RVA: 0x0000739C File Offset: 0x0000559C
        private void RemoveFuBen(long gameId)
        {
            lock (this.Mutex)
            {
                CoupleArenaFuBenData fuben;
                if (this.GameFuBenDict.TryGetValue(gameId, out fuben))
                {
                    ClientAgentManager.Instance().RemoveKfFuben(this.GameType, fuben.KfServerId, fuben.GameId);
                    this.GameFuBenDict.Remove(fuben.GameId);
                }
            }
        }

        // Token: 0x0600007E RID: 126 RVA: 0x00007428 File Offset: 0x00005628
        private void CheckRebuildRank(DateTime now)
        {
            lock (this.Mutex)
            {
                if (this.IsNeedSort)
                {
                    this.SyncData.RankList.Sort();
                    for (int i = 0; i < this.SyncData.RankList.Count; i++)
                    {
                        this.SyncData.RankList[i].Rank = i + 1;
                    }
                    this.SyncData.BuildRoleDict();
                    this.SyncData.ModifyTime = now;
                    this.IsNeedSort = false;
                    this.IsRankChanged = true;
                }
            }
        }

        // Token: 0x0600007F RID: 127 RVA: 0x000074F0 File Offset: 0x000056F0
        private void CheckFlushRank2Db()
        {
            lock (this.Mutex)
            {
                if (this.IsRankChanged)
                {
                    LogManager.WriteLog(LogTypes.Error, "Persistence.FlushRandList2Db begin", null, true);
                    this.Persistence.FlushRandList2Db(this.SyncData.RankList);
                    LogManager.WriteLog(LogTypes.Error, "Persistence.FlushRandList2Db end", null, true);
                    this.IsRankChanged = false;
                }
            }
        }

        // Token: 0x06000080 RID: 128 RVA: 0x0000757C File Offset: 0x0000577C
        private int CurrRankWeek(DateTime time)
        {
            int currWeekDay = TimeUtil.GetWeekDay1To7(time);
            _CoupleArenaWarTimePoint first = this._WarTimePointList.First<_CoupleArenaWarTimePoint>();
            int result;
            if (currWeekDay < first.Weekday || (currWeekDay == first.Weekday && time.TimeOfDay.Ticks < first.DayStartTicks))
            {
                result = TimeUtil.MakeFirstWeekday(time.AddDays(-7.0));
            }
            else
            {
                result = TimeUtil.MakeFirstWeekday(time);
            }
            return result;
        }

        // Token: 0x06000081 RID: 129 RVA: 0x000075FC File Offset: 0x000057FC
        private bool IsInWeekOnceActTimes(DateTime time)
        {
            int wd = TimeUtil.GetWeekDay1To7(time);
            foreach (_CoupleArenaWarTimePoint tp in this._WarTimePointList)
            {
                if (tp.Weekday == wd && time.TimeOfDay.Ticks >= tp.DayStartTicks && time.TimeOfDay.Ticks <= tp.DayEndTicks)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06000082 RID: 130 RVA: 0x000076A8 File Offset: 0x000058A8
        private bool IsInWeekRangeActTimes(DateTime time)
        {
            _CoupleArenaWarTimePoint first = this._WarTimePointList.First<_CoupleArenaWarTimePoint>();
            _CoupleArenaWarTimePoint last = this._WarTimePointList.Last<_CoupleArenaWarTimePoint>();
            int wd = TimeUtil.GetWeekDay1To7(time);
            return ((wd == first.Weekday && time.TimeOfDay.Ticks > first.DayStartTicks) || wd > first.Weekday) && (wd < last.Weekday || (wd == last.Weekday && time.TimeOfDay.Ticks < last.DayEndTicks));
        }

        // Token: 0x06000083 RID: 131 RVA: 0x00007748 File Offset: 0x00005948
        private bool IsValidCoupleIfExist(int roleId1, int roleId2)
        {
            bool result;
            lock (this.Mutex)
            {
                CoupleArenaCoupleDataK coupleData;
                this.SyncData.RoleDict.TryGetValue(roleId1, out coupleData);
                CoupleArenaCoupleDataK coupleData2;
                this.SyncData.RoleDict.TryGetValue(roleId2, out coupleData2);
                if (coupleData == null && coupleData2 == null)
                {
                    result = true;
                }
                else if (coupleData == null || coupleData2 == null)
                {
                    result = false;
                }
                else if (!object.ReferenceEquals(coupleData, coupleData2))
                {
                    result = false;
                }
                else if ((coupleData.ManRoleId == roleId1 && coupleData.WifeRoleId == roleId2) || (coupleData.ManRoleId == roleId2 && coupleData.WifeRoleId == roleId1))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        // Token: 0x06000084 RID: 132 RVA: 0x00007838 File Offset: 0x00005A38
        public void OnStopServer()
        {
            try
            {
                SysConOut.WriteLine("开始检测是否刷新情侣竞技数据到数据库...");
                lock (this.Mutex)
                {
                    this.CheckRebuildRank(TimeUtil.NowDateTime());
                    this.CheckFlushRank2Db();
                }
                SysConOut.WriteLine("结束检测是否刷新情侣竞技数据到数据库...");
            }
            catch (Exception ex)
            {
                LogManager.WriteException(ex.Message);
            }
        }

        // Token: 0x04000045 RID: 69
        private static CoupleArenaService _Instance = new CoupleArenaService();

        // Token: 0x04000046 RID: 70
        private List<_CoupleArenaDuanWeiCfg> _DuanWeiCfgList = new List<_CoupleArenaDuanWeiCfg>();

        // Token: 0x04000047 RID: 71
        private List<_CoupleArenaWarTimePoint> _WarTimePointList = new List<_CoupleArenaWarTimePoint>();

        // Token: 0x04000048 RID: 72
        public readonly GameTypes GameType = GameTypes.CoupleArena;

        // Token: 0x04000049 RID: 73
        public readonly GameTypes EvItemGameType = GameTypes.TianTi;

        // Token: 0x0400004A RID: 74
        private object Mutex = new object();

        // Token: 0x0400004B RID: 75
        private DateTime LastUpdateTime = DateTime.MinValue;

        // Token: 0x0400004C RID: 76
        private uint UpdateFrameCount = 0U;

        // Token: 0x0400004D RID: 77
        private CoupleArenaSyncData SyncData = new CoupleArenaSyncData();

        // Token: 0x0400004E RID: 78
        private bool IsNeedSort = false;

        // Token: 0x0400004F RID: 79
        private bool IsRankChanged = false;

        // Token: 0x04000050 RID: 80
        private Dictionary<long, CoupleArenaFuBenData> GameFuBenDict = new Dictionary<long, CoupleArenaFuBenData>();

        // Token: 0x04000051 RID: 81
        private CoupleArenaMatchTimeLimiter MatchTimeLimiter = new CoupleArenaMatchTimeLimiter();

        // Token: 0x04000052 RID: 82
        private CoupleArenaJoinDataUtil JoinDataUtil = new CoupleArenaJoinDataUtil();

        // Token: 0x04000053 RID: 83
        private CoupleArenaDivorceRecord DivorceRecord = new CoupleArenaDivorceRecord();

        // Token: 0x04000054 RID: 84
        private CoupleArenaPersistence Persistence = CoupleArenaPersistence.getInstance();
    }
}
