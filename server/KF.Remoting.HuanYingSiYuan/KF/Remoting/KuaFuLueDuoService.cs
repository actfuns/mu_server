using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;

namespace KF.Remoting
{
    // Token: 0x0200006F RID: 111
    public class KuaFuLueDuoService
    {
        // Token: 0x06000563 RID: 1379 RVA: 0x00047968 File Offset: 0x00045B68
        public static KuaFuLueDuoService Instance()
        {
            return KuaFuLueDuoService._instance;
        }

        // Token: 0x1700002E RID: 46
        // (get) Token: 0x06000564 RID: 1380 RVA: 0x00047980 File Offset: 0x00045B80
        // (set) Token: 0x06000565 RID: 1381 RVA: 0x0004799D File Offset: 0x00045B9D
        private Dictionary<int, KuaFuLueDuoServerInfo> ServerInfoDict
        {
            get
            {
                return this.Persistence.ServerInfoDict;
            }
            set
            {
                this.Persistence.ServerInfoDict = value;
            }
        }

        // Token: 0x1700002F RID: 47
        // (get) Token: 0x06000566 RID: 1382 RVA: 0x000479AC File Offset: 0x00045BAC
        // (set) Token: 0x06000567 RID: 1383 RVA: 0x000479C9 File Offset: 0x00045BC9
        private Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict
        {
            get
            {
                return this.Persistence.KuaFuLueDuoBHDataDict;
            }
            set
            {
                this.Persistence.KuaFuLueDuoBHDataDict = value;
            }
        }

        // Token: 0x17000030 RID: 48
        // (get) Token: 0x06000568 RID: 1384 RVA: 0x000479D8 File Offset: 0x00045BD8
        // (set) Token: 0x06000569 RID: 1385 RVA: 0x000479F5 File Offset: 0x00045BF5
        private Dictionary<int, KuaFuLueDuoRankListData> KuaFuLueDuoRankInfoDict
        {
            get
            {
                return this.Persistence.KuaFuLueDuoRankInfoDict;
            }
            set
            {
                this.Persistence.KuaFuLueDuoRankInfoDict = value;
            }
        }

        // Token: 0x17000031 RID: 49
        // (get) Token: 0x0600056A RID: 1386 RVA: 0x00047A04 File Offset: 0x00045C04
        // (set) Token: 0x0600056B RID: 1387 RVA: 0x00047A21 File Offset: 0x00045C21
        private int SeasonCount
        {
            get
            {
                return this.Persistence.SeasonCount;
            }
            set
            {
                this.Persistence.SeasonCount = value;
            }
        }

