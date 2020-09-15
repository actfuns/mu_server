using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.FluorescentGem
{
    // Token: 0x020002B0 RID: 688
    public class FluorescentGemManager
    {
        // Token: 0x06000A7C RID: 2684 RVA: 0x000A52AC File Offset: 0x000A34AC
        private void LoadFluorescentGemLevelTypeConfigData()
        {
            try
            {
                lock (this.FluorescentGemLevelTypeConfigDict)
                {
                    string fileName = "Config/Gem/GemDigType.xml";
                    GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
                    XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
                    if (null == xml)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件异常", fileName), null, true);
                    }
                    else
                    {
                        IEnumerable<XElement> xmlItems = xml.Elements();
                        this.FluorescentGemLevelTypeConfigDict.Clear();
                        foreach (XElement xmlItem in xmlItems)
                        {
                            FluorescentGemLevelTypeConfigData tmpData = new FluorescentGemLevelTypeConfigData();
                            int nTypeID = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
                            tmpData._NeedFluorescentPoint = (int)Global.GetSafeAttributeLong(xmlItem, "CostYingGuangFenMo");
                            tmpData._NeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "CostZuanShi");
                            this.FluorescentGemLevelTypeConfigDict.Add(nTypeID, tmpData);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadFluorescentGemLevelTypeConfigData", new object[0])));
            }
        }

        // Token: 0x06000A7D RID: 2685 RVA: 0x000A5438 File Offset: 0x000A3638
        private void LoadFluorescentGemDigConfigData()
        {
            try
            {
                lock (this.FluorescentGemDigConfigDict)
                {
                    string fileName = "Config/Gem/GemDig.xml";
                    GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
                    XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
                    if (null == xml)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件异常", fileName), null, true);
                    }
                    else
                    {
                        IEnumerable<XElement> xmlItems = xml.Elements();
                        this.FluorescentGemDigConfigDict.Clear();
                        foreach (XElement xmlItem in xmlItems)
                        {
                            int nTypeID = (int)Global.GetSafeAttributeLong(xmlItem, "TypeID");
                            List<FluorescentGemDigConfigData> list = new List<FluorescentGemDigConfigData>();
                            IEnumerable<XElement> xmlDatas = xmlItem.Elements();
                            foreach (XElement xmlData in xmlDatas)
                            {
                                list.Add(new FluorescentGemDigConfigData
                                {
                                    _GoodsID = (int)Global.GetSafeAttributeLong(xmlData, "GoodsID"),
                                    _StartValue = (int)Global.GetSafeAttributeLong(xmlData, "StartValues"),
                                    _EndValue = (int)Global.GetSafeAttributeLong(xmlData, "EndValues")
                                });
                            }
                            this.FluorescentGemDigConfigDict.Add(nTypeID, list);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-FluorescentGemDigConfigDict", new object[0])));
            }
        }

        // Token: 0x06000A7E RID: 2686 RVA: 0x000A564C File Offset: 0x000A384C
        private void LoadFluorescentGemUpConfigData()
        {
            try
            {
                lock (this.FluorescentGemUpConfigDict)
                {
                    string fileName = "Config/Gem/GemLevelup.xml";
                    GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
                    XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
                    if (null == xml)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件异常", fileName), null, true);
                    }
                    else
                    {
                        IEnumerable<XElement> xmlItems = xml.Elements();
                        this.FluorescentGemUpConfigDict.Clear();
                        foreach (XElement xmlItem in xmlItems)
                        {
                            FluorescentGemUpConfigData tmpData = new FluorescentGemUpConfigData();
                            int nGoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
                            tmpData._ElementsType = (int)Global.GetSafeAttributeLong(xmlItem, "ElementsTypeID");
                            tmpData._GemType = (int)Global.GetSafeAttributeLong(xmlItem, "GemTypeID");
                            tmpData._Level = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
                            tmpData._OldGoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "OldGoodsID");
                            tmpData._NewGoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "NewGoodsID");
                            tmpData._NeedOldGoodsCount = (int)Global.GetSafeAttributeLong(xmlItem, "NeedOldGoodsNum");
                            tmpData._NeedLevelOneGoodsCount = (int)Global.GetSafeAttributeLong(xmlItem, "NeedOneLevelNum");
                            tmpData._NeedGold = (int)Global.GetSafeAttributeLong(xmlItem, "CostBandJinBi");
                            this.FluorescentGemUpConfigDict.Add(nGoodsID, tmpData);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadFluorescentGemLevelTypeConfigData", new object[0])));
            }
        }

        // Token: 0x06000A7F RID: 2687 RVA: 0x000A5850 File Offset: 0x000A3A50
        private bool IsSameGem(FluorescentGemUpConfigData data1, FluorescentGemUpConfigData data2)
        {
            return data1 != null && null != data2 && (data1._ElementsType == data2._ElementsType && data1._GemType == data2._GemType);
        }

        // Token: 0x06000A80 RID: 2688 RVA: 0x000A58A4 File Offset: 0x000A3AA4
        private bool CheckEquipPositionIndex(int nIndex)
        {
            return nIndex > 0 && nIndex < 11;
        }

        // Token: 0x06000A81 RID: 2689 RVA: 0x000A58D0 File Offset: 0x000A3AD0
        private bool CheckGemTypeIndex(int nIndex)
        {
            return nIndex > 0 && nIndex < 4;
        }

        // Token: 0x06000A82 RID: 2690 RVA: 0x000A58FC File Offset: 0x000A3AFC
        private int GetFluorescentPointByGoodsID(int nGoodsID)
        {
            SystemXmlItem systemGoods = null;
            int result;
            if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemGoods))
            {
                result = 0;
            }
            else
            {
                result = systemGoods.GetIntValue("ChangeYingGuang", -1);
            }
            return result;
        }

        // Token: 0x06000A83 RID: 2691 RVA: 0x000A5938 File Offset: 0x000A3B38
        private int GetFluorescentGemBagSpace(GameClient client)
        {
            int result;
            if (null == client)
            {
                result = 0;
            }
            else
            {
                result = 220 - client.ClientData.FluorescentGemData.GemBagList.Count<GoodsData>();
            }
            return result;
        }

        // Token: 0x06000A84 RID: 2692 RVA: 0x000A59BC File Offset: 0x000A3BBC
        private void ResetBagAllGoods(GameClient client)
        {
            lock (client.ClientData.FluorescentGemData)
            {
                List<GoodsData> list = client.ClientData.FluorescentGemData.GemBagList;
                Dictionary<string, GoodsData> oldGoodsDict = new Dictionary<string, GoodsData>();
                List<GoodsData> toRemovedGoodsDataList = new List<GoodsData>();
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].BagIndex = 1;
                    int gridNum = Global.GetGoodsGridNumByID(list[i].GoodsID);
                    if (gridNum > 1)
                    {
                        GoodsData oldGoodsData = null;
                        string key = string.Format("{0}_{1}_{2}_{3}", new object[]
                        {
                            list[i].GoodsID,
                            list[i].Binding,
                            Global.DateTimeTicks(list[i].Starttime),
                            Global.DateTimeTicks(list[i].Endtime)
                        });
                        if (oldGoodsDict.TryGetValue(key, out oldGoodsData))
                        {
                            int toAddNum = Global.GMin(gridNum - oldGoodsData.GCount, list[i].GCount);
                            oldGoodsData.GCount += toAddNum;
                            list[i].BagIndex = 1;
                            oldGoodsData.BagIndex = 1;
                            list[i].GCount -= toAddNum;
                            if (!Global.ResetBagGoodsData(client, list[i]))
                            {
                                break;
                            }
                            if (oldGoodsData.GCount >= gridNum)
                            {
                                if (list[i].GCount > 0)
                                {
                                    oldGoodsDict[key] = list[i];
                                }
                                else
                                {
                                    oldGoodsDict.Remove(key);
                                    toRemovedGoodsDataList.Add(list[i]);
                                }
                            }
                            else if (list[i].GCount <= 0)
                            {
                                toRemovedGoodsDataList.Add(list[i]);
                            }
                        }
                        else
                        {
                            oldGoodsDict[key] = list[i];
                        }
                    }
                }
                for (int i = 0; i < toRemovedGoodsDataList.Count; i++)
                {
                    list.Remove(toRemovedGoodsDataList[i]);
                }
                list.Sort(delegate (GoodsData x, GoodsData y)
                {
                    int xPrice = Global.GetGoodsYinLiangNumByID(x.GoodsID);
                    int yPrice = Global.GetGoodsYinLiangNumByID(y.GoodsID);
                    int result;
                    if (yPrice == xPrice)
                    {
                        result = y.GCount - x.GCount;
                    }
                    else
                    {
                        result = xPrice - yPrice;
                    }
                    return result;
                });
                int index = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    bool flag2 = 0 == 0;
                    list[i].BagIndex = index++;
                    if (!Global.ResetBagGoodsData(client, list[i]))
                    {
                        break;
                    }
                }
            }
        }

        // Token: 0x06000A85 RID: 2693 RVA: 0x000A5CB8 File Offset: 0x000A3EB8
        public Dictionary<int, GoodsData> GetBagDict(GameClient client)
        {
            Dictionary<int, GoodsData> dict = new Dictionary<int, GoodsData>();
            foreach (GoodsData goods in client.ClientData.FluorescentGemData.GemBagList)
            {
                if (goods.BagIndex < 220)
                {
                    dict[goods.BagIndex] = goods;
                }
            }
            return dict;
        }

        // Token: 0x06000A86 RID: 2694 RVA: 0x000A5D4C File Offset: 0x000A3F4C
        public Dictionary<int, Dictionary<int, GoodsData>> GetEquipDict(GameClient client)
        {
            Dictionary<int, Dictionary<int, GoodsData>> result = new Dictionary<int, Dictionary<int, GoodsData>>();
            foreach (GoodsData gd in client.ClientData.FluorescentGemData.GemEquipList)
            {
                int _pos;
                int _type;
                this.ParsePosAndType(gd.BagIndex, out _pos, out _type);
                Dictionary<int, GoodsData> tmpGoodsDict = null;
                if (!result.TryGetValue(_pos, out tmpGoodsDict))
                {
                    tmpGoodsDict = new Dictionary<int, GoodsData>();
                    result[_pos] = tmpGoodsDict;
                }
                tmpGoodsDict[_type] = gd;
            }
            return result;
        }

        // Token: 0x06000A87 RID: 2695 RVA: 0x000A5DFC File Offset: 0x000A3FFC
        private EFluorescentGemDigErrorCode FluorescentGemDig(GameClient client, int nLevelType, int nDigType, out List<int> gemList)
        {
            gemList = new List<int>();
            try
            {
                if (null == client)
                {
                    return EFluorescentGemDigErrorCode.Error;
                }
                if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
                {
                    return EFluorescentGemDigErrorCode.Error;
                }
                if (!this.FluorescentGemLevelTypeConfigDict.ContainsKey(nLevelType))
                {
                    return EFluorescentGemDigErrorCode.LevelTypeError;
                }
                if (nDigType < 0 || nDigType > 1)
                {
                    return EFluorescentGemDigErrorCode.DigType;
                }
                int digNum = 0;
                switch (nDigType)
                {
                    case 0:
                        digNum = 1;
                        break;
                    case 1:
                        digNum = 10;
                        break;
                }
                lock (client.ClientData.FluorescentGemData)
                {
                    if (this.GetFluorescentGemBagSpace(client) < digNum)
                    {
                        return EFluorescentGemDigErrorCode.BagNotEnoughTen;
                    }
                    FluorescentGemLevelTypeConfigData levelTypeData = null;
                    lock (this.FluorescentGemLevelTypeConfigDict)
                    {
                        if (!this.FluorescentGemLevelTypeConfigDict.TryGetValue(nLevelType, out levelTypeData) || null == levelTypeData)
                        {
                            return EFluorescentGemDigErrorCode.LevelTypeDataError;
                        }
                    }
                    if (levelTypeData._NeedFluorescentPoint > 0)
                    {
                        if (client.ClientData.FluorescentPoint < levelTypeData._NeedFluorescentPoint * digNum)
                        {
                            return EFluorescentGemDigErrorCode.PointNotEnough;
                        }
                    }
                    if (levelTypeData._NeedDiamond > 0)
                    {
                        if (!MoneyUtil.CheckHasMoney(client, 163, levelTypeData._NeedDiamond * digNum) && !HuanLeDaiBiManager.GetInstance().HuanledaibiReplaceEnough(client, levelTypeData._NeedDiamond * digNum, DaiBiSySType.JingLingLieQu))
                        {
                            return EFluorescentGemDigErrorCode.DiamondNotEnough;
                        }
                    }
                    if (levelTypeData._NeedFluorescentPoint > 0)
                    {
                        if (!this.DecFluorescentPoint(client, levelTypeData._NeedFluorescentPoint * digNum, "宝石挖掘扣除", false))
                        {
                            return EFluorescentGemDigErrorCode.UpdatePointError;
                        }
                    }
                    if (levelTypeData._NeedDiamond > 0)
                    {
                        if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -levelTypeData._NeedDiamond * digNum, "荧光宝石挖掘", false, DaiBiSySType.YingGuanShiChouQu))
                        {
                            return EFluorescentGemDigErrorCode.UpdateDiamondError;
                        }
                    }
                    List<FluorescentGemDigConfigData> digList = null;
                    if (!this.FluorescentGemDigConfigDict.TryGetValue(nLevelType, out digList) || null == digList)
                    {
                        return EFluorescentGemDigErrorCode.DigDataError;
                    }
                    for (int i = 0; i < digNum; i++)
                    {
                        int nRandom = Global.GetRandomNumber(1, 100001);
                        int nGoodsID = 0;
                        for (int j = 0; j < digList.Count; j++)
                        {
                            if (nRandom >= digList[j]._StartValue && nRandom <= digList[j]._EndValue)
                            {
                                nGoodsID = digList[j]._GoodsID;
                                break;
                            }
                        }
                        if (!this.CheckIsFluorescentGemByGoodsID(nGoodsID))
                        {
                            return EFluorescentGemDigErrorCode.NotGem;
                        }
                        int nDBID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, 1, 0, "", 0, 0, 7000, "", true, 1, "荧光宝石挖掘", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
                        if (nDBID < 0)
                        {
                            return EFluorescentGemDigErrorCode.AddGoodsError;
                        }
                        gemList.Add(nGoodsID);
                        GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "挖掘", "系统", client.ClientData.RoleName, "修改", nGoodsID, client.ClientData.ZoneID, client.strUserID, nLevelType, client.ServerId, null);
                    }
                    GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "消耗", "系统", client.ClientData.RoleName, "修改", levelTypeData._NeedFluorescentPoint * digNum, client.ClientData.ZoneID, client.strUserID, levelTypeData._NeedDiamond * digNum, client.ServerId, null);
                }
                return EFluorescentGemDigErrorCode.Success;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return EFluorescentGemDigErrorCode.Error;
        }

        // Token: 0x06000A88 RID: 2696 RVA: 0x000A6270 File Offset: 0x000A4470
        private EFluorescentGemDigErrorCode FluorescentGemDig_BigNum(GameClient client, int nLevelType, int nDigType, out Dictionary<int, int> gemDict)
        {
            gemDict = new Dictionary<int, int>();
            try
            {
                if (null == client)
                {
                    return EFluorescentGemDigErrorCode.Error;
                }
                if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
                {
                    return EFluorescentGemDigErrorCode.Error;
                }
                if (!this.FluorescentGemLevelTypeConfigDict.ContainsKey(nLevelType))
                {
                    return EFluorescentGemDigErrorCode.LevelTypeError;
                }
                if (nDigType != 2)
                {
                    return EFluorescentGemDigErrorCode.DigType;
                }
                int digNum = 50;
                lock (client.ClientData.FluorescentGemData)
                {
                    if (this.GetFluorescentGemBagSpace(client) < digNum)
                    {
                        return EFluorescentGemDigErrorCode.BagNotEnoughTen;
                    }
                    FluorescentGemLevelTypeConfigData levelTypeData = null;
                    lock (this.FluorescentGemLevelTypeConfigDict)
                    {
                        if (!this.FluorescentGemLevelTypeConfigDict.TryGetValue(nLevelType, out levelTypeData) || null == levelTypeData)
                        {
                            return EFluorescentGemDigErrorCode.LevelTypeDataError;
                        }
                    }
                    if (levelTypeData._NeedFluorescentPoint > 0)
                    {
                        if (client.ClientData.FluorescentPoint < levelTypeData._NeedFluorescentPoint * digNum)
                        {
                            return EFluorescentGemDigErrorCode.PointNotEnough;
                        }
                    }
                    if (levelTypeData._NeedDiamond > 0)
                    {
                        if (!MoneyUtil.CheckHasMoney(client, 163, levelTypeData._NeedDiamond * digNum) && !HuanLeDaiBiManager.GetInstance().HuanledaibiReplaceEnough(client, levelTypeData._NeedDiamond * digNum, DaiBiSySType.JingLingLieQu))
                        {
                            return EFluorescentGemDigErrorCode.DiamondNotEnough;
                        }
                    }
                    if (levelTypeData._NeedFluorescentPoint > 0)
                    {
                        if (!this.DecFluorescentPoint(client, levelTypeData._NeedFluorescentPoint * digNum, "宝石挖掘扣除", false))
                        {
                            return EFluorescentGemDigErrorCode.UpdatePointError;
                        }
                    }
                    if (levelTypeData._NeedDiamond > 0)
                    {
                        if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -levelTypeData._NeedDiamond * digNum, "荧光宝石挖掘", false, DaiBiSySType.YingGuanShiChouQu))
                        {
                            return EFluorescentGemDigErrorCode.UpdateDiamondError;
                        }
                    }
                    List<FluorescentGemDigConfigData> digList = null;
                    if (!this.FluorescentGemDigConfigDict.TryGetValue(nLevelType, out digList) || null == digList)
                    {
                        return EFluorescentGemDigErrorCode.DigDataError;
                    }
                    for (int i = 0; i < digNum; i++)
                    {
                        int nRandom = Global.GetRandomNumber(1, 100001);
                        int nGoodsID = 0;
                        for (int j = 0; j < digList.Count; j++)
                        {
                            if (nRandom >= digList[j]._StartValue && nRandom <= digList[j]._EndValue)
                            {
                                nGoodsID = digList[j]._GoodsID;
                                break;
                            }
                        }
                        if (!this.CheckIsFluorescentGemByGoodsID(nGoodsID))
                        {
                            return EFluorescentGemDigErrorCode.NotGem;
                        }
                        if (!gemDict.ContainsKey(nGoodsID))
                        {
                            gemDict[nGoodsID] = 0;
                        }
                        Dictionary<int, int> dictionary;
                        int key;
                        (dictionary = gemDict)[key = nGoodsID] = dictionary[key] + 1;
                        GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "挖掘", "系统", client.ClientData.RoleName, "修改", nGoodsID, client.ClientData.ZoneID, client.strUserID, nLevelType, client.ServerId, null);
                    }
                    foreach (KeyValuePair<int, int> item in gemDict)
                    {
                        int nDBID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.Key, item.Value, 0, "", 0, 0, 7000, "", true, 1, "荧光宝石挖掘", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
                        if (nDBID < 0)
                        {
                            return EFluorescentGemDigErrorCode.AddGoodsError;
                        }
                    }
                    GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "消耗", "系统", client.ClientData.RoleName, "修改", levelTypeData._NeedFluorescentPoint * digNum, client.ClientData.ZoneID, client.strUserID, levelTypeData._NeedDiamond * digNum, client.ServerId, null);
                }
                return EFluorescentGemDigErrorCode.Success;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return EFluorescentGemDigErrorCode.Error;
        }

        // Token: 0x06000A89 RID: 2697 RVA: 0x000A676C File Offset: 0x000A496C
        private EFluorescentGemResolveErrorCode FluorescentGemResolve(GameClient client, int nBagIndex, int nResolveCount)
        {
            try
            {
                if (null == client)
                {
                    return EFluorescentGemResolveErrorCode.Error;
                }
                lock (client.ClientData.FluorescentGemData)
                {
                    GoodsData goodsData;
                    if ((goodsData = client.ClientData.FluorescentGemData.GemBagList.Find((GoodsData _g) => _g.BagIndex == nBagIndex)) == null)
                    {
                        return EFluorescentGemResolveErrorCode.GoodsNotExist;
                    }
                    if (!this.CheckIsFluorescentGemByGoodsID(goodsData.GoodsID))
                    {
                        return EFluorescentGemResolveErrorCode.NotGem;
                    }
                    if (nResolveCount <= 0 || nResolveCount > goodsData.GCount)
                    {
                        return EFluorescentGemResolveErrorCode.ResolveCountError;
                    }
                    int nTotalCount = this.GetFluorescentPointByGoodsID(goodsData.GoodsID) * nResolveCount;
                    if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, nResolveCount, false, false))
                    {
                        return EFluorescentGemResolveErrorCode.ResolveError;
                    }
                    this.AddFluorescentPoint(client, nTotalCount, "宝石分解获得", true);
                }
                return EFluorescentGemResolveErrorCode.Success;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return EFluorescentGemResolveErrorCode.Error;
        }

        // Token: 0x06000A8A RID: 2698 RVA: 0x000A6904 File Offset: 0x000A4B04
        public int GenerateBagIndex(int pos, int type)
        {
            return pos * 100 + type;
        }

        // Token: 0x06000A8B RID: 2699 RVA: 0x000A691C File Offset: 0x000A4B1C
        public void ParsePosAndType(int bagIndex, out int pos, out int type)
        {
            pos = bagIndex / 100;
            type = bagIndex % 100;
        }

        // Token: 0x06000A8C RID: 2700 RVA: 0x000A69F4 File Offset: 0x000A4BF4
        private EFluorescentGemUpErrorCode FluorescentGemUp(GameClient client, FluorescentGemUpTransferData upData, out int nNewGoodsDBID)
        {
            nNewGoodsDBID = -1;
            try
            {
                if (null == client)
                {
                    return EFluorescentGemUpErrorCode.Error;
                }
                if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
                {
                    return EFluorescentGemUpErrorCode.Error;
                }
                GoodsData goodsData = null;
                lock (client.ClientData.FluorescentGemData)
                {
                    if (upData._UpType == 0)
                    {
                        if (this.GetFluorescentGemBagSpace(client) < 1)
                        {
                            return EFluorescentGemUpErrorCode.BagNotEnoughOne;
                        }
                        if ((goodsData = client.ClientData.FluorescentGemData.GemBagList.Find((GoodsData _g) => _g.BagIndex == upData._BagIndex)) == null)
                        {
                            return EFluorescentGemUpErrorCode.GoodsNotExist;
                        }
                    }
                    else
                    {
                        if (!this.CheckEquipPositionIndex(upData._Position))
                        {
                            return EFluorescentGemUpErrorCode.PositionIndexError;
                        }
                        if (!this.CheckGemTypeIndex(upData._GemType))
                        {
                            return EFluorescentGemUpErrorCode.GemTypeError;
                        }
                        if ((goodsData = client.ClientData.FluorescentGemData.GemEquipList.Find((GoodsData _g) => _g.BagIndex == this.GenerateBagIndex(upData._Position, upData._GemType))) == null)
                        {
                            return EFluorescentGemUpErrorCode.GoodsNotExist;
                        }
                    }
                    if (null == goodsData)
                    {
                        return EFluorescentGemUpErrorCode.GoodsNotExist;
                    }
                    if (!this.CheckIsFluorescentGemByGoodsID(goodsData.GoodsID))
                    {
                        return EFluorescentGemUpErrorCode.NotGem;
                    }
                    FluorescentGemUpConfigData goodsConfig = null;
                    if (!this.FluorescentGemUpConfigDict.TryGetValue(goodsData.GoodsID, out goodsConfig) || null == goodsConfig)
                    {
                        return EFluorescentGemUpErrorCode.UpDataError;
                    }
                    if (goodsConfig._NewGoodsID <= 0)
                    {
                        return EFluorescentGemUpErrorCode.MaxLevel;
                    }
                    FluorescentGemUpConfigData nextGoodsConfig = null;
                    if (!this.FluorescentGemUpConfigDict.TryGetValue(goodsConfig._NewGoodsID, out nextGoodsConfig) || null == nextGoodsConfig)
                    {
                        return EFluorescentGemUpErrorCode.NextLevelDataError;
                    }
                    if (client.ClientData.Money1 + client.ClientData.YinLiang < nextGoodsConfig._NeedGold)
                    {
                        return EFluorescentGemUpErrorCode.GoldNotEnough;
                    }
                    int nNeedLevelOneGemCount = 0;
                    nNeedLevelOneGemCount = goodsConfig._NeedLevelOneGoodsCount * 3;
                    int nTotalLevelOneCount = 0;
                    if (upData._UpType == 1)
                    {
                        nTotalLevelOneCount += goodsConfig._NeedLevelOneGoodsCount;
                    }
                    using (Dictionary<int, int>.Enumerator enumerator = upData._DecGoodsDict.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<int, int> item = enumerator.Current;
                            GoodsData tmpGoods;
                            if ((tmpGoods = client.ClientData.FluorescentGemData.GemBagList.Find(delegate (GoodsData _g)
                            {
                                int bagIndex = _g.BagIndex;
                                KeyValuePair<int, int> item2 = item;
                                return bagIndex == item2.Key;
                            })) == null)
                            {
                                return EFluorescentGemUpErrorCode.DecGoodsNotExist;
                            }
                            int gcount = tmpGoods.GCount;
                            KeyValuePair<int, int> item3 = item;
                            if (gcount < item3.Value)
                            {
                                return EFluorescentGemUpErrorCode.DecGoodsNotEnough;
                            }
                            FluorescentGemUpConfigData tmpConfig = null;
                            if (this.FluorescentGemUpConfigDict.TryGetValue(tmpGoods.GoodsID, out tmpConfig) && null != tmpConfig)
                            {
                                if (this.IsSameGem(goodsConfig, tmpConfig))
                                {
                                    int num = nTotalLevelOneCount;
                                    int needLevelOneGoodsCount = tmpConfig._NeedLevelOneGoodsCount;
                                    item3 = item;
                                    nTotalLevelOneCount = num + needLevelOneGoodsCount * item3.Value;
                                }
                            }
                        }
                    }
                    if (nNeedLevelOneGemCount != nTotalLevelOneCount)
                    {
                        return EFluorescentGemUpErrorCode.GemNotEnough;
                    }
                    using (Dictionary<int, int>.Enumerator enumerator = upData._DecGoodsDict.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<int, int> item = enumerator.Current;
                            GoodsData tmpGoods;
                            if ((tmpGoods = client.ClientData.FluorescentGemData.GemBagList.Find(delegate (GoodsData _g)
                            {
                                int bagIndex = _g.BagIndex;
                                KeyValuePair<int, int> item4 = item;
                                return bagIndex == item4.Key;
                            })) == null)
                            {
                                return EFluorescentGemUpErrorCode.DecGoodsError;
                            }
                            ClientManager clientMgr = GameManager.ClientMgr;
                            SocketListener mySocketListener = Global._TCPManager.MySocketListener;
                            TCPClientPool tcpClientPool = Global._TCPManager.tcpClientPool;
                            TCPOutPacketPool tcpOutPacketPool = Global._TCPManager.TcpOutPacketPool;
                            GoodsData goodsData2 = tmpGoods;
                            KeyValuePair<int, int> item3 = item;
                            if (!clientMgr.NotifyUseGoods(mySocketListener, tcpClientPool, tcpOutPacketPool, client, goodsData2, item3.Value, false, false))
                            {
                                return EFluorescentGemUpErrorCode.DecGoodsError;
                            }
                        }
                    }
                    if (upData._UpType == 0)
                    {
                        if (!Global.SubBindTongQianAndTongQian(client, nextGoodsConfig._NeedGold, "荧光宝石升级"))
                        {
                            return EFluorescentGemUpErrorCode.GoldNotEnough;
                        }
                        int nDBID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsConfig._NewGoodsID, 1, 0, "", 0, 0, 7000, "", false, 1, "荧光宝石升级", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
                        if (nDBID < 0)
                        {
                            return EFluorescentGemUpErrorCode.AddGoodsError;
                        }
                        nNewGoodsDBID = nDBID;
                        GameManager.logDBCmdMgr.AddDBLogInfo(nDBID, "荧光宝石", "背包宝石升级", "系统", client.ClientData.RoleName, "修改", goodsData.GoodsID, client.ClientData.ZoneID, client.strUserID, goodsConfig._NewGoodsID, client.ServerId, null);
                    }
                    else
                    {
                        if (!Global.SubBindTongQianAndTongQian(client, nextGoodsConfig._NeedGold, "荧光宝石升级"))
                        {
                            return EFluorescentGemUpErrorCode.GoldNotEnough;
                        }
                        if (!this.NotifyUnEquipGem(client, new FluorescentGemSaveDBData
                        {
                            _RoleID = client.ClientData.RoleID,
                            _Position = upData._Position,
                            _GemType = upData._GemType
                        }, 1))
                        {
                            return EFluorescentGemUpErrorCode.DecGoodsError;
                        }
                        if (!this.NotifyEquipGem(client, new FluorescentGemSaveDBData
                        {
                            _RoleID = client.ClientData.RoleID,
                            _GoodsID = goodsConfig._NewGoodsID,
                            _Position = upData._Position,
                            _GemType = upData._GemType
                        }))
                        {
                            return EFluorescentGemUpErrorCode.EquipError;
                        }
                        this.UpdateProps(client);
                        GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                        GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
                        GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "装备栏宝石升级", "系统", client.ClientData.RoleName, "修改", goodsData.GoodsID, client.ClientData.ZoneID, client.strUserID, goodsConfig._NewGoodsID, client.ServerId, null);
                    }
                }
                return EFluorescentGemUpErrorCode.Success;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return EFluorescentGemUpErrorCode.Error;
        }

        // Token: 0x06000A8D RID: 2701 RVA: 0x000A7208 File Offset: 0x000A5408
        private EFluorescentGemEquipErrorCode FluorescentGemEquip(GameClient client, int nBagIndex, int nPositionIndex, int nGemType)
        {
            try
            {
                if (null == client)
                {
                    return EFluorescentGemEquipErrorCode.Error;
                }
                if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
                {
                    return EFluorescentGemEquipErrorCode.Error;
                }
                GoodsData goodsData;
                if ((goodsData = client.ClientData.FluorescentGemData.GemBagList.Find((GoodsData _g) => _g.BagIndex == nBagIndex)) == null)
                {
                    return EFluorescentGemEquipErrorCode.GoodsNotExist;
                }
                if (!this.CheckIsFluorescentGemByGoodsID(goodsData.GoodsID))
                {
                    return EFluorescentGemEquipErrorCode.NotGem;
                }
                if (!this.CheckEquipPositionIndex(nPositionIndex))
                {
                    return EFluorescentGemEquipErrorCode.PositionIndexError;
                }
                if (!this.CheckGemTypeIndex(nGemType))
                {
                    return EFluorescentGemEquipErrorCode.GemTypeError;
                }
                FluorescentGemUpConfigData goodsConfig = null;
                if (!this.FluorescentGemUpConfigDict.TryGetValue(goodsData.GoodsID, out goodsConfig) || null == goodsConfig)
                {
                    return EFluorescentGemEquipErrorCode.GemDataError;
                }
                if (nGemType != goodsConfig._GemType)
                {
                    return EFluorescentGemEquipErrorCode.GemTypeError;
                }
                GoodsData destGoods = client.ClientData.FluorescentGemData.GemEquipList.Find((GoodsData _g) => _g.BagIndex == this.GenerateBagIndex(nPositionIndex, nGemType));
                if (destGoods != null)
                {
                    EFluorescentGemUnEquipErrorCode unEquipRes = this.FluorescentGemUnEquip(client, 0, nPositionIndex, nGemType);
                    if (unEquipRes != EFluorescentGemUnEquipErrorCode.Success)
                    {
                        return EFluorescentGemEquipErrorCode.UnEquipError;
                    }
                }
                GoodsData equipGoods = Global.CopyGoodsData(goodsData);
                if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, 1, false, false))
                {
                    return EFluorescentGemEquipErrorCode.DecGoodsError;
                }
                if (!this.NotifyEquipGem(client, new FluorescentGemSaveDBData
                {
                    _RoleID = client.ClientData.RoleID,
                    _GoodsID = equipGoods.GoodsID,
                    _Position = nPositionIndex,
                    _GemType = nGemType,
                    _Bind = equipGoods.Binding
                }))
                {
                    return EFluorescentGemEquipErrorCode.EquipError;
                }
                this.UpdateProps(client);
                GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
                return EFluorescentGemEquipErrorCode.Success;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return EFluorescentGemEquipErrorCode.Error;
        }

        // Token: 0x06000A8E RID: 2702 RVA: 0x000A7528 File Offset: 0x000A5728
        private EFluorescentGemUnEquipErrorCode FluorescentGemUnEquip(GameClient client, int nUnEquipType, int nPositionIndex, int nGemType)
        {
            try
            {
                if (null == client)
                {
                    return EFluorescentGemUnEquipErrorCode.Error;
                }
                if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
                {
                    return EFluorescentGemUnEquipErrorCode.Error;
                }
                switch (nUnEquipType)
                {
                    case 0:
                        lock (client.ClientData.FluorescentGemData)
                        {
                            if (this.GetFluorescentGemBagSpace(client) < 1)
                            {
                                return EFluorescentGemUnEquipErrorCode.BagNotEnoughOne;
                            }
                            if (!this.CheckEquipPositionIndex(nPositionIndex))
                            {
                                return EFluorescentGemUnEquipErrorCode.PositionIndexError;
                            }
                            if (!this.CheckGemTypeIndex(nGemType))
                            {
                                return EFluorescentGemUnEquipErrorCode.GemTypeError;
                            }
                            GoodsData goodsData;
                            if ((goodsData = client.ClientData.FluorescentGemData.GemEquipList.Find((GoodsData _g) => _g.BagIndex == this.GenerateBagIndex(nPositionIndex, nGemType))) == null)
                            {
                                return EFluorescentGemUnEquipErrorCode.GoodsNotExist;
                            }
                            if (!this.NotifyUnEquipGem(client, new FluorescentGemSaveDBData
                            {
                                _RoleID = client.ClientData.RoleID,
                                _GoodsID = goodsData.GoodsID,
                                _Position = nPositionIndex,
                                _GemType = nGemType,
                                _Bind = goodsData.Binding
                            }, 0))
                            {
                                return EFluorescentGemUnEquipErrorCode.UnEquipError;
                            }
                        }
                        break;
                    case 1:
                        lock (client.ClientData.FluorescentGemData)
                        {
                            if (this.GetFluorescentGemBagSpace(client) < 3)
                            {
                                return EFluorescentGemUnEquipErrorCode.BagNotEnoughThree;
                            }
                            if (!this.CheckEquipPositionIndex(nPositionIndex))
                            {
                                return EFluorescentGemUnEquipErrorCode.PositionIndexError;
                            }
                            List<GoodsData> decList = new List<GoodsData>();
                            List<int> decGemTypeList = new List<int>();
                            foreach (GoodsData item in client.ClientData.FluorescentGemData.GemEquipList)
                            {
                                int _pos;
                                int _type;
                                this.ParsePosAndType(item.BagIndex, out _pos, out _type);
                                if (_pos == nPositionIndex)
                                {
                                    decList.Add(item);
                                    decGemTypeList.Add(_type);
                                }
                            }
                            for (int i = 0; i < decList.Count; i++)
                            {
                                GoodsData goodsData = decList[i];
                                if (null != goodsData)
                                {
                                    if (!this.NotifyUnEquipGem(client, new FluorescentGemSaveDBData
                                    {
                                        _RoleID = client.ClientData.RoleID,
                                        _GoodsID = goodsData.GoodsID,
                                        _Position = nPositionIndex,
                                        _GemType = decGemTypeList[i],
                                        _Bind = goodsData.Binding
                                    }, 0))
                                    {
                                        return EFluorescentGemUnEquipErrorCode.UnEquipError;
                                    }
                                }
                            }
                        }
                        break;
                }
                this.UpdateProps(client);
                GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
                return EFluorescentGemUnEquipErrorCode.Success;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return EFluorescentGemUnEquipErrorCode.Error;
        }

        // Token: 0x06000A8F RID: 2703 RVA: 0x000A7960 File Offset: 0x000A5B60
        private bool NotifyEquipGem(GameClient client, FluorescentGemSaveDBData data)
        {
            bool result;
            if (client == null || null == data)
            {
                result = false;
            }
            else if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
            {
                result = false;
            }
            else
            {
                byte[] sendBytes = DataHelper.ObjectToBytes<FluorescentGemSaveDBData>(data);
                if (!Global.sendToDB<bool, byte[]>(10209, sendBytes, client.ServerId))
                {
                    result = false;
                }
                else
                {
                    GoodsData equipGoods = new GoodsData();
                    equipGoods.GoodsID = data._GoodsID;
                    equipGoods.GCount = 1;
                    equipGoods.Binding = data._Bind;
                    equipGoods.Site = 7001;
                    equipGoods.BagIndex = this.GenerateBagIndex(data._Position, data._GemType);
                    lock (client.ClientData.FluorescentGemData)
                    {
                        client.ClientData.FluorescentGemData.GemEquipList.RemoveAll((GoodsData _g) => _g.BagIndex == equipGoods.BagIndex);
                        client.ClientData.FluorescentGemData.GemEquipList.Add(equipGoods);
                    }
                    client.sendCmd<FluorescentGemEquipChangesTransferData>(997, new FluorescentGemEquipChangesTransferData
                    {
                        _Position = data._Position,
                        _GemType = data._GemType,
                        _GoodsData = equipGoods
                    }, false);
                    GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "镶嵌", "系统", client.ClientData.RoleName, "修改", data._GoodsID, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, null);
                    result = true;
                }
            }
            return result;
        }

        // Token: 0x06000A90 RID: 2704 RVA: 0x000A7B94 File Offset: 0x000A5D94
        private bool NotifyUnEquipGem(GameClient client, FluorescentGemSaveDBData data, int nOP)
        {
            bool result;
            if (client == null || null == data)
            {
                result = false;
            }
            else if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
            {
                result = false;
            }
            else if (nOP < 0 || nOP > 1)
            {
                result = false;
            }
            else
            {
                byte[] sendBytes = DataHelper.ObjectToBytes<FluorescentGemSaveDBData>(data);
                if (!Global.sendToDB<bool, byte[]>(10210, sendBytes, client.ServerId))
                {
                    result = false;
                }
                else
                {
                    lock (client.ClientData.FluorescentGemData)
                    {
                        client.ClientData.FluorescentGemData.GemEquipList.RemoveAll((GoodsData _g) => _g.BagIndex == this.GenerateBagIndex(data._Position, data._GemType));
                    }
                    client.sendCmd<FluorescentGemEquipChangesTransferData>(997, new FluorescentGemEquipChangesTransferData
                    {
                        _Position = data._Position,
                        _GemType = data._GemType,
                        _GoodsData = null
                    }, false);
                    if (nOP == 0)
                    {
                        int nDBID = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, data._GoodsID, 1, 0, "", 0, data._Bind, 7000, "", true, 0, "荧光宝石卸下", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
                        if (nDBID < 0)
                        {
                            return false;
                        }
                    }
                    GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "卸下", "系统", client.ClientData.RoleName, "修改", data._GoodsID, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, null);
                    result = true;
                }
            }
            return result;
        }

        // Token: 0x06000A91 RID: 2705 RVA: 0x000A7DB4 File Offset: 0x000A5FB4
        public void LoadFluorescentGemConfigData()
        {
            this.LoadFluorescentGemDigConfigData();
            this.LoadFluorescentGemLevelTypeConfigData();
            this.LoadFluorescentGemUpConfigData();
        }

        // Token: 0x06000A92 RID: 2706 RVA: 0x000A7DCC File Offset: 0x000A5FCC
        public bool IsOpenFluorescentGem(GameClient client)
        {
            bool result;
            if (null == client)
            {
                result = false;
            }
            else if (!GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("FluorescentGem"))
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("版本控制未开启荧光宝石功能, RoleID={0}", client.ClientData.RoleID), null, true);
                result = false;
            }
            else
            {
                result = GlobalNew.IsGongNengOpened(client, GongNengIDs.FluorescentGem, false);
            }
            return result;
        }

        // Token: 0x06000A93 RID: 2707 RVA: 0x000A7E3C File Offset: 0x000A603C
        public bool CheckIsFluorescentGemByGoodsID(int nGoodsID)
        {
            SystemXmlItem systemGoods = null;
            return GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemGoods) && systemGoods.GetIntValue("Categoriy", -1) == 901;
        }

        // Token: 0x06000A94 RID: 2708 RVA: 0x000A7E80 File Offset: 0x000A6080
        public void OnLogin(GameClient client)
        {
            if (client != null)
            {
                if (client.ClientData.FluorescentGemData == null)
                {
                    client.ClientData.FluorescentGemData = new FluorescentGemData();
                }
                if (client.ClientData.FluorescentGemData.GemBagList == null)
                {
                    client.ClientData.FluorescentGemData.GemBagList = new List<GoodsData>();
                }
                if (client.ClientData.FluorescentGemData.GemEquipList == null)
                {
                    client.ClientData.FluorescentGemData.GemEquipList = new List<GoodsData>();
                }
                HashSet<int> usedBagIndex = new HashSet<int>();
                foreach (GoodsData gd in client.ClientData.FluorescentGemData.GemBagList)
                {
                    if (usedBagIndex.Contains(gd.BagIndex))
                    {
                        this.ResetBagAllGoods(client);
                        break;
                    }
                    usedBagIndex.Add(gd.BagIndex);
                }
                this.UpdateProps(client);
            }
        }

        // Token: 0x06000A95 RID: 2709 RVA: 0x000A7FAC File Offset: 0x000A61AC
        private void UpdateProps(GameClient client)
        {
            if (client != null)
            {
                EquipPropItem totalPros = new EquipPropItem();
                foreach (GoodsData gd in client.ClientData.FluorescentGemData.GemEquipList)
                {
                    SystemXmlItem systemGoods = null;
                    if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(gd.GoodsID, out systemGoods))
                    {
                        if (this.CheckIsFluorescentGemByGoodsID(gd.GoodsID))
                        {
                            EquipPropItem oneProp = GameManager.EquipPropsMgr.FindEquipPropItem(gd.GoodsID);
                            int i = 0;
                            while (oneProp != null && i < 177)
                            {
                                totalPros.ExtProps[i] += oneProp.ExtProps[i];
                                i++;
                            }
                        }
                    }
                }
                client.ClientData.PropsCacheManager.SetExtProps(new object[]
                {
                    17,
                    totalPros
                });
            }
        }

        // Token: 0x06000A96 RID: 2710 RVA: 0x000A80E4 File Offset: 0x000A62E4
        public bool AddFluorescentPoint(GameClient client, int nAddPoint, string reasonStr, bool notifyClient = true)
        {
            bool result;
            if (null == client)
            {
                result = false;
            }
            else if (nAddPoint <= 0)
            {
                result = false;
            }
            else
            {
                int nTotalPoint = client.ClientData.FluorescentPoint + nAddPoint;
                if (!this.UpdateFluorescentPoint2DB(client, nTotalPoint))
                {
                    result = false;
                }
                else
                {
                    client.ClientData.FluorescentPoint = nTotalPoint;
                    GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光粉末", reasonStr, "系统", client.ClientData.RoleName, "修改", nAddPoint, client.ClientData.ZoneID, client.strUserID, nTotalPoint, client.ServerId, null);
                    EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.Fluorescent, (long)nAddPoint, (long)nTotalPoint, reasonStr);
                    if (notifyClient)
                    {
                        GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FluorescentGem, nTotalPoint);
                    }
                    result = true;
                }
            }
            return result;
        }

        // Token: 0x06000A97 RID: 2711 RVA: 0x000A81B0 File Offset: 0x000A63B0
        public bool DecFluorescentPoint(GameClient client, int nDecPoint, string reasonStr, bool isGM = false)
        {
            bool result;
            if (null == client)
            {
                result = false;
            }
            else if (nDecPoint <= 0)
            {
                result = false;
            }
            else
            {
                long oldValue = (long)client.ClientData.FluorescentPoint;
                long targetValue = oldValue - (long)nDecPoint;
                if (targetValue < -2147483648L)
                {
                    result = false;
                }
                else
                {
                    int nTotalPoint = client.ClientData.FluorescentPoint - nDecPoint;
                    if (!isGM && nTotalPoint < 0)
                    {
                        result = false;
                    }
                    else if (!this.UpdateFluorescentPoint2DB(client, nTotalPoint))
                    {
                        result = false;
                    }
                    else
                    {
                        client.ClientData.FluorescentPoint = nTotalPoint;
                        GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光粉末", reasonStr, "系统", client.ClientData.RoleName, "修改", nDecPoint, client.ClientData.ZoneID, client.strUserID, nTotalPoint, client.ServerId, null);
                        EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.Fluorescent, (long)(-(long)nDecPoint), (long)nTotalPoint, reasonStr);
                        GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FluorescentGem, nTotalPoint);
                        result = true;
                    }
                }
            }
            return result;
        }

        // Token: 0x06000A98 RID: 2712 RVA: 0x000A82C0 File Offset: 0x000A64C0
        public bool UpdateFluorescentPoint2DB(GameClient client, int nTotalPoint)
        {
            bool result;
            if (null == client)
            {
                result = false;
            }
            else
            {
                string dbStrCmd = string.Format("{0}:{1}", client.ClientData.RoleID, nTotalPoint);
                byte[] dbBytesCmd = new UTF8Encoding().GetBytes(dbStrCmd);
                result = Global.sendToDB<bool, byte[]>(10208, dbBytesCmd, client.ServerId);
            }
            return result;
        }

        // Token: 0x06000A99 RID: 2713 RVA: 0x000A8320 File Offset: 0x000A6520
        public bool ModifyFluorescentPoint2DB(int rid, int nPointChg)
        {
            string dbStrCmd = string.Format("{0}:{1}", rid, nPointChg);
            byte[] dbBytesCmd = new UTF8Encoding().GetBytes(dbStrCmd);
            return Global.sendToDB<bool, byte[]>(10211, dbBytesCmd, 0);
        }

        // Token: 0x06000A9A RID: 2714 RVA: 0x000A8364 File Offset: 0x000A6564
        public GoodsData AddFluorescentGemData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
        {
            GoodsData gd = new GoodsData
            {
                Id = id,
                GoodsID = goodsID,
                Using = 0,
                Forge_level = forgeLevel,
                Starttime = startTime,
                Endtime = endTime,
                Site = site,
                Quality = quality,
                Props = "",
                GCount = goodsNum,
                Binding = binding,
                Jewellist = jewelList,
                BagIndex = bagIndex,
                AddPropIndex = addPropIndex,
                BornIndex = bornIndex,
                Lucky = lucky,
                Strong = strong,
                ExcellenceInfo = ExcellenceProperty,
                AppendPropLev = nAppendPropLev,
                ChangeLifeLevForEquip = nEquipChangeLife,
                WashProps = washProps
            };
            this.AddFluorescentGemData(client, gd);
            return gd;
        }

        // Token: 0x06000A9B RID: 2715 RVA: 0x000A8430 File Offset: 0x000A6630
        public void AddFluorescentGemData(GameClient client, GoodsData gd)
        {
            if (null != gd)
            {
                if (null == client.ClientData.FluorescentGemData)
                {
                    client.ClientData.FluorescentGemData = new FluorescentGemData();
                }
                lock (client.ClientData.FluorescentGemData)
                {
                    client.ClientData.FluorescentGemData.GemBagList.Add(gd);
                }
            }
        }

        // Token: 0x06000A9C RID: 2716 RVA: 0x000A84F0 File Offset: 0x000A66F0
        public void RemoveFluorescentGemData(GameClient client, GoodsData goodsData)
        {
            if (7000 == goodsData.Site)
            {
                lock (client.ClientData.FluorescentGemData)
                {
                    client.ClientData.FluorescentGemData.GemBagList.RemoveAll((GoodsData _g) => _g.BagIndex == goodsData.BagIndex);
                }
            }
        }

        // Token: 0x06000A9D RID: 2717 RVA: 0x000A85AC File Offset: 0x000A67AC
        public int GetIdleSlotOfFluorescentGemBag(GameClient client)
        {
            int idelPos = -1;
            int result;
            if (client.ClientData.FluorescentGemData == null || null == client.ClientData.FluorescentGemData)
            {
                result = idelPos;
            }
            else if (null == client.ClientData.GoodsDataList)
            {
                result = idelPos;
            }
            else
            {
                List<int> usedBagIndex = new List<int>();
                client.ClientData.FluorescentGemData.GemBagList.ForEach(delegate (GoodsData _g)
                {
                    usedBagIndex.Add(_g.BagIndex);
                });
                for (int i = 0; i < 220; i++)
                {
                    if (usedBagIndex.IndexOf(i) < 0)
                    {
                        idelPos = i;
                        break;
                    }
                }
                result = idelPos;
            }
            return result;
        }

        // Token: 0x06000A9E RID: 2718 RVA: 0x000A8674 File Offset: 0x000A6874
        public bool CanAddGoodsNum(GameClient client, int num)
        {
            return client != null && num > 0 && num + client.ClientData.FluorescentGemData.GemBagList.Count <= 220;
        }

        // Token: 0x06000A9F RID: 2719 RVA: 0x000A86E4 File Offset: 0x000A68E4
        public GoodsData GetGoodsByID(GameClient client, int goodsID, int bingding, string startTime, string endTime, ref int startIndex)
        {
            GoodsData result;
            if (null == client)
            {
                result = null;
            }
            else
            {
                List<GoodsData> list = new List<GoodsData>();
                lock (client.ClientData.FluorescentGemData)
                {
                    foreach (GoodsData goods in client.ClientData.FluorescentGemData.GemEquipList)
                    {
                        if (goods.GoodsID == goodsID && goods.Binding == bingding && Global.DateTimeEqual(goods.Endtime, endTime) && Global.DateTimeEqual(goods.Starttime, startTime))
                        {
                            list.Add(goods);
                        }
                    }
                    if (list == null || list.Count <= 0)
                    {
                        return null;
                    }
                    list.Sort((GoodsData x, GoodsData y) => x.BagIndex - y.BagIndex);
                    if (startIndex >= list.Count)
                    {
                        return null;
                    }
                    int i = startIndex;
                    if (i < list.Count)
                    {
                        startIndex = i + 1;
                        return list[i];
                    }
                }
                result = null;
            }
            return result;
        }

        // Token: 0x06000AA0 RID: 2720 RVA: 0x000A8880 File Offset: 0x000A6A80
        public TCPProcessCmdResults ProcessResetBagCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;
            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
                return TCPProcessCmdResults.RESULT_FAILED;
            }
            try
            {
                string[] fields = cmdData.Split(new char[]
                {
                    ':'
                });
                if (fields.Length != 1)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                int roleID = Convert.ToInt32(fields[0]);
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                if (!this.IsOpenFluorescentGem(client))
                {
                    return TCPProcessCmdResults.RESULT_OK;
                }
                this.ResetBagAllGoods(client);
                client.sendCmd<Dictionary<int, GoodsData>>(nID, this.GetBagDict(client), false);
                return TCPProcessCmdResults.RESULT_OK;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
            }
            return TCPProcessCmdResults.RESULT_FAILED;
        }

        // Token: 0x06000AA1 RID: 2721 RVA: 0x000A89F0 File Offset: 0x000A6BF0
        public TCPProcessCmdResults ProcessFluorescentGemDig(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;
            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                return TCPProcessCmdResults.RESULT_FAILED;
            }
            try
            {
                string[] fields = cmdData.Split(new char[]
                {
                    ':'
                });
                if (fields.Length != 3)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                int nRoleID = Convert.ToInt32(fields[0]);
                int nLevelType = Convert.ToInt32(fields[1]);
                int nDigType = Convert.ToInt32(fields[2]);
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (client == null || client.ClientData.RoleID != nRoleID)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                if (!this.IsOpenFluorescentGem(client))
                {
                    client.sendCmd<FluorescentGemDigTransferData>(nID, new FluorescentGemDigTransferData
                    {
                        _Result = -2
                    }, false);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                List<int> gemList = null;
                Dictionary<int, int> gemDict = null;
                EFluorescentGemDigErrorCode err;
                if (nDigType == 2)
                {
                    err = this.FluorescentGemDig_BigNum(client, nLevelType, nDigType, out gemDict);
                }
                else
                {
                    err = this.FluorescentGemDig(client, nLevelType, nDigType, out gemList);
                }
                client.sendCmd<FluorescentGemDigTransferData>(nID, new FluorescentGemDigTransferData
                {
                    _Result = (int)err,
                    _GemList = gemList,
                    _GemNumDict = gemDict
                }, false);
                return TCPProcessCmdResults.RESULT_OK;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            return TCPProcessCmdResults.RESULT_FAILED;
        }

        // Token: 0x06000AA2 RID: 2722 RVA: 0x000A8BFC File Offset: 0x000A6DFC
        public TCPProcessCmdResults ProcessFluorescentGemResolve(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;
            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                return TCPProcessCmdResults.RESULT_FAILED;
            }
            try
            {
                string[] fields = cmdData.Split(new char[]
                {
                    ':'
                });
                if (fields.Length != 3)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                int nRoleID = Convert.ToInt32(fields[0]);
                int nBagIndex = Convert.ToInt32(fields[1]);
                int nResolveCount = Convert.ToInt32(fields[2]);
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (client == null || client.ClientData.RoleID != nRoleID)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                if (!this.IsOpenFluorescentGem(client))
                {
                    client.sendCmd(nID, string.Format("{0}", -2), false);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                EFluorescentGemResolveErrorCode err = this.FluorescentGemResolve(client, nBagIndex, nResolveCount);
                string strcmd = string.Format("{0}", (int)err);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            return TCPProcessCmdResults.RESULT_FAILED;
        }

        // Token: 0x06000AA3 RID: 2723 RVA: 0x000A8DCC File Offset: 0x000A6FCC
        public TCPProcessCmdResults ProcessFluorescentGemUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            FluorescentGemUpTransferData upData = null;
            try
            {
                upData = DataHelper.BytesToObject<FluorescentGemUpTransferData>(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                return TCPProcessCmdResults.RESULT_FAILED;
            }
            try
            {
                if (null == upData)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令结构解析错误:FluorescentGemUpTransferData, CMD={0}", (TCPGameServerCmds)nID), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (client == null || client.ClientData.RoleID != upData._RoleID)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), upData._RoleID), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                if (!this.IsOpenFluorescentGem(client))
                {
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}", -2, -1), nID);
                    return TCPProcessCmdResults.RESULT_DATA;
                }
                int nNewGoodsDBID = -1;
                EFluorescentGemUpErrorCode err = this.FluorescentGemUp(client, upData, out nNewGoodsDBID);
                string strcmd = string.Format("{0}:{1}", (int)err, nNewGoodsDBID);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            return TCPProcessCmdResults.RESULT_FAILED;
        }

        // Token: 0x06000AA4 RID: 2724 RVA: 0x000A8F6C File Offset: 0x000A716C
        public TCPProcessCmdResults ProcessFluorescentGemEquip(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;
            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                return TCPProcessCmdResults.RESULT_FAILED;
            }
            try
            {
                string[] fields = cmdData.Split(new char[]
                {
                    ':'
                });
                if (fields.Length != 4)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                int nRoleID = Convert.ToInt32(fields[0]);
                int nBagIndex = Convert.ToInt32(fields[1]);
                int nPositionIndex = Convert.ToInt32(fields[2]);
                int nGemType = Convert.ToInt32(fields[3]);
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (client == null || client.ClientData.RoleID != nRoleID)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                if (!this.IsOpenFluorescentGem(client))
                {
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-2", nID);
                    return TCPProcessCmdResults.RESULT_DATA;
                }
                EFluorescentGemEquipErrorCode err = this.FluorescentGemEquip(client, nBagIndex, nPositionIndex, nGemType);
                string strcmd = string.Format("{0}", (int)err);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            return TCPProcessCmdResults.RESULT_FAILED;
        }

        // Token: 0x06000AA5 RID: 2725 RVA: 0x000A9144 File Offset: 0x000A7344
        public TCPProcessCmdResults ProcessFluorescentGemUnEquip(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;
            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                return TCPProcessCmdResults.RESULT_FAILED;
            }
            try
            {
                string[] fields = cmdData.Split(new char[]
                {
                    ':'
                });
                if (fields.Length != 3 && fields.Length != 4)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                int nRoleID = Convert.ToInt32(fields[0]);
                int nPositionIndex = Convert.ToInt32(fields[1]);
                int nUnEquipType = Convert.ToInt32(fields[2]);
                int nGemType = 0;
                if (nUnEquipType == 0)
                {
                    nGemType = Convert.ToInt32(fields[3]);
                }
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (client == null || client.ClientData.RoleID != nRoleID)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
                    return TCPProcessCmdResults.RESULT_FAILED;
                }
                if (!this.IsOpenFluorescentGem(client))
                {
                    client.sendCmd(nID, string.Format("{0}", -2), false);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                EFluorescentGemUnEquipErrorCode err = this.FluorescentGemUnEquip(client, nUnEquipType, nPositionIndex, nGemType);
                string strcmd = string.Format("{0}", (int)err);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            return TCPProcessCmdResults.RESULT_FAILED;
        }

        // Token: 0x06000AA6 RID: 2726 RVA: 0x000A933C File Offset: 0x000A753C
        public void GMClearGemBag(GameClient client)
        {
            if (null != client)
            {
                List<GoodsData> list = new List<GoodsData>(client.ClientData.FluorescentGemData.GemBagList);
                for (int i = 0; i < list.Count; i++)
                {
                    if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, list[i], list[i].GCount, false, false))
                    {
                    }
                }
                list.Clear();
                client.ClientData.FluorescentGemData.GemEquipList.Clear();
            }
        }

        // Token: 0x06000AA7 RID: 2727 RVA: 0x000A93E8 File Offset: 0x000A75E8
        public void GMAddFluorescentPoint(GameClient client, int nPoint)
        {
            if (null != client)
            {
                this.AddFluorescentPoint(client, nPoint, "GM命令增加", true);
            }
        }

        // Token: 0x06000AA8 RID: 2728 RVA: 0x000A9414 File Offset: 0x000A7614
        public void GMDecFluorescentPoint(GameClient client, int nPoint)
        {
            if (null != client)
            {
                this.DecFluorescentPoint(client, nPoint, "GM命令减少", true);
            }
        }

        // Token: 0x0400118A RID: 4490
        private Dictionary<int, FluorescentGemLevelTypeConfigData> FluorescentGemLevelTypeConfigDict = new Dictionary<int, FluorescentGemLevelTypeConfigData>();

        // Token: 0x0400118B RID: 4491
        private Dictionary<int, List<FluorescentGemDigConfigData>> FluorescentGemDigConfigDict = new Dictionary<int, List<FluorescentGemDigConfigData>>();

        // Token: 0x0400118C RID: 4492
        private Dictionary<int, FluorescentGemUpConfigData> FluorescentGemUpConfigDict = new Dictionary<int, FluorescentGemUpConfigData>();
    }
}
