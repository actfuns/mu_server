using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.MoRi
{
    
    public class MoRiJudgeManager : SingletonTemplate<MoRiJudgeManager>, IManager, ICmdProcessorEx, ICmdProcessor, IManager2, IEventListenerEx, IEventListener
    {
        
        private MoRiJudgeManager()
        {
        }

        
        
        
        public double[] AwardFactor { get; set; }

        
        
        
        public int MapCode { get; set; }

        
        public bool initialize()
        {
            return this.InitConfig();
        }

        
        private bool InitConfig()
        {
            try
            {
                XElement xml = XElement.Load(Global.GameResPath("Config/MoRiShenPan.xml"));
                IEnumerable<XElement> xmlItems = xml.Elements();
                foreach (XElement item in xmlItems)
                {
                    MoRiMonster monster = new MoRiMonster();
                    monster.Id = (int)Global.GetSafeAttributeLong(item, "ID");
                    monster.Name = Global.GetSafeAttributeStr(item, "Name");
                    monster.MonsterId = (int)Global.GetSafeAttributeLong(item, "MonstersID");
                    monster.BirthX = (int)Global.GetSafeAttributeLong(item, "X");
                    monster.BirthY = (int)Global.GetSafeAttributeLong(item, "Y");
                    monster.KillLimitSecond = (int)Global.GetSafeAttributeLong(item, "Time");
                    string addBossProps = Global.GetSafeAttributeStr(item, "Props");
                    if (!string.IsNullOrEmpty(addBossProps) && addBossProps != "-1")
                    {
                        foreach (string prop in addBossProps.Split(new char[]
                        {
                            '|'
                        }))
                        {
                            string[] prop_kv = prop.Split(new char[]
                            {
                                ','
                            });
                            if (prop_kv != null && prop_kv.Length == 2)
                            {
                                monster.ExtPropDict.Add((int)Enum.Parse(typeof(ExtPropIndexes), prop_kv[0]), (float)Convert.ToDouble(prop_kv[1]));
                            }
                        }
                    }
                    this.BossConfigList.Add(monster);
                }
                this.BossConfigList.Sort(delegate (MoRiMonster left, MoRiMonster right)
                {
                    int result;
                    if (left.Id < right.Id)
                    {
                        result = -1;
                    }
                    else if (left.Id > right.Id)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = 0;
                    }
                    return result;
                });
                SystemXmlItem systemFuBenItem = null;
                if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(70000, out systemFuBenItem))
                {
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("缺少末日审判副本配置 CopyID={0}", 70000), null, true);
                    return false;
                }
                this.MapCode = systemFuBenItem.GetIntValue("MapCode", -1);
                FuBenMapItem fubenItem = FuBenManager.FindMapCodeByFuBenID(70000, this.MapCode);
                if (fubenItem == null)
                {
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("末日审判地图 {0} 配置错误", this.MapCode), null, true);
                    return false;
                }
                this.CopyMaxAliveMinutes = fubenItem.MaxTime;
                GameMap gameMap = null;
                if (!GameManager.MapMgr.DictMaps.TryGetValue(this.MapCode, out gameMap))
                {
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("缺少末日审判地图 {0}", this.MapCode), null, true);
                    return false;
                }
                this.copyMapGirdWidth = gameMap.MapGridWidth;
                this.copyMapGirdHeight = gameMap.MapGridHeight;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", "Config/MoRiShenPan.xml"), ex, true);
                return false;
            }
            return true;
        }

        
        public bool initialize(ICoreInterface coreInterface)
        {
            return true;
        }

        
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(1301, 1, 1, SingletonTemplate<MoRiJudgeManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1302, 1, 1, SingletonTemplate<MoRiJudgeManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1304, 2, 2, SingletonTemplate<MoRiJudgeManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
            GlobalEventSource4Scene.getInstance().registerListener(10000, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10001, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10004, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
            GlobalEventSource4Scene.getInstance().registerListener(10005, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
            GlobalEventSource.getInstance().registerListener(11, SingletonTemplate<MoRiJudgeManager>.Instance());
            return true;
        }

        
        public bool showdown()
        {
            GlobalEventSource4Scene.getInstance().removeListener(10000, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10001, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10004, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
            GlobalEventSource4Scene.getInstance().removeListener(10005, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
            GlobalEventSource.getInstance().removeListener(11, SingletonTemplate<MoRiJudgeManager>.Instance());
            return true;
        }

        
        public bool destroy()
        {
            return true;
        }

        
        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() == 11)
            {
                MonsterDeadEventObject deadEv = eventObject as MonsterDeadEventObject;
                if (deadEv.getAttacker().ClientData.CopyMapID > 0 && deadEv.getAttacker().ClientData.FuBenSeqID > 0 && deadEv.getAttacker().ClientData.MapCode == this.MapCode && deadEv.getMonster().CurrentMapCode == this.MapCode)
                {
                    MoRiMonsterTag tag = deadEv.getMonster().Tag as MoRiMonsterTag;
                    if (tag != null)
                    {
                        MoRiJudgeCopy judgeCopy = null;
                        lock (this.copyDict)
                        {
                            if (!this.copyDict.TryGetValue(tag.CopySeqId, out judgeCopy))
                            {
                                return;
                            }
                        }
                        lock (judgeCopy)
                        {
                            if (judgeCopy.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
                            {
                                if (judgeCopy.MonsterList[tag.MonsterIdx].DeathMs <= 0L)
                                {
                                    judgeCopy.MonsterList[tag.MonsterIdx].DeathMs = TimeUtil.NOW();
                                    GameManager.ClientMgr.BroadSpecialCopyMapMessageStr(1305, string.Format("{0}:{1}:{2}:{3}", new object[]
                                    {
                                        2,
                                        this.BossConfigList[tag.MonsterIdx].Id,
                                        judgeCopy.MonsterList[tag.MonsterIdx].BirthMs,
                                        judgeCopy.MonsterList[tag.MonsterIdx].DeathMs
                                    }), judgeCopy.MyCopyMap, false);
                                    this.CalcAwardRate(judgeCopy);
                                    if (judgeCopy.MonsterList.Count == this.BossConfigList.Count)
                                    {
                                        judgeCopy.Passed = true;
                                        judgeCopy.m_eStatus = GameSceneStatuses.STATUS_END;
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
                                client.sendCmd(1303, e.RoleCount.ToString(), false);
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
                                        client.sendCmd(1304, string.Format("{0}:{1}", kuaFuServerLoginData.GameId, e2.TeamCombatAvg), false);
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
                            client.sendCmd(1306, string.Format("{0}:{1}", e3.GameId, e3.Reason), false);
                            client.ClientData.SignUpGameType = 0;
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
                                GlobalNew.RecordSwitchKuaFuServerLog(client);
                                client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
                                client.ClientData.SignUpGameType = 0;
                            }
                        }
                        eventObject.Handled = true;
                        break;
                    }
            }
        }

        
        public bool processCmd(GameClient client, string[] cmdParams)
        {
            return false;
        }

        
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            switch (nID)
            {
                case 1301:
                    return this.ProcessMoRiJudgeJoin(client, nID, bytes, cmdParams);
                case 1302:
                    return this.ProcessMoRiJudgeQuit(client, nID, bytes, cmdParams);
                case 1304:
                    return this.ProcessMoRiJudgeEnter(client, nID, bytes, cmdParams);
            }
            return true;
        }

        
        private bool ProcessMoRiJudgeJoin(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
                if (sceneType != SceneUIClasses.Normal)
                {
                    client.sendCmd(nID, -21, false);
                    return true;
                }
                if (!this.IsGongNengOpened(client, true))
                {
                    client.sendCmd(nID, -2001, false);
                    return true;
                }
                if (client.ClientData.SignUpGameType != 0)
                {
                    client.sendCmd(nID, -2002, false);
                    return true;
                }
                if (KuaFuManager.getInstance().IsInCannotJoinKuaFuCopyTime(client))
                {
                    client.sendCmd(nID, -2004, false);
                    return true;
                }
                SystemXmlItem systemFuBenItem = null;
                if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(70000, out systemFuBenItem))
                {
                    client.sendCmd(nID, -3, false);
                    return true;
                }
                int minLevel = systemFuBenItem.GetIntValue("MinLevel", -1);
                int maxLevel = systemFuBenItem.GetIntValue("MaxLevel", -1);
                int nMinZhuanSheng = systemFuBenItem.GetIntValue("MinZhuanSheng", -1);
                int nMaxZhuanSheng = systemFuBenItem.GetIntValue("MaxZhuanSheng", -1);
                if (client.ClientData.ChangeLifeCount < nMinZhuanSheng || (client.ClientData.ChangeLifeCount == nMinZhuanSheng && client.ClientData.Level < minLevel))
                {
                    client.sendCmd(nID, "-19".ToString(), false);
                    return true;
                }
                if (client.ClientData.ChangeLifeCount > nMaxZhuanSheng || (client.ClientData.ChangeLifeCount == nMaxZhuanSheng && client.ClientData.Level > maxLevel))
                {
                    client.sendCmd(nID, "-19".ToString(), false);
                    return true;
                }
                FuBenData fuBenData = Global.GetFuBenData(client, 70000);
                if (fuBenData != null && fuBenData.FinishNum >= systemFuBenItem.GetIntValue("FinishNumber", -1))
                {
                    client.sendCmd(nID, "-16".ToString(), false);
                    return true;
                }
                int result = 0;
                if (result == 1)
                {
                    client.ClientData.SignUpGameType = 3;
                    GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 3);
                }
                client.sendCmd(nID, result.ToString(), false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        private bool ProcessMoRiJudgeQuit(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!this.IsGongNengOpened(client, true))
                {
                    client.sendCmd(nID, 0.ToString(), false);
                    return true;
                }
                int result = 1;
                if (result >= 0)
                {
                    result = 0;
                    client.ClientData.SignUpGameType = 0;
                }
                client.sendCmd(nID, result.ToString(), false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        private bool ProcessMoRiJudgeEnter(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!this.IsGongNengOpened(client, true))
                {
                    client.sendCmd(nID, -2001 + ":0", false);
                    return true;
                }
                int result = 1;
                int flag = Global.SafeConvertToInt32(cmdParams[1]);
                if (flag > 0)
                {
                    if (result < 0)
                    {
                        flag = 0;
                    }
                }
                else
                {
                    client.ClientData.SignUpGameType = 0;
                    KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
                }
                if (flag <= 0)
                {
                    Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
                    client.sendCmd<int>(1302, 0, false);
                }
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        private double CalcAwardRate(MoRiJudgeCopy judgeCopy)
        {
            double result = 1.0;
            judgeCopy.LimitKillCount = 0;
            for (int i = 0; i < judgeCopy.MonsterList.Count; i++)
            {
                if (judgeCopy.MonsterList[i].DeathMs > 0L && this.BossConfigList[i].KillLimitSecond > 0 && judgeCopy.MonsterList[i].DeathMs - judgeCopy.MonsterList[i].BirthMs <= (long)(this.BossConfigList[i].KillLimitSecond * 1000))
                {
                    judgeCopy.LimitKillCount++;
                }
            }
            if (this.AwardFactor != null && judgeCopy.LimitKillCount - 1 >= 0 && judgeCopy.LimitKillCount - 1 < this.AwardFactor.Length)
            {
                result = this.AwardFactor[judgeCopy.LimitKillCount - 1];
            }
            return result;
        }

        
        private bool IsGongNengOpened(GameClient client, bool bHint = true)
        {
            return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("MoRiShenPan") && GlobalNew.IsGongNengOpened(client, GongNengIDs.MoRiJudge, bHint);
        }

        
        public bool OnInitGame(GameClient client)
        {
            int destX;
            int destY;
            bool result;
            if (!this.GetBirthPoint(out destX, out destY))
            {
                result = false;
            }
            else
            {
                client.ClientData.MapCode = this.MapCode;
                client.ClientData.PosX = destX;
                client.ClientData.PosY = destY;
                client.ClientData.FuBenSeqID = Global.GetClientKuaFuServerLoginData(client).FuBenSeqId;
                result = true;
            }
            return result;
        }

        
        public void OnLogOut(GameClient client)
        {
            MoRiJudgeCopy judgeCopy = null;
            lock (this.copyDict)
            {
                if (!this.copyDict.TryGetValue(client.ClientData.FuBenSeqID, out judgeCopy))
                {
                    return;
                }
            }
            lock (judgeCopy)
            {
                judgeCopy.RoleCount--;
                if (judgeCopy.m_eStatus != GameSceneStatuses.STATUS_END && judgeCopy.m_eStatus != GameSceneStatuses.STATUS_AWARD && judgeCopy.m_eStatus != GameSceneStatuses.STATUS_CLEAR)
                {
                    KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
                }
            }
        }

        
        public void AddCopyScene(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
        {
            if (copyMap.MapCode == this.MapCode)
            {
                int fuBenSeqId = copyMap.FuBenSeqID;
                int mapCode = copyMap.MapCode;
                lock (this.copyDict)
                {
                    MoRiJudgeCopy copy = null;
                    if (!this.copyDict.TryGetValue(fuBenSeqId, out copy))
                    {
                        copy = new MoRiJudgeCopy();
                        copy.MyCopyMap = copyMap;
                        copy.GameId = Global.GetClientKuaFuServerLoginData(client).GameId;
                        copy.StateTimeData.GameType = 3;
                        copy.StartTime = TimeUtil.NowDateTime();
                        copy.EndTime = copy.StartTime.AddMinutes((double)this.CopyMaxAliveMinutes);
                        copy.LimitKillCount = 0;
                        copy.RoleCount = 1;
                        copy.Passed = false;
                        this.copyDict[fuBenSeqId] = copy;
                    }
                    else
                    {
                        copy.RoleCount++;
                    }
                }
                FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, copyMap.FuBenSeqID, 0, copyMap.FubenMapID);
                copyMap.IsKuaFuCopy = true;
                copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)((this.CopyMaxAliveMinutes + 3) * 60000));
                GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 3);
            }
        }

        
        public void DelCopyScene(CopyMap copyMap)
        {
            if (copyMap != null && copyMap.MapCode == this.MapCode)
            {
                MoRiJudgeCopy judgeCopy = null;
                lock (this.copyDict)
                {
                    if (!this.copyDict.TryGetValue(copyMap.FuBenSeqID, out judgeCopy))
                    {
                        return;
                    }
                }
                lock (judgeCopy)
                {
                    if (judgeCopy.m_eStatus < GameSceneStatuses.STATUS_END)
                    {
                        judgeCopy.m_eStatus = GameSceneStatuses.STATUS_END;
                    }
                }
            }
        }

        
        public void TimerProc()
        {
            long nowMs = TimeUtil.NOW();
            if (nowMs >= this.NextHeartBeatMs)
            {
                this.NextHeartBeatMs = nowMs + 1020L;
                List<MoRiJudgeCopy> copyList = null;
                lock (this.copyDict)
                {
                    copyList = this.copyDict.Values.ToList<MoRiJudgeCopy>();
                }
                if (copyList != null && copyList.Count > 0)
                {
                    foreach (MoRiJudgeCopy judgeCopy in copyList)
                    {
                        lock (judgeCopy)
                        {
                            if (judgeCopy.m_eStatus == GameSceneStatuses.STATUS_NULL)
                            {
                                judgeCopy.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
                                judgeCopy.CurrStateBeginMs = nowMs;
                                judgeCopy.DeadlineMs = nowMs + (long)(this.CopyMaxAliveMinutes * 60 * 1000);
                                judgeCopy.StateTimeData.State = 2;
                                judgeCopy.StateTimeData.EndTicks = judgeCopy.DeadlineMs;
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, judgeCopy.StateTimeData, judgeCopy.MyCopyMap);
                            }
                            else if (judgeCopy.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
                            {
                                if (nowMs >= judgeCopy.CurrStateBeginMs + 1500L)
                                {
                                    judgeCopy.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
                                    judgeCopy.CurrStateBeginMs = nowMs;
                                }
                            }
                            else if (judgeCopy.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
                            {
                                if (nowMs >= judgeCopy.DeadlineMs || (nowMs >= judgeCopy.CurrStateBeginMs + 90000L && judgeCopy.RoleCount <= 0))
                                {
                                    judgeCopy.m_eStatus = GameSceneStatuses.STATUS_END;
                                    judgeCopy.CurrStateBeginMs = nowMs;
                                    break;
                                }
                                int nextMonsterIdx = -1;
                                if (judgeCopy.CurrMonsterIdx == -1)
                                {
                                    nextMonsterIdx = 0;
                                }
                                else if (judgeCopy.MonsterList[judgeCopy.CurrMonsterIdx].DeathMs > 0L && nowMs >= judgeCopy.MonsterList[judgeCopy.CurrMonsterIdx].DeathMs + 1300L)
                                {
                                    nextMonsterIdx = judgeCopy.CurrMonsterIdx + 1;
                                }
                                if (nextMonsterIdx != -1)
                                {
                                    if (nextMonsterIdx >= this.BossConfigList.Count)
                                    {
                                        judgeCopy.m_eStatus = GameSceneStatuses.STATUS_END;
                                        judgeCopy.CurrStateBeginMs = nowMs;
                                    }
                                    else
                                    {
                                        this.FlushMonster(judgeCopy, nextMonsterIdx);
                                    }
                                }
                            }
                            else if (judgeCopy.m_eStatus == GameSceneStatuses.STATUS_END)
                            {
                                GameManager.CopyMapMgr.KillAllMonster(judgeCopy.MyCopyMap);
                                judgeCopy.EndTime = TimeUtil.NowDateTime();
                                int roleCount = 0;
                                List<GameClient> clientList = judgeCopy.MyCopyMap.GetClientsList();
                                if (clientList != null && clientList.Count > 0)
                                {
                                    int combatSum = 0;
                                    foreach (GameClient client in clientList)
                                    {
                                        roleCount++;
                                        combatSum += client.ClientData.CombatForce;
                                        if (judgeCopy.Passed)
                                        {
                                            GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 1, 0, 3);
                                        }
                                        else
                                        {
                                            GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 0, 0, 1, 3);
                                        }
                                    }
                                    if (roleCount > 0)
                                    {
                                        int combatAvg = combatSum / roleCount;
                                    }
                                    if (judgeCopy.Passed)
                                    {
                                        GameManager.CopyMapMgr.CopyMapPassAwardForAll(clientList[0], judgeCopy.MyCopyMap, false);
                                    }
                                }
                                judgeCopy.m_eStatus = GameSceneStatuses.STATUS_AWARD;
                                judgeCopy.CurrStateBeginMs = nowMs;
                                judgeCopy.StateTimeData.State = 3;
                                judgeCopy.StateTimeData.EndTicks = nowMs + 30000L;
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, judgeCopy.StateTimeData, judgeCopy.MyCopyMap);
                            }
                            else if (judgeCopy.m_eStatus == GameSceneStatuses.STATUS_AWARD)
                            {
                                if (nowMs >= judgeCopy.CurrStateBeginMs + 30000L)
                                {
                                    lock (this.copyDict)
                                    {
                                        this.copyDict.Remove(judgeCopy.MyCopyMap.FuBenSeqID);
                                    }
                                    try
                                    {
                                        List<GameClient> clientList = judgeCopy.MyCopyMap.GetClientsList();
                                        if (clientList != null)
                                        {
                                            foreach (GameClient client in clientList)
                                            {
                                                KuaFuManager.getInstance().GotoLastMap(client);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        DataHelper.WriteExceptionLogEx(ex, "末日审判清场调度异常");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        
        private void FlushMonster(MoRiJudgeCopy judgeCopy, int nextMonsterIdx)
        {
            MoRiMonsterTag tag = new MoRiMonsterTag();
            tag.CopySeqId = judgeCopy.MyCopyMap.FuBenSeqID;
            tag.MonsterIdx = nextMonsterIdx;
            tag.ExtPropDict = null;
            if (nextMonsterIdx == this.BossConfigList.Count - 1)
            {
                tag.ExtPropDict = new Dictionary<int, float>();
                int i = 0;
                while (i < judgeCopy.MonsterList.Count && i < judgeCopy.CurrMonsterIdx)
                {
                    if (this.BossConfigList[i].KillLimitSecond != -1 && judgeCopy.MonsterList[i].DeathMs - judgeCopy.MonsterList[i].BirthMs <= (long)this.BossConfigList[i].KillLimitSecond * 1000L)
                    {
                        foreach (KeyValuePair<int, float> kvp in this.BossConfigList[i].ExtPropDict)
                        {
                            if (tag.ExtPropDict.ContainsKey(kvp.Key))
                            {
                                Dictionary<int, float> extPropDict;
                                int key;
                                (extPropDict = tag.ExtPropDict)[key = kvp.Key] = extPropDict[key] + kvp.Value;
                            }
                            else
                            {
                                tag.ExtPropDict.Add(kvp.Key, kvp.Value);
                            }
                        }
                    }
                    i++;
                }
            }
            GameManager.MonsterZoneMgr.AddDynamicMonsters(this.MapCode, this.BossConfigList[nextMonsterIdx].MonsterId, judgeCopy.MyCopyMap.CopyMapID, 1, this.BossConfigList[nextMonsterIdx].BirthX / this.copyMapGirdWidth, this.BossConfigList[nextMonsterIdx].BirthY / this.copyMapGirdHeight, 0, 0, SceneUIClasses.MoRiJudge, tag, null);
            judgeCopy.MonsterList.Add(new MoRiMonsterData
            {
                Id = this.BossConfigList[nextMonsterIdx].Id,
                BirthMs = TimeUtil.NOW(),
                DeathMs = -1L
            });
            judgeCopy.CurrMonsterIdx = nextMonsterIdx;
        }

        
        public void OnLoadDynamicMonsters(Monster monster)
        {
            MoRiMonsterTag tag = null;
            if (monster != null && (tag = (monster.Tag as MoRiMonsterTag)) != null)
            {
                MoRiJudgeCopy judgeCopy = null;
                lock (this.copyDict)
                {
                    if (!this.copyDict.TryGetValue(tag.CopySeqId, out judgeCopy))
                    {
                        return;
                    }
                }
                GameManager.ClientMgr.BroadSpecialCopyMapMessageStr(1305, string.Format("{0}:{1}:{2}:{3}", new object[]
                {
                    1,
                    this.BossConfigList[tag.MonsterIdx].Id,
                    judgeCopy.MonsterList[tag.MonsterIdx].BirthMs,
                    judgeCopy.MonsterList[tag.MonsterIdx].DeathMs
                }), judgeCopy.MyCopyMap, false);
                long toTick = TimeUtil.NowDateTime().Ticks + 36000000000L;
                if (tag.ExtPropDict != null)
                {
                    foreach (KeyValuePair<int, float> kvp in tag.ExtPropDict)
                    {
                        monster.TempPropsBuffer.AddTempExtProp(kvp.Key, (double)kvp.Value, toTick);
                    }
                }
            }
        }

        
        public bool ClientRelive(GameClient client)
        {
            bool result;
            if (client.ClientData.MapCode == this.MapCode)
            {
                int toPosX;
                int toPosY;
                if (!this.GetBirthPoint(out toPosX, out toPosY))
                {
                    result = false;
                }
                else
                {
                    client.ClientData.CurrentLifeV = client.ClientData.LifeV;
                    client.ClientData.CurrentMagicV = client.ClientData.MagicV;
                    client.ClientData.MoveAndActionNum = 0;
                    GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, toPosX, toPosY, -1);
                    Global.ClientRealive(client, toPosX, toPosY, -1);
                    result = true;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        private bool GetBirthPoint(out int toPosX, out int toPosY)
        {
            toPosX = -1;
            toPosY = -1;
            GameMap gameMap = null;
            bool result;
            if (!GameManager.MapMgr.DictMaps.TryGetValue(this.MapCode, out gameMap))
            {
                result = false;
            }
            else
            {
                int defaultBirthPosX = GameManager.MapMgr.DictMaps[this.MapCode].DefaultBirthPosX;
                int defaultBirthPosY = GameManager.MapMgr.DictMaps[this.MapCode].DefaultBirthPosY;
                int defaultBirthRadius = GameManager.MapMgr.DictMaps[this.MapCode].BirthRadius;
                Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this.MapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
                toPosX = (int)newPos.X;
                toPosY = (int)newPos.Y;
                result = true;
            }
            return result;
        }

        
        public void OnGMCommand(GameClient client, string[] cmdFields)
        {
            if (cmdFields[1] == "join")
            {
                this.ProcessMoRiJudgeJoin(client, 1301, null, null);
            }
            else if (cmdFields[1] == "quit")
            {
                this.ProcessMoRiJudgeQuit(client, 1302, null, null);
            }
        }

        
        public double GetCopyAwardRate(int copySeqId)
        {
            MoRiJudgeCopy judgeCopy = null;
            lock (this.copyDict)
            {
                if (!this.copyDict.TryGetValue(copySeqId, out judgeCopy))
                {
                    return 1.0;
                }
            }
            return this.CalcAwardRate(judgeCopy);
        }

        
        public void NotifyTimeStateAndBossEvent(GameClient client)
        {
            MoRiJudgeCopy judgeCopy = null;
            lock (this.copyDict)
            {
                if (!this.copyDict.TryGetValue(client.ClientData.FuBenSeqID, out judgeCopy))
                {
                    return;
                }
            }
            lock (judgeCopy)
            {
                client.sendCmd<GameSceneStateTimeData>(827, judgeCopy.StateTimeData, false);
                foreach (MoRiMonsterData boss in judgeCopy.MonsterList)
                {
                    if (boss.BirthMs > 0L)
                    {
                        GameManager.ClientMgr.BroadSpecialCopyMapMessageStr(1305, string.Format("{0}:{1}:{2}:{3}", new object[]
                        {
                            1,
                            boss.Id,
                            boss.BirthMs,
                            boss.DeathMs
                        }), judgeCopy.MyCopyMap, false);
                    }
                    if (boss.DeathMs > 0L)
                    {
                        GameManager.ClientMgr.BroadSpecialCopyMapMessageStr(1305, string.Format("{0}:{1}:{2}:{3}", new object[]
                        {
                            2,
                            boss.Id,
                            boss.BirthMs,
                            boss.DeathMs
                        }), judgeCopy.MyCopyMap, false);
                    }
                }
            }
        }

        
        private List<MoRiMonster> BossConfigList = new List<MoRiMonster>();

        
        private Dictionary<int, MoRiJudgeCopy> copyDict = new Dictionary<int, MoRiJudgeCopy>();

        
        private long NextHeartBeatMs = 0L;

        
        private int copyMapGirdWidth;

        
        private int copyMapGirdHeight;

        
        private int CopyMaxAliveMinutes = 15;
    }
}
