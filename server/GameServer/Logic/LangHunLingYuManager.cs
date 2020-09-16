using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
    
    public class LangHunLingYuManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
    {
        
        public static LangHunLingYuManager getInstance()
        {
            return LangHunLingYuManager.instance;
        }

        
        public bool initialize()
        {
            return this.InitConfig();
        }

        
        public bool initialize(ICoreInterface coreInterface)
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(1153, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1154, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1156, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1155, 2, 2, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1157, 2, 2, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1158, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1160, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1161, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1162, 2, 2, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            GlobalEventSource.getInstance().registerListener(12, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(23, 10000, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(24, 10000, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(25, 10000, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(26, 10000, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10015, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10016, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10017, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10018, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(10019, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(30, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(33, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(29, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().registerListener(27, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource.getInstance().registerListener(11, LangHunLingYuManager.getInstance());
            GlobalEventSource.getInstance().registerListener(28, LangHunLingYuManager.getInstance());
            GlobalEventSource.getInstance().registerListener(14, LangHunLingYuManager.getInstance());
            return true;
        }

        
        public bool startup()
        {
            ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("LangHunLingYuManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 1428);
            return true;
        }

        
        public bool showdown()
        {
            GlobalEventSource.getInstance().removeListener(11, LangHunLingYuManager.getInstance());
            GlobalEventSource.getInstance().removeListener(12, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(23, 10000, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(24, 10000, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(25, 10000, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(26, 10000, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10001, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10015, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10016, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10017, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10018, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource4Scene.getInstance().removeListener(10019, 35, LangHunLingYuManager.getInstance());
            GlobalEventSource.getInstance().removeListener(28, LangHunLingYuManager.getInstance());
            GlobalEventSource.getInstance().removeListener(14, LangHunLingYuManager.getInstance());
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
            switch (nID)
            {
                case 1153:
                    return this.ProcessLangHunLingYuJoinCmd(client, nID, bytes, cmdParams);
                case 1154:
                    return this.ProcessLangHunLingYuRoleDataCmd(client, nID, bytes, cmdParams);
                case 1155:
                    return this.ProcessLangHunLingYuCityDataCmd(client, nID, bytes, cmdParams);
                case 1156:
                    return this.ProcessLangHunLingYuWorldDataCmd(client, nID, bytes, cmdParams);
                case 1157:
                    return this.ProcessLangHunLingYuEnterCmd(client, nID, bytes, cmdParams);
                case 1158:
                    return this.ProcessGetDailyAwardsCmd(client, nID, bytes, cmdParams);
                case 1160:
                    return this.ProcessGetAdmireDataCmd(client, nID, bytes, cmdParams);
                case 1161:
                    return this.ProcessGetAdmireHistoryCmd(client, nID, bytes, cmdParams);
                case 1162:
                    return this.ProcessAdmireCmd(client, nID, bytes, cmdParams);
            }
            return true;
        }

        
        public void processEvent(EventObject eventObject)
        {
            int nID = eventObject.getEventType();
            int num = nID;
            if (num != 11)
            {
                if (num != 14)
                {
                    if (num == 28)
                    {
                        OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
                        if (onStartPlayGameEventObject.Client.SceneType == 35)
                        {
                            YongZheZhanChangClient.getInstance().ChangeRoleState(onStartPlayGameEventObject.Client.ClientData.RoleID, KuaFuRoleStates.None, false);
                            this.OnStartPlayGame(onStartPlayGameEventObject.Client);
                        }
                    }
                }
                else
                {
                    PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
                    if (null != playerInitGameEventObject)
                    {
                        this.OnInitGame(playerInitGameEventObject.getPlayer());
                    }
                }
            }
            else
            {
                MonsterDeadEventObject e = eventObject as MonsterDeadEventObject;
                this.OnProcessJunQiDead(e.getAttacker(), e.getMonster());
            }
        }

        
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
                        if (null != e3)
                        {
                            if ((long)e3.Player.ClientData.Faction == this.RuntimeData.ChengHaoBHid && e3.TargetZhiWu == 1)
                            {
                                LogManager.WriteLog(LogTypes.Warning, string.Format("圣域城主禁止委任首领职务", new object[0]), null, true);
                                eventObject.Handled = true;
                                eventObject.Result = false;
                            }
                        }
                        break;
                    }
                case 26:
                    {
                        PostBangHuiChangeEventObject e4 = eventObject as PostBangHuiChangeEventObject;
                        if (e4 != null && null != e4.Player)
                        {
                            this.UpdateChengHaoBuffer(e4.Player, 0L, this.RuntimeData.ChengHaoBHid);
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
                            QiZhiConfig qiZhiConfig = e7.Monster.Tag as QiZhiConfig;
                            if (null != qiZhiConfig)
                            {
                                e7.Monster.MonsterName = qiZhiConfig.InstallBhName;
                                e7.Monster.Camp = qiZhiConfig.BattleWhichSide;
                                e7.Result = true;
                                e7.Handled = true;
                            }
                        }
                        break;
                    }
                case 33:
                    {
                        PreMonsterInjureEventObject e8 = eventObject as PreMonsterInjureEventObject;
                        if (null != e8)
                        {
                            lock (this.RuntimeData.Mutex)
                            {
                                if (this.RuntimeData.JunQiMonsterHashSet.Contains(e8.Monster.MonsterInfo.ExtensionID))
                                {
                                    e8.Injure = this.RuntimeData.CutLifeV;
                                    e8.Result = true;
                                    e8.Handled = true;
                                }
                            }
                        }
                        break;
                    }
                default:
                    if (num != 10001)
                    {
                        switch (num)
                        {
                            case 10015:
                                {
                                    NotifyLhlyBangHuiDataGameEvent e9 = eventObject as NotifyLhlyBangHuiDataGameEvent;
                                    if (null != e9)
                                    {
                                        this.UpdateBangHuiDataEx(e9.Arg as LangHunLingYuBangHuiDataEx);
                                        e9.Handled = (e9.Result = true);
                                    }
                                    break;
                                }
                            case 10016:
                                {
                                    NotifyLhlyCityDataGameEvent e10 = eventObject as NotifyLhlyCityDataGameEvent;
                                    if (null != e10)
                                    {
                                        this.UpdateCityDataEx(e10.Arg as LangHunLingYuCityDataEx);
                                        e10.Handled = (e10.Result = true);
                                    }
                                    break;
                                }
                            case 10017:
                                {
                                    NotifyLhlyOtherCityListGameEvent e11 = eventObject as NotifyLhlyOtherCityListGameEvent;
                                    if (null != e11)
                                    {
                                        this.UpdateOtherCityList(e11.Arg);
                                        e11.Handled = (e11.Result = true);
                                    }
                                    break;
                                }
                            case 10018:
                                {
                                    NotifyLhlyCityOwnerHistGameEvent e12 = eventObject as NotifyLhlyCityOwnerHistGameEvent;
                                    if (null != e12)
                                    {
                                        this.UpdateCityOwnerHist(e12.Arg);
                                        e12.Handled = (e12.Result = true);
                                    }
                                    break;
                                }
                            case 10019:
                                {
                                    NotifyLhlyCityOwnerAdmireGameEvent e13 = eventObject as NotifyLhlyCityOwnerAdmireGameEvent;
                                    if (null != e13)
                                    {
                                        this.UpdateCityOwnerAdmire(e13.RoleID, e13.AdmireCount);
                                        e13.Handled = (e13.Result = true);
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        KuaFuNotifyEnterGameEvent e14 = eventObject as KuaFuNotifyEnterGameEvent;
                        if (null != e14)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("通知角色ID={0}拥有进入勇者战场资格,跨服GameID={1}", 0, 0), null, true);
                        }
                    }
                    break;
            }
        }

        
        public bool InitConfig()
        {
            bool success = true;
            string fileName = "";
            lock (this.RuntimeData.Mutex)
            {
                try
                {
                    this.RuntimeData.CutLifeV = (int)GameManager.systemParamsList.GetParamValueIntByName("CutLifeV", 10);
                    this.RuntimeData.CityLevelInfoDict.Clear();
                    fileName = "Config/City.xml";
                    string fullPathFileName = Global.GameResPath(fileName);
                    XElement xml = XElement.Load(fullPathFileName);
                    IEnumerable<XElement> nodes = xml.Elements();
                    int cityId = 0;
                    foreach (XElement node in nodes)
                    {
                        CityLevelInfo item = new CityLevelInfo();
                        item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item.CityLevel = (int)Global.GetSafeAttributeLong(node, "CityLevel");
                        item.CityNum = (int)Global.GetSafeAttributeLong(node, "CityNum");
                        item.MaxNum = (int)Global.GetSafeAttributeLong(node, "MaxNum");
                        item.ZhanMengZiJin = (int)Global.GetSafeAttributeLong(node, "ZhanMengZiJin");
                        item.AttackWeekDay = Global.StringToIntList(Global.GetSafeAttributeStr(node, "AttackWeekDay"), ',');
                        ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "Award"), ref item.Award, '|', ',');
                        ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "DayAward"), ref item.DayAward, '|', ',');
                        if (!ConfigParser.ParserTimeRangeListWithDay(item.BaoMingTime, Global.GetSafeAttributeStr(node, "BaoMingTime").Replace(';', '|'), true, '|', '-', ','))
                        {
                            LogManager.WriteLog(LogTypes.Fatal, string.Format("解析文件{0}的BaoMingTime出错", fileName), null, true);
                            return false;
                        }
                        if (!ConfigParser.ParserTimeRangeList(item.AttackTime, Global.GetSafeAttributeStr(node, "AttackTime"), true, '|', '-'))
                        {
                            LogManager.WriteLog(LogTypes.Fatal, string.Format("解析文件{0}的BaoMingTime出错", fileName), null, true);
                            return false;
                        }
                        this.RuntimeData.CityLevelInfoDict[item.CityLevel] = item;
                        for (int i = 0; i < item.CityNum; i++)
                        {
                            cityId++;
                            this.RuntimeData.CityDataExDict.Add(cityId, new LangHunLingYuCityDataEx
                            {
                                CityId = cityId,
                                CityLevel = item.CityLevel
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
                    return false;
                }
                try
                {
                    this.RuntimeData.MapBirthPointListDict.Clear();
                    fileName = "Config/SiegeWarfareBirthPoint.xml";
                    string fullPathFileName = Global.GameResPath(fileName);
                    XElement xml = XElement.Load(fullPathFileName);
                    IEnumerable<XElement> nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        MapBirthPoint item2 = new MapBirthPoint();
                        item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                        item2.Type = (int)Global.GetSafeAttributeLong(node, "Type");
                        item2.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
                        item2.BirthPosX = (int)Global.GetSafeAttributeLong(node, "BirthPosX");
                        item2.BirthPosY = (int)Global.GetSafeAttributeLong(node, "BirthPosY");
                        item2.BirthRangeX = (int)Global.GetSafeAttributeLong(node, "BirthRangeX");
                        item2.BirthRangeY = (int)Global.GetSafeAttributeLong(node, "BirthRangeY");
                        List<MapBirthPoint> list;
                        if (!this.RuntimeData.MapBirthPointListDict.TryGetValue(item2.Type, out list))
                        {
                            list = new List<MapBirthPoint>();
                            this.RuntimeData.MapBirthPointListDict.Add(item2.Type, list);
                        }
                        list.Add(item2);
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
                    return false;
                }
                try
                {
                    this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
                    this.RuntimeData.QiZhiBuffOwnerDataList.Clear();
                    this.RuntimeData.QiZhiBuffDisableParamsDict.Clear();
                    this.RuntimeData.QiZhiBuffEnableParamsDict.Clear();
                    fileName = "Config/CityWarQiZuo.xml";
                    string fullPathFileName = Global.GameResPath(fileName);
                    XElement xml = XElement.Load(fullPathFileName);
                    IEnumerable<XElement> nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        QiZhiConfig item3 = new QiZhiConfig();
                        item3.NPCID = (int)Global.GetSafeAttributeLong(node, "NPCID");
                        item3.BufferID = (int)Global.GetSafeAttributeLong(node, "BufferID");
                        item3.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
                        item3.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
                        item3.MonsterId = (int)Global.GetSafeAttributeLong(node, "JuQiID");
                        this.RuntimeData.JunQiMonsterHashSet.Add(item3.MonsterId);
                        this.RuntimeData.NPCID2QiZhiConfigDict[item3.NPCID] = item3;
                        this.RuntimeData.QiZhiBuffOwnerDataList.Add(new LangHunLingYuQiZhiBuffOwnerData
                        {
                            NPCID = item3.NPCID,
                            OwnerBHName = ""
                        });
                        this.RuntimeData.QiZhiBuffDisableParamsDict[item3.BufferID] = new double[]
                        {
                            0.0,
                            (double)item3.BufferID
                        };
                        this.RuntimeData.QiZhiBuffEnableParamsDict[item3.BufferID] = new double[]
                        {
                            0.0,
                            (double)item3.BufferID
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
                    return false;
                }
                try
                {
                    QiZhiConfig qiZhiConfig;
                    if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(this.RuntimeData.SuperQiZhiNpcId, out qiZhiConfig))
                    {
                        this.RuntimeData.SuperQiZhiOwnerBirthPosX = qiZhiConfig.PosX;
                        this.RuntimeData.SuperQiZhiOwnerBirthPosY = qiZhiConfig.PosY;
                    }
                    this.RuntimeData.SceneDataDict.Clear();
                    this.RuntimeData.LevelRangeSceneIdDict.Clear();
                    fileName = "Config/CityWar.xml";
                    string fullPathFileName = Global.GameResPath(fileName);
                    XElement xml = XElement.Load(fullPathFileName);
                    IEnumerable<XElement> nodes = xml.Elements();
                    foreach (XElement node in nodes)
                    {
                        LangHunLingYuSceneInfo sceneItem = new LangHunLingYuSceneInfo();
                        int id = (int)Global.GetSafeAttributeLong(node, "ID");
                        sceneItem.Id = id;
                        sceneItem.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode1");
                        sceneItem.MapCode_LongTa = (int)Global.GetSafeAttributeLong(node, "MapCode2");
                        sceneItem.MinLevel = (int)Global.GetSafeAttributeLong(node, "MinLevel");
                        sceneItem.MaxLevel = 10000;
                        sceneItem.MinZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MinZhuanSheng");
                        sceneItem.MaxZhuanSheng = 10000;
                        sceneItem.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
                        sceneItem.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(node, "WaitingEnterSecs");
                        sceneItem.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
                        sceneItem.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
                        GameMap gameMap = null;
                        if (!GameManager.MapMgr.DictMaps.TryGetValue(sceneItem.MapCode, out gameMap))
                        {
                            success = false;
                            LogManager.WriteLog(LogTypes.Fatal, string.Format("地图配置中缺少{0}所需的地图:{1}", fileName, sceneItem.MapCode), null, true);
                        }
                        if (!GameManager.MapMgr.DictMaps.TryGetValue(sceneItem.MapCode_LongTa, out gameMap))
                        {
                            success = false;
                            LogManager.WriteLog(LogTypes.Fatal, string.Format("地图配置中缺少{0}所需的地图:{1}", fileName, sceneItem.MapCode_LongTa), null, true);
                        }
                        RangeKey range = new RangeKey(Global.GetUnionLevel(sceneItem.MinZhuanSheng, sceneItem.MinLevel, false), Global.GetUnionLevel(sceneItem.MaxZhuanSheng, sceneItem.MaxLevel, false), null);
                        this.RuntimeData.LevelRangeSceneIdDict[range] = sceneItem;
                        this.RuntimeData.SceneDataDict[id] = sceneItem;
                        this.RuntimeData.SceneInfoId = id;
                        this.RuntimeData.SceneDataList.Add(sceneItem);
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
                }
                try
                {
                    fileName = "Config/SiegeWarfareExp.xml";
                    this._LevelAwardsMgr.LoadFromXMlFile(fileName, "", "ID", 0);
                }
                catch (Exception ex)
                {
                    LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
                    return false;
                }
            }
            this.RestoreLangHunLingYuNpc();
            return success;
        }

        
        public int GetCityLevelById(int cityId)
        {
            lock (this.RuntimeData.Mutex)
            {
                LangHunLingYuCityDataEx cityDataEx;
                if (this.RuntimeData.CityDataExDict.TryGetValue(cityId, out cityDataEx))
                {
                    return cityDataEx.CityLevel;
                }
            }
            return 0;
        }

        
        public int GetBangHuiCityLevel(int bhid)
        {
            lock (this.RuntimeData.Mutex)
            {
                LangHunLingYuBangHuiDataEx bangHuiData;
                if (this.RuntimeData.BangHuiDataExDict.TryGetValue((long)bhid, out bangHuiData))
                {
                    return bangHuiData.Level;
                }
            }
            return 0;
        }

        
        public string GetBangHuiName(int bhid, out int zoneId)
        {
            zoneId = 0;
            LangHunLingYuBangHuiDataEx bangHuiDataEx;
            string result;
            if (!this.RuntimeData.BangHuiDataExDict.TryGetValue((long)bhid, out bangHuiDataEx))
            {
                result = GLang.GetLang(6, new object[0]);
            }
            else
            {
                zoneId = bangHuiDataEx.ZoneId;
                result = bangHuiDataEx.BhName;
            }
            return result;
        }

        
        private void UpdateCityDataEx(LangHunLingYuCityDataEx cityDataEx)
        {
            if (null != cityDataEx)
            {
                lock (this.RuntimeData.Mutex)
                {
                    HashSet<long> AddBangHuiIdHashSet = new HashSet<long>();
                    HashSet<long> removedBangHuiIdHashSet = new HashSet<long>();
                    HashSet<long> oldBangHuiAttackerIdHashSet = new HashSet<long>();
                    HashSet<long> newBangHuiAttackerIdHashSet = new HashSet<long>();
                    HashSet<long> allBangHuiIdHashSet = new HashSet<long>();
                    AddBangHuiIdHashSet = new HashSet<long>(cityDataEx.Site);
                    allBangHuiIdHashSet.UnionWith(cityDataEx.Site);
                    LangHunLingYuCityDataEx oldCityDataEx;
                    if (this.RuntimeData.CityDataExDict.TryGetValue(cityDataEx.CityId, out oldCityDataEx))
                    {
                        AddBangHuiIdHashSet.ExceptWith(oldCityDataEx.Site);
                        removedBangHuiIdHashSet = new HashSet<long>(oldCityDataEx.Site);
                        removedBangHuiIdHashSet.ExceptWith(cityDataEx.Site);
                        for (int i = 1; i < oldCityDataEx.Site.Length; i++)
                        {
                            long bhid = oldCityDataEx.Site[i];
                            if (bhid > 0L && !oldBangHuiAttackerIdHashSet.Contains(bhid))
                            {
                                oldBangHuiAttackerIdHashSet.Add(bhid);
                            }
                        }
                        allBangHuiIdHashSet.UnionWith(oldCityDataEx.Site);
                    }
                    for (int i = 1; i < cityDataEx.Site.Length; i++)
                    {
                        long bhid = cityDataEx.Site[i];
                        if (bhid > 0L && !newBangHuiAttackerIdHashSet.Contains(bhid))
                        {
                            newBangHuiAttackerIdHashSet.Add(bhid);
                        }
                    }
                    this.RuntimeData.CityDataExDict[cityDataEx.CityId] = cityDataEx;
                    LangHunLingYuCityData cityData;
                    if (!this.RuntimeData.CityDataDict.TryGetValue(cityDataEx.CityId, out cityData))
                    {
                        cityData = new LangHunLingYuCityData();
                        cityData.CityId = cityDataEx.CityId;
                        cityData.CityLevel = cityDataEx.CityLevel;
                        this.RuntimeData.CityDataDict[cityDataEx.CityId] = cityData;
                    }
                    LangHunLingYuBangHuiDataEx bangHuiDataEx;
                    if (this.RuntimeData.BangHuiDataExDict.TryGetValue(cityDataEx.Site[0], out bangHuiDataEx))
                    {
                        cityData.Owner = new BangHuiMiniData
                        {
                            BHID = bangHuiDataEx.Bhid,
                            BHName = bangHuiDataEx.BhName,
                            ZoneID = bangHuiDataEx.ZoneId
                        };
                    }
                    else
                    {
                        cityData.Owner = null;
                    }
                    cityData.AttackerList.Clear();
                    for (int i = 1; i < cityDataEx.Site.Length; i++)
                    {
                        long id = cityDataEx.Site[i];
                        if (id > 0L && this.RuntimeData.BangHuiDataExDict.TryGetValue(id, out bangHuiDataEx))
                        {
                            cityData.AttackerList.Add(new BangHuiMiniData
                            {
                                BHID = bangHuiDataEx.Bhid,
                                BHName = bangHuiDataEx.BhName,
                                ZoneID = bangHuiDataEx.ZoneId
                            });
                        }
                    }
                    foreach (long id in AddBangHuiIdHashSet)
                    {
                        LangHunLingYuBangHuiData bangHuiData;
                        if (!this.RuntimeData.BangHuiDataDict.TryGetValue(id, out bangHuiData))
                        {
                            bangHuiData = new LangHunLingYuBangHuiData();
                            this.RuntimeData.BangHuiDataDict[id] = bangHuiData;
                        }
                        bangHuiData.SelfCityList.Add(cityDataEx.CityId);
                        bangHuiData.SelfCityList.Sort();
                    }
                    foreach (long id in removedBangHuiIdHashSet)
                    {
                        LangHunLingYuBangHuiData bangHuiData;
                        if (!this.RuntimeData.BangHuiDataDict.TryGetValue(id, out bangHuiData))
                        {
                            bangHuiData = new LangHunLingYuBangHuiData();
                            this.RuntimeData.BangHuiDataDict[id] = bangHuiData;
                        }
                        bangHuiData.SelfCityList.Remove(cityDataEx.CityId);
                    }
                    foreach (long id in newBangHuiAttackerIdHashSet.Except(oldBangHuiAttackerIdHashSet))
                    {
                        LangHunLingYuBangHuiData bangHuiData;
                        if (this.RuntimeData.BangHuiDataDict.TryGetValue(id, out bangHuiData))
                        {
                            bangHuiData.SignUpState = 1;
                        }
                    }
                    foreach (long id in oldBangHuiAttackerIdHashSet.Except(newBangHuiAttackerIdHashSet))
                    {
                        LangHunLingYuBangHuiData bangHuiData;
                        if (this.RuntimeData.BangHuiDataDict.TryGetValue(id, out bangHuiData))
                        {
                            bangHuiData.SignUpState = 0;
                        }
                    }
                    long chengHaoBhid = 0L;
                    if (this.RuntimeData.CityDataExDict.TryGetValue(1, out cityDataEx))
                    {
                        chengHaoBhid = cityDataEx.Site[0];
                        this.ReplaceLangHunLingYuNpc();
                    }
                    foreach (long id in allBangHuiIdHashSet)
                    {
                        LangHunLingYuBangHuiData bangHuiData;
                        if (this.RuntimeData.BangHuiDataDict.TryGetValue(id, out bangHuiData))
                        {
                            bangHuiData.DayAwardFlags = 0;
                            foreach (int cityId in bangHuiData.SelfCityList)
                            {
                                if (this.RuntimeData.CityDataExDict.TryGetValue(cityId, out cityDataEx))
                                {
                                    if (cityDataEx.Site[0] == id)
                                    {
                                        bangHuiData.DayAwardFlags = Global.SetIntSomeBit(cityDataEx.CityLevel, bangHuiData.DayAwardFlags, true);
                                    }
                                }
                            }
                        }
                    }
                    this.BroadcastBangHuiCityData(allBangHuiIdHashSet, this.RuntimeData.ChengHaoBHid, chengHaoBhid);
                    this.RuntimeData.ChengHaoBHid = chengHaoBhid;
                }
            }
        }

        
        private void UpdateBangHuiDataEx(LangHunLingYuBangHuiDataEx bangHuiDataEx)
        {
            if (null != bangHuiDataEx)
            {
                lock (this.RuntimeData.Mutex)
                {
                    this.RuntimeData.BangHuiDataExDict[(long)bangHuiDataEx.Bhid] = bangHuiDataEx;
                    LangHunLingYuBangHuiData bangHuiData;
                    if (!this.RuntimeData.BangHuiDataDict.TryGetValue((long)bangHuiDataEx.Bhid, out bangHuiData))
                    {
                        bangHuiData = new LangHunLingYuBangHuiData();
                        this.RuntimeData.BangHuiDataDict[(long)bangHuiDataEx.Bhid] = bangHuiData;
                    }
                }
            }
        }

        
        private void UpdateOtherCityList(Dictionary<int, List<int>> list)
        {
            lock (this.RuntimeData.Mutex)
            {
                this.RuntimeData.OtherCityList = list;
            }
        }

        
        private void UpdateCityOwnerAdmire(int rid, int admirecount)
        {
            lock (this.RuntimeData.Mutex)
            {
                if (null != this.RuntimeData.OwnerHistList)
                {
                    foreach (LangHunLingYuKingHist data in this.RuntimeData.OwnerHistList)
                    {
                        if (data.rid == rid)
                        {
                            data.AdmireCount = admirecount;
                        }
                    }
                }
            }
        }

        
        private void UpdateCityOwnerHist(List<LangHunLingYuKingHist> list)
        {
            lock (this.RuntimeData.Mutex)
            {
                this.RuntimeData.OwnerHistList = list;
                this.ReplaceLangHunLingYuNpc();
            }
        }

        
        private void BroadcastBangHuiCityData(HashSet<long> newBangHuiIdHashSet, long oldBhid, long newBhid)
        {
            int count = GameManager.ClientMgr.GetMaxClientCount();
            for (int i = 0; i < count; i++)
            {
                GameClient client = GameManager.ClientMgr.FindClientByNid(i);
                if (null != client)
                {
                    if (client.ClientData.Faction > 0 && newBangHuiIdHashSet.Contains((long)client.ClientData.Faction))
                    {
                        this.ProcessLangHunLingYuRoleDataCmd(client, 1154, null, null);
                    }
                    if (oldBhid != newBhid)
                    {
                        this.UpdateChengHaoBuffer(client, oldBhid, newBhid);
                    }
                }
            }
        }

        
        private void OnInitGame(GameClient client)
        {
            this.UpdateChengHaoBuffer(client, 0L, this.RuntimeData.ChengHaoBHid);
        }

        
        private void UpdateChengHaoBuffer(GameClient client, long oldBhid, long newBhid)
        {
            if (newBhid > 0L && (long)client.ClientData.Faction == newBhid)
            {
                double[] bufferParams = new double[]
                {
                    1.0
                };
                if (client.ClientData.BHZhiWu == 1)
                {
                    Global.UpdateBufferData(client, BufferItemTypes.ShengYuChengZhu_Title, bufferParams, 1, true);
                }
                else
                {
                    Global.UpdateBufferData(client, BufferItemTypes.LangHunLingYu_ChengHao, bufferParams, 1, true);
                }
            }
            else
            {
                double[] array = new double[1];
                double[] bufferParams = array;
                Global.UpdateBufferData(client, BufferItemTypes.ShengYuChengZhu_Title, bufferParams, 1, true);
                Global.UpdateBufferData(client, BufferItemTypes.LangHunLingYu_ChengHao, bufferParams, 1, true);
            }
        }

        
        public void OnLogin(GameClient client)
        {
            this.UpdateChengHaoBuffer(client, 0L, this.RuntimeData.ChengHaoBHid);
        }

        
        public bool CanGetAwardsByEnterTime(GameClient client)
        {
            int secs = DataHelper.UnixSecondsNow() - Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
            return (long)secs >= GameManager.systemParamsList.GetParamValueIntByName("JiaRuTime", 0) * 60L * 60L;
        }

        
        public void CheckTipsIconState(GameClient client)
        {
            int awardFlags = 0;
            bool canGetAwards = false;
            bool canFight = false;
            lock (this.RuntimeData.Mutex)
            {
                canGetAwards = this.CanGetAwardsByEnterTime(client);
                if (canGetAwards)
                {
                    LangHunLingYuBangHuiData langHunLingYuBangHuiData;
                    if (this.RuntimeData.BangHuiDataDict.TryGetValue((long)client.ClientData.Faction, out langHunLingYuBangHuiData))
                    {
                        awardFlags = langHunLingYuBangHuiData.DayAwardFlags;
                    }
                    int nowDayId = Global.GetOffsetDayNow();
                    int lastDayID = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwards");
                    int flags = 0;
                    if (lastDayID == nowDayId)
                    {
                        flags = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwardsFlags");
                    }
                    int flags2 = (awardFlags ^ flags) & awardFlags;
                    if (flags2 == 0)
                    {
                        canGetAwards = false;
                    }
                }
                LangHunLingYuBangHuiData bangHuiData;
                if (this.RuntimeData.BangHuiDataDict.TryGetValue((long)client.ClientData.Faction, out bangHuiData))
                {
                    if (bangHuiData.SelfCityList.Count > 0)
                    {
                        canFight = true;
                    }
                }
            }
            client._IconStateMgr.AddFlushIconState(15002, canGetAwards);
            client._IconStateMgr.AddFlushIconState(15003, canFight);
            client._IconStateMgr.SendIconStateToClient(client);
        }

        
        public bool ProcessLangHunLingYuJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                int bhid = client.ClientData.Faction;
                if (bhid <= 0)
                {
                    result = -1000;
                }
                else if (client.ClientData.BHZhiWu != 1)
                {
                    result = -1002;
                }
                else if (!this.IsGongNengOpened(client, true))
                {
                    result = -13;
                }
                else
                {
                    BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bhid, 0);
                    if (null == bangHuiMiniData)
                    {
                        result = -1001;
                    }
                    else
                    {
                        int bhZoneID = 0;
                        CityLevelInfo sceneItem = null;
                        LangHunLingYuGameStates state = LangHunLingYuGameStates.None;
                        int cityLevel = this.GetBangHuiCityLevel(bhid) + 1;
                        if (cityLevel > 10)
                        {
                            result = -4004;
                        }
                        else
                        {
                            result = this.CheckSignUpCondition(cityLevel, ref sceneItem, ref state);
                            if (state != LangHunLingYuGameStates.SignUp)
                            {
                                result = -2001;
                            }
                            else
                            {
                                LangHunLingYuBangHuiData bangHuiData = null;
                                lock (this.RuntimeData.Mutex)
                                {
                                    if (this.RuntimeData.BangHuiDataDict.TryGetValue((long)bhid, out bangHuiData))
                                    {
                                        if (null != bangHuiData.SelfCityList)
                                        {
                                            foreach (int cityID in bangHuiData.SelfCityList)
                                            {
                                                LangHunLingYuCityDataEx cityDataEx;
                                                if (this.RuntimeData.CityDataExDict.TryGetValue(cityID, out cityDataEx) && cityDataEx.CityLevel >= cityLevel)
                                                {
                                                    result = -1004;
                                                    break;
                                                }
                                            }
                                        }
                                        if (bangHuiData.SignUpTime > TimeUtil.NOW() - 10000L)
                                        {
                                            result = -2005;
                                            goto IL_2E3;
                                        }
                                    }
                                    else
                                    {
                                        bangHuiData = new LangHunLingYuBangHuiData();
                                        this.RuntimeData.BangHuiDataDict.Add((long)bhid, bangHuiData);
                                    }
                                    if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, sceneItem.ZhanMengZiJin, out bhZoneID))
                                    {
                                        result = -9;
                                    }
                                    else
                                    {
                                        bangHuiData.SignUpTime = TimeUtil.NOW();
                                        result = YongZheZhanChangClient.getInstance().LangHunLingYuSignUp(bangHuiMiniData.BHName, bangHuiMiniData.BHID, bangHuiMiniData.ZoneID, 10, 1, 0);
                                        if (result >= 0)
                                        {
                                            EventLogManager.AddGameEvent(LogRecordType.LangHunLingYu, new object[]
                                            {
                                                32,
                                                bangHuiMiniData.BHID,
                                                sceneItem.ZhanMengZiJin
                                            });
                                        }
                                        else
                                        {
                                            GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bhid, sceneItem.ZhanMengZiJin);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                IL_2E3:
                client.sendCmd<int>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        public bool ProcessLangHunLingYuEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                CityLevelInfo sceneItem = null;
                LangHunLingYuGameStates state = LangHunLingYuGameStates.None;
                int result = 0;
                int roleId = client.ClientData.RoleID;
                int cityId = Global.SafeConvertToInt32(cmdParams[1]);
                int bhid = client.ClientData.Faction;
                bool guanZhanGM = VideoLogic.getInstance().IsGuanZhanGM(client);
                if (cityId < 1 || cityId > 1023)
                {
                    result = -18;
                }
                else
                {
                    int uniolLevel = Global.GetUnionLevel(client, false);
                    if (uniolLevel < Global.GetUnionLevel(this.RuntimeData.MinZhuanSheng, this.RuntimeData.MinLevel, false))
                    {
                        result = -19;
                    }
                    else if (!this.IsGongNengOpened(client, true))
                    {
                        result = -12;
                    }
                    else if (bhid <= 0 && !guanZhanGM)
                    {
                        result = -1000;
                    }
                    else
                    {
                        bool canEnter = false;
                        bool gameOver = true;
                        LangHunLingYuCityDataEx cityData;
                        lock (this.RuntimeData.Mutex)
                        {
                            if (this.RuntimeData.CityDataExDict.TryGetValue(cityId, out cityData))
                            {
                                if (guanZhanGM)
                                {
                                    canEnter = true;
                                    gameOver = false;
                                }
                                else
                                {
                                    for (int i = 0; i < cityData.Site.Length; i++)
                                    {
                                        long id = cityData.Site[i];
                                        if (id == (long)bhid)
                                        {
                                            canEnter = true;
                                        }
                                        if (i > 0 && id > 0L)
                                        {
                                            gameOver = false;
                                        }
                                    }
                                }
                            }
                        }
                        if (!canEnter)
                        {
                            result = -1003;
                        }
                        else if (gameOver)
                        {
                            result = -4006;
                        }
                        else if (!this.CheckMap(client))
                        {
                            result = -21;
                        }
                        else
                        {
                            result = this.CheckFightCondition(this.GetCityLevelById(cityId), ref sceneItem, ref state);
                            if (result >= 0 && state == LangHunLingYuGameStates.Start)
                            {
                                lock (this.RuntimeData.Mutex)
                                {
                                    KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
                                    if (null != clientKuaFuServerLoginData)
                                    {
                                        if (YongZheZhanChangClient.getInstance().LangHunLingYuKuaFuLoginData(roleId, cityId, cityData.GameId, clientKuaFuServerLoginData))
                                        {
                                            result = 0;
                                        }
                                        else
                                        {
                                            result = -11000;
                                        }
                                    }
                                }
                                if (result >= 0)
                                {
                                    if (result >= 0)
                                    {
                                        GlobalNew.RecordSwitchKuaFuServerLog(client);
                                        client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
                                        EventLogManager.AddRoleEvent(client, OpTypes.Enter, OpTags.LangHunLingYu, LogRecordType.IntValue, new object[]
                                        {
                                            cityId
                                        });
                                    }
                                    else
                                    {
                                        Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
                                    }
                                }
                            }
                            else
                            {
                                result = -2001;
                            }
                        }
                    }
                }
                client.sendCmd<int>(nID, result, false);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return true;
        }

        
        public bool ProcessLangHunLingYuRoleDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleId = client.ClientData.RoleID;
                int bhid = client.ClientData.Faction;
                LangHunLingYuRoleData roleData = new LangHunLingYuRoleData();
                if (bhid > 0)
                {
                    LangHunLingYuBangHuiData bangHuiData = null;
                    lock (this.RuntimeData.Mutex)
                    {
                        if (!this.RuntimeData.BangHuiDataDict.TryGetValue((long)bhid, out bangHuiData))
                        {
                            LangHunLingYuCityData cityData;
                            if (this.RuntimeData.CityDataDict.TryGetValue(1, out cityData))
                            {
                                roleData.SelfCityList.Insert(0, cityData);
                            }
                            goto IL_30B;
                        }
                        int maxLevel = 0;
                        bool findTopLevelCityId = false;
                        roleData.SignUpState = bangHuiData.SignUpState;
                        foreach (int id in bangHuiData.SelfCityList)
                        {
                            LangHunLingYuCityData cityData;
                            if (id > 0 && this.RuntimeData.CityDataDict.TryGetValue(id, out cityData))
                            {
                                if (id == 1)
                                {
                                    findTopLevelCityId = true;
                                }
                                roleData.SelfCityList.Add(cityData);
                                if (cityData.Owner != null && cityData.Owner.BHID == bhid && cityData.CityLevel > maxLevel)
                                {
                                    maxLevel = cityData.CityLevel;
                                }
                            }
                        }
                        if (!findTopLevelCityId)
                        {
                            LangHunLingYuCityData cityData;
                            if (this.RuntimeData.CityDataDict.TryGetValue(1, out cityData))
                            {
                                roleData.SelfCityList.Insert(0, cityData);
                            }
                        }
                    }
                    int lastDayID = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwards");
                    int flags = 0;
                    int nowDayId = Global.GetOffsetDayNow();
                    if (lastDayID == nowDayId)
                    {
                        flags = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwardsFlags");
                    }
                    List<int> levelList = new List<int>();
                    lock (this.RuntimeData.Mutex)
                    {
                        if (null != bangHuiData.SelfCityList)
                        {
                            foreach (int id in bangHuiData.SelfCityList)
                            {
                                LangHunLingYuCityDataEx cityDataEx;
                                if (this.RuntimeData.CityDataExDict.TryGetValue(id, out cityDataEx))
                                {
                                    if (cityDataEx.Site[0] == (long)bhid)
                                    {
                                        if (0 == Global.GetIntSomeBit(flags, cityDataEx.CityLevel))
                                        {
                                            levelList.Add(cityDataEx.CityLevel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    roleData.GetDayAwardsState = new List<int>(levelList);
                }
                IL_30B:
                client.sendCmd<LangHunLingYuRoleData>(nID, roleData, false);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return true;
        }

        
        public bool ProcessLangHunLingYuCityDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleId = client.ClientData.RoleID;
                int bhid = client.ClientData.Faction;
                int cityId = Global.SafeConvertToInt32(cmdParams[1]);
                LangHunLingYuCityData cityData = null;
                if ((bhid > 0 && cityId >= 1) || cityId <= 1023)
                {
                    lock (this.RuntimeData.Mutex)
                    {
                        if (this.RuntimeData.CityDataDict.TryGetValue(bhid, out cityData))
                        {
                        }
                    }
                }
                client.sendCmd<LangHunLingYuCityData>(nID, cityData, false);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return true;
        }

        
        public bool ProcessGetAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleID = Convert.ToInt32(cmdParams[0]);
                client.sendCmd<LangHunLingYuKingShowData>(nID, new LangHunLingYuKingShowData
                {
                    AdmireCount = Global.GetLHLYAdmireCount(client),
                    RoleData4Selector = Global.RoleDataEx2RoleData4Selector(this.OwnerRoleData)
                }, false);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return true;
        }

        
        public bool ProcessGetAdmireHistoryCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleID = Convert.ToInt32(cmdParams[0]);
                List<LangHunLingYuKingShowDataHist> showDataList = new List<LangHunLingYuKingShowDataHist>();
                lock (this.RuntimeData.Mutex)
                {
                    if (this.RuntimeData.OwnerHistList != null && this.RuntimeData.OwnerHistList.Count > 1)
                    {
                        for (int index = this.RuntimeData.OwnerHistList.Count - 1; index >= 0; index--)
                        {
                            LangHunLingYuKingHist histData = this.RuntimeData.OwnerHistList[index];
                            if (null != histData.CityOwnerRoleData)
                            {
                                RoleDataEx rd = DataHelper.BytesToObject<RoleDataEx>(histData.CityOwnerRoleData, 0, histData.CityOwnerRoleData.Length);
                                if (null != rd)
                                {
                                    if (index != this.RuntimeData.OwnerHistList.Count - 1 || this.RuntimeData.ChengHaoBHid == 0L)
                                    {
                                        showDataList.Add(new LangHunLingYuKingShowDataHist
                                        {
                                            AdmireCount = histData.AdmireCount,
                                            CompleteTime = histData.CompleteTime,
                                            RoleData4Selector = Global.RoleDataEx2RoleData4Selector(rd),
                                            BHName = rd.BHName
                                        });
                                        if (showDataList.Count == 9)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    client.sendCmd<List<LangHunLingYuKingShowDataHist>>(nID, showDataList, false);
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return true;
        }

        
        public bool ProcessAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleID = Convert.ToInt32(cmdParams[0]);
                int type = Convert.ToInt32(cmdParams[1]);
                MoBaiData MoBaiConfig = null;
                string strcmd;
                if (!Data.MoBaiDataInfoList.TryGetValue(1, out MoBaiConfig))
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
                int AdmireCount = Global.GetLHLYAdmireCount(client);
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
                    if (!Global.SubBindTongQianAndTongQian(client, MoBaiConfig.NeedJinBi, "膜拜圣域城主"))
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
                        GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.JinBiZhanGongAward, AddBangGongTypes.LHLYMoBai, 0);
                    }
                    if (MoBaiConfig.LingJingAwardByJinBi > 0)
                    {
                        GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByJinBi, "膜拜圣域城主", true, true, false);
                    }
                }
                else if (type == 2)
                {
                    if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MoBaiConfig.NeedZuanShi, "膜拜圣域城主", true, true, false, DaiBiSySType.None))
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
                        GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.ZuanShiZhanGongAward, AddBangGongTypes.LHLYMoBai, 0);
                    }
                    if (MoBaiConfig.LingJingAwardByZuanShi > 0)
                    {
                        GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByZuanShi, "膜拜圣域城主", true, true, false);
                    }
                }
                if (null != this.OwnerRoleData)
                {
                    YongZheZhanChangClient.getInstance().LangHunLingYunAdmire(this.OwnerRoleData.RoleID);
                }
                Global.ProcessIncreaseLHLYAdmireCount(client);
                strcmd = string.Format("{0}", 1);
                client.sendCmd(nID, strcmd, false);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return true;
        }

        
        public bool ProcessLangHunLingYuWorldDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleId = client.ClientData.RoleID;
                int bhid = client.ClientData.Faction;
                LangHunLingYuWorldData worldData = new LangHunLingYuWorldData();
                if (bhid > 0 || VideoLogic.getInstance().IsGuanZhanGM(client))
                {
                    lock (this.RuntimeData.Mutex)
                    {
                        for (int i = 1; i <= 31; i++)
                        {
                            LangHunLingYuCityData cityData;
                            if (this.RuntimeData.CityDataDict.TryGetValue(i, out cityData))
                            {
                                worldData.CityList.Add(cityData);
                            }
                        }
                    }
                }
                client.sendCmd<LangHunLingYuWorldData>(nID, worldData, false);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return true;
        }

        
        public bool ProcessGetDailyAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 1;
                int roleID = Convert.ToInt32(cmdParams[0]);
                int bhid = client.ClientData.Faction;
                if (bhid <= 0 || client.ClientData.Faction != bhid)
                {
                    result = -12;
                }
                else if (!this.CanGetAwardsByEnterTime(client))
                {
                    result = -2006;
                }
                else
                {
                    int lastDayID = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwards");
                    int flags = 0;
                    int nowDayId = Global.GetOffsetDayNow();
                    if (lastDayID == nowDayId)
                    {
                        flags = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwardsFlags");
                    }
                    List<int> levelList = new List<int>();
                    lock (this.RuntimeData.Mutex)
                    {
                        LangHunLingYuBangHuiData bangHuiData;
                        if (!this.RuntimeData.BangHuiDataDict.TryGetValue((long)bhid, out bangHuiData))
                        {
                            result = -20;
                            goto IL_3E3;
                        }
                        if (null != bangHuiData.SelfCityList)
                        {
                            foreach (int id in bangHuiData.SelfCityList)
                            {
                                LangHunLingYuCityDataEx cityDataEx;
                                if (this.RuntimeData.CityDataExDict.TryGetValue(id, out cityDataEx) && cityDataEx.Site[0] == (long)bhid)
                                {
                                    if (0 == Global.GetIntSomeBit(flags, cityDataEx.CityLevel))
                                    {
                                        levelList.Add(cityDataEx.CityLevel);
                                    }
                                }
                            }
                        }
                    }
                    bool getSomeAwards = false;
                    foreach (int level in levelList)
                    {
                        CityLevelInfo awardsItem;
                        if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(level, out awardsItem))
                        {
                            LogManager.WriteLog(LogTypes.Error, "城池等级每日奖励未配置：Level=" + level, null, true);
                        }
                        else
                        {
                            List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(awardsItem.DayAward.Items, -1);
                            if (Global.CanAddGoodsDataList(client, goodsDataList))
                            {
                                for (int i = 0; i < goodsDataList.Count; i++)
                                {
                                    Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "圣域争霸胜利战盟每日奖励", "1900-01-01 12:00:00", 0, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, 0, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, 0, null, null, 0, true);
                                    GoodsData goodsData = goodsDataList[i];
                                    GameManager.logDBCmdMgr.AddDBLogInfo(goodsData.Id, Global.ModifyGoodsLogName(goodsData), "圣域争霸胜利战盟每日奖励", Global.GetMapName(client.ClientData.MapCode), client.ClientData.RoleName, "增加", goodsData.GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
                                }
                                flags = Global.SetIntSomeBit(level, flags, true);
                                getSomeAwards = true;
                                Global.SaveRoleParamsInt32ValueToDB(client, "LangHunLingYuDayAwards", Global.GetOffsetDayNow(), true);
                                Global.SaveRoleParamsInt32ValueToDB(client, "LangHunLingYuDayAwardsFlags", flags, true);
                                EventLogManager.AddRoleEvent(client, OpTypes.GiveAwards, OpTags.LangHunLingYuDailyAwards, LogRecordType.IntValue, new object[]
                                {
                                    level
                                });
                            }
                            else
                            {
                                result = -100;
                            }
                        }
                    }
                    if (getSomeAwards)
                    {
                        this.CheckTipsIconState(client);
                    }
                }
                IL_3E3:
                client.sendCmd(nID, string.Format("{0}", result), false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        private bool CheckMap(GameClient client)
        {
            SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
            return sceneType == SceneUIClasses.Normal;
        }

        
        private int CheckSignUpCondition(int cityLevel, ref CityLevelInfo sceneItem, ref LangHunLingYuGameStates state)
        {
            int result = 0;
            cityLevel = Math.Max(cityLevel, 1);
            lock (this.RuntimeData.Mutex)
            {
                if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(cityLevel, out sceneItem))
                {
                    return -12;
                }
            }
            result = -2001;
            DateTime now = TimeUtil.NowDateTime();
            lock (this.RuntimeData.Mutex)
            {
                for (int i = 0; i < sceneItem.BaoMingTime.Count - 1; i += 2)
                {
                    TimeSpan ts = now.TimeOfDay.Add(TimeSpan.FromDays((double)now.DayOfWeek));
                    if (ts >= sceneItem.BaoMingTime[i] && ts <= sceneItem.BaoMingTime[i + 1])
                    {
                        state = LangHunLingYuGameStates.SignUp;
                        result = 1;
                        break;
                    }
                }
            }
            return result;
        }

        
        private int CheckFightCondition(int cityLevel, ref CityLevelInfo sceneItem, ref LangHunLingYuGameStates state)
        {
            int result = 0;
            cityLevel = Math.Max(cityLevel, 1);
            lock (this.RuntimeData.Mutex)
            {
                if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(cityLevel, out sceneItem))
                {
                    return -12;
                }
            }
            result = -2001;
            DateTime now = TimeUtil.NowDateTime();
            lock (this.RuntimeData.Mutex)
            {
                if (sceneItem.AttackWeekDay.Contains((int)now.DayOfWeek))
                {
                    for (int i = 0; i < sceneItem.AttackTime.Count - 1; i += 2)
                    {
                        if (now.TimeOfDay >= sceneItem.AttackTime[i] && now.TimeOfDay <= sceneItem.AttackTime[i + 1])
                        {
                            state = LangHunLingYuGameStates.Start;
                            result = 0;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        
        private TimeSpan GetStartTime(int cityLevel)
        {
            CityLevelInfo sceneItem = null;
            TimeSpan startTime = TimeSpan.MinValue;
            DateTime now = TimeUtil.NowDateTime();
            lock (this.RuntimeData.Mutex)
            {
                if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(cityLevel, out sceneItem))
                {
                    goto IL_108;
                }
            }
            lock (this.RuntimeData.Mutex)
            {
                for (int i = 0; i < sceneItem.AttackTime.Count - 1; i += 2)
                {
                    if (now.TimeOfDay >= sceneItem.AttackTime[i] && now.TimeOfDay <= sceneItem.AttackTime[i + 1])
                    {
                        startTime = sceneItem.AttackTime[i];
                        break;
                    }
                }
            }
            IL_108:
            if (startTime < TimeSpan.Zero)
            {
                startTime = now.TimeOfDay;
            }
            return startTime;
        }

        
        private string GetBHName(int bangHuiID)
        {
            BangHuiMiniData bhData = Global.GetBangHuiMiniData(bangHuiID, 0);
            string result;
            if (null != bhData)
            {
                result = bhData.BHName;
            }
            else
            {
                result = GLang.GetLang(6, new object[0]);
            }
            return result;
        }

        
        private void ProcessTimeAddRoleExp(LangHunLingYuScene scene)
        {
            long ticks = TimeUtil.NOW();
            if (ticks - scene.LastAddBangZhanAwardsTicks >= 10000L)
            {
                scene.LastAddBangZhanAwardsTicks = ticks;
                this.NotifyQiZhiBuffOwnerDataList(scene);
                this.NotifyLongTaRoleDataList(scene);
                this.NotifyLongTaOwnerData(scene);
                foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                {
                    List<GameClient> list = copyMap.GetClientsList();
                    foreach (GameClient client in list)
                    {
                        if (null != client)
                        {
                            this._LevelAwardsMgr.ProcessBangZhanAwards(client);
                        }
                    }
                }
            }
        }

        
        public bool GetZhanMengBirthPoint(LangHunLingYuSceneInfo sceneInfo, GameClient client, int toMapCode, out int mapCode, out int posX, out int posY)
        {
            mapCode = sceneInfo.MapCode;
            posX = -1;
            posY = -1;
            bool result;
            if (client.ClientData.GuanZhanGM > 0)
            {
                if (VideoLogic.getInstance().GetGuanZhanPos(toMapCode, ref posX, ref posY))
                {
                    mapCode = toMapCode;
                }
                result = true;
            }
            else
            {
                int bhid = client.ClientData.Faction;
                lock (this.RuntimeData.Mutex)
                {
                    int site = client.ClientData.BattleWhichSide - 1;
                    int round = 0;
                    if (sceneInfo.MapCode_LongTa == toMapCode)
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
                                goto IL_105;
                            }
                        }
                        mapCode = sceneInfo.MapCode_LongTa;
                        posX = (int)pt.X;
                        posY = (int)pt.Y;
                        return true;
                    }
                    IL_105:
                    round = 0;
                    LangHunLingYuScene scene = client.SceneObject as LangHunLingYuScene;
                    if (scene != null && client.ClientData.Faction == scene.SuperQiZhiOwnerBhid && toMapCode == sceneInfo.MapCode)
                    {
                        for (; ; )
                        {
                            mapCode = toMapCode;
                            posX = Global.GetRandomNumber(this.RuntimeData.SuperQiZhiOwnerBirthPosX - 400, this.RuntimeData.SuperQiZhiOwnerBirthPosX + 400);
                            posY = Global.GetRandomNumber(this.RuntimeData.SuperQiZhiOwnerBirthPosY - 400, this.RuntimeData.SuperQiZhiOwnerBirthPosY + 400);
                            if (!Global.InObs(ObjectTypes.OT_CLIENT, toMapCode, posX, posY, 0, 0))
                            {
                                break;
                            }
                            if (round++ >= 100)
                            {
                                goto IL_1D1;
                            }
                        }
                        return true;
                    }
                    IL_1D1:
                    List<MapBirthPoint> list;
                    if (!this.RuntimeData.MapBirthPointListDict.TryGetValue(site, out list) || list.Count == 0)
                    {
                        return true;
                    }
                    round = 0;
                    for (; ; )
                    {
                        int rnd = Global.GetRandomNumber(0, list.Count);
                        MapBirthPoint mapBirthPoint = list[rnd];
                        posX = mapBirthPoint.BirthPosX + Global.GetRandomNumber(-mapBirthPoint.BirthRangeX, mapBirthPoint.BirthRangeX);
                        posY = mapBirthPoint.BirthPosY + Global.GetRandomNumber(-mapBirthPoint.BirthRangeY, mapBirthPoint.BirthRangeY);
                        if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, posX, posY, 0, 0))
                        {
                            break;
                        }
                        if (round++ >= 1000)
                        {
                            goto Block_12;
                        }
                    }
                    return true;
                    Block_12:;
                }
                result = true;
            }
            return result;
        }

        
        public bool ClientRelive(GameClient client)
        {
            int mapCode = client.ClientData.MapCode;
            LangHunLingYuSceneInfo sceneInfo = client.SceneInfoObject as LangHunLingYuSceneInfo;
            if (null != sceneInfo)
            {
                int toMapCode;
                int toPosX;
                int toPosY;
                if (this.GetZhanMengBirthPoint(sceneInfo, client, toMapCode = sceneInfo.MapCode, out toMapCode, out toPosX, out toPosY))
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

        
        public bool ClientChangeMap(GameClient client, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
        {
            LangHunLingYuSceneInfo sceneInfo = client.SceneInfoObject as LangHunLingYuSceneInfo;
            if (null != sceneInfo)
            {
                if (toNewMapCode == sceneInfo.MapCode || toNewMapCode == sceneInfo.MapCode_LongTa)
                {
                    if (client.ClientData.MapCode != sceneInfo.MapCode_LongTa)
                    {
                        int toMapCode;
                        int toPosX;
                        int toPosY;
                        if (this.GetZhanMengBirthPoint(sceneInfo, client, toNewMapCode, out toMapCode, out toPosX, out toPosY))
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

        
        public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
        {
            bool isQiZuo = false;
            LangHunLingYuScene scene = client.SceneObject as LangHunLingYuScene;
            CopyMap copyMap;
            if (scene != null && scene.CopyMapDict.TryGetValue(client.ClientData.MapCode, out copyMap))
            {
                lock (this.RuntimeData.Mutex)
                {
                    QiZhiConfig item;
                    if (scene.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out item))
                    {
                        isQiZuo = true;
                        if (item.Alive)
                        {
                            return isQiZuo;
                        }
                        if (item.KillerBhid <= 0L || (long)client.ClientData.Faction == item.KillerBhid || Math.Abs(TimeUtil.NOW() - item.DeadTicks) >= 10000L)
                        {
                            if (client.ClientData.Faction <= 0)
                            {
                                GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(45, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                            }
                            else if (Math.Abs(client.ClientData.PosX - item.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - item.PosY) <= 1000)
                            {
                                item.Alive = true;
                                int zoneId;
                                item.InstallBhName = this.GetBangHuiName(client.ClientData.Faction, out zoneId);
                                item.BattleWhichSide = client.ClientData.BattleWhichSide;
                                this.CreateMonster(copyMap, item, item.MonsterId);
                                this.UpdateQiZhiBangHui(scene, npcExtentionID, client.ClientData.Faction, client.ClientData.BHName, zoneId);
                                Global.BroadcastBangHuiMsg(-1, client.ClientData.Faction, StringUtil.substitute(GLang.GetLang(48, new object[0]), new object[]
                                {
                                    Global.FormatRoleName(client, client.ClientData.RoleName),
                                    Global.GetServerLineName2(),
                                    Global.GetMapName(client.ClientData.MapCode)
                                }), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlySysHint);
                            }
                        }
                    }
                }
            }
            return isQiZuo;
        }

        
        public void OnProcessJunQiDead(GameClient client, Monster monster)
        {
            if (client != null && this.RuntimeData.JunQiMonsterHashSet.Contains(monster.MonsterInfo.ExtensionID))
            {
                int zoneId;
                string bhName = this.GetBangHuiName(client.ClientData.Faction, out zoneId);
                LangHunLingYuScene scene = client.SceneObject as LangHunLingYuScene;
                QiZhiConfig qizhiConfig = monster.Tag as QiZhiConfig;
                if (scene != null && null != qizhiConfig)
                {
                    lock (this.RuntimeData.Mutex)
                    {
                        qizhiConfig.KillerBhid = (long)client.ClientData.Faction;
                        qizhiConfig.InstallBhName = "";
                        qizhiConfig.InstallBhid = 0L;
                        qizhiConfig.DeadTicks = TimeUtil.NOW();
                        qizhiConfig.Alive = false;
                        this.UpdateQiZhiBangHui(scene, qizhiConfig.NPCID, 0, bhName, 0);
                    }
                }
            }
        }

        
        private void ResetQiZhiBuff(GameClient client)
        {
            if (!VideoLogic.getInstance().IsGuanZhanGM(client))
            {
                LangHunLingYuScene scene = client.SceneObject as LangHunLingYuScene;
                int toMapCode = client.ClientData.MapCode;
                List<int> bufferIDList = new List<int>();
                lock (this.RuntimeData.Mutex)
                {
                    EquipPropItem item = null;
                    int bufferID = 0;
                    if (scene != null && client.SceneType == 35)
                    {
                        for (int i = 0; i < scene.QiZhiBuffOwnerDataList.Count; i++)
                        {
                            bool add = false;
                            LangHunLingYuQiZhiBuffOwnerData ownerData = scene.QiZhiBuffOwnerDataList[i];
                            QiZhiConfig qiZhiConfig;
                            if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(ownerData.NPCID, out qiZhiConfig))
                            {
                                bufferID = qiZhiConfig.BufferID;
                                item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferID);
                                if (null != item)
                                {
                                    if (ownerData.OwnerBHID == client.ClientData.Faction)
                                    {
                                        add = true;
                                    }
                                }
                            }
                            this.UpdateQiZhiBuff4GameClient(client, item, bufferID, add);
                        }
                    }
                    else
                    {
                        foreach (QiZhiConfig qiZhiConfig in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
                        {
                            this.UpdateQiZhiBuff4GameClient(client, item, qiZhiConfig.BufferID, false);
                        }
                    }
                }
            }
        }

        
        public void OnStartPlayGame(GameClient client)
        {
            this.ResetQiZhiBuff(client);
            this.BroadcastLuoLanChengZhuLoginHint(client);
            LangHunLingYuScene scene = client.SceneObject as LangHunLingYuScene;
            if (null != scene)
            {
                this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
            }
        }

        
        public void BroadcastLuoLanChengZhuLoginHint(GameClient client)
        {
        }

        
        private void CreateMonster(CopyMap copyMap, QiZhiConfig qiZhiConfig, int monsterId)
        {
            GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, monsterId, copyMap.CopyMapID, 1, qiZhiConfig.PosX / this.RuntimeData.MapGridWidth, qiZhiConfig.PosY / this.RuntimeData.MapGridHeight, 0, 0, SceneUIClasses.LangHunLingYu, qiZhiConfig, null);
        }

        
        private bool RefuseChangeBangHui(int bhid)
        {
            CityLevelInfo sceneItem = null;
            LangHunLingYuGameStates state = LangHunLingYuGameStates.None;
            this.CheckFightCondition(1, ref sceneItem, ref state);
            if (state == LangHunLingYuGameStates.Start)
            {
                lock (this.RuntimeData.Mutex)
                {
                    LangHunLingYuBangHuiData bangHuiData;
                    if (this.RuntimeData.BangHuiDataDict.TryGetValue((long)bhid, out bangHuiData))
                    {
                        if (bangHuiData.SelfCityList.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        
        public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
        {
            bool result;
            if (this.RefuseChangeBangHui(e.BHID))
            {
                e.Result = false;
                GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(402, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
        {
            bool result;
            if (this.RefuseChangeBangHui(e.BHID))
            {
                e.Result = false;
                GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(403, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        
        
        private RoleDataEx OwnerRoleData
        {
            get
            {
                RoleDataEx ownerRoleData;
                lock (this.OwnerRoleDataMutex)
                {
                    ownerRoleData = this._OwnerRoleData;
                }
                return ownerRoleData;
            }
            set
            {
                lock (this.OwnerRoleDataMutex)
                {
                    this._OwnerRoleData = value;
                }
            }
        }

        
        public void ReplaceLangHunLingYuNpc()
        {
            if (this.RuntimeData.OwnerHistList == null || this.RuntimeData.OwnerHistList.Count == 0 || this.RuntimeData.ChengHaoBHid == 0L)
            {
                this.RestoreLangHunLingYuNpc();
            }
            else
            {
                LangHunLingYuKingHist OwnerData = this.RuntimeData.OwnerHistList[this.RuntimeData.OwnerHistList.Count - 1];
                RoleDataEx rd = DataHelper.BytesToObject<RoleDataEx>(OwnerData.CityOwnerRoleData, 0, OwnerData.CityOwnerRoleData.Length);
                if (rd == null || rd.RoleID <= 0)
                {
                    this.RestoreLangHunLingYuNpc();
                }
                else
                {
                    this.OwnerRoleData = rd;
                    NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 134);
                    if (null != npc)
                    {
                        npc.ShowNpc = false;
                        GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
                        FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang3, false);
                        FakeRoleManager.ProcessNewFakeRole(new SafeClientData
                        {
                            RoleData = rd
                        }, npc.MapCode, FakeRoleTypes.DiaoXiang3, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, 134);
                    }
                }
            }
        }

        
        public void RestoreLangHunLingYuNpc()
        {
            this.OwnerRoleData = null;
            NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 134);
            if (null != npc)
            {
                npc.ShowNpc = true;
                GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
                FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang3, false);
            }
        }

        
        public void LangHunLingYuBuildMaxCityOwnerInfo(LangHunLingYuStatisticalData statisticalData, int ServerID)
        {
            if (statisticalData.CityId == 1)
            {
                BangHuiDetailData bangHuiDetailData = this.GetBangHuiDetailDataAuto(statisticalData.SiteBhids[0], -1, ServerID);
                if (null != bangHuiDetailData)
                {
                    statisticalData.rid = bangHuiDetailData.BZRoleID;
                    RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, statisticalData.rid), ServerID);
                    if (dbRd != null && dbRd.RoleID > 0)
                    {
                        statisticalData.CityOwnerRoleData = DataHelper.ObjectToBytes<RoleDataEx>(dbRd);
                    }
                }
            }
        }

        
        public BangHuiDetailData GetBangHuiDetailDataAuto(int bhid, int roleID = -1, int ServerID = 0)
        {
            BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, ServerID);
            if (null != bangHuiDetailData)
            {
                if (roleID <= 0 && bangHuiDetailData.BZRoleID > 0)
                {
                    bangHuiDetailData = Global.GetBangHuiDetailData(bangHuiDetailData.BZRoleID, bhid, ServerID);
                }
            }
            return bangHuiDetailData;
        }

        
        public bool CanEnterKuaFuMap(KuaFuServerLoginData kuaFuServerLoginData)
        {
            int rid = kuaFuServerLoginData.RoleId;
            LangHunLingYuFuBenData fuBenData;
            lock (this.RuntimeData.Mutex)
            {
                if (!this.RuntimeData.FuBenDataDict.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
                {
                    fuBenData = null;
                }
                else if (fuBenData.State >= GameFuBenState.End)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("圣域争霸副本已结束,禁止角色{0}进入", rid), null, true);
                    return false;
                }
            }
            if (null == fuBenData)
            {
                LangHunLingYuFuBenData newFuBenData = YongZheZhanChangClient.getInstance().GetLangHunLingYuGameFuBenData((int)kuaFuServerLoginData.GameId);
                if (newFuBenData == null || newFuBenData.State == GameFuBenState.End)
                {
                    LogManager.WriteLog(LogTypes.Error, ("获取不到有效的副本数据," + newFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
                    return false;
                }
                if (newFuBenData.ServerId != GameManager.ServerId)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("玩家请求进入的圣域争霸活动GameId={0}，不在本服务器{1}", fuBenData.GameId, GameManager.ServerId), null, true);
                    return false;
                }
                lock (this.RuntimeData.Mutex)
                {
                    if (!this.RuntimeData.FuBenDataDict.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
                    {
                        fuBenData = newFuBenData;
                        fuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
                        this.RuntimeData.FuBenDataDict[fuBenData.GameId] = fuBenData;
                    }
                }
            }
            return true;
        }

        
        public bool OnInitGameKuaFu(GameClient client)
        {
            int bhid = client.ClientData.Faction;
            long rid = (long)client.ClientData.RoleID;
            int side = 0;
            KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
            LangHunLingYuFuBenData fuBenData;
            lock (this.RuntimeData.Mutex)
            {
                if (!this.RuntimeData.FuBenDataDict.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("玩家请求进入的圣域争霸活动GameId={0}，不在本服务器{1},角色{2}({3})", new object[]
                    {
                        fuBenData.GameId,
                        GameManager.ServerId,
                        rid,
                        Global.FormatRoleName4(client)
                    }), null, true);
                    return false;
                }
                if (fuBenData.State >= GameFuBenState.End)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("圣域争霸副本已结束,禁止角色{0}({1})进入", rid, Global.FormatRoleName4(client)), null, true);
                    return false;
                }
            }
            bool result;
            if (fuBenData.CityDataEx == null)
            {
                result = false;
            }
            else
            {
                if (client.ClientData.GuanZhanGM == 0)
                {
                    if (null != fuBenData.CityDataEx.Site)
                    {
                        for (int i = 0; i < fuBenData.CityDataEx.Site.Length; i++)
                        {
                            if (fuBenData.CityDataEx.Site[i] == (long)bhid)
                            {
                                side = i + 1;
                            }
                        }
                    }
                    if (side <= 0)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("角色{0}({1})所在帮会({2})不在指定的圣域争霸活动GameId={3}", new object[]
                        {
                            rid,
                            Global.FormatRoleName4(client),
                            bhid,
                            fuBenData.GameId
                        }), null, true);
                        return false;
                    }
                }
                LangHunLingYuSceneInfo sceneInfo;
                lock (this.RuntimeData.Mutex)
                {
                    int sceneIndex = fuBenData.GameId % this.RuntimeData.SceneDataList.Count;
                    sceneInfo = this.RuntimeData.SceneDataList[sceneIndex];
                    client.SceneInfoObject = sceneInfo;
                    client.ClientData.MapCode = sceneInfo.MapCode;
                }
                client.ClientData.BattleWhichSide = side;
                kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
                int toMapCode;
                int toPosX;
                int toPosY;
                if (!this.GetZhanMengBirthPoint(sceneInfo, client, client.ClientData.MapCode, out toMapCode, out toPosX, out toPosY))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("角色{0}({1})无法获取有效的阵营和出生点,进入跨服失败,side={2}", rid, Global.FormatRoleName4(client), side), null, true);
                    result = false;
                }
                else
                {
                    client.ClientData.PosX = toPosX;
                    client.ClientData.PosY = toPosY;
                    client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
                    result = true;
                }
            }
            return result;
        }

        
        public bool IsGongNengOpened(GameClient client, bool hint = false)
        {
            return client.ClientData.GuanZhanGM > 0 || (GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("LangHunLingYu") && !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot9) && GlobalNew.IsGongNengOpened(client, GongNengIDs.LangHunLingYu, hint));
        }

        
        public void FillGuanZhanData(GameClient client, GuanZhanData gzData)
        {
            lock (this.RuntimeData.Mutex)
            {
                LangHunLingYuScene scene = client.SceneObject as LangHunLingYuScene;
                if (null != scene)
                {
                    gzData.SideName.Add(GLang.GetLang(5002, new object[0]));
                    gzData.SideName.Add(GLang.GetLang(5003, new object[0]));
                    foreach (KeyValuePair<int, LangHunLingYuClientContextData> kvp in scene.ClientContextDataDict)
                    {
                        GameClient sceneClient = GameManager.ClientMgr.FindClient(kvp.Key);
                        if (sceneClient != null && sceneClient.ClientData.HideGM <= 0)
                        {
                            SceneUIClasses sceneType = Global.GetMapSceneType(sceneClient.ClientData.MapCode);
                            if (SceneUIClasses.LangHunLingYu == sceneType)
                            {
                                int side = (kvp.Value.BattleWhichSide == 1) ? 1 : 2;
                                List<GuanZhanRoleMiniData> roleDataList = null;
                                if (!gzData.RoleMiniDataDict.TryGetValue(side, out roleDataList))
                                {
                                    roleDataList = new List<GuanZhanRoleMiniData>();
                                    gzData.RoleMiniDataDict[side] = roleDataList;
                                }
                                GuanZhanRoleMiniData roleMiniData = new GuanZhanRoleMiniData();
                                roleMiniData.RoleID = sceneClient.ClientData.RoleID;
                                roleMiniData.Name = Global.FormatRoleNameWithZoneId2(sceneClient);
                                roleMiniData.Level = sceneClient.ClientData.Level;
                                roleMiniData.ChangeLevel = sceneClient.ClientData.ChangeLifeCount;
                                roleMiniData.Occupation = sceneClient.ClientData.Occupation;
                                roleMiniData.RoleSex = sceneClient.ClientData.RoleSex;
                                roleMiniData.BHZhiWu = sceneClient.ClientData.BHZhiWu;
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

        
        private void InitScene(LangHunLingYuScene scene, GameClient client)
        {
            foreach (LangHunLingYuQiZhiBuffOwnerData item in this.RuntimeData.QiZhiBuffOwnerDataList)
            {
                scene.QiZhiBuffOwnerDataList.Add(new LangHunLingYuQiZhiBuffOwnerData
                {
                    NPCID = item.NPCID
                });
            }
            foreach (QiZhiConfig item2 in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
            {
                scene.NPCID2QiZhiConfigDict.Add(item2.NPCID, item2.Clone() as QiZhiConfig);
            }
        }

        
        public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
        {
            bool result;
            if (sceneType == SceneUIClasses.LangHunLingYu)
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
                    int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
                    DateTime now = TimeUtil.NowDateTime();
                    lock (this.RuntimeData.Mutex)
                    {
                        LangHunLingYuScene scene = null;
                        if (!this.RuntimeData.SceneDict.TryGetValue(fuBenSeqId, out scene))
                        {
                            LangHunLingYuFuBenData fuBenData;
                            if (!this.RuntimeData.FuBenDataDict.TryGetValue(gameId, out fuBenData))
                            {
                                LogManager.WriteLog(LogTypes.Error, "圣域争霸没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
                                return false;
                            }
                            if (fuBenData.State >= GameFuBenState.End)
                            {
                                LogManager.WriteLog(LogTypes.Error, "圣域争霸副本已经结束#GameID=" + gameId, null, true);
                                return false;
                            }
                            scene = new LangHunLingYuScene();
                            scene.CleanAllInfo();
                            scene.GameId = gameId;
                            this.RuntimeData.MapGridWidth = gameMap.MapGridWidth;
                            this.RuntimeData.MapGridHeight = gameMap.MapGridHeight;
                            int cityLevel = this.GetCityLevelById(fuBenData.CityId);
                            if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(cityLevel, out scene.LevelInfo))
                            {
                                LogManager.WriteLog(LogTypes.Error, "圣域争霸没有为副本找到对应的城池等级配置:CityId=" + fuBenData.CityId, null, true);
                            }
                            scene.SceneInfo = (client.SceneInfoObject as LangHunLingYuSceneInfo);
                            DateTime startTime = now.Date.Add(this.GetStartTime(scene.LevelInfo.ID));
                            scene.StartTimeTicks = startTime.Ticks / 10000L;
                            scene.m_lEndTime = scene.StartTimeTicks + (long)((scene.SceneInfo.PrepareSecs + scene.SceneInfo.FightingSecs) * 1000);
                            this.InitScene(scene, client);
                            this.RuntimeData.SceneDict[fuBenSeqId] = scene;
                            scene.CityData.CityId = fuBenData.CityDataEx.CityId;
                            scene.CityData.CityLevel = fuBenData.CityDataEx.CityLevel;
                            LangHunLingYuBangHuiDataEx bangHuiDataEx;
                            if (this.RuntimeData.BangHuiDataExDict.TryGetValue(fuBenData.CityDataEx.Site[0], out bangHuiDataEx))
                            {
                                scene.LongTaOwnerData.OwnerBHid = bangHuiDataEx.Bhid;
                                scene.LongTaOwnerData.OwnerBHName = bangHuiDataEx.BhName;
                                scene.LongTaOwnerData.OwnerBHZoneId = bangHuiDataEx.ZoneId;
                            }
                        }
                        scene.CopyMapDict[mapCode] = copyMap;
                        int bhid = client.ClientData.Faction;
                        if (!this.RuntimeData.BangHuiMiniDataCacheDict.ContainsKey(bhid))
                        {
                            this.RuntimeData.BangHuiMiniDataCacheDict[bhid] = Global.GetBangHuiMiniData(bhid, client.ServerId);
                        }
                        LangHunLingYuClientContextData clientContextData;
                        if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
                        {
                            clientContextData = new LangHunLingYuClientContextData
                            {
                                RoleId = roleId,
                                ServerId = client.ServerId,
                                BattleWhichSide = client.ClientData.BattleWhichSide
                            };
                            scene.ClientContextDataDict[roleId] = clientContextData;
                        }
                        client.SceneObject = scene;
                        client.SceneGameId = (long)scene.GameId;
                        client.SceneContextData2 = clientContextData;
                        copyMap.SetRemoveTicks(scene.StartTimeTicks + (long)(scene.SceneInfo.TotalSecs * 1000));
                        copyMap.IsKuaFuCopy = true;
                    }
                    YongZheZhanChangClient.getInstance().GameFuBenRoleChangeState(roleId, 5, 0, 0);
                    result = true;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
        {
            bool result;
            if (sceneType == SceneUIClasses.LangHunLingYu)
            {
                lock (this.RuntimeData.Mutex)
                {
                    LangHunLingYuScene LangHunLingYuScene;
                    this.RuntimeData.SceneDict.TryRemove(copyMap.FuBenSeqID, out LangHunLingYuScene);
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        
        public void OnLogout(GameClient client)
        {
            YongZheZhanChangClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.StartGame, false);
        }

        
        public void TimerProc(object sender, EventArgs e)
        {
            lock (this.RuntimeData.Mutex)
            {
                if (this.RuntimeData.StatisticalDataQueue.Count > 0)
                {
                    LangHunLingYuStatisticalData data = this.RuntimeData.StatisticalDataQueue.Peek();
                    int result = YongZheZhanChangClient.getInstance().GameFuBenComplete(data);
                    if (result >= 0)
                    {
                        this.RuntimeData.StatisticalDataQueue.Dequeue();
                    }
                }
            }
            foreach (LangHunLingYuScene scene in this.RuntimeData.SceneDict.Values)
            {
                lock (this.RuntimeData.Mutex)
                {
                    DateTime now = TimeUtil.NowDateTime();
                    long ticks = TimeUtil.NOW();
                    if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
                    {
                        if (ticks >= scene.StartTimeTicks)
                        {
                            LangHunLingYuFuBenData fuBenData;
                            if (this.RuntimeData.FuBenDataDict.TryGetValue(scene.GameId, out fuBenData) && fuBenData.State == GameFuBenState.End)
                            {
                                scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
                                scene.m_lLeaveTime = TimeUtil.NOW();
                            }
                            scene.m_lPrepareTime = scene.StartTimeTicks;
                            scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
                            scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
                            scene.StateTimeData.GameType = 10;
                            scene.StateTimeData.State = (int)scene.m_eStatus;
                            scene.StateTimeData.EndTicks = scene.m_lBeginTime;
                            foreach (CopyMap copy in scene.CopyMapDict.Values)
                            {
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copy);
                            }
                        }
                    }
                    else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
                    {
                        if (ticks >= scene.m_lBeginTime)
                        {
                            scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
                            scene.m_lEndTime = scene.m_lBeginTime + (long)(scene.SceneInfo.FightingSecs * 1000);
                            scene.StateTimeData.GameType = 10;
                            scene.StateTimeData.State = (int)scene.m_eStatus;
                            scene.StateTimeData.EndTicks = scene.m_lEndTime;
                            foreach (CopyMap copy in scene.CopyMapDict.Values)
                            {
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copy);
                            }
                        }
                    }
                    else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
                    {
                        if (ticks >= scene.m_lEndTime)
                        {
                            scene.m_eStatus = GameSceneStatuses.STATUS_END;
                            scene.m_lLeaveTime = scene.m_lEndTime + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
                            scene.StateTimeData.GameType = 10;
                            scene.StateTimeData.State = 5;
                            scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
                            foreach (CopyMap copy in scene.CopyMapDict.Values)
                            {
                                GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copy);
                            }
                            this.ProcessWangChengZhanResult(scene, true);
                        }
                        else
                        {
                            if (scene.HasGuangMu)
                            {
                                foreach (CopyMap copy in scene.CopyMapDict.Values)
                                {
                                    if (copy.MapCode == scene.SceneInfo.MapCode)
                                    {
                                        scene.HasGuangMu = false;
                                        for (int guangMuId = 4; guangMuId < 9; guangMuId++)
                                        {
                                            GameManager.CopyMapMgr.AddGuangMuEvent(copy, guangMuId, 0);
                                        }
                                    }
                                }
                            }
                            this.ProcessWangChengZhanResult(scene, false);
                        }
                    }
                    else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
                    {
                        scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
                        LangHunLingYuStatisticalData statisticalData = new LangHunLingYuStatisticalData();
                        statisticalData.CompliteTime = TimeUtil.NowDateTime();
                        statisticalData.CityId = scene.CityData.CityId;
                        statisticalData.GameId = scene.GameId;
                        statisticalData.SiteBhids[0] = scene.LongTaOwnerData.OwnerBHid;
                        this.LangHunLingYuBuildMaxCityOwnerInfo(statisticalData, scene.LongTaOwnerData.OwnerBHServerId);
                        this.RuntimeData.StatisticalDataQueue.Enqueue(statisticalData);
                        LangHunLingYuFuBenData fuBenData;
                        if (this.RuntimeData.FuBenDataDict.TryGetValue(scene.GameId, out fuBenData))
                        {
                            fuBenData.State = GameFuBenState.End;
                        }
                        lock (this.RuntimeData.Mutex)
                        {
                            LangHunLingYuBangHuiDataEx oldBangHuiData = null;
                            LangHunLingYuCityDataEx oldCityDataEx = null;
                            if (this.RuntimeData.CityDataExDict.TryGetValue(statisticalData.CityId, out oldCityDataEx))
                            {
                                this.RuntimeData.BangHuiDataExDict.TryGetValue(oldCityDataEx.Site[0], out oldBangHuiData);
                            }
                            LangHunLingYuBangHuiDataEx newBangHuiData = null;
                            this.RuntimeData.BangHuiDataExDict.TryGetValue((long)statisticalData.SiteBhids[0], out newBangHuiData);
                            int oldZoneID = (oldBangHuiData == null) ? 0 : oldBangHuiData.ZoneId;
                            int oldBHID = (oldBangHuiData == null) ? 0 : oldBangHuiData.Bhid;
                            int oldLev = (oldBangHuiData == null) ? 0 : oldBangHuiData.Level;
                            int newZoneID = (newBangHuiData == null) ? 0 : newBangHuiData.ZoneId;
                            int newBHID = (newBangHuiData == null) ? 0 : newBangHuiData.Bhid;
                            int newLev = (newBangHuiData == null) ? 0 : newBangHuiData.Level;
                            EventLogManager.AddLangHunLingYuEvent(statisticalData.GameId, statisticalData.CityId, oldZoneID, oldBHID, oldLev, newZoneID, newBHID, newLev);
                        }
                    }
                    else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
                    {
                        if (ticks >= scene.m_lLeaveTime)
                        {
                            foreach (CopyMap copy in scene.CopyMapDict.Values)
                            {
                                copy.SetRemoveTicks(scene.m_lLeaveTime + 300000L);
                                try
                                {
                                    List<GameClient> objsList = copy.GetClientsList();
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
                                    DataHelper.WriteExceptionLogEx(ex, "圣域争霸系统清场调度异常");
                                }
                            }
                            scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
                        }
                    }
                }
            }
        }

        
        public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool otherInfo = true)
        {
            lock (this.RuntimeData.Mutex)
            {
                LangHunLingYuScene scene = client.SceneObject as LangHunLingYuScene;
                if (scene != null)
                {
                    if (timeState)
                    {
                        client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
                    }
                    if (otherInfo)
                    {
                        client.sendCmd<List<LangHunLingYuQiZhiBuffOwnerData>>(1151, scene.QiZhiBuffOwnerDataList, false);
                        client.sendCmd<List<BangHuiRoleCountData>>(1150, scene.LongTaBHRoleCountList, false);
                        client.sendCmd<LangHunLingYuLongTaOwnerData>(1152, scene.LongTaOwnerData, false);
                    }
                }
            }
        }

        
        public void ProcessWangChengZhanResult(LangHunLingYuScene scene, bool finish)
        {
            try
            {
                DateTime now = TimeUtil.NowDateTime();
                int remailSecs = (int)((scene.m_lEndTime - TimeUtil.NOW()) / 1000L);
                if (!finish)
                {
                    if (remailSecs < 0)
                    {
                        remailSecs = 0;
                    }
                    this.UpdateQiZhiBuffParams(remailSecs);
                    bool ret = this.TryGenerateNewHuangChengBangHui(scene);
                    if (ret)
                    {
                        this.HandleHuangChengResultEx(scene, false);
                    }
                    else
                    {
                        this.ProcessTimeAddRoleExp(scene);
                    }
                }
                else
                {
                    this.TryGenerateNewHuangChengBangHui(scene);
                    this.HandleHuangChengResultEx(scene, true);
                    this.GiveLangHunLingYuAwards(scene);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteException(ex.ToString());
            }
        }

        
        private void UpdateQiZhiBuffParams(int secs)
        {
            lock (this.RuntimeData.Mutex)
            {
                foreach (int key in this.RuntimeData.QiZhiBuffEnableParamsDict.Keys)
                {
                    this.RuntimeData.QiZhiBuffEnableParamsDict[key][0] = (double)secs;
                }
            }
        }

        
        private void GiveLangHunLingYuAwards(LangHunLingYuScene scene)
        {
            LangHunLingYuAwardsData successAwardsData = new LangHunLingYuAwardsData();
            LangHunLingYuAwardsData faildAwardsData = new LangHunLingYuAwardsData();
            successAwardsData.Success = 1;
            successAwardsData.AwardsItemDataList = scene.LevelInfo.Award.Items;
            successAwardsData.successBhName = (faildAwardsData.successBhName = ((scene.LongTaOwnerData.OwnerBHid > 0) ? scene.LongTaOwnerData.OwnerBHName : ""));
            HashSet<int> resultIds = new HashSet<int>();
            foreach (CopyMap copyMap in scene.CopyMapDict.Values)
            {
                List<GameClient> objList = copyMap.GetClientsList();
                foreach (GameClient client in objList)
                {
                    if (client.ClientData.GuanZhanGM > 0)
                    {
                        client.sendCmd<LangHunLingYuAwardsData>(1159, successAwardsData, false);
                    }
                    else
                    {
                        LangHunLingYuAwardsData awardsData = (client.ClientData.Faction == scene.LongTaOwnerData.OwnerBHid) ? successAwardsData : faildAwardsData;
                        if (awardsData.AwardsItemDataList != null)
                        {
                            if (Global.CanAddGoodsNum(client, awardsData.AwardsItemDataList.Count))
                            {
                                foreach (AwardsItemData item in awardsData.AwardsItemDataList)
                                {
                                    Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "圣域争霸胜利奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
                                }
                            }
                            else
                            {
                                Global.UseMailGivePlayerAward2(client, awardsData.AwardsItemDataList, GLang.GetLang(404, new object[0]), GLang.GetLang(404, new object[0]), 0, 0, 0);
                            }
                        }
                        EventLogManager.AddRoleEvent(client, OpTypes.GiveAwards, OpTags.LangHunLingYu, LogRecordType.IntValue2, new object[]
                        {
                            client.ClientData.Faction,
                            awardsData.Success
                        });
                        if (resultIds.Add(client.ClientData.Faction))
                        {
                            EventLogManager.AddGameEvent(LogRecordType.LangHunLingYu, new object[]
                            {
                                35,
                                client.ClientData.Faction,
                                awardsData.Success
                            });
                        }
                        client.sendCmd<LangHunLingYuAwardsData>(1159, awardsData, false);
                    }
                }
            }
        }

        
        public bool TryGenerateNewHuangChengBangHui(LangHunLingYuScene scene)
        {
            int newBHid = 0;
            int newBHServerID = 0;
            this.GetTheOnlyOneBangHui(scene, out newBHid, out newBHServerID);
            lock (this.RuntimeData.Mutex)
            {
                if (newBHid <= 0 || newBHid == scene.LongTaOwnerData.OwnerBHid)
                {
                    scene.LastTheOnlyOneBangHui = 0;
                    return false;
                }
                if (scene.LastTheOnlyOneBangHui != newBHid)
                {
                    scene.LastTheOnlyOneBangHui = newBHid;
                    scene.BangHuiTakeHuangGongTicks = TimeUtil.NOW();
                    return false;
                }
                if (scene.LastTheOnlyOneBangHui > 0)
                {
                    long ticks = TimeUtil.NOW();
                    EventLogManager.AddGameEvent(LogRecordType.LangHunLingYuLongTaOnlyBangHuiLog, new object[]
                    {
                        scene.CityData.CityId,
                        newBHid,
                        ticks - scene.BangHuiTakeHuangGongTicks,
                        "狼魂领域龙塔占领持续时间"
                    });
                    if (ticks - scene.BangHuiTakeHuangGongTicks > (long)this.RuntimeData.MaxTakingHuangGongSecs)
                    {
                        scene.LongTaOwnerData.OwnerBHid = scene.LastTheOnlyOneBangHui;
                        scene.LongTaOwnerData.OwnerBHName = this.GetBangHuiName(newBHid, out scene.LongTaOwnerData.OwnerBHZoneId);
                        scene.LongTaOwnerData.OwnerBHServerId = newBHServerID;
                        return true;
                    }
                }
            }
            return false;
        }

        
        public void GetTheOnlyOneBangHui(LangHunLingYuScene scene, out int newBHid, out int newBHServerID)
        {
            newBHid = 0;
            newBHServerID = 0;
            CopyMap copyMap;
            if (scene.CopyMapDict.TryGetValue(scene.SceneInfo.MapCode_LongTa, out copyMap))
            {
                List<GameClient> lsClients = copyMap.GetClientsList();
                lsClients = Global.GetMapAliveClientsEx(lsClients, scene.SceneInfo.MapCode_LongTa, true, 0L);
                lock (this.RuntimeData.Mutex)
                {
                    Dictionary<int, BangHuiRoleCountData> dict = new Dictionary<int, BangHuiRoleCountData>();
                    for (int i = 0; i < lsClients.Count; i++)
                    {
                        GameClient client = lsClients[i];
                        if (client.ClientData.GuanZhanGM <= 0)
                        {
                            int bhid = client.ClientData.Faction;
                            if (bhid > 0)
                            {
                                BangHuiRoleCountData data;
                                if (!dict.TryGetValue(bhid, out data))
                                {
                                    data = new BangHuiRoleCountData
                                    {
                                        BHID = bhid,
                                        RoleCount = 0,
                                        ServerID = client.ServerId
                                    };
                                    dict.Add(bhid, data);
                                }
                                data.RoleCount++;
                            }
                        }
                    }
                    scene.LongTaBHRoleCountList = dict.Values.ToList<BangHuiRoleCountData>();
                    if (scene.LongTaBHRoleCountList.Count == 1)
                    {
                        newBHid = scene.LongTaBHRoleCountList[0].BHID;
                        newBHServerID = scene.LongTaBHRoleCountList[0].ServerID;
                        EventLogManager.AddGameEvent(LogRecordType.LangHunLingYuLongTaOnlyBangHuiLog, new object[]
                        {
                            scene.CityData.CityId,
                            newBHid,
                            -1,
                            "狼魂领域龙塔唯一帮会"
                        });
                    }
                }
            }
        }

        
        public void NotifyLongTaRoleDataList(LangHunLingYuScene scene)
        {
            foreach (CopyMap copyMap in scene.CopyMapDict.Values)
            {
                GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<BangHuiRoleCountData>>(1150, scene.LongTaBHRoleCountList, copyMap);
            }
        }

        
        public void NotifyLongTaOwnerData(LangHunLingYuScene scene)
        {
            foreach (CopyMap copyMap in scene.CopyMapDict.Values)
            {
                GameManager.ClientMgr.BroadSpecialCopyMapMessage<LangHunLingYuLongTaOwnerData>(1152, scene.LongTaOwnerData, copyMap);
            }
        }

        
        public void UpdateQiZhiBangHui(LangHunLingYuScene scene, int npcExtentionID, int bhid, string bhName, int zoneId)
        {
            int oldBHID = 0;
            int bufferID = 0;
            lock (this.RuntimeData.Mutex)
            {
                for (int i = 0; i < scene.QiZhiBuffOwnerDataList.Count; i++)
                {
                    if (scene.QiZhiBuffOwnerDataList[i].NPCID == npcExtentionID)
                    {
                        oldBHID = scene.QiZhiBuffOwnerDataList[i].OwnerBHID;
                        scene.QiZhiBuffOwnerDataList[i].OwnerBHID = bhid;
                        scene.QiZhiBuffOwnerDataList[i].OwnerBHName = bhName;
                        scene.QiZhiBuffOwnerDataList[i].OwnerBHZoneId = zoneId;
                        break;
                    }
                }
                QiZhiConfig qiZhiConfig;
                if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out qiZhiConfig))
                {
                    bufferID = qiZhiConfig.BufferID;
                }
            }
            if (bhid != oldBHID)
            {
                if (npcExtentionID == this.RuntimeData.SuperQiZhiNpcId)
                {
                    scene.SuperQiZhiOwnerBhid = bhid;
                }
                try
                {
                    EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferID);
                    if (null != item)
                    {
                        foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                        {
                            List<GameClient> clientList = copyMap.GetClientsList();
                            for (int i = 0; i < clientList.Count; i++)
                            {
                                GameClient c = clientList[i];
                                if (c != null)
                                {
                                    bool add = false;
                                    if (c.ClientData.Faction == oldBHID)
                                    {
                                        add = false;
                                    }
                                    else if (c.ClientData.Faction == bhid)
                                    {
                                        add = true;
                                    }
                                    this.UpdateQiZhiBuff4GameClient(c, item, bufferID, add);
                                }
                            }
                        }
                    }
                    this.NotifyQiZhiBuffOwnerDataList(scene);
                }
                catch (Exception ex)
                {
                    LogManager.WriteException("旗帜状态变化,设置旗帜Buff时发生异常:" + ex.ToString());
                }
            }
        }

        
        private void UpdateQiZhiBuff4GameClient(GameClient client, EquipPropItem item, int bufferID, bool add)
        {
            try
            {
                if (add && null != item)
                {
                    client.ClientData.PropsCacheManager.SetExtProps(new object[]
                    {
                        PropsSystemTypes.BufferByGoodsProps,
                        bufferID,
                        item.ExtProps
                    });
                    Global.UpdateBufferData(client, (BufferItemTypes)bufferID, this.RuntimeData.QiZhiBuffEnableParamsDict[bufferID], 1, true);
                }
                else
                {
                    client.ClientData.PropsCacheManager.SetExtProps(new object[]
                    {
                        PropsSystemTypes.BufferByGoodsProps,
                        bufferID,
                        PropsCacheManager.ConstExtProps
                    });
                    Global.UpdateBufferData(client, (BufferItemTypes)bufferID, this.RuntimeData.QiZhiBuffDisableParamsDict[bufferID], 1, true);
                }
                GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
            }
            catch (Exception ex)
            {
                LogManager.WriteException(ex.ToString());
            }
        }

        
        public void NotifyQiZhiBuffOwnerDataList(LangHunLingYuScene scene)
        {
            lock (this.RuntimeData.Mutex)
            {
                byte[] bytes = DataHelper.ObjectToBytes<List<LangHunLingYuQiZhiBuffOwnerData>>(scene.QiZhiBuffOwnerDataList);
            }
            foreach (CopyMap copyMap in scene.CopyMapDict.Values)
            {
                GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<LangHunLingYuQiZhiBuffOwnerData>>(1151, scene.QiZhiBuffOwnerDataList, copyMap);
            }
        }

        
        private void HandleHuangChengResultEx(LangHunLingYuScene scene, bool isBattleOver = false)
        {
            int bhid = scene.LongTaOwnerData.OwnerBHid;
            string bhName = scene.LongTaOwnerData.OwnerBHName;
            if (isBattleOver)
            {
                if (bhid <= 0)
                {
                    string broadCastMsg = StringUtil.substitute(GLang.GetLang(405, new object[0]), new object[0]);
                    foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                    {
                        GameManager.ClientMgr.BroadSpecialCopyMapMsg(copyMap, broadCastMsg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
                    }
                    return;
                }
            }
            if (scene.LastTheOnlyOneBangHui > 0)
            {
                string broadCastMsg;
                if (!isBattleOver)
                {
                    long nSecond = (scene.m_lEndTime - TimeUtil.NOW()) / 1000L;
                    broadCastMsg = StringUtil.substitute(GLang.GetLang(406, new object[0]), new object[]
                    {
                        bhName,
                        nSecond / 60L,
                        nSecond % 60L
                    });
                }
                else
                {
                    broadCastMsg = StringUtil.substitute(GLang.GetLang(671, new object[0]), new object[]
                    {
                        bhName
                    });
                }
                foreach (CopyMap copyMap in scene.CopyMapDict.Values)
                {
                    GameManager.ClientMgr.BroadSpecialCopyMapMsg(copyMap, broadCastMsg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
                }
            }
        }

        
        public const SceneUIClasses ManagerType = SceneUIClasses.LangHunLingYu;

        
        private static LangHunLingYuManager instance = new LangHunLingYuManager();

        
        public LangHunLingYuData RuntimeData = new LangHunLingYuData();

        
        public LevelAwardsMgr _LevelAwardsMgr = new LevelAwardsMgr();

        
        private object OwnerRoleDataMutex = new object();

        
        private RoleDataEx _OwnerRoleData = null;
    }
}
