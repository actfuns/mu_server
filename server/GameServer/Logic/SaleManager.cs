using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;

namespace GameServer.Logic
{
    
    public class SaleManager : IManager
    {
        
        public static SaleManager getInstance()
        {
            return SaleManager.instance;
        }

        
        public bool initialize()
        {
            SaleManager.InitConfig();
            return true;
        }

        
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessor(652, 3, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_OPENMARKET2));
            TCPCmdDispatcher.getInstance().registerProcessor(653, 3, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_MARKETSALEMONEY2));
            TCPCmdDispatcher.getInstance().registerProcessor(654, 7, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_SALEGOODS2));
            TCPCmdDispatcher.getInstance().registerProcessor(655, 1, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_SELFSALEGOODSLIST2));
            TCPCmdDispatcher.getInstance().registerProcessor(656, 2, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_OTHERSALEGOODSLIST2));
            TCPCmdDispatcher.getInstance().registerProcessor(657, 1, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_MARKETROLELIST2));
            TCPCmdDispatcher.getInstance().registerProcessor(658, 5, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_MARKETGOODSLIST2));
            TCPCmdDispatcher.getInstance().registerProcessor(659, 5, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_MARKETBUYGOODS2));
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

        
        
        
        public static double JiaoYiShuiJinBi { get; private set; }

        
        
        
        public static double JiaoYiShuiZuanShi { get; private set; }

        
        
        
        public static double JiaoYiShuiMoBi { get; private set; }

        
        
        
        public static int MaxSaleNum { get; private set; }

        
        public static void InitConfig()
        {
            SaleManager.MaxSaleNum = (int)GameManager.systemParamsList.GetParamValueIntByName("ShangJiaNumber", -1);
            double[] jiaoYiShuiParams = GameManager.systemParamsList.GetParamValueDoubleArrayByName("JiaoYiShui", ',');
            if (null != jiaoYiShuiParams)
            {
                if (jiaoYiShuiParams.Length >= 2)
                {
                    SaleManager.JiaoYiShuiJinBi = jiaoYiShuiParams[0];
                    SaleManager.JiaoYiShuiZuanShi = jiaoYiShuiParams[1];
                }
                if (jiaoYiShuiParams.Length >= 3)
                {
                    SaleManager.JiaoYiShuiMoBi = jiaoYiShuiParams[2];
                }
            }
            long maxSaleGoodsTime = GameManager.systemParamsList.GetParamValueIntByName("ShangJiaTime", -1) * 1000L;
            if (maxSaleGoodsTime > 0L)
            {
                SaleManager.MaxSaleGoodsTime = maxSaleGoodsTime;
            }
            foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.JiaoYiType.SystemXmlItemDict)
            {
                int id = kv.Key;
                int type = kv.Value.GetIntValue("Type", -1);
                Dictionary<int, List<SaleGoodsData>> item;
                if (!SaleManager._SaleGoodsDict2.TryGetValue(type, out item))
                {
                    item = new Dictionary<int, List<SaleGoodsData>>();
                    SaleManager._SaleGoodsDict2.Add(type, item);
                    SaleManager._TypeHashSet.Add(type);
                }
                if (!item.ContainsKey(id))
                {
                    item.Add(id, new List<SaleGoodsData>());
                }
                SaleManager._IDHashSet.Add(id);
                int[] goodsIDs = kv.Value.GetIntArrayValue("GoodsID", '|');
                if (null == goodsIDs)
                {
                    int[] categoriys = kv.Value.GetIntArrayValue("Categoriy", '|');
                    int occupation = kv.Value.GetIntValue("Occupation", -1);
                    if (null != categoriys)
                    {
                        foreach (int category in categoriys)
                        {
                            if (!SaleManager._Categoriy2TabIDDict.ContainsKey((long)occupation * 100000000L + (long)category))
                            {
                                SaleManager._Categoriy2TabIDDict.Add((long)occupation * 100000000L + (long)category, new int[]
                                {
                                    type,
                                    id
                                });
                            }
                        }
                    }
                    else
                    {
                        SaleManager.OthersGoodsTypeAndID = new int[]
                        {
                            type,
                            id
                        };
                    }
                }
                else
                {
                    foreach (int goodsID in goodsIDs)
                    {
                        if (!SaleManager._GoodsID2TabIDDict.ContainsKey(goodsID))
                        {
                            SaleManager._GoodsID2TabIDDict.Add(goodsID, new int[]
                            {
                                type,
                                id
                            });
                        }
                    }
                }
            }
        }

        
        public static int[] GetTypeAndID(int goodsID)
        {
            int[] typeAndID = null;
            lock (SaleManager._GoodsID2TabIDDict)
            {
                if (!SaleManager._GoodsID2TabIDDict.TryGetValue(goodsID, out typeAndID))
                {
                    int category = Global.GetGoodsCatetoriy(goodsID);
                    int occupation = Global.GetMainOccupationByGoodsID(goodsID);
                    if (SaleManager._Categoriy2TabIDDict.TryGetValue((long)(occupation * 100000000 + category), out typeAndID))
                    {
                        SaleManager._GoodsID2TabIDDict.Add(goodsID, typeAndID);
                        return typeAndID;
                    }
                }
            }
            int[] result;
            if (null == typeAndID)
            {
                result = SaleManager.OthersGoodsTypeAndID;
            }
            else
            {
                result = typeAndID;
            }
            return result;
        }

        
        public static bool IsValidType(int type, int id)
        {
            return SaleManager._TypeHashSet.Contains(type) && (id <= 0 || SaleManager._IDHashSet.Contains(id));
        }

        
        public static void AddSaleGoodsData(SaleGoodsData saleGoodsData)
        {
            int goodsID = saleGoodsData.SalingGoodsData.GoodsID;
            int[] typeAndID = SaleManager.GetTypeAndID(goodsID);
            if (null != typeAndID)
            {
                lock (SaleManager.Mutex_SaleGoodsDict)
                {
                    List<SaleGoodsData> list = SaleManager._SaleGoodsDict2[typeAndID[0]][typeAndID[1]];
                    SaleManager.UpdateOrderdList(list, saleGoodsData, true, true, SearchOrderTypes.OrderByMoney);
                    SaleManager._SaleGoodsDict[saleGoodsData.GoodsDbID] = saleGoodsData;
                    SaleManager.UpdateCachedListForSaleGoodsData(saleGoodsData, typeAndID, true);
                }
            }
        }

        
        public static void AddSaleGoodsItem(SaleGoodsItem saleGoodsItem)
        {
            SaleGoodsData saleGoodsData = new SaleGoodsData
            {
                GoodsDbID = saleGoodsItem.GoodsDbID,
                SalingGoodsData = saleGoodsItem.SalingGoodsData,
                RoleID = saleGoodsItem.Client.ClientData.RoleID,
                RoleName = Global.FormatRoleName(saleGoodsItem.Client, saleGoodsItem.Client.ClientData.RoleName),
                RoleLevel = saleGoodsItem.Client.ClientData.Level
            };
            SaleManager.AddSaleGoodsData(saleGoodsData);
        }

        
        public static void AddLiXianSaleGoodsItem(LiXianSaleGoodsItem liXianSaleGoodsItem)
        {
            SaleGoodsData saleGoodsData = new SaleGoodsData
            {
                GoodsDbID = liXianSaleGoodsItem.GoodsDbID,
                SalingGoodsData = liXianSaleGoodsItem.SalingGoodsData,
                RoleID = liXianSaleGoodsItem.RoleID,
                RoleName = liXianSaleGoodsItem.RoleName,
                RoleLevel = liXianSaleGoodsItem.RoleLevel
            };
            SaleManager.AddSaleGoodsData(saleGoodsData);
        }

        
        public static void RemoveSaleGoodsItem(int goodsDbID)
        {
            lock (SaleManager.Mutex_SaleGoodsDict)
            {
                SaleGoodsData saleGoodsData;
                if (SaleManager._SaleGoodsDict.TryGetValue(goodsDbID, out saleGoodsData))
                {
                    int goodsID = saleGoodsData.SalingGoodsData.GoodsID;
                    int[] typeAndID = SaleManager.GetTypeAndID(goodsID);
                    if (null != typeAndID)
                    {
                        List<SaleGoodsData> list = SaleManager._SaleGoodsDict2[typeAndID[0]][typeAndID[1]];
                        SaleManager.UpdateOrderdList(list, saleGoodsData, true, false, SearchOrderTypes.OrderByMoney);
                    }
                    SaleManager._SaleGoodsDict.Remove(goodsDbID);
                    SaleManager.UpdateCachedListForSaleGoodsData(saleGoodsData, typeAndID, false);
                }
            }
        }

        
        public static List<SaleGoodsData> GetSaleGoodsDataList(int type, int id = 0)
        {
            List<SaleGoodsData> saleGoodsDataList = null;
            lock (SaleManager.Mutex_SaleGoodsDict)
            {
                Dictionary<int, List<SaleGoodsData>> subDict;
                if (type == 1)
                {
                    saleGoodsDataList = new List<SaleGoodsData>();
                    foreach (KeyValuePair<int, Dictionary<int, List<SaleGoodsData>>> kv in SaleManager._SaleGoodsDict2)
                    {
                        if (kv.Value != null && kv.Value.Count > 0)
                        {
                            foreach (KeyValuePair<int, List<SaleGoodsData>> kv2 in kv.Value)
                            {
                                if (kv2.Value != null && kv2.Value.Count > 0)
                                {
                                    saleGoodsDataList.BinaryCombineDesc(kv2.Value, SaleGoodsMoneyCompare.Instance);
                                }
                            }
                        }
                    }
                }
                else if (SaleManager._SaleGoodsDict2.TryGetValue(type, out subDict))
                {
                    if (!subDict.TryGetValue(id, out saleGoodsDataList))
                    {
                        if (id <= 0 && saleGoodsDataList == null)
                        {
                            saleGoodsDataList = new List<SaleGoodsData>();
                            foreach (KeyValuePair<int, List<SaleGoodsData>> kv3 in subDict)
                            {
                                if (kv3.Key > 0 && null != kv3.Value)
                                {
                                    saleGoodsDataList.BinaryCombineDesc(kv3.Value, SaleGoodsMoneyCompare.Instance);
                                }
                            }
                        }
                    }
                }
            }
            if (null == saleGoodsDataList)
            {
                saleGoodsDataList = new List<SaleGoodsData>();
            }
            return saleGoodsDataList;
        }

        
        private static List<SaleGoodsData> GetCachedSaleGoodsList(SearchArgs searchArgs)
        {
            Predicate<SaleGoodsData> match = null;
            Predicate<SaleGoodsData> predicate2 = null;
            List<SaleGoodsData> saleGoodsDataList = null;
            SearchArgs args = new SearchArgs(searchArgs);
            int colorFlags = -1;
            int moneyFlags = -1;
            int orderBy = -1;
            int orderTypeFlags = -1;
            lock (Mutex_SaleGoodsDict)
            {
                while (!_OrderdSaleGoodsListDict.TryGetValue(searchArgs, out saleGoodsDataList))
                {
                    if (searchArgs.ColorFlags < 0x3f)
                    {
                        colorFlags = searchArgs.ColorFlags;
                        searchArgs.ColorFlags = 0x3f;
                    }
                    else if (searchArgs.MoneyFlags < 7)
                    {
                        moneyFlags = searchArgs.MoneyFlags;
                        searchArgs.MoneyFlags = 7;
                    }
                    else if (searchArgs.OrderBy > 0)
                    {
                        orderBy = searchArgs.OrderBy;
                        searchArgs.OrderBy = 0;
                    }
                    else
                    {
                        if (searchArgs.OrderTypeFlags <= 1)
                        {
                            break;
                        }
                        orderTypeFlags = searchArgs.OrderTypeFlags;
                        searchArgs.OrderTypeFlags = 0;
                    }
                }
                if (null == saleGoodsDataList)
                {
                    saleGoodsDataList = GetSaleGoodsDataList(searchArgs.Type, searchArgs.ID);
                    _OrderdSaleGoodsListDict.Add(searchArgs.Clone(), new List<SaleGoodsData>(saleGoodsDataList));
                }
                if (orderTypeFlags > 0)
                {
                    saleGoodsDataList = new List<SaleGoodsData>(saleGoodsDataList);
                    saleGoodsDataList.Sort(GetComparerFor(true, true, (SearchOrderTypes)orderTypeFlags));
                    searchArgs.OrderTypeFlags = orderTypeFlags;
                    _OrderdSaleGoodsListDict.Add(searchArgs.Clone(), saleGoodsDataList);
                }
                if (orderBy > 0)
                {
                    saleGoodsDataList = new List<SaleGoodsData>(saleGoodsDataList);
                    searchArgs.OrderBy = orderBy;
                    saleGoodsDataList.Reverse();
                    _OrderdSaleGoodsListDict.Add(searchArgs.Clone(), saleGoodsDataList);
                }
                if (moneyFlags > 0)
                {
                    searchArgs.MoneyFlags = moneyFlags;
                    saleGoodsDataList = new List<SaleGoodsData>(saleGoodsDataList);
                    if (match == null)
                    {
                        match = delegate (SaleGoodsData x)
                        {
                            bool flag;
                            if (x.SalingGoodsData.SaleMoney1 > 0)
                            {
                                flag = (moneyFlags & 1) == 0;
                            }
                            else if (x.SalingGoodsData.SaleYuanBao > 0)
                            {
                                flag = (moneyFlags & 2) == 0;
                            }
                            else
                            {
                                flag = (x.SalingGoodsData.SaleYinPiao <= 0) || ((moneyFlags & 4) == 0);
                            }
                            return flag;
                        };
                    }
                    saleGoodsDataList.RemoveAll(match);
                    _OrderdSaleGoodsListDict.Add(searchArgs.Clone(), saleGoodsDataList);
                }
                if (colorFlags > 0)
                {
                    saleGoodsDataList = new List<SaleGoodsData>(saleGoodsDataList);
                    searchArgs.ColorFlags = colorFlags;
                    if (predicate2 == null)
                    {
                        predicate2 = x => ((1 << ((Global.GetEquipColor(x.SalingGoodsData) - 1) & 0x1f)) & searchArgs.ColorFlags) == 0;
                    }
                    saleGoodsDataList.RemoveAll(predicate2);
                    _OrderdSaleGoodsListDict.Add(searchArgs.Clone(), saleGoodsDataList);
                }
            }
            return saleGoodsDataList;
        }

        
        public static int GetSaleMoneyType(GoodsData goodsData)
        {
            int result;
            if (goodsData.SaleMoney1 > 0)
            {
                result = 1;
            }
            else if (goodsData.SaleYuanBao > 0)
            {
                result = 2;
            }
            else
            {
                result = 4;
            }
            return result;
        }

        
        public static void UpdateCachedListForSaleGoodsData(SaleGoodsData saleGoodsData, int[] typeAndID, bool add)
        {
            lock (SaleManager.Mutex_SaleGoodsDict)
            {
                if (SaleManager._OrderdSaleGoodsListDict.Count != 0)
                {
                    if (null != typeAndID)
                    {
                        List<SearchArgs> list = new List<SearchArgs>();
                        int color = Global.GetEquipColor(saleGoodsData.SalingGoodsData);
                        int moneyType = SaleManager.GetSaleMoneyType(saleGoodsData.SalingGoodsData);
                        List<SearchArgs> argsList = SaleManager._OrderdSaleGoodsListDict.Keys.ToList<SearchArgs>();
                        foreach (SearchArgs key in argsList)
                        {
                            if ((key.ID == 0 && key.Type == typeAndID[0]) || key.ID == typeAndID[1] || key.Type == 1)
                            {
                                if ((1 << color - 1 & key.ColorFlags) != 0)
                                {
                                    if ((key.MoneyFlags & moneyType) != 0)
                                    {
                                        SaleManager.UpdateOrderdList(SaleManager._OrderdSaleGoodsListDict[key], saleGoodsData, key.OrderBy == 0, add, (SearchOrderTypes)key.OrderTypeFlags);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        
        private static IComparer<SaleGoodsData> GetComparerFor(bool desc, bool add, SearchOrderTypes searchOrderType)
        {
            IComparer<SaleGoodsData> compare;
            switch (searchOrderType)
            {
                case SearchOrderTypes.OrderByMoney:
                    if (desc && !add)
                    {
                        compare = SaleGoodsMoneyCompare2.Instance;
                    }
                    else
                    {
                        compare = SaleGoodsMoneyCompare.Instance;
                    }
                    return compare;
                case SearchOrderTypes.OrderByMoneyPerItem:
                    if (desc && !add)
                    {
                        compare = SaleGoodsMoneyPerItemCompare.DescInstance;
                    }
                    else
                    {
                        compare = SaleGoodsMoneyPerItemCompare.AscInstance;
                    }
                    return compare;
                case (SearchOrderTypes)3:
                    break;
                case SearchOrderTypes.OrderBySuit:
                    if (desc && !add)
                    {
                        compare = SaleGoodsSuitCompare.DescInstance;
                    }
                    else
                    {
                        compare = SaleGoodsSuitCompare.AscInstance;
                    }
                    return compare;
                default:
                    if (searchOrderType == SearchOrderTypes.OrderByNameAndColor)
                    {
                        if (desc && !add)
                        {
                            compare = SaleGoodsNameAndColorCompare.DescInstance;
                        }
                        else
                        {
                            compare = SaleGoodsNameAndColorCompare.AscInstance;
                        }
                        return compare;
                    }
                    break;
            }
            compare = SaleGoodsMoneyCompare.Instance;
            return compare;
        }

        
        private static void UpdateOrderdList(List<SaleGoodsData> list, SaleGoodsData saleGoodsData, bool desc, bool add, SearchOrderTypes searchOrderType)
        {
            if (add)
            {
                if (desc)
                {
                    list.BinaryInsertDesc(saleGoodsData, SaleManager.GetComparerFor(desc, add, searchOrderType));
                }
                else
                {
                    list.BinaryInsertAsc(saleGoodsData, SaleManager.GetComparerFor(desc, add, searchOrderType));
                }
            }
            else
            {
                int index = list.BinarySearch(saleGoodsData, SaleManager.GetComparerFor(desc, add, searchOrderType));
                if (index < 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].GoodsDbID == saleGoodsData.GoodsDbID)
                        {
                            list.RemoveAt(i);
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = index; i >= 0; i--)
                    {
                        if (list[i].GoodsDbID == saleGoodsData.GoodsDbID)
                        {
                            list.RemoveAt(i);
                            return;
                        }
                    }
                    for (int i = index + 1; i < list.Count; i++)
                    {
                        if (list[i].GoodsDbID == saleGoodsData.GoodsDbID)
                        {
                            list.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        
        private static void FixSearchArgs(SearchArgs searchArgs)
        {
            if (!SaleManager.IsValidType(searchArgs.Type, searchArgs.ID))
            {
                searchArgs.Type = 1;
                searchArgs.ID = 1;
            }
            int searchOrderType = searchArgs.OrderTypeFlags;
            for (int i = 0; i < 32; i++)
            {
                int mask = 1 << i;
                if ((searchOrderType & mask) != 0)
                {
                    searchArgs.OrderTypeFlags &= mask;
                    if (searchOrderType > 8)
                    {
                    }
                    break;
                }
            }
        }

        
        public static List<SaleGoodsData> GetSaleGoodsDataList(SearchArgs searchArgs, List<int> GoodsIds)
        {
            SaleManager.FixSearchArgs(searchArgs);
            List<SaleGoodsData> saleGoodsDataList = SaleManager.GetCachedSaleGoodsList(searchArgs);
            if (GoodsIds != null && GoodsIds.Count > 0)
            {
                saleGoodsDataList = saleGoodsDataList.FindAll((SaleGoodsData x) => GoodsIds.Contains(x.SalingGoodsData.GoodsID));
            }
            return saleGoodsDataList;
        }

        
        public const int ConstAllColorFlags = 63;

        
        public const int ConstAllMoneyFlags = 7;

        
        private static SaleManager instance = new SaleManager();

        
        private static object Mutex_SaleGoodsDict = new object();

        
        private static Dictionary<int, SaleGoodsData> _SaleGoodsDict = new Dictionary<int, SaleGoodsData>();

        
        private static Dictionary<int, Dictionary<int, List<SaleGoodsData>>> _SaleGoodsDict2 = new Dictionary<int, Dictionary<int, List<SaleGoodsData>>>();

        
        private static Dictionary<SearchArgs, List<SaleGoodsData>> _OrderdSaleGoodsListDict = new Dictionary<SearchArgs, List<SaleGoodsData>>(SearchArgs.Compare);

        
        private static Dictionary<int, int[]> _GoodsID2TabIDDict = new Dictionary<int, int[]>();

        
        private static Dictionary<long, int[]> _Categoriy2TabIDDict = new Dictionary<long, int[]>();

        
        private static HashSet<int> _TypeHashSet = new HashSet<int>();

        
        private static HashSet<int> _IDHashSet = new HashSet<int>();

        
        private static int[] OthersGoodsTypeAndID = null;

        
        public static long MaxSaleGoodsTime = 43200000L;

        
        private static Dictionary<string, Dictionary<int, int>> _SearchText2GoodsIDDict = new Dictionary<string, Dictionary<int, int>>();

        
        private static Dictionary<string, List<int>> _SearchText2GoodsIDDict2 = new Dictionary<string, List<int>>();
    }
}
