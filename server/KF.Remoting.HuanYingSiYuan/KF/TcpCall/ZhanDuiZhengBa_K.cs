using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AutoCSer.Metadata;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using KF.Remoting.Data;
using Remoting;
using Server.Tools;
using Tmsk.Contract;

namespace KF.TcpCall
{
    
    [AutoCSer.Net.TcpStaticServer.Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = MemberFilters.Static, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
    public static class ZhanDuiZhengBa_K
    {
        
        static ZhanDuiZhengBa_K()
        {
            ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(1, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_Idle_Update), null));
            ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(2, new Action<DateTime>(ZhanDuiZhengBa_K.MS_Init_Enter), null, null));
            ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(4, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_InitPkLoop_Update), null));
            ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(5, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_NotifyEnter_Update), null));
            ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(6, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_PkLoopStart_Update), null));
            ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(7, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_PkLoopEnd_Update), null));
            ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(3, new Action<DateTime>(ZhanDuiZhengBa_K.MS_TodayPkEnd_Enter), new Action<DateTime>(ZhanDuiZhengBa_K.MS_TodayPkEnd_Update), null));
            ZhanDuiZhengBa_K.StateMachine.SetCurrState(1, TimeUtil.NowDateTime());
        }

        
        public static bool InitConfig()
        {
            lock (ZhanDuiZhengBa_K.Mutex)
            {
                ZhanDuiZhengBa_K.Initialize = false;
                bool bOk = ZhanDuiZhengBa_K._Config.Load(KuaFuServerManager.GetResourcePath("Config\\TeamMatch.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\TeamMatchBirthPoint.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
                ZhanDuiZhengBaConsts.MinZhanDuiNum = (Global.TestMode ? 0 : 4);
                if (!bOk)
                {
                    LogManager.WriteLog(LogTypes.Error, "ZhanDuiZhengBa_K.InitConfig failed!", null, true);
                }
                ZhanDuiZhengBa_K.Initialize = bOk;
            }
            return true;
        }

        
        public static void Update()
        {
            if (ZhanDuiZhengBa_K.Initialize)
            {
                DateTime now = TimeUtil.NowDateTime();
                if (now.Day != ZhanDuiZhengBa_K.lastUpdateTime.Day)
                {
                    ZhanDuiZhengBa_K.FixSyncData(now);
                }
                else
                {
                    lock (ZhanDuiZhengBa_K.Mutex)
                    {
                        ZhanDuiZhengBa_K.StateMachine.Tick(now);
                    }
                }
                KFCallMsg[] asyncEvArray = null;
                lock (ZhanDuiZhengBa_K.Mutex)
                {
                    asyncEvArray = ZhanDuiZhengBa_K.AsyncEvQ.ToArray();
                    ZhanDuiZhengBa_K.AsyncEvQ.Clear();
                }
                foreach (KFCallMsg msg in asyncEvArray)
                {
                    ClientAgentManager.Instance().BroadCastMsg(msg, 0);
                }
                ZhanDuiZhengBa_K.lastUpdateTime = now;
            }
        }

        
        private static void MS_Idle_Update(DateTime now)
        {
            if (now.Day == ZhanDuiZhengBaConfig.StartDay)
            {
                int month = TimeUtil.MakeYearMonth(now);
                lock (ZhanDuiZhengBa_K.Mutex)
                {
                    if (month == ZhanDuiZhengBa_K.SyncData.Month)
                    {
                        for (int i = 0; i < ZhanDuiZhengBa_K._Config.MatchConfigList.Count; i++)
                        {
                            ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList[i];
                            if (i == 0 && now.TimeOfDay.Ticks < matchConfig.DayBeginTick)
                            {
                                return;
                            }
                            if (now.TimeOfDay.Ticks >= matchConfig.DayBeginTick)
                            {
                                if (!Consts.TestMode)
                                {
                                    if (now.TimeOfDay.Ticks >= matchConfig.DayBeginTick && now.TimeOfDay.Ticks < matchConfig.DayEndTick)
                                    {
                                        if (ZhanDuiZhengBa_K.SyncData.RealActID != matchConfig.ID)
                                        {
                                            ZhanDuiZhengBa_K.SyncData.RealActID = matchConfig.ID;
                                            ZhanDuiZhengBa_K.StateMachine.SetCurrState(2, now);
                                        }
                                        return;
                                    }
                                    if (i == ZhanDuiZhengBa_K._Config.MatchConfigList.Count - 1 && now.TimeOfDay.Ticks >= matchConfig.DayEndTick)
                                    {
                                        if (ZhanDuiZhengBa_K.SyncData.RealActID != matchConfig.ID)
                                        {
                                            ZhanDuiZhengBa_K.SyncData.RealActID = matchConfig.ID;
                                        }
                                        return;
                                    }
                                }
                                else if (now.TimeOfDay.Ticks >= matchConfig.DayBeginTick && now.TimeOfDay.Ticks < matchConfig.DayEndTick)
                                {
                                    if (ZhanDuiZhengBa_K.SyncData.RealActID < matchConfig.ID || matchConfig.ID == 1)
                                    {
                                        ZhanDuiZhengBa_K.SyncData.RealActID = matchConfig.ID;
                                        ZhanDuiZhengBa_K.StateMachine.SetCurrState(2, now);
                                        return;
                                    }
                                }
                                else if (i == ZhanDuiZhengBa_K._Config.MatchConfigList.Count - 1 || now.TimeOfDay.Ticks < ZhanDuiZhengBa_K._Config.MatchConfigList[i + 1].DayBeginTick)
                                {
                                    if (ZhanDuiZhengBa_K.SyncData.RealActID != matchConfig.ID)
                                    {
                                        ZhanDuiZhengBa_K.SyncData.RealActID = matchConfig.ID;
                                        ZhanDuiZhengBa_K.StateMachine.SetCurrState(2, now);
                                    }
                                    return;
                                }
                            }
                        }
                        for (int i = 0; i < ZhanDuiZhengBa_K._Config.MatchConfigList.Count; i++)
                        {
                            ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList[i];
                            if (now.TimeOfDay.Ticks >= matchConfig.DayBeginTick)
                            {
                                if (i == ZhanDuiZhengBa_K._Config.MatchConfigList.Count - 1 || now.TimeOfDay.Ticks < ZhanDuiZhengBa_K._Config.MatchConfigList[i + 1].DayBeginTick)
                                {
                                    ZhanDuiZhengBa_K.SyncData.RealActID = matchConfig.ID;
                                    ZhanDuiZhengBa_K.StateMachine.SetCurrState(7, now);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ZhanDuiZhengBa_K.SyncData.RealActID = 0;
            }
        }

        
        private static void MS_Init_Enter(DateTime now)
        {
            ZhanDuiZhengBa_K.TodayJoinRoleDatas.Clear();
            foreach (ZhanDuiZhengBaZhanDuiData role in ZhanDuiZhengBa_K.SyncData.ZhanDuiList)
            {
                if (role.State == 0 || role.State == 1)
                {
                    ZhanDuiZhengBa_K.TodayJoinRoleDatas.Add(new ZhanDuiZhengBa_K.JoinRolePkData
                    {
                        ZhanDuiID = role.ZhanDuiID,
                        ZoneId = role.ZoneId,
                        RoleName = role.ZhanDuiName,
                        Group = role.Group,
                        Rank = role.DuanWeiRank
                    });
                }
            }
            ZhanDuiZhengBa_K.ThisLoopPkLogs.Clear();
            ZhanDuiZhengBa_K.StateMachine.SetCurrState(4, now);
        }

        
        private static bool CreateGameFuBen(DateTime now, ZhanDuiZhengBaMatchConfig matchConfig, ZhanDuiZhengBa_K.JoinRolePkData joinRole1, ZhanDuiZhengBa_K.JoinRolePkData joinRole2)
        {
            bool result;
            if (joinRole1.CurrGameID > 0)
            {
                result = true;
            }
            else
            {
                int betterZhanDuiID = (joinRole1.Rank < joinRole2.Rank) ? joinRole1.ZhanDuiID : joinRole2.ZhanDuiID;
                int toServerId = 0;
                int gameId = TianTiPersistence.Instance.GetNextGameId();
                if (ClientAgentManager.Instance().AssginKfFuben(ZhanDuiZhengBa_K.GameType, (long)gameId, 10, out toServerId))
                {
                    ZhanDuiZhengBaFuBenData copyData = new ZhanDuiZhengBaFuBenData();
                    copyData.GameID = (long)gameId;
                    copyData.SideDict[(long)joinRole1.ZhanDuiID] = 1;
                    copyData.SideDict[(long)joinRole2.ZhanDuiID] = 2;
                    copyData.BetterZhanDuiID = betterZhanDuiID;
                    copyData.ConfigID = matchConfig.ID;
                    copyData.JoinGrade = (int)matchConfig.JoinGrade;
                    copyData.NewGrade = (int)matchConfig.WillUpGrade;
                    copyData.ServerID = toServerId;
                    copyData.RoleDict.AddRange(TianTi5v5Service.GetZhanDuiMemberIDs(joinRole1.ZhanDuiID));
                    if (joinRole1.ZhanDuiID != joinRole2.ZhanDuiID)
                    {
                        copyData.RoleDict.AddRange(TianTi5v5Service.GetZhanDuiMemberIDs(joinRole2.ZhanDuiID));
                    }
                    joinRole1.ToServerID = (joinRole2.ToServerID = toServerId);
                    joinRole1.CurrGameID = (joinRole2.CurrGameID = gameId);
                    joinRole1.CopyData = (joinRole2.CopyData = copyData);
                    joinRole1.WaitReqEnter = (joinRole2.WaitReqEnter = true);
                    ZhanDuiZhengBaNtfEnterData data = new ZhanDuiZhengBaNtfEnterData();
                    data.ZhanDuiID1 = joinRole1.ZhanDuiID;
                    data.ZhanDuiID2 = joinRole2.ZhanDuiID;
                    data.ToServerId = toServerId;
                    data.GameId = gameId;
                    data.Day = ZhanDuiZhengBa_K.SyncData.RealActID;
                    ZhanDuiZhengBa_K.AsyncEvQ.Enqueue(KFCallMsg.New<ZhanDuiZhengBaNtfEnterData>(KuaFuEventTypes.ZhanDuiZhengBa_NotifyEnter, data));
                    LogManager.WriteLog(LogTypes.Info, string.Format("战队争霸第{0}轮战队成员通知入场，zhanduiID1={1},zhanduiID2={2}", ZhanDuiZhengBa_K.SyncData.RealActID, joinRole1.ZhanDuiID, joinRole2.ZhanDuiID), null, true);
                    ZhanDuiZhengBaPkLogData log = new ZhanDuiZhengBaPkLogData();
                    log.Month = ZhanDuiZhengBa_K.SyncData.Month;
                    log.ID = ZhanDuiZhengBa_K.SyncData.RealActID;
                    log.ZhanDuiID1 = joinRole1.ZhanDuiID;
                    log.ZoneID1 = joinRole1.ZoneId;
                    log.ZhanDuiName1 = joinRole1.RoleName;
                    log.ZhanDuiID2 = joinRole2.ZhanDuiID;
                    log.ZoneID2 = joinRole2.ZoneId;
                    log.ZhanDuiName2 = joinRole2.RoleName;
                    log.StartTime = now;
                    log.BetterZhanDuiID = betterZhanDuiID;
                    log.GameID = (long)gameId;
                    ZhanDuiZhengBa_K.ThisLoopPkLogs[gameId] = log;
                    result = true;
                }
                else
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("战队争霸第{0}轮分配游戏服务器失败，zhanduiID1={1},zhanduiID2={2}", ZhanDuiZhengBa_K.SyncData.RealActID, joinRole1.ZhanDuiID, joinRole2.ZhanDuiID), null, true);
                    result = false;
                }
            }
            return result;
        }

        
        private static void MS_InitPkLoop_Update(DateTime now)
        {
            ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
            List<ZhanDuiZhengBa_K.JoinRolePkData> tmpRoleDatas = new List<ZhanDuiZhengBa_K.JoinRolePkData>();
            using (List<RangeKey>.Enumerator enumerator = ZhanDuiZhengBaUtils.GetDayPkGroupRange(ZhanDuiZhengBa_K.SyncData.RealActID).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    RangeKey range = enumerator.Current;
                    List<ZhanDuiZhengBa_K.JoinRolePkData> groupRoles = ZhanDuiZhengBa_K.TodayJoinRoleDatas.FindAll((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.Group >= range.Left && _r.Group <= range.Right);
                    if (groupRoles != null && groupRoles.Count == 2)
                    {
                        ZhanDuiZhengBa_K.JoinRolePkData joinRole = groupRoles[0];
                        ZhanDuiZhengBa_K.JoinRolePkData joinRole2 = groupRoles[1];
                        if (joinRole.CurrGameID <= 0)
                        {
                            if (!ZhanDuiZhengBa_K.CreateGameFuBen(now, matchConfig, joinRole, joinRole2))
                            {
                                return;
                            }
                        }
                    }
                    else if (groupRoles != null && groupRoles.Count == 1)
                    {
                        if (!Consts.TestMode)
                        {
                            ZhanDuiZhengBa_K.JoinRolePkData joinRole3 = groupRoles[0];
                            joinRole3.ToServerID = 0;
                            joinRole3.CurrGameID = 0;
                            joinRole3.WaitReqEnter = false;
                        }
                        else
                        {
                            ZhanDuiZhengBa_K.JoinRolePkData joinRole = groupRoles[0];
                            ZhanDuiZhengBa_K.JoinRolePkData joinRole2 = groupRoles[0];
                            if (joinRole.CurrGameID <= 0)
                            {
                                if (!ZhanDuiZhengBa_K.CreateGameFuBen(now, matchConfig, joinRole, joinRole2))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            ZhanDuiZhengBa_K.StateMachine.SetCurrState(5, now);
        }

        
        private static void MS_NotifyEnter_Update(DateTime now)
        {
            ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
            if (now.TimeOfDay.Ticks >= matchConfig.DayBeginTick + 20000000L * (long)matchConfig.WaitSeconds)
            {
                ZhanDuiZhengBa_K.StateMachine.SetCurrState(6, now);
            }
        }

        
        private static void MS_PkLoopStart_Update(DateTime now)
        {
            ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
            if (now.TimeOfDay.Ticks >= matchConfig.DayBeginTick + 10000000L * (long)(matchConfig.WaitSeconds + matchConfig.FightSeconds + matchConfig.ClearSeconds))
            {
                ZhanDuiZhengBa_K.StateMachine.SetCurrState(7, now);
            }
        }

        
        private static void MS_PkLoopEnd_Update(DateTime now)
        {
            ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
            if (now.TimeOfDay.Ticks >= matchConfig.DayBeginTick + 10000000L * (long)(matchConfig.WaitSeconds + matchConfig.FightSeconds + matchConfig.ClearSeconds))
            {
                foreach (ZhanDuiZhengBaPkLogData kvp in ZhanDuiZhengBa_K.ThisLoopPkLogs.Values.ToList<ZhanDuiZhengBaPkLogData>())
                {
                    ZhanDuiZhengBa_K.ZhengBaPkResult((int)kvp.GameID, kvp.BetterZhanDuiID);
                }
                ZhanDuiZhengBa_K.ThisLoopPkLogs.Clear();
                foreach (ZhanDuiZhengBa_K.JoinRolePkData role in ZhanDuiZhengBa_K.TodayJoinRoleDatas)
                {
                    if (role.CurrGameID > 0 || role.ToServerID > 0)
                    {
                        ClientAgentManager.Instance().RemoveKfFuben(ZhanDuiZhengBa_K.GameType, role.ToServerID, (long)role.CurrGameID);
                    }
                    role.ToServerID = 0;
                    role.CurrGameID = 0;
                }
                ZhanDuiZhengBa_K.StateMachine.SetCurrState(3, now);
            }
        }

        
        private static void MS_TodayPkEnd_Enter(DateTime now)
        {
            ZhanDuiZhengBa_K.FixSyncData(now);
        }

        
        private static void MS_TodayPkEnd_Update(DateTime now)
        {
            ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
            if (now.TimeOfDay.Ticks >= matchConfig.DayEndTick)
            {
                ZhanDuiZhengBa_K.StateMachine.SetCurrState(1, now);
            }
        }

        
        public static void LoadSyncData(DateTime now, bool reload = false)
        {
            int selectRoleIfNewCreate = 64;
            lock (ZhanDuiZhengBa_K.Mutex)
            {
                ZhanDuiZhengBaSyncData syncData = ZhanDuiZhengBa_K.Persistence.LoadZhengBaSyncData(now, selectRoleIfNewCreate);
                ZhanDuiZhengBa_K.SyncData = syncData;
                ZhanDuiZhengBa_K.FixSyncData(now);
            }
        }

        
        private static bool FixSyncData_State(DateTime now)
        {
            bool bForceModify = false;
            int endID = 0;
            lock (ZhanDuiZhengBa_K.Mutex)
            {
                if (now.Day > ZhanDuiZhengBaConsts.StartMonthDay)
                {
                    endID = ZhanDuiZhengBa_K._Config.MatchConfigList[ZhanDuiZhengBa_K._Config.MatchConfigList.Count - 1].ID;
                }
                else if (now.Day == ZhanDuiZhengBaConsts.StartMonthDay)
                {
                    for (int i = 0; i < ZhanDuiZhengBa_K._Config.MatchConfigList.Count; i++)
                    {
                        ZhanDuiZhengBaMatchConfig c0 = ZhanDuiZhengBa_K._Config.MatchConfigList[i];
                        if (now.TimeOfDay.Ticks < c0.DayBeginTick)
                        {
                            break;
                        }
                        if (now.TimeOfDay.Ticks >= c0.ResultTick)
                        {
                            endID = c0.ID;
                        }
                    }
                }
                if (endID == ZhanDuiZhengBaUtils.WhichDayResultByGrade(EZhengBaGrade.Grade1))
                {
                    ZhanDuiZhengBa_K.SyncData.HasSeasonEnd = true;
                    ZhanDuiZhengBa_K.SyncData.TopZhanDui = ZhanDuiZhengBa_K.Persistence.GetLastTopZhanDui(ZhanDuiZhengBa_K.SyncData.Month);
                }
                else
                {
                    ZhanDuiZhengBa_K.SyncData.HasSeasonEnd = false;
                    ZhanDuiZhengBa_K.SyncData.TopZhanDui = ZhanDuiZhengBa_K.Persistence.GetLastTopZhanDui(ZhengBaUtils.MakeMonth(now.AddMonths(-1)));
                }
                int id;
                for (id = 1; id <= endID; id++)
                {
                    ZhanDuiZhengBaMatchConfig config = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == id);
                    EZhengBaGrade preGrade = config.JoinGrade;
                    EZhengBaGrade willUpGrade = config.WillUpGrade;
                    List<ZhanDuiZhengBaZhanDuiData> roleList = ZhanDuiZhengBa_K.SyncData.ZhanDuiList.FindAll((ZhanDuiZhengBaZhanDuiData _r) => _r.Grade > (int)willUpGrade);
                    if (roleList.Count > 0)
                    {
                        List<ZhanDuiZhengBaZhanDuiData> upGradeList = new List<ZhanDuiZhengBaZhanDuiData>();
                        using (List<RangeKey>.Enumerator enumerator = ZhanDuiZhengBaUtils.GetDayPkGroupRange(id).GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                RangeKey range = enumerator.Current;
                                List<ZhanDuiZhengBaZhanDuiData> groupRoleList = ZhanDuiZhengBa_K.SyncData.ZhanDuiList.FindAll((ZhanDuiZhengBaZhanDuiData _r) => _r.Group >= range.Left && _r.Group <= range.Right && _r.Grade <= (int)preGrade);
                                if (groupRoleList.Count != 0)
                                {
                                    if (!groupRoleList.Exists((ZhanDuiZhengBaZhanDuiData _r) => _r.Grade <= (int)willUpGrade))
                                    {
                                        groupRoleList.Sort(delegate (ZhanDuiZhengBaZhanDuiData _l, ZhanDuiZhengBaZhanDuiData _r)
                                        {
                                            int result;
                                            if (_l.Grade != _r.Grade)
                                            {
                                                result = _l.Grade - _r.Grade;
                                            }
                                            else
                                            {
                                                result = _l.DuanWeiRank - _r.DuanWeiRank;
                                            }
                                            return result;
                                        });
                                        ZhanDuiZhengBaZhanDuiData selectRole = groupRoleList[0];
                                        LogManager.WriteLog(LogTypes.Error, string.Format("战队争霸::晋级补位 [s{0}.{1}] {2}->{3}", new object[]
                                        {
                                            selectRole.ZoneId,
                                            selectRole.ZhanDuiID,
                                            selectRole.Grade,
                                            (int)willUpGrade
                                        }), null, true);
                                        selectRole.Grade = (int)willUpGrade;
                                        bForceModify = true;
                                        upGradeList.Add(selectRole);
                                        if (groupRoleList.Count >= 2)
                                        {
                                            ZhanDuiZhengBaZhanDuiData faildRole = groupRoleList[1];
                                            faildRole.Grade = (int)preGrade;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ZhanDuiZhengBa_K.SyncData.RealActID = endID;
                foreach (ZhanDuiZhengBaZhanDuiData role in ZhanDuiZhengBa_K.SyncData.ZhanDuiList)
                {
                    if (endID <= 0)
                    {
                        if (role.Grade != 64 || role.State != 0)
                        {
                            role.Grade = 64;
                            role.State = 0;
                            bForceModify = true;
                        }
                    }
                    else
                    {
                        EZhengBaGrade upGrade = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == endID).WillUpGrade;
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
                        if (role.Grade == 1)
                        {
                            ZhanDuiZhengBa_K.SyncData.TopZhanDui = role.ZhanDuiID;
                        }
                    }
                }
            }
            return bForceModify;
        }

        
        private static void FixSyncData(DateTime now)
        {
            lock (ZhanDuiZhengBa_K.Mutex)
            {
                bool bModify = false;
                bModify |= ZhanDuiZhengBa_K.FixSyncData_State(now);
                if (bModify)
                {
                    foreach (ZhanDuiZhengBaZhanDuiData role in ZhanDuiZhengBa_K.SyncData.ZhanDuiList)
                    {
                        ZhanDuiZhengBa_K.Persistence.UpdateRole(ZhanDuiZhengBa_K.SyncData.Month, role.ZhanDuiID, role.Grade, role.State);
                    }
                }
                ZhanDuiZhengBa_K.SyncData.RoleModTime = now;
            }
        }

        
        [AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
        public static ZhanDuiZhengBaSyncData SyncZhengBaData(ZhanDuiZhengBaSyncData lastSyncData)
        {
            ZhanDuiZhengBaSyncData result = new ZhanDuiZhengBaSyncData();
            lock (ZhanDuiZhengBa_K.Mutex)
            {
                result.Month = ZhanDuiZhengBa_K.SyncData.Month;
                result.RealActID = ZhanDuiZhengBa_K.SyncData.RealActID;
                result.RoleModTime = ZhanDuiZhengBa_K.SyncData.RoleModTime;
                result.HasSeasonEnd = ZhanDuiZhengBa_K.SyncData.HasSeasonEnd;
                result.IsThisMonthInActivity = ZhanDuiZhengBa_K.SyncData.IsThisMonthInActivity;
                result.CenterTime = TimeUtil.NowDateTime();
                result.TopZhanDui = ZhanDuiZhengBa_K.SyncData.TopZhanDui;
                if (result.RoleModTime > lastSyncData.RoleModTime && ZhanDuiZhengBa_K.SyncData.ZhanDuiList != null)
                {
                    result.ZhanDuiList = new List<ZhanDuiZhengBaZhanDuiData>(ZhanDuiZhengBa_K.SyncData.ZhanDuiList);
                }
                result.PKLogModTime = ZhanDuiZhengBa_K.SyncData.PKLogModTime;
                if (result.PKLogModTime > lastSyncData.PKLogModTime && ZhanDuiZhengBa_K.SyncData.PKLogList != null)
                {
                    result.PKLogList = new List<ZhanDuiZhengBaPkLogData>(ZhanDuiZhengBa_K.SyncData.PKLogList);
                }
            }
            return result;
        }

        
        [AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
        public static int ZhengBaKuaFuLogin(AutoCSer.Net.TcpInternalServer.ServerSocketSender socket, int zhanDuiID, int gameId, int srcServerID, out ZhanDuiZhengBaFuBenData copyData)
        {
            copyData = null;
            lock (ZhanDuiZhengBa_K.Mutex)
            {
                ZhanDuiZhengBa_K.JoinRolePkData roleData = ZhanDuiZhengBa_K.TodayJoinRoleDatas.Find((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == zhanDuiID && _r.CurrGameID == gameId);
                ZhanDuiZhengBaPkLogData logData = null;
                ZhanDuiZhengBa_K.ThisLoopPkLogs.TryGetValue(gameId, out logData);
                if (roleData == null || logData == null)
                {
                    return -12;
                }
                if (!roleData.WaitReqEnter)
                {
                    return -12;
                }
                copyData = roleData.CopyData;
            }
            KuaFuServerInfo serverInfo = KuaFuServerManager.GetKuaFuServerInfo(srcServerID);
            int result;
            if (null != serverInfo)
            {
                copyData.IPs = new string[]
                {
                    serverInfo.DbIp,
                    serverInfo.DbIp
                };
                copyData.Ports = new int[]
                {
                    serverInfo.DbPort,
                    serverInfo.LogDbPort
                };
                result = 0;
            }
            else
            {
                result = -11000;
            }
            return result;
        }

        
        [AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
        public static int ZhengBaRequestEnter(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
        {
            gameId = 0;
            kuaFuServerID = 0;
            ips = null;
            ports = null;
            lock (ZhanDuiZhengBa_K.Mutex)
            {
                if (ZhanDuiZhengBa_K.StateMachine.GetCurrState() != 5)
                {
                    if (!Consts.TestMode || ZhanDuiZhengBa_K.StateMachine.GetCurrState() != 6)
                    {
                        return -2001;
                    }
                }
                ZhanDuiZhengBa_K.JoinRolePkData roleData = ZhanDuiZhengBa_K.TodayJoinRoleDatas.Find((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == zhanDuiID);
                if (roleData == null || roleData.CurrGameID == 0)
                {
                    return -4006;
                }
                ZhanDuiZhengBaPkLogData logData = null;
                ZhanDuiZhengBa_K.ThisLoopPkLogs.TryGetValue(roleData.CurrGameID, out logData);
                if (roleData == null || logData == null)
                {
                    return -4006;
                }
                if (!roleData.WaitReqEnter)
                {
                    return -2001;
                }
                gameId = roleData.CurrGameID;
                kuaFuServerID = roleData.ToServerID;
            }
            KuaFuServerInfo serverInfo = KuaFuServerManager.GetKuaFuServerInfo(kuaFuServerID);
            int result;
            if (null != serverInfo)
            {
                ips = new string[]
                {
                    serverInfo.Ip
                };
                ports = new int[]
                {
                    serverInfo.Port
                };
                result = 0;
            }
            else
            {
                result = -11001;
            }
            return result;
        }

        
        [AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
        public static List<ZhanDuiZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner1)
        {
            List<ZhanDuiZhengBaNtfPkResultData> result;
            lock (ZhanDuiZhengBa_K.Mutex)
            {
                LogManager.WriteLog(LogTypes.Trace, string.Format("ZhanDuiZhengBa::ZhengBaPkResult,gameid={0},winner1={1}", gameId, winner1), null, true);
                ZhanDuiZhengBaPkLogData log = null;
                if (!ZhanDuiZhengBa_K.ThisLoopPkLogs.TryGetValue(gameId, out log))
                {
                    result = null;
                }
                else if (winner1 != log.ZhanDuiID1 && winner1 != log.ZhanDuiID2)
                {
                    result = null;
                }
                else
                {
                    ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == log.ID);
                    ZhanDuiZhengBa_K.JoinRolePkData winJoinRole = ZhanDuiZhengBa_K.TodayJoinRoleDatas.Find((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == winner1 && _r.CurrGameID == gameId);
                    ZhanDuiZhengBa_K.JoinRolePkData faildJoinRole = ZhanDuiZhengBa_K.TodayJoinRoleDatas.Find((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID != winner1 && _r.CurrGameID == gameId);
                    if (faildJoinRole != null && winJoinRole != null && faildJoinRole.ZhanDuiID != winJoinRole.ZhanDuiID)
                    {
                        ZhanDuiZhengBaZhanDuiData roleData = ZhanDuiZhengBa_K.SyncData.ZhanDuiList.Find((ZhanDuiZhengBaZhanDuiData _r) => _r.ZhanDuiID == faildJoinRole.ZhanDuiID);
                        if (roleData != null)
                        {
                            int newState = 2;
                            LogManager.WriteLog(LogTypes.Trace, string.Format("ZhanDuiZhengBa::ZhengBaPkResult,gameid={0},zhanduiid={1},newstate={2}", gameId, roleData.ZhanDuiID, newState), null, true);
                            if (ZhanDuiZhengBa_K.Persistence.UpdateRole(ZhanDuiZhengBa_K.SyncData.Month, roleData.ZhanDuiID, roleData.Grade, newState))
                            {
                                roleData.State = newState;
                                ZhanDuiZhengBa_K.SyncData.RoleModTime = TimeUtil.NowDateTime();
                            }
                        }
                        ZhanDuiZhengBa_K.TodayJoinRoleDatas.RemoveAll((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == faildJoinRole.ZhanDuiID);
                    }
                    if (winJoinRole != null)
                    {
                        bool bSaveUpdate = false;
                        ZhanDuiZhengBaZhanDuiData roleData = ZhanDuiZhengBa_K.SyncData.ZhanDuiList.Find((ZhanDuiZhengBaZhanDuiData _r) => _r.ZhanDuiID == winJoinRole.ZhanDuiID);
                        if (roleData != null)
                        {
                            int newGrade = (int)matchConfig.WillUpGrade;
                            int newState = 1;
                            LogManager.WriteLog(LogTypes.Trace, string.Format("ZhanDuiZhengBa::ZhengBaPkResult,gameid={0},zhanduiid={1},newstate={2}", gameId, roleData.ZhanDuiID, newState), null, true);
                            if (ZhanDuiZhengBa_K.Persistence.UpdateRole(ZhanDuiZhengBa_K.SyncData.Month, roleData.ZhanDuiID, newGrade, newState))
                            {
                                roleData.Grade = newGrade;
                                roleData.State = newState;
                                ZhanDuiZhengBa_K.SyncData.RoleModTime = TimeUtil.NowDateTime();
                                bSaveUpdate = true;
                            }
                        }
                        if (bSaveUpdate)
                        {
                            log.UpGrade = true;
                            ZhanDuiZhengBa_K.TodayJoinRoleDatas.RemoveAll((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == winJoinRole.ZhanDuiID);
                        }
                    }
                    log.EndTime = TimeUtil.NowDateTime();
                    if (winner1 > 0 && winner1 == log.ZhanDuiID1)
                    {
                        log.PkResult = 1;
                    }
                    else if (winner1 > 0 && winner1 == log.ZhanDuiID2)
                    {
                        log.PkResult = 2;
                    }
                    else
                    {
                        log.PkResult = 0;
                    }
                    ZhanDuiZhengBa_K.SyncData.PKLogList.Add(log);
                    TimeUtil.AgeByDateTime(ref ZhanDuiZhengBa_K.SyncData.PKLogModTime);
                    ZhanDuiZhengBa_K.Persistence.SavePkLog(log);
                    ZhanDuiZhengBa_K.ThisLoopPkLogs.Remove(gameId);
                    result = null;
                }
            }
            return result;
        }

        
        private static object Mutex = new object();

        
        private static bool Initialize = false;

        
        private static GameTypes GameType = GameTypes.ZhanDuiZhengBa;

        
        private static StateMachineSimple StateMachine = new StateMachineSimple(8);

        
        private static ZhanDuiZhengBaSyncData SyncData = new ZhanDuiZhengBaSyncData
        {
            Month = ZhanDuiZhengBaUtils.MakeMonth(TimeUtil.NowDateTime())
        };

        
        private static DateTime lastUpdateTime = TimeUtil.NowDateTime();

        
        private static Queue<KFCallMsg> AsyncEvQ = new Queue<KFCallMsg>();

        
        private static ZhanDuiZhengBaConfig _Config = new ZhanDuiZhengBaConfig();

        
        private static List<ZhanDuiZhengBa_K.JoinRolePkData> TodayJoinRoleDatas = new List<ZhanDuiZhengBa_K.JoinRolePkData>();

        
        private static Dictionary<int, ZhanDuiZhengBaPkLogData> ThisLoopPkLogs = new Dictionary<int, ZhanDuiZhengBaPkLogData>();

        
        private static ZhanDuiZhengBaPersistence Persistence = ZhanDuiZhengBaPersistence.Instance;

        
        private class JoinRolePkData
        {
            
            public int ZhanDuiID;

            
            public int ZoneId;

            
            public string RoleName;

            
            public int Group;

            
            public int Rank;

            
            public int ToServerID;

            
            public int CurrGameID;

            
            public bool WaitReqEnter;

            
            public ZhanDuiZhengBaFuBenData CopyData;
        }

        
        public class StateType
        {
            
            public const int None = 0;

            
            public const int Idle = 1;

            
            public const int Init = 2;

            
            public const int TodayPkEnd = 3;

            
            public const int InitPkLoop = 4;

            
            public const int NotifyEnter = 5;

            
            public const int PkLoopStart = 6;

            
            public const int PkLoopEnd = 7;

            
            public const int Max = 8;
        }

        
        internal static class TcpStaticServer
        {
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ZhanDuiZhengBaSyncData _M16(ZhanDuiZhengBaSyncData lastSyncData)
            {
                return ZhanDuiZhengBa_K.SyncZhengBaData(lastSyncData);
            }

            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int _M17(AutoCSer.Net.TcpInternalServer.ServerSocketSender _sender_, int zhanDuiID, int gameId, int srcServerID, out ZhanDuiZhengBaFuBenData copyData)
            {
                return ZhanDuiZhengBa_K.ZhengBaKuaFuLogin(_sender_, zhanDuiID, gameId, srcServerID, out copyData);
            }

            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static List<ZhanDuiZhengBaNtfPkResultData> _M18(int gameId, int winner1)
            {
                return ZhanDuiZhengBa_K.ZhengBaPkResult(gameId, winner1);
            }

            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int _M19(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
            {
                return ZhanDuiZhengBa_K.ZhengBaRequestEnter(zhanDuiID, out gameId, out kuaFuServerID, out ips, out ports);
            }
        }
    }
}
