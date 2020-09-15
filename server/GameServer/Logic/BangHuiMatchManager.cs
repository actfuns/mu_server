using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
    // Token: 0x02000211 RID: 529
    public class BangHuiMatchManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
    {
        // Token: 0x060006CA RID: 1738 RVA: 0x000608A8 File Offset: 0x0005EAA8
        public static BangHuiMatchManager getInstance()
        {
            return BangHuiMatchManager.instance;
        }

        // Token: 0x060006CB RID: 1739 RVA: 0x000608C0 File Offset: 0x0005EAC0
        public bool initialize()
        {
            return this.InitConfig();
        }

        // Token: 0x060006CC RID: 1740 RVA: 0x000608E4 File Offset: 0x0005EAE4
        public bool initialize(ICoreInterface coreInterface)
        {
            ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("BangHuiMatchManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
            return true;
        }

        // Token: 0x060006CD RID: 1741 RVA: 0x00060924 File Offset: 0x0005EB24
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(1165, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1166, 2, 2, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1167, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1168, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1169, 2, 2, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1170, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1171, 2, 2, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1172, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1174, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1175, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1176, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1177, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1195, 4, 4, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1196, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1197, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            GlobalEventSource4Scene.getInstance().registerListener(23, 10000, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(24, 10000, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(25, 10000, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(26, 10000, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(33, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(30, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(27, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(29, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10032, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().registerListener(14, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().registerListener(28, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().registerListener(10, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().registerListener(11, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().registerListener(55, BangHuiMatchManager.getInstance());
            return true;
        }

        // Token: 0x060006CE RID: 1742 RVA: 0x00060BBC File Offset: 0x0005EDBC
        public bool showdown()
        {
            GlobalEventSource4Scene.getInstance().removeListener(23, 10000, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(24, 10000, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(25, 10000, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(26, 10000, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(33, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(30, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(27, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(29, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10032, 45, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().removeListener(14, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().removeListener(28, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().removeListener(10, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().removeListener(11, BangHuiMatchManager.getInstance());
            GlobalEventSource.getInstance().removeListener(55, BangHuiMatchManager.getInstance());
            return true;
        }

        // Token: 0x060006CF RID: 1743 RVA: 0x00060CEC File Offset: 0x0005EEEC
        public bool destroy()
        {
            return true;
        }

        // Token: 0x060006D0 RID: 1744 RVA: 0x00060D00 File Offset: 0x0005EF00
        public bool processCmd(GameClient client, string[] cmdParams)
        {
            return false;
        }

        // Token: 0x060006D1 RID: 1745 RVA: 0x00060D14 File Offset: 0x0005EF14
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            if (nID != 1165 && nID != 1168 && nID != 1169 && nID != 1166 && nID != 1195 && nID != 1196 && nID != 1197)
            {
                if (!this.IsGongNengOpened(client, false))
                {
                    GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "", GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                    return true;
                }
            }
            if (nID == 1195 || nID == 1196 || nID == 1197)
            {
                if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.BangHuiMatchGuess, true))
                {
                    GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                    return true;
                }
            }
            switch (nID)
            {
                case 1165:
                    return this.ProcessGetBangHuiMatchMainInfoCmd(client, nID, bytes, cmdParams);
                case 1166:
                    return this.ProcessGetBangHuiMatchRankInfoCmd(client, nID, bytes, cmdParams);
                case 1167:
                    return this.ProcessGetBangHuiMatchAnalysisDataCmd(client, nID, bytes, cmdParams);
                case 1168:
                    return this.ProcessGetBangHuiMatchAdmireDataCmd(client, nID, bytes, cmdParams);
                case 1169:
                    return this.ProcessBangHuiMatchAdmireCmd(client, nID, bytes, cmdParams);
                case 1170:
                    return this.ProcessBangHuiMatchJoinCmd(client, nID, bytes, cmdParams);
                case 1171:
                    return this.ProcessBangHuiMatchEnterCmd(client, nID, bytes, cmdParams);
                case 1172:
                    return this.ProcessGetBangHuiMatchStateCmd(client, nID, bytes, cmdParams);
                case 1173:
                    break;
                case 1174:
                    return this.ProcessGetBangHuiMatchAwardInfoCmd(client, nID, bytes, cmdParams);
                case 1175:
                    return this.ProcessGetBangHuiMatchAwardCmd(client, nID, bytes, cmdParams);
                case 1176:
                    return this.ProcessGetBangHuiMatchRankAwardCmd(client, nID, bytes, cmdParams);
                case 1177:
                    return this.ProcessGetBangHuiMatchRankInfoMiniCmd(client, nID, bytes, cmdParams);
                default:
                    switch (nID)
                    {
                        case 1195:
                            return this.ProcessBangHuiMatchGuess(client, nID, bytes, cmdParams);
                        case 1196:
                            return this.ProcessGetBangHuiMatchGuessInfo(client, nID, bytes, cmdParams);
                        case 1197:
                            return this.ProcessGetBangHuiMatchGuessAward(client, nID, bytes, cmdParams);
                    }
                    break;
            }
            return true;
        }

        // Token: 0x060006D2 RID: 1746 RVA: 0x00060F60 File Offset: 0x0005F160
        public void processEvent(EventObject eventObject)
        {
            int eventType = eventObject.getEventType();
            if (eventType == 28)
            {
                OnStartPlayGameEventObject e = eventObject as OnStartPlayGameEventObject;
                this.OnStartPlayGame(e.Client);
            }
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
            if (eventType == 11)
            {
                MonsterDeadEventObject e2 = eventObject as MonsterDeadEventObject;
                this.OnProcessMonsterDead(e2.getAttacker(), e2.getMonster());
            }
            if (eventType == 55)
            {
                PlayerLogoutFinishEventObject e3 = eventObject as PlayerLogoutFinishEventObject;
                this.OnLogoutFinish(e3.getPlayer());
            }
            if (eventType == 14)
            {
                PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
                if (null != playerInitGameEventObject)
                {
                    this.OnInitGame(playerInitGameEventObject.getPlayer());
                }
            }
        }

        // Token: 0x060006D3 RID: 1747 RVA: 0x00061068 File Offset: 0x0005F268
        public void processEvent(EventObjectEx eventObject)
        {
            int eventType = eventObject.EventType;
            int num = eventType;
            switch (num)
            {
                case 23:
                    {
                        PreBangHuiAddMemberEventObject e = eventObject as PreBangHuiAddMemberEventObject;
                        if (null != e)
                        {
                            eventObject.Handled = this.OnPreBangHuiAddMember(e);
                        }
                        break;
                    }
                case 24:
                    {
                        PreBangHuiRemoveMemberEventObject e2 = eventObject as PreBangHuiRemoveMemberEventObject;
                        if (null != e2)
                        {
                            eventObject.Handled = this.OnPreBangHuiRemoveMember(e2);
                        }
                        break;
                    }
                case 25:
                    {
                        PreBangHuiChangeZhiWuEventObject e3 = eventObject as PreBangHuiChangeZhiWuEventObject;
                        if (e3 != null && e3.TargetZhiWu == 1)
                        {
                            if ((long)e3.Player.ClientData.Faction == this.RuntimeData.ChengHaoBHid_Gold)
                            {
                                e3.ErrorCode = -201;
                                eventObject.Handled = true;
                                eventObject.Result = false;
                                GameManager.ClientMgr.NotifyImportantMsg(e3.Player, GLang.GetLang(2600, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                            }
                        }
                        break;
                    }
                case 26:
                    {
                        PostBangHuiChangeEventObject e4 = eventObject as PostBangHuiChangeEventObject;
                        if (e4 != null && null != e4.Player)
                        {
                            this.UpdateChengHaoBuffer(e4.Player);
                        }
                        break;
                    }
                case 27:
                    {
                        ProcessClickOnNpcEventObject e5 = eventObject as ProcessClickOnNpcEventObject;
                        if (null != e5)
                        {
                            if (null != e5.Npc)
                            {
                                int npcId = e5.Npc.NpcID;
                            }
                            if (this.OnSpriteClickOnNpc(e5.Client, e5.NpcId, e5.ExtensionID))
                            {
                                e5.Result = false;
                                e5.Handled = true;
                            }
                        }
                        break;
                    }
                case 28:
                case 31:
                case 32:
                    break;
                case 29:
                    {
                        OnClientChangeMapEventObject e6 = eventObject as OnClientChangeMapEventObject;
                        if (null != e6)
                        {
                            e6.Handled = (e6.Result = this.ClientChangeMap(e6.Client, ref e6.ToMapCode, ref e6.ToPosX, ref e6.ToPosY));
                        }
                        break;
                    }
                case 30:
                    {
                        OnCreateMonsterEventObject e7 = eventObject as OnCreateMonsterEventObject;
                        if (null != e7)
                        {
                            BHMatchQiZhiConfig qiZhiConfig = e7.Monster.Tag as BHMatchQiZhiConfig;
                            if (null != qiZhiConfig)
                            {
                                e7.Monster.Camp = qiZhiConfig.BattleWhichSide;
                                e7.Result = true;
                                e7.Handled = true;
                            }
                        }
                        break;
                    }
                case 33:
                    {
                        PreMonsterInjureEventObject obj = eventObject as PreMonsterInjureEventObject;
                        if (obj != null && obj.SceneType == 45)
                        {
                            Monster injureMonster = obj.Monster;
                            if (injureMonster != null)
                            {
                                if (this.IsQiZhiExtensionID(injureMonster.MonsterInfo.ExtensionID))
                                {
                                    this.RuntimeData.MonsterIDVsDamage.TryGetValue(injureMonster.MonsterInfo.ExtensionID, out obj.Injure);
                                    eventObject.Handled = true;
                                    eventObject.Result = true;
                                }
                            }
                        }
                        break;
                    }
                default:
                    if (num == 10032)
                    {
                        this.HandleNtfEnterEvent((eventObject as KFBHMatchNtfEnterData).Data);
                        eventObject.Handled = true;
                    }
                    break;
            }
        }

        // Token: 0x060006D4 RID: 1748 RVA: 0x000613B4 File Offset: 0x0005F5B4
        public bool InitConfig()
        {
            bool success = true;
            string fileName = "";
            lock (this.RuntimeData.Mutex)
            {
                try
                {
                    if (!this.RuntimeData.CommonConfigData.Load(Global.GameResPath("Config\\LeagueMatch.xml"), Global.GameResPath("Config\\LeagueOpen.xml"), Global.GameResPath("Config\\LeagueSuperList.xml"), Global.GameResPath("Config\\LeagueNewRandom.xml"), GameCoreInterface.getinstance().GetPlatformType().ToString()))
                    {
                        LogManager.WriteLog(LogTypes.Error, "BangHuiMatchService.InitConfig failed!", null, true);
                    }
                    foreach (BHMatchConfig item in this.RuntimeData.CommonConfigData.BHMatchConfigDict.Values)
                    {
                        string winAwardTag = (string)item.WinAwardTag;
                        string failAwardTag = (string)item.FailAwardTag;
                        AwardsItemList WinAwardsItemList = new AwardsItemList();
                        AwardsItemList LoseAwardsItemList = new AwardsItemList();
                        ConfigParser.ParseAwardsItemList(winAwardTag, ref WinAwardsItemList, '|', ',');
                        ConfigParser.ParseAwardsItemList(failAwardTag, ref LoseAwardsItemList, '|', ',');
                        item.WinAwardTag = WinAwardsItemList;
                        item.FailAwardTag = LoseAwardsItemList;
                    }
                    this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
                    fileName = "Config\\LeagueWar.xml";
                    string fullPathFileName = Global.GameResPath(fileName);
                    XElement xml = XElement.Load(fullPathFileName);
                    IEnumerable<XElement> nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        BHMatchQiZhiConfig item2 = new BHMatchQiZhiConfig();
                        item2.NPCID = (int)Global.GetSafeAttributeLong(node, "QiZuoID");
                        string strQiZuoSite = Global.GetSafeAttributeStr(node, "QiZuoSite");
                        string[] QiZuoSiteFields = strQiZuoSite.Split(new char[]
                        {
                            '|'
                        });
                        if (QiZuoSiteFields.Length == 2)
                        {
                            item2.PosX = Global.SafeConvertToInt32(QiZuoSiteFields[0]);
                            item2.PosY = Global.SafeConvertToInt32(QiZuoSiteFields[1]);
                        }
                        string strRebirthSite = Global.GetSafeAttributeStr(node, "RebirthSite");
                        string[] RebirthSiteFields = strRebirthSite.Split(new char[]
                        {
                            '|'
                        });
                        if (RebirthSiteFields.Length == 2)
                        {
                            item2.RebirthSiteX = Global.SafeConvertToInt32(RebirthSiteFields[0]);
                            item2.RebirthSiteY = Global.SafeConvertToInt32(RebirthSiteFields[1]);
                        }
                        item2.RebirthRadius = (int)Global.GetSafeAttributeLong(node, "RebirthRadius");
                        item2.ProduceTime = (int)Global.GetSafeAttributeLong(node, "ProduceTime");
                        item2.ProduceNum = (int)Global.GetSafeAttributeLong(node, "ProduceNum");
                        this.RuntimeData.NPCID2QiZhiConfigDict[item2.NPCID] = item2;
                    }
                    this.RuntimeData.MapBirthPointDict.Clear();
                    fileName = "Config\\LeagueBirthPoint.xml";
                    fullPathFileName = Global.GameResPath(fileName);
                    xml = XElement.Load(fullPathFileName);
                    nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        BHMatchBirthPoint item3 = new BHMatchBirthPoint();
                        item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item3.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
                        item3.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
                        item3.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
                        this.RuntimeData.MapBirthPointDict[item3.ID] = item3;
                    }
                    this.RuntimeData.RankAwardConfigList_Gold.Clear();
                    fileName = "Config\\LeagueSuperAward.xml";
                    fullPathFileName = Global.GameResPath(fileName);
                    xml = XElement.Load(fullPathFileName);
                    nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        BHMatchRankAwardConfig item4 = new BHMatchRankAwardConfig();
                        item4.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item4.BeginNum = (int)Global.GetSafeAttributeLong(node, "BeginNum");
                        item4.EndNum = (int)Global.GetSafeAttributeLong(node, "EndNum");
                        ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "GoodsOne"), ref item4.AwardsItemList, '|', ',');
                        this.RuntimeData.RankAwardConfigList_Gold.Add(item4);
                    }
                    this.RuntimeData.RankAwardConfigList_Rookie.Clear();
                    fileName = "Config\\LeagueNewAward.xml";
                    fullPathFileName = Global.GameResPath(fileName);
                    xml = XElement.Load(fullPathFileName);
                    nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        BHMatchRankAwardConfig item4 = new BHMatchRankAwardConfig();
                        item4.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item4.BeginNum = (int)Global.GetSafeAttributeLong(node, "BeginNum");
                        item4.EndNum = (int)Global.GetSafeAttributeLong(node, "EndNum");
                        ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "GoodsOne"), ref item4.AwardsItemList, '|', ',');
                        this.RuntimeData.RankAwardConfigList_Rookie.Add(item4);
                    }
                    this.RuntimeData.BHMatchGoldGuessConfigDict.Clear();
                    fileName = "Config\\LeagueSustain.xml";
                    fullPathFileName = Global.GameResPath(fileName);
                    xml = XElement.Load(fullPathFileName);
                    nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        BHMatchGoldGuessConfig item5 = new BHMatchGoldGuessConfig();
                        item5.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item5.Round = (int)Global.GetSafeAttributeLong(node, "Round");
                        item5.Cost = (int)Global.GetSafeAttributeLong(node, "Cost");
                        item5.WinAward = (int)Global.GetSafeAttributeLong(node, "WinAward");
                        item5.FaillAward = (int)Global.GetSafeAttributeLong(node, "FaillAward");
                        int[] MinLevelArray = Global.String2IntArray(Global.GetSafeAttributeStr(node, "MinLevel"), '|');
                        item5.UnionLevLimit = Global.GetUnionLevel(MinLevelArray[0], MinLevelArray[1], false);
                        this.RuntimeData.BHMatchGoldGuessConfigDict[item5.ID] = item5;
                    }
                    this.RuntimeData.BHMatchGodDamagePeriod = (int)GameManager.systemParamsList.GetParamValueIntByName("LeagueHurtTime", -1);
                    this.RuntimeData.BHMatchGodDamagePctList.Clear();
                    double[] doubleArray = GameManager.systemParamsList.GetParamValueDoubleArrayByName("LeagueHurt", ',');
                    foreach (double value in doubleArray)
                    {
                        this.RuntimeData.BHMatchGodDamagePctList.Add(value);
                    }
                    doubleArray = GameManager.systemParamsList.GetParamValueDoubleArrayByName("LeagueDeHurt", ',');
                    this.RuntimeData.BHMatchGodDebuffTemple = doubleArray[0];
                    this.RuntimeData.BHMatchGodDebuffQiZhi = doubleArray[1];
                    this.RuntimeData.GoldWingGoodsID = (int)GameManager.systemParamsList.GetParamValueIntByName("LeagueWing", -1);
                    this.RuntimeData.MonsterIDVsDamage.Clear();
                    string strMonsterIDVsDamage = GameManager.systemParamsList.GetParamValueByName("LeagueQiZhi");
                    string[] MonsterIDVsDamageFields = strMonsterIDVsDamage.Split(new char[]
                    {
                        '|'
                    });
                    for (int i = 0; i < MonsterIDVsDamageFields.Length; i++)
                    {
                        string[] tempFields = MonsterIDVsDamageFields[i].Split(new char[]
                        {
                            ','
                        });
                        int MonsterID = Global.SafeConvertToInt32(tempFields[0]);
                        int Damage = Global.SafeConvertToInt32(tempFields[1]);
                        this.RuntimeData.MonsterIDVsDamage[MonsterID] = Damage;
                        if (i == 0)
                        {
                            this.RuntimeData.BattleQiZhiMonsterID1 = MonsterID;
                        }
                        if (i == 1)
                        {
                            this.RuntimeData.BattleQiZhiMonsterID2 = MonsterID;
                        }
                    }
                    int[] intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("LeagueShenDian", ',');
                    this.RuntimeData.TempleProduceTime = intArray[0];
                    this.RuntimeData.TempleProduceNum = intArray[1];
                    intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("LeagueMvpNum", ',');
                    this.RuntimeData.MVPScoreFactorKill = intArray[0];
                    this.RuntimeData.MVPScoreFactorQiZhi = intArray[1];
                    this.RuntimeData.MVPScoreFactorTemple = intArray[2];
                    intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("LeagueNewNum", ',');
                    this.RuntimeData.CommonConfigData.RookieWinScoreFactor = intArray[0];
                    this.RuntimeData.CommonConfigData.RookieLoseScoreFactor = intArray[1];
                    this.RuntimeData.CommonConfigData.RookiePromotionNum = (int)GameManager.systemParamsList.GetParamValueIntByName("LeagueUp", -1);
                }
                catch (Exception ex)
                {
                    success = false;
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
                }
            }
            return success;
        }

        // Token: 0x060006D5 RID: 1749 RVA: 0x00061DB0 File Offset: 0x0005FFB0
        private int GetCurrentBHMatchGoldRound()
        {
            int GoldRound = 0;
            BHMatchConfig matchGoldConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(1);
            BHMatchConfig matchRookieConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(2);
            if (this.BHMatchSyncDataCache.CurSeasonID_Rookie == this.BHMatchSyncDataCache.CurSeasonID_Gold)
            {
                if (this.BHMatchSyncDataCache.RoundGoldReal > matchGoldConfig.RoundNum)
                {
                    GoldRound = matchGoldConfig.RoundNum + 1;
                }
                else if (this.BHMatchSyncDataCache.RoundGoldReal <= matchGoldConfig.RoundNum)
                {
                    GoldRound = this.BHMatchSyncDataCache.RoundGoldReal;
                }
            }
            else
            {
                GoldRound = this.BHMatchSyncDataCache.RoundGoldReal;
            }
            return GoldRound;
        }

        // Token: 0x060006D6 RID: 1750 RVA: 0x00062004 File Offset: 0x00060204
        public bool ProcessGetBangHuiMatchMainInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                BangHuiMatchMainInfo mainInfo = new BangHuiMatchMainInfo();
                if (!this.RuntimeData.CommonConfigData.CheckOpenState(TimeUtil.NowDateTime()))
                {
                    client.sendCmd<BangHuiMatchMainInfo>(nID, mainInfo, false);
                    return true;
                }
                mainInfo.round = this.GetCurrentBHMatchGoldRound();
                List<BangHuiMatchPKInfo> pkinfoList = null;
                lock (this.RuntimeData.Mutex)
                {
                    if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
                    {
                        pkinfoList = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
                    }
                }
                foreach (BangHuiMatchPKInfo item2 in pkinfoList.FindAll((BangHuiMatchPKInfo x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && (int)x.round == mainInfo.round))
                {
                    mainInfo.CurRoundPKInfo.Add((BangHuiMatchPKInfo)item2.Clone());
                }
                foreach (BangHuiMatchPKInfo item2 in pkinfoList.FindAll((BangHuiMatchPKInfo x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && (int)x.round == mainInfo.round - 1))
                {
                    mainInfo.LastRoundPKInfo.Add((BangHuiMatchPKInfo)item2.Clone());
                }
                if (mainInfo.CurRoundPKInfo != null && mainInfo.CurRoundPKInfo.Count != 0)
                {
                    DateTime curSeasonTm = BangHuiMatchUtils.GetSeasonDateTm(this.BHMatchSyncDataCache.CurSeasonID_Gold);
                    BHMatchConfig matchGoldConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(1);
                    BHMatchConfig sceneItem = null;
                    BangHuiMatchGameStates timeState = BangHuiMatchGameStates.None;
                    this.CheckCondition(client, 1, ref sceneItem, ref timeState);
                    if (TimeUtil.NowDateTime() < curSeasonTm)
                    {
                        timeState = BangHuiMatchGameStates.Wait;
                    }
                    else if (this.BHMatchSyncDataCache.CurSeasonID_Rookie == this.BHMatchSyncDataCache.CurSeasonID_Gold && this.BHMatchSyncDataCache.RoundGoldReal > matchGoldConfig.RoundNum)
                    {
                        timeState = BangHuiMatchGameStates.End;
                    }
                    else if (timeState == BangHuiMatchGameStates.SignUp || timeState == BangHuiMatchGameStates.Wait)
                    {
                        timeState = BangHuiMatchGameStates.Wait;
                    }
                    mainInfo.seasonid = this.BHMatchSyncDataCache.CurSeasonID_Gold;
                    mainInfo.timestate = (int)timeState;
                }
                List<BHMatchSupportData> supportList = client.ClientData.BHMatchSupportList.FindAll((BHMatchSupportData x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && x.round == mainInfo.round);
                using (List<BangHuiMatchPKInfo>.Enumerator enumerator = mainInfo.CurRoundPKInfo.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BangHuiMatchPKInfo item = enumerator.Current;
                        BHMatchSupportData supportData = supportList.Find((BHMatchSupportData x) => x.bhid1 == item.bhid1 && x.bhid2 == item.bhid2);
                        if (null != supportData)
                        {
                            item.guess = (byte)supportData.guess;
                        }
                    }
                }
                supportList = client.ClientData.BHMatchSupportList.FindAll((BHMatchSupportData x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && x.round == mainInfo.round - 1);
                using (List<BangHuiMatchPKInfo>.Enumerator enumerator = mainInfo.LastRoundPKInfo.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BangHuiMatchPKInfo item = enumerator.Current;
                        BHMatchSupportData supportData = supportList.Find((BHMatchSupportData x) => x.bhid1 == item.bhid1 && x.bhid2 == item.bhid2);
                        if (null != supportData)
                        {
                            item.guess = (byte)supportData.guess;
                        }
                    }
                }
                client.sendCmd<BangHuiMatchMainInfo>(nID, mainInfo, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006D7 RID: 1751 RVA: 0x0006252C File Offset: 0x0006072C
        private BangHuiMatchRankInfo SearchMyRankInfo(GameClient client, int type, List<BangHuiMatchRankInfo> rankInfoList)
        {
            BangHuiMatchRankInfo myRankInfo = null;
            BangHuiMatchRankInfo result;
            if (rankInfoList.Count == 0)
            {
                result = myRankInfo;
            }
            else
            {
                switch (type)
                {
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                        myRankInfo = rankInfoList.Find((BangHuiMatchRankInfo x) => x.Key == client.ClientData.RoleID);
                        goto IL_9D;
                }
                myRankInfo = rankInfoList.Find((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
                IL_9D:
                result = myRankInfo;
            }
            return result;
        }

        // Token: 0x060006D8 RID: 1752 RVA: 0x000625E0 File Offset: 0x000607E0
        private BangHuiMatchRankInfo GeneralRankInfo(GameClient client, int type)
        {
            BHMatchBHData bhData = null;
            BangHuiMatchRankInfo myRankInfo = new BangHuiMatchRankInfo();
            List<int> RoleAnalysisData = this.GetBHMatchRoleAnalysisData(client);
            BangHuiMatchRankInfo result;
            if (null == RoleAnalysisData)
            {
                result = null;
            }
            else
            {
                switch (type)
                {
                    case 0:
                    case 2:
                    case 8:
                        bhData = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(1, client.ClientData.Faction);
                        if (null == bhData)
                        {
                            return null;
                        }
                        if (type == 0)
                        {
                            myRankInfo.Value = bhData.last_win;
                        }
                        else if (type == 2)
                        {
                            myRankInfo.Value = bhData.hist_champion;
                        }
                        else if (type == 8)
                        {
                            myRankInfo.Value = bhData.cur_win;
                        }
                        break;
                    case 1:
                    case 3:
                    case 9:
                        bhData = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(2, client.ClientData.Faction);
                        if (null == bhData)
                        {
                            return null;
                        }
                        if (type == 1)
                        {
                            myRankInfo.Value = bhData.last_score;
                        }
                        else if (type == 3)
                        {
                            myRankInfo.Value = bhData.hist_champion;
                        }
                        else if (type == 9)
                        {
                            myRankInfo.Value = bhData.cur_score;
                        }
                        break;
                    case 4:
                        myRankInfo.Value = RoleAnalysisData[4];
                        break;
                    case 5:
                        myRankInfo.Value = RoleAnalysisData[7];
                        break;
                    case 6:
                        myRankInfo.Value = RoleAnalysisData[2];
                        break;
                    case 7:
                        myRankInfo.Value = RoleAnalysisData[5];
                        break;
                    case 10:
                        myRankInfo.Value = RoleAnalysisData[3];
                        break;
                    case 11:
                        myRankInfo.Value = RoleAnalysisData[6];
                        break;
                    case 12:
                    case 13:
                        myRankInfo.Value = RoleAnalysisData[8];
                        break;
                }
                if (null != bhData)
                {
                    myRankInfo.Key = client.ClientData.Faction;
                    myRankInfo.Param1 = Global.FormatBangHuiNameWithZone(bhData.zoneid_bh, bhData.bhname);
                    myRankInfo.Param2 = Global.FormatRoleNameWithZoneId2(client);
                    myRankInfo.Key = client.ClientData.Faction;
                }
                else
                {
                    myRankInfo.Key = client.ClientData.RoleID;
                    myRankInfo.Param1 = Global.FormatRoleNameWithZoneId2(client);
                }
                result = myRankInfo;
            }
            return result;
        }

        // Token: 0x060006D9 RID: 1753 RVA: 0x000628B4 File Offset: 0x00060AB4
        private int GetBangHuiMatchRankNum(GameClient client, int ranktype)
        {
            List<BangHuiMatchRankInfo> rankInfoList = new List<BangHuiMatchRankInfo>();
            lock (this.RuntimeData.Mutex)
            {
                if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
                {
                    this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(ranktype, out rankInfoList);
                }
            }
            int result;
            if (rankInfoList.Count == 0)
            {
                result = 0;
            }
            else
            {
                int rankNum;
                switch (ranktype)
                {
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                        rankNum = rankInfoList.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.RoleID);
                        goto IL_10B;
                }
                rankNum = rankInfoList.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
                IL_10B:
                if (-1 == rankNum)
                {
                    result = 0;
                }
                else
                {
                    result = rankNum + 1;
                }
            }
            return result;
        }

        // Token: 0x060006DA RID: 1754 RVA: 0x000629F8 File Offset: 0x00060BF8
        public bool ProcessGetBangHuiMatchRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleID = Global.SafeConvertToInt32(cmdParams[0]);
                int type = Global.SafeConvertToInt32(cmdParams[1]);
                List<BangHuiMatchRankInfo> rankInfoList = new List<BangHuiMatchRankInfo>();
                lock (this.RuntimeData.Mutex)
                {
                    if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
                    {
                        this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(type, out rankInfoList);
                    }
                }
                List<BangHuiMatchRankInfo> returnRankInfoList = new List<BangHuiMatchRankInfo>(rankInfoList);
                if (returnRankInfoList.Count != 0 && type > 7 && type != 8)
                {
                    BangHuiMatchRankInfo myRankInfo = this.SearchMyRankInfo(client, type, returnRankInfoList);
                    if (null != myRankInfo)
                    {
                        returnRankInfoList.Add(myRankInfo);
                    }
                    else
                    {
                        myRankInfo = this.GeneralRankInfo(client, type);
                        if (null != myRankInfo)
                        {
                            returnRankInfoList.Add(myRankInfo);
                        }
                    }
                }
                client.sendCmd<List<BangHuiMatchRankInfo>>(nID, returnRankInfoList, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006DB RID: 1755 RVA: 0x00062B38 File Offset: 0x00060D38
        public bool ProcessGetBangHuiMatchAnalysisDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                string gamestate = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(client.ClientData.Faction);
                if (string.IsNullOrEmpty(gamestate))
                {
                    return true;
                }
                string[] gamestateFields = gamestate.Split(new char[]
                {
                    ':'
                });
                if (gamestateFields.Length != 2)
                {
                    return true;
                }
                int matchType = Global.SafeConvertToInt32(gamestateFields[0]);
                int signState = Global.SafeConvertToInt32(gamestateFields[1]);
                BHMatchBHData bhGoldData = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(1, client.ClientData.Faction);
                BHMatchBHData bhRookieData = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(2, client.ClientData.Faction);
                BangHuiAnalysisInfo analysisInfo = new BangHuiAnalysisInfo();
                analysisInfo.listAnalysisData = new List<int>(new int[24]);
                List<int> roleAnalysisData = this.GetBHMatchRoleAnalysisData(client);
                if (null == roleAnalysisData)
                {
                    client.sendCmd<BangHuiAnalysisInfo>(nID, analysisInfo, false);
                    return true;
                }
                List<int> roleAnalysisExData = this.GetBHMatchRAnalysisExData(client);
                analysisInfo.listAnalysisData[0] = roleAnalysisExData[0];
                analysisInfo.listAnalysisData[1] = roleAnalysisExData[1];
                analysisInfo.listAnalysisData[4] = roleAnalysisExData[2];
                analysisInfo.listAnalysisData[5] = roleAnalysisExData[3];
                analysisInfo.listAnalysisData[2] = roleAnalysisData[2];
                analysisInfo.listAnalysisData[3] = this.GetBangHuiMatchRankNum(client, 6);
                analysisInfo.listAnalysisData[6] = roleAnalysisData[5];
                analysisInfo.listAnalysisData[7] = this.GetBangHuiMatchRankNum(client, 7);
                analysisInfo.listAnalysisData[8] = roleAnalysisData[8];
                analysisInfo.listAnalysisData[9] = roleAnalysisData[9];
                analysisInfo.listAnalysisData[10] = roleAnalysisData[10];
                analysisInfo.listAnalysisData[11] = 0;
                analysisInfo.listAnalysisData[12] = ((bhGoldData != null) ? bhGoldData.hist_champion : 0);
                analysisInfo.listAnalysisData[13] = this.GetBangHuiMatchRankNum(client, 2);
                analysisInfo.listAnalysisData[14] = ((bhGoldData != null) ? bhGoldData.best_record : 0);
                analysisInfo.listAnalysisData[15] = ((bhGoldData != null) ? bhGoldData.hist_win : 0);
                analysisInfo.listAnalysisData[16] = 0;
                analysisInfo.listAnalysisData[17] = ((bhRookieData != null) ? bhRookieData.hist_champion : 0);
                analysisInfo.listAnalysisData[18] = this.GetBangHuiMatchRankNum(client, 3);
                analysisInfo.listAnalysisData[19] = ((bhGoldData != null) ? bhGoldData.best_record : 0);
                analysisInfo.listAnalysisData[20] = ((bhRookieData != null) ? bhRookieData.hist_win : 0);
                analysisInfo.listAnalysisData[21] = 0;
                int bhkill = 0;
                bhkill += ((bhGoldData != null) ? bhGoldData.hist_kill : 0);
                bhkill += ((bhRookieData != null) ? bhRookieData.hist_kill : 0);
                analysisInfo.listAnalysisData[22] = bhkill;
                int bhbullshit = 0;
                bhbullshit += ((bhGoldData != null) ? bhGoldData.hist_bullshit : 0);
                bhbullshit += ((bhRookieData != null) ? bhRookieData.hist_bullshit : 0);
                analysisInfo.listAnalysisData[23] = bhbullshit;
                client.sendCmd<BangHuiAnalysisInfo>(nID, analysisInfo, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006DC RID: 1756 RVA: 0x00062F00 File Offset: 0x00061100
        public bool ProcessGetBangHuiMatchAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleID = Convert.ToInt32(cmdParams[0]);
                client.sendCmd<BHMatchKingShowData>(nID, new BHMatchKingShowData
                {
                    AdmireCount = Global.GetBHMatchAdmireCount(client),
                    RoleData4Selector = Global.RoleDataEx2RoleData4Selector(this.OwnerRoleData)
                }, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006DD RID: 1757 RVA: 0x00062F7C File Offset: 0x0006117C
        public bool ProcessBangHuiMatchAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleID = Convert.ToInt32(cmdParams[0]);
                int type = Convert.ToInt32(cmdParams[1]);
                MoBaiData MoBaiConfig = null;
                string strcmd;
                if (!Data.MoBaiDataInfoList.TryGetValue(6, out MoBaiConfig))
                {
                    strcmd = string.Format("{0}", -2);
                    client.sendCmd(nID, strcmd, false);
                    return true;
                }
                if (client.ClientData.ChangeLifeCount < MoBaiConfig.MinZhuanSheng || (client.ClientData.ChangeLifeCount == MoBaiConfig.MinZhuanSheng && client.ClientData.Level < MoBaiConfig.MinLevel))
                {
                    strcmd = string.Format("{0}", -2);
                    client.sendCmd(nID, strcmd, false);
                    return true;
                }
                int nRealyNum = MoBaiConfig.AdrationMaxLimit;
                int AdmireCount = Global.GetBHMatchAdmireCount(client);
                if (this.OwnerRoleData != null && client.ClientData.RoleID == this.OwnerRoleData.RoleID)
                {
                    nRealyNum += MoBaiConfig.ExtraNumber;
                }
                int nVIPLev = client.ClientData.VipLevel;
                int[] nArrayVIPAdded = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPMoBaiNum", ',');
                if (nVIPLev > VIPEumValue.VIPENUMVALUE_MAXLEVEL || nArrayVIPAdded.Length < 1)
                {
                    strcmd = string.Format("{0}", -2);
                    client.sendCmd(nID, strcmd, false);
                    return true;
                }
                nRealyNum += nArrayVIPAdded[nVIPLev];
                double awardmuti = 0.0;
                JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
                if (null != activity)
                {
                    JieRiMultConfig config = activity.GetConfig(12);
                    if (null != config)
                    {
                        awardmuti += config.GetMult();
                    }
                }
                SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
                if (null != spAct)
                {
                    awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_Admire);
                }
                awardmuti = Math.Max(1.0, awardmuti);
                nRealyNum = (int)((double)nRealyNum * awardmuti);
                if (AdmireCount >= nRealyNum)
                {
                    strcmd = string.Format("{0}", -3);
                    client.sendCmd(nID, strcmd, false);
                    return true;
                }
                double nRate;
                if (client.ClientData.ChangeLifeCount == 0)
                {
                    nRate = 1.0;
                }
                else
                {
                    nRate = Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
                }
                if (type == 1)
                {
                    if (!Global.SubBindTongQianAndTongQian(client, MoBaiConfig.NeedJinBi, "膜拜战盟联赛盟主"))
                    {
                        strcmd = string.Format("{0}", -4);
                        client.sendCmd(nID, strcmd, false);
                        return true;
                    }
                    long nExp = (long)(nRate * (double)MoBaiConfig.JinBiExpAward);
                    if (nExp > 0L)
                    {
                        GameManager.ClientMgr.ProcessRoleExperience(client, nExp, true, true, false, "none");
                    }
                    if (MoBaiConfig.JinBiZhanGongAward > 0)
                    {
                        GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.JinBiZhanGongAward, AddBangGongTypes.BHMatchMoBai, 0);
                    }
                    if (MoBaiConfig.LingJingAwardByJinBi > 0)
                    {
                        GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByJinBi, "膜拜战盟联赛盟主", true, true, false);
                    }
                    if (MoBaiConfig.ShenLiJingHuaByJinBi > 0)
                    {
                        GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, MoBaiConfig.ShenLiJingHuaByJinBi, "膜拜战盟联赛盟主", true, true);
                    }
                }
                else if (type == 2)
                {
                    if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MoBaiConfig.NeedZuanShi, "膜拜战盟联赛盟主", true, true, false, DaiBiSySType.None))
                    {
                        strcmd = string.Format("{0}", -5);
                        client.sendCmd(nID, strcmd, false);
                        return true;
                    }
                    int nExp2 = (int)(nRate * (double)MoBaiConfig.ZuanShiExpAward);
                    if (nExp2 > 0)
                    {
                        GameManager.ClientMgr.ProcessRoleExperience(client, (long)nExp2, true, true, false, "none");
                    }
                    if (MoBaiConfig.ZuanShiZhanGongAward > 0)
                    {
                        GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.ZuanShiZhanGongAward, AddBangGongTypes.BHMatchMoBai, 0);
                    }
                    if (MoBaiConfig.LingJingAwardByZuanShi > 0)
                    {
                        GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByZuanShi, "膜拜战盟联赛盟主", true, true, false);
                    }
                    if (MoBaiConfig.ShenLiJingHuaByZuanShi > 0)
                    {
                        GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, MoBaiConfig.ShenLiJingHuaByZuanShi, "膜拜战盟联赛盟主", true, true);
                    }
                }
                Global.ProcessIncreaseBHMatchAdmireCount(client);
                strcmd = string.Format("{0}", 1);
                client.sendCmd(nID, strcmd, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006DE RID: 1758 RVA: 0x000634D4 File Offset: 0x000616D4
        public bool ProcessBangHuiMatchJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                if (this.IsGongNengOpened(client, false))
                {
                    if (client.ClientData.BHZhiWu != 1)
                    {
                        result = -1002;
                    }
                    else
                    {
                        BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(client.ClientData.Faction, 0);
                        if (null == bangHuiMiniData)
                        {
                            result = -1001;
                        }
                        else
                        {
                            string gamestate = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(client.ClientData.Faction);
                            if (string.IsNullOrEmpty(gamestate))
                            {
                                result = -11003;
                            }
                            else
                            {
                                string[] gamestateFields = gamestate.Split(new char[]
                                {
                                    ':'
                                });
                                if (gamestateFields.Length != 2)
                                {
                                    result = -11003;
                                }
                                else
                                {
                                    int matchType = Global.SafeConvertToInt32(gamestateFields[0]);
                                    int signState = Global.SafeConvertToInt32(gamestateFields[1]);
                                    if (matchType != 2)
                                    {
                                        result = -5;
                                    }
                                    else
                                    {
                                        BHMatchConfig sceneItem = null;
                                        BangHuiMatchGameStates state = BangHuiMatchGameStates.None;
                                        if (!this.CheckMap(client))
                                        {
                                            result = -21;
                                        }
                                        else
                                        {
                                            result = this.CheckCondition(client, matchType, ref sceneItem, ref state);
                                        }
                                        if (state != BangHuiMatchGameStates.SignUp)
                                        {
                                            result = -2001;
                                        }
                                        else if (signState == -4005)
                                        {
                                            result = -12;
                                        }
                                        if (result >= 0)
                                        {
                                            result = TianTiClient.getInstance().RookieSignUp_BHMatch(client.ClientData.Faction, bangHuiMiniData.ZoneID, bangHuiMiniData.BHName, client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.ZoneID);
                                            if (result >= 0)
                                            {
                                                client.ClientData.SignUpGameType = 24;
                                            }
                                        }
                                    }
                                }
                            }
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

        // Token: 0x060006DF RID: 1759 RVA: 0x000636E0 File Offset: 0x000618E0
        public bool ProcessBangHuiMatchEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int bhid = Global.SafeConvertToInt32(cmdParams[1]);
                if (client.ClientData.GuanZhanGM == 0)
                {
                    bhid = client.ClientData.Faction;
                }
                int result;
                if (!this.IsGongNengOpened(client, true))
                {
                    result = -13;
                }
                else
                {
                    string gamestate = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(bhid);
                    if (string.IsNullOrEmpty(gamestate))
                    {
                        result = -11003;
                    }
                    else
                    {
                        string[] gamestateFields = gamestate.Split(new char[]
                        {
                            ':'
                        });
                        if (gamestateFields.Length != 2)
                        {
                            result = -11003;
                        }
                        else
                        {
                            int matchType = Global.SafeConvertToInt32(gamestateFields[0]);
                            int signState = Global.SafeConvertToInt32(gamestateFields[1]);
                            BHMatchConfig sceneItem = null;
                            BangHuiMatchGameStates state = BangHuiMatchGameStates.None;
                            if (!this.CheckMap(client))
                            {
                                result = -21;
                            }
                            else
                            {
                                result = this.CheckCondition(client, matchType, ref sceneItem, ref state);
                                if (state != BangHuiMatchGameStates.Start)
                                {
                                    result = -2001;
                                }
                                else
                                {
                                    KuaFuServerInfo kfserverInfo = null;
                                    BHMatchFuBenData fubenData = TianTiClient.getInstance().GetFuBenDataByBhid_BHMatch(bhid);
                                    if (fubenData == null || !KuaFuManager.getInstance().TryGetValue(fubenData.ServerId, out kfserverInfo))
                                    {
                                        result = -11000;
                                    }
                                    else
                                    {
                                        KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
                                        if (null != clientKuaFuServerLoginData)
                                        {
                                            clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
                                            clientKuaFuServerLoginData.GameId = fubenData.GameId;
                                            clientKuaFuServerLoginData.GameType = 24;
                                            clientKuaFuServerLoginData.EndTicks = 0L;
                                            clientKuaFuServerLoginData.ServerId = client.ServerId;
                                            clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
                                            clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
                                            clientKuaFuServerLoginData.FuBenSeqId = bhid;
                                        }
                                        GlobalNew.RecordSwitchKuaFuServerLog(client);
                                        client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
                                    }
                                }
                            }
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

        // Token: 0x060006E0 RID: 1760 RVA: 0x00063968 File Offset: 0x00061B68
        public bool ProcessGetBangHuiMatchStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int lastMatchType = 0;
                int rankNum = 51;
                if (!this.RuntimeData.CommonConfigData.CheckOpenState(TimeUtil.NowDateTime()))
                {
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                    {
                        0,
                        0,
                        lastMatchType,
                        rankNum,
                        this.BHMatchSyncDataCache.CurSeasonID_Gold
                    }), false);
                    return true;
                }
                string gamestate = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(client.ClientData.Faction);
                if (string.IsNullOrEmpty(gamestate))
                {
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                    {
                        0,
                        0,
                        lastMatchType,
                        rankNum,
                        this.BHMatchSyncDataCache.CurSeasonID_Gold
                    }), false);
                    return true;
                }
                string[] gamestateFields = gamestate.Split(new char[]
                {
                    ':'
                });
                if (gamestateFields.Length != 2)
                {
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                    {
                        0,
                        0,
                        lastMatchType,
                        rankNum,
                        this.BHMatchSyncDataCache.CurSeasonID_Gold
                    }), false);
                    return true;
                }
                int matchType = Global.SafeConvertToInt32(gamestateFields[0]);
                int signState = Global.SafeConvertToInt32(gamestateFields[1]);
                int CurSeasonID = (matchType == 1) ? this.BHMatchSyncDataCache.CurSeasonID_Gold : this.BHMatchSyncDataCache.CurSeasonID_Rookie;
                DateTime curSeasonTm = BangHuiMatchUtils.GetSeasonDateTm(CurSeasonID);
                BHMatchConfig sceneItem = null;
                BangHuiMatchGameStates timeState = BangHuiMatchGameStates.None;
                this.CheckCondition(client, matchType, ref sceneItem, ref timeState);
                if (signState == -4009)
                {
                    timeState = BangHuiMatchGameStates.Bye;
                }
                else if (signState == -4005)
                {
                    BHMatchConfig matchGoldConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(1);
                    BHMatchConfig matchRookieConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(2);
                    if (matchType == 1 && TimeUtil.NowDateTime() < curSeasonTm)
                    {
                        timeState = BangHuiMatchGameStates.Wait;
                    }
                    else if ((matchType == 1 && this.BHMatchSyncDataCache.CurSeasonID_Rookie == this.BHMatchSyncDataCache.CurSeasonID_Gold && this.BHMatchSyncDataCache.RoundGoldReal > matchGoldConfig.RoundNum) || (matchType == 2 && this.BHMatchSyncDataCache.CurSeasonID_Rookie != this.BHMatchSyncDataCache.CurSeasonID_Gold))
                    {
                        timeState = BangHuiMatchGameStates.End;
                    }
                    else if (timeState == BangHuiMatchGameStates.SignUp || timeState == BangHuiMatchGameStates.Wait)
                    {
                        timeState = BangHuiMatchGameStates.Wait;
                    }
                }
                else if (matchType == 2 && TimeUtil.NowDateTime() < curSeasonTm)
                {
                    timeState = BangHuiMatchGameStates.End;
                }
                else if (timeState == BangHuiMatchGameStates.Wait || timeState == BangHuiMatchGameStates.Start)
                {
                    timeState = BangHuiMatchGameStates.NotJoin;
                }
                List<BangHuiMatchRankInfo> lastGoldRank = null;
                List<BangHuiMatchRankInfo> lastRookieRank = null;
                lock (this.RuntimeData.Mutex)
                {
                    this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(0, out lastGoldRank);
                    this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(1, out lastRookieRank);
                }
                int num;
                if (lastGoldRank == null)
                {
                    num = -1;
                }
                else
                {
                    num = lastGoldRank.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
                }
                int goldIndex = num;
                int num2;
                if (lastRookieRank == null)
                {
                    num2 = -1;
                }
                else
                {
                    num2 = lastRookieRank.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
                }
                int rookieIndex = num2;
                if (-1 != goldIndex)
                {
                    lastMatchType = 1;
                    rankNum = goldIndex + 1;
                }
                else if (TianTiClient.getInstance().CheckRookieJoinLast_BHMatch(client.ClientData.Faction))
                {
                    lastMatchType = 2;
                }
                if (-1 != rookieIndex)
                {
                    lastMatchType = 2;
                    rankNum = rookieIndex + 1;
                }
                if (0 != lastMatchType)
                {
                    int LastMatchSeasonID = (lastMatchType == 1) ? this.BHMatchSyncDataCache.CurSeasonID_Gold : this.BHMatchSyncDataCache.CurSeasonID_Rookie;
                    DateTime lastMatchSeasonTm = this.GetLastSeasonLastMatchEndTime(lastMatchType);
                    int EnterBHUnixSecs = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
                    DateTime enterBHTm = new DateTime(DataHelper.UnixSecondsToTicks(EnterBHUnixSecs) * 10000L);
                    if (enterBHTm > lastMatchSeasonTm)
                    {
                        lastMatchType = 0;
                        rankNum = 0;
                    }
                    else
                    {
                        List<int> RankAwardsHist = Global.GetRoleParamsIntListFromDB(client, "42");
                        if (RankAwardsHist.Count == 2 && RankAwardsHist[lastMatchType - 1] == LastMatchSeasonID)
                        {
                            lastMatchType = 0;
                            rankNum = 0;
                        }
                    }
                }
                string awardsInfo = Global.GetRoleParamByName(client, "39");
                if (!string.IsNullOrEmpty(awardsInfo))
                {
                    timeState = BangHuiMatchGameStates.Awards;
                }
                client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                {
                    matchType,
                    (int)timeState,
                    lastMatchType,
                    rankNum,
                    this.BHMatchSyncDataCache.CurSeasonID_Gold
                }), false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006E1 RID: 1761 RVA: 0x00063F80 File Offset: 0x00062180
        public bool ProcessGetBangHuiMatchAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                string awardsInfo = Global.GetRoleParamByName(client, "39");
                if (!string.IsNullOrEmpty(awardsInfo))
                {
                    int lastGroupId = 0;
                    int success = 0;
                    int mvprid = 0;
                    List<string> awardsParamList = Global.StringToList(awardsInfo, '&');
                    if (awardsParamList.Count != 6 && !string.IsNullOrEmpty(awardsInfo))
                    {
                        byte[] strBytes = Convert.FromBase64String(awardsInfo);
                        awardsInfo = new UTF8Encoding().GetString(strBytes);
                        awardsParamList = Global.StringToList(awardsInfo, '&');
                    }
                    ConfigParser.ParseStrInt3(awardsInfo, ref lastGroupId, ref success, ref mvprid, '&');
                    if (awardsParamList.Count >= 6 && lastGroupId > 0)
                    {
                        string mvpname = awardsParamList[3];
                        int mvpocc = Global.SafeConvertToInt32(awardsParamList[4]);
                        int mvpsex = Global.SafeConvertToInt32(awardsParamList[5]);
                        BHMatchConfig lastSceneItem = null;
                        if (this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue(lastGroupId, out lastSceneItem))
                        {
                            this.NtfCanGetAward(client, success, lastSceneItem, mvprid, mvpname, mvpocc, mvpsex, "");
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006E2 RID: 1762 RVA: 0x000640DC File Offset: 0x000622DC
        public bool ProcessGetBangHuiMatchAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int err = 1;
                string awardsInfo = Global.GetRoleParamByName(client, "39");
                if (!string.IsNullOrEmpty(awardsInfo))
                {
                    int lastGroupId = 0;
                    int success = 0;
                    int mvprid = 0;
                    List<string> awardsParamList = Global.StringToList(awardsInfo, '&');
                    if (awardsParamList.Count != 6 && !string.IsNullOrEmpty(awardsInfo))
                    {
                        byte[] strBytes = Convert.FromBase64String(awardsInfo);
                        awardsInfo = new UTF8Encoding().GetString(strBytes);
                        awardsParamList = Global.StringToList(awardsInfo, '&');
                    }
                    ConfigParser.ParseStrInt3(awardsInfo, ref lastGroupId, ref success, ref mvprid, '&');
                    bool clear = true;
                    if (awardsParamList.Count >= 6 && lastGroupId > 0)
                    {
                        string mvpname = awardsParamList[3];
                        int mvpocc = Global.SafeConvertToInt32(awardsParamList[4]);
                        int mvpsex = Global.SafeConvertToInt32(awardsParamList[5]);
                        BHMatchConfig lastSceneItem = null;
                        if (this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue(lastGroupId, out lastSceneItem))
                        {
                            err = this.GiveRoleAwards(client, success, lastSceneItem);
                            if (err < 0)
                            {
                                clear = false;
                            }
                        }
                    }
                    if (clear)
                    {
                        Global.SaveRoleParamsStringToDB(client, "39", "", true);
                    }
                    client.sendCmd<int>(nID, err, false);
                }
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006E3 RID: 1763 RVA: 0x0006426C File Offset: 0x0006246C
        private DateTime GetLastSeasonLastMatchEndTime(int lastMatchType)
        {
            bool calGold = lastMatchType == 1;
            int LastMatchSeasonID = (lastMatchType == 1) ? this.BHMatchSyncDataCache.CurSeasonID_Gold : this.BHMatchSyncDataCache.CurSeasonID_Rookie;
            DateTime lastMatchSeasonTm = BangHuiMatchUtils.GetSeasonDateTm(LastMatchSeasonID);
            TimeSpan end = TimeSpan.MinValue;
            lock (this.RuntimeData.Mutex)
            {
                foreach (BHMatchConfig item in this.RuntimeData.CommonConfigData.BHMatchConfigDict.Values)
                {
                    if (item.ID != 1 || calGold)
                    {
                        int lastWeakMatch = item.TimePoints.Count / 2 - item.RoundNum % (item.TimePoints.Count / 2);
                        for (int i = 0; i < item.TimePoints.Count - 1; i += 2)
                        {
                            TimeSpan myTmp = item.TimePoints[i + 1];
                            if (myTmp.Days == 0)
                            {
                                myTmp += new TimeSpan(7, 0, 0, 0);
                            }
                            if (myTmp > end && i / 2 < lastWeakMatch)
                            {
                                end = myTmp;
                            }
                        }
                    }
                }
            }
            end -= new TimeSpan(1, 0, 0, 0);
            TimeSpan endSunday = new TimeSpan(7, 0, 0, 0) - end;
            return lastMatchSeasonTm - endSunday;
        }

        // Token: 0x060006E4 RID: 1764 RVA: 0x00064534 File Offset: 0x00062734
        public bool ProcessGetBangHuiMatchRankAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                List<BangHuiMatchRankInfo> lastGoldRank = null;
                List<BangHuiMatchRankInfo> lastRookieRank = null;
                lock (this.RuntimeData.Mutex)
                {
                    this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(0, out lastGoldRank);
                    this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(1, out lastRookieRank);
                }
                int num;
                if (lastGoldRank == null)
                {
                    num = -1;
                }
                else
                {
                    num = lastGoldRank.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
                }
                int goldIndex = num;
                int num2;
                if (lastRookieRank == null)
                {
                    num2 = -1;
                }
                else
                {
                    num2 = lastRookieRank.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
                }
                int rookieIndex = num2;
                int lastMatchType = 0;
                int rankNum = 51;
                if (-1 != goldIndex)
                {
                    lastMatchType = 1;
                    rankNum = goldIndex + 1;
                }
                else if (TianTiClient.getInstance().CheckRookieJoinLast_BHMatch(client.ClientData.Faction))
                {
                    lastMatchType = 2;
                }
                if (-1 != rookieIndex)
                {
                    lastMatchType = 2;
                    rankNum = rookieIndex + 1;
                }
                if (lastMatchType == 0)
                {
                    result = -5;
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                int LastMatchSeasonID = (lastMatchType == 1) ? this.BHMatchSyncDataCache.CurSeasonID_Gold : this.BHMatchSyncDataCache.CurSeasonID_Rookie;
                DateTime lastMatchSeasonTm = this.GetLastSeasonLastMatchEndTime(lastMatchType);
                int EnterBHUnixSecs = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
                DateTime enterBHTm = new DateTime(DataHelper.UnixSecondsToTicks(EnterBHUnixSecs) * 10000L);
                if (enterBHTm > lastMatchSeasonTm)
                {
                    result = -2006;
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                List<int> RankAwardsHist = Global.GetRoleParamsIntListFromDB(client, "42");
                if (RankAwardsHist.Count != 2)
                {
                    RankAwardsHist.Add(0);
                    RankAwardsHist.Add(0);
                }
                if (RankAwardsHist[lastMatchType - 1] == LastMatchSeasonID)
                {
                    result = -200;
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                BHMatchRankAwardConfig rankAwardInfo = null;
                lock (this.RuntimeData.Mutex)
                {
                    if (lastMatchType == 1)
                    {
                        rankAwardInfo = this.RuntimeData.RankAwardConfigList_Gold.Find((BHMatchRankAwardConfig x) => rankNum >= x.BeginNum && rankNum <= x.EndNum);
                    }
                    else
                    {
                        rankAwardInfo = this.RuntimeData.RankAwardConfigList_Rookie.Find((BHMatchRankAwardConfig x) => rankNum >= x.BeginNum && (x.EndNum == -1 || rankNum <= x.EndNum));
                    }
                }
                if (rankAwardInfo != null && !Global.CanAddGoodsNum(client, rankAwardInfo.AwardsItemList.Items.Count))
                {
                    result = -100;
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                if (rankAwardInfo != null)
                {
                    foreach (AwardsItemData item in rankAwardInfo.AwardsItemList.Items)
                    {
                        Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "战盟联赛排行榜奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
                    }
                }
                RankAwardsHist[lastMatchType - 1] = LastMatchSeasonID;
                Global.SaveRoleParamsIntListToDB(client, RankAwardsHist, "42", true);
                List<int> roleAnalysisExData = this.GetBHMatchRAnalysisExData(client);
                if (null != roleAnalysisExData)
                {
                    if (lastMatchType == 1)
                    {
                        if (roleAnalysisExData[1] == 0 || roleAnalysisExData[1] > rankNum)
                        {
                            roleAnalysisExData[1] = rankNum;
                        }
                        if (rankNum == 1)
                        {
                            List<int> list;
                            (list = roleAnalysisExData)[0] = list[0] + 1;
                            GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_BHMatchGoldChampion, new int[0]));
                        }
                    }
                    else
                    {
                        if (roleAnalysisExData[3] == 0 || roleAnalysisExData[3] > rankNum)
                        {
                            roleAnalysisExData[3] = rankNum;
                        }
                        if (rankNum == 1)
                        {
                            List<int> list;
                            (list = roleAnalysisExData)[2] = list[2] + 1;
                        }
                    }
                    this.SaveBHMatchRAnalysisExData(client, roleAnalysisExData);
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

        // Token: 0x060006E5 RID: 1765 RVA: 0x00064B34 File Offset: 0x00062D34
        public bool ProcessGetBangHuiMatchRankInfoMiniCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                List<BangHuiMatchRankInfo> rankInfoList = new List<BangHuiMatchRankInfo>();
                for (int typeLoop = 0; typeLoop <= 7; typeLoop++)
                {
                    List<BangHuiMatchRankInfo> curRankInfoList = new List<BangHuiMatchRankInfo>();
                    lock (this.RuntimeData.Mutex)
                    {
                        if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict.V)
                        {
                            this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(typeLoop, out curRankInfoList);
                        }
                    }
                    if (curRankInfoList == null || curRankInfoList.Count == 0)
                    {
                        rankInfoList.Add(new BangHuiMatchRankInfo());
                    }
                    else
                    {
                        rankInfoList.Add(curRankInfoList[0]);
                    }
                }
                client.sendCmd<List<BangHuiMatchRankInfo>>(nID, rankInfoList, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006E6 RID: 1766 RVA: 0x00064D28 File Offset: 0x00062F28
        public bool ProcessBangHuiMatchGuess(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                int roleID = Convert.ToInt32(cmdParams[0]);
                int bhid1 = Convert.ToInt32(cmdParams[1]);
                int bhid2 = Convert.ToInt32(cmdParams[2]);
                int guess = Convert.ToInt32(cmdParams[3]);
                if (!this.RuntimeData.CommonConfigData.CheckOpenState(TimeUtil.NowDateTime()))
                {
                    result = -2001;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                    {
                        result,
                        bhid1,
                        bhid2,
                        guess
                    }), false);
                    return true;
                }
                int goldRound = this.GetCurrentBHMatchGoldRound();
                BHMatchSupportData findData = client.ClientData.BHMatchSupportList.Find((BHMatchSupportData x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && x.round == goldRound && x.bhid1 == bhid1 && x.bhid2 == bhid2);
                if (null != findData)
                {
                    result = -5;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                    {
                        result,
                        bhid1,
                        bhid2,
                        guess
                    }), false);
                    return true;
                }
                DateTime curSeasonTm = BangHuiMatchUtils.GetSeasonDateTm(this.BHMatchSyncDataCache.CurSeasonID_Gold);
                BHMatchConfig matchGoldConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(1);
                BHMatchConfig sceneItem = null;
                BangHuiMatchGameStates timeState = BangHuiMatchGameStates.None;
                this.CheckCondition(client, 1, ref sceneItem, ref timeState);
                if (TimeUtil.NowDateTime() < curSeasonTm)
                {
                    timeState = BangHuiMatchGameStates.Wait;
                }
                else if (this.BHMatchSyncDataCache.CurSeasonID_Rookie == this.BHMatchSyncDataCache.CurSeasonID_Gold && this.BHMatchSyncDataCache.RoundGoldReal > matchGoldConfig.RoundNum)
                {
                    timeState = BangHuiMatchGameStates.End;
                }
                if (BangHuiMatchGameStates.SignUp != timeState)
                {
                    result = -2001;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                    {
                        result,
                        bhid1,
                        bhid2,
                        guess
                    }), false);
                    return true;
                }
                List<BangHuiMatchPKInfo> pkinfoList = null;
                lock (this.RuntimeData.Mutex)
                {
                    if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
                    {
                        pkinfoList = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
                    }
                }
                List<BangHuiMatchPKInfo> CurRoundPKInfo = new List<BangHuiMatchPKInfo>(pkinfoList.FindAll((BangHuiMatchPKInfo x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && (int)x.round == goldRound));
                BangHuiMatchPKInfo pkInfo = CurRoundPKInfo.Find((BangHuiMatchPKInfo x) => x.bhid1 == bhid1 && x.bhid2 == bhid2);
                if (null == pkInfo)
                {
                    result = -5;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                    {
                        result,
                        bhid1,
                        bhid2,
                        guess
                    }), false);
                    return true;
                }
                BHMatchGoldGuessConfig guessConfigData;
                lock (this.RuntimeData.Mutex)
                {
                    if (!this.RuntimeData.BHMatchGoldGuessConfigDict.TryGetValue(goldRound, out guessConfigData))
                    {
                        result = -3;
                        client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                        {
                            result,
                            bhid1,
                            bhid2,
                            guess
                        }), false);
                        return true;
                    }
                }
                if (Global.GetUnionLevel(client, false) < guessConfigData.UnionLevLimit)
                {
                    result = -19;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                    {
                        result,
                        bhid1,
                        bhid2,
                        guess
                    }), false);
                    return true;
                }
                if (client.ClientData.Money1 + client.ClientData.YinLiang < guessConfigData.Cost)
                {
                    result = -9;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                    {
                        result,
                        bhid1,
                        bhid2,
                        guess
                    }), false);
                    return true;
                }
                if (!Global.SubBindTongQianAndTongQian(client, guessConfigData.Cost, "战盟联赛竞猜"))
                {
                    result = -9;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                    {
                        result,
                        bhid1,
                        bhid2,
                        guess
                    }), false);
                    return true;
                }
                BHMatchSupportData supportData = new BHMatchSupportData();
                supportData.season = this.BHMatchSyncDataCache.CurSeasonID_Gold;
                supportData.round = goldRound;
                supportData.bhid1 = bhid1;
                supportData.bhid2 = bhid2;
                supportData.guess = guess;
                supportData.rid = client.ClientData.RoleID;
                client.ClientData.BHMatchSupportList.Add(supportData);
                Global.SendToDB<BHMatchSupportData>(14021, supportData, client.ServerId);
                client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
                {
                    result,
                    bhid1,
                    bhid2,
                    guess
                }), false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006E7 RID: 1767 RVA: 0x00065508 File Offset: 0x00063708
        public bool ProcessGetBangHuiMatchGuessInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                List<BHMatchSupportData4Client> BHMatchSupportList4Client = new List<BHMatchSupportData4Client>();
                List<BangHuiMatchPKInfo> pkinfoList = null;
                lock (this.RuntimeData.Mutex)
                {
                    if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
                    {
                        pkinfoList = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
                    }
                }
                int goldRound = this.GetCurrentBHMatchGoldRound();
                using (List<BHMatchSupportData>.Enumerator enumerator = client.ClientData.BHMatchSupportList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BHMatchSupportData item = enumerator.Current;
                        if (item.isaward == 0 && (item.season != this.BHMatchSyncDataCache.CurSeasonID_Gold || item.round != goldRound))
                        {
                            BHMatchSupportData4Client findData = BHMatchSupportList4Client.Find((BHMatchSupportData4Client x) => x.season == item.season && x.round == item.round);
                            if (null == findData)
                            {
                                findData = new BHMatchSupportData4Client();
                                findData.season = item.season;
                                findData.round = item.round;
                                BHMatchSupportList4Client.Add(findData);
                            }
                            BHMatchGoldGuessConfig guessConfigData;
                            lock (this.RuntimeData.Mutex)
                            {
                                if (!this.RuntimeData.BHMatchGoldGuessConfigDict.TryGetValue(item.round, out guessConfigData))
                                {
                                    continue;
                                }
                            }
                            BangHuiMatchPKInfo pkInfo = pkinfoList.Find((BangHuiMatchPKInfo x) => x.season == item.season && (int)x.round == item.round && x.bhid1 == item.bhid1 && x.bhid2 == item.bhid2);
                            if (pkInfo != null && (int)pkInfo.result == item.guess)
                            {
                                findData.right++;
                                findData.jifen += guessConfigData.WinAward;
                            }
                            else
                            {
                                findData.jifen += guessConfigData.FaillAward;
                            }
                        }
                    }
                }
                BHMatchSupportList4Client.Sort(delegate (BHMatchSupportData4Client left, BHMatchSupportData4Client right)
                {
                    int result;
                    if (left.season < right.season)
                    {
                        result = -1;
                    }
                    else if (left.season > right.season)
                    {
                        result = 1;
                    }
                    else if (left.round < right.round)
                    {
                        result = -1;
                    }
                    else if (left.round > right.round)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = 0;
                    }
                    return result;
                });
                client.sendCmd<List<BHMatchSupportData4Client>>(nID, BHMatchSupportList4Client, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006E8 RID: 1768 RVA: 0x00065888 File Offset: 0x00063A88
        public bool ProcessGetBangHuiMatchGuessAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                if (!this.RuntimeData.CommonConfigData.CheckOpenState(TimeUtil.NowDateTime()))
                {
                    result = -2001;
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                List<BangHuiMatchPKInfo> pkinfoList = null;
                lock (this.RuntimeData.Mutex)
                {
                    if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
                    {
                        pkinfoList = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
                    }
                }
                int addJiFen = 0;
                int goldRound = this.GetCurrentBHMatchGoldRound();
                using (List<BHMatchSupportData>.Enumerator enumerator = client.ClientData.BHMatchSupportList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BHMatchSupportData item = enumerator.Current;
                        if (item.isaward == 0 && (item.season != this.BHMatchSyncDataCache.CurSeasonID_Gold || item.round != goldRound))
                        {
                            BHMatchGoldGuessConfig guessConfigData;
                            lock (this.RuntimeData.Mutex)
                            {
                                if (!this.RuntimeData.BHMatchGoldGuessConfigDict.TryGetValue(item.round, out guessConfigData))
                                {
                                    continue;
                                }
                            }
                            BangHuiMatchPKInfo pkInfo = pkinfoList.Find((BangHuiMatchPKInfo x) => x.season == item.season && (int)x.round == item.round && x.bhid1 == item.bhid1 && x.bhid2 == item.bhid2);
                            if (pkInfo != null && (int)pkInfo.result == item.guess)
                            {
                                addJiFen += guessConfigData.WinAward;
                            }
                            else
                            {
                                addJiFen += guessConfigData.FaillAward;
                            }
                            item.isaward = 1;
                            Global.SendToDB<BHMatchSupportData>(14021, item, client.ServerId);
                        }
                    }
                }
                GameManager.ClientMgr.ModifyBHMatchGuessJiFenValue(client, addJiFen, "战盟联赛竞猜奖励", true, true, false);
                client.sendCmd<int>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x060006E9 RID: 1769 RVA: 0x00065BE8 File Offset: 0x00063DE8
        public void FillGuanZhanData(GameClient client, GuanZhanData gzData)
        {
            lock (this.RuntimeData.Mutex)
            {
                BangHuiMatchScene scene;
                if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
                {
                    gzData.SideName.Add(scene.GameStatisticalData.bhname1);
                    gzData.SideName.Add(scene.GameStatisticalData.bhname2);
                    foreach (KeyValuePair<int, BHMatchClientContextData> kvp in scene.ClientContextDataDict)
                    {
                        GameClient sceneClient = GameManager.ClientMgr.FindClient(kvp.Key);
                        if (sceneClient != null && sceneClient.ClientData.HideGM <= 0)
                        {
                            SceneUIClasses sceneType = Global.GetMapSceneType(sceneClient.ClientData.MapCode);
                            if (SceneUIClasses.BangHuiMatch == sceneType)
                            {
                                List<GuanZhanRoleMiniData> roleDataList = null;
                                if (!gzData.RoleMiniDataDict.TryGetValue(kvp.Value.BattleWhichSide, out roleDataList))
                                {
                                    roleDataList = new List<GuanZhanRoleMiniData>();
                                    gzData.RoleMiniDataDict[kvp.Value.BattleWhichSide] = roleDataList;
                                }
                                GuanZhanRoleMiniData roleMiniData = new GuanZhanRoleMiniData();
                                roleMiniData.RoleID = sceneClient.ClientData.RoleID;
                                roleMiniData.Name = Global.FormatRoleNameWithZoneId2(sceneClient);
                                roleMiniData.Level = sceneClient.ClientData.Level;
                                roleMiniData.ChangeLevel = sceneClient.ClientData.ChangeLifeCount;
                                roleMiniData.Occupation = sceneClient.ClientData.Occupation;
                                roleMiniData.RoleSex = sceneClient.ClientData.RoleSex;
                                roleMiniData.BHZhiWu = sceneClient.ClientData.BHZhiWu;
                                roleMiniData.Param1 = kvp.Value.Kill;
                                roleDataList.Add(roleMiniData);
                            }
                        }
                    }
                    foreach (List<GuanZhanRoleMiniData> rolelist in gzData.RoleMiniDataDict.Values)
                    {
                        rolelist.Sort(delegate (GuanZhanRoleMiniData left, GuanZhanRoleMiniData right)
                        {
                            int leftZhiWu = (left.BHZhiWu == 0) ? 5 : left.BHZhiWu;
                            int rightZhiWu = (right.BHZhiWu == 0) ? 5 : right.BHZhiWu;
                            int result;
                            if (leftZhiWu < rightZhiWu)
                            {
                                result = -1;
                            }
                            else if (leftZhiWu > rightZhiWu)
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

        // Token: 0x060006EA RID: 1770 RVA: 0x00065EA4 File Offset: 0x000640A4
        private bool CheckMap(GameClient client)
        {
            SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
            return sceneType == SceneUIClasses.Normal;
        }

        // Token: 0x060006EB RID: 1771 RVA: 0x00065ED8 File Offset: 0x000640D8
        private void OnInitGame(GameClient client)
        {
            lock (this.RuntimeData.Mutex)
            {
                if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
                {
                    List<BangHuiMatchPKInfo> pkinfoList = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
                }
            }
            List<BHMatchSupportData> mySupports = Global.sendToDB<List<BHMatchSupportData>, string>(14020, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.RuntimeData.BHMatchPKInfoMinSeasonID, this.RuntimeData.BHMatchPKInfoMinRound), client.ServerId);
            client.ClientData.BHMatchSupportList.Clear();
            if (mySupports != null)
            {
                client.ClientData.BHMatchSupportList.AddRange(mySupports);
            }
        }

        // Token: 0x060006EC RID: 1772 RVA: 0x00065FC4 File Offset: 0x000641C4
        public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
        {
            BHMatchFuBenData kroleData = TianTiClient.getInstance().GetFuBenDataByGameId_BHMatch((int)kuaFuServerLoginData.GameId);
            bool result;
            if (kroleData == null || kroleData.ServerId != GameManager.ServerId)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

        // Token: 0x060006ED RID: 1773 RVA: 0x00066030 File Offset: 0x00064230
        public bool OnInitGameKuaFu(GameClient client)
        {
            KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
            BHMatchFuBenData fuBenData;
            lock (this.RuntimeData.Mutex)
            {
                if (!this.RuntimeData.FuBenItemData.TryGetValue((long)((int)kuaFuServerLoginData.GameId), out fuBenData))
                {
                    fuBenData = null;
                }
            }
            if (null == fuBenData)
            {
                BHMatchFuBenData newFuBenData;
                if (client.ClientData.GuanZhanGM > 0)
                {
                    newFuBenData = TianTiClient.getInstance().GetFuBenDataByGameId_BHMatch((int)kuaFuServerLoginData.GameId);
                }
                else
                {
                    newFuBenData = TianTiClient.getInstance().GetFuBenDataByBhid_BHMatch(client.ClientData.Faction);
                }
                if (newFuBenData == null)
                {
                    LogManager.WriteLog(LogTypes.Error, ("获取不到有效的副本数据," + newFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
                    return false;
                }
                lock (this.RuntimeData.Mutex)
                {
                    if (!this.RuntimeData.FuBenItemData.TryGetValue(kuaFuServerLoginData.GameId, out fuBenData))
                    {
                        fuBenData = newFuBenData;
                        fuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
                        this.RuntimeData.FuBenItemData[fuBenData.GameId] = fuBenData;
                    }
                }
            }
            if (fuBenData.bhid1 == client.ClientData.Faction)
            {
                client.ClientData.BattleWhichSide = 1;
            }
            else if (fuBenData.bhid2 == client.ClientData.Faction)
            {
                client.ClientData.BattleWhichSide = 2;
            }
            else
            {
                client.ClientData.HideGM = 1;
            }
            BHMatchConfig sceneInfo;
            lock (this.RuntimeData.Mutex)
            {
                kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
                if (!this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue((int)fuBenData.Type, out sceneInfo))
                {
                    return false;
                }
                client.SceneInfoObject = sceneInfo;
                client.ClientData.MapCode = sceneInfo.MapCode;
            }
            int toMapCode;
            int toPosX;
            int toPosY;
            bool result;
            if (!this.GetZhanMengBirthPoint(sceneInfo, client, client.ClientData.MapCode, out toMapCode, out toPosX, out toPosY, false))
            {
                LogManager.WriteLog(LogTypes.Error, "无法获取有效的阵营和出生点,进入跨服失败,side=" + client.ClientData.BattleWhichSide, null, true);
                result = false;
            }
            else
            {
                client.ClientData.PosX = toPosX;
                client.ClientData.PosY = toPosY;
                client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
                result = true;
            }
            return result;
        }

        // Token: 0x060006EE RID: 1774 RVA: 0x00066338 File Offset: 0x00064538
        public bool GetZhanMengBirthPoint(BHMatchConfig sceneInfo, GameClient client, int toMapCode, out int mapCode, out int posX, out int posY, bool isLogin = false)
        {
            mapCode = sceneInfo.MapCode;
            posX = 0;
            posY = 0;
            double distance = 0.0;
            int side = client.ClientData.BattleWhichSide;
            bool result;
            if (client.ClientData.HideGM > 0)
            {
                if (VideoLogic.getInstance().GetGuanZhanPos(toMapCode, ref posX, ref posY))
                {
                    mapCode = toMapCode;
                }
                result = true;
            }
            else
            {
                lock (this.RuntimeData.Mutex)
                {
                    int round = 0;
                    if (toMapCode == sceneInfo.MapCode_LongTa)
                    {
                        Point pt;
                        for (; ; )
                        {
                            pt = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, sceneInfo.MapCode_LongTa);
                            if (!Global.InObs(ObjectTypes.OT_CLIENT, sceneInfo.MapCode_LongTa, (int)pt.X, (int)pt.Y, 0, 0))
                            {
                                break;
                            }
                            if (round++ >= 1000)
                            {
                                goto IL_103;
                            }
                        }
                        mapCode = sceneInfo.MapCode_LongTa;
                        posX = (int)pt.X;
                        posY = (int)pt.Y;
                        return true;
                    }
                    IL_103:
                    BHMatchBirthPoint birthPoint = null;
                    if (!this.RuntimeData.MapBirthPointDict.TryGetValue(side, out birthPoint))
                    {
                        return false;
                    }
                    posX = birthPoint.PosX;
                    posY = birthPoint.PosY;
                    distance = Global.GetTwoPointDistance(new Point((double)posX, (double)posY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
                    if (isLogin)
                    {
                        return true;
                    }
                }
                BangHuiMatchScene scene = null;
                if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
                {
                    foreach (KeyValuePair<int, BHMatchQiZhiConfig> item in scene.NPCID2QiZhiConfigDict)
                    {
                        if (item.Value.BattleWhichSide == side && item.Value.RebirthSiteX != 0 && item.Value.RebirthSiteY != 0)
                        {
                            double tempdis = Global.GetTwoPointDistance(new Point((double)item.Value.PosX, (double)item.Value.PosY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
                            if (tempdis < distance)
                            {
                                distance = tempdis;
                                Point BirthPoint = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, item.Value.RebirthSiteX / scene.MapGridWidth, item.Value.RebirthSiteY / scene.MapGridHeight, item.Value.RebirthRadius / scene.MapGridWidth, 0, false);
                                posX = (int)BirthPoint.X;
                                posY = (int)BirthPoint.Y;
                            }
                        }
                    }
                }
                result = true;
            }
            return result;
        }

        // Token: 0x060006EF RID: 1775 RVA: 0x0006667C File Offset: 0x0006487C
        private void HandleNtfEnterEvent(BHMatchNtfEnterData data)
        {
            foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
            {
                if (this.IsGongNengOpened(client, false) && this.CheckMap(client))
                {
                    if (client != null && (data.bhid1 == client.ClientData.Faction || data.bhid2 == client.ClientData.Faction))
                    {
                        client.sendCmd<int>(1171, 1, false);
                    }
                }
            }
            LogManager.WriteLog(LogTypes.Error, string.Format("通知战盟ID={0}，ID={1}拥有进入战盟联赛资格", data.bhid1, data.bhid2), null, true);
        }

        // Token: 0x060006F0 RID: 1776 RVA: 0x00066760 File Offset: 0x00064960
        public void SwitchLastGoldBH_GM()
        {
            TianTiClient.getInstance().SwitchLastGoldBH_GM();
        }

        // Token: 0x060006F1 RID: 1777 RVA: 0x00066804 File Offset: 0x00064A04
        private void TimerProc(object sender, EventArgs e)
        {
            lock (this.RuntimeData.Mutex)
            {
                BHMatchSyncData SyncData = TianTiClient.getInstance().SyncData_BHMatch(this.BHMatchSyncDataCache.BHMatchRankInfoDict.Age, this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.Age, this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Age);
                this.BHMatchSyncDataCache.LastSeasonID_Gold = SyncData.LastSeasonID_Gold;
                this.BHMatchSyncDataCache.CurSeasonID_Gold = SyncData.CurSeasonID_Gold;
                this.BHMatchSyncDataCache.LastSeasonID_Rookie = SyncData.LastSeasonID_Rookie;
                this.BHMatchSyncDataCache.CurSeasonID_Rookie = SyncData.CurSeasonID_Rookie;
                this.BHMatchSyncDataCache.RoundGoldReal = SyncData.RoundGoldReal;
                this.BHMatchSyncDataCache.RoundRookieReal = SyncData.RoundRookieReal;
                if (this.BHMatchSyncDataCache.BHMatchRankInfoDict.Age != SyncData.BHMatchRankInfoDict.Age)
                {
                    this.BHMatchSyncDataCache.BHMatchRankInfoDict = SyncData.BHMatchRankInfoDict;
                    bool chgChampion = false;
                    chgChampion |= this.RefreshBHMatchChampionBH(1);
                    chgChampion |= this.RefreshBHMatchChampionBH(0);
                    if (chgChampion)
                    {
                        int count = GameManager.ClientMgr.GetMaxClientCount();
                        for (int i = 0; i < count; i++)
                        {
                            GameClient client = GameManager.ClientMgr.FindClientByNid(i);
                            if (null != client)
                            {
                                this.UpdateChengHaoBuffer(client);
                            }
                        }
                    }
                    this.SaveBHMatchBHListGoldJoin();
                }
                if (this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.Age != SyncData.BHMatchPKInfoList_Gold.Age)
                {
                    this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold = SyncData.BHMatchPKInfoList_Gold;
                    int goldRound = this.GetCurrentBHMatchGoldRound();
                    List<BangHuiMatchPKInfo> CurRoundPKInfo = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V.FindAll((BangHuiMatchPKInfo x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && (int)x.round == goldRound);
                    if (null != CurRoundPKInfo)
                    {
                        for (int i = 0; i < CurRoundPKInfo.Count; i++)
                        {
                            BangHuiMatchPKInfo pkInfo = CurRoundPKInfo[i];
                            BHMatchBHData bhData = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(1, pkInfo.bhid1);
                            BHMatchBHData bhData2 = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(1, pkInfo.bhid2);
                            List<BangHuiMatchRankInfo> rankInfoList = new List<BangHuiMatchRankInfo>();
                            if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
                            {
                                this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(8, out rankInfoList);
                            }
                            if (null != bhData)
                            {
                                pkInfo.win1 = (byte)bhData.cur_win;
                                pkInfo.winpct1 = (byte)(((goldRound == 1) ? 0 : ((byte)((double)bhData.cur_win / (double)(goldRound - 1) * 100.0))));
                                pkInfo.rank1 = (byte)(rankInfoList.FindIndex((BangHuiMatchRankInfo x) => x.Key == pkInfo.bhid1) + 1);
                            }
                            if (null != bhData2)
                            {
                                pkInfo.win2 = (byte)bhData2.cur_win;
                                pkInfo.winpct2 = (byte)(((goldRound == 1) ? 0 : ((byte)((double)bhData2.cur_win / (double)(goldRound - 1) * 100.0))));
                                pkInfo.rank2 = (byte)(rankInfoList.FindIndex((BangHuiMatchRankInfo x) => x.Key == pkInfo.bhid2) + 1);
                            }
                        }
                    }
                    foreach (BangHuiMatchPKInfo item in this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V)
                    {
                        if (item.season < this.RuntimeData.BHMatchPKInfoMinSeasonID)
                        {
                            this.RuntimeData.BHMatchPKInfoMinSeasonID = item.season;
                            this.RuntimeData.BHMatchPKInfoMinRound = (int)item.round;
                        }
                        else if (item.season == this.RuntimeData.BHMatchPKInfoMinSeasonID && (int)item.round < this.RuntimeData.BHMatchPKInfoMinRound)
                        {
                            this.RuntimeData.BHMatchPKInfoMinSeasonID = item.season;
                            this.RuntimeData.BHMatchPKInfoMinRound = (int)item.round;
                        }
                    }
                }
                if (this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Age != SyncData.BHMatchChampionRoleData_Gold.Age)
                {
                    this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold = SyncData.BHMatchChampionRoleData_Gold;
                    if (null != this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Bytes0)
                    {
                        this.OwnerRoleData = DataHelper.BytesToObject<RoleDataEx>(this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Bytes0, 0, this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Bytes0.Length);
                    }
                    else
                    {
                        this.OwnerRoleData = null;
                    }
                    this.ReplaceBangHuiMatchNpc();
                }
            }
        }

        // Token: 0x060006F2 RID: 1778 RVA: 0x00066D74 File Offset: 0x00064F74
        public bool IsGongNengOpened(GameClient client, bool hint = false)
        {
            return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot6) && GlobalNew.IsGongNengOpened(client, GongNengIDs.BangHuiMatch, hint);
        }

        // Token: 0x060006F3 RID: 1779 RVA: 0x00066DA4 File Offset: 0x00064FA4
        private void NtfCanGetAward(GameClient client, int success, BHMatchConfig sceneInfo, int mvprid, string mvpname, int mvpocc, int mvpsex, string sucessBHName = "")
        {
            long addExp = Global.GetExpMultiByZhuanShengExpXiShu(client, sceneInfo.Exp);
            int addBindJinBi = sceneInfo.BandJinBi;
            List<AwardsItemData> awardsItemDataList;
            if (success > 0)
            {
                awardsItemDataList = (sceneInfo.WinAwardTag as AwardsItemList).Items;
            }
            else
            {
                addExp = (long)((double)addExp * 0.8);
                addBindJinBi = (int)((double)addBindJinBi * 0.8);
                awardsItemDataList = (sceneInfo.FailAwardTag as AwardsItemList).Items;
            }
            addExp -= addExp % 10000L;
            addBindJinBi -= addBindJinBi % 10000;
            client.sendCmd<BangHuiMatchAwardsData>(1174, new BangHuiMatchAwardsData
            {
                Exp = addExp,
                BindJinBi = addBindJinBi,
                Success = success,
                AwardsItemDataList = awardsItemDataList,
                SuccessBHName = sucessBHName,
                MvpRoleName = mvpname,
                MvpOccupation = mvpocc,
                MvpRoleSex = mvpsex
            }, false);
        }

        // Token: 0x060006F4 RID: 1780 RVA: 0x00066E84 File Offset: 0x00065084
        private int GiveRoleAwards(GameClient client, int success, BHMatchConfig sceneInfo)
        {
            long addExp = Global.GetExpMultiByZhuanShengExpXiShu(client, sceneInfo.Exp);
            int addBindJinBi = sceneInfo.BandJinBi;
            List<AwardsItemData> awardsItemDataList;
            if (success > 0)
            {
                awardsItemDataList = (sceneInfo.WinAwardTag as AwardsItemList).Items;
            }
            else
            {
                addExp = (long)((double)addExp * 0.8);
                addBindJinBi = (int)((double)addBindJinBi * 0.8);
                awardsItemDataList = (sceneInfo.FailAwardTag as AwardsItemList).Items;
            }
            addExp -= addExp % 10000L;
            addBindJinBi -= addBindJinBi % 10000;
            int result;
            if (awardsItemDataList != null && !Global.CanAddGoodsNum(client, awardsItemDataList.Count))
            {
                result = -100;
            }
            else
            {
                if (addExp > 0L)
                {
                    GameManager.ClientMgr.ProcessRoleExperience(client, addExp, true, true, false, "none");
                }
                if (addBindJinBi > 0)
                {
                    GameManager.ClientMgr.AddMoney1(client, addBindJinBi, "战盟联赛奖励", true);
                }
                if (awardsItemDataList != null)
                {
                    foreach (AwardsItemData item in awardsItemDataList)
                    {
                        Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "战盟联赛奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
                    }
                }
                result = 1;
            }
            return result;
        }

        // Token: 0x060006F5 RID: 1781 RVA: 0x00067034 File Offset: 0x00065234
        public void GiveAwards(BangHuiMatchScene scene)
        {
            try
            {
                foreach (BHMatchClientContextData contextData in scene.ClientContextDataDict.Values)
                {
                    int success;
                    if (contextData.BattleWhichSide == scene.SuccessSide)
                    {
                        success = 1;
                    }
                    else
                    {
                        success = 0;
                    }
                    GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleId);
                    if (0 == contextData.BattleWhichSide)
                    {
                        if (null != client)
                        {
                            string sucessBHName = (scene.SuccessSide == 1) ? scene.GameStatisticalData.bhname1 : scene.GameStatisticalData.bhname2;
                            this.NtfCanGetAward(client, 1, scene.SceneInfo, scene.ClientContextMVP.RoleId, scene.ClientContextMVP.RoleName, scene.ClientContextMVP.Occupation, scene.ClientContextMVP.RoleSex, sucessBHName);
                        }
                    }
                    else
                    {
                        string awardsInfo = string.Format("{0}&{1}&{2}&{3}&{4}&{5}", new object[]
                        {
                            scene.SceneInfo.ID,
                            success,
                            scene.ClientContextMVP.RoleId,
                            scene.ClientContextMVP.RoleName,
                            scene.ClientContextMVP.Occupation,
                            scene.ClientContextMVP.RoleSex
                        });
                        byte[] bytes = new UTF8Encoding().GetBytes(awardsInfo);
                        awardsInfo = Convert.ToBase64String(bytes);
                        if (client != null)
                        {
                            int score = contextData.TotalScore;
                            contextData.TotalScore = 0;
                            Global.SaveRoleParamsStringToDB(client, "39", awardsInfo, true);
                            this.NtfCanGetAward(client, success, scene.SceneInfo, scene.ClientContextMVP.RoleId, scene.ClientContextMVP.RoleName, scene.ClientContextMVP.Occupation, scene.ClientContextMVP.RoleSex, "");
                        }
                        else
                        {
                            Global.UpdateRoleParamByNameOffline(contextData.RoleId, "39", awardsInfo, contextData.ServerId);
                        }
                    }
                }
                this.PushGameResultData(scene);
            }
            catch (Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "战盟联赛系统清场调度异常");
            }
        }

        // Token: 0x060006F6 RID: 1782 RVA: 0x000672AC File Offset: 0x000654AC
        public void PushGameResultData(BangHuiMatchScene scene)
        {
            BHMatchFuBenData fuBenData;
            if (this.RuntimeData.FuBenItemData.TryGetValue((long)scene.GameId, out fuBenData))
            {
                scene.GameStatisticalData.bhid1 = fuBenData.bhid1;
                scene.GameStatisticalData.bhid2 = fuBenData.bhid2;
                scene.GameStatisticalData.result = (byte)scene.SuccessSide;
                int totalScore = scene.ScoreData.Score1 + scene.ScoreData.Score2;
                int addScore = (totalScore == 0) ? 0 : ((int)((double)scene.ScoreData.Score1 / (double)totalScore * 100.0));
                int addScore2 = (totalScore == 0) ? 0 : ((int)((double)scene.ScoreData.Score2 / (double)totalScore * 100.0));
                if (1 == scene.SuccessSide)
                {
                    scene.GameStatisticalData.score1 = this.RuntimeData.CommonConfigData.RookieWinScoreFactor + addScore;
                    scene.GameStatisticalData.score2 = this.RuntimeData.CommonConfigData.RookieLoseScoreFactor + addScore2;
                }
                else if (2 == scene.SuccessSide)
                {
                    scene.GameStatisticalData.score1 = this.RuntimeData.CommonConfigData.RookieLoseScoreFactor + addScore;
                    scene.GameStatisticalData.score2 = this.RuntimeData.CommonConfigData.RookieWinScoreFactor + addScore2;
                }
                else
                {
                    scene.GameStatisticalData.score1 = this.RuntimeData.CommonConfigData.RookieLoseScoreFactor + addScore;
                    scene.GameStatisticalData.score2 = this.RuntimeData.CommonConfigData.RookieLoseScoreFactor + addScore2;
                }
                foreach (BHMatchClientContextData contextData in scene.ClientContextDataDict.Values)
                {
                    if (0 != contextData.BattleWhichSide)
                    {
                        if (1 == contextData.BattleWhichSide)
                        {
                            scene.GameStatisticalData.rolecount1++;
                        }
                        else
                        {
                            scene.GameStatisticalData.rolecount2++;
                        }
                        BHMatchRoleData roleData = new BHMatchRoleData();
                        roleData.rid = contextData.RoleId;
                        roleData.rname = contextData.RoleName;
                        roleData.zoneid = contextData.ZoneID;
                        roleData.kill = contextData.Kill;
                        roleData.mvp = ((contextData.RoleId == scene.ClientContextMVP.RoleId) ? 1 : 0);
                        roleData.bhid = ((contextData.BattleWhichSide == 1) ? fuBenData.bhid1 : fuBenData.bhid2);
                        if (contextData.Kill != 0 || object.ReferenceEquals(contextData, scene.ClientContextMVP))
                        {
                            scene.GameStatisticalData.roleStatisticalData[roleData.rid] = roleData;
                        }
                        GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleId);
                        List<int> roleAnalysisData;
                        if (client != null)
                        {
                            roleAnalysisData = this.GetBHMatchRoleAnalysisData(client);
                        }
                        else
                        {
                            roleAnalysisData = this.GetBHMatchRoleAnalysisData(contextData.RoleId, contextData.ServerId);
                        }
                        if (null != roleAnalysisData)
                        {
                            List<int> list;
                            (list = roleAnalysisData)[8] = list[8] + contextData.Kill;
                            (list = roleAnalysisData)[9] = list[9] + contextData.Kill;
                            (list = roleAnalysisData)[11] = list[11] + 1;
                            (list = roleAnalysisData)[10] = list[10] + ((contextData.BattleWhichSide == scene.SuccessSide) ? 1 : 0);
                            if (contextData.RoleId == scene.ClientContextMVP.RoleId)
                            {
                                if (fuBenData.Type == 1)
                                {
                                    (list = roleAnalysisData)[3] = list[3] + 1;
                                    (list = roleAnalysisData)[2] = list[2] + 1;
                                }
                                else
                                {
                                    (list = roleAnalysisData)[6] = list[6] + 1;
                                    (list = roleAnalysisData)[5] = list[5] + 1;
                                }
                            }
                        }
                        if (client != null)
                        {
                            this.SaveBHMatchRoleAnalysisData(client, roleAnalysisData);
                            GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_BHMatchJoin, new int[0]));
                            GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_BHMatchGoldMVP, new int[0]));
                            GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_BHMatchWin, new int[0]));
                        }
                        else
                        {
                            this.SaveBHMatchRoleAnalysisDataOffline(contextData.RoleId, roleAnalysisData, contextData.ServerId);
                        }
                    }
                }
                RoleDataEx dbRd = this.GetBHMatchBZRoleData(scene.GameStatisticalData.bhid1, scene.GameStatisticalData.serverid1);
                if (null != dbRd)
                {
                    scene.GameStatisticalData.rid1 = dbRd.RoleID;
                    scene.GameStatisticalData.rname1 = dbRd.RoleName;
                    scene.GameStatisticalData.zoneid_r1 = dbRd.ZoneID;
                    if (fuBenData.Type == 1)
                    {
                        scene.GameStatisticalData.bzroledata1 = DataHelper.ObjectToBytes<RoleDataEx>(dbRd);
                    }
                }
                RoleDataEx dbRd2 = this.GetBHMatchBZRoleData(scene.GameStatisticalData.bhid2, scene.GameStatisticalData.serverid2);
                if (null != dbRd2)
                {
                    scene.GameStatisticalData.rid2 = dbRd2.RoleID;
                    scene.GameStatisticalData.rname2 = dbRd2.RoleName;
                    scene.GameStatisticalData.zoneid_r2 = dbRd2.ZoneID;
                    if (fuBenData.Type == 1)
                    {
                        scene.GameStatisticalData.bzroledata2 = DataHelper.ObjectToBytes<RoleDataEx>(dbRd2);
                    }
                }
                TianTiClient.getInstance().GameFuBenComplete_BHMatch(scene.GameStatisticalData);
            }
        }

        // Token: 0x060006F7 RID: 1783 RVA: 0x000678C0 File Offset: 0x00065AC0
        private RoleDataEx GetBHMatchBZRoleData(int bhid, int serverid)
        {
            BangHuiDetailData bhData = Global.GetBangHuiDetailData(-1, bhid, serverid);
            RoleDataEx result;
            if (null == bhData)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("无法获取帮会详细信息 BangHuiID={0} ServerID={1}", bhid, serverid), null, true);
                result = null;
            }
            else
            {
                RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bhData.BZRoleID), serverid);
                if (dbRd == null || dbRd.RoleID <= 0)
                {
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("无法获取帮主详细信息 BangHuiID={0} BZRoleID={1} ServerID={2}", bhid, bhData.BZRoleID, serverid), null, true);
                    result = null;
                }
                else
                {
                    result = dbRd;
                }
            }
            return result;
        }

        // Token: 0x060006F8 RID: 1784 RVA: 0x00067980 File Offset: 0x00065B80
        public List<int> GetBHMatchRoleAnalysisData(int rid, int serverid)
        {
            List<int> result;
            if (0 == this.BHMatchSyncDataCache.CurSeasonID_Gold)
            {
                result = null;
            }
            else
            {
                List<int> countList = Global.GetRoleParamsIntListFromDBOffline(rid, "40", serverid);
                this.FilterBHMatchRoleAnalysisData(countList);
                result = countList;
            }
            return result;
        }

        // Token: 0x060006F9 RID: 1785 RVA: 0x000679C4 File Offset: 0x00065BC4
        public List<int> GetBHMatchRoleAnalysisData(GameClient client)
        {
            List<int> result;
            if (0 == this.BHMatchSyncDataCache.CurSeasonID_Gold)
            {
                result = null;
            }
            else
            {
                List<int> countList = Global.GetRoleParamsIntListFromDB(client, "40");
                this.FilterBHMatchRoleAnalysisData(countList);
                result = countList;
            }
            return result;
        }

        // Token: 0x060006FA RID: 1786 RVA: 0x00067A04 File Offset: 0x00065C04
        public List<int> GetBHMatchRAnalysisExData(GameClient client)
        {
            List<int> countList = Global.GetRoleParamsIntListFromDB(client, "41");
            if (countList.Count != 4)
            {
                for (int i = countList.Count; i < 4; i++)
                {
                    countList.Add(0);
                }
            }
            return countList;
        }

        // Token: 0x060006FB RID: 1787 RVA: 0x00067A50 File Offset: 0x00065C50
        private void FilterBHMatchRoleAnalysisData(List<int> countList)
        {
            if (countList.Count != 14)
            {
                for (int i = countList.Count; i < 14; i++)
                {
                    countList.Add(0);
                }
            }
            if (this.BHMatchSyncDataCache.CurSeasonID_Gold != countList[0])
            {
                countList[1] = countList[0];
                countList[0] = this.BHMatchSyncDataCache.CurSeasonID_Gold;
                countList[4] = countList[3];
                countList[3] = 0;
                countList[8] = 0;
            }
            if (this.BHMatchSyncDataCache.CurSeasonID_Rookie != countList[0])
            {
                countList[13] = countList[12];
                countList[12] = this.BHMatchSyncDataCache.CurSeasonID_Rookie;
                countList[7] = countList[6];
                countList[6] = 0;
                countList[8] = 0;
            }
            if (this.BHMatchSyncDataCache.LastSeasonID_Gold != countList[1])
            {
                countList[1] = this.BHMatchSyncDataCache.LastSeasonID_Gold;
                countList[4] = 0;
            }
            if (this.BHMatchSyncDataCache.LastSeasonID_Rookie != countList[13])
            {
                countList[13] = this.BHMatchSyncDataCache.LastSeasonID_Rookie;
                countList[7] = 0;
            }
        }

        // Token: 0x060006FC RID: 1788 RVA: 0x00067BB7 File Offset: 0x00065DB7
        private void SaveBHMatchRoleAnalysisDataOffline(int rid, List<int> countList, int serverid)
        {
            Global.SaveRoleParamsIntListToDBOffline(rid, countList, "40", serverid);
        }

        // Token: 0x060006FD RID: 1789 RVA: 0x00067BC8 File Offset: 0x00065DC8
        private void SaveBHMatchRoleAnalysisData(GameClient client, List<int> countList)
        {
            Global.SaveRoleParamsIntListToDB(client, countList, "40", true);
        }

        // Token: 0x060006FE RID: 1790 RVA: 0x00067BD9 File Offset: 0x00065DD9
        private void SaveBHMatchRAnalysisExData(GameClient client, List<int> countList)
        {
            Global.SaveRoleParamsIntListToDB(client, countList, "41", true);
        }

        // Token: 0x060006FF RID: 1791 RVA: 0x00067BEC File Offset: 0x00065DEC
        public void RestoreBangHuiMatchNpc()
        {
            NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 141);
            if (null != npc)
            {
                npc.ShowNpc = true;
                GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
                FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.BangHuiMatchBZ, false);
            }
        }

        // Token: 0x06000700 RID: 1792 RVA: 0x00067C44 File Offset: 0x00065E44
        public void ReplaceBangHuiMatchNpc()
        {
            if (null == this.OwnerRoleData)
            {
                this.RestoreBangHuiMatchNpc();
            }
            else
            {
                NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 141);
                if (null != npc)
                {
                    npc.ShowNpc = false;
                    GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
                    FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.BangHuiMatchBZ, false);
                    FakeRoleManager.ProcessNewFakeRole(new SafeClientData
                    {
                        RoleData = this.OwnerRoleData
                    }, npc.MapCode, FakeRoleTypes.BangHuiMatchBZ, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, 141);
                }
            }
        }

        // Token: 0x06000701 RID: 1793 RVA: 0x00067D00 File Offset: 0x00065F00
        private void UpdateChengHaoBuffer(GameClient client)
        {
            if (this.RuntimeData.ChengHaoBHid_Rookie > 0L && (long)client.ClientData.Faction == this.RuntimeData.ChengHaoBHid_Rookie)
            {
                double[] bufferParams = new double[]
                {
                    1.0
                };
                if (client.ClientData.BHZhiWu == 1)
                {
                    Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchBZ_RookieChengHao, bufferParams, 1, true);
                }
                else
                {
                    Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchCY_RookieChengHao, bufferParams, 1, true);
                }
            }
            else
            {
                double[] array = new double[1];
                double[] bufferParams = array;
                Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchBZ_RookieChengHao, bufferParams, 1, true);
                Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchCY_RookieChengHao, bufferParams, 1, true);
            }
            if (this.RuntimeData.ChengHaoBHid_Gold > 0L && (long)client.ClientData.Faction == this.RuntimeData.ChengHaoBHid_Gold)
            {
                double[] bufferParams = new double[]
                {
                    1.0
                };
                if (client.ClientData.BHZhiWu == 1)
                {
                    Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchBZ_GoldChengHao, bufferParams, 1, true);
                    FashionManager.getInstance().GetFashionByMagic(client, FashionIdConsts.BangHuiMatchYuYi, true);
                }
                else
                {
                    Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchCY_GoldChengHao, bufferParams, 1, true);
                    FashionManager.getInstance().DelFashionByMagic(client, FashionIdConsts.BangHuiMatchYuYi);
                }
            }
            else
            {
                double[] array = new double[1];
                double[] bufferParams = array;
                Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchBZ_GoldChengHao, bufferParams, 1, true);
                Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchCY_GoldChengHao, bufferParams, 1, true);
                FashionManager.getInstance().DelFashionByMagic(client, FashionIdConsts.BangHuiMatchYuYi);
            }
        }

        // Token: 0x06000702 RID: 1794 RVA: 0x00067EA0 File Offset: 0x000660A0
        public void SaveBHMatchBHListGoldJoin()
        {
            List<BangHuiMatchRankInfo> rankInfoList = new List<BangHuiMatchRankInfo>();
            if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
            {
                this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(8, out rankInfoList);
            }
            string goldjoin = "";
            int i = 0;
            while (rankInfoList != null && i < rankInfoList.Count)
            {
                goldjoin += string.Format("{0}|", rankInfoList[i].Key);
                i++;
            }
            if (!string.IsNullOrEmpty(goldjoin) && goldjoin.Substring(goldjoin.Length - 1) == "|")
            {
                goldjoin = goldjoin.Substring(0, goldjoin.Length - 1);
            }
            GameManager.GameConfigMgr.UpdateGameConfigItem("bhmatch_goldjoin", goldjoin, true);
        }

        // Token: 0x06000703 RID: 1795 RVA: 0x00067F74 File Offset: 0x00066174
        public bool RefreshBHMatchChampionBH(int rankType)
        {
            bool chgChampion = false;
            int newChampionBH = 0;
            List<BangHuiMatchRankInfo> rankInfoList = new List<BangHuiMatchRankInfo>();
            if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
            {
                this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(rankType, out rankInfoList);
            }
            if (rankInfoList != null && rankInfoList.Count != 0)
            {
                newChampionBH = rankInfoList[0].Key;
            }
            if (rankType == 1)
            {
                if (this.RuntimeData.ChengHaoBHid_Rookie != (long)newChampionBH)
                {
                    chgChampion = true;
                }
                this.RuntimeData.ChengHaoBHid_Rookie = (long)newChampionBH;
            }
            if (rankType == 0)
            {
                if (this.RuntimeData.ChengHaoBHid_Gold != (long)newChampionBH)
                {
                    chgChampion = true;
                }
                this.RuntimeData.ChengHaoBHid_Gold = (long)newChampionBH;
            }
            return chgChampion;
        }

        // Token: 0x06000704 RID: 1796 RVA: 0x00068048 File Offset: 0x00066248
        public bool PreRemoveBangHui(GameClient client)
        {
            string gamestate = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(client.ClientData.Faction);
            bool result;
            if (string.IsNullOrEmpty(gamestate))
            {
                result = true;
            }
            else
            {
                string[] gamestateFields = gamestate.Split(new char[]
                {
                    ':'
                });
                if (gamestateFields.Length != 2)
                {
                    result = true;
                }
                else
                {
                    int matchType = Global.SafeConvertToInt32(gamestateFields[0]);
                    int signState = Global.SafeConvertToInt32(gamestateFields[1]);
                    result = (matchType != 1);
                }
            }
            return result;
        }

        // Token: 0x06000705 RID: 1797 RVA: 0x000680D4 File Offset: 0x000662D4
        public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
        {
            BHMatchFuBenData fubenData = TianTiClient.getInstance().GetFuBenDataByBhid_BHMatch(e.BHID);
            string gamestate = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(e.BHID);
            bool result;
            if (string.IsNullOrEmpty(gamestate))
            {
                result = false;
            }
            else
            {
                string[] gamestateFields = gamestate.Split(new char[]
                {
                    ':'
                });
                if (gamestateFields.Length != 2)
                {
                    result = false;
                }
                else
                {
                    int matchType = Global.SafeConvertToInt32(gamestateFields[0]);
                    int signState = Global.SafeConvertToInt32(gamestateFields[1]);
                    BHMatchConfig sceneItem = null;
                    BangHuiMatchGameStates timeState = BangHuiMatchGameStates.None;
                    this.CheckCondition(e.Player, matchType, ref sceneItem, ref timeState);
                    if (timeState == BangHuiMatchGameStates.Start && null != fubenData)
                    {
                        e.Result = false;
                        GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(2601, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        // Token: 0x06000706 RID: 1798 RVA: 0x000681BC File Offset: 0x000663BC
        public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
        {
            BHMatchFuBenData fubenData = TianTiClient.getInstance().GetFuBenDataByBhid_BHMatch(e.BHID);
            string gamestate = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(e.BHID);
            bool result;
            if (string.IsNullOrEmpty(gamestate))
            {
                result = false;
            }
            else
            {
                string[] gamestateFields = gamestate.Split(new char[]
                {
                    ':'
                });
                if (gamestateFields.Length != 2)
                {
                    result = false;
                }
                else
                {
                    int matchType = Global.SafeConvertToInt32(gamestateFields[0]);
                    int signState = Global.SafeConvertToInt32(gamestateFields[1]);
                    BHMatchConfig sceneItem = null;
                    BangHuiMatchGameStates timeState = BangHuiMatchGameStates.None;
                    this.CheckCondition(e.Player, matchType, ref sceneItem, ref timeState);
                    if (timeState == BangHuiMatchGameStates.Start && null != fubenData)
                    {
                        e.Result = false;
                        GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(2601, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        // Token: 0x06000707 RID: 1799 RVA: 0x000682A4 File Offset: 0x000664A4
        private void InitScene(BangHuiMatchScene scene, GameClient client)
        {
            foreach (BHMatchQiZhiConfig item in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
            {
                scene.NPCID2QiZhiConfigDict.Add(item.NPCID, item.Clone() as BHMatchQiZhiConfig);
            }
        }

        // Token: 0x06000708 RID: 1800 RVA: 0x00068320 File Offset: 0x00066520
        public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
        {
            bool result;
            if (sceneType == SceneUIClasses.BangHuiMatch)
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
                    DateTime now = TimeUtil.NowDateTime();
                    lock (this.RuntimeData.Mutex)
                    {
                        BangHuiMatchScene scene = null;
                        if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
                        {
                            BHMatchConfig sceneInfo = null;
                            BHMatchFuBenData fuBenData;
                            if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
                            {
                                LogManager.WriteLog(LogTypes.Error, "战盟联赛没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
                            }
                            if (!this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue((int)fuBenData.Type, out sceneInfo))
                            {
                                LogManager.WriteLog(LogTypes.Error, "战盟联赛没有为副本找到对应的场景数据,ID:" + fuBenData.Type, null, true);
                            }
                            scene = new BangHuiMatchScene();
                            scene.CleanAllInfo();
                            scene.GameId = (int)gameId;
                            scene.FuBenSeqId = fuBenSeqId;
                            scene.SceneInfo = sceneInfo;
                            scene.MapGridWidth = gameMap.MapGridWidth;
                            scene.MapGridHeight = gameMap.MapGridHeight;
                            DateTime startTime = now.Date.Add(this.GetStartTime(sceneInfo.ID));
                            scene.StartTimeTicks = startTime.Ticks / 10000L;
                            this.InitScene(scene, client);
                            scene.GameStatisticalData.GameId = (int)gameId;
                            this.SceneDict[fuBenSeqId] = scene;
                        }
                        scene.CopyMapDict[mapCode] = copyMap;
                        BHMatchClientContextData clientContextData;
                        if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
                        {
                            clientContextData = new BHMatchClientContextData
                            {
                                RoleId = roleId,
                                ServerId = client.ServerId,
                                BattleWhichSide = client.ClientData.BattleWhichSide,
                                RoleName = client.ClientData.RoleName,
                                Occupation = client.ClientData.Occupation,
                                RoleSex = client.ClientData.RoleSex,
                                ZoneID = client.ClientData.ZoneID
                            };
                            scene.ClientContextDataDict[roleId] = clientContextData;
                        }
                        if (client.ClientData.BattleWhichSide == 1)
                        {
                            scene.GameStatisticalData.bhname1 = client.ClientData.BHName;
                            scene.GameStatisticalData.serverid1 = client.ServerId;
                        }
                        if (client.ClientData.BattleWhichSide == 2)
                        {
                            scene.GameStatisticalData.bhname2 = client.ClientData.BHName;
                            scene.GameStatisticalData.serverid2 = client.ServerId;
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

        // Token: 0x06000709 RID: 1801 RVA: 0x000686B0 File Offset: 0x000668B0
        public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
        {
            bool result;
            if (sceneType == SceneUIClasses.BangHuiMatch)
            {
                lock (this.RuntimeData.Mutex)
                {
                    BangHuiMatchScene BangHuiMatchScene;
                    this.SceneDict.TryRemove(copyMap.FuBenSeqID, out BangHuiMatchScene);
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600070A RID: 1802 RVA: 0x00068728 File Offset: 0x00066928
        private int CheckCondition(GameClient client, int matchType, ref BHMatchConfig sceneItem, ref BangHuiMatchGameStates state)
        {
            int result = 0;
            sceneItem = null;
            lock (this.RuntimeData.Mutex)
            {
                if (!this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue(matchType, out sceneItem))
                {
                    return -12;
                }
            }
            state = BangHuiMatchGameStates.SignUp;
            result = 0;
            DateTime now = TimeUtil.NowDateTime();
            lock (this.RuntimeData.Mutex)
            {
                for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
                {
                    if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.ApplyOverTime && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1] + (double)sceneItem.ApplyStartTime)
                    {
                        if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
                        {
                            state = BangHuiMatchGameStates.Wait;
                            result = 0;
                        }
                        else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
                        {
                            state = BangHuiMatchGameStates.Start;
                            result = 0;
                        }
                        else
                        {
                            state = BangHuiMatchGameStates.None;
                            result = -2001;
                        }
                        break;
                    }
                }
            }
            return result;
        }

        // Token: 0x0600070B RID: 1803 RVA: 0x00068928 File Offset: 0x00066B28
        private TimeSpan GetStartTime(int sceneId)
        {
            BHMatchConfig sceneItem = null;
            TimeSpan startTime = TimeSpan.MinValue;
            DateTime now = TimeUtil.NowDateTime();
            lock (this.RuntimeData.Mutex)
            {
                if (!this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue(sceneId, out sceneItem))
                {
                    goto IL_158;
                }
            }
            lock (this.RuntimeData.Mutex)
            {
                for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
                {
                    if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.ApplyOverTime && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
                    {
                        startTime = TimeSpan.FromSeconds(sceneItem.SecondsOfDay[i]);
                        break;
                    }
                }
            }
            IL_158:
            if (startTime < TimeSpan.Zero)
            {
                startTime = now.TimeOfDay;
            }
            return startTime;
        }

        // Token: 0x0600070C RID: 1804 RVA: 0x00068AD0 File Offset: 0x00066CD0
        public bool ClientRelive(GameClient client)
        {
            int mapCode = client.ClientData.MapCode;
            BHMatchConfig sceneInfo = client.SceneInfoObject as BHMatchConfig;
            if (null != sceneInfo)
            {
                int toMapCode;
                int toPosX;
                int toPosY;
                if (this.GetZhanMengBirthPoint(sceneInfo, client, toMapCode = sceneInfo.MapCode, out toMapCode, out toPosX, out toPosY, false))
                {
                    client.ClientData.CurrentLifeV = client.ClientData.LifeV;
                    client.ClientData.CurrentMagicV = client.ClientData.MagicV;
                    client.ClientData.MoveAndActionNum = 0;
                    GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, toPosX, toPosY, -1);
                    if (toMapCode != client.ClientData.MapCode)
                    {
                        GameManager.ClientMgr.NotifyMySelfRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, -1);
                        client.ClientData.KuaFuChangeMapCode = toMapCode;
                        GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, toPosX, toPosY, -1, 1);
                    }
                    else
                    {
                        Global.ClientRealive(client, toPosX, toPosY, -1);
                    }
                    return true;
                }
            }
            return false;
        }

        // Token: 0x0600070D RID: 1805 RVA: 0x00068C3C File Offset: 0x00066E3C
        public void OnProcessMonsterDead(GameClient client, Monster monster)
        {
            if (client != null && this.IsQiZhiExtensionID(monster.MonsterInfo.ExtensionID))
            {
                BangHuiMatchScene scene = client.SceneObject as BangHuiMatchScene;
                BHMatchQiZhiConfig qizhiConfig = monster.Tag as BHMatchQiZhiConfig;
                if (scene != null && null != qizhiConfig)
                {
                    lock (this.RuntimeData.Mutex)
                    {
                        qizhiConfig.DeadTicks = TimeUtil.NOW();
                        qizhiConfig.Alive = false;
                        qizhiConfig.BattleWhichSide = client.ClientData.BattleWhichSide;
                        qizhiConfig.OwnTicks = 0L;
                        qizhiConfig.OwnTicksDelta = 0L;
                        this.UpdateQiZhiBangHuiOwnNum(scene);
                        foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                        {
                            GameManager.ClientMgr.BroadSpecialCopyMapMessage<BangHuiMatchScoreData>(1173, scene.ScoreData, copyMap);
                        }
                    }
                }
            }
        }

        // Token: 0x0600070E RID: 1806 RVA: 0x00068DC0 File Offset: 0x00066FC0
        private void ProcessEnd(BangHuiMatchScene scene, int successSide, long nowTicks)
        {
            if (successSide != 0)
            {
                List<BHMatchClientContextData> winSidePlayerList = new List<BHMatchClientContextData>();
                foreach (BHMatchClientContextData item in scene.ClientContextDataDict.Values)
                {
                    if (item.BattleWhichSide == successSide)
                    {
                        winSidePlayerList.Add(item);
                    }
                }
                winSidePlayerList.Sort(delegate (BHMatchClientContextData left, BHMatchClientContextData right)
                {
                    int result;
                    if (left.TotalScore > right.TotalScore)
                    {
                        result = -1;
                    }
                    else if (left.TotalScore < right.TotalScore)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = 0;
                    }
                    return result;
                });
                if (winSidePlayerList.Count != 0)
                {
                    scene.ClientContextMVP = winSidePlayerList[0];
                }
            }
            this.CompleteScene(scene, successSide);
            scene.m_eStatus = GameSceneStatuses.STATUS_END;
            scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
            scene.StateTimeData.GameType = 24;
            scene.StateTimeData.State = 5;
            scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
            foreach (CopyMap copyMap in scene.CopyMapDict.Values)
            {
                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copyMap);
            }
        }

        // Token: 0x0600070F RID: 1807 RVA: 0x00068F38 File Offset: 0x00067138
        public void TimerProc()
        {
            long nowTicks = TimeUtil.NOW();
            if (nowTicks >= BangHuiMatchManager.NextHeartBeatTicks)
            {
                BangHuiMatchManager.NextHeartBeatTicks = nowTicks + 1020L;
                foreach (BangHuiMatchScene scene in this.SceneDict.Values)
                {
                    lock (this.RuntimeData.Mutex)
                    {
                        int nID = scene.FuBenSeqId;
                        if (nID >= 0)
                        {
                            DateTime now = TimeUtil.NowDateTime();
                            long ticks = TimeUtil.NOW();
                            if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
                            {
                                if (ticks >= scene.StartTimeTicks)
                                {
                                    scene.m_lPrepareTime = scene.StartTimeTicks;
                                    scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
                                    scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
                                    scene.StateTimeData.GameType = 24;
                                    scene.StateTimeData.State = (int)scene.m_eStatus;
                                    scene.StateTimeData.EndTicks = scene.m_lBeginTime;
                                    foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                                    {
                                        GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copyMap);
                                    }
                                }
                            }
                            else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
                            {
                                if (ticks >= scene.m_lBeginTime)
                                {
                                    scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
                                    scene.m_lEndTime = scene.m_lBeginTime + (long)(scene.SceneInfo.FightingSecs * 1000);
                                    scene.StateTimeData.GameType = 24;
                                    scene.StateTimeData.State = (int)scene.m_eStatus;
                                    scene.StateTimeData.EndTicks = scene.m_lEndTime;
                                    foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                                    {
                                        GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copyMap);
                                    }
                                    foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                                    {
                                        if (copyMap.MapCode == scene.SceneInfo.MapCode)
                                        {
                                            for (int guangMuId = 1; guangMuId <= 2; guangMuId++)
                                            {
                                                GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, guangMuId, 0);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
                            {
                                if (ticks >= scene.m_lEndTime)
                                {
                                    int successSide = 0;
                                    if (scene.ScoreData.Score1 > scene.ScoreData.Score2)
                                    {
                                        successSide = 1;
                                    }
                                    else if (scene.ScoreData.Score2 > scene.ScoreData.Score1)
                                    {
                                        successSide = 2;
                                    }
                                    if (0 != scene.LT_BattleWhichSide)
                                    {
                                        if (successSide != 0 && successSide != scene.LT_BattleWhichSide)
                                        {
                                            scene.GameStatisticalData.bullshit = true;
                                        }
                                        successSide = scene.LT_BattleWhichSide;
                                    }
                                    this.ProcessEnd(scene, successSide, ticks);
                                }
                                else
                                {
                                    this.CheckSceneScoreTime(scene, ticks);
                                    this.CheckSceneTempleDamage(scene, ticks);
                                }
                            }
                            else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
                            {
                                scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
                                this.GiveAwards(scene);
                                foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                                {
                                    GameManager.CopyMapMgr.KillAllMonster(copyMap);
                                }
                                BHMatchFuBenData fuBenData;
                                if (this.RuntimeData.FuBenItemData.TryGetValue((long)scene.GameId, out fuBenData))
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("战盟联赛跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
                                    this.RuntimeData.FuBenItemData.Remove((long)scene.GameId);
                                }
                            }
                            else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
                            {
                                if (ticks >= scene.m_lLeaveTime)
                                {
                                    foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                                    {
                                        copyMap.SetRemoveTicks(scene.m_lLeaveTime);
                                        scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
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
                                            DataHelper.WriteExceptionLogEx(ex, "战盟联赛系统清场调度异常");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000710 RID: 1808 RVA: 0x000695E0 File Offset: 0x000677E0
        public void CompleteScene(BangHuiMatchScene scene, int successSide)
        {
            scene.SuccessSide = successSide;
        }

        // Token: 0x06000711 RID: 1809 RVA: 0x000695EC File Offset: 0x000677EC
        public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
        {
            lock (this.RuntimeData.Mutex)
            {
                BangHuiMatchScene scene;
                if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
                {
                    if (timeState)
                    {
                        client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
                    }
                    if (sideScore)
                    {
                        client.sendCmd<BangHuiMatchScoreData>(1173, scene.ScoreData, false);
                    }
                }
            }
        }

        // Token: 0x06000712 RID: 1810 RVA: 0x00069694 File Offset: 0x00067894
        public void RemoveBattleSceneBuffForRole(BangHuiMatchScene scene, GameClient client)
        {
            this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_QiZhi, 0);
            this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_Temple, 0);
        }

        // Token: 0x06000713 RID: 1811 RVA: 0x000696B4 File Offset: 0x000678B4
        public void OnKillRole(GameClient client, GameClient other)
        {
            lock (this.RuntimeData.Mutex)
            {
                BangHuiMatchScene scene = client.SceneObject as BangHuiMatchScene;
                if (scene != null && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
                {
                    if (client.ClientData.BattleWhichSide == 1)
                    {
                        scene.GameStatisticalData.kill1++;
                    }
                    if (client.ClientData.BattleWhichSide == 2)
                    {
                        scene.GameStatisticalData.kill2++;
                    }
                    BHMatchClientContextData clientContextData = client.SceneContextData2 as BHMatchClientContextData;
                    if (null != clientContextData)
                    {
                        clientContextData.TotalScore += this.CalMVPScore(scene, this.RuntimeData.MVPScoreFactorKill);
                        clientContextData.Kill++;
                    }
                    this.UpdateLongTaOwnInfo(scene);
                }
            }
        }

        // Token: 0x06000714 RID: 1812 RVA: 0x000697CC File Offset: 0x000679CC
        public void OnInjureMonster(GameClient client, Monster monster, long injure)
        {
            BHMatchQiZhiConfig tagInfo = monster.Tag as BHMatchQiZhiConfig;
            if (null != tagInfo)
            {
                lock (this.RuntimeData.Mutex)
                {
                    BangHuiMatchScene scene = client.SceneObject as BangHuiMatchScene;
                    if (scene != null && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
                    {
                        BHMatchClientContextData clientContextData = client.SceneContextData2 as BHMatchClientContextData;
                        if (null != clientContextData)
                        {
                            clientContextData.TotalScore += this.CalMVPScore(scene, this.RuntimeData.MVPScoreFactorQiZhi);
                        }
                    }
                }
            }
        }

        // Token: 0x06000715 RID: 1813 RVA: 0x00069898 File Offset: 0x00067A98
        public void LeaveFuBen(GameClient client)
        {
            BangHuiMatchScene scene = client.SceneObject as BangHuiMatchScene;
            if (null != scene)
            {
                this.RemoveBattleSceneBuffForRole(scene, client);
            }
        }

        // Token: 0x06000716 RID: 1814 RVA: 0x000698C5 File Offset: 0x00067AC5
        public void OnLogout(GameClient client)
        {
            this.LeaveFuBen(client);
        }

        // Token: 0x06000717 RID: 1815 RVA: 0x000698D0 File Offset: 0x00067AD0
        public void OnLogoutFinish(GameClient client)
        {
            BangHuiMatchScene scene = client.SceneObject as BangHuiMatchScene;
            if (null != scene)
            {
                if (client.ClientData.MapCode == scene.SceneInfo.MapCode_LongTa)
                {
                    this.UpdateLongTaOwnInfo(scene);
                }
            }
        }

        // Token: 0x06000718 RID: 1816 RVA: 0x0006991C File Offset: 0x00067B1C
        public void OnStartPlayGame(GameClient client)
        {
            BangHuiMatchScene scene = client.SceneObject as BangHuiMatchScene;
            if (null != scene)
            {
                if (client.ClientData.MapCode == scene.SceneInfo.MapCode_LongTa)
                {
                    BHMatchClientContextData clientContextData = client.SceneContextData2 as BHMatchClientContextData;
                    clientContextData.TempleEnterTicks = TimeUtil.NOW();
                    clientContextData.TempleDamageTimes = 0;
                }
                if (this.IsBangHuiMatchMap(client.ClientData.MapCode))
                {
                    this.UpdateLongTaOwnInfo(scene);
                }
                this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
                if (scene.LT_BattleWhichSide == client.ClientData.BattleWhichSide)
                {
                    this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_Temple, 2080013);
                }
                else
                {
                    this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_Temple, 0);
                }
                this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_QiZhi, this.GetQiZhiBuffGoodsIDBySide(scene, client.ClientData.BattleWhichSide));
            }
            this.UpdateChengHaoBuffer(client);
        }

        // Token: 0x06000719 RID: 1817 RVA: 0x00069A14 File Offset: 0x00067C14
        public bool ClientChangeMap(GameClient client, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
        {
            BHMatchConfig sceneInfo = client.SceneInfoObject as BHMatchConfig;
            if (null != sceneInfo)
            {
                if (toNewMapCode == sceneInfo.MapCode || toNewMapCode == sceneInfo.MapCode_LongTa)
                {
                    if (client.ClientData.MapCode != sceneInfo.MapCode_LongTa)
                    {
                        int toMapCode;
                        int toPosX;
                        int toPosY;
                        if (this.GetZhanMengBirthPoint(sceneInfo, client, toNewMapCode, out toMapCode, out toPosX, out toPosY, false))
                        {
                            toNewMapCode = toMapCode;
                            toNewPosX = toPosX;
                            toNewPosY = toPosY;
                        }
                    }
                }
            }
            return true;
        }

        // Token: 0x0600071A RID: 1818 RVA: 0x00069AAC File Offset: 0x00067CAC
        private void UpdateLongTaOwnInfo(BangHuiMatchScene scene)
        {
            if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
            {
                lock (this.RuntimeData.Mutex)
                {
                    CopyMap copyMapLongTa = null;
                    if (scene.CopyMapDict.TryGetValue(scene.SceneInfo.MapCode_LongTa, out copyMapLongTa))
                    {
                        this.RuntimeData.UpdateLongTaOwnInfoTimes += 1L;
                        List<GameClient> lsClients = copyMapLongTa.GetClientsList();
                        lsClients = Global.GetMapAliveClientsEx(lsClients, scene.SceneInfo.MapCode_LongTa, true, this.RuntimeData.UpdateLongTaOwnInfoTimes);
                        Dictionary<int, BHMatchRoleCountData> dict = new Dictionary<int, BHMatchRoleCountData>();
                        for (int i = 0; i < lsClients.Count; i++)
                        {
                            GameClient client = lsClients[i];
                            if (client != null && client.ClientData.HideGM <= 0)
                            {
                                if (client.ClientData.Faction > 0)
                                {
                                    BHMatchRoleCountData data;
                                    if (!dict.TryGetValue(client.ClientData.BattleWhichSide, out data))
                                    {
                                        data = new BHMatchRoleCountData
                                        {
                                            bhid = client.ClientData.Faction,
                                            bhname = client.ClientData.BHName,
                                            rolecount = 0,
                                            BattleWhichSide = client.ClientData.BattleWhichSide,
                                            serverid = client.ServerId
                                        };
                                        dict.Add(client.ClientData.BattleWhichSide, data);
                                    }
                                    data.rolecount++;
                                }
                            }
                        }
                        int PlayerNum = 0;
                        int PlayerNum2 = 0;
                        BHMatchRoleCountData countData = null;
                        dict.TryGetValue(1, out countData);
                        if (null != countData)
                        {
                            PlayerNum = countData.rolecount;
                        }
                        BHMatchRoleCountData countData2 = null;
                        dict.TryGetValue(2, out countData2);
                        if (null != countData2)
                        {
                            PlayerNum2 = countData2.rolecount;
                        }
                        bool NotifyScoreData = false;
                        if (scene.ScoreData.PlayerNum1 != PlayerNum || scene.ScoreData.PlayerNum2 != PlayerNum2)
                        {
                            NotifyScoreData = true;
                        }
                        scene.ScoreData.PlayerNum1 = PlayerNum;
                        scene.ScoreData.PlayerNum2 = PlayerNum2;
                        if (dict.Count == 1)
                        {
                            BHMatchRoleCountData countData3 = dict.Values.FirstOrDefault<BHMatchRoleCountData>();
                            if (scene.LT_BattleWhichSide != countData3.BattleWhichSide)
                            {
                                this.HandleTakeTemple(scene, lsClients, countData3.BattleWhichSide);
                            }
                            scene.ScoreData.LT_BHName = countData3.bhname;
                            scene.LT_BHServerID = countData3.serverid;
                            scene.LT_BattleWhichSide = countData3.BattleWhichSide;
                            scene.LT_OwnTicks = TimeUtil.NOW();
                            scene.LT_OwnTicksDelta = 0L;
                        }
                        else if (dict.Count == 2)
                        {
                            if (0 == scene.LT_BattleWhichSide)
                            {
                                BHMatchRoleCountData countData3;
                                if (dict.TryGetValue(lsClients[0].ClientData.BattleWhichSide, out countData3))
                                {
                                    this.HandleTakeTemple(scene, lsClients, countData3.BattleWhichSide);
                                    scene.ScoreData.LT_BHName = countData3.bhname;
                                    scene.LT_BHServerID = countData3.serverid;
                                    scene.LT_BattleWhichSide = countData3.BattleWhichSide;
                                    scene.LT_OwnTicks = TimeUtil.NOW();
                                    scene.LT_OwnTicksDelta = 0L;
                                }
                            }
                        }
                        else
                        {
                            scene.ScoreData.LT_BHName = "";
                            scene.LT_BattleWhichSide = 0;
                            scene.LT_OwnTicks = 0L;
                            scene.LT_OwnTicksDelta = 0L;
                        }
                        if (NotifyScoreData)
                        {
                            foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                            {
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<BangHuiMatchScoreData>(1173, scene.ScoreData, copyMap);
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0600071B RID: 1819 RVA: 0x00069EE4 File Offset: 0x000680E4
        private void HandleTakeTemple(BangHuiMatchScene scene, List<GameClient> lsClients, int BattleWhichSide)
        {
            for (int i = 0; i < lsClients.Count; i++)
            {
                GameClient client = lsClients[i];
                if (client.ClientData.BattleWhichSide == BattleWhichSide)
                {
                    BHMatchClientContextData clientContextData = client.SceneContextData2 as BHMatchClientContextData;
                    if (null != clientContextData)
                    {
                        clientContextData.TotalScore += this.CalMVPScore(scene, this.RuntimeData.MVPScoreFactorTemple);
                    }
                    this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_Temple, 2080013);
                }
                else
                {
                    this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_Temple, 0);
                }
            }
            scene.GameStatisticalData.templechg++;
        }

        // Token: 0x0600071C RID: 1820 RVA: 0x00069F98 File Offset: 0x00068198
        private int GetQiZhiBuffGoodsIDBySide(BangHuiMatchScene scene, int side)
        {
            int BuffGoodsID;
            switch ((side == 1) ? scene.ScoreData.QiZhi1 : scene.ScoreData.QiZhi2)
            {
                case 1:
                    BuffGoodsID = 2080014;
                    break;
                case 2:
                    BuffGoodsID = 2080015;
                    break;
                case 3:
                    BuffGoodsID = 2080016;
                    break;
                case 4:
                    BuffGoodsID = 2080017;
                    break;
                default:
                    BuffGoodsID = 0;
                    break;
            }
            return BuffGoodsID;
        }

        // Token: 0x0600071D RID: 1821 RVA: 0x0006A00C File Offset: 0x0006820C
        private double CalClientDehurtValue(BangHuiMatchScene scene, GameClient client)
        {
            double dehurtv = 0.0;
            int QiZhiNum = (client.ClientData.BattleWhichSide == 1) ? scene.ScoreData.QiZhi1 : scene.ScoreData.QiZhi2;
            int LongTaOwn = (client.ClientData.BattleWhichSide == scene.LT_BattleWhichSide) ? 1 : 0;
            dehurtv += this.RuntimeData.BHMatchGodDebuffQiZhi * (double)QiZhiNum;
            return dehurtv + this.RuntimeData.BHMatchGodDebuffTemple * (double)LongTaOwn;
        }

        // Token: 0x0600071E RID: 1822 RVA: 0x0006A08C File Offset: 0x0006828C
        private void UpdateQiZhiBangHuiOwnNum(BangHuiMatchScene scene)
        {
            int QiZhi = 0;
            int QiZhi2 = 0;
            lock (this.RuntimeData.Mutex)
            {
                foreach (BHMatchQiZhiConfig item in scene.NPCID2QiZhiConfigDict.Values)
                {
                    if (item.BattleWhichSide == 1 && item.Alive)
                    {
                        QiZhi++;
                    }
                    if (item.BattleWhichSide == 2 && item.Alive)
                    {
                        QiZhi2++;
                    }
                }
            }
            scene.ScoreData.QiZhi1 = QiZhi;
            scene.ScoreData.QiZhi2 = QiZhi2;
            foreach (CopyMap copyMap in scene.CopyMapDict.Values)
            {
                List<GameClient> objsList = copyMap.GetClientsList();
                if (objsList != null && objsList.Count > 0)
                {
                    for (int i = 0; i < objsList.Count; i++)
                    {
                        GameClient c = objsList[i];
                        if (c != null && c.ClientData.CopyMapID == copyMap.CopyMapID)
                        {
                            this.UpdateBuff4GameClient(c, BufferItemTypes.BangHuiMatchDeHurt_QiZhi, this.GetQiZhiBuffGoodsIDBySide(scene, c.ClientData.BattleWhichSide));
                        }
                    }
                }
            }
        }

        // Token: 0x0600071F RID: 1823 RVA: 0x0006A268 File Offset: 0x00068468
        private void CheckSceneTempleDamage(BangHuiMatchScene scene, long nowTicks)
        {
            CopyMap copyMap = null;
            if (scene.CopyMapDict.TryGetValue(scene.SceneInfo.MapCode_LongTa, out copyMap))
            {
                List<GameClient> lsClients = copyMap.GetClientsList();
                lsClients = Global.GetMapAliveClientsEx(lsClients, scene.SceneInfo.MapCode_LongTa, false, 0L);
                bool AnyOneDead = false;
                for (int i = 0; i < lsClients.Count; i++)
                {
                    GameClient client = lsClients[i];
                    if (client != null && client.ClientData.HideGM <= 0)
                    {
                        BHMatchClientContextData clientContextData = client.SceneContextData2 as BHMatchClientContextData;
                        int beginSeconds = (int)(nowTicks - clientContextData.TempleEnterTicks) / 1000;
                        int hurtTimes = beginSeconds / this.RuntimeData.BHMatchGodDamagePeriod;
                        if (hurtTimes > 0 && clientContextData.TempleDamageTimes != hurtTimes)
                        {
                            clientContextData.TempleDamageTimes = hurtTimes;
                            double hurtPct;
                            if (hurtTimes > this.RuntimeData.BHMatchGodDamagePctList.Count)
                            {
                                hurtPct = this.RuntimeData.BHMatchGodDamagePctList[this.RuntimeData.BHMatchGodDamagePctList.Count - 1];
                            }
                            else
                            {
                                hurtPct = this.RuntimeData.BHMatchGodDamagePctList[hurtTimes - 1];
                            }
                            double realHurtPct = hurtPct * (1.0 - this.CalClientDehurtValue(scene, client));
                            double hurtValue = (double)client.ClientData.LifeV * realHurtPct;
                            int v = client.ClientData.CurrentLifeV;
                            client.ClientData.CurrentLifeV -= (int)hurtValue;
                            hurtValue = (double)(v - client.ClientData.CurrentLifeV);
                            if (hurtValue <= 0.0)
                            {
                                return;
                            }
                            GameManager.ClientMgr.SubSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, hurtValue);
                            GameManager.ClientMgr.NotifySpriteInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.RoleID, client.ClientData.RoleID, 0, (int)hurtValue, (double)client.ClientData.CurrentLifeV, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
                            client.sendCmd(1178, "", false);
                            AnyOneDead |= (client.ClientData.CurrentLifeV <= 0);
                        }
                    }
                }
                if (AnyOneDead)
                {
                    this.UpdateLongTaOwnInfo(scene);
                }
            }
        }

        // Token: 0x06000720 RID: 1824 RVA: 0x0006A520 File Offset: 0x00068720
        private void CheckSceneScoreTime(BangHuiMatchScene scene, long nowTicks)
        {
            lock (this.RuntimeData.Mutex)
            {
                bool NotifyScoreData = false;
                foreach (KeyValuePair<int, BHMatchQiZhiConfig> item in scene.NPCID2QiZhiConfigDict)
                {
                    BHMatchQiZhiConfig qizhi = item.Value;
                    if (qizhi.BattleWhichSide != 0 && qizhi.Alive)
                    {
                        qizhi.OwnTicksDelta += nowTicks - qizhi.OwnTicks;
                        qizhi.OwnTicks = nowTicks;
                        if (qizhi.OwnTicksDelta >= (long)(qizhi.ProduceTime * 1000) && qizhi.ProduceTime > 0)
                        {
                            int calRate = (int)(qizhi.OwnTicksDelta / (long)(qizhi.ProduceTime * 1000));
                            qizhi.OwnTicksDelta -= (long)(calRate * qizhi.ProduceTime * 1000);
                            if (qizhi.BattleWhichSide == 1)
                            {
                                scene.ScoreData.Score1 += calRate * qizhi.ProduceNum;
                            }
                            if (qizhi.BattleWhichSide == 2)
                            {
                                scene.ScoreData.Score2 += calRate * qizhi.ProduceNum;
                            }
                            NotifyScoreData = true;
                        }
                    }
                }
                if (scene.LT_BattleWhichSide != 0)
                {
                    scene.LT_OwnTicksDelta += nowTicks - scene.LT_OwnTicks;
                    scene.LT_OwnTicks = nowTicks;
                    if (scene.LT_OwnTicksDelta >= (long)(this.RuntimeData.TempleProduceTime * 1000) && this.RuntimeData.TempleProduceTime > 0)
                    {
                        int calRate = (int)(scene.LT_OwnTicksDelta / (long)(this.RuntimeData.TempleProduceTime * 1000));
                        scene.LT_OwnTicksDelta -= (long)(calRate * this.RuntimeData.TempleProduceTime * 1000);
                        if (scene.LT_BattleWhichSide == 1)
                        {
                            scene.ScoreData.Score1 += calRate * this.RuntimeData.TempleProduceNum;
                        }
                        if (scene.LT_BattleWhichSide == 2)
                        {
                            scene.ScoreData.Score2 += calRate * this.RuntimeData.TempleProduceNum;
                        }
                        NotifyScoreData = true;
                    }
                }
                if (NotifyScoreData)
                {
                    foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                    {
                        GameManager.ClientMgr.BroadSpecialCopyMapMessage<BangHuiMatchScoreData>(1173, scene.ScoreData, copyMap);
                    }
                }
            }
        }

        // Token: 0x06000721 RID: 1825 RVA: 0x0006A858 File Offset: 0x00068A58
        private int CalMVPScore(BangHuiMatchScene scene, int factor)
        {
            int beginSecs = (int)(TimeUtil.NOW() - scene.m_lBeginTime) / 1000;
            return (int)((1.0 + (double)beginSecs / 60.0 * 0.075) * (double)factor);
        }

        // Token: 0x06000722 RID: 1826 RVA: 0x0006A8A4 File Offset: 0x00068AA4
        private void UpdateBuff4GameClient(GameClient client, BufferItemTypes bufferItem, int bufferGoodsID)
        {
            double[] actionParams = new double[]
            {
                (double)bufferGoodsID
            };
            Global.UpdateBufferData(client, bufferItem, actionParams, 1, false);
        }

        // Token: 0x06000723 RID: 1827 RVA: 0x0006A8CC File Offset: 0x00068ACC
        public bool IsQiZhiExtensionID(int QiZhiID)
        {
            return QiZhiID == this.RuntimeData.BattleQiZhiMonsterID1 || QiZhiID == this.RuntimeData.BattleQiZhiMonsterID2;
        }

        // Token: 0x06000724 RID: 1828 RVA: 0x0006A90C File Offset: 0x00068B0C
        public bool IsBangHuiMatchMap(int MapCode)
        {
            lock (this.RuntimeData.Mutex)
            {
                foreach (KeyValuePair<int, BHMatchConfig> item in this.RuntimeData.CommonConfigData.BHMatchConfigDict)
                {
                    if (item.Value.MapCode == MapCode || item.Value.MapCode_LongTa == MapCode)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Token: 0x06000725 RID: 1829 RVA: 0x0006A9DC File Offset: 0x00068BDC
        public void InstallJunQi(BangHuiMatchScene scene, CopyMap copyMap, GameClient client, BHMatchQiZhiConfig item)
        {
            GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.SceneInfo.MapCode);
            if (copyMap != null && null != gameMap)
            {
                item.Alive = true;
                item.BattleWhichSide = client.ClientData.BattleWhichSide;
                item.OwnTicks = TimeUtil.NOW();
                int BattleQiZhiMonsterID = 0;
                if (client.ClientData.BattleWhichSide == 1)
                {
                    BattleQiZhiMonsterID = this.RuntimeData.BattleQiZhiMonsterID1;
                }
                else if (client.ClientData.BattleWhichSide == 2)
                {
                    BattleQiZhiMonsterID = this.RuntimeData.BattleQiZhiMonsterID2;
                }
                GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, BattleQiZhiMonsterID, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, SceneUIClasses.BangHuiMatch, item, null);
            }
        }

        // Token: 0x06000726 RID: 1830 RVA: 0x0006AAC4 File Offset: 0x00068CC4
        public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
        {
            BHMatchQiZhiConfig item = null;
            bool isQiZuo = false;
            bool installJunQi = false;
            CopyMap copyMap = null;
            BangHuiMatchScene scene = client.SceneObject as BangHuiMatchScene;
            bool result;
            if (scene == null || !scene.CopyMapDict.TryGetValue(client.ClientData.MapCode, out copyMap))
            {
                result = isQiZuo;
            }
            else
            {
                lock (this.RuntimeData.Mutex)
                {
                    if (scene.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out item))
                    {
                        isQiZuo = true;
                        if (item.Alive)
                        {
                            return isQiZuo;
                        }
                        if (client.ClientData.BattleWhichSide != item.BattleWhichSide && Math.Abs(TimeUtil.NOW() - item.DeadTicks) < 3000L)
                        {
                            GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(12, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                        }
                        else if (Math.Abs(client.ClientData.PosX - item.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - item.PosY) <= 1000)
                        {
                            installJunQi = true;
                        }
                    }
                    if (installJunQi)
                    {
                        this.InstallJunQi(scene, copyMap, client, item);
                        this.UpdateQiZhiBangHuiOwnNum(scene);
                        foreach (CopyMap copy in scene.CopyMapDict.Values)
                        {
                            GameManager.ClientMgr.BroadSpecialCopyMapMessage<BangHuiMatchScoreData>(1173, scene.ScoreData, copy);
                        }
                    }
                }
                result = isQiZuo;
            }
            return result;
        }

        // Token: 0x04000BE6 RID: 3046
        public const SceneUIClasses ManagerType = SceneUIClasses.BangHuiMatch;

        // Token: 0x04000BE7 RID: 3047
        private static BangHuiMatchManager instance = new BangHuiMatchManager();

        // Token: 0x04000BE8 RID: 3048
        public BangHuiMatchData RuntimeData = new BangHuiMatchData();

        // Token: 0x04000BE9 RID: 3049
        public BHMatchSyncData BHMatchSyncDataCache = new BHMatchSyncData();

        // Token: 0x04000BEA RID: 3050
        private RoleDataEx OwnerRoleData = null;

        // Token: 0x04000BEB RID: 3051
        public ConcurrentDictionary<int, BangHuiMatchScene> SceneDict = new ConcurrentDictionary<int, BangHuiMatchScene>();

        // Token: 0x04000BEC RID: 3052
        private static long NextHeartBeatTicks = 0L;
    }
}
