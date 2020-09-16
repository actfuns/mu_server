using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
    
    public class ArtifactManager : IManager
    {
        
        public static ArtifactManager GetInstance()
        {
            return ArtifactManager.Instance;
        }

        
        public bool initialize()
        {
            return true;
        }

        
        public bool startup()
        {
            return true;
        }

        
        public bool showdown()
        {
            return true;
        }

        
        public bool destroy()
        {
            return true;
        }

        
        public static void initArtifact()
        {
            ArtifactManager.LoadArtifactData();
            ArtifactManager.LoadArtifactSuitData();
        }

        
        public static void LoadArtifactData()
        {
            string fileName = "Config/ZaiZao.xml";
            GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
            XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
            if (null == xml)
            {
                LogManager.WriteLog(LogTypes.Fatal, "加载Config/ZaiZao.xml时出错!!!文件不存在", null, true);
            }
            else
            {
                try
                {
                    ArtifactManager._artifactList.Clear();
                    IEnumerable<XElement> xmlItems = xml.Elements();
                    foreach (XElement xmlItem in xmlItems)
                    {
                        if (xmlItem != null)
                        {
                            ArtifactData config = new ArtifactData();
                            config.ArtifactID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
                            config.ArtifactName = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "Name", ""));
                            config.NewEquitID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NewEquitID", "0"));
                            config.NeedEquitID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedEquitID", "0"));
                            config.NeedGoldBind = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedBandJinBi", "0"));
                            config.NeedZaiZao = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedZaiZao", "0"));
                            config.SuccessRate = (int)(Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "SuccessRate", "0")) * 100.0);
                            string needMaterial = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "NeedGoods", ""));
                            if (needMaterial.Length > 0)
                            {
                                config.NeedMaterial = new Dictionary<int, int>();
                                string[] materials = needMaterial.Split(new char[]
                                {
                                    '|'
                                });
                                foreach (string str in materials)
                                {
                                    string[] one = str.Split(new char[]
                                    {
                                        ','
                                    });
                                    int key = int.Parse(one[0]);
                                    int value = int.Parse(one[1]);
                                    config.NeedMaterial.Add(key, value);
                                }
                            }
                            string failMaterial = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "XiaoHuiGoods", ""));
                            if (failMaterial.Length > 0)
                            {
                                config.FailMaterial = new Dictionary<int, int>();
                                string[] materials = failMaterial.Split(new char[]
                                {
                                    '|'
                                });
                                foreach (string str in materials)
                                {
                                    string[] one = str.Split(new char[]
                                    {
                                        ','
                                    });
                                    int key = int.Parse(one[0]);
                                    int value = int.Parse(one[1]);
                                    config.FailMaterial.Add(key, value);
                                }
                            }
                            ArtifactManager._artifactList.Add(config);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Fatal, "加载Config/ZaiZao.xml时文件出错", ex, true);
                }
            }
        }

        
        public static ArtifactData GetArtifactDataByNeedId(int needID)
        {
            foreach (ArtifactData d in ArtifactManager._artifactList)
            {
                if (d.NeedEquitID == needID)
                {
                    return d;
                }
            }
            return null;
        }

        
        public static void LoadArtifactSuitData()
        {
            string fileName = "Config/TaoZhuangProps.xml";
            GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
            XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
            if (null == xml)
            {
                LogManager.WriteLog(LogTypes.Fatal, "加载Config/TaoZhuangProps.xml时出错!!!文件不存在", null, true);
            }
            else
            {
                try
                {
                    ArtifactManager._artifactSuitList.Clear();
                    IEnumerable<XElement> xmlItems = xml.Elements();
                    foreach (XElement xmlItem in xmlItems)
                    {
                        if (xmlItem != null)
                        {
                            ArtifactSuitData config = new ArtifactSuitData();
                            config.SuitID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
                            config.SuitName = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "Name", ""));
                            config.IsMulti = (Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Multi", "0")) > 0);
                            string equipIdStr = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0"));
                            if (equipIdStr.Length > 0)
                            {
                                config.EquipIDList = new List<int>();
                                string[] all = equipIdStr.Split(new char[]
                                {
                                    ','
                                });
                                foreach (string one in all)
                                {
                                    config.EquipIDList.Add(int.Parse(one));
                                }
                            }
                            string addString = Convert.ToString(Global.GetDefAttributeStr(xmlItem, "TaoZhuangProps", ""));
                            if (addString.Length > 0)
                            {
                                config.SuitAttr = new Dictionary<int, Dictionary<string, string>>();
                                string[] addArr = addString.Split(new char[]
                                {
                                    '|'
                                });
                                foreach (string str in addArr)
                                {
                                    string[] oneArr = str.Split(new char[]
                                    {
                                        ','
                                    });
                                    int count = int.Parse(oneArr[0]);
                                    if (config.SuitAttr.ContainsKey(count))
                                    {
                                        config.SuitAttr[count].Add(oneArr[1], oneArr[2]);
                                    }
                                    else
                                    {
                                        Dictionary<string, string> value = new Dictionary<string, string>();
                                        value.Add(oneArr[1], oneArr[2]);
                                        config.SuitAttr.Add(int.Parse(oneArr[0]), value);
                                    }
                                }
                            }
                            ArtifactManager._artifactSuitList.Add(config);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Fatal, "加载Config/TaoZhuangProps.xml时文件出错", ex, true);
                }
            }
        }

        
        public static ArtifactSuitData GetArtifactSuitDataByEquipID(int equipID)
        {
            foreach (ArtifactSuitData d in ArtifactManager._artifactSuitList)
            {
                foreach (int id in d.EquipIDList)
                {
                    if (id == equipID)
                    {
                        return d;
                    }
                }
            }
            return null;
        }

        
        public static ArtifactSuitData GetArtifactSuitDataBySuitID(int suitID)
        {
            foreach (ArtifactSuitData d in ArtifactManager._artifactSuitList)
            {
                if (d.SuitID == suitID)
                {
                    return d;
                }
            }
            return null;
        }

        
        public static ArtifactResultData UpArtifact(GameClient client, int equipID, bool isUseBind)
        {
            ArtifactResultData result = new ArtifactResultData();
            ArtifactResultData result2;
            if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Artifact, false))
            {
                result.State = -1;
                result2 = result;
            }
            else
            {
                GoodsData equipData = Global.GetGoodsByDbID(client, equipID);
                if (equipData == null)
                {
                    result.State = -2;
                    result2 = result;
                }
                else
                {
                    int catetoriy = Global.GetGoodsCatetoriy(equipData.GoodsID);
                    if (!GoodsUtil.CanUpgrade(catetoriy, 9))
                    {
                        if ((catetoriy < 0 || catetoriy > 6) && (catetoriy < 11 || catetoriy > 21))
                        {
                            result.State = -3;
                            return result;
                        }
                    }
                    SystemXmlItem systemGoods = null;
                    if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(equipData.GoodsID, out systemGoods))
                    {
                        result.State = -2;
                        result2 = result;
                    }
                    else
                    {
                        int nSuitID = systemGoods.GetIntValue("SuitID", -1);
                        ArtifactData artifactDataBasic = ArtifactManager.GetArtifactDataByNeedId(equipData.GoodsID);
                        if (artifactDataBasic == null)
                        {
                            result.State = -3;
                            result2 = result;
                        }
                        else if (Global.IsRoleHasEnoughMoney(client, artifactDataBasic.NeedZaiZao, 101) <= 0)
                        {
                            result.State = -4;
                            result2 = result;
                        }
                        else
                        {
                            int goldBind = Global.GetTotalBindTongQianAndTongQianVal(client);
                            if (artifactDataBasic.NeedGoldBind > goldBind)
                            {
                                result.State = -5;
                                result2 = result;
                            }
                            else
                            {
                                foreach (KeyValuePair<int, int> d in artifactDataBasic.NeedMaterial)
                                {
                                    int materialId = d.Key;
                                    int count = d.Value;
                                    int totalCount = Global.GetTotalGoodsCountByID(client, materialId);
                                    if (totalCount < count)
                                    {
                                        result.State = -6;
                                        return result;
                                    }
                                }
                                foreach (KeyValuePair<int, int> d in artifactDataBasic.FailMaterial)
                                {
                                    int materialId = d.Key;
                                    int count = d.Value;
                                    int totalCount = Global.GetTotalGoodsCountByID(client, materialId);
                                    if (totalCount < count)
                                    {
                                        result.State = -6;
                                        return result;
                                    }
                                }
                                int freeBagIndex = Global.GetIdleSlotOfBagGoods(client);
                                if (freeBagIndex < 0)
                                {
                                    result.State = -7;
                                    result2 = result;
                                }
                                else if (!Global.SubBindTongQianAndTongQian(client, artifactDataBasic.NeedGoldBind, "神器再造"))
                                {
                                    result.State = -5;
                                    result2 = result;
                                }
                                else
                                {
                                    bool isSuccess = false;
                                    int failCount = Global.GetRoleParamsInt32FromDB(client, "ArtifactFailCount");
                                    int failMax = (int)GameManager.systemParamsList.GetParamValueIntByName("ZaiZaoBaoDi", -1);
                                    if (failCount >= failMax)
                                    {
                                        isSuccess = true;
                                        failCount = 0;
                                        ArtifactManager.SetArtifactFailCount(client, failCount);
                                    }
                                    else
                                    {
                                        int rate = Global.GetRandomNumber(0, 100);
                                        if (rate < artifactDataBasic.SuccessRate)
                                        {
                                            isSuccess = true;
                                            failCount = 0;
                                            ArtifactManager.SetArtifactFailCount(client, failCount);
                                        }
                                    }
                                    bool useBind = false;
                                    bool useTimeLimit = false;
                                    if (!isSuccess)
                                    {
                                        foreach (KeyValuePair<int, int> d in artifactDataBasic.FailMaterial)
                                        {
                                            int materialId = d.Key;
                                            int count = d.Value;
                                            if (Global.UseGoodsBindOrNot(client, materialId, count, isUseBind, out useBind, out useTimeLimit) < 1)
                                            {
                                                result.State = -6;
                                                return result;
                                            }
                                        }
                                        failCount++;
                                        Global.SaveRoleParamsInt32ValueToDB(client, "ArtifactFailCount", failCount, true);
                                        GameManager.logDBCmdMgr.AddDBLogInfo(artifactDataBasic.NewEquitID, artifactDataBasic.ArtifactName, "神器再造失败", client.ClientData.RoleName, client.ClientData.RoleName, "再造", 1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, equipData);
                                        EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.ShenQiZaiZao, LogRecordType.ShenQiZaiZao, new object[]
                                        {
                                            artifactDataBasic.NewEquitID,
                                            0,
                                            failCount
                                        });
                                        result.State = 0;
                                        result2 = result;
                                    }
                                    else
                                    {
                                        foreach (KeyValuePair<int, int> d in artifactDataBasic.NeedMaterial)
                                        {
                                            int materialId = d.Key;
                                            int count = d.Value;
                                            bool oneUseBind = false;
                                            bool oneUseTimeLimit = false;
                                            if (Global.UseGoodsBindOrNot(client, materialId, count, isUseBind, out oneUseBind, out oneUseTimeLimit) < 1)
                                            {
                                                result.State = -6;
                                                return result;
                                            }
                                            useBind = (useBind || oneUseBind);
                                            useTimeLimit = (useTimeLimit || oneUseTimeLimit);
                                        }
                                        GameManager.ClientMgr.ModifyZaiZaoValue(client, -artifactDataBasic.NeedZaiZao, "神器再造", true, true, false);
                                        EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.ShenQiZaiZao, LogRecordType.ShenQiZaiZao, new object[]
                                        {
                                            artifactDataBasic.NewEquitID,
                                            1,
                                            0
                                        });
                                        int _Forge_level = equipData.Forge_level;
                                        int _AppendPropLev = equipData.AppendPropLev;
                                        int _Lucky = equipData.Lucky;
                                        int _ExcellenceInfo = equipData.ExcellenceInfo;
                                        List<int> _WashProps = equipData.WashProps;
                                        int _JuHun_lenvel = equipData.JuHunID;
                                        int _Binding = equipData.Binding;
                                        List<int> _ElementhrtsProps = equipData.ElementhrtsProps;
                                        if (useBind)
                                        {
                                            _Binding = 1;
                                        }
                                        if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, equipID, false, false))
                                        {
                                            result.State = -8;
                                            result2 = result;
                                        }
                                        else
                                        {
                                            int nItemDBID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, artifactDataBasic.NewEquitID, 1, equipData.Quality, "", _Forge_level, _Binding, 0, equipData.Jewellist, false, 1, "神器再造", "1900-01-01 12:00:00", equipData.AddPropIndex, equipData.BornIndex, _Lucky, 0, _ExcellenceInfo, _AppendPropLev, equipData.ChangeLifeLevForEquip, _WashProps, _ElementhrtsProps, _JuHun_lenvel, true);
                                            if (nItemDBID < 0)
                                            {
                                                result.State = -9;
                                                result2 = result;
                                            }
                                            else
                                            {
                                                string broadcastMsg = StringUtil.substitute(GLang.GetLang(11, new object[0]), new object[]
                                                {
                                                    Global.FormatRoleName(client, client.ClientData.RoleName),
                                                    artifactDataBasic.ArtifactName,
                                                    nSuitID + 1
                                                });
                                                Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.HintMsg, broadcastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
                                                result.State = 1;
                                                result.EquipDbID = nItemDBID;
                                                result.Bind = _Binding;
                                                result2 = result;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result2;
        }

        
        public static void SetArtifactProp(GameClient client)
        {
            Dictionary<int, List<int>> suitTypeList = new Dictionary<int, List<int>>();
            int attackMin = 0;
            int attackMax = 0;
            int defenseMin = 0;
            int defenseMax = 0;
            int mAttackMin = 0;
            int mAttackMax = 0;
            int mDefenseMin = 0;
            int mDefenseMax = 0;
            if (client.ClientData.GoodsDataList != null)
            {
                lock (client.ClientData.GoodsDataList)
                {
                    for (int i = 0; i < client.ClientData.GoodsDataList.Count; i++)
                    {
                        GoodsData goodsData = client.ClientData.GoodsDataList[i];
                        if (goodsData.Using > 0)
                        {
                            SystemXmlItem systemGoods = null;
                            if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
                            {
                                int categoriy = systemGoods.GetIntValue("Categoriy", -1);
                                bool isElementHrt = ElementhrtsManager.IsElementHrt(categoriy);
                                if (categoriy < 49 || isElementHrt)
                                {
                                    ArtifactSuitData suitData = ArtifactManager.GetArtifactSuitDataByEquipID(goodsData.GoodsID);
                                    if (suitData != null)
                                    {
                                        if (suitTypeList.ContainsKey(suitData.SuitID))
                                        {
                                            bool isAdd = true;
                                            List<int> value = suitTypeList[suitData.SuitID];
                                            if (!suitData.IsMulti)
                                            {
                                                foreach (int id in value)
                                                {
                                                    if (id == goodsData.GoodsID)
                                                    {
                                                        isAdd = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (isAdd)
                                            {
                                                value.Add(goodsData.GoodsID);
                                            }
                                        }
                                        else
                                        {
                                            List<int> value = new List<int>();
                                            value.Add(goodsData.GoodsID);
                                            suitTypeList.Add(suitData.SuitID, value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    double[] props = new double[177];
                    foreach (KeyValuePair<int, List<int>> type in suitTypeList)
                    {
                        int count = type.Value.Count;
                        if (count >= 2)
                        {
                            ArtifactSuitData suitData = ArtifactManager.GetArtifactSuitDataBySuitID(type.Key);
                            foreach (KeyValuePair<int, Dictionary<string, string>> attrs in suitData.SuitAttr)
                            {
                                if (count >= attrs.Key)
                                {
                                    foreach (KeyValuePair<string, string> attr in attrs.Value)
                                    {
                                        string[] values = attr.Value.Split(new char[]
                                        {
                                            '-'
                                        });
                                        string key = attr.Key;
                                        if (key == null)
                                        {
                                            goto IL_486;
                                        }
                                        if (!(key == "Attack"))
                                        {
                                            if (!(key == "Defense"))
                                            {
                                                if (!(key == "Mattack"))
                                                {
                                                    if (!(key == "Mdefense"))
                                                    {
                                                        goto IL_486;
                                                    }
                                                    mDefenseMin += int.Parse(values[0]);
                                                    mDefenseMax += int.Parse(values[1]);
                                                    props[5] += (double)int.Parse(values[0]);
                                                    props[6] += (double)int.Parse(values[1]);
                                                }
                                                else
                                                {
                                                    mAttackMin += int.Parse(values[0]);
                                                    mAttackMax += int.Parse(values[1]);
                                                    props[9] += (double)int.Parse(values[0]);
                                                    props[10] += (double)int.Parse(values[1]);
                                                }
                                            }
                                            else
                                            {
                                                defenseMin += int.Parse(values[0]);
                                                defenseMax += int.Parse(values[1]);
                                                props[3] += (double)int.Parse(values[0]);
                                                props[4] += (double)int.Parse(values[1]);
                                            }
                                        }
                                        else
                                        {
                                            attackMin += int.Parse(values[0]);
                                            attackMax += int.Parse(values[1]);
                                            props[7] += (double)int.Parse(values[0]);
                                            props[8] += (double)int.Parse(values[1]);
                                        }
                                        continue;
                                        IL_486:
                                        ExtPropIndexes propsIndex = ConfigParser.GetPropIndexByPropName(attr.Key);
                                        if (ExtPropIndexes.Strong <= propsIndex && propsIndex < ExtPropIndexes.Max)
                                        {
                                            props[(int)propsIndex] += double.Parse(values[0]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    client.ClientData.PropsCacheManager.SetExtProps(new object[]
                    {
                        7,
                        props
                    });
                }
            }
        }

        
        public static void SetRebornEquipArtifactProp(GameClient client)
        {
            Dictionary<int, List<int>> suitTypeList = new Dictionary<int, List<int>>();
            if (client.ClientData.RebornGoodsDataList != null)
            {
                lock (client.ClientData.RebornGoodsDataList)
                {
                    for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
                    {
                        GoodsData goodsData = client.ClientData.RebornGoodsDataList[i];
                        if (goodsData.Using > 0)
                        {
                            SystemXmlItem systemGoods = null;
                            if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
                            {
                                if (RebornEquip.IsRebornType(goodsData.GoodsID) && RebornEquip.IsRebornEquip(goodsData.GoodsID))
                                {
                                    ArtifactSuitData suitData = ArtifactManager.GetArtifactSuitDataByEquipID(goodsData.GoodsID);
                                    if (suitData != null)
                                    {
                                        if (suitTypeList.ContainsKey(suitData.SuitID))
                                        {
                                            bool isAdd = true;
                                            List<int> value = suitTypeList[suitData.SuitID];
                                            if (!suitData.IsMulti)
                                            {
                                                foreach (int id in value)
                                                {
                                                    if (id == goodsData.GoodsID)
                                                    {
                                                        isAdd = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (isAdd)
                                            {
                                                value.Add(goodsData.GoodsID);
                                            }
                                        }
                                        else
                                        {
                                            List<int> value = new List<int>();
                                            value.Add(goodsData.GoodsID);
                                            suitTypeList.Add(suitData.SuitID, value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    double[] props = new double[177];
                    foreach (KeyValuePair<int, List<int>> type in suitTypeList)
                    {
                        int count = type.Value.Count;
                        if (count >= 2)
                        {
                            ArtifactSuitData suitData = ArtifactManager.GetArtifactSuitDataBySuitID(type.Key);
                            foreach (KeyValuePair<int, Dictionary<string, string>> attrs in suitData.SuitAttr)
                            {
                                if (count >= attrs.Key)
                                {
                                    foreach (KeyValuePair<string, string> attr in attrs.Value)
                                    {
                                        string key = attr.Key;
                                        if (key == null)
                                        {
                                            goto IL_C25;
                                        }
                                        var data = new Dictionary<string, int>(42)
                                            {
                                                {
                                                    "HolyAttack",
                                                    0
                                                },
                                                {
                                                    "HolyDefense",
                                                    1
                                                },
                                                {
                                                    "HolyPenetratePercent",
                                                    2
                                                },
                                                {
                                                    "HolyAbsorbPercent",
                                                    3
                                                },
                                                {
                                                    "HolyWeakPercent",
                                                    4
                                                },
                                                {
                                                    "HolyDoubleAttackPercent",
                                                    5
                                                },
                                                {
                                                    "HolyDoubleAttackInjure",
                                                    6
                                                },
                                                {
                                                    "ShadowAttack",
                                                    7
                                                },
                                                {
                                                    "ShadowDefense",
                                                    8
                                                },
                                                {
                                                    "ShadowPenetratePercent",
                                                    9
                                                },
                                                {
                                                    "ShadowAbsorbPercent",
                                                    10
                                                },
                                                {
                                                    "ShadowWeakPercent",
                                                    11
                                                },
                                                {
                                                    "ShadowDoubleAttackPercent",
                                                    12
                                                },
                                                {
                                                    "ShadowDoubleAttackInjure",
                                                    13
                                                },
                                                {
                                                    "NatureAttack",
                                                    14
                                                },
                                                {
                                                    "NatureDefense",
                                                    15
                                                },
                                                {
                                                    "NaturePenetratePercent",
                                                    16
                                                },
                                                {
                                                    "NatureAbsorbPercent",
                                                    17
                                                },
                                                {
                                                    "NatureWeakPercent",
                                                    18
                                                },
                                                {
                                                    "NatureDoubleAttackPercent",
                                                    19
                                                },
                                                {
                                                    "NatureDoubleAttackInjure",
                                                    20
                                                },
                                                {
                                                    "ChaosAttack",
                                                    21
                                                },
                                                {
                                                    "ChaosDefense",
                                                    22
                                                },
                                                {
                                                    "ChaosPenetratePercent",
                                                    23
                                                },
                                                {
                                                    "ChaosAbsorbPercent",
                                                    24
                                                },
                                                {
                                                    "ChaosWeakPercent",
                                                    25
                                                },
                                                {
                                                    "ChaosDoubleAttackPercent",
                                                    26
                                                },
                                                {
                                                    "ChaosDoubleAttackInjure",
                                                    27
                                                },
                                                {
                                                    "IncubusAttack",
                                                    28
                                                },
                                                {
                                                    "IncubusDefense",
                                                    29
                                                },
                                                {
                                                    "IncubusPenetratePercent",
                                                    30
                                                },
                                                {
                                                    "IncubusAbsorbPercent",
                                                    31
                                                },
                                                {
                                                    "IncubusWeakPercent",
                                                    32
                                                },
                                                {
                                                    "IncubusDoubleAttackPercent",
                                                    33
                                                },
                                                {
                                                    "IncubusDoubleAttackInjure",
                                                    34
                                                },
                                                {
                                                    "RebornAttack",
                                                    35
                                                },
                                                {
                                                    "RebornDefense",
                                                    36
                                                },
                                                {
                                                    "RebornPenetratePercent",
                                                    37
                                                },
                                                {
                                                    "RebornAbsorbPercent",
                                                    38
                                                },
                                                {
                                                    "RebornWeakPercent",
                                                    39
                                                },
                                                {
                                                    "RebornDoubleAttackPercent",
                                                    40
                                                },
                                                {
                                                    "RebornDoubleAttackInjure",
                                                    41
                                                }
                                            };
                                        int num;
                                        if (!data.TryGetValue(key, out num))
                                        {
                                            goto IL_C25;
                                        }
                                        switch (num)
                                        {
                                            case 0:
                                                props[122] += (double)int.Parse(attr.Value);
                                                break;
                                            case 1:
                                                props[123] += (double)int.Parse(attr.Value);
                                                break;
                                            case 2:
                                                props[124] += double.Parse(attr.Value);
                                                break;
                                            case 3:
                                                props[125] += double.Parse(attr.Value);
                                                break;
                                            case 4:
                                                props[126] += double.Parse(attr.Value);
                                                break;
                                            case 5:
                                                props[127] += double.Parse(attr.Value);
                                                break;
                                            case 6:
                                                props[128] += double.Parse(attr.Value);
                                                break;
                                            case 7:
                                                props[129] += (double)int.Parse(attr.Value);
                                                break;
                                            case 8:
                                                props[130] += (double)int.Parse(attr.Value);
                                                break;
                                            case 9:
                                                props[131] += double.Parse(attr.Value);
                                                break;
                                            case 10:
                                                props[132] += double.Parse(attr.Value);
                                                break;
                                            case 11:
                                                props[133] += double.Parse(attr.Value);
                                                break;
                                            case 12:
                                                props[134] += double.Parse(attr.Value);
                                                break;
                                            case 13:
                                                props[135] += double.Parse(attr.Value);
                                                break;
                                            case 14:
                                                props[136] += (double)int.Parse(attr.Value);
                                                break;
                                            case 15:
                                                props[137] += (double)int.Parse(attr.Value);
                                                break;
                                            case 16:
                                                props[138] += double.Parse(attr.Value);
                                                break;
                                            case 17:
                                                props[139] += double.Parse(attr.Value);
                                                break;
                                            case 18:
                                                props[140] += double.Parse(attr.Value);
                                                break;
                                            case 19:
                                                props[141] += double.Parse(attr.Value);
                                                break;
                                            case 20:
                                                props[142] += double.Parse(attr.Value);
                                                break;
                                            case 21:
                                                props[143] += (double)int.Parse(attr.Value);
                                                break;
                                            case 22:
                                                props[144] += (double)int.Parse(attr.Value);
                                                break;
                                            case 23:
                                                props[145] += double.Parse(attr.Value);
                                                break;
                                            case 24:
                                                props[146] += double.Parse(attr.Value);
                                                break;
                                            case 25:
                                                props[147] += double.Parse(attr.Value);
                                                break;
                                            case 26:
                                                props[148] += double.Parse(attr.Value);
                                                break;
                                            case 27:
                                                props[149] += double.Parse(attr.Value);
                                                break;
                                            case 28:
                                                props[150] += (double)int.Parse(attr.Value);
                                                break;
                                            case 29:
                                                props[151] += (double)int.Parse(attr.Value);
                                                break;
                                            case 30:
                                                props[152] += double.Parse(attr.Value);
                                                break;
                                            case 31:
                                                props[153] += double.Parse(attr.Value);
                                                break;
                                            case 32:
                                                props[154] += double.Parse(attr.Value);
                                                break;
                                            case 33:
                                                props[155] += double.Parse(attr.Value);
                                                break;
                                            case 34:
                                                props[156] += double.Parse(attr.Value);
                                                break;
                                            case 35:
                                                props[157] += (double)int.Parse(attr.Value);
                                                break;
                                            case 36:
                                                props[158] += (double)int.Parse(attr.Value);
                                                break;
                                            case 37:
                                                props[159] += double.Parse(attr.Value);
                                                break;
                                            case 38:
                                                props[160] += double.Parse(attr.Value);
                                                break;
                                            case 39:
                                                props[161] += double.Parse(attr.Value);
                                                break;
                                            case 40:
                                                props[162] += double.Parse(attr.Value);
                                                break;
                                            case 41:
                                                props[163] += double.Parse(attr.Value);
                                                break;
                                            default:
                                                goto IL_C25;
                                        }
                                        continue;
                                        IL_C25:
                                        ExtPropIndexes propsIndex = ConfigParser.GetPropIndexByPropName(attr.Key);
                                        if (ExtPropIndexes.Strong <= propsIndex && propsIndex < ExtPropIndexes.Max)
                                        {
                                            props[(int)propsIndex] += double.Parse(attr.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    client.ClientData.PropsCacheManager.SetExtProps(new object[]
                    {
                        46,
                        props
                    });
                }
            }
        }

        
        public static void SetArtifactFailCount(GameClient client, int count)
        {
            Global.SaveRoleParamsInt32ValueToDB(client, "ArtifactFailCount", count, true);
        }

        
        private static ArtifactManager Instance = new ArtifactManager();

        
        private static List<ArtifactData> _artifactList = new List<ArtifactData>();

        
        private static List<ArtifactSuitData> _artifactSuitList = new List<ArtifactSuitData>();

        
        public enum ArtifactResultType
        {
            
            Success = 1,
            
            Fail = 0,
            
            EnoOpen = -1,
            
            EnoEquip = -2,
            
            EcantUp = -3,
            
            EnoZaiZao = -4,
            
            EnoGold = -5,
            
            EnoMaterial = -6,
            
            EnoBag = -7,
            
            EdelEquip = -8,
            
            EaddEquip = -9
        }
    }
}