        // Token: 0x0600056C RID: 1388 RVA: 0x00047A30 File Offset: 0x00045C30
        public bool InitConfig()
        {
            bool result;
            if (this.StateMachine.GetCurrState() >= KuaFuLueDuoStateMachine.StateType.SignUp)
            {
                LogManager.WriteLog(LogTypes.Error, "因为跨服掠夺活动正在进行,本活动配置未重新加载!", null, true);
                result = true;
            }
            else
            {
                this.Initialized = false;
                this.RuntimeData.CrusadeSeason = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("CrusadeSeason", 13);
                this.RuntimeData.CrusadeAttackerNum = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("CrusadeAttackerNum", 3);
                this.RuntimeData.CrusadeOre = KuaFuServerManager.systemParamsList.GetParamValueIntArrayByName("CrusadeOre");
                this.RuntimeData.CrusadeMinApply = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("CrusadeMinApply", 10000);
                this.RuntimeData.CrusadeApplyCD = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("CrusadeApplyCD", 300);
                this.RuntimeData.KuaFuLueDuoWaitRankSecs = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("KuaFuLueDuoWaitRankSecs", 180);
                if (!this.RuntimeData.Load(KuaFuServerManager.GetResourcePath("Config\\CrusadeWar.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\CrusadeGroup.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.platformType))
                {
                    LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.InitConfig failed!", null, true);
                    throw new ConfigurationErrorsException("KuaFuLueDuoService.InitConfig failed!");
                }
                if (!this.LoadDatabase(TimeUtil.NowDateTime(), true))
                {
                    LogManager.WriteLog(LogTypes.Error, "从数据库加载跨服掠夺数据失败", null, true);
                    result = false;
                }
                else
                {
                    this.Initialized = true;
                    result = true;
                }
            }
            return result;
        }

        // Token: 0x0600056D RID: 1389 RVA: 0x00047B98 File Offset: 0x00045D98
        private int ComputeCurrentSeasonID(DateTime now)
        {
            return TimeUtil.GetOffsetDay2(TimeUtil.NowDateTime());
        }

        // Token: 0x0600056E RID: 1390 RVA: 0x00047BB4 File Offset: 0x00045DB4
        public bool LoadDatabase(DateTime now, bool hist)
        {
            try
            {
                lock (this.Mutex)
                {
                    int max = this.RuntimeData.ZoneGroupConfigDict.Keys.Max();
                    this.Persistence.ZoneID2GroupIDs = new int[max + 1];
                    this.Persistence.ZoneID2ServerIDs = new int[max + 1];
                    foreach (KeyValuePair<int, KuaFuLueDuoGroupItem> kv in this.RuntimeData.ZoneGroupConfigDict)
                    {
                        if (now >= kv.Value.StartTime)
                        {
                            this.Persistence.ZoneID2GroupIDs[kv.Key] = kv.Value.GroupID;
                            this.Persistence.ZoneID2ServerIDs[kv.Key] = kv.Value.ServerNumber;
                        }
                    }
                    this.SeasonCount = this.Persistence.LoadSeasonCount();
                    this.SeasonID = this.ComputeCurrentSeasonID(now);
                    int count = this.SeasonCount % this.RuntimeData.CrusadeSeason;
                    if (count == 0)
                    {
                        count = this.RuntimeData.CrusadeSeason;
                    }
                    int[] ids = this.Persistence.GetHistSeasonIDs(count);
                    this.LastSeasonID = ids[0];
                    this.MinSeasonID = ids[2];
                    if (count == this.RuntimeData.CrusadeSeason && !hist)
                    {
                        this.MinSeasonID = this.SeasonID;
                        this.LastSeasonID = this.SeasonID;
                        this.Persistence.ClearLastSeasonData();
                    }
                    this.SyncDataDict.Clear();
                    this.ServerInfoDict.Clear();
                    this.KuaFuLueDuoBHDataDict.Clear();
                    this.Persistence.JingJiaDict.Clear();
                    this.KuaFuLueDuoRankInfoDict.Clear();
                    foreach (KuaFuLueDuoGroupItem data in this.RuntimeData.GroupConfigDict.Values)
                    {
                        if (now >= data.StartTime)
                        {
                            KuaFuLueDuoServerInfo serverInfo = new KuaFuLueDuoServerInfo
                            {
                                ServerId = data.ServerNumber
                            };
                            serverInfo.ZhengFuList = new List<int>();
                            serverInfo.ShiChouList = new List<int>();
                            serverInfo.MingXingList = new List<KuaFuLueDuoRankInfo>();
                            serverInfo.MingXingZhanMengList = "";
                            this.ServerInfoDict[data.ServerNumber] = serverInfo;
                        }
                    }
                    if (!this.Persistence.LoadDatabase(this.SeasonID, this.LastSeasonID, this.MinSeasonID))
                    {
                        LogManager.WriteLog(LogTypes.Error, "加载跨服掠夺数据失败", null, true);
                        return false;
                    }
                    this.InitGroupServerList();
                    this.InitFuBenManagerData(now);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.LoadDatabase failed!", ex, true);
            }
            return false;
        }

        // Token: 0x0600056F RID: 1391 RVA: 0x00047F40 File Offset: 0x00046140
        public void OnStopServer()
        {
            this.Persistence.DelayWriteDataProc();
        }

        // Token: 0x06000570 RID: 1392 RVA: 0x00047F50 File Offset: 0x00046150
        public void Update(DateTime now)
        {
            try
            {
                if (this.Initialized)
                {
                    if ((now - this.LastUpdateTime).TotalMilliseconds >= 2000.0)
                    {
                        if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot6))
                        {
                            this.UpdateFrameCount += 1U;
                            this.StateMachine.Tick(now, 0);
                            this.Persistence.DelayWriteDataProc();
                            this.LastUpdateTime = now;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.Update failed!", ex, true);
            }
        }

        // Token: 0x06000571 RID: 1393 RVA: 0x00047FFC File Offset: 0x000461FC
        private void InitFuBenManagerData(DateTime now)
        {
            if (this.StateMachine.GetCurrState() == KuaFuLueDuoStateMachine.StateType.None)
            {
                this.LastUpdateTime = now;
                this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.Init, null, new Action<DateTime, int>(this.MS_Init_Update), null));
                this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.SignUp, null, new Action<DateTime, int>(this.MS_SignUp_Update), null));
                this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.PrepareGame, null, new Action<DateTime, int>(this.MS_PrepareGame_Update), null));
                this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.NotifyEnter, null, new Action<DateTime, int>(this.MS_NotifyEnter_Update), null));
                this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.GameStart, null, new Action<DateTime, int>(this.MS_GameStart_Update), null));
                this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.RankAnalyse, null, new Action<DateTime, int>(this.MS_RankAnalyse_Enter), null));
                this.StateMachine.SetCurrState(KuaFuLueDuoStateMachine.StateType.Init, TimeUtil.NowDateTime(), 0);
                this.StateMachine.Tick(now, 0);
            }
        }

        // Token: 0x06000572 RID: 1394 RVA: 0x0004810C File Offset: 0x0004630C
        private int GetServerIdByZoneId(int zoneId)
        {
            KuaFuLueDuoGroupItem g;
            int result;
            if (this.RuntimeData.ZoneGroupConfigDict.TryGetValue(zoneId, out g))
            {
                result = g.ServerNumber;
            }
            else
            {
                LogManager.WriteLog(LogTypes.Error, "跨服掠夺服务器分组配置表中未包含的区号:" + zoneId, null, true);
                result = zoneId;
            }
            return result;
        }

        // Token: 0x06000573 RID: 1395 RVA: 0x00048198 File Offset: 0x00046398
        private void InitGroupServerList()
        {
            DateTime now = TimeUtil.NowDateTime();
            lock (this.Mutex)
            {
                foreach (KeyValuePair<int, KuaFuLueDuoGroupItem> kv in this.RuntimeData.GroupConfigDict)
                {
                    int group = kv.Value.GroupID;
                    int sid = kv.Value.ServerNumber;
                    if (!(now < kv.Value.StartTime))
                    {
                        KuaFuLueDuoSyncData syncData;
                        if (!this.SyncDataDict.TryGetValue(group, out syncData))
                        {
                            syncData = new KuaFuLueDuoSyncData();
                            this.SyncDataDict[group] = syncData;
                        }
                        syncData.BhZiYuanDict.Clear();
                        syncData.ServerZiYuanDict.Clear();
                        KuaFuLueDuoRankListData rankData;
                        if (this.Persistence.KuaFuLueDuoRankInfoDict.TryGetValue(group, out rankData))
                        {
                            syncData.KuaFuLueDuoRankInfoDict = rankData;
                            TimeUtil.AgeByNow(ref syncData.KuaFuLueDuoRankInfoDict.Age);
                        }
                        KuaFuLueDuoServerInfo serverInfo;
                        if (!syncData.ServerInfoDict.TryGetValue(sid, out serverInfo))
                        {
                            serverInfo = new KuaFuLueDuoServerInfo
                            {
                                ServerId = sid
                            };
                            serverInfo.ZoneIdRangeList = kv.Value.ZoneNumber;
                            syncData.ServerInfoDict[sid] = serverInfo;
                            serverInfo.ZhengFuList = new List<int>();
                            serverInfo.ShiChouList = new List<int>();
                        }
                        KuaFuLueDuoServerInfo serverInfo2;
                        if (this.ServerInfoDict.TryGetValue(sid, out serverInfo2))
                        {
                            serverInfo.MingXingZhanMengList = serverInfo2.MingXingZhanMengList;
                            serverInfo.ZiYuan = serverInfo2.ZiYuan;
                            serverInfo.LastZiYuan = serverInfo2.LastZiYuan;
                            serverInfo.ZhengFuList = serverInfo2.ZhengFuList.Distinct<int>().ToList<int>();
                            serverInfo.ShiChouList = serverInfo2.ShiChouList.Distinct<int>().ToList<int>();
                            for (int idx = serverInfo.ShiChouList.Count - 1; idx >= 0; idx--)
                            {
                                KuaFuLueDuoGroupItem g2;
                                if (!this.RuntimeData.GroupConfigDict.TryGetValue(serverInfo.ShiChouList[idx], out g2) || g2.GroupID != group)
                                {
                                    serverInfo.ShiChouList.RemoveAt(idx);
                                }
                            }
                        }
                    }
                }
                foreach (KuaFuLueDuoSyncData syncData in this.SyncDataDict.Values)
                {
                    KuaFuLueDuoServerInfo[] list = syncData.ServerInfoDict.Values.ToArray<KuaFuLueDuoServerInfo>();
                    Array.Sort<KuaFuLueDuoServerInfo>(list, delegate (KuaFuLueDuoServerInfo x, KuaFuLueDuoServerInfo y)
                    {
                        int ret = y.ZiYuan - x.ZiYuan;
                        int result;
                        if (ret != 0)
                        {
                            result = ret;
                        }
                        else
                        {
                            result = x.ServerId - y.ServerId;
                        }
                        return result;
                    });
                    for (int i = 0; i < list.Length; i++)
                    {
                        KuaFuLueDuoServerInfo data = list[i];
                        KuaFuLueDuoServerJingJiaState jjData;
                        if (!syncData.StateList.TryGetValue(data.ServerId, out jjData))
                        {
                            if (!this.Persistence.JingJiaDict.TryGetValue(data.ServerId, out jjData))
                            {
                                jjData = new KuaFuLueDuoServerJingJiaState
                                {
                                    ServerId = data.ServerId
                                };
                                jjData.JingJiaList = new List<KuaFuLueDuoBangHuiJingJiaData>();
                            }
                            syncData.StateList[data.ServerId] = jjData;
                        }
                        jjData.ZiYuan = data.ZiYuan;
                        jjData.Round = KuaFuLueDuoUtils.GetJingJiaRoundByIndex(i);
                        jjData.State = 0;
                    }
                    syncData.ServerInfoDictAge = TimeUtil.AgeByNow(ref syncData.StateAge);
                }
            }
        }

        // Token: 0x06000574 RID: 1396 RVA: 0x000485C4 File Offset: 0x000467C4
        private void MS_Init_Update(DateTime now, int param)
        {
            this.SeasonID = this.ComputeCurrentSeasonID(now);
            this.GameState = 0;
            if (this.LastDate != now.Date)
            {
                this.LastDate = now.Date;
                if (!this.LoadDatabase(now, true))
                {
                    LogManager.WriteLog(LogTypes.Error, "MS_Init_Update从数据库加载跨服掠夺数据失败", null, true);
                    return;
                }
            }
            TimeSpan ts = TimeUtil.GetTimeOfWeekNow2();
            KuaFuLueDuoConfig matchConfig = this.RuntimeData.GetKuaFuLueDuoConfig(param);
            if (!(ts < matchConfig.ApplyTimePoints[0]))
            {
                KuaFuLueDuoStateMachine.StateType nextGameState;
                if (ts < matchConfig.ApplyTimePoints[matchConfig.ApplyTimePoints.Count - 1])
                {
                    nextGameState = KuaFuLueDuoStateMachine.StateType.SignUp;
                    this.GameState = 1;
                }
                else
                {
                    int secs = (int)(ts - matchConfig.TimePoints[0]).TotalSeconds;
                    if (Consts.TestMode && secs >= 0 && secs < matchConfig.TotalSecs)
                    {
                        this.GameState = 2;
                        nextGameState = KuaFuLueDuoStateMachine.StateType.PrepareGame;
                    }
                    else
                    {
                        if (secs >= 0)
                        {
                            return;
                        }
                        this.GameState = 2;
                        nextGameState = KuaFuLueDuoStateMachine.StateType.PrepareGame;
                    }
                }
                KuaFuLueDuoStateMachine.StateType state = this.StateMachine.GetCurrState();
                if (state != nextGameState)
                {
                    this.StateMachine.SetCurrState(nextGameState, now, param);
                }
                this.StateMachine.Tag1 = 0L;
                this.StateMachine.Tag2 = TimeSpan.MinValue;
                LogManager.WriteLog(LogTypes.Analysis, string.Format("KuaFuLueDuo::MS_Init_Update GameState:{0} To:{1} SeasonID:{2} SeasonIDLast:{3} Round:{4}", new object[]
                {
                    state,
                    this.GameState,
                    this.SeasonID,
                    this.LastSeasonID,
                    this.SignUpRound
                }), null, true);
            }
        }

        // Token: 0x06000575 RID: 1397 RVA: 0x000487B4 File Offset: 0x000469B4
        private void MS_SignUp_Update(DateTime now, int param)
        {
            TimeSpan nowTs = TimeUtil.GetTimeOfWeekNow2();
            KuaFuLueDuoConfig matchConfig = this.RuntimeData.GetKuaFuLueDuoConfig(param);
            int signUpRound = -1;
            for (int i = 0; i < matchConfig.ApplyTimePoints.Count - 1; i++)
            {
                if (nowTs >= matchConfig.ApplyTimePoints[i] && nowTs < matchConfig.ApplyTimePoints[i + 1])
                {
                    signUpRound = i + 1;
                    break;
                }
            }
            if (signUpRound < 0)
            {
                signUpRound = 5;
                KuaFuLueDuoStateMachine.StateType lastState = this.StateMachine.GetCurrState();
                KuaFuLueDuoStateMachine.StateType nextGameState = KuaFuLueDuoStateMachine.StateType.PrepareGame;
                this.StateMachine.SetCurrState(nextGameState, now, param);
                LogManager.WriteLog(LogTypes.Analysis, string.Format("KuaFuLueDuo::MS_SignUp_Update GameState:{0} To:{1} SeasonID:{2} LastSeasonID:{3} Round:{4}", new object[]
                {
                    lastState,
                    nextGameState,
                    this.SeasonID,
                    this.LastSeasonID,
                    this.SignUpRound
                }), null, true);
            }
            if (signUpRound != this.SignUpRound)
            {
                this.SignUpRound = signUpRound;
                this.GameState = 1;
                int lastSignUpRound = this.Persistence.GetSignUpRound();
                this.Persistence.SaveSignUpRound(this.SignUpRound);
                if (this.SignUpRound == 1 && lastSignUpRound != this.SignUpRound)
                {
                    lock (this.Mutex)
                    {
                        this.LoadDatabase(TimeUtil.NowDateTime(), false);
                        foreach (KeyValuePair<int, KuaFuLueDuoGroupItem> kv in this.RuntimeData.GroupConfigDict)
                        {
                            int group = kv.Value.GroupID;
                            int sid = kv.Value.ServerNumber;
                            KuaFuLueDuoSyncData syncData;
                            if (!this.SyncDataDict.TryGetValue(group, out syncData))
                            {
                                syncData = new KuaFuLueDuoSyncData();
                                this.SyncDataDict[group] = syncData;
                            }
                            KuaFuLueDuoServerInfo serverInfo;
                            if (!syncData.ServerInfoDict.TryGetValue(sid, out serverInfo))
                            {
                                serverInfo = new KuaFuLueDuoServerInfo
                                {
                                    ServerId = sid
                                };
                                serverInfo.ZoneIdRangeList = kv.Value.ZoneNumber;
                                syncData.ServerInfoDict[sid] = serverInfo;
                            }
                            int newZiYuan = Math.Min(serverInfo.ZiYuan + this.RuntimeData.CrusadeOre[0], this.RuntimeData.CrusadeOre[1]);
                            if (newZiYuan != serverInfo.ZiYuan)
                            {
                                serverInfo.ZiYuan = newZiYuan;
                                TimeUtil.AgeByNow(ref syncData.ServerInfoDictAge);
                                this.Persistence.SaveKuaFuLueDuoServerData(this.SeasonID, serverInfo);
                                KuaFuLueDuoServerJingJiaState jjData;
                                if (syncData.StateList.TryGetValue(sid, out jjData))
                                {
                                    jjData.ZiYuan = serverInfo.ZiYuan;
                                }
                            }
                        }
                    }
                }
                this.UpdateSignUpRound();
            }
        }

        // Token: 0x06000576 RID: 1398 RVA: 0x00048B18 File Offset: 0x00046D18
        private void UpdateSignUpRound()
        {
            if (this.SignUpRound > 0)
            {
                lock (this.Mutex)
                {
                    foreach (KuaFuLueDuoSyncData syncData in this.SyncDataDict.Values)
                    {
                        syncData.SignUpRound = this.SignUpRound;
                        foreach (KuaFuLueDuoServerJingJiaState jjData in syncData.StateList.Values)
                        {
                            if (jjData.Round < this.SignUpRound)
                            {
                                jjData.State = 2;
                            }
                            else if (jjData.Round == this.SignUpRound)
                            {
                                jjData.State = 1;
                            }
                            else
                            {
                                jjData.State = 0;
                            }
                        }
                        TimeUtil.AgeByNow(ref syncData.StateAge);
                    }
                }
            }
        }

        // Token: 0x06000577 RID: 1399 RVA: 0x00048C68 File Offset: 0x00046E68
        private void MS_PrepareGame_Update(DateTime now, int param)
        {
            TimeSpan nowTs = TimeUtil.GetTimeOfWeekNow2();
            this.GameState = 2;
            KuaFuLueDuoConfig matchConfig = this.RuntimeData.GetKuaFuLueDuoConfig(param);
            KuaFuLueDuoStateMachine.StateType nextGameState = KuaFuLueDuoStateMachine.StateType.PrepareGame;
            for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
            {
                TimeSpan ts = nowTs - matchConfig.TimePoints[i];
                if (ts.TotalSeconds < 0.0)
                {
                    this.StateMachine.Tag2 = matchConfig.TimePoints[i];
                    break;
                }
                if (ts.TotalSeconds <= (double)matchConfig.GameSecs)
                {
                    nextGameState = KuaFuLueDuoStateMachine.StateType.GameStart;
                    break;
                }
                if (i < matchConfig.TimePoints.Count - 1 || nowTs < matchConfig.TimePoints[i + 1])
                {
                    nextGameState = KuaFuLueDuoStateMachine.StateType.GameStart;
                    break;
                }
            }
            if (ClientAgentManager.Instance().IsAnyKfAgentAlive())
            {
                bool change = false;
                foreach (KuaFuLueDuoSyncData syncData in this.SyncDataDict.Values)
                {
                    foreach (KuaFuLueDuoServerJingJiaState sData in syncData.StateList.Values)
                    {
                        KuaFuLueDuoFuBenData fubenData = sData.FuBenData;
                        if (sData.FuBenData == null)
                        {
                            fubenData = new KuaFuLueDuoFuBenData();
                            fubenData.GameId = (long)TianTiPersistence.Instance.GetNextGameId();
                            fubenData.DestServerId = sData.ServerId;
                            fubenData.LeftZiYuan = sData.ZiYuan;
                            if (sData.ZiYuan > 0)
                            {
                                syncData.ServerZiYuanDict[sData.ServerId] = new FightInfo
                                {
                                    ZiYuan = sData.ZiYuan
                                };
                            }
                            if (fubenData.GameId > 0L)
                            {
                                sData.FuBenData = fubenData;
                                fubenData.ServerIdList.Add(sData.ServerId);
                                for (int i = 0; i < sData.JingJiaList.Count; i++)
                                {
                                    KuaFuLueDuoBangHuiJingJiaData jData = sData.JingJiaList[i];
                                    fubenData.BhDataList.Add(jData.BhId);
                                    int selfServerId = this.GetServerIdByZoneId(jData.ZoneId);
                                    if (!fubenData.ServerIdList.Contains(selfServerId))
                                    {
                                        fubenData.ServerIdList.Add(selfServerId);
                                    }
                                }
                                if (sData.JingJiaList.Count == 0)
                                {
                                    sData.FuBenData.ServerId = sData.ServerId;
                                    this.FuBenMgr.FuBenDataDict[fubenData.GameId] = fubenData;
                                    change = true;
                                }
                            }
                        }
                        if (sData.FuBenData != null && sData.FuBenData.ServerId == 0)
                        {
                            int toServerId = 0;
                            if (!ClientAgentManager.Instance().AssginKfFuben(GameTypes.KuaFuLueDuo, fubenData.GameId, 60, out toServerId))
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("跨服掠夺分配副本分配游戏服务器失败,serverid={0},bhid={1}", fubenData.DestServerId, string.Join<int>(",", fubenData.BhDataList)), null, true);
                                return;
                            }
                            fubenData.ServerId = toServerId;
                            this.FuBenMgr.FuBenDataDict[fubenData.GameId] = fubenData;
                            change = true;
                            LogManager.WriteLog(LogTypes.Analysis, string.Format("跨服掠夺分配副本,gameId={0},serverid={1},dest={3},bhid={2}", new object[]
                            {
                                fubenData.GameId,
                                fubenData.DestServerId,
                                string.Join<int>(",", fubenData.BhDataList),
                                toServerId
                            }), null, true);
                        }
                    }
                    if (change)
                    {
                        TimeUtil.AgeByNow(ref syncData.FuBenStateAge);
                    }
                }
                KuaFuLueDuoStateMachine.StateType gameState = this.StateMachine.GetCurrState();
                if (nextGameState != gameState)
                {
                    this.StateMachine.SetCurrState(KuaFuLueDuoStateMachine.StateType.NotifyEnter, now, param);
                    LogManager.WriteLog(LogTypes.Analysis, string.Format("KuaFuLueDuo::MS_PrepareGame_Update To:{0} SeasonID:{1}", KuaFuLueDuoStateMachine.StateType.NotifyEnter, this.SeasonID), null, true);
                }
            }
        }

        // Token: 0x06000578 RID: 1400 RVA: 0x0004912C File Offset: 0x0004732C
        private void MS_NotifyEnter_Update(DateTime now, int param)
        {
            KuaFuLueDuoStateMachine.StateType nextGameState = KuaFuLueDuoStateMachine.StateType.GameStart;
            this.GameState = 2;
            KuaFuLueDuoFuBenMgrData fubenMgr = this.FuBenMgr;
            foreach (KuaFuLueDuoFuBenData item in fubenMgr.FuBenDataDict.Values)
            {
                KuaFuLueDuoNtfEnterData SyncData = new KuaFuLueDuoNtfEnterData();
                SyncData.BhIdList.AddRange(item.BhDataList);
                ClientAgentManager.Instance().BroadCastAsyncEvent(GameTypes.HuanYingSiYuan, new AsyncDataItem(KuaFuEventTypes.KuaFuLueDuoNtfEnter, new object[]
                {
                    SyncData
                }), 0);
            }
            KuaFuLueDuoStateMachine.StateType gameState = this.StateMachine.GetCurrState();
            this.StateMachine.SetCurrState(nextGameState, now, param);
            LogManager.WriteLog(LogTypes.Analysis, string.Format("KuaFuLueDuo::MS_PrepareGame_Update GameState:{0} To:{1} SeasonID:{2} LastSeasonID:{3} Round:{4}", new object[]
            {
                gameState,
                nextGameState,
                this.SeasonID,
                this.LastSeasonID,
                this.SignUpRound
            }), null, true);
        }

        // Token: 0x06000579 RID: 1401 RVA: 0x00049254 File Offset: 0x00047454
        private void MS_GameStart_Update(DateTime now, int param)
        {
            TimeSpan nowTs = TimeUtil.GetTimeOfWeekNow2();
            if (!(nowTs < this.StateMachine.Tag2))
            {
                this.GameState = 3;
                KuaFuLueDuoConfig matchConfig = this.RuntimeData.GetKuaFuLueDuoConfig(param);
                KuaFuLueDuoStateMachine.StateType nextGameState = KuaFuLueDuoStateMachine.StateType.GameStart;
                for (int i = 0; i < matchConfig.TimePoints.Count - 1; i += 2)
                {
                    if ((nowTs - matchConfig.TimePoints[i]).TotalSeconds > (double)matchConfig.GameSecs && (i < matchConfig.TimePoints.Count - 1 || nowTs < matchConfig.TimePoints[i + 1]))
                    {
                        nextGameState = KuaFuLueDuoStateMachine.StateType.RankAnalyse;
                        break;
                    }
                }
                KuaFuLueDuoStateMachine.StateType gameState = this.StateMachine.GetCurrState();
                if (nextGameState != gameState)
                {
                    this.StateMachine.Tag2 = TimeSpan.MaxValue;
                    this.StateMachine.SetCurrState(nextGameState, now, param);
                    LogManager.WriteLog(LogTypes.Analysis, string.Format("KuaFuLueDuo::MS_GameStart_Update GameState:{0} To:{1} SeasonID:{2} LastSeasonID:{3} Round:{4}", new object[]
                    {
                        gameState,
                        nextGameState,
                        this.SeasonID,
                        this.LastSeasonID,
                        this.SignUpRound
                    }), null, true);
                }
            }
        }

        // Token: 0x0600057A RID: 1402 RVA: 0x000493B8 File Offset: 0x000475B8
        private void HandleUnCompleteFuBenData()
        {
            foreach (KeyValuePair<long, KuaFuLueDuoFuBenData> fubenItem in this.FuBenMgr.FuBenDataDict)
            {
                KuaFuLueDuoFuBenData fubenData = fubenItem.Value;
                if (fubenData.State < 3)
                {
                    KuaFuLueDuoGroupItem g;
                    if (!this.RuntimeData.GroupConfigDict.TryGetValue(fubenData.DestServerId, out g))
                    {
                        continue;
                    }
                    KuaFuLueDuoSyncData syncData;
                    if (!this.SyncDataDict.TryGetValue(g.GroupID, out syncData))
                    {
                        continue;
                    }
                    KuaFuLueDuoServerInfo sData;
                    if (syncData.ServerInfoDict.TryGetValue(fubenData.DestServerId, out sData))
                    {
                        sData.LastZiYuan = 0;
                        this.Persistence.SaveKuaFuLueDuoServerData(this.SeasonID, sData);
                    }
                    KuaFuData<KuaFuLueDuoBHData> d0 = null;
                    foreach (int bhid in fubenData.BhDataList)
                    {
                        if (this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out d0))
                        {
                            d0.V.last_ziyuan = 0;
                            d0.V.season = this.SeasonID;
                            TimeUtil.AgeByNow(ref d0.Age);
                            this.Persistence.SaveKuaFuLueDuoBHData(d0.V);
                        }
                    }
                }
                ClientAgentManager.Instance().RemoveKfFuben(GameTypes.KuaFuLueDuo, fubenData.ServerId, fubenData.GameId);
            }
            this.FuBenMgr.Clear();
        }

        // Token: 0x0600057B RID: 1403 RVA: 0x00049598 File Offset: 0x00047798
        private void MS_RankAnalyse_Enter(DateTime now, int param)
        {
            this.GameState = 4;
            TimeSpan nowTs = TimeUtil.GetTimeOfWeekNow2();
            if (nowTs < this.StateMachine.Tag2)
            {
                if (this.StateMachine.Tag2 == TimeSpan.MaxValue)
                {
                    this.StateMachine.Tag2 = TimeUtil.GetTimeOfWeek2(TimeUtil.NowDateTime().AddSeconds((double)this.RuntimeData.KuaFuLueDuoWaitRankSecs));
                }
            }
            else
            {
                this.HandleUnCompleteFuBenData();
                this.Persistence.DelayWriteDataProc();
                this.SeasonCount++;
                this.Persistence.SaveSeasonID(this.SeasonID);
                this.LoadDatabase(TimeUtil.NowDateTime(), true);
                this.GameState = 0;
                KuaFuLueDuoStateMachine.StateType gameState = this.StateMachine.GetCurrState();
                KuaFuLueDuoStateMachine.StateType nextGameState = KuaFuLueDuoStateMachine.StateType.Init;
                this.StateMachine.SetCurrState(nextGameState, now, param);
                LogManager.WriteLog(LogTypes.Analysis, string.Format("KuaFuLueDuo::MS_RankAnalyse_Enter GameState:{0} To:{1} SeasonID:{2} LastSeasonID:{3} Round:{4}", new object[]
                {
                    gameState,
                    nextGameState,
                    this.SeasonID,
                    this.LastSeasonID,
                    this.SignUpRound
                }), null, true);
            }
        }

        // Token: 0x0600057C RID: 1404 RVA: 0x000496DC File Offset: 0x000478DC
        public KuaFuLueDuoSyncData SyncData_KuaFuLueDuo(byte[] bytes)
        {
            try
            {
                KuaFuLueDuoSyncData result = DataHelper2.BytesToObject<KuaFuLueDuoSyncData>(bytes, 0, bytes.Length);
                if (null == result)
                {
                    result = new KuaFuLueDuoSyncData();
                }
                lock (this.Mutex)
                {
                    KuaFuLueDuoSyncData syncData = null;
                    result.SeasonID = this.SeasonID;
                    result.LastSeasonID = this.LastSeasonID;
                    if (result.ServerID < 9000)
                    {
                        KuaFuLueDuoGroupItem g;
                        if (!this.RuntimeData.ZoneGroupConfigDict.TryGetValue(result.ServerID, out g))
                        {
                            result.ServerGameState = this.GameState;
                            result.GameState = -1039;
                        }
                        else if (!this.SyncDataDict.TryGetValue(g.GroupID, out syncData))
                        {
                            result.GameState = 0;
                        }
                        else
                        {
                            result.GroupID = syncData.GroupID;
                            result.GameState = this.GameState;
                            result.ServerGameState = this.GameState;
                            if (result.ServerInfoDictAge != syncData.ServerInfoDictAge)
                            {
                                result.ServerInfoDictAge = syncData.ServerInfoDictAge;
                                result.ServerInfoDict = syncData.ServerInfoDict;
                            }
                            if (result.KuaFuLueDuoRankInfoDict.Age != syncData.KuaFuLueDuoRankInfoDict.Age)
                            {
                                result.KuaFuLueDuoRankInfoDict = syncData.KuaFuLueDuoRankInfoDict;
                            }
                            if (result.StateAge != syncData.StateAge)
                            {
                                result.StateAge = syncData.StateAge;
                                result.StateList = syncData.StateList;
                                result.SignUpRound = syncData.SignUpRound;
                            }
                            if (result.FuBenStateAge != syncData.FuBenStateAge)
                            {
                                result.FuBenStateAge = syncData.FuBenStateAge;
                                result.ServerZiYuanDict = syncData.ServerZiYuanDict;
                                result.BhZiYuanDict = syncData.BhZiYuanDict;
                            }
                        }
                    }
                    else
                    {
                        result.ServerGameState = this.GameState;
                        result.GameState = -1039;
                        if (result.FuBenStateAge == 1L)
                        {
                            if (null != result.ServerZiYuanDict)
                            {
                                foreach (KeyValuePair<int, FightInfo> kv in result.ServerZiYuanDict)
                                {
                                    KuaFuLueDuoGroupItem g;
                                    if (!this.RuntimeData.ZoneGroupConfigDict.TryGetValue(kv.Key, out g))
                                    {
                                        result.ServerGameState = this.GameState;
                                        result.GameState = -1039;
                                        break;
                                    }
                                    if (!this.SyncDataDict.TryGetValue(g.GroupID, out syncData))
                                    {
                                        result.GameState = 0;
                                        break;
                                    }
                                    syncData.ServerZiYuanDict[kv.Key] = kv.Value;
                                    TimeUtil.AgeByNow(ref syncData.FuBenStateAge);
                                }
                            }
                            if (null != result.BhZiYuanDict)
                            {
                                foreach (KeyValuePair<int, FightInfo> kv in result.BhZiYuanDict)
                                {
                                    KuaFuData<KuaFuLueDuoBHData> bh;
                                    if (this.KuaFuLueDuoBHDataDict.TryGetValue(kv.Key, out bh))
                                    {
                                        KuaFuLueDuoGroupItem g;
                                        if (this.RuntimeData.ZoneGroupConfigDict.TryGetValue(bh.V.zoneid, out g))
                                        {
                                            if (this.SyncDataDict.TryGetValue(g.GroupID, out syncData))
                                            {
                                                syncData.BhZiYuanDict[kv.Key] = kv.Value;
                                                TimeUtil.AgeByNow(ref syncData.FuBenStateAge);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.SyncData_KuaFuLueDuo failed!", ex, true);
            }
            return new KuaFuLueDuoSyncData
            {
                GameState = -11003
            };
        }

        // Token: 0x0600057D RID: 1405 RVA: 0x00049B8C File Offset: 0x00047D8C
        public KuaFuLueDuoJingJiaResult JingJia_KuaFuLueDuo(int bhid, int zoneid_bh, string bhname, int ziJin, int serverId, int oldZiJin)
        {
            KuaFuLueDuoJingJiaResult result = new KuaFuLueDuoJingJiaResult(0, 0, 0);
            try
            {
                DateTime now = TimeUtil.NowDateTime();
                long nowTicks = TimeUtil.NOW();
                lock (this.Mutex)
                {
                    KuaFuLueDuoConfig matchConfig = this.RuntimeData.GetKuaFuLueDuoConfig(0);
                    if (null == matchConfig)
                    {
                        return -3;
                    }
                    KuaFuLueDuoGroupItem g;
                    KuaFuLueDuoGroupItem g2;
                    if (!this.RuntimeData.ZoneGroupConfigDict.TryGetValue(zoneid_bh, out g) || !this.RuntimeData.ZoneGroupConfigDict.TryGetValue(zoneid_bh, out g2) || g.GroupID != g2.GroupID)
                    {
                        return -1039;
                    }
                    KuaFuLueDuoSyncData syncData = null;
                    if (!this.SyncDataDict.TryGetValue(g.GroupID, out syncData))
                    {
                        return -1039;
                    }
                    if (!Consts.TestMode)
                    {
                        if (!ClientAgentManager.Instance().IsAgentAlive(serverId))
                        {
                            return -11000;
                        }
                    }
                    KuaFuData<KuaFuLueDuoBHData> bhData;
                    if (!this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out bhData))
                    {
                        bhData = new KuaFuData<KuaFuLueDuoBHData>();
                        bhData.V.bhid = bhid;
                        bhData.V.bhname = bhname;
                        bhData.V.zoneid = zoneid_bh;
                        this.KuaFuLueDuoBHDataDict[bhid] = bhData;
                    }
                    if (bhData.V.jingjia_sid > 0 && bhData.V.jingjia_sid != serverId)
                    {
                        return -1004;
                    }
                    result.ZiJin = bhData.V.jingjia;
                    KuaFuLueDuoServerJingJiaState sData;
                    if (bhData.V.jingjia > oldZiJin)
                    {
                        result.Result = -11000;
                    }
                    else if (!syncData.StateList.TryGetValue(serverId, out sData) || sData.Round != syncData.SignUpRound)
                    {
                        result.Result = -1042;
                    }
                    else if (nowTicks < bhData.V.JingJiaTicks)
                    {
                        result.Result = -2007;
                        result.CDTicks = bhData.V.JingJiaTicks;
                    }
                    else
                    {
                        bhData.V.JingJiaTicks = TimeUtil.NOW() + (long)(this.RuntimeData.CrusadeApplyCD * 1000);
                        KuaFuLueDuoBangHuiJingJiaData jdata = sData.JingJiaList.Find((KuaFuLueDuoBangHuiJingJiaData x) => x.BhId == bhid);
                        if (null != jdata)
                        {
                            result.ZiJin = jdata.ZiJin;
                            jdata.ZiJin = ziJin;
                        }
                        else
                        {
                            bool flag2;
                            if (sData.JingJiaList.Count >= this.RuntimeData.CrusadeAttackerNum)
                            {
                                flag2 = !sData.JingJiaList.Any((KuaFuLueDuoBangHuiJingJiaData x) => x.ZiJin < ziJin);
                            }
                            else
                            {
                                flag2 = false;
                            }
                            if (flag2)
                            {
                                result = -1038;
                                return result;
                            }
                            sData.JingJiaList.Add(new KuaFuLueDuoBangHuiJingJiaData
                            {
                                BhId = bhid,
                                BhName = bhname,
                                ZoneId = zoneid_bh,
                                ServerId = serverId,
                                ZiJin = ziJin
                            });
                            sData.JingJiaList.Sort();
                            sData.JingJiaList.Reverse();
                            for (int i = sData.JingJiaList.Count - 1; i >= this.RuntimeData.CrusadeAttackerNum; i--)
                            {
                                KuaFuLueDuoBangHuiJingJiaData bh = sData.JingJiaList[i];
                                sData.JingJiaList.RemoveAt(i);
                                KuaFuData<KuaFuLueDuoBHData> bh2;
                                if (this.KuaFuLueDuoBHDataDict.TryGetValue(bh.BhId, out bh2))
                                {
                                    bh2.V.jingjia_sid = 0;
                                    bh2.V.jingjia = 0;
                                    bh2.V.last_jingjia = bh2.V.jingjia;
                                    this.Persistence.SaveKuaFuLueDuoBHSeasonData(this.SeasonID, bh2.V);
                                }
                            }
                        }
                        bhData.V.season = this.SeasonID;
                        bhData.V.bhname = bhname;
                        bhData.V.jingjia = ziJin;
                        bhData.V.jingjia_sid = serverId;
                        bhData.V.group = g.GroupID;
                        TimeUtil.AgeByNow(ref bhData.Age);
                        TimeUtil.AgeByNow(ref syncData.StateAge);
                        bhData.V.last_ziyuan = 0;
                        this.Persistence.SaveKuaFuLueDuoBHData(bhData.V);
                        this.Persistence.SaveKuaFuLueDuoBHSeasonData(this.SeasonID, bhData.V);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.RookieSignUp_KuaFuLueDuo failed!", ex, true);
            }
            return -11003;
        }

        // Token: 0x0600057E RID: 1406 RVA: 0x0004A13C File Offset: 0x0004833C
        public KuaFuLueDuoFuBenData GetFuBenDataByServerId_KuaFuLueDuo(int serverId)
        {
            KuaFuLueDuoFuBenData fuBenData = null;
            try
            {
                lock (this.Mutex)
                {
                    foreach (KeyValuePair<long, KuaFuLueDuoFuBenData> kv in this.FuBenMgr.FuBenDataDict)
                    {
                        if (kv.Value.DestServerId == serverId)
                        {
                            fuBenData = kv.Value;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.GetFuBenDataByServerId_KuaFuLueDuo failed!", ex, true);
            }
            return fuBenData;
        }

        // Token: 0x0600057F RID: 1407 RVA: 0x0004A220 File Offset: 0x00048420
        public KuaFuLueDuoFuBenData GetFuBenDataByGameId_KuaFuLueDuo(long gameid)
        {
            KuaFuLueDuoFuBenData fuBenData = null;
            try
            {
                lock (this.Mutex)
                {
                    this.FuBenMgr.FuBenDataDict.TryGetValue(gameid, out fuBenData);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.GetFuBenDataByGameId_KuaFuLueDuo failed!", ex, true);
            }
            return fuBenData;
        }

        // Token: 0x06000580 RID: 1408 RVA: 0x0004A2A8 File Offset: 0x000484A8
        public byte[] GetRoleData_KuaFuLueDuo(long rid)
        {
            byte[] result = null;
            try
            {
                int rankNum = this.Persistence.GetRoleKillNum(this.MinSeasonID, rid);
                result = DataHelper2.ObjectToBytes<int>(rankNum);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.GetRoleData_KuaFuLueDuo failed!", ex, true);
            }
            return result;
        }

        // Token: 0x06000581 RID: 1409 RVA: 0x0004A300 File Offset: 0x00048500
        public KuaFuCmdData GetBHDataByBhid_KuaFuLueDuo(int bhid, long age)
        {
            try
            {
                lock (this.Mutex)
                {
                    KuaFuData<KuaFuLueDuoBHData> bhData = null;
                    this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out bhData);
                    if (bhData == null)
                    {
                        return null;
                    }
                    if (age != bhData.Age)
                    {
                        return new KuaFuCmdData
                        {
                            Age = bhData.Age,
                            Bytes0 = DataHelper2.ObjectToBytes<KuaFuLueDuoBHData>(bhData.V)
                        };
                    }
                    return new KuaFuCmdData
                    {
                        Age = bhData.Age
                    };
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.GetBHDataByBhid_KuaFuLueDuo failed!", ex, true);
            }
            return null;
        }

        // Token: 0x06000582 RID: 1410 RVA: 0x0004A3E4 File Offset: 0x000485E4
        public int GameFuBenComplete_KuaFuLueDuo(KuaFuLueDuoStatisticalData data)
        {
            int result = 0;
            try
            {
                lock (this.Mutex)
                {
                    KuaFuLueDuoGroupItem g;
                    if (!this.RuntimeData.ZoneGroupConfigDict.TryGetValue(data.DestServerID, out g))
                    {
                        return -1039;
                    }
                    KuaFuLueDuoFuBenData fubenData = null;
                    if (data.GameId == -2L)
                    {
                        fubenData = new KuaFuLueDuoFuBenData
                        {
                            ServerId = 9801,
                            DestServerId = data.DestServerID
                        };
                    }
                    else
                    {
                        this.FuBenMgr.FuBenDataDict.TryGetValue(data.GameId, out fubenData);
                        if (fubenData == null || fubenData.State == 3)
                        {
                            return 3;
                        }
                        fubenData.State = 3;
                    }
                    KuaFuLueDuoSyncData syncData;
                    if (this.SyncDataDict.TryGetValue(g.GroupID, out syncData))
                    {
                        if (data.DestServerID > 0)
                        {
                            KuaFuLueDuoServerInfo sData;
                            if (syncData.ServerInfoDict.TryGetValue(data.DestServerID, out sData))
                            {
                                sData.LastZiYuan = Math.Max(sData.ZiYuan - data.LeftZiYuan, 0);
                                sData.ZiYuan = data.LeftZiYuan;
                                this.Persistence.SaveKuaFuLueDuoServerData(this.SeasonID, sData);
                            }
                        }
                        if (data.SuccessServerID > 0 && data.LeftZiYuan <= 0)
                        {
                            KuaFuLueDuoServerInfo sData;
                            if (syncData.ServerInfoDict.TryGetValue(data.SuccessServerID, out sData))
                            {
                                if (null == sData.ZhengFuList)
                                {
                                    sData.ZhengFuList = new List<int>();
                                }
                                if (!sData.ZhengFuList.Contains(data.DestServerID))
                                {
                                    sData.ZhengFuList.Add(fubenData.DestServerId);
                                    this.Persistence.SaveKuaFuLueDuoServerRankData(this.SeasonID, g.GroupID, sData, data.DestServerID, data.Percent);
                                }
                                this.Persistence.SaveKuaFuLueDuoServerZhengFuData(this.SeasonID, sData);
                            }
                        }
                        foreach (KuaFuLueDuoLueDuoResultData lueDuoData in data.LueDuoResultList)
                        {
                            KuaFuData<KuaFuLueDuoBHData> d0;
                            if (this.KuaFuLueDuoBHDataDict.TryGetValue(lueDuoData.bhid, out d0))
                            {
                                d0.V.season = this.SeasonID;
                                d0.V.sum_ziyuan += lueDuoData.ziyuan;
                                d0.V.last_ziyuan = lueDuoData.ziyuan;
                                if (string.IsNullOrEmpty(lueDuoData.bhname))
                                {
                                    lueDuoData.bhname = d0.V.bhname;
                                }
                                else if (d0.V.bhname != lueDuoData.bhname)
                                {
                                    d0.V.bhname = lueDuoData.bhname;
                                }
                                TimeUtil.AgeByNow(ref d0.Age);
                                this.Persistence.SaveKuaFuLueDuoBHData(d0.V);
                            }
                        }
                        foreach (KuaFuLueDuoRoleData rData in data.roleStatisticalData)
                        {
                            this.Persistence.SaveKuaFuLueDuoRoleData(this.SeasonID, rData.rid, rData.rname, rData.zoneid, rData.kill);
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = -11;
                LogManager.WriteLog(LogTypes.Error, "KuaFuLueDuoService.GameFuBenComplete_KuaFuLueDuo failed!", ex, true);
            }
            return result;
        }

        // Token: 0x040002EF RID: 751
        public const GameTypes GameType = GameTypes.KuaFuLueDuo;

        // Token: 0x040002F0 RID: 752
        public const GameTypes NotifyGameType = GameTypes.HuanYingSiYuan;

        // Token: 0x040002F1 RID: 753
        private static KuaFuLueDuoService _instance = new KuaFuLueDuoService();

        // Token: 0x040002F2 RID: 754
        private object Mutex = new object();

        // Token: 0x040002F3 RID: 755
        private bool Initialized = false;

        // Token: 0x040002F4 RID: 756
        private KuaFuLueDuoCommonData RuntimeData = new KuaFuLueDuoCommonData();

        // Token: 0x040002F5 RID: 757
        public KuaFuLueDuoPersistence Persistence = KuaFuLueDuoPersistence.Instance;

        // Token: 0x040002F6 RID: 758
        private int GameState;

        // Token: 0x040002F7 RID: 759
        private DateTime LastDate;

        // Token: 0x040002F8 RID: 760
        private Dictionary<int, KuaFuLueDuoSyncData> SyncDataDict = new Dictionary<int, KuaFuLueDuoSyncData>();

        // Token: 0x040002F9 RID: 761
        private int SeasonID;

        // Token: 0x040002FA RID: 762
        private int LastSeasonID;

        // Token: 0x040002FB RID: 763
        private int MinSeasonID;

        // Token: 0x040002FC RID: 764
        private int SignUpRound;

        // Token: 0x040002FD RID: 765
        private KuaFuLueDuoFuBenMgrData FuBenMgr = new KuaFuLueDuoFuBenMgrData();

        // Token: 0x040002FE RID: 766
        private KuaFuLueDuoStateMachine StateMachine = new KuaFuLueDuoStateMachine();

        // Token: 0x040002FF RID: 767
        private uint UpdateFrameCount = 0U;

        // Token: 0x04000300 RID: 768
        private DateTime LastUpdateTime = DateTime.MinValue;
    }
}
