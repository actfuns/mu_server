using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
    
    public class ElementWarManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx, IManager2, IEventListener
    {
        
        public static ElementWarManager getInstance()
        {
            return ElementWarManager.instance;
        }

        
        public bool initialize()
        {
            return this.InitConfig();
        }

        
        public bool initialize(ICoreInterface coreInterface)
        {
            return true;
        }

        
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(1010, 1, 1, ElementWarManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1011, 1, 1, ElementWarManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1012, 2, 2, ElementWarManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            GlobalEventSource4Scene.getInstance().registerListener(10001, 28, ElementWarManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10000, 28, ElementWarManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10004, 28, ElementWarManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10005, 28, ElementWarManager.getInstance());
            return true;
        }

        
        public bool showdown()
        {
            GlobalEventSource4Scene.getInstance().removeListener(10001, 28, ElementWarManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10000, 28, ElementWarManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10004, 28, ElementWarManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10005, 28, ElementWarManager.getInstance());
            return true;
        }

        
        public bool destroy()
        {
            return true;
        }

        
        public bool processCmd(GameClient client, string[] cmdParams)
        {
            return false;
        }

        
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            bool result;
            switch (nID)
            {
                case 1010:
                    result = this.ProcessJoinCmd(client, nID, bytes, cmdParams);
                    break;
                case 1011:
                    result = this.ProcessQuitCmd(client, nID, bytes, cmdParams);
                    break;
                case 1012:
                    result = this.ProcessEnterCmd(client, nID, bytes, cmdParams);
                    break;
                default:
                    result = true;
                    break;
            }
            return result;
        }

        
        public void processEvent(EventObject eventObject)
        {
        }

        
        public void processEvent(EventObjectEx eventObject)
        {
            switch (eventObject.EventType)
            {
                case 10000:
                    {
                        KuaFuFuBenRoleCountEvent e = eventObject as KuaFuFuBenRoleCountEvent;
                        if (null != e)
                        {
                            GameClient client = GameManager.ClientMgr.FindClient(e.RoleId);
                            if (null != client)
                            {
                                client.sendCmd<int>(1013, e.RoleCount, false);
                            }
                            eventObject.Handled = true;
                        }
                        break;
                    }
                case 10001:
                    {
                        KuaFuNotifyEnterGameEvent e2 = eventObject as KuaFuNotifyEnterGameEvent;
                        if (null != e2)
                        {
                            KuaFuServerLoginData kuaFuServerLoginData = e2.Arg as KuaFuServerLoginData;
                            if (null != kuaFuServerLoginData)
                            {
                                GameClient client = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
                                if (null != client)
                                {
                                    KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
                                    if (null != clientKuaFuServerLoginData)
                                    {
                                        clientKuaFuServerLoginData.RoleId = kuaFuServerLoginData.RoleId;
                                        clientKuaFuServerLoginData.GameId = kuaFuServerLoginData.GameId;
                                        clientKuaFuServerLoginData.GameType = kuaFuServerLoginData.GameType;
                                        clientKuaFuServerLoginData.EndTicks = kuaFuServerLoginData.EndTicks;
                                        clientKuaFuServerLoginData.ServerId = kuaFuServerLoginData.ServerId;
                                        clientKuaFuServerLoginData.ServerIp = kuaFuServerLoginData.ServerIp;
                                        clientKuaFuServerLoginData.ServerPort = kuaFuServerLoginData.ServerPort;
                                        clientKuaFuServerLoginData.FuBenSeqId = kuaFuServerLoginData.FuBenSeqId;
                                        client.sendCmd(1012, string.Format("{0}:{1}", kuaFuServerLoginData.GameId, e2.TeamCombatAvg), false);
                                    }
                                }
                            }
                            eventObject.Handled = true;
                        }
                        break;
                    }
                case 10004:
                    {
                        KuaFuNotifyCopyCancelEvent e3 = eventObject as KuaFuNotifyCopyCancelEvent;
                        GameClient client = GameManager.ClientMgr.FindClient(e3.RoleId);
                        if (client != null)
                        {
                            client.ClientData.SignUpGameType = 0;
                            client.sendCmd(1016, string.Format("{0}:{1}", e3.GameId, e3.Reason), false);
                        }
                        eventObject.Handled = true;
                        break;
                    }
                case 10005:
                    {
                        KuaFuNotifyRealEnterGameEvent e4 = eventObject as KuaFuNotifyRealEnterGameEvent;
                        if (e4 != null)
                        {
                            GameClient client = GameManager.ClientMgr.FindClient(e4.RoleId);
                            if (client != null)
                            {
                                client.ClientData.SignUpGameType = 0;
                                GlobalNew.RecordSwitchKuaFuServerLog(client);
                                client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
                            }
                        }
                        eventObject.Handled = true;
                        break;
                    }
            }
        }

        
        public bool InitConfig()
        {
            bool success = true;
            string fileName = "";
            lock (this._runtimeData.Mutex)
            {
                try
                {
                    this._runtimeData.MonsterOrderConfigList.Clear();
                    fileName = Global.GameResPath("Config/YuanSuShiLian.xml");
                    XElement xml = CheckHelper.LoadXml(fileName, true);
                    if (null == xml)
                    {
                        return false;
                    }
                    IEnumerable<XElement> nodes = xml.Elements();
                    foreach (XElement xmlItem in nodes)
                    {
                        if (xmlItem != null)
                        {
                            ElementWarMonsterConfigInfo config = new ElementWarMonsterConfigInfo();
                            config.OrderID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
                            config.MonsterCount = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0"));
                            string[] ids = Global.GetDefAttributeStr(xmlItem, "MonstersID", "0,0,0").Split(new char[]
                            {
                                '|'
                            });
                            config.MonsterIDs = new List<int>();
                            foreach (string id in ids)
                            {
                                config.MonsterIDs.Add(int.Parse(id));
                            }
                            config.Up1 = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "UpOne", "0"));
                            config.Up2 = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "UpTwo", "0"));
                            config.Up3 = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "UpThree", "0"));
                            config.Up4 = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "UpFour", "0"));
                            config.X = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "X", "0"));
                            config.Y = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Y", "0"));
                            config.Radius = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Radius", "0"));
                            this._runtimeData.MonsterOrderConfigList.Add(config.OrderID, config);
                        }
                    }
                    this._runtimeData.AwardLight = GameManager.systemParamsList.GetParamValueIntArrayByName("YuanSuShiLianAward", ',');
                    this._runtimeData.YuanSuShiLianAward2 = GameManager.systemParamsList.GetParamValueIntArrayByName("YuanSuShiLianAward2", ',');
                }
                catch (Exception ex)
                {
                    success = false;
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
                }
            }
            return success;
        }

        
        public bool ProcessJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
                if (sceneType != SceneUIClasses.Normal)
                {
                    client.sendCmd(nID, "-21".ToString(), false);
                    return true;
                }
                if (!this.IsGongNengOpened(client, true))
                {
                    client.sendCmd(nID, "-2001".ToString(), false);
                    return true;
                }
                if (client.ClientData.SignUpGameType != 0)
                {
                    client.sendCmd(nID, "-2002".ToString(), false);
                    return true;
                }
                if (KuaFuManager.getInstance().IsInCannotJoinKuaFuCopyTime(client))
                {
                    client.sendCmd(nID, "-2004".ToString(), false);
                    return true;
                }
                SystemXmlItem systemFuBenItem = null;
                if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(this._runtimeData.CopyID, out systemFuBenItem))
                {
                    client.sendCmd(nID, "-3".ToString(), false);
                    return true;
                }
                int minLevel = systemFuBenItem.GetIntValue("MinLevel", -1);
                int minZhuanSheng = systemFuBenItem.GetIntValue("MinZhuanSheng", -1);
                int levelLimit = minZhuanSheng * 100 + minLevel;
                if (client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level < levelLimit)
                {
                    client.sendCmd(nID, "-19".ToString(), false);
                    return true;
                }
                int oldCount = this.GetElementWarCount(client);
                if (oldCount >= systemFuBenItem.GetIntValue("FinishNumber", -1))
                {
                    client.sendCmd(nID, "-16".ToString(), false);
                    return true;
                }
                int result = 0;
                if (result > 0)
                {
                    client.ClientData.SignUpGameType = 4;
                    GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 4);
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

        
        public bool ProcessQuitCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!this.IsGongNengOpened(client, false))
                {
                    client.sendCmd(nID, 0.ToString(), false);
                    return true;
                }
                client.ClientData.SignUpGameType = 0;
                int result = 0;
                client.sendCmd<int>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        public bool ProcessEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!this.IsGongNengOpened(client, false))
                {
                    client.sendCmd(nID, 0.ToString(), false);
                    return true;
                }
                client.ClientData.SignUpGameType = 0;
                int flag = Global.SafeConvertToInt32(cmdParams[1]);
                if (flag > 0)
                {
                    int result = 0;
                    if (result < 0)
                    {
                        flag = 0;
                    }
                }
                else
                {
                    KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
                }
                if (flag <= 0)
                {
                    Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
                    client.sendCmd(1011, 0.ToString(), false);
                }
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        public bool OnInitGame(GameClient client)
        {
            GameMap gameMap = null;
            bool result;
            if (GameManager.MapMgr.DictMaps.TryGetValue(this._runtimeData.MapID, out gameMap))
            {
                int defaultBirthPosX = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosX;
                int defaultBirthPosY = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosY;
                int defaultBirthRadius = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].BirthRadius;
                Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this._runtimeData.MapID, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
                client.ClientData.MapCode = this._runtimeData.MapID;
                client.ClientData.PosX = (int)newPos.X;
                client.ClientData.PosY = (int)newPos.Y;
                client.ClientData.FuBenSeqID = Global.GetClientKuaFuServerLoginData(client).FuBenSeqId;
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        public bool ClientRelive(GameClient client)
        {
            GameMap gameMap = null;
            bool result;
            if (GameManager.MapMgr.DictMaps.TryGetValue(this._runtimeData.MapID, out gameMap))
            {
                int defaultBirthPosX = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosX;
                int defaultBirthPosY = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosY;
                int defaultBirthRadius = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].BirthRadius;
                Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this._runtimeData.MapID, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
                client.ClientData.CurrentLifeV = client.ClientData.LifeV;
                client.ClientData.CurrentMagicV = client.ClientData.MagicV;
                client.ClientData.MoveAndActionNum = 0;
                GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, (int)newPos.X, (int)newPos.Y, -1);
                Global.ClientRealive(client, (int)newPos.X, (int)newPos.Y, -1);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        public bool IsGongNengOpened(GameClient client, bool hint = false)
        {
            return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("ElementWar") && GlobalNew.IsGongNengOpened(client, GongNengIDs.ElementWar, hint);
        }

        
        public int GetElementWarCount(GameClient client)
        {
            int day = Global.GetRoleParamsInt32FromDB(client, "ElementWarDayId");
            int count = Global.GetRoleParamsInt32FromDB(client, "ElementWarCount");
            int today = Global.GetOffsetDayNow();
            int result;
            if (today == day)
            {
                result = count;
            }
            else
            {
                Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarDayId", today, true);
                Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarCount", 0, true);
                result = 0;
            }
            return result;
        }

        
        public void AddElementWarCount(GameClient client)
        {
            int count = this.GetElementWarCount(client);
            Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarCount", count + 1, true);
        }

        
        public bool AddCopyScene(GameClient client, CopyMap copyMap)
        {
            bool result;
            if (copyMap.MapCode == this._runtimeData.MapID)
            {
                int fuBenSeqId = copyMap.FuBenSeqID;
                int mapCode = copyMap.MapCode;
                lock (ElementWarManager._mutex)
                {
                    ElementWarScene newScene = null;
                    if (!this._sceneDict.TryGetValue(fuBenSeqId, out newScene))
                    {
                        newScene = new ElementWarScene();
                        newScene.CopyMapInfo = copyMap;
                        newScene.CleanAllInfo();
                        newScene.GameId = Global.GetClientKuaFuServerLoginData(client).GameId;
                        newScene.MapID = mapCode;
                        newScene.CopyID = copyMap.CopyMapID;
                        newScene.FuBenSeqId = fuBenSeqId;
                        newScene.PlayerCount = 1;
                        this._sceneDict[fuBenSeqId] = newScene;
                    }
                    else
                    {
                        newScene.PlayerCount++;
                    }
                    copyMap.IsKuaFuCopy = true;
                    copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this._runtimeData.TotalSecs * 1000));
                    GameManager.ClientMgr.BroadSpecialCopyMapMessage<ElementWarScoreData>(1014, newScene.ScoreData, newScene.CopyMapInfo);
                }
                GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 4);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        public bool RemoveCopyScene(CopyMap copyMap)
        {
            bool result;
            if (copyMap.MapCode == this._runtimeData.MapID)
            {
                lock (ElementWarManager._mutex)
                {
                    ElementWarScene scene;
                    this._sceneDict.TryRemove(copyMap.FuBenSeqID, out scene);
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        public void TimerProc()
        {
            long nowTicks = TimeUtil.NOW();
            if (nowTicks >= ElementWarManager._nextHeartBeatTicks)
            {
                ElementWarManager._nextHeartBeatTicks = nowTicks + 1020L;
                long nowSecond = nowTicks / 1000L;
                foreach (ElementWarScene scene in this._sceneDict.Values)
                {
                    lock (ElementWarManager._mutex)
                    {
                        int nID = scene.FuBenSeqId;
                        int nCopyID = scene.CopyID;
                        int nMapID = scene.MapID;
                        if (nID >= 0 && nCopyID >= 0 && nMapID >= 0)
                        {
                            CopyMap copyMap = scene.CopyMapInfo;
                            if (scene.SceneStatus == GameSceneStatuses.STATUS_NULL)
                            {
                                scene.PrepareTime = nowSecond;
                                scene.BeginTime = nowSecond + (long)this._runtimeData.PrepareSecs;
                                scene.SceneStatus = GameSceneStatuses.STATUS_PREPARE;
                                scene.StateTimeData.GameType = 4;
                                scene.StateTimeData.State = (int)scene.SceneStatus;
                                scene.StateTimeData.EndTicks = nowTicks + (long)(this._runtimeData.PrepareSecs * 1000);
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
                            }
                            else if (scene.SceneStatus == GameSceneStatuses.STATUS_PREPARE)
                            {
                                if (nowSecond >= scene.BeginTime)
                                {
                                    scene.SceneStatus = GameSceneStatuses.STATUS_BEGIN;
                                    scene.EndTime = nowSecond + (long)this._runtimeData.FightingSecs;
                                    scene.StateTimeData.GameType = 4;
                                    scene.StateTimeData.State = (int)scene.SceneStatus;
                                    scene.StateTimeData.EndTicks = nowTicks + (long)(this._runtimeData.FightingSecs * 1000);
                                    GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
                                }
                            }
                            else if (scene.SceneStatus == GameSceneStatuses.STATUS_BEGIN)
                            {
                                if (nowSecond >= scene.EndTime)
                                {
                                    scene.SceneStatus = GameSceneStatuses.STATUS_END;
                                }
                                else
                                {
                                    bool bNeedCreateMonster = false;
                                    int upWave = 0;
                                    lock (scene)
                                    {
                                        ElementWarMonsterConfigInfo configInfo = this._runtimeData.GetOrderConfig(scene.MonsterWave);
                                        if (configInfo == null)
                                        {
                                            scene.MonsterWaveOld = 1;
                                            scene.MonsterWave = 0;
                                            scene.SceneStatus = GameSceneStatuses.STATUS_END;
                                        }
                                        else if (scene.CreateMonsterTime > 0L && nowSecond - scene.CreateMonsterTime >= (long)configInfo.Up1)
                                        {
                                            scene.MonsterWave = 0;
                                            scene.SceneStatus = GameSceneStatuses.STATUS_END;
                                        }
                                        else
                                        {
                                            if (scene.CreateMonsterTime > 0L && scene.IsMonsterFlag == 0 && scene.ScoreData.MonsterCount <= 0L)
                                            {
                                                if (scene.MonsterWave >= scene.MonsterWaveTotal)
                                                {
                                                    scene.MonsterWaveOld = scene.MonsterWave;
                                                    scene.MonsterWave = 0;
                                                    scene.SceneStatus = GameSceneStatuses.STATUS_END;
                                                    continue;
                                                }
                                                bNeedCreateMonster = true;
                                                if (nowSecond - scene.CreateMonsterTime <= (long)configInfo.Up4)
                                                {
                                                    upWave = 4;
                                                }
                                                else if (nowSecond - scene.CreateMonsterTime <= (long)configInfo.Up3)
                                                {
                                                    upWave = 3;
                                                }
                                                else if (nowSecond - scene.CreateMonsterTime <= (long)configInfo.Up2)
                                                {
                                                    upWave = 2;
                                                }
                                                else if (nowSecond - scene.CreateMonsterTime < (long)configInfo.Up1)
                                                {
                                                    upWave = 1;
                                                }
                                            }
                                            if (scene.CreateMonsterTime <= 0L)
                                            {
                                                bNeedCreateMonster = true;
                                            }
                                            if (bNeedCreateMonster)
                                            {
                                                this.CreateMonster(scene, upWave);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (scene.SceneStatus == GameSceneStatuses.STATUS_END)
                            {
                                this.GiveAwards(scene);
                                scene.SceneStatus = GameSceneStatuses.STATUS_AWARD;
                                scene.EndTime = nowSecond;
                                scene.LeaveTime = scene.EndTime + (long)this._runtimeData.ClearRolesSecs;
                                scene.StateTimeData.GameType = 4;
                                scene.StateTimeData.State = 3;
                                scene.StateTimeData.EndTicks = nowTicks + (long)(this._runtimeData.ClearRolesSecs * 1000);
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMapInfo);
                            }
                            else if (scene.SceneStatus == GameSceneStatuses.STATUS_AWARD)
                            {
                                if (nowSecond >= scene.LeaveTime)
                                {
                                    copyMap.SetRemoveTicks(scene.LeaveTime);
                                    scene.SceneStatus = GameSceneStatuses.STATUS_CLEAR;
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
                                        DataHelper.WriteExceptionLogEx(ex, "【元素试炼】清场调度异常");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void CreateMonster(ElementWarScene scene, int upWave)
        {
            CopyMap copyMap = scene.CopyMapInfo;
            GameMap gameMap = null;
            if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapID, out gameMap))
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("元素试炼报错 地图配置 ID = {0}", scene.MapID), null, true);
            }
            else
            {
                long nowTicket = TimeUtil.NOW();
                long nowSecond = nowTicket / 1000L;
                lock (scene)
                {
                    if (scene.MonsterWave >= scene.MonsterWaveTotal)
                    {
                        scene.SceneStatus = GameSceneStatuses.STATUS_END;
                    }
                    else
                    {
                        scene.IsMonsterFlag = 1;
                        int wave = scene.MonsterWave + upWave;
                        if (wave > scene.MonsterWaveTotal)
                        {
                            wave = scene.MonsterWaveTotal;
                        }
                        ElementWarMonsterConfigInfo waveConfig = this._runtimeData.GetOrderConfig(wave);
                        if (waveConfig == null)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("元素试炼报错 刷怪波次 = {0}", wave), null, true);
                        }
                        else
                        {
                            scene.MonsterCountCreate = waveConfig.MonsterCount;
                            scene.MonsterWave = wave;
                            scene.CreateMonsterTime = nowSecond;
                            for (int i = 0; i < waveConfig.MonsterCount; i++)
                            {
                                int monsterID = waveConfig.MonsterIDs[i % waveConfig.MonsterIDs.Count];
                                int gridX = gameMap.CorrectWidthPointToGridPoint(waveConfig.X + Global.GetRandomNumber(-waveConfig.Radius, waveConfig.Radius)) / gameMap.MapGridWidth;
                                int gridY = gameMap.CorrectHeightPointToGridPoint(waveConfig.Y + Global.GetRandomNumber(-waveConfig.Radius, waveConfig.Radius)) / gameMap.MapGridHeight;
                                int gridNum = 0;
                                GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapID, monsterID, scene.CopyMapInfo.CopyMapID, 1, gridX, gridY, gridNum, 0, SceneUIClasses.Normal, null, null);
                            }
                            scene.ScoreData.Wave = waveConfig.OrderID;
                            scene.ScoreData.EndTime = nowTicket + (long)(waveConfig.Up1 * 1000);
                            scene.ScoreData.MonsterCount = (long)waveConfig.MonsterCount;
                            GameManager.ClientMgr.BroadSpecialCopyMapMessage<ElementWarScoreData>(1014, scene.ScoreData, scene.CopyMapInfo);
                        }
                    }
                }
            }
        }

        
        public bool IsElementWarCopy(int FubenID)
        {
            return FubenID == this._runtimeData.CopyID;
        }

        
        public void KillMonster(GameClient client, Monster monster)
        {
            if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsElementWarCopy(client.ClientData.FuBenID))
            {
                ElementWarScene scene = null;
                if (this._sceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene) && scene != null)
                {
                    if (scene.AddKilledMonster(monster))
                    {
                        if (scene.SceneStatus < GameSceneStatuses.STATUS_END)
                        {
                            lock (scene)
                            {
                                scene.MonsterCountKill++;
                                scene.ScoreData.MonsterCount -= 1L;
                                scene.ScoreData.MonsterCount = ((scene.ScoreData.MonsterCount < 0L) ? 0L : scene.ScoreData.MonsterCount);
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<ElementWarScoreData>(1014, scene.ScoreData, scene.CopyMapInfo);
                                if (scene.IsMonsterFlag == 1 && scene.MonsterCountKill >= scene.MonsterCountCreate && scene.ScoreData.MonsterCount <= 0L)
                                {
                                    scene.MonsterWaveOld = scene.MonsterWave;
                                    if (scene.MonsterWave >= scene.MonsterWaveTotal)
                                    {
                                        scene.SceneStatus = GameSceneStatuses.STATUS_END;
                                    }
                                    else
                                    {
                                        scene.IsMonsterFlag = 0;
                                        scene.MonsterCountKill = 0;
                                        scene.MonsterCountCreate = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void GiveAwards(ElementWarScene scene)
        {
            try
            {
                FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(scene.CopyMapInfo.FubenMapID, scene.MapID);
                if (fuBenMapItem != null)
                {
                    int zhanLi = 0;
                    List<GameClient> objsList = scene.CopyMapInfo.GetClientsList();
                    if (objsList != null && objsList.Count > 0)
                    {
                        for (int i = 0; i < objsList.Count; i++)
                        {
                            GameClient client = objsList[i];
                            if (client != null && client == GameManager.ClientMgr.FindClient(client.ClientData.RoleID))
                            {
                                long nExp = (long)fuBenMapItem.Experience;
                                int money = fuBenMapItem.Money1;
                                int wave = scene.MonsterWaveOld;
                                int light = fuBenMapItem.LightAward + this._runtimeData.AwardLight[wave];
                                int ysfm = fuBenMapItem.YuanSuFenMoaward + this._runtimeData.YuanSuShiLianAward2[wave];
                                if (nExp > 0L)
                                {
                                    GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false, true, false, "none");
                                }
                                if (money > 0)
                                {
                                    GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money, string.Format("副本{0}通关奖励", scene.CopyID), false);
                                }
                                if (light > 0)
                                {
                                    GameManager.FluorescentGemMgr.AddFluorescentPoint(client, light, "元素试炼", true);
                                }
                                if (ysfm > 0)
                                {
                                    GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, ysfm, "元素试炼", true, false);
                                }
                                ElementWarAwardsData awardsData = new ElementWarAwardsData
                                {
                                    Wave = scene.MonsterWaveOld,
                                    Exp = nExp,
                                    Money = money,
                                    Light = light,
                                    ysfm = ysfm
                                };
                                this.AddElementWarCount(client);
                                GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 1, 0, 4);
                                client.sendCmd<ElementWarAwardsData>(1015, awardsData, false);
                                zhanLi += client.ClientData.CombatForce;
                                Global.UpdateFuBenDataForQuickPassTimer(client, scene.CopyMapInfo.FubenMapID, 0, 1);
                            }
                        }
                    }
                    if (objsList != null && objsList.Count > 0)
                    {
                        int roleCount = objsList.Count;
                        zhanLi /= roleCount;
                    }
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "【元素试炼】清场调度异常");
            }
        }

        
        public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool scoreInfo = true)
        {
            lock (ElementWarManager._mutex)
            {
                ElementWarScene scene;
                if (this._sceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
                {
                    if (timeState)
                    {
                        client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
                    }
                    if (scoreInfo)
                    {
                        client.sendCmd<ElementWarScoreData>(1014, scene.ScoreData, false);
                    }
                }
            }
        }

        
        public void LeaveFuBen(GameClient client)
        {
            ElementWarScene scene = null;
            lock (this._sceneDict)
            {
                if (!this._sceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
                {
                    return;
                }
            }
            lock (scene)
            {
                scene.PlayerCount--;
                if (scene.SceneStatus != GameSceneStatuses.STATUS_END && scene.SceneStatus != GameSceneStatuses.STATUS_AWARD && scene.SceneStatus != GameSceneStatuses.STATUS_CLEAR)
                {
                    KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
                }
            }
        }

        
        public void OnLogout(GameClient client)
        {
            this.LeaveFuBen(client);
        }

        
        public const SceneUIClasses _sceneType = SceneUIClasses.ElementWar;

        
        public const GameTypes _gameType = GameTypes.ElementWar;

        
        public ElementWarData _runtimeData = new ElementWarData();

        
        private static ElementWarManager instance = new ElementWarManager();

        
        public static object _mutex = new object();

        
        public ConcurrentDictionary<int, ElementWarScene> _sceneDict = new ConcurrentDictionary<int, ElementWarScene>();

        
        private static long _nextHeartBeatTicks = 0L;
    }
}
