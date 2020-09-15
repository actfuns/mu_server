using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Contract;

namespace KF.Remoting
{
    // Token: 0x02000074 RID: 116
    internal class ZhengBaManagerK
    {
        // Token: 0x060005BC RID: 1468 RVA: 0x0004D798 File Offset: 0x0004B998
        private ZhengBaManagerK()
        {
            this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.Idle, null, new Action<DateTime>(this.MS_Idle_Update), null));
            this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.TodayPkStart, new Action<DateTime>(this.MS_TodayPkStart_Enter), null, null));
            this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.InitPkLoop, new Action<DateTime>(this.MS_InitPkLoop_Enter), null, null));
            this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.NotifyEnter, null, new Action<DateTime>(this.MS_NotifyEnter_Update), null));
            this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.PkLoopStart, null, new Action<DateTime>(this.MS_PkLoopStart_Update), null));
            this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.PkLoopEnd, null, new Action<DateTime>(this.MS_PkLoopEnd_Update), null));
            this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.TodayPkEnd, new Action<DateTime>(this.MS_TodayPkEnd_Enter), null, null));
            this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.Idle, TimeUtil.NowDateTime());
            this.Persistence.MonthRankFirstCreate = delegate (int selectRoleIfNewCreate)
            {
                lock (this.Mutex)
                {
                    this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaButtetinJoin, new object[]
                    {
                        new ZhengBaBulletinJoinData
                        {
                            NtfType = ZhengBaBulletinJoinData.ENtfType.BulletinServer,
                            Args1 = selectRoleIfNewCreate
                        }
                    }));
                    this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaButtetinJoin, new object[]
                    {
                        new ZhengBaBulletinJoinData
                        {
                            NtfType = ZhengBaBulletinJoinData.ENtfType.MailJoinRole
                        }
                    }));
                }
            };
        }

        // Token: 0x060005BD RID: 1469 RVA: 0x0004D950 File Offset: 0x0004BB50
        public static ZhengBaManagerK Instance()
        {
            return ZhengBaManagerK._instance;
        }

        // Token: 0x060005BE RID: 1470 RVA: 0x0004D968 File Offset: 0x0004BB68
        public AsyncDataItem[] Update()
        {
            DateTime now = TimeUtil.NowDateTime();
            if (now.Month != this.lastUpdateTime.Month)
            {
                this.ReloadSyncData(now);
            }
            else if (now.Day != this.lastUpdateTime.Day)
            {
                this.FixSyncData(now);
            }
            else
            {
                lock (this.Mutex)
                {
                    this.StateMachine.Tick(now);
                }
            }
            AsyncDataItem[] asyncEvArray = null;
            lock (this.Mutex)
            {
                asyncEvArray = this.AsyncEvQ.ToArray();
                this.AsyncEvQ.Clear();
            }
            this.lastUpdateTime = now;
            return asyncEvArray;
        }

        // Token: 0x060005BF RID: 1471 RVA: 0x0004DA98 File Offset: 0x0004BC98
        private void MS_Idle_Update(DateTime now)
        {
            if (this.SyncData.RealActDay >= 1 && this.SyncData.RealActDay <= 7)
            {
                ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
                if (this.lastUpdateTime.TimeOfDay.Ticks < matchConfig.DayBeginTick && now.TimeOfDay.Ticks >= matchConfig.DayBeginTick)
                {
                    this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.TodayPkStart, now);
                }
                else if (this.lastUpdateTime.TimeOfDay.Ticks < matchConfig.DayEndTick && now.TimeOfDay.Ticks >= matchConfig.DayEndTick)
                {
                    this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.TodayPkEnd, now);
                }
            }
        }

        // Token: 0x060005C0 RID: 1472 RVA: 0x0004DB88 File Offset: 0x0004BD88
        private void MS_TodayPkStart_Enter(DateTime now)
        {
            this.SyncData.TodayIsPking = true;
            this.TodayJoinRoleDatas.Clear();
            this.CurrLoopIndex = 0;
            this.HadUpGradeRoleNum = 0;
            foreach (ZhengBaRoleInfoData role in this.SyncData.RoleList)
            {
                if (role.State == 0 || role.State == 1)
                {
                    this.TodayJoinRoleDatas.Add(new ZhengBaManagerK.JoinRolePkData
                    {
                        RoleID = role.RoleId,
                        ZoneId = role.ZoneId,
                        RoleName = role.RoleName,
                        Group = role.Group
                    });
                }
            }
            if (this.SyncData.RealActDay == 3)
            {
                this.RandomGroup.Clear();
                this.RandomGroup.AddRange(Enumerable.Range(1, 16));
                Random r = new Random((int)now.Ticks);
                int i = 0;
                while (this.TodayJoinRoleDatas.Count >= 16 && i < 50)
                {
                    int idx = r.Next(0, this.RandomGroup.Count);
                    int idx2 = r.Next(0, this.RandomGroup.Count);
                    int tmp = this.RandomGroup[idx];
                    this.RandomGroup[idx] = this.RandomGroup[idx2];
                    this.RandomGroup[idx2] = tmp;
                    i++;
                }
            }
            this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.InitPkLoop, now);
        }

        // Token: 0x060005C1 RID: 1473 RVA: 0x0004DDC0 File Offset: 0x0004BFC0
        private void MS_InitPkLoop_Enter(DateTime now)
        {
            this.ThisLoopPkLogs.Clear();
            this.CurrLoopIndex++;
            ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
            if (this.HadUpGradeRoleNum >= matchConfig.MaxUpGradeNum || this.TodayJoinRoleDatas.Count <= 1)
            {
                this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.TodayPkEnd, now);
            }
            else
            {
                if (matchConfig.Mathching == EZhengBaMatching.Random)
                {
                    Random r = new Random((int)now.Ticks);
                    int i = 0;
                    while (this.TodayJoinRoleDatas.Count > 0 && i < this.TodayJoinRoleDatas.Count * 2)
                    {
                        int idx = r.Next(0, this.TodayJoinRoleDatas.Count);
                        int idx2 = r.Next(0, this.TodayJoinRoleDatas.Count);
                        ZhengBaManagerK.JoinRolePkData tmp = this.TodayJoinRoleDatas[idx];
                        this.TodayJoinRoleDatas[idx] = this.TodayJoinRoleDatas[idx2];
                        this.TodayJoinRoleDatas[idx2] = tmp;
                        i++;
                    }
                }
                else if (matchConfig.Mathching == EZhengBaMatching.Group)
                {
                    List<ZhengBaManagerK.JoinRolePkData> tmpRoleDatas = new List<ZhengBaManagerK.JoinRolePkData>();
                    using (List<RangeKey>.Enumerator enumerator = ZhengBaUtils.GetDayPkGroupRange(this.SyncData.RealActDay).GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            RangeKey range = enumerator.Current;
                            List<ZhengBaManagerK.JoinRolePkData> groupRoles = this.TodayJoinRoleDatas.FindAll((ZhengBaManagerK.JoinRolePkData _r) => _r.Group >= range.Left && _r.Group <= range.Right);
                            if (groupRoles != null && groupRoles.Count == 2)
                            {
                                tmpRoleDatas.AddRange(groupRoles);
                            }
                        }
                    }
                    this.TodayJoinRoleDatas.Clear();
                    this.TodayJoinRoleDatas.AddRange(tmpRoleDatas);
                }
                else
                {
                    Debug.Assert(false, "unknown pk match type");
                }
                int currIdx = 0;
                for (int i = 0; i < this.TodayJoinRoleDatas.Count / 2; i++)
                {
                    ZhengBaManagerK.JoinRolePkData joinRole = this.TodayJoinRoleDatas[currIdx++];
                    ZhengBaManagerK.JoinRolePkData joinRole2 = this.TodayJoinRoleDatas[currIdx++];
                    int toServerId = 0;
                    int gameId = TianTiPersistence.Instance.GetNextGameId();
                    if (ClientAgentManager.Instance().AssginKfFuben(TianTiService.Instance.GameType, (long)gameId, 2, out toServerId))
                    {
                        joinRole.ToServerID = (joinRole2.ToServerID = toServerId);
                        joinRole.CurrGameID = (joinRole2.CurrGameID = gameId);
                        joinRole.WaitReqEnter = (joinRole2.WaitReqEnter = true);
                        joinRole.WaitKuaFuLogin = (joinRole2.WaitKuaFuLogin = false);
                        ZhengBaNtfEnterData data = new ZhengBaNtfEnterData();
                        data.RoleId1 = joinRole.RoleID;
                        data.RoleId2 = joinRole2.RoleID;
                        data.ToServerId = toServerId;
                        data.GameId = gameId;
                        data.Day = this.SyncData.RealActDay;
                        data.Loop = this.CurrLoopIndex;
                        this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaNtfEnter, new object[]
                        {
                            data
                        }));
                        ZhengBaPkLogData log = new ZhengBaPkLogData();
                        log.Month = this.SyncData.Month;
                        log.Day = this.SyncData.RealActDay;
                        log.RoleID1 = joinRole.RoleID;
                        log.ZoneID1 = joinRole.ZoneId;
                        log.RoleName1 = joinRole.RoleName;
                        log.RoleID2 = joinRole2.RoleID;
                        log.ZoneID2 = joinRole2.ZoneId;
                        log.RoleName2 = joinRole2.RoleName;
                        log.StartTime = now;
                        this.ThisLoopPkLogs[gameId] = log;
                    }
                    else
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("众神争霸第{0}天第{1}轮分配游戏服务器失败，role1={2},role2={3}", new object[]
                        {
                            this.SyncData.RealActDay,
                            this.CurrLoopIndex,
                            joinRole.RoleID,
                            joinRole2.RoleID
                        }), null, true);
                    }
                }
                while (currIdx < this.TodayJoinRoleDatas.Count)
                {
                    ZhengBaManagerK.JoinRolePkData joinRole3 = this.TodayJoinRoleDatas[currIdx++];
                    joinRole3.ToServerID = 0;
                    joinRole3.CurrGameID = 0;
                    joinRole3.WaitReqEnter = false;
                    joinRole3.WaitKuaFuLogin = false;
                }
                this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.NotifyEnter, now);
            }
        }

        // Token: 0x060005C2 RID: 1474 RVA: 0x0004E2C4 File Offset: 0x0004C4C4
        private void MS_NotifyEnter_Update(DateTime now)
        {
            ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
            if (this.StateMachine.ContinueTicks(now) >= (long)matchConfig.WaitSeconds * 10000000L)
            {
                int i = 0;
                int currIdx = 0;
                while (i < this.TodayJoinRoleDatas.Count / 2)
                {
                    ZhengBaManagerK.JoinRolePkData joinRole = this.TodayJoinRoleDatas[currIdx++];
                    if (joinRole.WaitReqEnter)
                    {
                        joinRole.WaitReqEnter = false;
                        joinRole.WaitKuaFuLogin = false;
                        ZhengBaPkLogData log = null;
                        if (this.ThisLoopPkLogs.TryGetValue(joinRole.CurrGameID, out log))
                        {
                            log.IsMirror1 = true;
                            this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaMirrorFight, new object[]
                            {
                                new ZhengBaMirrorFightData
                                {
                                    GameId = joinRole.CurrGameID,
                                    RoleId = joinRole.RoleID,
                                    ToServerId = joinRole.ToServerID
                                }
                            }));
                        }
                    }
                    ZhengBaManagerK.JoinRolePkData joinRole2 = this.TodayJoinRoleDatas[currIdx++];
                    if (joinRole2.WaitReqEnter)
                    {
                        joinRole2.WaitReqEnter = false;
                        joinRole2.WaitKuaFuLogin = false;
                        ZhengBaPkLogData log = null;
                        if (this.ThisLoopPkLogs.TryGetValue(joinRole2.CurrGameID, out log))
                        {
                            log.IsMirror2 = true;
                            this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaMirrorFight, new object[]
                            {
                                new ZhengBaMirrorFightData
                                {
                                    GameId = joinRole2.CurrGameID,
                                    RoleId = joinRole2.RoleID,
                                    ToServerId = joinRole2.ToServerID
                                }
                            }));
                        }
                    }
                    i++;
                }
                this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.PkLoopStart, now);
            }
        }

        // Token: 0x060005C3 RID: 1475 RVA: 0x0004E4DC File Offset: 0x0004C6DC
        private void MS_PkLoopStart_Update(DateTime now)
        {
            ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
            if (this.StateMachine.ContinueTicks(now) >= (long)(matchConfig.FightSeconds + matchConfig.ClearSeconds) * 10000000L)
            {
                this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.PkLoopEnd, now);
            }
        }

        // Token: 0x060005C4 RID: 1476 RVA: 0x0004E568 File Offset: 0x0004C768
        private void MS_PkLoopEnd_Update(DateTime now)
        {
            ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
            if (this.StateMachine.ContinueTicks(now) >= (long)matchConfig.IntervalSeconds * 10000000L)
            {
                foreach (KeyValuePair<int, ZhengBaPkLogData> kvp in this.ThisLoopPkLogs)
                {
                    kvp.Value.PkResult = 0;
                    kvp.Value.UpGrade = false;
                    kvp.Value.EndTime = now;
                    this.Persistence.SavePkLog(kvp.Value);
                    this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaPkLog, new object[]
                    {
                        kvp.Value
                    }));
                }
                this.ThisLoopPkLogs.Clear();
                foreach (ZhengBaManagerK.JoinRolePkData role in this.TodayJoinRoleDatas)
                {
                    if (role.CurrGameID > 0 || role.ToServerID > 0)
                    {
                        ClientAgentManager.Instance().RemoveKfFuben(TianTiService.Instance.GameType, role.ToServerID, (long)role.CurrGameID);
                    }
                    role.ToServerID = 0;
                    role.CurrGameID = 0;
                }
                if (now.TimeOfDay.Ticks >= matchConfig.DayEndTick)
                {
                    this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.TodayPkEnd, now);
                }
                else
                {
                    this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.InitPkLoop, now);
                }
            }
        }

        // Token: 0x060005C5 RID: 1477 RVA: 0x0004E740 File Offset: 0x0004C940
        private void MS_TodayPkEnd_Enter(DateTime now)
        {
            this.SyncData.TodayIsPking = false;
            this.FixSyncData(now);
            this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.Idle, now);
            this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaButtetinJoin, new object[]
            {
                new ZhengBaBulletinJoinData
                {
                    NtfType = ZhengBaBulletinJoinData.ENtfType.DayLoopEnd,
                    Args1 = this.SyncData.RealActDay
                }
            }));
        }

        // Token: 0x060005C6 RID: 1478 RVA: 0x0004E7B0 File Offset: 0x0004C9B0
        public void InitConfig()
        {
            if (!this._Config.Load(KuaFuServerManager.GetResourcePath("Config\\Match.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\Sustain.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\MatchBirthPoint.xml", KuaFuServerManager.ResourcePathTypes.GameRes)))
            {
                LogManager.WriteLog(LogTypes.Error, "ZhengBaManagerK.InitConfig failed!", null, true);
            }
        }

        // Token: 0x060005C7 RID: 1479 RVA: 0x0004E81C File Offset: 0x0004CA1C
        public void ReloadSyncData(DateTime now)
        {
            int selectRoleIfNewCreate = 100;
            ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1);
            if (matchConfig.MinRank > 0)
            {
                selectRoleIfNewCreate = matchConfig.MinRank;
            }
            long dayBeginTicks = matchConfig.DayBeginTick;
            ZhengBaSyncData syncData = this.Persistence.LoadZhengBaSyncData(now, selectRoleIfNewCreate, dayBeginTicks);
            lock (this.Mutex)
            {
                this.SyncData = syncData;
                this.FixSyncData(now);
            }
        }

        // Token: 0x060005C8 RID: 1480 RVA: 0x0004EAA4 File Offset: 0x0004CCA4
        private bool FixSyncData_State(DateTime now)
        {
            bool bForceModify = false;
            int nowDay = now.Day - ZhengBaConsts.StartMonthDay + 1;
            lock (this.Mutex)
            {
                int rankOfDay;
                for (rankOfDay = 7; rankOfDay >= 1; rankOfDay--)
                {
                    EZhengBaGrade willUpGrade = ZhengBaUtils.GetDayUpGrade(rankOfDay);
                    ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == rankOfDay);
                    List<ZhengBaRoleInfoData> roleList = this.SyncData.RoleList.FindAll((ZhengBaRoleInfoData _r) => _r.Grade == (int)willUpGrade);
                    if (roleList.Count > 0)
                    {
                        int needUpNum = matchConfig.MaxUpGradeNum - roleList.Count;
                        if (needUpNum > 0)
                        {
                            List<ZhengBaRoleInfoData> upGradeList = new List<ZhengBaRoleInfoData>();
                            if (rankOfDay <= 3)
                            {
                                List<ZhengBaRoleInfoData> luckList = this.SyncData.RoleList.FindAll((ZhengBaRoleInfoData _r) => _r.Grade > (int)willUpGrade);
                                luckList.Sort(delegate (ZhengBaRoleInfoData _l, ZhengBaRoleInfoData _r)
                                {
                                    int result;
                                    if (_l.Grade < _r.Grade)
                                    {
                                        result = -1;
                                    }
                                    else if (_l.Grade > _r.Grade)
                                    {
                                        result = 1;
                                    }
                                    else
                                    {
                                        result = _l.DuanWeiRank - _r.DuanWeiRank;
                                    }
                                    return result;
                                });
                                foreach (ZhengBaRoleInfoData luckRole in luckList.GetRange(0, Math.Min(needUpNum, luckList.Count)))
                                {
                                    upGradeList.Add(luckRole);
                                    LogManager.WriteLog(LogTypes.Error, string.Format("晋级补位 [s{0}.{1}] {2}->{3}", new object[]
                                    {
                                        luckRole.ZoneId,
                                        luckRole.RoleId,
                                        luckRole.Grade,
                                        (int)willUpGrade
                                    }), null, true);
                                    luckRole.Grade = (int)willUpGrade;
                                    bForceModify = true;
                                }
                            }
                            else
                            {
                                using (List<RangeKey>.Enumerator enumerator2 = ZhengBaUtils.GetDayPkGroupRange(rankOfDay).GetEnumerator())
                                {
                                    while (enumerator2.MoveNext())
                                    {
                                        RangeKey range = enumerator2.Current;
                                        List<ZhengBaRoleInfoData> groupRoleList = this.SyncData.RoleList.FindAll((ZhengBaRoleInfoData _r) => _r.Group >= range.Left && _r.Group <= range.Right);
                                        if (!groupRoleList.Exists((ZhengBaRoleInfoData _r) => _r.Grade <= (int)ZhengBaUtils.GetDayUpGrade(rankOfDay)))
                                        {
                                            groupRoleList.RemoveAll((ZhengBaRoleInfoData _r) => _r.Grade != (int)ZhengBaUtils.GetDayUpGrade(rankOfDay - 1));
                                            if (groupRoleList.Count > 0)
                                            {
                                                groupRoleList.Sort((ZhengBaRoleInfoData _l, ZhengBaRoleInfoData _r) => _l.DuanWeiRank - _r.DuanWeiRank);
                                                ZhengBaRoleInfoData selectRole = groupRoleList[0];
                                                LogManager.WriteLog(LogTypes.Error, string.Format("晋级补位 [s{0}.{1}] {2}->{3}", new object[]
                                                {
                                                    selectRole.ZoneId,
                                                    selectRole.RoleId,
                                                    selectRole.Grade,
                                                    (int)willUpGrade
                                                }), null, true);
                                                selectRole.Grade = (int)ZhengBaUtils.GetDayUpGrade(rankOfDay);
                                                bForceModify = true;
                                                upGradeList.Add(selectRole);
                                            }
                                        }
                                    }
                                }
                            }
                            foreach (ZhengBaRoleInfoData luckRole in upGradeList)
                            {
                                this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaButtetinJoin, new object[]
                                {
                                    new ZhengBaBulletinJoinData
                                    {
                                        NtfType = ZhengBaBulletinJoinData.ENtfType.MailUpgradeRole,
                                        Args1 = luckRole.RoleId
                                    }
                                }));
                            }
                        }
                        break;
                    }
                }
                this.SyncData.RankResultOfDay = rankOfDay;
                this.SyncData.RealActDay = rankOfDay;
                foreach (ZhengBaRoleInfoData role in this.SyncData.RoleList)
                {
                    if (rankOfDay <= 0)
                    {
                        if (role.Grade != 100 || role.State != 0 || role.Group != 0)
                        {
                            role.Grade = 100;
                            role.State = 0;
                            role.Group = 0;
                            bForceModify = true;
                        }
                    }
                    else
                    {
                        EZhengBaGrade upGrade = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == rankOfDay).WillUpGrade;
                        if (role.Grade <= (int)upGrade && role.State != 1)
                        {
                            role.State = 1;
                            bForceModify = true;
                        }
                        if (role.Grade > (int)upGrade && role.State != 2)
                        {
                            role.State = 2;
                            bForceModify = true;
                        }
                    }
                    if (role.Grade == 1 && this.SyncData.LastKingModTime != this.SyncData.Month)
                    {
                        this.SyncData.LastKingData = role;
                        this.SyncData.LastKingModTime = this.SyncData.Month;
                    }
                }
                if (nowDay > 0 && this.SyncData.RealActDay < nowDay)
                {
                    if (this.SyncData.RealActDay < 7)
                    {
                        ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay + 1);
                        if (now.TimeOfDay.Ticks < matchConfig.DayBeginTick)
                        {
                            this.SyncData.RealActDay++;
                        }
                    }
                    else
                    {
                        this.SyncData.RealActDay = 8;
                    }
                }
            }
            return bForceModify;
        }

        // Token: 0x060005C9 RID: 1481 RVA: 0x0004F264 File Offset: 0x0004D464
        private bool FixSyncData_Group(DateTime now)
        {
            bool bForceModify = false;
            lock (this.Mutex)
            {
                if (this.SyncData.RealActDay < 3)
                {
                    return false;
                }
                List<int> unUsedGroupList = Enumerable.Range(1, 16).ToList<int>();
                List<ZhengBaRoleInfoData> unGroupRoleList = new List<ZhengBaRoleInfoData>();
                foreach (ZhengBaRoleInfoData role in this.SyncData.RoleList)
                {
                    if (role.Grade <= 16)
                    {
                        if (role.Group >= 1 && role.Group <= 16)
                        {
                            unUsedGroupList.Remove(role.Group);
                        }
                        else
                        {
                            role.Group = 0;
                            unGroupRoleList.Add(role);
                        }
                    }
                }
                if (unGroupRoleList.Count <= unUsedGroupList.Count && unGroupRoleList.Count > 0)
                {
                    unGroupRoleList.Sort((ZhengBaRoleInfoData _l, ZhengBaRoleInfoData _r) => _l.ZoneId * _l.DuanWeiRank * now.TimeOfDay.Minutes % _r.RoleId - _r.ZoneId * _r.DuanWeiRank * now.TimeOfDay.Minutes % _l.RoleId);
                    foreach (ZhengBaRoleInfoData role in unGroupRoleList)
                    {
                        role.Group = unUsedGroupList.Last<int>();
                        unUsedGroupList.RemoveAt(unUsedGroupList.Count<int>() - 1);
                        LogManager.WriteLog(LogTypes.Error, string.Format("晋级补分组 [s{0}.{1}] Group:{2}", role.ZoneId, role.RoleId, role.Group), null, true);
                        bForceModify = true;
                    }
                }
                else if (unGroupRoleList.Count > 0)
                {
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("晋级补分组发生异常，待补人数={0}，可用分组数={1}", unGroupRoleList.Count, unUsedGroupList.Count), null, false);
                }
            }
            return bForceModify;
        }

        // Token: 0x060005CA RID: 1482 RVA: 0x0004F4DC File Offset: 0x0004D6DC
        private void FixSyncData(DateTime now)
        {
            lock (this.Mutex)
            {
                bool bModify = false;
                bModify |= this.FixSyncData_State(now);
                bModify |= this.FixSyncData_Group(now);
                if (bModify)
                {
                    foreach (ZhengBaRoleInfoData role in this.SyncData.RoleList)
                    {
                        this.Persistence.UpdateRole(this.SyncData.Month, role.RoleId, role.Grade, role.State, role.Group);
                    }
                }
                this.SyncData.RoleModTime = now;
                this.SyncData.SupportModTime = now;
            }
        }

        // Token: 0x060005CB RID: 1483 RVA: 0x0004F5D8 File Offset: 0x0004D7D8
        public ZhengBaSyncData SyncZhengBaData(ZhengBaSyncData lastSyncData)
        {
            ZhengBaSyncData result = new ZhengBaSyncData();
            lock (this.Mutex)
            {
                result.Month = this.SyncData.Month;
                result.RealActDay = this.SyncData.RealActDay;
                result.RoleModTime = this.SyncData.RoleModTime;
                result.SupportModTime = this.SyncData.SupportModTime;
                result.LastKingModTime = this.SyncData.LastKingModTime;
                result.TodayIsPking = this.SyncData.TodayIsPking;
                result.IsThisMonthInActivity = this.SyncData.IsThisMonthInActivity;
                result.RankResultOfDay = this.SyncData.RankResultOfDay;
                result.CenterTime = TimeUtil.NowDateTime();
                if (result.RoleModTime > lastSyncData.RoleModTime && this.SyncData.RoleList != null)
                {
                    result.RoleList = new List<ZhengBaRoleInfoData>(this.SyncData.RoleList);
                }
                if (result.SupportModTime > lastSyncData.SupportModTime && this.SyncData.SupportList != null)
                {
                    result.SupportList = new List<ZhengBaSupportAnalysisData>(this.SyncData.SupportList);
                }
                if (result.LastKingModTime > lastSyncData.LastKingModTime)
                {
                    result.LastKingData = this.SyncData.LastKingData;
                }
            }
            return result;
        }

        // Token: 0x060005CC RID: 1484 RVA: 0x0004F7C0 File Offset: 0x0004D9C0
        public int ZhengBaSupport(ZhengBaSupportLogData data)
        {
            int result;
            if (data == null || !this.Persistence.SaveSupportLog(data))
            {
                result = -15;
            }
            else
            {
                lock (this.Mutex)
                {
                    ZhengBaSupportAnalysisData support;
                    if ((support = this.SyncData.SupportList.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == data.ToUnionGroup && _s.Group == data.ToGroup)) == null)
                    {
                        support = new ZhengBaSupportAnalysisData
                        {
                            UnionGroup = data.ToUnionGroup,
                            Group = data.ToGroup,
                            RankOfDay = data.RankOfDay
                        };
                        this.SyncData.SupportList.Add(support);
                    }
                    if (data.SupportType == 1)
                    {
                        support.TotalSupport++;
                    }
                    else if (data.SupportType == 2)
                    {
                        support.TotalOppose++;
                    }
                    else if (data.SupportType == 3)
                    {
                        support.TotalYaZhu++;
                    }
                    this.SyncData.SupportModTime = TimeUtil.NowDateTime();
                    this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaSupport, new object[]
                    {
                        data
                    }));
                }
                result = 1;
            }
            return result;
        }

        // Token: 0x060005CD RID: 1485 RVA: 0x0004F9D0 File Offset: 0x0004DBD0
        public int ZhengBaRequestEnter(int roleId, int gameId, EZhengBaEnterType enter)
        {
            lock (this.Mutex)
            {
                if (this.StateMachine.GetCurrState() != ZhengBaStateMachine.StateType.NotifyEnter)
                {
                    return -2001;
                }
                ZhengBaManagerK.JoinRolePkData roleData = this.TodayJoinRoleDatas.Find((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == roleId && _r.CurrGameID == gameId);
                ZhengBaPkLogData logData = null;
                this.ThisLoopPkLogs.TryGetValue(gameId, out logData);
                if (roleData == null || logData == null)
                {
                    return -12;
                }
                if (!roleData.WaitReqEnter)
                {
                    return -12;
                }
                roleData.WaitReqEnter = false;
                roleData.WaitKuaFuLogin = true;
                if (enter == EZhengBaEnterType.Mirror)
                {
                    if (logData.RoleID1 == roleId)
                    {
                        logData.IsMirror1 = true;
                    }
                    else if (logData.RoleID2 == roleId)
                    {
                        logData.IsMirror2 = true;
                    }
                    this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaMirrorFight, new object[]
                    {
                        new ZhengBaMirrorFightData
                        {
                            RoleId = roleId,
                            GameId = gameId,
                            ToServerId = roleData.ToServerID
                        }
                    }));
                }
            }
            return 0;
        }

        // Token: 0x060005CE RID: 1486 RVA: 0x0004FBC0 File Offset: 0x0004DDC0
        public int ZhengBaKuaFuLogin(int roleid, int gameId)
        {
            lock (this.Mutex)
            {
                ZhengBaManagerK.JoinRolePkData roleData = this.TodayJoinRoleDatas.Find((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == roleid && _r.CurrGameID == gameId);
                ZhengBaPkLogData logData = null;
                this.ThisLoopPkLogs.TryGetValue(gameId, out logData);
                if (roleData == null || logData == null)
                {
                    return -12;
                }
                if (!roleData.WaitKuaFuLogin)
                {
                    return -12;
                }
            }
            return 0;
        }

        // Token: 0x060005CF RID: 1487 RVA: 0x0004FD84 File Offset: 0x0004DF84
        public List<ZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner1, int FirstLeaveRoleId)
        {
            List<ZhengBaNtfPkResultData> result;
            lock (this.Mutex)
            {
                ZhengBaPkLogData log = null;
                if (!this.ThisLoopPkLogs.TryGetValue(gameId, out log))
                {
                    result = null;
                }
                else
                {
                    if (FirstLeaveRoleId == log.RoleID1)
                    {
                        winner1 = log.RoleID2;
                    }
                    else if (FirstLeaveRoleId == log.RoleID2)
                    {
                        winner1 = log.RoleID1;
                    }
                    if (winner1 != log.RoleID1 && winner1 != log.RoleID2)
                    {
                        result = null;
                    }
                    else
                    {
                        ZhengBaManagerK.JoinRolePkData joinRole = this.TodayJoinRoleDatas.Find((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == log.RoleID1 && _r.CurrGameID == gameId);
                        ZhengBaManagerK.JoinRolePkData joinRole2 = this.TodayJoinRoleDatas.Find((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == log.RoleID2 && _r.CurrGameID == gameId);
                        if (joinRole == null || joinRole2 == null)
                        {
                            result = null;
                        }
                        else
                        {
                            ZhengBaMatchConfig matchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
                            ZhengBaNtfPkResultData ntf = new ZhengBaNtfPkResultData
                            {
                                RoleID = joinRole.RoleID
                            };
                            ZhengBaNtfPkResultData ntf2 = new ZhengBaNtfPkResultData
                            {
                                RoleID = joinRole2.RoleID
                            };
                            ZhengBaManagerK.JoinRolePkData winJoinRole = null;
                            ZhengBaNtfPkResultData winNtf = null;
                            if (winner1 > 0 && winner1 == joinRole.RoleID)
                            {
                                winJoinRole = joinRole;
                                winNtf = ntf;
                            }
                            else if (winner1 > 0 && winner1 == joinRole2.RoleID)
                            {
                                winJoinRole = joinRole2;
                                winNtf = ntf2;
                            }
                            if (winJoinRole != null && winNtf != null)
                            {
                                winNtf.IsWin = true;
                                winJoinRole.WinTimes++;
                                if (winJoinRole.WinTimes >= matchConfig.NeedWinTimes && this.HadUpGradeRoleNum < matchConfig.MaxUpGradeNum)
                                {
                                    int calcGroup = (this.RandomGroup.Count > 0) ? this.RandomGroup.Last<int>() : 0;
                                    bool bSaveUpdate = false;
                                    ZhengBaRoleInfoData roleData = this.SyncData.RoleList.Find((ZhengBaRoleInfoData _r) => _r.RoleId == winJoinRole.RoleID);
                                    if (roleData != null)
                                    {
                                        int newGrade = (int)matchConfig.WillUpGrade;
                                        int newState = 1;
                                        int newGroup = (calcGroup != 0) ? calcGroup : roleData.Group;
                                        if (this.Persistence.UpdateRole(this.SyncData.Month, roleData.RoleId, newGrade, newState, newGroup))
                                        {
                                            roleData.Grade = newGrade;
                                            roleData.State = newState;
                                            roleData.Group = newGroup;
                                            this.SyncData.RoleModTime = TimeUtil.NowDateTime();
                                            bSaveUpdate = true;
                                            if (newGrade != 1)
                                            {
                                                this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaButtetinJoin, new object[]
                                                {
                                                    new ZhengBaBulletinJoinData
                                                    {
                                                        NtfType = ZhengBaBulletinJoinData.ENtfType.MailUpgradeRole,
                                                        Args1 = roleData.RoleId
                                                    }
                                                }));
                                            }
                                        }
                                    }
                                    if (bSaveUpdate)
                                    {
                                        winNtf.RandGroup = calcGroup;
                                        winNtf.IsUpGrade = true;
                                        winNtf.NewGrade = roleData.Grade;
                                        log.UpGrade = true;
                                        this.HadUpGradeRoleNum++;
                                        this.RandomGroup.Remove(calcGroup);
                                        this.TodayJoinRoleDatas.RemoveAll((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == winJoinRole.RoleID);
                                    }
                                }
                            }
                            log.EndTime = TimeUtil.NowDateTime();
                            if (winner1 > 0 && winner1 == log.RoleID1)
                            {
                                log.PkResult = 1;
                            }
                            else if (winner1 > 0 && winner1 == log.RoleID2)
                            {
                                log.PkResult = 2;
                            }
                            else
                            {
                                log.PkResult = 0;
                            }
                            ntf.StillNeedWin = Math.Max(0, matchConfig.NeedWinTimes - joinRole.WinTimes);
                            ntf.LeftUpGradeNum = matchConfig.MaxUpGradeNum - this.HadUpGradeRoleNum;
                            ntf2.StillNeedWin = Math.Max(0, matchConfig.NeedWinTimes - joinRole2.WinTimes);
                            ntf2.LeftUpGradeNum = matchConfig.MaxUpGradeNum - this.HadUpGradeRoleNum;
                            this.Persistence.SavePkLog(log);
                            this.ThisLoopPkLogs.Remove(gameId);
                            this.AsyncEvQ.Enqueue(new AsyncDataItem(KuaFuEventTypes.ZhengBaPkLog, new object[]
                            {
                                log
                            }));
                            result = new List<ZhengBaNtfPkResultData>
                            {
                                ntf,
                                ntf2
                            };
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x04000313 RID: 787
        private static ZhengBaManagerK _instance = new ZhengBaManagerK();

        // Token: 0x04000314 RID: 788
        private ZhengBaSyncData SyncData = new ZhengBaSyncData
        {
            Month = ZhengBaUtils.MakeMonth(TimeUtil.NowDateTime())
        };

        // Token: 0x04000315 RID: 789
        private DateTime lastUpdateTime = TimeUtil.NowDateTime();

        // Token: 0x04000316 RID: 790
        private Queue<AsyncDataItem> AsyncEvQ = new Queue<AsyncDataItem>();

        // Token: 0x04000317 RID: 791
        private int HadUpGradeRoleNum = 0;

        // Token: 0x04000318 RID: 792
        private List<int> RandomGroup = new List<int>();

        // Token: 0x04000319 RID: 793
        private ZhengBaConfig _Config = new ZhengBaConfig();

        // Token: 0x0400031A RID: 794
        private List<ZhengBaManagerK.JoinRolePkData> TodayJoinRoleDatas = new List<ZhengBaManagerK.JoinRolePkData>();

        // Token: 0x0400031B RID: 795
        private Dictionary<int, ZhengBaPkLogData> ThisLoopPkLogs = new Dictionary<int, ZhengBaPkLogData>();

        // Token: 0x0400031C RID: 796
        private int CurrLoopIndex = 0;

        // Token: 0x0400031D RID: 797
        private ZhengBaStateMachine StateMachine = new ZhengBaStateMachine();

        // Token: 0x0400031E RID: 798
        private object Mutex = new object();

        // Token: 0x0400031F RID: 799
        private ZhengBaPersistence Persistence = ZhengBaPersistence.Instance;

        // Token: 0x02000075 RID: 117
        private class JoinRolePkData
        {
            // Token: 0x04000323 RID: 803
            public int RoleID;

            // Token: 0x04000324 RID: 804
            public int ZoneId;

            // Token: 0x04000325 RID: 805
            public string RoleName;

            // Token: 0x04000326 RID: 806
            public int Group;

            // Token: 0x04000327 RID: 807
            public int ToServerID;

            // Token: 0x04000328 RID: 808
            public int CurrGameID;

            // Token: 0x04000329 RID: 809
            public bool WaitReqEnter;

            // Token: 0x0400032A RID: 810
            public bool WaitKuaFuLogin;

            // Token: 0x0400032B RID: 811
            public int WinTimes;
        }
    }
}
