using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Interface;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
    // Token: 0x020002A2 RID: 674
    public class EraManager : IManager, ICmdProcessorEx, ICmdProcessor
    {
        // Token: 0x060009FA RID: 2554 RVA: 0x0009E980 File Offset: 0x0009CB80
        public static EraManager getInstance()
        {
            return EraManager.instance;
        }

        // Token: 0x060009FB RID: 2555 RVA: 0x0009E998 File Offset: 0x0009CB98
        public bool initialize()
        {
            return this.InitConfig();
        }

        // Token: 0x060009FC RID: 2556 RVA: 0x0009E9BC File Offset: 0x0009CBBC
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(1090, 1, 1, EraManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1091, 2, 2, EraManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1092, 2, 2, EraManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            return true;
        }

        // Token: 0x060009FD RID: 2557 RVA: 0x0009EA18 File Offset: 0x0009CC18
        public bool showdown()
        {
            return true;
        }

        // Token: 0x060009FE RID: 2558 RVA: 0x0009EA2C File Offset: 0x0009CC2C
        public bool destroy()
        {
            return true;
        }

        // Token: 0x060009FF RID: 2559 RVA: 0x0009EA40 File Offset: 0x0009CC40
        public bool processCmd(GameClient client, string[] cmdParams)
        {
            return false;
        }

        // Token: 0x06000A00 RID: 2560 RVA: 0x0009EA54 File Offset: 0x0009CC54
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            bool result;
            if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.JunTuan, false) || !GlobalNew.IsGongNengOpened(client, GongNengIDs.Era, false))
            {
                GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
                result = true;
            }
            else
            {
                switch (nID)
                {
                    case 1090:
                        result = this.ProcessGetEraDataCmd(client, nID, bytes, cmdParams);
                        break;
                    case 1091:
                        result = this.ProcessEraDonateCmd(client, nID, bytes, cmdParams);
                        break;
                    case 1092:
                        result = this.ProcessEraAwardCmd(client, nID, bytes, cmdParams);
                        break;
                    default:
                        result = true;
                        break;
                }
            }
            return result;
        }

        // Token: 0x06000A01 RID: 2561 RVA: 0x0009EB10 File Offset: 0x0009CD10
        public void OnLogin(GameClient client)
        {
            if (JunTuanClient.getInstance().GetCurrentEraID() <= 0)
            {
                string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                {
                    13,
                    0,
                    "",
                    0,
                    0
                });
                client.sendCmd(770, strcmd, false);
            }
            else
            {
                string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                {
                    13,
                    1,
                    "",
                    0,
                    0
                });
                client.sendCmd(770, strcmd, false);
            }
        }

        // Token: 0x06000A02 RID: 2562 RVA: 0x0009EBD4 File Offset: 0x0009CDD4
        public bool InitConfig()
        {
            return this.LoadEraUIConfigFile() && this.LoadEraTaskConfigFile() && this.LoadEraRewardConfigFile() && this.LoadEraDropConfigFile() && this.LoadEraContributionConfigFile();
        }

        // Token: 0x06000A03 RID: 2563 RVA: 0x0009EC30 File Offset: 0x0009CE30
        public bool LoadEraUIConfigFile()
        {
            try
            {
                GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraUI.xml"));
                XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraUI.xml"));
                if (null == xml)
                {
                    return false;
                }
                Dictionary<int, EraUIConfig> tempEraUIConfigDict = new Dictionary<int, EraUIConfig>();
                IEnumerable<XElement> xmlItems = xml.Elements();
                foreach (XElement xmlItem in xmlItems)
                {
                    EraUIConfig data = new EraUIConfig();
                    data.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
                    data.EraID = (int)Global.GetSafeAttributeLong(xmlItem, "EraID");
                    DateTime.TryParse(Global.GetSafeAttributeStr(xmlItem, "StartTime"), out data.StartTime);
                    DateTime.TryParse(Global.GetSafeAttributeStr(xmlItem, "EndTime"), out data.EndTime);
                    tempEraUIConfigDict[data.ID] = data;
                }
                lock (this.ConfigMutex)
                {
                    this.EraUIConfigDict = tempEraUIConfigDict;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/EraUI.xml", ex.Message), null, true);
                return false;
            }
            return true;
        }

        // Token: 0x06000A04 RID: 2564 RVA: 0x0009EDE4 File Offset: 0x0009CFE4
        public bool LoadEraTaskConfigFile()
        {
            try
            {
                GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraTask.xml"));
                XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraTask.xml"));
                if (null == xml)
                {
                    return false;
                }
                Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>> tempEraTaskConfigDict = new Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>>();
                IEnumerable<XElement> xmlItems = xml.Elements();
                foreach (XElement xmlItem in xmlItems)
                {
                    EraTaskConfig data = new EraTaskConfig();
                    data.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
                    data.EraID = (int)Global.GetSafeAttributeLong(xmlItem, "EraID");
                    data.EraStage = (int)Global.GetSafeAttributeLong(xmlItem, "EraStage");
                    KeyValuePair<int, int> kvp = new KeyValuePair<int, int>(data.EraID, data.EraStage);
                    List<EraTaskConfig> taskConfigList = null;
                    if (!tempEraTaskConfigDict.TryGetValue(kvp, out taskConfigList))
                    {
                        taskConfigList = new List<EraTaskConfig>();
                        tempEraTaskConfigDict[kvp] = taskConfigList;
                    }
                    data.Reward = (int)Global.GetSafeAttributeLong(xmlItem, "Reward");
                    string tempValue = Global.GetSafeAttributeStr(xmlItem, "CompletionCondition");
                    string[] tempFields = tempValue.Split(new char[]
                    {
                        '|'
                    });
                    foreach (string item in tempFields)
                    {
                        string[] strkvp = item.Split(new char[]
                        {
                            ','
                        });
                        if (strkvp.Length == 2)
                        {
                            int goodsid = Convert.ToInt32(strkvp[0]);
                            int num = Convert.ToInt32(strkvp[1]);
                            data.CompletionCondition.Add(new KeyValuePair<int, int>(goodsid, num));
                        }
                    }
                    taskConfigList.Add(data);
                }
                lock (this.ConfigMutex)
                {
                    this.EraTaskConfigDict = tempEraTaskConfigDict;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/EraTask.xml", ex.Message), null, true);
                return false;
            }
            return true;
        }

        // Token: 0x06000A05 RID: 2565 RVA: 0x0009F068 File Offset: 0x0009D268
        public bool LoadEraRewardConfigFile()
        {
            try
            {
                GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraReward.xml"));
                XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraReward.xml"));
                if (null == xml)
                {
                    return false;
                }
                Dictionary<int, EraAwardConfig> tempEraAwardConfigDict = new Dictionary<int, EraAwardConfig>();
                IEnumerable<XElement> xmlItems = xml.Elements();
                foreach (XElement xmlItem in xmlItems)
                {
                    EraAwardConfig data = new EraAwardConfig();
                    data.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
                    data.EraID = (int)Global.GetSafeAttributeLong(xmlItem, "EraID");
                    data.EraName = Global.GetSafeAttributeStr(xmlItem, "Name");
                    data.AwardType = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
                    string StartTime = Global.GetSafeAttributeStr(xmlItem, "StartTime");
                    if (!string.IsNullOrEmpty(StartTime))
                    {
                        DateTime.TryParse(StartTime, out data.StartTime);
                    }
                    string EndTime = Global.GetSafeAttributeStr(xmlItem, "EndTime");
                    if (!string.IsNullOrEmpty(EndTime))
                    {
                        DateTime.TryParse(EndTime, out data.EndTime);
                    }
                    data.EraRanking = (int)Global.GetSafeAttributeLong(xmlItem, "EraRanking");
                    data.Progress = (int)Global.GetSafeAttributeLong(xmlItem, "Progress");
                    data.Contribution = (int)Global.GetSafeAttributeLong(xmlItem, "Contribution");
                    ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xmlItem, "LeaderReward"), ref data.LeaderReward, '|', ',');
                    ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xmlItem, "MasterReward"), ref data.MasterReward, '|', ',');
                    ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xmlItem, "Reward"), ref data.Reward, '|', ',');
                    tempEraAwardConfigDict[data.ID] = data;
                }
                lock (this.ConfigMutex)
                {
                    this.EraAwardConfigDict = tempEraAwardConfigDict;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/EraReward.xml", ex.Message), null, true);
                return false;
            }
            return true;
        }

        // Token: 0x06000A06 RID: 2566 RVA: 0x0009F2F0 File Offset: 0x0009D4F0
        public bool LoadEraDropConfigFile()
        {
            try
            {
                GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraDrop.xml"));
                XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraDrop.xml"));
                if (null == xml)
                {
                    return false;
                }
                Dictionary<int, List<EraDropConfig>> tempEraDropConfigDict = new Dictionary<int, List<EraDropConfig>>();
                IEnumerable<XElement> xmlItems = xml.Elements();
                foreach (XElement xmlItem in xmlItems)
                {
                    int dropID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
                    List<EraDropConfig> dropList = null;
                    if (!tempEraDropConfigDict.TryGetValue(dropID, out dropList))
                    {
                        dropList = new List<EraDropConfig>();
                        tempEraDropConfigDict[dropID] = dropList;
                    }
                    EraDropConfig data = new EraDropConfig();
                    data.ID = dropID;
                    data.EraID = (int)Global.GetSafeAttributeLong(xmlItem, "EraID");
                    data.EraStage = (int)Global.GetSafeAttributeLong(xmlItem, "EraStage");
                    data.Fixedaward = Global.GetSafeAttributeStr(xmlItem, "Fixedaward");
                    data.MaxList = (int)Global.GetSafeAttributeLong(xmlItem, "MaxList");
                    int basePercent = 0;
                    FallGoodsItem fallGoodsItem = null;
                    data.fallGoodsItemList = new List<FallGoodsItem>();
                    string goodsData = Global.GetSafeAttributeStr(xmlItem, "GoodsID");
                    string[] goodsFields = goodsData.Split(new char[]
                    {
                        '|'
                    });
                    for (int i = 0; i < goodsFields.Length; i++)
                    {
                        string item = goodsFields[i].Trim();
                        if (!(item == ""))
                        {
                            string[] itemFields = item.Split(new char[]
                            {
                                ','
                            });
                            if (itemFields.Length == 7)
                            {
                                fallGoodsItem = null;
                                try
                                {
                                    fallGoodsItem = new FallGoodsItem
                                    {
                                        GoodsID = Convert.ToInt32(itemFields[0]),
                                        BasePercent = basePercent,
                                        SelfPercent = (int)(Convert.ToDouble(itemFields[1]) * 100000.0),
                                        Binding = Convert.ToInt32(itemFields[2]),
                                        LuckyRate = (int)Convert.ToDouble(itemFields[3]),
                                        FallLevelID = Convert.ToInt32(itemFields[4]),
                                        ZhuiJiaID = Convert.ToInt32(itemFields[5]),
                                        ExcellencePropertyID = Convert.ToInt32(itemFields[6])
                                    };
                                    basePercent += fallGoodsItem.SelfPercent;
                                }
                                catch (Exception)
                                {
                                    fallGoodsItem = null;
                                }
                                if (null == fallGoodsItem)
                                {
                                    LogManager.WriteLog(LogTypes.Error, string.Format("解析纪元掉落项时发生错误, GoodsPackID={0}, GoodsID={1}", data.ID, item), null, true);
                                }
                                else
                                {
                                    data.fallGoodsItemList.Add(fallGoodsItem);
                                }
                            }
                        }
                    }
                    dropList.Add(data);
                }
                lock (this.ConfigMutex)
                {
                    this.EraDropConfigDict = tempEraDropConfigDict;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/EraDrop.xml", ex.Message), null, true);
                return false;
            }
            return true;
        }

        // Token: 0x06000A07 RID: 2567 RVA: 0x0009F688 File Offset: 0x0009D888
        public bool LoadEraContributionConfigFile()
        {
            try
            {
                GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraContribution.xml"));
                XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraContribution.xml"));
                if (null == xml)
                {
                    return false;
                }
                List<EraContributionConfig> tempEraContributionConfigList = new List<EraContributionConfig>();
                IEnumerable<XElement> xmlItems = xml.Elements();
                foreach (XElement xmlItem in xmlItems)
                {
                    EraContributionConfig data = new EraContributionConfig();
                    data.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
                    data.EraID = (int)Global.GetSafeAttributeLong(xmlItem, "EraID");
                    data.ProgressID = (int)Global.GetSafeAttributeLong(xmlItem, "ProgressID");
                    data.GoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
                    data.Contribution = (int)Global.GetSafeAttributeLong(xmlItem, "Contribution");
                    int[] goodsFields = Global.GetSafeAttributeIntArray(xmlItem, "MonsterID", -1, ',');
                    foreach (int item in goodsFields)
                    {
                        data.MonsterIDSet.Add(item);
                    }
                    tempEraContributionConfigList.Add(data);
                }
                lock (this.ConfigMutex)
                {
                    this.EraContributionList = tempEraContributionConfigList;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/EraContribution.xml", ex.Message), null, true);
                return false;
            }
            return true;
        }

        // Token: 0x06000A08 RID: 2568 RVA: 0x0009F900 File Offset: 0x0009DB00
        public void EraTimer_Work()
        {
            if (!GameManager.IsKuaFuServer)
            {
                var curEraID = JunTuanClient.getInstance().GetCurrentEraID();
                if (curEraID > 0)
                {
                    int CurrentDayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
                    if (CurrentDayID != this.CheckRankAwardDayID && this.InRankAwardTime())
                    {
                        List<KFEraRankData> kfEraRankData = JunTuanClient.getInstance().GetJunTuanEraRankData(false);
                        if (null != kfEraRankData)
                        {
                            int RankAwardEraID = GameManager.GameConfigMgr.GetGameConfigItemInt("era_rank_award", 0);
                            if (curEraID != RankAwardEraID)
                            {
                                List<JunTuanBaseData> juntuanBaseList = JunTuanClient.getInstance().GetJunTuanBaseDataList(true);
                                using (List<JunTuanBaseData>.Enumerator enumerator = juntuanBaseList.GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        var baseData = enumerator.Current;
                                        if (baseData.BhList != null && baseData.BhList.Count != 0)
                                        {
                                            KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(baseData.JunTuanId, false);
                                            if (null == kfEraData)
                                            {
                                                LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 获取纪元数据失败", curEraID, baseData.JunTuanId), null, true);
                                            }
                                            else
                                            {
                                                int RankNum = kfEraRankData.FindIndex((KFEraRankData x) => x.JunTuanID == baseData.JunTuanId);
                                                RankNum = ((RankNum != -1) ? (RankNum + 1) : RankNum);
                                                Dictionary<int, EraAwardConfig> tempEraAwardConfigDict = null;
                                                lock (this.ConfigMutex)
                                                {
                                                    tempEraAwardConfigDict = this.EraAwardConfigDict;
                                                }
                                                List<EraAwardConfig> AwardsConfigList = tempEraAwardConfigDict.Values.ToList<EraAwardConfig>();
                                                EraAwardConfig awardConfig = AwardsConfigList.Find((EraAwardConfig x) => x.AwardType == 2 && x.EraID == curEraID && x.EraRanking == RankNum);
                                                if (null == awardConfig)
                                                {
                                                    LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 排行{2} 获取奖励配置失败", curEraID, baseData.JunTuanId, RankNum), null, true);
                                                }
                                                else
                                                {
                                                    if (awardConfig.Progress == 4)
                                                    {
                                                        if (kfEraData.EraStageProcess != 100)
                                                        {
                                                            LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 排行{2} 进度不满足要求", curEraID, baseData.JunTuanId, RankNum), null, true);
                                                            continue;
                                                        }
                                                    }
                                                    else if (awardConfig.Progress > 0 && awardConfig.Progress >= (int)kfEraData.EraStage)
                                                    {
                                                        LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 排行{2} 进度不满足要求", curEraID, baseData.JunTuanId, RankNum), null, true);
                                                        continue;
                                                    }
                                                    List<JunTuanRoleData> juntuanRoleList = JunTuanClient.getInstance().GetJunTuanRoleList(baseData.BhList[0], baseData.JunTuanId);
                                                    if (null == juntuanRoleList)
                                                    {
                                                        LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 排行{2} 获取juntuanRoleList失败", curEraID, baseData.JunTuanId, RankNum), null, true);
                                                    }
                                                    else
                                                    {
                                                        foreach (int bhid in baseData.BhList)
                                                        {
                                                            BangHuiDetailData bhData = Global.GetBangHuiDetailData(-1, bhid, 0);
                                                            if (null == bhData)
                                                            {
                                                                LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 排行{2} 战盟{3} 获取bhData失败", new object[]
                                                                {
                                                                    curEraID,
                                                                    baseData.JunTuanId,
                                                                    RankNum,
                                                                    bhid
                                                                }), null, true);
                                                            }
                                                            else
                                                            {
                                                                foreach (JunTuanRoleData bhRole in juntuanRoleList)
                                                                {
                                                                    if (bhRole.BhId != bhid)
                                                                    {
                                                                        LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 排行{2} 战盟{3} 角色{4} bhRole.BhId != bhid失败", new object[]
                                                                        {
                                                                            curEraID,
                                                                            baseData.JunTuanId,
                                                                            RankNum,
                                                                            bhid,
                                                                            bhRole.RoleId
                                                                        }), null, true);
                                                                    }
                                                                    else
                                                                    {
                                                                        int eraDonate = GameManager.ClientMgr.GetEraDonateValueOffline(bhRole.RoleId);
                                                                        if (awardConfig.Contribution > 0 && eraDonate < awardConfig.Contribution)
                                                                        {
                                                                            LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 排行{2} 战盟{3} 角色{4} 贡献{5} 贡献度不满足要求", new object[]
                                                                            {
                                                                                curEraID,
                                                                                baseData.JunTuanId,
                                                                                RankNum,
                                                                                bhid,
                                                                                bhRole.RoleId,
                                                                                eraDonate
                                                                            }), null, true);
                                                                        }
                                                                        else
                                                                        {
                                                                            LogManager.WriteLog(LogTypes.Error, string.Format("纪元{0}奖励 军团{1} 排行{2} 战盟{3} 角色{4} 贡献{5} 成功！", new object[]
                                                                            {
                                                                                curEraID,
                                                                                baseData.JunTuanId,
                                                                                RankNum,
                                                                                bhid,
                                                                                bhRole.RoleId,
                                                                                eraDonate
                                                                            }), null, true);
                                                                            string sContent = (RankNum != -1) ? string.Format(GLang.GetLang(109, new object[0]), awardConfig.EraName, RankNum) : string.Format(GLang.GetLang(110, new object[0]), awardConfig.EraName);
                                                                            if (bhRole.JuTuanZhiWu == 1)
                                                                            {
                                                                                List<GoodsData> AwardGoods = Global.ConvertToGoodsDataList(awardConfig.LeaderReward.Items, -1);
                                                                                Global.UseMailGivePlayerAward3(bhRole.RoleId, AwardGoods, GLang.GetLang(111, new object[0]), sContent, 0, 0, 0);
                                                                            }
                                                                            else if (bhRole.JuTuanZhiWu == 2)
                                                                            {
                                                                                List<GoodsData> AwardGoods = Global.ConvertToGoodsDataList(awardConfig.MasterReward.Items, -1);
                                                                                Global.UseMailGivePlayerAward3(bhRole.RoleId, AwardGoods, GLang.GetLang(111, new object[0]), sContent, 0, 0, 0);
                                                                            }
                                                                            else
                                                                            {
                                                                                List<GoodsData> AwardGoods = Global.ConvertToGoodsDataList(awardConfig.Reward.Items, -1);
                                                                                Global.UseMailGivePlayerAward3(bhRole.RoleId, AwardGoods, GLang.GetLang(111, new object[0]), sContent, 0, 0, 0);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                this.CheckRankAwardDayID = CurrentDayID;
                                GameManager.GameConfigMgr.SetGameConfigItem("era_rank_award", curEraID.ToString());
                                Global.UpdateDBGameConfigg("era_rank_award", curEraID.ToString());
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000A09 RID: 2569 RVA: 0x000A0174 File Offset: 0x0009E374
        private bool InRankAwardTime()
        {
            int curEraID = JunTuanClient.getInstance().GetCurrentEraID();
            bool result;
            if (curEraID <= 0)
            {
                result = false;
            }
            else
            {
                Dictionary<int, EraAwardConfig> tempEraAwardConfigDict = null;
                lock (this.ConfigMutex)
                {
                    tempEraAwardConfigDict = this.EraAwardConfigDict;
                }
                foreach (EraAwardConfig item in tempEraAwardConfigDict.Values)
                {
                    if (item.EraID == curEraID && item.AwardType == 2)
                    {
                        DateTime now = TimeUtil.NowDateTime();
                        if (now >= item.StartTime && now <= item.EndTime)
                        {
                            return true;
                        }
                    }
                }
                result = false;
            }
            return result;
        }

        // Token: 0x06000A0A RID: 2570 RVA: 0x000A0284 File Offset: 0x0009E484
        private int CheckCanGetAward(GameClient client, EraAwardConfig awardConfig, KFEraData kfEraData)
        {
            int result;
            if (null == kfEraData)
            {
                result = -11003;
            }
            else
            {
                List<int> AwardStateList = this.GetEraAwardStateData(client);
                if (AwardStateList == null || AwardStateList.Count == 0)
                {
                    result = -11003;
                }
                else
                {
                    Dictionary<int, int> EraAwardStateDict = new Dictionary<int, int>();
                    for (int idx = 1; idx < AwardStateList.Count - 1; idx += 2)
                    {
                        int key = AwardStateList[idx];
                        int value = AwardStateList[idx + 1];
                        EraAwardStateDict.Add(key, value);
                    }
                    int awardState = 0;
                    if (EraAwardStateDict.TryGetValue(awardConfig.ID, out awardState))
                    {
                        result = -200;
                    }
                    else if (awardConfig.Contribution > 0 && GameManager.ClientMgr.GetEraDonateValue(client) < awardConfig.Contribution)
                    {
                        result = -12;
                    }
                    else if (awardConfig.EraID != kfEraData.EraID)
                    {
                        result = -12;
                    }
                    else
                    {
                        if (awardConfig.Progress == 4)
                        {
                            if (kfEraData.EraStageProcess != 100)
                            {
                                return -12;
                            }
                        }
                        else if (awardConfig.Progress > 0 && awardConfig.Progress >= (int)kfEraData.EraStage)
                        {
                            return -12;
                        }
                        if (awardConfig.AwardType == 1)
                        {
                            if (0 < awardConfig.Progress && awardConfig.Progress <= kfEraData.EraTimePointList.Count)
                            {
                                int EnterBHUnixSecs = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
                                DateTime enterBHTm = new DateTime(DataHelper.UnixSecondsToTicks(EnterBHUnixSecs) * 10000L);
                                DateTime enterJTTm = Global.GetRoleParamsDateTimeFromDB(client, "10182");
                                DateTime stageTm = kfEraData.EraTimePointList[awardConfig.Progress - 1];
                                if (enterBHTm > stageTm || enterJTTm > stageTm)
                                {
                                    return -2006;
                                }
                            }
                        }
                        else
                        {
                            if (awardConfig.AwardType == 2)
                            {
                                return -12;
                            }
                            if (awardConfig.AwardType == 3)
                            {
                            }
                        }
                        result = 0;
                    }
                }
            }
            return result;
        }

        // Token: 0x06000A0B RID: 2571 RVA: 0x000A04CC File Offset: 0x0009E6CC
        public void OnChangeEraID(int CurrentJunTuanEraID)
        {
            foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
            {
                if (client.ClientData.Faction > 0 && client.ClientData.JunTuanId > 0)
                {
                    int donateVal = GameManager.ClientMgr.GetEraDonateValue(client);
                    GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.EraDonate, donateVal);
                }
            }
            if (0 == CurrentJunTuanEraID)
            {
                GameManager.ClientMgr.NotifyAllActivityState(13, 0, "", "", 0);
            }
            else
            {
                GameManager.ClientMgr.NotifyAllActivityState(13, 1, "", "", 0);
            }
        }

        // Token: 0x06000A0C RID: 2572 RVA: 0x000A05A8 File Offset: 0x0009E7A8
        public void CheckAllJunTuanEraIcon(int juntuanID)
        {
            foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
            {
                if (client.ClientData.JunTuanId == juntuanID)
                {
                    if (client._IconStateMgr.CheckJunTuanEraIcon(client))
                    {
                        client._IconStateMgr.SendIconStateToClient(client);
                    }
                }
            }
        }

        // Token: 0x06000A0D RID: 2573 RVA: 0x000A0634 File Offset: 0x0009E834
        public void OnJunTuanZhiWuChanged(GameClient client)
        {
            int donateVal = GameManager.ClientMgr.GetEraDonateValue(client);
            GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.EraDonate, donateVal);
        }

        // Token: 0x06000A0E RID: 2574 RVA: 0x000A0660 File Offset: 0x0009E860
        public bool CheckJunTuanEraIcon(GameClient client)
        {
            bool result2;
            if (0 == client.ClientData.JunTuanId)
            {
                result2 = false;
            }
            else
            {
                KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, true);
                if (null != kfEraData)
                {
                    Dictionary<int, EraAwardConfig> tempEraAwardConfigDict = null;
                    lock (this.ConfigMutex)
                    {
                        tempEraAwardConfigDict = this.EraAwardConfigDict;
                    }
                    foreach (EraAwardConfig item in tempEraAwardConfigDict.Values)
                    {
                        if (item.EraID == JunTuanClient.getInstance().GetCurrentEraID())
                        {
                            int result = this.CheckCanGetAward(client, item, kfEraData);
                            if (result == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
                result2 = false;
            }
            return result2;
        }

        // Token: 0x06000A0F RID: 2575 RVA: 0x000A077C File Offset: 0x0009E97C
        public List<int> GetEraAwardStateData(GameClient client)
        {
            int curEraID = JunTuanClient.getInstance().GetCurrentEraID();
            List<int> result;
            if (curEraID <= 0)
            {
                result = null;
            }
            else
            {
                List<int> countList = Global.GetRoleParamsIntListFromDB(client, "45");
                if (countList.Count < 1)
                {
                    for (int i = countList.Count; i < 1; i++)
                    {
                        countList.Add(0);
                    }
                    countList[0] = curEraID;
                }
                if (countList[0] != curEraID)
                {
                    countList.Clear();
                    for (int i = countList.Count; i < 1; i++)
                    {
                        countList.Add(0);
                    }
                    countList[0] = curEraID;
                }
                result = countList;
            }
            return result;
        }

        // Token: 0x06000A10 RID: 2576 RVA: 0x000A0834 File Offset: 0x0009EA34
        private void SaveEraAwardStateData(GameClient client, List<int> countList)
        {
            Global.SaveRoleParamsIntListToDB(client, countList, "45", true);
        }

        // Token: 0x06000A11 RID: 2577 RVA: 0x000A088C File Offset: 0x0009EA8C
        public int GetEraFallGoodsMaxCount(IObject attacker, int goodsPackID)
        {
            int result;
            if (!(attacker is GameClient))
            {
                result = 0;
            }
            else if ((attacker as GameClient).ClientData.JunTuanId == 0)
            {
                result = 0;
            }
            else
            {
                Dictionary<int, List<EraDropConfig>> tempEraDropConfigDict = null;
                lock (this.ConfigMutex)
                {
                    tempEraDropConfigDict = this.EraDropConfigDict;
                }
                List<EraDropConfig> dropConfigList = null;
                if (!tempEraDropConfigDict.TryGetValue(goodsPackID, out dropConfigList))
                {
                    result = 0;
                }
                else
                {
                    KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData((attacker as GameClient).ClientData.JunTuanId, false);
                    if (null == kfEraData)
                    {
                        result = 0;
                    }
                    else
                    {
                        EraDropConfig dropConfig = dropConfigList.Find((EraDropConfig x) => x.EraID == kfEraData.EraID && x.EraStage == (int)kfEraData.EraStage);
                        if (null == dropConfig)
                        {
                            result = 0;
                        }
                        else
                        {
                            result = dropConfig.MaxList;
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x06000A12 RID: 2578 RVA: 0x000A09E8 File Offset: 0x0009EBE8
        public List<FallGoodsItem> GetEraFallGoodsItem(GameClient client, int goodsPackID)
        {
            List<FallGoodsItem> result;
            if (client.ClientData.JunTuanId == 0)
            {
                result = null;
            }
            else
            {
                Dictionary<int, List<EraDropConfig>> tempEraDropConfigDict = null;
                lock (this.ConfigMutex)
                {
                    tempEraDropConfigDict = this.EraDropConfigDict;
                }
                List<EraDropConfig> dropConfigList = null;
                if (!tempEraDropConfigDict.TryGetValue(goodsPackID, out dropConfigList))
                {
                    result = null;
                }
                else
                {
                    KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
                    if (null == kfEraData)
                    {
                        result = null;
                    }
                    else
                    {
                        EraDropConfig dropConfig = dropConfigList.Find((EraDropConfig x) => x.EraID == kfEraData.EraID && x.EraStage == (int)kfEraData.EraStage);
                        if (null == dropConfig)
                        {
                            result = null;
                        }
                        else
                        {
                            result = dropConfig.fallGoodsItemList;
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x06000A13 RID: 2579 RVA: 0x000A0AE0 File Offset: 0x0009ECE0
        private EraData BuildEraData4Client(GameClient client)
        {
            KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
            List<KFEraRankData> kfEraRankData = JunTuanClient.getInstance().GetJunTuanEraRankData(false);
            EraData eraData4Client = new EraData();
            if (null != kfEraData)
            {
                eraData4Client.EraID = kfEraData.EraID;
                eraData4Client.EraStage = kfEraData.EraStage;
                eraData4Client.EraStageProcess = kfEraData.EraStageProcess;
                eraData4Client.FastEraStage = kfEraData.FastEraStage;
                eraData4Client.FastEraStateProcess = kfEraData.FastEraStateProcess;
                eraData4Client.EraTaskList = kfEraData.EraTaskList;
            }
            if (null != kfEraRankData)
            {
                foreach (KFEraRankData item in kfEraRankData)
                {
                    EraRankData rankData = new EraRankData();
                    rankData.RankValue = item.RankValue;
                    rankData.JunTuanID = item.JunTuanID;
                    JunTuanBaseData juntuanBaseData = JunTuanManager.getInstance().GetJunTuanBaseDataByJunTuanID(item.JunTuanID);
                    rankData.JunTuanName = ((juntuanBaseData == null) ? "" : juntuanBaseData.JunTuanName);
                    rankData.EraStage = item.EraStage;
                    rankData.EraStageProcess = item.EraStageProcess;
                    eraData4Client.EraRankList.Add(rankData);
                }
            }
            Dictionary<int, EraAwardConfig> tempEraAwardConfigDict = null;
            lock (this.ConfigMutex)
            {
                tempEraAwardConfigDict = this.EraAwardConfigDict;
            }
            foreach (EraAwardConfig item2 in tempEraAwardConfigDict.Values)
            {
                if (item2.EraID == JunTuanClient.getInstance().GetCurrentEraID())
                {
                    int result = this.CheckCanGetAward(client, item2, kfEraData);
                    if (result != 0)
                    {
                        if (result == -200)
                        {
                            eraData4Client.EraAwardStateDict[item2.ID] = 1;
                        }
                        else
                        {
                            eraData4Client.EraAwardStateDict[item2.ID] = 2;
                        }
                    }
                }
            }
            return eraData4Client;
        }

        // Token: 0x06000A14 RID: 2580 RVA: 0x000A0D40 File Offset: 0x0009EF40
        public bool ProcessGetEraDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int roleID = Convert.ToInt32(cmdParams[0]);
                if (client.ClientData.JunTuanId == 0 || JunTuanClient.getInstance().GetCurrentEraID() <= 0)
                {
                    return true;
                }
                client.sendCmd<EraData>(nID, this.BuildEraData4Client(client), false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000A15 RID: 2581 RVA: 0x000A0E2C File Offset: 0x0009F02C
        public bool ProcessEraDonateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                int roleID = Convert.ToInt32(cmdParams[0]);
                int donateStage = Convert.ToInt32(cmdParams[1]);
                if (client.ClientData.JunTuanId == 0 || JunTuanClient.getInstance().GetCurrentEraID() <= 0)
                {
                    return true;
                }
                if (this.InRankAwardTime())
                {
                    result = -2001;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, 0, 0), false);
                    return true;
                }
                List<EraContributionConfig> tempEraContributionList = null;
                Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>> tempEraTaskConfigDict = null;
                lock (this.ConfigMutex)
                {
                    tempEraContributionList = this.EraContributionList;
                    tempEraTaskConfigDict = this.EraTaskConfigDict;
                }
                KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
                if (null == kfEraData)
                {
                    result = -11003;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, 0, 0), false);
                    return true;
                }
                KeyValuePair<int, int> kvp = new KeyValuePair<int, int>(kfEraData.EraID, donateStage);
                List<EraTaskConfig> taskConfigList = null;
                if (!tempEraTaskConfigDict.TryGetValue(kvp, out taskConfigList))
                {
                    result = -3;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, 0, 0), false);
                    return true;
                }
                if ((int)kfEraData.EraStage < donateStage)
                {
                    result = -12;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, 0, 0), false);
                    return true;
                }
                Dictionary<int, int> GoodsIDVsBagNumDict = new Dictionary<int, int>();
                foreach (EraTaskConfig taskConfig in taskConfigList)
                {
                    foreach (KeyValuePair<int, int> item4 in taskConfig.CompletionCondition)
                    {
                        int goodsNum = 0;
                        if (!GoodsIDVsBagNumDict.TryGetValue(item4.Key, out goodsNum))
                        {
                            int value = Global.GetTotalGoodsCountByID(client, item4.Key);
                            GoodsIDVsBagNumDict.Add(item4.Key, value);
                        }
                    }
                }
                foreach (EraTaskConfig taskConfig in taskConfigList)
                {
                    List<KeyValuePair<int, int>> GoodsIDVsNumList = new List<KeyValuePair<int, int>>();
                    foreach (KeyValuePair<int, int> item4 in taskConfig.CompletionCondition)
                    {
                        int goodsNum = 0;
                        GoodsIDVsBagNumDict.TryGetValue(item4.Key, out goodsNum);
                        GoodsIDVsNumList.Add(new KeyValuePair<int, int>(item4.Key, goodsNum));
                    }
                    int var = GoodsIDVsNumList[0].Value;
                    int var2 = GoodsIDVsNumList[1].Value;
                    int var3 = GoodsIDVsNumList[2].Value;
                    if (var != 0 || var2 != 0 || 0 != var3)
                    {
                        if (!JunTuanClient.getInstance().EraDonate(client.ClientData.JunTuanId, taskConfig.ID, var, var2, var3))
                        {
                            result = -11003;
                            client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, 0, 0), false);
                            return true;
                        }
                        int addDonateValue = 0;
                        using (List<KeyValuePair<int, int>>.Enumerator enumerator2 = GoodsIDVsNumList.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                KeyValuePair<int, int> item = enumerator2.Current;
                                int num = 0;
                                KeyValuePair<int, int> item2 = item;
                                if (num != item2.Value)
                                {
                                    bool usedBinding = false;
                                    bool usedTimeLimited = false;
                                    ClientManager clientMgr = GameManager.ClientMgr;
                                    SocketListener mySocketListener = Global._TCPManager.MySocketListener;
                                    TCPClientPool tcpClientPool = Global._TCPManager.tcpClientPool;
                                    TCPOutPacketPool tcpOutPacketPool = Global._TCPManager.TcpOutPacketPool;
                                    item2 = item;
                                    int key = item2.Key;
                                    item2 = item;
                                    if (clientMgr.NotifyUseGoods(mySocketListener, tcpClientPool, tcpOutPacketPool, client, key, item2.Value, false, out usedBinding, out usedTimeLimited, false))
                                    {
                                        EraContributionConfig donateConfig = tempEraContributionList.Find(delegate (EraContributionConfig x)
                                        {
                                            bool result2;
                                            if (x.EraID == kfEraData.EraID && x.ProgressID == donateStage)
                                            {
                                                int goodsID = x.GoodsID;
                                                KeyValuePair<int, int> item3 = item;
                                                result2 = (goodsID == item3.Key);
                                            }
                                            else
                                            {
                                                result2 = false;
                                            }
                                            return result2;
                                        });
                                        if (null != donateConfig)
                                        {
                                            int num2 = addDonateValue;
                                            int contribution = donateConfig.Contribution;
                                            item2 = item;
                                            addDonateValue = num2 + contribution * item2.Value;
                                        }
                                    }
                                }
                            }
                        }
                        GameManager.ClientMgr.ModifyEraDonateValue(client, addDonateValue, "捐献", true, true, false);
                    }
                }
                kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
                if (null == kfEraData)
                {
                    result = -11003;
                    client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, 0, 0), false);
                    return true;
                }
                client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, kfEraData.EraStage, kfEraData.EraStageProcess), false);
                client.sendCmd<EraData>(1090, this.BuildEraData4Client(client), false);
                if (client._IconStateMgr.CheckJunTuanEraIcon(client))
                {
                    client._IconStateMgr.SendIconStateToClient(client);
                }
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x06000A16 RID: 2582 RVA: 0x000A1518 File Offset: 0x0009F718
        public bool ProcessEraAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                int result = 0;
                int roleID = Convert.ToInt32(cmdParams[0]);
                int awardID = Convert.ToInt32(cmdParams[1]);
                if (client.ClientData.JunTuanId == 0 || JunTuanClient.getInstance().GetCurrentEraID() <= 0)
                {
                    return true;
                }
                Dictionary<int, EraAwardConfig> tempEraAwardConfigDict = null;
                lock (this.ConfigMutex)
                {
                    tempEraAwardConfigDict = this.EraAwardConfigDict;
                }
                EraAwardConfig awardConfig = null;
                if (!tempEraAwardConfigDict.TryGetValue(awardID, out awardConfig))
                {
                    result = -3;
                    client.sendCmd(nID, string.Format("{0}:{1}", result, awardID), false);
                    return true;
                }
                if (awardConfig.AwardType == 2)
                {
                    result = -12;
                    client.sendCmd(nID, string.Format("{0}:{1}", result, awardID), false);
                    return true;
                }
                KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
                result = this.CheckCanGetAward(client, awardConfig, kfEraData);
                if (result != 0)
                {
                    client.sendCmd(nID, string.Format("{0}:{1}", result, awardID), false);
                    return true;
                }
                List<AwardsItemData> awardsItemDataList = awardConfig.LeaderReward.Items;
                if (awardsItemDataList != null && !Global.CanAddGoodsNum(client, awardsItemDataList.Count))
                {
                    result = -100;
                    client.sendCmd(nID, string.Format("{0}:{1}", result, awardID), false);
                    return true;
                }
                if (awardsItemDataList != null)
                {
                    foreach (AwardsItemData item in awardsItemDataList)
                    {
                        Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "军团纪元", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
                    }
                }
                List<int> AwardStateList = this.GetEraAwardStateData(client);
                if (null != AwardStateList)
                {
                    if (awardConfig.AwardType == 1 || awardConfig.AwardType == 3)
                    {
                        AwardStateList.Add(awardID);
                        AwardStateList.Add(1);
                    }
                    this.SaveEraAwardStateData(client, AwardStateList);
                }
                client.sendCmd(nID, string.Format("{0}:{1}", result, awardID), false);
                if (client._IconStateMgr.CheckJunTuanEraIcon(client))
                {
                    client._IconStateMgr.SendIconStateToClient(client);
                }
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        // Token: 0x04001094 RID: 4244
        private const string EraUI_FileName = "Config/EraUI.xml";

        // Token: 0x04001095 RID: 4245
        private const string EraTask_FileName = "Config/EraTask.xml";

        // Token: 0x04001096 RID: 4246
        private const string EraReward_FileName = "Config/EraReward.xml";

        // Token: 0x04001097 RID: 4247
        private const string EraDrop_FileName = "Config/EraDrop.xml";

        // Token: 0x04001098 RID: 4248
        private const string EraContribution_FileName = "Config/EraContribution.xml";

        // Token: 0x04001099 RID: 4249
        private object ConfigMutex = new object();

        // Token: 0x0400109A RID: 4250
        public Dictionary<int, EraUIConfig> EraUIConfigDict = new Dictionary<int, EraUIConfig>();

        // Token: 0x0400109B RID: 4251
        public Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>> EraTaskConfigDict = new Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>>();

        // Token: 0x0400109C RID: 4252
        public Dictionary<int, EraAwardConfig> EraAwardConfigDict = new Dictionary<int, EraAwardConfig>();

        // Token: 0x0400109D RID: 4253
        public Dictionary<int, List<EraDropConfig>> EraDropConfigDict = new Dictionary<int, List<EraDropConfig>>();

        // Token: 0x0400109E RID: 4254
        public List<EraContributionConfig> EraContributionList = new List<EraContributionConfig>();

        // Token: 0x0400109F RID: 4255
        private int CheckRankAwardDayID;

        // Token: 0x040010A0 RID: 4256
        private static EraManager instance = new EraManager();
    }
}
