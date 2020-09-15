using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Logic.Reborn;
using GameServer.Server;
using KF.Contract.Data;
using KF.Remoting;
using KF.TcpCall;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
    // Token: 0x0200008D RID: 141
    public class EscapeBattleManager : ICopySceneManager, IManager, ICmdProcessorEx, ICmdProcessor, IManager2, IEventListener, IEventListenerEx
    {
        // Token: 0x06000212 RID: 530 RVA: 0x000218FC File Offset: 0x0001FAFC
        private void InitScene(EscapeBattleScene scene, GameClient client)
        {
            foreach (EscapeBattleCollection item in this.RuntimeData.CollectionConfigList)
            {
                scene.CollectionConfigList.Add(item.Clone());
            }
            EscapeMapSafeArea areaInfo;
            lock (this.RuntimeData.Mutex)
            {
                areaInfo = this.RuntimeData.EscapeMapSafeAreaList[0];
            }
            scene.ScoreData.safeArea.AreaID = areaInfo.ID;
            scene.ScoreData.safeArea.PosX = areaInfo.StartSafePoint[0];
            scene.ScoreData.safeArea.PosY = areaInfo.StartSafePoint[1];
            scene.ScoreData.targetSafeArea.AreaID = areaInfo.ID;
            scene.ScoreData.targetSafeArea.PosX = areaInfo.StartSafePoint[0];
            scene.ScoreData.targetSafeArea.PosY = areaInfo.StartSafePoint[1];
            scene.ScoreData.AreaChangeTm = DateTime.MinValue;
            for (int i = 0; i < scene.TopClientCalExtProps.Length; i++)
            {
                scene.TopClientCalExtProps[i] = new double[177];
            }
        }

        // Token: 0x06000213 RID: 531 RVA: 0x00021B08 File Offset: 0x0001FD08
        bool ICopySceneManager.AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
        {
            bool result;
            if (sceneType == SceneUIClasses.EscapeBattle)
            {
                GameMap gameMap = null;
                if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
                {
                    result = false;
                }
                else
                {
                    int fuBenSeqId = copyMap.FuBenSeqID;
                    int mapCode = copyMap.MapCode;
                    int roleId = client.ClientData.RoleID;
                    long gameId = Global.GetClientKuaFuServerLoginData(client).GameId;
                    DateTime now = TimeUtil.NowDateTime().Date.Add(this.RuntimeData.DiffTimeSpan);
                    lock (this.RuntimeData.Mutex)
                    {
                        EscapeBattleScene scene = null;
                        if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
                        {
                            EscapeBattleFuBenData fuBenData;
                            if (!this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)((int)gameId), out fuBenData))
                            {
                                LogManager.WriteLog(LogTypes.Error, "魔界大逃杀没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
                            }
                            EscapeBattleMatchConfig sceneInfo = this.RuntimeData.Config.MatchConfigList.Find((EscapeBattleMatchConfig x) => x.MapCode == client.ClientData.MapCode);
                            if (null == sceneInfo)
                            {
                                LogManager.WriteLog(LogTypes.Error, "魔界大逃杀没有为副本找到对应的场景数据,MapCodeID:" + client.ClientData.MapCode, null, true);
                            }
                            scene = new EscapeBattleScene();
                            scene.CleanAllInfo();
                            scene.GameId = (int)gameId;
                            scene.CopyMap = copyMap;
                            scene.m_nMapCode = mapCode;
                            scene.CopyMapId = copyMap.CopyMapID;
                            scene.FuBenSeqId = fuBenSeqId;
                            scene.SceneInfo = sceneInfo;
                            scene.MapGridWidth = gameMap.MapGridWidth;
                            scene.MapGridHeight = gameMap.MapGridHeight;
                            scene.FuBenData = fuBenData;
                            DateTime startTime = now.Add(this.GetStartTime(sceneInfo.MapCode));
                            scene.StartTimeTicks = startTime.Ticks / 10000L;
                            this.InitScene(scene, client);
                            scene.GameStatisticalData.GameId = (int)gameId;
                            this.SceneDict[fuBenSeqId] = scene;
                        }
                        scene.ClientDict[client.ClientData.RoleID] = client;
                        EscapeBattleTeamInfo teamInfo = scene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
                        if (null == teamInfo)
                        {
                            TianTi5v5ZhanDuiData zhanduiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
                            if (null != zhanduiData)
                            {
                                teamInfo = new EscapeBattleTeamInfo
                                {
                                    TeamID = zhanduiData.ZhanDuiID,
                                    TeamName = zhanduiData.ZhanDuiName
                                };
                                scene.ScoreData.BattleTeamList.Add(teamInfo);
                                scene.GameStatisticalData.ZhanDuiDict[client.ClientData.ZhanDuiID] = zhanduiData;
                                scene.GameStatisticalData.ZhanDuiIDVsServerIDDict[client.ClientData.ZhanDuiID] = client.ServerId;
                            }
                        }
                        List<EscapeBattleRoleInfo> clientContextDataList;
                        if (!scene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out clientContextDataList))
                        {
                            clientContextDataList = new List<EscapeBattleRoleInfo>();
                            scene.ClientContextDataDict[client.ClientData.ZhanDuiID] = clientContextDataList;
                        }
                        EscapeBattleRoleInfo clientContextData = clientContextDataList.Find((EscapeBattleRoleInfo x) => x.RoleID == roleId);
                        if (null == clientContextData)
                        {
                            clientContextData = new EscapeBattleRoleInfo
                            {
                                RoleID = roleId,
                                Name = client.ClientData.RoleName,
                                Level = client.ClientData.Level,
                                ChangeLevel = client.ClientData.ChangeLifeCount,
                                ZoneID = client.ClientData.ZoneID,
                                Occupation = client.ClientData.Occupation,
                                RoleSex = client.ClientData.RoleSex,
                                LifeV = client.ClientData.CurrentLifeV,
                                MaxLifeV = client.ClientData.LifeV,
                                ZhanDuiID = client.ClientData.ZhanDuiID,
                                OnLine = true
                            };
                            clientContextDataList.Add(clientContextData);
                        }
                        else
                        {
                            clientContextData.Occupation = client.ClientData.Occupation;
                            clientContextData.RoleSex = client.ClientData.RoleSex;
                            clientContextData.LifeV = client.ClientData.CurrentLifeV;
                            clientContextData.MaxLifeV = client.ClientData.LifeV;
                            clientContextData.OnLine = true;
                        }
                        client.SceneObject = scene;
                        client.SceneGameId = (long)scene.GameId;
                        client.SceneContextData2 = clientContextData;
                        copyMap.IsKuaFuCopy = true;
                        copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
                    }
                    result = true;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x06000214 RID: 532 RVA: 0x00022124 File Offset: 0x00020324
        bool ICopySceneManager.RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
        {
            bool result;
            if (sceneType == SceneUIClasses.EscapeBattle)
            {
                lock (this.RuntimeData.Mutex)
                {
                    EscapeBattleScene EscapeBattleScene;
                    this.SceneDict.TryRemove(copyMap.FuBenSeqID, out EscapeBattleScene);
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x06000215 RID: 533 RVA: 0x0002219C File Offset: 0x0002039C
        void ICopySceneManager.TimerProc()
        {
            long nowTicks = TimeUtil.NOW();
            if (nowTicks >= EscapeBattleManager.NextHeartBeatTicks)
            {
                EscapeBattleManager.NextHeartBeatTicks = nowTicks + 1020L;
                foreach (EscapeBattleScene scene in this.SceneDict.Values)
                {
                    lock (this.RuntimeData.Mutex)
                    {
                        int nID = scene.FuBenSeqId;
                        int nCopyID = scene.CopyMapId;
                        int nMapCodeID = scene.m_nMapCode;
                        if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
                        {
                            CopyMap copyMap = scene.CopyMap;
                            DateTime now = TimeUtil.NowDateTime();
                            long ticks = TimeUtil.NOW();
                            if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_PREPARE || scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_BEGIN || scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_FIGHT || scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_ASS)
                            {
                                this.CheckCreateDynamicMonster(scene, ticks);
                                this.CheckDeleteDynamicMonster(scene, false, ticks);
                            }
                            if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_NULL)
                            {
                                if (ticks >= scene.StartTimeTicks)
                                {
                                    scene.m_lPrepareTime = scene.StartTimeTicks;
                                    scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.WaitSeconds * 1000);
                                    scene.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_PREPARE;
                                    scene.StateTimeData.GameType = 37;
                                    scene.StateTimeData.State = (int)scene.m_eStatus;
                                    scene.StateTimeData.EndTicks = scene.m_lBeginTime;
                                    GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
                                    this.InitCreateDynamicMonster(scene, ticks);
                                }
                            }
                            else if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_PREPARE)
                            {
                                if (ticks >= scene.m_lBeginTime)
                                {
                                    scene.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_BEGIN;
                                    scene.m_lFightTime = scene.m_lBeginTime + (long)(scene.SceneInfo.SafeSecs * 1000);
                                    scene.StateTimeData.GameType = 37;
                                    scene.StateTimeData.State = (int)scene.m_eStatus;
                                    scene.StateTimeData.EndTicks = scene.m_lFightTime;
                                    GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
                                    for (int guangMuId = 1; guangMuId <= 3; guangMuId++)
                                    {
                                        GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, guangMuId, 0);
                                    }
                                    this.InitCreateDynamicMonster(scene, ticks);
                                    this.BroadStateInfoAndScoreInfo(scene.CopyMap, -1, false, true);
                                }
                            }
                            else if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_BEGIN)
                            {
                                if (ticks >= scene.m_lFightTime)
                                {
                                    scene.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_FIGHT;
                                    scene.m_lEndTime = scene.m_lFightTime + (long)(scene.SceneInfo.BattleEndTime * 1000);
                                    scene.StateTimeData.GameType = 37;
                                    scene.StateTimeData.State = (int)scene.m_eStatus;
                                    scene.StateTimeData.EndTicks = scene.m_lEndTime;
                                    GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
                                    this.InitCreateDynamicMonster(scene, ticks);
                                    this.CheckDeleteDynamicMonster(scene, true, ticks);
                                    this.InitSceneBuffOnFightAss(scene, ticks);
                                    this.CheckUpdateSafeArea(scene, ticks);
                                    this.CheckZhanDuiWinLoseState(scene);
                                }
                            }
                            else if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_FIGHT || scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_ASS)
                            {
                                this.CheckUpdateSafeArea(scene, ticks);
                                this.CheckSceneAreaDamage(scene, ticks);
                                this.CheckSceneToAssState(scene, ticks);
                                if (ticks >= scene.m_lEndTime)
                                {
                                    this.ProcessEnd(scene, true, ticks);
                                }
                            }
                            else if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_END)
                            {
                                scene.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_AWARD;
                                this.GiveAwards(scene, scene.GameStatisticalData.ZhanDuiIDWin);
                                GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
                                EscapeBattleFuBenData fuBenData;
                                if (this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)scene.GameId, out fuBenData))
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("魔界大逃杀跨服副本GameID={0},战斗结束", fuBenData.GameID), null, true);
                                }
                            }
                            else if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_AWARD)
                            {
                                if (ticks >= scene.m_lLeaveTime)
                                {
                                    copyMap.SetRemoveTicks(scene.m_lLeaveTime);
                                    scene.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_CLEAR;
                                    this.RuntimeData.GameStateQueue.Enqueue(new KeyValuePair<int, int>(scene.GameId, (int)scene.m_eStatus));
                                    try
                                    {
                                        List<GameClient> objsList = copyMap.GetClientsList();
                                        if (objsList != null && objsList.Count > 0)
                                        {
                                            for (int i = 0; i < objsList.Count; i++)
                                            {
                                                GameClient c = objsList[i];
                                                if (c != null)
                                                {
                                                    KuaFuManager.getInstance().GotoLastMap(c);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        DataHelper.WriteExceptionLogEx(ex, "魔界大逃杀系统清场调度异常");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000216 RID: 534 RVA: 0x00022774 File Offset: 0x00020974
        public void CheckUpdateSafeArea(EscapeBattleScene scene, long nowMs)
        {
            if (scene.ScoreData.AreaChangeTm.Ticks / 10000L <= nowMs)
            {
                EscapeMapSafeArea areaInfo = null;
                EscapeMapSafeArea areaInfoTar = null;
                lock (this.RuntimeData.Mutex)
                {
                    if (scene.SafeAreaRefreshState >= this.RuntimeData.EscapeMapSafeAreaList.Count)
                    {
                        return;
                    }
                    scene.SafeAreaRefreshState++;
                    int SafeAreaIdx = Math.Min(scene.SafeAreaRefreshState, this.RuntimeData.EscapeMapSafeAreaList.Count - 1);
                    areaInfo = this.RuntimeData.EscapeMapSafeAreaList[SafeAreaIdx];
                    int SafeAreaIdxTar = Math.Min(scene.SafeAreaRefreshState + 1, this.RuntimeData.EscapeMapSafeAreaList.Count - 1);
                    areaInfoTar = this.RuntimeData.EscapeMapSafeAreaList[SafeAreaIdxTar];
                    if (SafeAreaIdx == SafeAreaIdxTar && scene.ScoreData.safeArea.AreaID == scene.ScoreData.targetSafeArea.AreaID)
                    {
                        return;
                    }
                }
                scene.ScoreData.safeArea.AreaID = scene.ScoreData.targetSafeArea.AreaID;
                scene.ScoreData.safeArea.PosX = scene.ScoreData.targetSafeArea.PosX;
                scene.ScoreData.safeArea.PosY = scene.ScoreData.targetSafeArea.PosY;
                int PosX = scene.ScoreData.safeArea.PosX;
                int PosY = scene.ScoreData.safeArea.PosY;
                this.RandomOnePointInCircle(PosX, PosY, 0, areaInfo.SafeRadius - areaInfoTar.SafeRadius, ref PosX, ref PosY);
                scene.ScoreData.targetSafeArea.AreaID = areaInfoTar.ID;
                scene.ScoreData.targetSafeArea.PosX = PosX;
                scene.ScoreData.targetSafeArea.PosY = PosY;
                scene.ScoreData.AreaChangeTm = new DateTime(nowMs * 10000L + (long)areaInfo.TimeStage * 10000000L);
                this.BroadStateInfoAndScoreInfo(scene.CopyMap, -1, false, true);
            }
        }

        // Token: 0x06000217 RID: 535 RVA: 0x000229D0 File Offset: 0x00020BD0
        private void CheckSceneToAssState(EscapeBattleScene scene, long nowMs)
        {
            if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_FIGHT)
            {
                int alivecount = 0;
                List<GameClient> lsClients = scene.CopyMap.GetClientsList();
                if (lsClients == null || lsClients.Count <= 0)
                {
                    this.ProcessEnd(scene, true, nowMs);
                }
                else
                {
                    foreach (GameClient client in lsClients)
                    {
                        if (client.ClientData.HideGM <= 0)
                        {
                            EscapeBattleRoleInfo clientContextData = client.SceneContextData2 as EscapeBattleRoleInfo;
                            if (client.ClientData.CurrentLifeV > 0 || clientContextData.ReliveCount > 0)
                            {
                                alivecount++;
                            }
                        }
                    }
                    bool chgtoass = false;
                    lock (this.RuntimeData.Mutex)
                    {
                        if (alivecount < scene.SceneInfo.FanaticismStartNum)
                        {
                            chgtoass = true;
                        }
                    }
                    if (chgtoass)
                    {
                        scene.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_ASS;
                        scene.StateTimeData.State = (int)scene.m_eStatus;
                        this.InitCreateDynamicMonster(scene, nowMs);
                        this.BroadStateInfoAndScoreInfo(scene.CopyMap, -1, true, true);
                    }
                }
            }
        }

        // Token: 0x06000218 RID: 536 RVA: 0x00022B50 File Offset: 0x00020D50
        private void InitSceneBuffOnFightAss(EscapeBattleScene scene, long nowMs)
        {
            if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_FIGHT)
            {
                GameClient TopClient = null;
                int CombatForce = 0;
                foreach (KeyValuePair<int, GameClient> kvp in scene.ClientDict)
                {
                    GameClient c = kvp.Value;
                    if (c != null && c.ClientData.HideGM == 0 && c.ClientData.CombatForce > CombatForce)
                    {
                        CombatForce = c.ClientData.CombatForce;
                        TopClient = c;
                    }
                }
                if (null != TopClient)
                {
                    double TopMaxLifeV = RoleAlgorithm.GetExtProp(TopClient, 13);
                    double TopMaxAttack = RoleAlgorithm.GetExtProp(TopClient, 8);
                    double TopMinAttackt = RoleAlgorithm.GetExtProp(TopClient, 7);
                    double TopMaxMAttack = RoleAlgorithm.GetExtProp(TopClient, 10);
                    double TopMinMAttack = RoleAlgorithm.GetExtProp(TopClient, 9);
                    double TopAttack = Math.Max(TopMaxAttack, TopMinAttackt);
                    TopAttack = Math.Max(TopAttack, TopMaxMAttack);
                    TopAttack = Math.Max(TopAttack, TopMinMAttack);
                    double TopMaxDefense = RoleAlgorithm.GetExtProp(TopClient, 4);
                    double TopMinDefense = RoleAlgorithm.GetExtProp(TopClient, 3);
                    double TopMaxMDefense = RoleAlgorithm.GetExtProp(TopClient, 6);
                    double TopMinMDefense = RoleAlgorithm.GetExtProp(TopClient, 5);
                    double TopDefense = Math.Max(TopMaxDefense, TopMinDefense);
                    TopDefense = Math.Max(TopDefense, TopMaxMDefense);
                    TopDefense = Math.Max(TopDefense, TopMinMDefense);
                    double TopHitV = RoleAlgorithm.GetExtProp(TopClient, 18);
                    double TopDodge = RoleAlgorithm.GetExtProp(TopClient, 19);
                    for (int i = 0; i < scene.TopClientCalExtProps.Length; i++)
                    {
                        scene.TopClientCalExtProps[i][13] = TopMaxLifeV * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][8] = TopAttack * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][7] = TopAttack * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][10] = TopAttack * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][9] = TopAttack * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][4] = TopDefense * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][3] = TopDefense * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][6] = TopDefense * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][5] = TopDefense * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][18] = TopHitV * this.RuntimeData.BuffAttributeProportion[i];
                        scene.TopClientCalExtProps[i][19] = TopDodge * this.RuntimeData.BuffAttributeProportion[i];
                    }
                }
            }
            if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_FIGHT)
            {
                List<GameClient> objsList = scene.CopyMap.GetClientsList();
                if (objsList != null && objsList.Count > 0)
                {
                    for (int j = 0; j < objsList.Count; j++)
                    {
                        GameClient c = objsList[j];
                        if (c != null)
                        {
                            lock (this.RuntimeData.Mutex)
                            {
                                this.UpdateBuff4GameClient(c, BufferItemTypes.EscapeBattleDevil, this.RuntimeData.DevilLossNum);
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000219 RID: 537 RVA: 0x00022F68 File Offset: 0x00021168
        private void InitCreateDynamicMonster(EscapeBattleScene scene, long nowMs)
        {
            List<EscapeBattleCollection> collectionList = scene.CollectionConfigList.FindAll((EscapeBattleCollection x) => x.eState == scene.m_eStatus);
            if (null != collectionList)
            {
                foreach (EscapeBattleCollection item in collectionList)
                {
                    long refreshMs = nowMs + (long)(item.RefreshTime * 1000);
                    this.AddDelayCreateMonster(scene, refreshMs, item);
                }
            }
        }

        // Token: 0x0600021A RID: 538 RVA: 0x00023010 File Offset: 0x00021210
        private void AddDelayCreateMonster(EscapeBattleScene scene, long ticks, object monster)
        {
            lock (this.RuntimeData.Mutex)
            {
                List<object> list = null;
                if (!scene.CreateMonsterQueue.TryGetValue(ticks, out list))
                {
                    list = new List<object>();
                    scene.CreateMonsterQueue.Add(ticks, list);
                }
                list.Add(monster);
            }
        }

        // Token: 0x0600021B RID: 539 RVA: 0x0002308C File Offset: 0x0002128C
        public void CheckDeleteDynamicMonster(EscapeBattleScene scene, bool chgState, long nowMs)
        {
            CopyMap copyMap = scene.CopyMap;
            List<object> objList = GameManager.MonsterMgr.GetCopyMapIDMonsterList(copyMap.CopyMapID);
            objList = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, objList, false);
            if (null != objList)
            {
                int i = 0;
                while (i < objList.Count)
                {
                    Monster monster = objList[i] as Monster;
                    if (null != monster)
                    {
                        EscapeBattleCollection tagInfo = monster.Tag as EscapeBattleCollection;
                        if (monster.MonsterType != 1001 && null != tagInfo)
                        {
                            bool killMonster = false;
                            if (chgState && scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_FIGHT && tagInfo.cType == EscapeBCollectType.EBCT_LifeSeed)
                            {
                                killMonster = true;
                            }
                            if (tagInfo.CollectLiveTime > 0)
                            {
                                long birthTicks = monster.GetMonsterBirthTick() / 10000L;
                                long nowTicks = TimeUtil.NOW();
                                if ((nowTicks - birthTicks) / 1000L > (long)tagInfo.CollectLiveTime)
                                {
                                    killMonster = true;
                                }
                            }
                            if (killMonster)
                            {
                                Global.SystemKillMonster(monster);
                            }
                        }
                    }
                    IL_11A:
                    i++;
                    continue;
                    goto IL_11A;
                }
            }
        }

        // Token: 0x0600021C RID: 540 RVA: 0x000231CC File Offset: 0x000213CC
        public void CheckCreateDynamicMonster(EscapeBattleScene scene, long nowMs)
        {
            lock (this.RuntimeData.Mutex)
            {
                while (scene.CreateMonsterQueue.Count > 0)
                {
                    KeyValuePair<long, List<object>> pair = scene.CreateMonsterQueue.First<KeyValuePair<long, List<object>>>();
                    if (nowMs < pair.Key)
                    {
                        break;
                    }
                    try
                    {
                        foreach (object obj in pair.Value)
                        {
                            if (obj is EscapeBattleCollection)
                            {
                                EscapeBattleCollection item = obj as EscapeBattleCollection;
                                int PosX = 0;
                                int PosY = 0;
                                if (this.RandomOnePointInArea(scene, item.RefreshRegion, ref PosX, ref PosY, ObjectTypes.OT_MONSTER))
                                {
                                    GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.RefreshMonsterId, scene.CopyMapId, item.RefreshMonsterNum, PosX / scene.MapGridWidth, PosY / scene.MapGridHeight, 3, 0, SceneUIClasses.EscapeBattle, item, null);
                                }
                            }
                        }
                    }
                    finally
                    {
                        scene.CreateMonsterQueue.RemoveAt(0);
                    }
                }
            }
        }

        // Token: 0x0600021D RID: 541 RVA: 0x00023368 File Offset: 0x00021568
        public void RandomOnePointInCircle(int centerX, int centerY, int radiusMin, int radiusMax, ref int PosX, ref int PosY)
        {
            PosX = centerX;
            PosY = centerY;
            int RandomRadius = Global.GetRandomNumber(radiusMin, radiusMax);
            double RandomAngle = 6.2831853071795862 * Global.GetRandom();
            PosX += (int)(Math.Cos(RandomAngle) * (double)RandomRadius);
            PosY += (int)(Math.Sin(RandomAngle) * (double)RandomRadius);
        }

        // Token: 0x0600021E RID: 542 RVA: 0x000233F0 File Offset: 0x000215F0
        public bool RandomOnePointInArea(EscapeBattleScene scene, int RefreshRegion, ref int PosX, ref int PosY, ObjectTypes objType = ObjectTypes.OT_MONSTER)
        {
            EscapeMapSafeArea areaInfo;
            lock (this.RuntimeData.Mutex)
            {
                if (1 == RefreshRegion)
                {
                    areaInfo = this.RuntimeData.EscapeMapSafeAreaList[0];
                }
                else
                {
                    areaInfo = this.RuntimeData.EscapeMapSafeAreaList.Find((EscapeMapSafeArea x) => x.ID == scene.ScoreData.safeArea.AreaID);
                }
            }
            bool result;
            if (null == areaInfo)
            {
                result = false;
            }
            else
            {
                int trytimes = 0;
                for (; ; )
                {
                    if (1 == RefreshRegion)
                    {
                        PosX = areaInfo.StartSafePoint[0];
                        PosY = areaInfo.StartSafePoint[1];
                    }
                    else
                    {
                        PosX = scene.ScoreData.safeArea.PosX;
                        PosY = scene.ScoreData.safeArea.PosY;
                    }
                    int RadiusMin = 0;
                    int RadiusMax;
                    if (1 == RefreshRegion || 2 == RefreshRegion)
                    {
                        RadiusMax = areaInfo.SafeRadius;
                    }
                    else
                    {
                        RadiusMin = areaInfo.SafeRadius;
                        RadiusMax = (int)((double)areaInfo.SafeRadius * 1.5);
                    }
                    this.RandomOnePointInCircle(PosX, PosY, RadiusMin, RadiusMax, ref PosX, ref PosY);
                    if (!Global.InObsByGridXY(objType, scene.m_nMapCode, PosX / scene.MapGridWidth, PosY / scene.MapGridHeight, 0, 0))
                    {
                        break;
                    }
                    trytimes++;
                    if (trytimes > 100)
                    {
                        goto Block_7;
                    }
                }
                return true;
                Block_7:
                result = false;
            }
            return result;
        }

        // Token: 0x0600021F RID: 543 RVA: 0x00023624 File Offset: 0x00021824
        private void ProcessEnd(EscapeBattleScene scene, bool overTime, long nowTicks)
        {
            if (scene.m_eStatus < EscapeBattleGameSceneStatuses.STATUS_END)
            {
                if (!overTime)
                {
                    EscapeBattleTeamInfo winZhanDui = scene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.RankNum == 0 || x.RankNum == 1);
                    if (null != winZhanDui)
                    {
                        winZhanDui.RankNum = 1;
                        scene.GameStatisticalData.ZhanDuiIDWin = winZhanDui.TeamID;
                    }
                    List<GameClient> winZhanDuiClientList = scene.CopyMap.GetClientsList();
                    if (winZhanDuiClientList != null && winZhanDuiClientList.Count > 0)
                    {
                        winZhanDuiClientList = winZhanDuiClientList.FindAll((GameClient x) => x.ClientData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin);
                        foreach (GameClient client in winZhanDuiClientList)
                        {
                            if (client.ClientData.HideGM <= 0)
                            {
                                EscapeBattleRoleInfo clientContextData = client.SceneContextData2 as EscapeBattleRoleInfo;
                                if (client.ClientData.CurrentLifeV > 0 || clientContextData.ReliveCount > 0)
                                {
                                    scene.GameStatisticalData.WinZhanDuiAliveCount++;
                                }
                            }
                        }
                    }
                }
                scene.m_eStatus = EscapeBattleGameSceneStatuses.STATUS_END;
                scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearSeconds * 1000);
                scene.StateTimeData.GameType = 37;
                scene.StateTimeData.State = 7;
                scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
            }
        }

        // Token: 0x06000220 RID: 544 RVA: 0x00023864 File Offset: 0x00021A64
        public void BroadStateInfoAndScoreInfo(CopyMap copyMap, int specZhanDui = -1, bool timeState = true, bool sideScore = true)
        {
            List<GameClient> objsList = copyMap.GetClientsList();
            if (objsList != null && objsList.Count > 0)
            {
                for (int i = 0; i < objsList.Count; i++)
                {
                    GameClient c = objsList[i];
                    if (specZhanDui <= 0 || c.ClientData.ZhanDuiID == specZhanDui)
                    {
                        if (c != null && c.ClientData.CopyMapID == copyMap.CopyMapID)
                        {
                            this.NotifyTimeStateInfoAndScoreInfo(c, timeState, sideScore);
                        }
                    }
                }
            }
        }

        // Token: 0x06000221 RID: 545 RVA: 0x000238F8 File Offset: 0x00021AF8
        public void NotifySpriteInjured(GameClient client)
        {
            EscapeBattleScene scene = client.SceneObject as EscapeBattleScene;
            if (null != scene)
            {
                this.BroadStateInfoAndScoreInfo(scene.CopyMap, client.ClientData.ZhanDuiID, false, true);
            }
        }

        // Token: 0x06000222 RID: 546 RVA: 0x000239B0 File Offset: 0x00021BB0
        public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
        {
            lock (this.RuntimeData.Mutex)
            {
                EscapeBattleScene scene;
                if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
                {
                    if (timeState)
                    {
                        client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
                    }
                    if (sideScore)
                    {
                        EscapeBattleRoleInfo clientContextData = client.SceneContextData2 as EscapeBattleRoleInfo;
                        EscapeBattleSideScore ScoreData = scene.ScoreData.Clone();
                        ScoreData.eStatus = (EscapeBattleGameSceneStatuses)scene.StateTimeData.State;
                        ScoreData.ReliveCount = clientContextData.ReliveCount;
                        List<EscapeBattleRoleInfo> clientContextDataList;
                        if (scene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out clientContextDataList))
                        {
                            ScoreData.BattleRoleList = clientContextDataList.FindAll((EscapeBattleRoleInfo x) => x.OnLine && x.RoleID != client.ClientData.RoleID);
                            using (List<EscapeBattleRoleInfo>.Enumerator enumerator = ScoreData.BattleRoleList.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    EscapeBattleRoleInfo role = enumerator.Current;
                                    List<GameClient> objsList = scene.CopyMap.GetClientsList();
                                    if (objsList != null && objsList.Count >= 0)
                                    {
                                        GameClient other = objsList.Find((GameClient x) => x.ClientData.RoleID == role.RoleID);
                                        if (null != other)
                                        {
                                            role.LifeV = other.ClientData.CurrentLifeV;
                                            role.MaxLifeV = other.ClientData.LifeV;
                                            if (other.ClientData.HideGM > 0)
                                            {
                                                role.LifeV = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        client.sendCmd<EscapeBattleSideScore>(2113, ScoreData, false);
                    }
                }
            }
        }

        // Token: 0x06000223 RID: 547 RVA: 0x00023CC4 File Offset: 0x00021EC4
        public void PushGameResultData(EscapeBattleScene scene, int zhanduiId)
        {
            List<EscapeBattleRoleInfo> clientContextDataList;
            if (scene.ClientContextDataDict.TryGetValue(zhanduiId, out clientContextDataList))
            {
                foreach (EscapeBattleRoleInfo contextData in clientContextDataList)
                {
                    GameClient client = null;
                    if (scene.ClientDict.TryGetValue(contextData.RoleID, out client))
                    {
                        List<int> roleAnalysisData = this.GetEscapeBattleRoleAnalysisData(client);
                        if (null != roleAnalysisData)
                        {
                            List<int> list;
                            (list = roleAnalysisData)[2] = list[2] + contextData.KillRoleNum;
                            if (contextData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
                            {
                                (list = roleAnalysisData)[3] = list[3] + 1;
                            }
                        }
                        this.SaveEscapeBattleRoleAnalysisData(client, roleAnalysisData);
                        client = GameManager.ClientMgr.FindClient(contextData.RoleID);
                        if (null != client)
                        {
                            GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_EscapeRoleKill, new int[0]));
                        }
                    }
                }
            }
            List<int> ZhanDuiIDVsModJiFen = new List<int>();
            TianTi5v5ZhanDuiData zhanduiData;
            if (scene.GameStatisticalData.ZhanDuiDict.TryGetValue(zhanduiId, out zhanduiData))
            {
                int ServerID;
                if (scene.GameStatisticalData.ZhanDuiIDVsServerIDDict.TryGetValue(zhanduiId, out ServerID))
                {
                    EscapeBattleTeamInfo teamInfo = scene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == zhanduiData.ZhanDuiID);
                    if (null != teamInfo)
                    {
                        EscapeBDuanAwardsConfig awardConfig = this.GetEscapeBattleAwardConfigByJiFen(zhanduiData.EscapeJiFen);
                        if (null != awardConfig)
                        {
                            int EscapeJiFenOld = zhanduiData.EscapeJiFen;
                            if (zhanduiData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
                            {
                                List<int> AliveVsJiFen = awardConfig.WinRankValue.Find((List<int> x) => x[0] == scene.GameStatisticalData.WinZhanDuiAliveCount);
                                if (null != AliveVsJiFen)
                                {
                                    zhanduiData.EscapeJiFen += AliveVsJiFen[1];
                                }
                            }

                            else
                            {
                                int leaveNum = scene.ScoreData.BattleTeamList.Count - teamInfo.RankNum + 1;
                                List<int> LeaveNumVsJiFen;
                                if (teamInfo.RankNum == 0)
                                {
                                    LeaveNumVsJiFen = awardConfig.LoseRankValue[awardConfig.LoseRankValue.Count - 1];
                                }
                                else
                                {
                                    LeaveNumVsJiFen = awardConfig.LoseRankValue.Find((List<int> x) => x[0] == leaveNum);
                                }
                                if (null != LeaveNumVsJiFen)
                                {
                                    zhanduiData.EscapeJiFen -= LeaveNumVsJiFen[1];
                                }
                            }
                            zhanduiData.EscapeJiFen = Math.Max(zhanduiData.EscapeJiFen, 0);
                            zhanduiData.EscapeLastFightTime = TimeUtil.NowDateTime();
                            ZhanDuiIDVsModJiFen.Add(zhanduiId);
                            ZhanDuiIDVsModJiFen.Add(zhanduiData.EscapeJiFen);
                            TianTi5v5Manager.getInstance().UpdateEscapeZhanDuiData2DB(zhanduiData, ServerID);
                        }
                    }
                }
            }
            KeyValuePair<int, List<int>> resultPair = new KeyValuePair<int, List<int>>(scene.GameId, ZhanDuiIDVsModJiFen);
            this.RuntimeData.PKResultQueue.Enqueue(resultPair);
        }

        // Token: 0x06000224 RID: 548 RVA: 0x000240EC File Offset: 0x000222EC
        public void GiveAwards(EscapeBattleScene scene, int zhanduiId)
        {
            try
            {
                using (Dictionary<int, List<EscapeBattleRoleInfo>>.Enumerator enumerator = scene.ClientContextDataDict.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<int, List<EscapeBattleRoleInfo>> kvp = enumerator.Current;
                        bool flag;
                        if (zhanduiId > 0)
                        {
                            KeyValuePair<int, List<EscapeBattleRoleInfo>> kvp3 = kvp;
                            flag = (kvp3.Key == zhanduiId);
                        }
                        else
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            EscapeBattleTeamInfo teamInfo = scene.ScoreData.BattleTeamList.Find(delegate (EscapeBattleTeamInfo x)
                            {
                                int teamID = x.TeamID;
                                KeyValuePair<int, List<EscapeBattleRoleInfo>> kvp2 = kvp;
                                return teamID == kvp2.Key;
                            });
                            if (null != teamInfo)
                            {
                                if (zhanduiId > 0 || teamInfo.RankNum == 0)
                                {
                                    Dictionary<int, TianTi5v5ZhanDuiData> zhanDuiDict = scene.GameStatisticalData.ZhanDuiDict;
                                    KeyValuePair<int, List<EscapeBattleRoleInfo>> kvp3 = kvp;
                                    TianTi5v5ZhanDuiData zhanduiData;
                                    if (zhanDuiDict.TryGetValue(kvp3.Key, out zhanduiData))
                                    {
                                        kvp3 = kvp;
                                        this.PushGameResultData(scene, kvp3.Key);
                                        kvp3 = kvp;
                                        foreach (EscapeBattleRoleInfo contextData in kvp3.Value)
                                        {
                                            int success;
                                            if (contextData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
                                            {
                                                success = 1;
                                            }
                                            else
                                            {
                                                success = 0;
                                            }
                                            GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleID);
                                            if (client != null && client.ClientData.MapCode == scene.m_nMapCode)
                                            {
                                                this.NtfCanGetAward(client, success, scene, zhanduiData, teamInfo);
                                                this.GiveRoleAwards(client, success, scene, zhanduiData, teamInfo, false);
                                            }
                                            else
                                            {
                                                scene.ClientDict.TryGetValue(contextData.RoleID, out client);
                                                this.GiveRoleAwards(client, success, scene, zhanduiData, teamInfo, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "魔界大逃杀系统清场调度异常");
            }
        }

        // Token: 0x06000225 RID: 549 RVA: 0x000243B4 File Offset: 0x000225B4
        private void NtfCanGetAward(GameClient client, int success, EscapeBattleScene scene, TianTi5v5ZhanDuiData zhanduiData, EscapeBattleTeamInfo teamInfo)
        {
            EscapeBattleRoleInfo contextData = client.SceneContextData2 as EscapeBattleRoleInfo;
            EscapeBDuanAwardsConfig awardConfig = this.GetEscapeBattleAwardConfigByJiFen(zhanduiData.EscapeJiFen);
            if (awardConfig != null && null != contextData)
            {
                EscapeBattleAwardsData awardsData = new EscapeBattleAwardsData();
                awardsData.Success = success;
                awardsData.RankNum = teamInfo.RankNum;
                awardsData.AwardID = awardConfig.ID;
                awardsData.ZhanDuiKillNum = teamInfo.ZhanDuiKillNum;
                List<int> roleAnalysisData = this.GetEscapeBattleRoleAnalysisData(client);
                if (null != roleAnalysisData)
                {
                    awardsData.WinToDay = roleAnalysisData[3];
                }
                if (success > 0)
                {
                    List<int> AliveVsJiFen = awardConfig.WinRankValue.Find((List<int> x) => x[0] == scene.GameStatisticalData.WinZhanDuiAliveCount);
                    if (null != AliveVsJiFen)
                    {
                        awardsData.ModJiFen = AliveVsJiFen[1];
                    }
                }
                else
                {
                    int leaveNum = scene.ScoreData.BattleTeamList.Count - teamInfo.RankNum + 1;
                    List<int> LeaveNumVsJiFen;
                    if (teamInfo.RankNum == 0)
                    {
                        LeaveNumVsJiFen = awardConfig.LoseRankValue[awardConfig.LoseRankValue.Count - 1];
                    }
                    else
                    {
                        LeaveNumVsJiFen = awardConfig.LoseRankValue.Find((List<int> x) => x[0] == leaveNum);
                    }
                    if (null != LeaveNumVsJiFen)
                    {
                        awardsData.ModJiFen = -LeaveNumVsJiFen[1];
                    }
                }
                client.sendCmd<EscapeBattleAwardsData>(2111, awardsData, false);
            }
        }

        // Token: 0x06000226 RID: 550 RVA: 0x0002456C File Offset: 0x0002276C
        private int GiveRoleAwards(GameClient client, int success, EscapeBattleScene scene, TianTi5v5ZhanDuiData zhanduiData, EscapeBattleTeamInfo teamInfo, bool froceMail)
        {
            EscapeBattleRoleInfo contextData = client.SceneContextData2 as EscapeBattleRoleInfo;
            EscapeBDuanAwardsConfig awardConfig = this.GetEscapeBattleAwardConfigByJiFen(zhanduiData.EscapeJiFen);
            int result;
            if (awardConfig == null || null == contextData)
            {
                result = -5;
            }
            else
            {
                List<AwardsItemData> awardsItemDataList;
                string sContent;
                if (success > 0)
                {
                    awardsItemDataList = (awardConfig.WinAwardsItemList as AwardsItemList).Items;
                    sContent = GLang.GetLang(8009, new object[0]);
                }
                else
                {
                    awardsItemDataList = (awardConfig.LoseAwardsItemList as AwardsItemList).Items;
                    sContent = GLang.GetLang(8010, new object[0]);
                }
                string sSubject = "魔界大逃杀奖励";
                List<AwardsItemData> AllAwardsItemDataList = new List<AwardsItemData>();
                int RoleWinToDay = 0;
                List<int> roleAnalysisData = this.GetEscapeBattleRoleAnalysisData(client);
                if (null != roleAnalysisData)
                {
                    RoleWinToDay = roleAnalysisData[3];
                }
                if (awardConfig.FirstWinAwardsItemList != null && success > 0 && 1 == RoleWinToDay)
                {
                    AllAwardsItemDataList.AddRange((awardConfig.FirstWinAwardsItemList as AwardsItemList).Items);
                }
                else
                {
                    AllAwardsItemDataList.AddRange(awardsItemDataList);
                }
                int BagInt;
                if (AllAwardsItemDataList != null && (froceMail || !RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, AllAwardsItemDataList, out BagInt)))
                {
                    Global.UseMailGivePlayerAward2(client, AllAwardsItemDataList, GLang.GetLang(8008, new object[0]), sContent, 0, 0, 0);
                }
                else if (AllAwardsItemDataList != null)
                {
                    foreach (AwardsItemData item in AllAwardsItemDataList)
                    {
                        Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, sSubject, "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
                    }
                }
                this.SetSeasonRankDataFlags(client, true, false);
                result = 1;
            }
            return result;
        }

        // Token: 0x06000227 RID: 551 RVA: 0x00024790 File Offset: 0x00022990
        public bool ClientRelive(GameClient client)
        {
            EscapeBattleScene scene = client.SceneObject as EscapeBattleScene;
            bool result;
            if (scene == null || client.ClientData.HideGM > 0 || client.ClientData.CurrentLifeV > 0)
            {
                result = false;
            }
            else
            {
                EscapeBattleRoleInfo clientContextData = client.SceneContextData2 as EscapeBattleRoleInfo;
                bool escapeRelive = false;
                if (clientContextData.ReliveCount > 0)
                {
                    escapeRelive = true;
                }
                int toPosX = 0;
                int toPosY = 0;
                if (escapeRelive)
                {
                    if (!this.RandomOnePointInArea(scene, 2, ref toPosX, ref toPosY, ObjectTypes.OT_CLIENT))
                    {
                        return true;
                    }
                }
                else
                {
                    toPosX = client.ClientData.PosX;
                    toPosY = client.ClientData.PosY;
                }
                lock (this.RuntimeData.Mutex)
                {
                    if (escapeRelive)
                    {
                        clientContextData.ReliveCount--;
                        this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
                        this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleDevil, this.RuntimeData.DevilLossNum);
                    }
                    else
                    {
                        lock (VideoLogic.getInstance().Mutex)
                        {
                            client.ClientData.GuanZhanGM = 1;
                            client.ClientData.HideGM = 1;
                            List<int> tempTrackingRoleIDList = new List<int>(client.ClientData.TrackingRoleIDList);
                            foreach (int rid in tempTrackingRoleIDList)
                            {
                                GameClient tClient = GameManager.ClientMgr.FindClient(rid);
                                if (null != tClient)
                                {
                                    VideoLogic.getInstance().CancleTracking(tClient, true);
                                    VideoLogic.getInstance().TryTrackingOther(tClient, client);
                                }
                            }
                        }
                    }
                    client.ClientData.CurrentLifeV = client.ClientData.LifeV;
                    client.ClientData.CurrentMagicV = client.ClientData.MagicV;
                    client.ClientData.MoveAndActionNum = 0;
                    GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, toPosX, toPosY, -1);
                    Global.ClientRealive(client, toPosX, toPosY, -1);
                    if (!escapeRelive)
                    {
                        List<object> objsList = Global.GetAll9Clients(client);
                        GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, objsList);
                    }
                }
                this.BroadStateInfoAndScoreInfo(scene.CopyMap, client.ClientData.ZhanDuiID, false, true);
                result = true;
            }
            return result;
        }

        // Token: 0x06000228 RID: 552 RVA: 0x00024AA0 File Offset: 0x00022CA0
        private bool GetBirthPoint(int mapCode, int side, out int toPosX, out int toPosY)
        {
            toPosX = -1;
            toPosY = -1;
            GameMap gameMap = null;
            bool result;
            if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
            {
                result = false;
            }
            else
            {
                int defaultBirthPosX = this.RuntimeData.MapBirthPointDict[1 + side % this.RuntimeData.MapBirthPointDict.Count].PosX;
                int defaultBirthPosY = this.RuntimeData.MapBirthPointDict[1 + side % this.RuntimeData.MapBirthPointDict.Count].PosY;
                int defaultBirthRadius = this.RuntimeData.MapBirthPointDict[1 + side % this.RuntimeData.MapBirthPointDict.Count].BirthRadius;
                Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
                toPosX = (int)newPos.X;
                toPosY = (int)newPos.Y;
                result = true;
            }
            return result;
        }

        // Token: 0x06000229 RID: 553 RVA: 0x00024BB4 File Offset: 0x00022DB4
        public void OnKillRole(GameClient client, GameClient other)
        {
            lock (this.RuntimeData.Mutex)
            {
                EscapeBattleScene scene;
                if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
                {
                    if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_FIGHT || scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_ASS)
                    {
                        EscapeBattleRoleInfo clientContextData = client.SceneContextData2 as EscapeBattleRoleInfo;
                        clientContextData.KillRoleNum++;
                        EscapeBattleTeamInfo killTeamInfo = scene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
                        if (null != killTeamInfo)
                        {
                            killTeamInfo.ZhanDuiKillNum++;
                        }
                        long BufferVal = 0L;
                        BufferData bufferData = Global.GetBufferDataByID(other, 2090002);
                        if (null != bufferData)
                        {
                            BufferVal = bufferData.BufferVal;
                        }
                        this.UpdateBuff4GameClient(other, BufferItemTypes.EscapeBattleDevil, -(int)BufferVal);
                        this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleDevil, (int)BufferVal);
                        double addLife = (double)this.RuntimeData.KillReplyHp;
                        GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, addLife, "魔界大逃杀击杀");
                        this.BroadStateInfoAndScoreInfo(scene.CopyMap, -1, false, true);
                        this.CheckZhanDuiWinLoseState(scene);
                    }
                }
            }
        }

        // Token: 0x0600022A RID: 554 RVA: 0x00024DC0 File Offset: 0x00022FC0
        public void CheckZhanDuiWinLoseState(EscapeBattleScene scene)
        {
            lock (this.RuntimeData.Mutex)
            {
                if (scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_FIGHT || scene.m_eStatus == EscapeBattleGameSceneStatuses.STATUS_ASS)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (scene.ScoreData.BattleTeamList.Count == 1)
                    {
                        this.ProcessEnd(scene, false, nowTicks);
                    }
                    else
                    {
                        foreach (EscapeBattleTeamInfo zhandui in scene.ScoreData.BattleTeamList)
                        {
                            if (zhandui.RankNum <= 0)
                            {
                                bool lucky = false;
                                List<EscapeBattleRoleInfo> clientContextDataList;
                                if (scene.ClientContextDataDict.TryGetValue(zhandui.TeamID, out clientContextDataList))
                                {
                                    using (List<EscapeBattleRoleInfo>.Enumerator enumerator2 = clientContextDataList.GetEnumerator())
                                    {
                                        while (enumerator2.MoveNext())
                                        {
                                            EscapeBattleRoleInfo role = enumerator2.Current;
                                            if (role.OnLine)
                                            {
                                                List<GameClient> objsList = scene.CopyMap.GetClientsList();
                                                if (objsList != null && objsList.Count > 0)
                                                {
                                                    GameClient client = objsList.Find((GameClient x) => x.ClientData.RoleID == role.RoleID);
                                                    if (client != null && client.ClientData.HideGM <= 0)
                                                    {
                                                        if (client.ClientData.CurrentLifeV > 0 || role.ReliveCount > 0)
                                                        {
                                                            lucky = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!lucky)
                                {
                                    zhandui.RankNum = scene.ScoreData.BattleTeamList.FindAll((EscapeBattleTeamInfo x) => x.RankNum == 0).Count;
                                    if (zhandui.RankNum <= 2)
                                    {
                                        this.ProcessEnd(scene, false, nowTicks);
                                    }
                                    if (zhandui.RankNum > 1)
                                    {
                                        this.GiveAwards(scene, zhandui.TeamID);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0600022B RID: 555 RVA: 0x00025098 File Offset: 0x00023298
        public void RoleLeaveFuBen(GameClient client)
        {
            EscapeBattleScene scene = client.SceneObject as EscapeBattleScene;
            if (null != scene)
            {
                EscapeBattleRoleInfo clientLianShaContextData = client.SceneContextData2 as EscapeBattleRoleInfo;
                clientLianShaContextData.OnLine = false;
                this.CheckZhanDuiWinLoseState(scene);
            }
        }

        // Token: 0x0600022C RID: 556 RVA: 0x0002510C File Offset: 0x0002330C
        public void OnCaiJiFinish(GameClient client, Monster monster)
        {
            lock (this.RuntimeData.Mutex)
            {
                EscapeBattleScene scene;
                if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
                {
                    if (scene.m_eStatus != EscapeBattleGameSceneStatuses.STATUS_PREPARE)
                    {
                        EscapeBattleCollection monsterItem = monster.Tag as EscapeBattleCollection;
                        if (monsterItem != null)
                        {
                            if (monsterItem.IsDeath >= 0)
                            {
                                this.AddDelayCreateMonster(scene, TimeUtil.NOW(), monsterItem);
                            }
                            if (EscapeBCollectType.EBCT_God == monsterItem.cType)
                            {
                                this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleGod, monsterItem.CollectGodNum);
                            }
                            else if (EscapeBCollectType.EBCT_LifeSeed == monsterItem.cType)
                            {
                                EscapeBattleTeamInfo TeamInfo = scene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
                                if (null != TeamInfo)
                                {
                                    TeamInfo.LifeSeed++;
                                    this.RefreshTeamMemberReliveCount(scene, TeamInfo);
                                    this.BroadStateInfoAndScoreInfo(scene.CopyMap, -1, false, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0600022D RID: 557 RVA: 0x000252DC File Offset: 0x000234DC
        public void RefreshTeamMemberReliveCount(EscapeBattleScene scene, EscapeBattleTeamInfo TeamInfo)
        {
            List<int[]> SeedVsNumList;
            lock (this.RuntimeData.Mutex)
            {
                SeedVsNumList = this.RuntimeData.LifeSeedNum.FindAll((int[] x) => x[0] <= TeamInfo.LifeSeed);
            }
            if (null != SeedVsNumList)
            {
                List<EscapeBattleRoleInfo> clientContextDataList;
                if (scene.ClientContextDataDict.TryGetValue(TeamInfo.TeamID, out clientContextDataList))
                {
                    foreach (EscapeBattleRoleInfo role in clientContextDataList)
                    {
                        role.ReliveCount = SeedVsNumList.Sum((int[] x) => x[1]);
                        role.ReliveCount = Math.Min(this.RuntimeData.MaxLifeNum, role.ReliveCount);
                    }
                }
            }
        }

        // Token: 0x0600022E RID: 558 RVA: 0x00025420 File Offset: 0x00023620
        public int GetCaiJiMonsterTime(GameClient client, Monster monster)
        {
            EscapeBattleCollection tag = (monster != null) ? (monster.Tag as EscapeBattleCollection) : null;
            int result;
            if (tag == null)
            {
                result = -200;
            }
            else
            {
                result = tag.CollectTime;
            }
            return result;
        }

        // Token: 0x0600022F RID: 559 RVA: 0x00025498 File Offset: 0x00023698
        private void CheckSceneAreaDamage(EscapeBattleScene scene, long nowTicks)
        {
            if (scene.AreaDamageTicks <= nowTicks)
            {
                EscapeMapSafeArea areaInfo = this.RuntimeData.EscapeMapSafeAreaList.Find((EscapeMapSafeArea x) => x.ID == scene.ScoreData.safeArea.AreaID);
                if (null != areaInfo)
                {
                    scene.AreaDamageTicks = nowTicks + (long)(areaInfo.GodFireHitTime * 1000);
                    List<GameClient> lsClients = scene.CopyMap.GetClientsList();
                    lsClients = Global.GetMapAliveClientsEx(lsClients, scene.m_nMapCode, false, 0L);
                    for (int i = 0; i < lsClients.Count; i++)
                    {
                        GameClient client = lsClients[i];
                        if (client != null && client.ClientData.HideGM <= 0)
                        {
                            Point center = new Point
                            {
                                X = (double)scene.ScoreData.safeArea.PosX,
                                Y = (double)scene.ScoreData.safeArea.PosY
                            };
                            if (Global.GetTwoPointDistance(client.CurrentPos, center) > (double)areaInfo.SafeRadius)
                            {
                                double hurtPct = areaInfo.GodFireHitPercent;
                                double realHurtPct = hurtPct * (1.0 - this.CalClientDehurtValue(scene, client));
                                double hurtValue = (double)client.ClientData.LifeV * realHurtPct + (double)areaInfo.GodFireHitHp;
                                int v = client.ClientData.CurrentLifeV;
                                client.ClientData.CurrentLifeV -= (int)hurtValue;
                                hurtValue = (double)(v - client.ClientData.CurrentLifeV);
                                if (hurtValue <= 0.0)
                                {
                                    break;
                                }
                                GameManager.ClientMgr.SubSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, hurtValue);
                                GameManager.ClientMgr.NotifySpriteInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.RoleID, client.ClientData.RoleID, 0, (int)hurtValue, (double)client.ClientData.CurrentLifeV, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
                                bool AnyOneDead = client.ClientData.CurrentLifeV <= 0;
                                if (AnyOneDead)
                                {
                                    this.CheckZhanDuiWinLoseState(scene);
                                    long BufferVal = 0L;
                                    BufferData bufferData = Global.GetBufferDataByID(client, 2090002);
                                    if (null != bufferData)
                                    {
                                        BufferVal = bufferData.BufferVal;
                                    }
                                    this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleDevil, -(int)BufferVal);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000230 RID: 560 RVA: 0x00025784 File Offset: 0x00023984
        private double CalClientDehurtValue(EscapeBattleScene scene, GameClient client)
        {
            return 0.0;
        }

        // Token: 0x06000231 RID: 561 RVA: 0x000257A4 File Offset: 0x000239A4
        private void UpdateBuff4GameClient(GameClient client, BufferItemTypes bufferItem, int modNum)
        {
            if (modNum != 0)
            {
                long BufferVal = 0L;
                BufferData bufferData = Global.GetBufferDataByID(client, (int)bufferItem);
                if (null != bufferData)
                {
                    BufferVal = bufferData.BufferVal;
                }
                BufferVal = Math.Max(0L, BufferVal + (long)modNum);
                lock (this.RuntimeData.Mutex)
                {
                    if (BufferItemTypes.EscapeBattleGod == bufferItem)
                    {
                        BufferVal = Math.Min(BufferVal, (long)this.RuntimeData.BuffMaxLayerNum[0]);
                    }
                    if (BufferItemTypes.EscapeBattleDevil == bufferItem)
                    {
                        BufferVal = Math.Min(BufferVal, (long)this.RuntimeData.BuffMaxLayerNum[1]);
                    }
                }
                double[] actionParams = new double[]
                {
                    (double)BufferVal
                };
                Global.UpdateBufferData(client, bufferItem, actionParams, 1, false);
                this.RefreshBuffGameClientProps(client, bufferItem, BufferVal);
            }
        }

        // Token: 0x06000232 RID: 562 RVA: 0x000258A0 File Offset: 0x00023AA0
        private void RefreshBuffGameClientProps(GameClient client, BufferItemTypes bufferItem, long BufferVal)
        {
            EscapeBattleScene scene = client.SceneObject as EscapeBattleScene;
            if (null != scene)
            {
                EscapeBattlePropNotify ebProp = new EscapeBattlePropNotify();
                double[] CalExtProps = new double[177];
                ebProp.MergeProp = new Dictionary<int, double[]>();
                for (int i = 0; i < 177; i++)
                {
                    if (BufferItemTypes.EscapeBattleGod == bufferItem)
                    {
                        CalExtProps[i] = scene.TopClientCalExtProps[0][i] * (double)BufferVal;
                    }
                    if (BufferItemTypes.EscapeBattleDevil == bufferItem)
                    {
                        CalExtProps[i] = scene.TopClientCalExtProps[1][i] * (double)BufferVal;
                    }
                }
                if (BufferItemTypes.EscapeBattleGod == bufferItem)
                {
                    ebProp.Type = 0;
                    if (!ebProp.MergeProp.ContainsKey(ebProp.Type))
                    {
                        ebProp.MergeProp.Add(ebProp.Type, CalExtProps);
                    }
                    else
                    {
                        ebProp.MergeProp[ebProp.Type] = CalExtProps;
                    }
                }
                else
                {
                    ebProp.Type = 1;
                    if (!ebProp.MergeProp.ContainsKey(ebProp.Type))
                    {
                        ebProp.MergeProp.Add(ebProp.Type, CalExtProps);
                    }
                    else
                    {
                        ebProp.MergeProp[ebProp.Type] = CalExtProps;
                    }
                }
                client.ClientData.PurePropsCacheManager.SetExtProps(new object[]
                {
                    PropsSystemTypes.BufferPropsManager,
                    (int)bufferItem,
                    CalExtProps
                });
                GameManager.ClientMgr.NotifyUpdateEscapeBattleProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, ebProp);
                client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
                {
                    DelayExecProcIds.RecalcProps,
                    DelayExecProcIds.NotifyRefreshProps
                });
            }
        }

        // Token: 0x06000233 RID: 563 RVA: 0x00025A60 File Offset: 0x00023C60
        public static EscapeBattleManager getInstance()
        {
            return EscapeBattleManager.instance;
        }

        // Token: 0x06000234 RID: 564 RVA: 0x00025A78 File Offset: 0x00023C78
        public bool initialize()
        {
            return this.InitConfig();
        }

        // Token: 0x06000235 RID: 565 RVA: 0x00025A9C File Offset: 0x00023C9C
        public bool initialize(ICoreInterface coreInterface)
        {
            ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("EscapeBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 2000);
            return true;
        }

        // Token: 0x06000236 RID: 566 RVA: 0x00025ADC File Offset: 0x00023CDC
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(2112, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(2116, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(2117, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(2110, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(2114, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(2111, 1, 1, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(2115, 1, 1, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(2119, 1, 1, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            GlobalEventSource.getInstance().registerListener(10, EscapeBattleManager.getInstance());
            GlobalEventSource.getInstance().registerListener(13, EscapeBattleManager.getInstance());
            GlobalEventSource.getInstance().registerListener(28, EscapeBattleManager.getInstance());
            GlobalEventSource.getInstance().registerListener(12, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(21, 58, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(61, 10007, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(62, 10007, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(60, 59, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10002, 59, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(63, 10000, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(65, 10000, EscapeBattleManager.getInstance());
            this.NotifyEnterHandler = new EventSourceEx<KFCallMsg>.HandlerData
            {
                ID = 59,
                EventType = 10037,
                Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
            };
            this.NotifyGameStateHandler = new EventSourceEx<KFCallMsg>.HandlerData
            {
                ID = 59,
                EventType = 10038,
                Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
            };
            KFCallManager.MsgSource.registerListener(10037, this.NotifyEnterHandler);
            KFCallManager.MsgSource.registerListener(10038, this.NotifyGameStateHandler);
            return true;
        }

        // Token: 0x06000237 RID: 567 RVA: 0x00025D24 File Offset: 0x00023F24
        public bool showdown()
        {
            GlobalEventSource.getInstance().removeListener(10, EscapeBattleManager.getInstance());
            GlobalEventSource.getInstance().removeListener(13, EscapeBattleManager.getInstance());
            GlobalEventSource.getInstance().registerListener(28, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(21, 58, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(61, 10007, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(62, 10007, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(60, 59, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10002, 59, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(63, 10000, EscapeBattleManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(65, 10000, EscapeBattleManager.getInstance());
            KFCallManager.MsgSource.removeListener(10037, this.NotifyEnterHandler);
            return true;
        }

        // Token: 0x06000238 RID: 568 RVA: 0x00025E20 File Offset: 0x00024020
        public bool destroy()
        {
            return true;
        }

        // Token: 0x06000239 RID: 569 RVA: 0x00025E34 File Offset: 0x00024034
        public bool processCmd(GameClient client, string[] cmdParams)
        {
            return false;
        }

        // Token: 0x0600023A RID: 570 RVA: 0x00025E48 File Offset: 0x00024048
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            switch (nID)
            {
                case 2110:
                    return this.ProcessEscapeEnterCmd(client, nID, bytes, cmdParams);
                case 2112:
                    return this.ProcessGetMainInfoListCmd(client, nID, bytes, cmdParams);
                case 2114:
                    return this.ProcessEscapeRankInfoCmd(client, nID, bytes, cmdParams);
                case 2115:
                    return this.ProcessGetPaiHangAwardsCmd(client, nID, bytes, cmdParams);
                case 2116:
                    return this.ProcessEscapeJoinCmd(client, nID, bytes, cmdParams);
                case 2117:
                    return this.ProcessEscapeInviteCmd(client, nID, bytes, cmdParams);
                case 2119:
                    return this.ProcessEscapeDevilBuyCmd(client, nID, bytes, cmdParams);
            }
            return true;
        }

        // Token: 0x0600023B RID: 571 RVA: 0x00025EF8 File Offset: 0x000240F8
        public void processEvent(EventObject eventObject)
        {
            int eventType = eventObject.getEventType();
            if (eventType == 10)
            {
                PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
                if (null != playerDeadEvent)
                {
                    if (playerDeadEvent.Type == PlayerDeadEventTypes.ByRole)
                    {
                        this.OnKillRole(playerDeadEvent.getAttackerRole(), playerDeadEvent.getPlayer());
                    }
                }
            }
            else if (eventType == 28)
            {
                OnStartPlayGameEventObject e = eventObject as OnStartPlayGameEventObject;
                if (null != e)
                {
                    this.OnStartPlayGame(e.Client);
                }
            }
            else if (eventObject.getEventType() == 13)
            {
                PlayerLeaveFuBenEventObject eventObj = (PlayerLeaveFuBenEventObject)eventObject;
                this.RoleLeaveFuBen(eventObj.getPlayer());
            }
            else if (eventObject.getEventType() == 12)
            {
                PlayerLogoutEventObject eventObj2 = (PlayerLogoutEventObject)eventObject;
                if (eventObj2.getPlayer().ClientData.SceneType == SceneUIClasses.EscapeReady)
                {
                    this.CancleJoinState(eventObj2.getPlayer());
                }
            }
        }

        // Token: 0x0600023C RID: 572 RVA: 0x00026004 File Offset: 0x00024204
        public void processEvent(EventObjectEx eventObject)
        {
            int eventType = eventObject.EventType;
            if (eventType != 21)
            {
                switch (eventType)
                {
                    case 60:
                        this.NotifyTimeStateInfoAndScoreInfo(eventObject.Sender as GameClient, true, true);
                        break;
                    case 61:
                        {
                            EventObjectEx_I1 data = eventObject as EventObjectEx_I1;
                            if (data != null && data.Param1 == 37)
                            {
                                eventObject.Handled = true;
                                if (this.OnKuaFuLogin(eventObject.Sender as KuaFuServerLoginData))
                                {
                                    eventObject.Result = true;
                                }
                            }
                            break;
                        }
                    case 62:
                        {
                            EventObjectEx_I1 data = eventObject as EventObjectEx_I1;
                            if (data != null && data.Param1 == 37)
                            {
                                eventObject.Handled = true;
                                if (this.OnKuaFuInitGame(eventObject.Sender as GameClient))
                                {
                                    eventObject.Handled = true;
                                    eventObject.Result = true;
                                }
                            }
                            break;
                        }
                    case 63:
                        {
                            PreZhanDuiChangeMemberEventObject eventObj = (PreZhanDuiChangeMemberEventObject)eventObject;
                            eventObj.Handled = this.OnPreZhanDuiChangeMember(eventObj);
                            break;
                        }
                    case 64:
                        break;
                    case 65:
                        if (null != eventObject)
                        {
                            if ((int)eventObject.Args[1] == this.RuntimeData.ReadyMapCode && (int)eventObject.Args[2] != this.RuntimeData.ReadyMapCode)
                            {
                                this.CancleJoinState(eventObject.Args[0] as GameClient);
                            }
                        }
                        break;
                    default:
                        if (eventType == 10002)
                        {
                            CaiJiEventObject e = eventObject as CaiJiEventObject;
                            if (null != e)
                            {
                                GameClient client = e.Source as GameClient;
                                Monster monster = e.Target as Monster;
                                this.OnCaiJiFinish(client, monster);
                                eventObject.Handled = true;
                                eventObject.Result = true;
                            }
                        }
                        break;
                }
            }
            else
            {
                PreGotoLastMapEventObject data2 = eventObject as PreGotoLastMapEventObject;
                if (data2 != null && data2.SceneType == 58)
                {
                    this.CancleJoinState(data2.Player);
                }
            }
        }

        // Token: 0x0600023D RID: 573 RVA: 0x00026220 File Offset: 0x00024420
        public bool KFCallMsgFunc(KFCallMsg msg)
        {
            switch (msg.KuaFuEventType)
            {
                case 10037:
                    if (!GameManager.IsKuaFuServer)
                    {
                        EscapeBattleNtfEnterData data = msg.Get<EscapeBattleNtfEnterData>();
                        if (null != data)
                        {
                            string zhanduiIdArray = string.Join<int>("|", data.ZhanDuiIDList.ToArray());
                            LogManager.WriteLog(LogTypes.Error, string.Format("通知战队:{0} 拥有进入魔界大逃杀资格", zhanduiIdArray), null, true);
                            DateTime fightTime = TimeUtil.NowDateTime().Date.Add(this.GetStartTime(0)).AddSeconds((double)(this.RuntimeData.Config.MatchConfigList[0].WaitSeconds + this.RuntimeData.Config.MatchConfigList[0].SafeSecs));
                            DateTime endTime = fightTime.AddSeconds((double)this.RuntimeData.Config.MatchConfigList[0].BattleEndTime);
                            foreach (int zhanDuiID in data.ZhanDuiIDList)
                            {
                                TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, GameManager.ServerId);
                                if (null != zhanDuiData)
                                {
                                    lock (this.RuntimeData.Mutex)
                                    {
                                        EscapeBattlePiPeiState piPeiState;
                                        if (this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out piPeiState))
                                        {
                                            piPeiState.GameID = data.GameId;
                                            piPeiState.State = 3;
                                            piPeiState.FightTime = fightTime;
                                            piPeiState.EndTime = endTime;
                                        }
                                    }
                                    foreach (TianTi5v5ZhanDuiRoleData role in zhanDuiData.teamerList)
                                    {
                                        GameClient c = GameManager.ClientMgr.FindClient(role.RoleID);
                                        if (c != null && c.ClientData.SceneMapCode == this.RuntimeData.ReadyMapCode)
                                        {
                                            this.ProcessEscapeEnterCmd(c, 2110, null, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 10038:
                    lock (this.RuntimeData.Mutex)
                    {
                        int[] data2 = msg.Get<int[]>();
                        if (data2 != null && data2.Length >= 3)
                        {
                            lock (this.RuntimeData.Mutex)
                            {
                                EscapeBattlePiPeiState piPeiState;
                                if (this.RuntimeData.ConfirmBattleDict.TryGetValue(data2[0], out piPeiState) && piPeiState.GameID == data2[1])
                                {
                                    if (piPeiState.State >= 3)
                                    {
                                        piPeiState.State = data2[2];
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            return true;
        }

        // Token: 0x0600023E RID: 574 RVA: 0x00026674 File Offset: 0x00024874
        public bool InitConfig()
        {
            bool success = true;
            string fileName = "";
            lock (this.RuntimeData.Mutex)
            {
                try
                {
                    this.RuntimeData.TeamBattleMap = GameManager.systemParamsList.GetParamValueIntArrayByName("EscapeTeamApply", ',');
                    Dictionary<int, MapSettingItem> dict = Data.SettingsDict.Value;
                    if (null != dict)
                    {
                        foreach (MapSettingItem item in dict.Values)
                        {
                            if (item.MapType == 58)
                            {
                                this.RuntimeData.ReadyMapCode = item.Code;
                                break;
                            }
                        }
                    }
                    if (!this.RuntimeData.Config.Load(Global.GameResPath("Config\\EscapeActivityRules.xml")))
                    {
                        return false;
                    }
                    EscapeBattleMatchConfig config = this.RuntimeData.Config.MatchConfigList[0];
                    EscapeBattleConsts.MinZhanDuiNumPerGame = config.MatchTeamNum;
                    EscapeBattleConsts.MinRoleNumPerGame = config.EnterBattleNum;
                    EscapeBattleConsts.BattleSignSecs = config.BattleSignSecs;
                    this.RuntimeData.AwardsConfig = new List<EscapeBattleAwardsConfig>();
                    fileName = "Config/EscapeRankAward.xml";
                    string fullPathFileName = Global.GameResPath(fileName);
                    XElement xml = XElement.Load(fullPathFileName);
                    IEnumerable<XElement> nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        EscapeBattleAwardsConfig item2 = new EscapeBattleAwardsConfig();
                        item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item2.MinRank = (int)ConfigHelper.GetElementAttributeValueLong(node, "StarRank", 0L);
                        item2.MaxRank = (int)ConfigHelper.GetElementAttributeValueLong(node, "EndRank", 0L);
                        if (item2.MinRank == -1)
                        {
                            item2.MinRank = 1;
                        }
                        if (item2.MaxRank == -1)
                        {
                            item2.MaxRank = int.MaxValue;
                        }
                        string str = Global.GetSafeAttributeStr(node, "Award");
                        if (!string.IsNullOrEmpty(str))
                        {
                            ConfigParser.ParseAwardsItemList(str, ref item2.Award, '|', ',');
                        }
                        this.RuntimeData.AwardsConfig.Add(item2);
                    }
                    this.RuntimeData.DuanAwardsConfig = new List<EscapeBDuanAwardsConfig>();
                    fileName = "Config/EscapeDanList.xml";
                    fullPathFileName = Global.GameResPath(fileName);
                    xml = XElement.Load(fullPathFileName);
                    nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        EscapeBDuanAwardsConfig item3 = new EscapeBDuanAwardsConfig();
                        item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item3.RankValue = (int)Global.GetSafeAttributeLong(node, "RankValue");
                        item3.WinRankValue = ConfigHelper.ParserIntArrayList(Global.GetSafeAttributeStr(node, "WinRankValue"), true, '|', ',');
                        item3.LoseRankValue = ConfigHelper.ParserIntArrayList(Global.GetSafeAttributeStr(node, "LoseRankValue"), true, '|', ',');
                        item3.LoseRankValue.Sort(delegate (List<int> left, List<int> right)
                        {
                            int result;
                            if (left[0] < right[0])
                            {
                                result = -1;
                            }
                            else if (left[0] > right[0])
                            {
                                result = 1;
                            }
                            else
                            {
                                result = 0;
                            }
                            return result;
                        });
                        item3.RankLevelName = Global.GetSafeAttributeStr(node, "RankLevelName");
                        AwardsItemList FirstWinAwardsItemList = new AwardsItemList();
                        item3.FirstWinAwardsItemList = FirstWinAwardsItemList;
                        ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "FirstWinRankReward"), ref FirstWinAwardsItemList, '|', ',');
                        AwardsItemList WinAwardsItemList = new AwardsItemList();
                        item3.WinAwardsItemList = WinAwardsItemList;
                        ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "WinRankReward"), ref WinAwardsItemList, '|', ',');
                        AwardsItemList LoseAwardsItemList = new AwardsItemList();
                        item3.LoseAwardsItemList = LoseAwardsItemList;
                        ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "LoseRankReward"), ref LoseAwardsItemList, '|', ',');
                        this.RuntimeData.DuanAwardsConfig.Add(item3);
                    }
                    this.RuntimeData.MapBirthPointDict = new Dictionary<int, EscapeBattleBirthPoint>();
                    fileName = "Config/EscapePlayPoint.xml";
                    fullPathFileName = Global.GameResPath(fileName);
                    xml = XElement.Load(fullPathFileName);
                    nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        EscapeBattleBirthPoint item4 = new EscapeBattleBirthPoint();
                        item4.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item4.MapCodeID = (int)Global.GetSafeAttributeLong(node, "MapId");
                        string[] strFields = Global.GetSafeAttributeStr(node, "MapTeamPoint").Split(new char[]
                        {
                            ','
                        });
                        if (strFields.Length == 2)
                        {
                            item4.PosX = Global.SafeConvertToInt32(strFields[0]);
                            item4.PosY = Global.SafeConvertToInt32(strFields[1]);
                        }
                        item4.BirthRadius = (int)Global.GetSafeAttributeLong(node, "MapTeamRange");
                        this.RuntimeData.MapBirthPointDict[item4.ID] = item4;
                    }
                    this.RuntimeData.CollectionConfigList = new List<EscapeBattleCollection>();
                    fileName = "Config/EscapeMapCollection.xml";
                    fullPathFileName = Global.GameResPath(fileName);
                    xml = XElement.Load(fullPathFileName);
                    nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        EscapeBattleCollection item5 = new EscapeBattleCollection();
                        item5.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item5.MapCodeID = (int)Global.GetSafeAttributeLong(node, "MapId");
                        item5.cType = (EscapeBCollectType)Global.GetSafeAttributeLong(node, "CollectType");
                        item5.eState = (EscapeBattleGameSceneStatuses)Global.GetSafeAttributeLong(node, "CollectRefreshStage");
                        item5.RefreshRegion = (int)Global.GetSafeAttributeLong(node, "RefreshRegion");
                        item5.RefreshTime = (int)Global.GetSafeAttributeLong(node, "RefreshTime");
                        item5.RefreshMonsterId = (int)Global.GetSafeAttributeLong(node, "RefreshMonsterId");
                        item5.RefreshMonsterNum = (int)Global.GetSafeAttributeLong(node, "RefreshMonsterNum");
                        item5.CollectTime = (int)Global.GetSafeAttributeLong(node, "CollectTime");
                        item5.CollectGodNum = (int)Global.GetSafeAttributeLong(node, "CollectGodNum");
                        item5.CollectLiveTime = (int)Global.GetSafeAttributeLong(node, "CollectLiveTime");
                        item5.IsDeath = (int)Global.GetSafeAttributeLong(node, "IsDeath");
                        this.RuntimeData.CollectionConfigList.Add(item5);
                    }
                    this.RuntimeData.EscapeMapSafeAreaList = new List<EscapeMapSafeArea>();
                    fileName = "Config/EscapeMapSafeArea.xml";
                    fullPathFileName = Global.GameResPath(fileName);
                    xml = XElement.Load(fullPathFileName);
                    nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        EscapeMapSafeArea item6 = new EscapeMapSafeArea();
                        item6.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item6.eState = (EscapeBattleGameSceneStatuses)Global.GetSafeAttributeLong(node, "RefreshStage");
                        item6.TimeStage = (int)Global.GetSafeAttributeLong(node, "TimeStage");
                        item6.StartSafePoint = Global.GetSafeAttributeIntArray(node, "StartSafePoint", -1, '|');
                        item6.SafeRadius = (int)Global.GetSafeAttributeLong(node, "SafeRadius");
                        item6.GodFireHitTime = (int)Global.GetSafeAttributeLong(node, "GodFireHitTime");
                        item6.GodFireHitPercent = Global.GetSafeAttributeDouble(node, "GodFireHitPercent");
                        item6.GodFireHitHp = (int)Global.GetSafeAttributeLong(node, "GodFireHitHp");
                        this.RuntimeData.EscapeMapSafeAreaList.Add(item6);
                    }
                    this.RuntimeData.EscapeMapSafeAreaList.Sort(delegate (EscapeMapSafeArea left, EscapeMapSafeArea right)
                    {
                        int result;
                        if (left.SafeRadius > right.SafeRadius)
                        {
                            result = -1;
                        }
                        else if (left.SafeRadius < right.SafeRadius)
                        {
                            result = 1;
                        }
                        else
                        {
                            result = 0;
                        }
                        return result;
                    });
                    this.RuntimeData.BuyDevilLossDiamonds = (int)GameManager.systemParamsList.GetParamValueIntByName("BuyDevilLossDiamonds", -1);
                    this.RuntimeData.KillReplyHp = (int)GameManager.systemParamsList.GetParamValueIntByName("KillReplyHp", -1);
                    this.RuntimeData.BuffMaxLayerNum = GameManager.systemParamsList.GetParamValueIntArrayByName("BuffMaxLayerNum", '|');
                    this.RuntimeData.BuffAttributeProportion = GameManager.systemParamsList.GetParamValueDoubleArrayByName("BuffAttributeProportion", '|');
                    this.RuntimeData.BuffAttributeType = GameManager.systemParamsList.GetParamValueIntArrayByName("BuffAttributeType", ',');
                    this.RuntimeData.LifeSeedNum = new List<int[]>();
                    List<string> LifeSeedNumLifeSeedNum = GameManager.systemParamsList.GetParamValueStringListByName("LifeSeedNum", '|');
                    foreach (string item7 in LifeSeedNumLifeSeedNum)
                    {
                        this.RuntimeData.LifeSeedNum.Add(Global.StringArray2IntArray(item7.Split(new char[]
                        {
                            ','
                        })));
                    }
                    this.RuntimeData.MaxLifeNum = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxLifeNum", -1);
                    this.RuntimeData.DevilLossNum = (int)GameManager.systemParamsList.GetParamValueIntByName("DevilLossNum", -1);
                    this.RuntimeData.ReadyMapCode = (int)GameManager.systemParamsList.GetParamValueIntByName("TeamEnterMap", -1);
                    DateTime.TryParse(GameManager.systemParamsList.GetParamValueByName("EscapeStartTime"), out this.RuntimeData.EscapeStartTime);
                    List<string> strDebuffExtProps = GameManager.systemParamsList.GetParamValueStringListByName("EscapeAttribute", '|');
                    foreach (string item7 in strDebuffExtProps)
                    {
                        double[] array = Global.String2DoubleArray(item7, ',');
                        this.RuntimeData.DebuffCalExtProps[(int)array[0]] = array[1];
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
                }
            }
            return success;
        }

        // Token: 0x0600023F RID: 575 RVA: 0x00027194 File Offset: 0x00025394
        public bool ProcessGetMainInfoListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                int timeOffset = 0;
                int zhanDuiID = client.ClientData.ZhanDuiID;
                int rank = 0;
                if (!this.CheckOpenState(TimeUtil.NowDateTime()))
                {
                    result = 6;
                }
                else
                {
                    EscapeBattleAwardsConfig duanWeiRankAward = null;
                    rank = this.CanGetMonthRankAwards(client, out duanWeiRankAward);
                    DateTime now = TimeUtil.NowDateTime().Add(this.RuntimeData.DiffTimeSpan);
                    TimeSpan nowTs = TimeUtil.TimeOfWeek(now);
                    lock (this.RuntimeData.Mutex)
                    {
                        if (Consts.TestMode)
                        {
                            timeOffset = (int)(this.RuntimeData.Config.MatchConfigList[0].TimePoints[0] - nowTs).TotalSeconds % 604800;
                            result = 1;
                        }
                        else
                        {
                            timeOffset = (int)this.RuntimeData.DiffTimeSpan.TotalSeconds % 7 * 86400;
                            foreach (EscapeBattleMatchConfig config in this.RuntimeData.Config.MatchConfigList)
                            {
                                for (int i = 0; i < config.TimePoints.Count - 1; i += 2)
                                {
                                    if (config.TimePoints[i] <= nowTs && nowTs < config.TimePoints[i + 1])
                                    {
                                        result = 1;
                                        break;
                                    }
                                }
                            }
                        }
                        EscapeBattlePiPeiState piPeiState;
                        if (this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out piPeiState))
                        {
                            if (piPeiState.State == 2)
                            {
                                result = piPeiState.State;
                            }
                            else if (piPeiState.State == 3)
                            {
                                ReturnValue<int> rt = TcpCall.EscapeBattle_K.GetZhanDuiState(client.ClientData.ZhanDuiID);
                                if (rt.IsReturn)
                                {
                                    piPeiState.State = rt.Value;
                                    if (rt.Value != 3)
                                    {
                                        piPeiState.GameID = 0;
                                    }
                                    if (piPeiState.State != 0)
                                    {
                                        result = rt.Value;
                                    }
                                }
                                else
                                {
                                    result = 0;
                                }
                            }
                        }
                    }
                }
                client.sendCmd(nID, string.Format("{0}:{1}:{2}", timeOffset, result, rank), false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000240 RID: 576 RVA: 0x00027540 File Offset: 0x00025740
        public bool ProcessEscapeJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                if (this.IsGongNengOpened(client, false))
                {
                    int zhanDuiID = client.ClientData.ZhanDuiID;
                    if (zhanDuiID <= 0)
                    {
                        result = -4013;
                    }
                    else if (client.ClientData.ZhanDuiZhiWu != 1)
                    {
                        result = -4016;
                    }
                    else if (!this.CheckMap(client))
                    {
                        result = -21;
                    }
                    else if (!this.CheckTime())
                    {
                        result = -2001;
                    }
                    else
                    {
                        EscapeBattlePiPeiState piPeiState = null;
                        lock (this.RuntimeData.Mutex)
                        {
                            if (!this.RuntimeData.TeamBattleMap.Contains(client.ClientData.MapCode))
                            {
                                result = -21;
                                goto IL_316;
                            }
                            EscapeBattleMatchConfig config = this.RuntimeData.Config.MatchConfigList[0];
                            if (config.SignCondition == null)
                            {
                                result = -3;
                                goto IL_316;
                            }
                            if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out piPeiState))
                            {
                                piPeiState = new EscapeBattlePiPeiState();
                                this.RuntimeData.ConfirmBattleDict[zhanDuiID] = piPeiState;
                            }
                            if (piPeiState.State >= 2)
                            {
                                result = -5;
                                goto IL_316;
                            }
                            TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
                            if (!Consts.TestMode && zhanDuiData.teamerList.Count < config.EnterBattleNum)
                            {
                                result = -4026;
                                goto IL_316;
                            }
                            if (!Consts.TestMode && zhanDuiData.teamerList.Any(delegate (TianTi5v5ZhanDuiRoleData x)
                            {
                                int unionLevel = Global.GetUnionLevel(x.ZhuanSheng, x.Level, false);
                                int needLevel = Global.GetUnionLevel(config.SignCondition[0], config.SignCondition[1], false);
                                bool result2;
                                if (unionLevel < needLevel)
                                {
                                    result2 = true;
                                }
                                else
                                {
                                    GameClient c = GameManager.ClientMgr.FindClient(x.RoleID);
                                    result2 = (c != null && Global.GetUnionLevel(c, false) < needLevel);
                                }
                                return result2;
                            }))
                            {
                                result = -19;
                                goto IL_316;
                            }
                            piPeiState.RoleList = new List<EscapeBattleJoinRoleInfo>();
                            foreach (TianTi5v5ZhanDuiRoleData role in zhanDuiData.teamerList)
                            {
                                EscapeBattleJoinRoleInfo rd = new EscapeBattleJoinRoleInfo
                                {
                                    RoleID = role.RoleID,
                                    RoleName = role.RoleName,
                                    Level = role.Level,
                                    ChangeLevel = role.ZhuanSheng,
                                    CombatForce = role.ZhanLi,
                                    IsLeader = (role.RoleID == zhanDuiData.LeaderRoleID)
                                };
                                piPeiState.RoleList.Add(rd);
                                if (rd.IsLeader)
                                {
                                    rd.Join = true;
                                }
                            }
                            piPeiState.EscapeJiFen = zhanDuiData.EscapeJiFen;
                            piPeiState.State = 2;
                        }
                        GameManager.ClientMgr.BroadZhanDuiMessage<List<EscapeBattleJoinRoleInfo>>(2118, piPeiState.RoleList, zhanDuiID);
                        Global.GotoMap(client, this.RuntimeData.ReadyMapCode);
                    }
                }
                IL_316:
                client.sendCmd<int>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000241 RID: 577 RVA: 0x000278E4 File Offset: 0x00025AE4
        public bool ProcessEscapeInviteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                if (this.IsGongNengOpened(client, false))
                {
                    int zhanDuiID = client.ClientData.ZhanDuiID;
                    if (zhanDuiID <= 0)
                    {
                        result = -4013;
                    }
                    else if (client.ClientData.ZhanDuiZhiWu != 1)
                    {
                        result = -4016;
                    }
                    else if (!this.CheckMap(client))
                    {
                        result = -21;
                    }
                    else if (!this.CheckTime())
                    {
                        result = -2001;
                    }
                    else
                    {
                        EscapeBattlePiPeiState piPeiState = null;
                        lock (this.RuntimeData.Mutex)
                        {
                            if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out piPeiState))
                            {
                                result = -4035;
                                goto IL_12F;
                            }
                            if (piPeiState.State < 2)
                            {
                                result = -4035;
                                goto IL_12F;
                            }
                            if (piPeiState.State >= 3)
                            {
                                result = -4037;
                                goto IL_12F;
                            }
                        }
                        GameManager.ClientMgr.BroadZhanDuiMessage<List<EscapeBattleJoinRoleInfo>>(2118, piPeiState.RoleList, zhanDuiID);
                    }
                }
                IL_12F:
                client.sendCmd<int>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000242 RID: 578 RVA: 0x00027AB0 File Offset: 0x00025CB0
        public bool ProcessEscapeRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                EscapeBattleRankInfo rankInfo = new EscapeBattleRankInfo();
                int zhanDuiID = client.ClientData.ZhanDuiID;
                if (zhanDuiID > 0)
                {
                    if (!GameManager.IsKuaFuServer)
                    {
                        TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
                        lock (this.RuntimeData.Mutex)
                        {
                            rankInfo.myZhanDuiRankInfo.Key = zhanDuiID;
                            rankInfo.myZhanDuiRankInfo.Param1 = zhanDuiData.ZhanDouLi;
                            rankInfo.myZhanDuiRankInfo.StrParam1 = zhanDuiData.ZhanDuiName;
                            rankInfo.myZhanDuiRankInfo.ZoneID = zhanDuiData.ZoneID;
                            rankInfo.myZhanDuiRankInfo.Value = zhanDuiData.EscapeJiFen;
                            if (this.RuntimeData.SyncData.RankList != null && this.RuntimeData.SyncData.RankList.Count > 0)
                            {
                                rankInfo.SelfRank = this.RuntimeData.SyncData.RankList.FindIndex((KFEscapeRankInfo x) => x.Key == zhanDuiID) + 1;
                                int count = Math.Min(this.RuntimeData.SyncData.RankList.Count, EscapeBattleConsts.MaxRankNum);
                                rankInfo.rankInfo2Client = this.RuntimeData.SyncData.RankList.GetRange(0, count);
                            }
                        }
                    }
                }
                client.sendCmd<EscapeBattleRankInfo>(nID, rankInfo, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000243 RID: 579 RVA: 0x00027CC0 File Offset: 0x00025EC0
        public bool ProcessEscapeEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                if (!this.CheckMap(client))
                {
                    result = -12;
                }
                else
                {
                    int zhanDuiID = client.ClientData.ZhanDuiID;
                    DateTime now = TimeUtil.NowDateTime().Add(this.RuntimeData.DiffTimeSpan);
                    lock (this.RuntimeData.Mutex)
                    {
                        EscapeBattlePiPeiState piPeiState;
                        if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out piPeiState) || piPeiState.State == 0)
                        {
                            result = -4038;
                            goto IL_233;
                        }
                        if (piPeiState.State == 2)
                        {
                            if (client.ClientData.MapCode != this.RuntimeData.ReadyMapCode)
                            {
                                Global.GotoMap(client, this.RuntimeData.ReadyMapCode);
                                goto IL_233;
                            }
                        }
                        else if (piPeiState.State == 3)
                        {
                            if (now > piPeiState.FightTime)
                            {
                                result = -2008;
                                goto IL_233;
                            }
                        }
                        else if (piPeiState.State == 4)
                        {
                            result = -4006;
                            goto IL_233;
                        }
                    }
                    int gameID = 0;
                    int kuafuServerID;
                    string[] ips;
                    int[] ports;
                    ReturnValue<int> rt = TcpCall.EscapeBattle_K.ZhengBaRequestEnter(client.ClientData.ZhanDuiID, out gameID, out kuafuServerID, out ips, out ports);
                    if (rt.Type != ReturnType.Success || rt.Value < 0)
                    {
                        result = rt.Value;
                    }
                    else
                    {
                        KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
                        kuaFuServerLoginData.RoleId = client.ClientData.RoleID;
                        kuaFuServerLoginData.ServerId = client.ServerId;
                        kuaFuServerLoginData.GameType = 37;
                        kuaFuServerLoginData.GameId = (long)gameID;
                        kuaFuServerLoginData.EndTicks = TimeUtil.UTCTicks();
                        kuaFuServerLoginData.TargetServerID = kuafuServerID;
                        kuaFuServerLoginData.ServerIp = ips[0];
                        kuaFuServerLoginData.ServerPort = ports[0];
                        kuaFuServerLoginData.Param1 = client.ClientData.ZhanDuiID;
                        GlobalNew.RecordSwitchKuaFuServerLog(client);
                        client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
                    }
                }
                IL_233:
                client.sendCmd<int>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000244 RID: 580 RVA: 0x00027F68 File Offset: 0x00026168
        public bool ProcessGetPaiHangAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = -20;
                EscapeBattleAwardsConfig duanWeiRankAward = null;
                int rank = this.CanGetMonthRankAwards(client, out duanWeiRankAward);
                if (rank > 0)
                {
                    List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(duanWeiRankAward.Award.Items, -1);
                    if (!Global.CanAddGoodsDataList(client, goodsDataList))
                    {
                        result = -100;
                    }
                    else
                    {
                        this.SetSeasonRankDataFlags(client, false, true);
                        result = 0;
                        for (int i = 0; i < goodsDataList.Count; i++)
                        {
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "天梯月段位排名奖励", "1900-01-01 12:00:00", 0, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, 0, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, 0, null, null, 0, true);
                        }
                    }
                }
                client.sendCmd<int>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000245 RID: 581 RVA: 0x000280E8 File Offset: 0x000262E8
        public bool ProcessEscapeDevilBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                EscapeBattleScene scene = client.SceneObject as EscapeBattleScene;
                if (scene == null || scene.m_eStatus != EscapeBattleGameSceneStatuses.STATUS_ASS)
                {
                    result = -5;
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                long BufferVal = 0L;
                BufferData bufferData = Global.GetBufferDataByID(client, 2090002);
                if (null != bufferData)
                {
                    BufferVal = bufferData.BufferVal;
                }
                int moneyCost = 0;
                lock (this.RuntimeData.Mutex)
                {
                    moneyCost = (int)((long)this.RuntimeData.BuffMaxLayerNum[1] - BufferVal) * this.RuntimeData.BuyDevilLossDiamonds;
                }
                if (moneyCost <= 0)
                {
                    result = -5;
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, moneyCost, "魔界大逃杀购买魔神契约", true, true, false, DaiBiSySType.None))
                {
                    result = -10;
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                lock (this.RuntimeData.Mutex)
                {
                    this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleDevil, this.RuntimeData.BuffMaxLayerNum[1]);
                }
                client.sendCmd<int>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000246 RID: 582 RVA: 0x000282E0 File Offset: 0x000264E0
        public bool IsGongNengOpened(GameClient client, bool hint = false)
        {
            return GlobalNew.IsGongNengOpened(client, GongNengIDs.EscapeBattle, false);
        }

        // Token: 0x06000247 RID: 583 RVA: 0x000282FC File Offset: 0x000264FC
        private bool CheckMap(GameClient client)
        {
            bool result;
            if (client.ClientData.MapCode == this.RuntimeData.ReadyMapCode)
            {
                result = true;
            }
            else
            {
                lock (this.RuntimeData.Mutex)
                {
                    result = this.RuntimeData.TeamBattleMap.Contains(client.ClientData.MapCode);
                }
            }
            return result;
        }

        // Token: 0x06000248 RID: 584 RVA: 0x000283B8 File Offset: 0x000265B8
        public void OnStartPlayGame(GameClient client)
        {
            SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
            DateTime now = TimeUtil.NowDateTime();
            DateTime centerTime = TimeUtil.NowDateTime().Add(this.RuntimeData.DiffTimeSpan);
            lock (this.RuntimeData.Mutex)
            {
                if (client.ClientData.MapCode == this.RuntimeData.ReadyMapCode)
                {
                    int deltaSecs = (int)centerTime.TimeOfDay.TotalSeconds % EscapeBattleConsts.BattleSignSecs;
                    GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();
                    StateTimeData.GameType = 37;
                    StateTimeData.State = 1;
                    StateTimeData.EndTicks = now.AddSeconds((double)(EscapeBattleConsts.BattleSignSecs - deltaSecs)).Ticks / 10000L;
                    client.sendCmd<GameSceneStateTimeData>(827, StateTimeData, false);
                }
                else if (SceneUIClasses.EscapeBattle == sceneType)
                {
                    client.ClientData.PctPropsCacheManager.SetExtProps(new object[]
                    {
                        PropsSystemTypes.BufferPropsManager,
                        9050,
                        this.RuntimeData.DebuffCalExtProps
                    });
                    client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
                    {
                        DelayExecProcIds.RecalcProps,
                        DelayExecProcIds.NotifyRefreshProps
                    });
                    EscapeBattleScene scene = client.SceneObject as EscapeBattleScene;
                    if (null != scene)
                    {
                        EscapeBattleTeamInfo TeamInfo = scene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
                        if (null != TeamInfo)
                        {
                            this.RefreshTeamMemberReliveCount(scene, TeamInfo);
                        }
                    }
                }
            }
        }

        // Token: 0x06000249 RID: 585 RVA: 0x000285D8 File Offset: 0x000267D8
        private bool CheckTime()
        {
            bool result;
            if (Consts.TestMode)
            {
                result = true;
            }
            else
            {
                DateTime now = TimeUtil.NowDateTime();
                lock (this.RuntimeData.Mutex)
                {
                    for (int i = 0; i < this.RuntimeData.Config.MatchConfigList.Count; i++)
                    {
                        EscapeBattleMatchConfig item = this.RuntimeData.Config.MatchConfigList[i];
                        for (int j = 0; j < item.TimePoints.Count; j += 2)
                        {
                            if (now.DayOfWeek == (DayOfWeek)item.TimePoints[j].Days && now.TimeOfDay.TotalSeconds >= item.SecondsOfDay[j] && now.TimeOfDay.TotalSeconds <= item.SecondsOfDay[j + 1])
                            {
                                return true;
                            }
                        }
                    }
                }
                result = false;
            }
            return result;
        }

        // Token: 0x0600024A RID: 586 RVA: 0x00028720 File Offset: 0x00026920
        private TimeSpan GetStartTime(int MapCodeID)
        {
            EscapeBattleMatchConfig sceneItem = null;
            TimeSpan startTime = TimeSpan.MinValue;
            DateTime now = TimeUtil.NowDateTime();
            lock (this.RuntimeData.Mutex)
            {
                sceneItem = this.RuntimeData.Config.MatchConfigList.FirstOrDefault<EscapeBattleMatchConfig>();
                if (null == sceneItem)
                {
                    goto IL_189;
                }
            }
            lock (this.RuntimeData.Mutex)
            {
                for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
                {
                    if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
                    {
                        double nowSeconds = now.TimeOfDay.TotalSeconds + (double)(sceneItem.BattleSignSecs / 4);
                        int fromstartSeconds = (int)(nowSeconds - sceneItem.SecondsOfDay[i]);
                        int startSeconds = (int)nowSeconds - fromstartSeconds % sceneItem.BattleSignSecs;
                        startTime = TimeSpan.FromSeconds((double)startSeconds);
                        break;
                    }
                }
            }
            IL_189:
            if (startTime < TimeSpan.Zero)
            {
                startTime = now.TimeOfDay;
            }
            return startTime;
        }

        // Token: 0x0600024B RID: 587 RVA: 0x000288F8 File Offset: 0x00026AF8
        private bool OnKuaFuLogin(KuaFuServerLoginData data)
        {
            EscapeBattleFuBenData copyInfo = null;
            int zhanDuiID = data.Param1;
            lock (this.RuntimeData.Mutex)
            {
                this.RuntimeData.KuaFuCopyDataDict.TryGetValue(data.GameId, out copyInfo);
            }
            if (null == copyInfo)
            {
                ReturnValue<int> rt = TcpCall.EscapeBattle_K.ZhengBaKuaFuLogin(zhanDuiID, (int)data.GameId, data.ServerId, out copyInfo);
                if (!rt.IsReturn || rt.Value < 0)
                {
                    return false;
                }
                lock (this.RuntimeData.Mutex)
                {
                    if (!this.RuntimeData.KuaFuCopyDataDict.ContainsKey(data.GameId))
                    {
                        this.RuntimeData.KuaFuCopyDataDict[data.GameId] = copyInfo;
                    }
                }
            }
            bool result;
            if (copyInfo != null && GameManager.ServerId == copyInfo.ServerID && copyInfo.SideDict.ContainsKey((long)zhanDuiID))
            {
                data.ips = copyInfo.IPs;
                data.ports = copyInfo.Ports;
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600024C RID: 588 RVA: 0x00028A7C File Offset: 0x00026C7C
        public bool OnKuaFuInitGame(GameClient client)
        {
            int zhanDuiID = client.ClientData.ZhanDuiID;
            int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
            bool result;
            if (gameId <= 0 || zhanDuiID <= 0)
            {
                result = false;
            }
            else
            {
                lock (this.RuntimeData.Mutex)
                {
                    EscapeBattleMatchConfig matchConfig = this.RuntimeData.Config.MatchConfigList.First<EscapeBattleMatchConfig>();
                    EscapeBattleFuBenData copyInfo = null;
                    if (!this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)gameId, out copyInfo))
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("未找到活动KuaFuCopyData数据,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, gameId, matchConfig.MapCode), null, true);
                        result = false;
                    }
                    else
                    {
                        if (copyInfo.FuBenSeqID == 0)
                        {
                            copyInfo.FuBenSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
                        }
                        int toX = 0;
                        int toY = 0;
                        int side = 0;
                        EscapeBattleScene scene;
                        if (!copyInfo.SideDict.TryGetValue((long)client.ClientData.ZhanDuiID, out side))
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("未找到活动阵营数据KuaFuCopyData,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, gameId, matchConfig.MapCode), null, true);
                            result = false;
                        }
                        else if (this.SceneDict.TryGetValue(copyInfo.FuBenSeqID, out scene) && scene.m_eStatus >= EscapeBattleGameSceneStatuses.STATUS_FIGHT)
                        {
                            result = false;
                        }
                        else if (!this.GetBirthPoint(matchConfig.MapCode, side, out toX, out toY))
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("roleid={0},mapcode={1},side={2} 未找到出生点", client.ClientData.RoleID, matchConfig.MapCode, side), null, true);
                            result = false;
                        }
                        else
                        {
                            Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = copyInfo.FuBenSeqID;
                            client.ClientData.MapCode = matchConfig.MapCode;
                            client.ClientData.PosX = toX;
                            client.ClientData.PosY = toY;
                            client.ClientData.FuBenSeqID = copyInfo.FuBenSeqID;
                            client.ClientData.BattleWhichSide = side;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x0600024D RID: 589 RVA: 0x00028D20 File Offset: 0x00026F20
        public void TimerProc(object sender, EventArgs e)
        {
            DateTime now = TimeUtil.NowDateTime();
            DateTime centerTime = TimeUtil.NowDateTime().Add(this.RuntimeData.DiffTimeSpan);
            long nowTicks = TimeUtil.NOW();
            List<GameClient> clientList = GameManager.ClientMgr.GetMapGameClients(this.RuntimeData.ReadyMapCode);
            if (null != clientList)
            {
                lock (this.RuntimeData.Mutex)
                {
                    int startSecs = (int)centerTime.TimeOfDay.TotalSeconds / EscapeBattleConsts.BattleSignSecs;
                    if (startSecs != this.RuntimeData.LastStartSecs)
                    {
                        this.RuntimeData.LastStartSecs = startSecs;
                        int deltaSecs = (int)centerTime.TimeOfDay.TotalSeconds % EscapeBattleConsts.BattleSignSecs;
                        GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();
                        StateTimeData.GameType = 37;
                        StateTimeData.State = 1;
                        StateTimeData.EndTicks = now.AddSeconds((double)EscapeBattleConsts.BattleSignSecs).Ticks / 10000L;
                        foreach (GameClient client in clientList)
                        {
                            client.sendCmd<GameSceneStateTimeData>(827, StateTimeData, false);
                        }
                    }
                    List<int> removedList = new List<int>();
                    foreach (KeyValuePair<int, EscapeBattlePiPeiState> kv in this.RuntimeData.ConfirmBattleDict)
                    {
                        bool ready = false;
                        int readyNum = 0;
                        int zhanDuiID = kv.Key;
                        EscapeBattlePiPeiState piPeiState = kv.Value;
                        if (piPeiState.State <= 2)
                        {
                            List<GameClient> notifyList = new List<GameClient>();
                            using (List<EscapeBattleJoinRoleInfo>.Enumerator enumerator3 = piPeiState.RoleList.GetEnumerator())
                            {
                                while (enumerator3.MoveNext())
                                {
                                    EscapeBattleJoinRoleInfo role = enumerator3.Current;
                                    GameClient c = clientList.Find((GameClient x) => x.ClientData.RoleID == role.RoleID);
                                    if (c != null && c.ClientData.SceneMapCode == this.RuntimeData.ReadyMapCode)
                                    {
                                        notifyList.Add(c);
                                        readyNum++;
                                        role.Join = true;
                                        if (role.IsLeader)
                                        {
                                            ready = true;
                                        }
                                    }
                                    else if (!role.IsLeader)
                                    {
                                        role.Join = false;
                                    }
                                }
                            }
                            readyNum = (ready ? readyNum : 0);
                            if (readyNum != piPeiState.ReadyNum || piPeiState.State == 2 != ready)
                            {
                                ReturnValue<int> result = TcpCall.EscapeBattle_K.ZhanDuiJoin(zhanDuiID, piPeiState.EscapeJiFen, readyNum);
                                if (result.IsReturn && result.Value >= 0)
                                {
                                    piPeiState.ReadyNum = readyNum;
                                }
                                if (piPeiState.State == 2)
                                {
                                    foreach (GameClient c in notifyList)
                                    {
                                        c.sendCmd<List<EscapeBattleJoinRoleInfo>>(2120, piPeiState.RoleList, true);
                                    }
                                }
                            }
                            if (piPeiState.State != 2 && notifyList.Count > 0)
                            {
                                foreach (GameClient c in notifyList)
                                {
                                    Global.GotoLastMap(c, 1);
                                }
                            }
                            TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Manager.getInstance().GetZhanDuiData(kv.Key, GameManager.ServerId);
                            if (null == zhanDuiData)
                            {
                                removedList.Add(kv.Key);
                            }
                        }
                    }
                    foreach (int id in removedList)
                    {
                        this.RuntimeData.ConfirmBattleDict.Remove(id);
                    }
                }
            }
            while (this.RuntimeData.SyncDataByTime.RunByInterval(nowTicks))
            {
                EscapeBattleSyncData syncDataRequest = this.RuntimeData.SyncDataRequest;
                ReturnValue<EscapeBattleSyncData> result2 = TcpCall.EscapeBattle_K.SyncZhengBaData(syncDataRequest);
                if (!result2.IsReturn)
                {
                    break;
                }
                EscapeBattleSyncData syncData = result2.Value;
                if (syncData == null)
                {
                    break;
                }
                long endTicks = TimeUtil.NOW();
                if (endTicks - nowTicks < 1000L)
                {
                    long diff = endTicks - nowTicks;
                    this.RuntimeData.SyncData.CenterTime = syncData.CenterTime.AddMilliseconds((double)diff);
                    this.RuntimeData.DiffTimeSpan = this.RuntimeData.SyncData.CenterTime - TimeUtil.NowDateTime();
                }
                lock (this.RuntimeData.Mutex)
                {
                    if (syncDataRequest != this.RuntimeData.SyncDataRequest)
                    {
                        break;
                    }
                    this.RuntimeData.SyncData.Season = syncData.Season;
                    this.RuntimeData.SyncData.State = syncData.State;
                    if (syncData.RankModTime != syncDataRequest.RankModTime)
                    {
                        this.RuntimeData.SyncDataRequest.RankModTime = syncData.RankModTime;
                        this.RuntimeData.SyncData.RankList = syncData.RankList;
                        this.RuntimeData.SyncData.SeasonRankList = syncData.SeasonRankList;
                    }
                }
            }
            lock (this.RuntimeData.Mutex)
            {
                for (int i = 0; i < this.RuntimeData.PKResultQueue.Count; i++)
                {
                    KeyValuePair<int, List<int>> pkResult = this.RuntimeData.PKResultQueue.Peek();
                    if (TcpCall.EscapeBattle_K.GameResult(pkResult.Key, pkResult.Value).Type != ReturnType.Success)
                    {
                        break;
                    }
                    this.RuntimeData.PKResultQueue.Dequeue();
                }
                for (int i = 0; i < this.RuntimeData.GameStateQueue.Count; i++)
                {
                    KeyValuePair<int, int> state = this.RuntimeData.GameStateQueue.Peek();
                    if (TcpCall.EscapeBattle_K.GameState(state.Key, state.Value).Type != ReturnType.Success)
                    {
                        break;
                    }
                    this.RuntimeData.GameStateQueue.Dequeue();
                }
            }
        }

        // Token: 0x0600024E RID: 590 RVA: 0x00029554 File Offset: 0x00027754
        public void InitEscapeBattleZhanDuiData(TianTi5v5ZhanDuiData zhanduiData)
        {
            if (zhanduiData != null && 0 != this.RuntimeData.SyncData.Season)
            {
                DateTime SeasonTime = TimeUtil.GetRealDate(this.RuntimeData.SyncData.Season);
                if (zhanduiData.EscapeLastFightTime < SeasonTime)
                {
                    zhanduiData.EscapeJiFen = 0;
                }
            }
        }

        // Token: 0x0600024F RID: 591 RVA: 0x000295B8 File Offset: 0x000277B8
        public TianTi5v5ZhanDuiData GetZhanDuiData(int zhanDuiID, int serverID)
        {
            TianTi5v5ZhanDuiData zhanduiData = TianTi5v5Manager.getInstance().GetZhanDuiData(zhanDuiID, serverID);
            TianTi5v5ZhanDuiData result;
            if (null == zhanduiData)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("获取战队数据失败 ZhanDuiID={0} ServerID={1}", zhanDuiID, serverID), null, true);
                result = null;
            }
            else
            {
                this.InitEscapeBattleZhanDuiData(zhanduiData);
                result = zhanduiData;
            }
            return result;
        }

        // Token: 0x06000250 RID: 592 RVA: 0x00029610 File Offset: 0x00027810
        public EscapeBDuanAwardsConfig GetEscapeBattleAwardConfigByJiFen(int jifen)
        {
            EscapeBDuanAwardsConfig awardConfig = null;
            lock (this.RuntimeData.Mutex)
            {
                foreach (EscapeBDuanAwardsConfig item in this.RuntimeData.DuanAwardsConfig)
                {
                    if ((item.RankValue < 0 || jifen >= item.RankValue) && (awardConfig == null || item.ID > awardConfig.ID))
                    {
                        awardConfig = item;
                    }
                }
            }
            return awardConfig;
        }

        // Token: 0x06000251 RID: 593 RVA: 0x000296E8 File Offset: 0x000278E8
        protected void SetSeasonRankDataFlags(GameClient client, bool bFight, bool bHasGet)
        {
            long seasonAwardDatas = Global.GetRoleParamsInt64FromDB(client, "10256");
            uint[] seasonAwardDataArr = LongUnion.DecodeDec(seasonAwardDatas, EscapeBattleConsts.SeasonAwardDataBitInfo);
            if (null != seasonAwardDataArr)
            {
                uint fight = 0U;
                uint lastFight = 0U;
                uint hasGet = 0U;
                uint zhanDuiIDFlag = 0U;
                int lastSeason = this.RuntimeData.SyncData.Season - EscapeBattleConsts.DaysPerSeason;
                if ((ulong)seasonAwardDataArr[0] == (ulong)((long)this.RuntimeData.SyncData.Season))
                {
                    fight = seasonAwardDataArr[1];
                    lastFight = seasonAwardDataArr[2];
                    hasGet = seasonAwardDataArr[3];
                }
                else if ((ulong)seasonAwardDataArr[0] == (ulong)((long)lastSeason))
                {
                    lastFight = seasonAwardDataArr[1];
                }
                if (bFight || fight > 0U)
                {
                    zhanDuiIDFlag = (uint)(client.ClientData.ZhanDuiID % 10000);
                    fight = 1U;
                }
                if (bHasGet || hasGet > 0U)
                {
                    hasGet = 1U;
                }
                seasonAwardDatas = LongUnion.CreateDec(EscapeBattleConsts.SeasonAwardDataBitInfo, new uint[]
                {
                    (uint)this.RuntimeData.SyncData.Season,
                    fight,
                    lastFight,
                    hasGet,
                    zhanDuiIDFlag
                });
                Global.SaveRoleParamsInt64ValueToDB(client, "10256", seasonAwardDatas, true);
            }
        }

        // Token: 0x06000252 RID: 594 RVA: 0x00029880 File Offset: 0x00027A80
        public int CanGetMonthRankAwards(GameClient client, out EscapeBattleAwardsConfig duanWeiRankAward)
        {
            duanWeiRankAward = null;
            int offsetDay = TimeUtil.GetOffsetDayNow();
            int result;
            if (offsetDay - this.RuntimeData.SyncData.Season > EscapeBattleConsts.DaysPerSeason)
            {
                result = 0;
            }
            else if (this.RuntimeData.SyncData.SeasonRankList == null)
            {
                result = 0;
            }
            else
            {
                int zhanDuiId = client.ClientData.ZhanDuiID;
                int lastSeason = this.RuntimeData.SyncData.Season - EscapeBattleConsts.DaysPerSeason;
                DateTime seasonStartTime = TimeUtil.GetRealDate(lastSeason);
                TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiId, client.ServerId);
                lock (this.RuntimeData.Mutex)
                {
                    if (zhanDuiData != null && zhanDuiData.EscapeLastFightTime > seasonStartTime)
                    {
                        int rank = this.RuntimeData.SyncData.SeasonRankList.FindIndex((KFEscapeRankInfo x) => x.Key == zhanDuiId) + 1;
                        if (rank <= 0)
                        {
                            return 0;
                        }
                        duanWeiRankAward = this.RuntimeData.AwardsConfig.Find((EscapeBattleAwardsConfig x) => x.MinRank <= rank && x.MaxRank >= rank);
                        if (null != duanWeiRankAward)
                        {
                            uint zhanDuiFlag = (uint)(client.ClientData.ZhanDuiID % 10000);
                            long seasonAwardDatas = Global.GetRoleParamsInt64FromDB(client, "10256");
                            uint[] seasonAwardDataArr = LongUnion.DecodeDec(seasonAwardDatas, EscapeBattleConsts.SeasonAwardDataBitInfo);
                            if (seasonAwardDataArr == null || seasonAwardDataArr[4] != zhanDuiFlag)
                            {
                                return 0;
                            }
                            if ((ulong)seasonAwardDataArr[0] == (ulong)((long)lastSeason))
                            {
                                if (seasonAwardDataArr[1] > 0U && seasonAwardDataArr[3] == 0U)
                                {
                                    return rank;
                                }
                            }
                        }
                    }
                }
                result = 0;
            }
            return result;
        }

        // Token: 0x06000253 RID: 595 RVA: 0x00029ABC File Offset: 0x00027CBC
        public List<int> GetEscapeBattleRoleAnalysisData(GameClient client)
        {
            List<int> result;
            if (0 == this.RuntimeData.SyncData.Season)
            {
                result = null;
            }
            else
            {
                List<int> countList = Global.GetRoleParamsIntListFromDB(client, "156");
                this.FilterEscapeBattleAnalysisData(countList);
                result = countList;
            }
            return result;
        }

        // Token: 0x06000254 RID: 596 RVA: 0x00029B04 File Offset: 0x00027D04
        private void FilterEscapeBattleAnalysisData(List<int> countList)
        {
            if (countList.Count != 4)
            {
                for (int i = countList.Count; i < 4; i++)
                {
                    countList.Add(0);
                }
            }
            int dayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
            if (this.RuntimeData.SyncData.Season != countList[0])
            {
                countList[0] = this.RuntimeData.SyncData.Season;
            }
            if (dayID != countList[1])
            {
                countList[1] = dayID;
                countList[3] = 0;
            }
        }

        // Token: 0x06000255 RID: 597 RVA: 0x00029BA2 File Offset: 0x00027DA2
        private void SaveEscapeBattleRoleAnalysisData(GameClient client, List<int> countList)
        {
            Global.SaveRoleParamsIntListToDB(client, countList, "156", true);
        }

        // Token: 0x06000256 RID: 598 RVA: 0x00029BB4 File Offset: 0x00027DB4
        public bool CanUseMaigc(GameClient client)
        {
            EscapeBattleScene scene = client.SceneObject as EscapeBattleScene;
            return null == scene || scene.m_eStatus >= EscapeBattleGameSceneStatuses.STATUS_FIGHT;
        }

        // Token: 0x06000257 RID: 599 RVA: 0x00029C6C File Offset: 0x00027E6C
        public void FillGuanZhanData(GameClient client, GuanZhanData gzData)
        {
            lock (this.RuntimeData.Mutex)
            {
                EscapeBattleScene scene = client.SceneObject as EscapeBattleScene;
                if (null != scene)
                {
                    List<EscapeBattleRoleInfo> clientContextDataList;
                    if (scene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out clientContextDataList))
                    {
                        foreach (EscapeBattleRoleInfo item in clientContextDataList)
                        {
                            GameClient sceneClient = GameManager.ClientMgr.FindClient(item.RoleID);
                            if (sceneClient != null && sceneClient.ClientData.HideGM <= 0)
                            {
                                SceneUIClasses sceneType = Global.GetMapSceneType(sceneClient.ClientData.MapCode);
                                if (SceneUIClasses.EscapeBattle == sceneType)
                                {
                                    List<GuanZhanRoleMiniData> roleDataList = null;
                                    if (!gzData.RoleMiniDataDict.TryGetValue(client.ClientData.BattleWhichSide, out roleDataList))
                                    {
                                        roleDataList = new List<GuanZhanRoleMiniData>();
                                        gzData.RoleMiniDataDict[client.ClientData.BattleWhichSide] = roleDataList;
                                    }
                                    GuanZhanRoleMiniData roleMiniData = new GuanZhanRoleMiniData();
                                    roleMiniData.RoleID = sceneClient.ClientData.RoleID;
                                    roleMiniData.Name = Global.FormatRoleNameWithZoneId2(sceneClient);
                                    roleMiniData.Level = sceneClient.ClientData.Level;
                                    roleMiniData.ChangeLevel = sceneClient.ClientData.ChangeLifeCount;
                                    roleMiniData.Occupation = sceneClient.ClientData.Occupation;
                                    roleMiniData.RoleSex = sceneClient.ClientData.RoleSex;
                                    roleMiniData.BHZhiWu = sceneClient.ClientData.BHZhiWu;
                                    roleMiniData.Param1 = item.KillRoleNum;
                                    roleMiniData.Param2 = ((client.ClientData.CurrentLifeV > 0 || item.ReliveCount > 0) ? 1 : 0);
                                    roleDataList.Add(roleMiniData);
                                }
                            }
                        }
                        foreach (List<GuanZhanRoleMiniData> rolelist in gzData.RoleMiniDataDict.Values)
                        {
                            rolelist.Sort(delegate (GuanZhanRoleMiniData left, GuanZhanRoleMiniData right)
                            {
                                int result;
                                if (left.Param2 > right.Param2)
                                {
                                    result = -1;
                                }
                                else if (left.Param2 < right.Param2)
                                {
                                    result = 1;
                                }
                                else if (left.Param1 > right.Param1)
                                {
                                    result = -1;
                                }
                                else if (left.Param1 < right.Param1)
                                {
                                    result = 1;
                                }
                                else
                                {
                                    result = 0;
                                }
                                return result;
                            });
                        }
                    }
                }
            }
        }

        // Token: 0x06000258 RID: 600 RVA: 0x00029F30 File Offset: 0x00028130
        public bool CheckOpenState(DateTime now)
        {
            bool result;
            lock (this.RuntimeData.Mutex)
            {
                if (now < this.RuntimeData.EscapeStartTime)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        // Token: 0x06000259 RID: 601 RVA: 0x00029F98 File Offset: 0x00028198
        public void CancleJoinState(GameClient client)
        {
            if (client.ClientData.ZhanDuiZhiWu == 1)
            {
                int zhanDuiID = client.ClientData.ZhanDuiID;
                lock (this.RuntimeData.Mutex)
                {
                    EscapeBattlePiPeiState piPeiState;
                    if (zhanDuiID > 0 && this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out piPeiState))
                    {
                        if (piPeiState.State == 2)
                        {
                            piPeiState.State = 0;
                        }
                    }
                }
            }
        }

        // Token: 0x0600025A RID: 602 RVA: 0x0002A044 File Offset: 0x00028244
        public bool OnPreZhanDuiChangeMember(PreZhanDuiChangeMemberEventObject e)
        {
            DateTime now = TimeUtil.NowDateTime();
            long nowTicks = now.Ticks;
            TimeSpan ts = new TimeSpan((int)now.DayOfWeek, now.Hour, now.Minute, now.Second);
            lock (this.RuntimeData.Mutex)
            {
                List<TimeSpan> timeList = this.RuntimeData.Config.MatchConfigList[0].TimePoints;
                for (int i = 0; i < timeList.Count - 1; i += 2)
                {
                    if (ts >= timeList[i] && ts < timeList[i])
                    {
                        EscapeBattlePiPeiState piPeiState;
                        if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(e.ZhanDuiID, out piPeiState) || piPeiState.State >= 2)
                        {
                            return false;
                        }
                        e.Result = false;
                    }
                }
            }
            bool result;
            if (!e.Result)
            {
                GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(8001, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x04000351 RID: 849
        public const SceneUIClasses ManagerType = SceneUIClasses.EscapeBattle;

        // Token: 0x04000352 RID: 850
        public const GameTypes GameType = GameTypes.EscapeBattle;

        // Token: 0x04000353 RID: 851
        public ConcurrentDictionary<int, EscapeBattleScene> SceneDict = new ConcurrentDictionary<int, EscapeBattleScene>();

        // Token: 0x04000354 RID: 852
        private static long NextHeartBeatTicks = 0L;

        // Token: 0x04000355 RID: 853
        private static EscapeBattleManager instance = new EscapeBattleManager();

        // Token: 0x04000356 RID: 854
        private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler = null;

        // Token: 0x04000357 RID: 855
        private EventSourceEx<KFCallMsg>.HandlerData NotifyGameStateHandler = null;

        // Token: 0x04000358 RID: 856
        public EscapeBattleData RuntimeData = new EscapeBattleData();
    }
}
