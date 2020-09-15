using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000790 RID: 1936
	public class SaleManager : IManager
	{
		// Token: 0x06003256 RID: 12886 RVA: 0x002CBCDC File Offset: 0x002C9EDC
		public static SaleManager getInstance()
		{
			return SaleManager.instance;
		}

		// Token: 0x06003257 RID: 12887 RVA: 0x002CBCF4 File Offset: 0x002C9EF4
		public bool initialize()
		{
			SaleManager.InitConfig();
			return true;
		}

		// Token: 0x06003258 RID: 12888 RVA: 0x002CBD10 File Offset: 0x002C9F10
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

		// Token: 0x06003259 RID: 12889 RVA: 0x002CBDFC File Offset: 0x002C9FFC
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600325A RID: 12890 RVA: 0x002CBE10 File Offset: 0x002CA010
		public bool destroy()
		{
			return true;
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x0600325B RID: 12891 RVA: 0x002CBE24 File Offset: 0x002CA024
		// (set) Token: 0x0600325C RID: 12892 RVA: 0x002CBE3A File Offset: 0x002CA03A
		public static double JiaoYiShuiJinBi { get; private set; }

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x0600325D RID: 12893 RVA: 0x002CBE44 File Offset: 0x002CA044
		// (set) Token: 0x0600325E RID: 12894 RVA: 0x002CBE5A File Offset: 0x002CA05A
		public static double JiaoYiShuiZuanShi { get; private set; }

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x0600325F RID: 12895 RVA: 0x002CBE64 File Offset: 0x002CA064
		// (set) Token: 0x06003260 RID: 12896 RVA: 0x002CBE7A File Offset: 0x002CA07A
		public static double JiaoYiShuiMoBi { get; private set; }

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06003261 RID: 12897 RVA: 0x002CBE84 File Offset: 0x002CA084
		// (set) Token: 0x06003262 RID: 12898 RVA: 0x002CBE9A File Offset: 0x002CA09A
		public static int MaxSaleNum { get; private set; }

		// Token: 0x06003263 RID: 12899 RVA: 0x002CBEA4 File Offset: 0x002CA0A4
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

		// Token: 0x06003264 RID: 12900 RVA: 0x002CC174 File Offset: 0x002CA374
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

		// Token: 0x06003265 RID: 12901 RVA: 0x002CC234 File Offset: 0x002CA434
		public static bool IsValidType(int type, int id)
		{
			return SaleManager._TypeHashSet.Contains(type) && (id <= 0 || SaleManager._IDHashSet.Contains(id));
		}

		// Token: 0x06003266 RID: 12902 RVA: 0x002CC26C File Offset: 0x002CA46C
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

		// Token: 0x06003267 RID: 12903 RVA: 0x002CC314 File Offset: 0x002CA514
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

		// Token: 0x06003268 RID: 12904 RVA: 0x002CC398 File Offset: 0x002CA598
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

		// Token: 0x06003269 RID: 12905 RVA: 0x002CC3F4 File Offset: 0x002CA5F4
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

		// Token: 0x0600326A RID: 12906 RVA: 0x002CC4B0 File Offset: 0x002CA6B0
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

		// Token: 0x0600326B RID: 12907 RVA: 0x002CC7E4 File Offset: 0x002CA9E4
		private static List<SaleGoodsData> GetCachedSaleGoodsList(SearchArgs searchArgs)
		{
			List<SaleGoodsData> saleGoodsDataList = null;
			SearchArgs args = new SearchArgs(searchArgs);
			int colorFlags = -1;
			int moneyFlags = -1;
			int orderBy = -1;
			int orderTypeFlags = -1;
			lock (SaleManager.Mutex_SaleGoodsDict)
			{
				while (!SaleManager._OrderdSaleGoodsListDict.TryGetValue(searchArgs, out saleGoodsDataList))
				{
					if (searchArgs.ColorFlags < 63)
					{
						colorFlags = searchArgs.ColorFlags;
						searchArgs.ColorFlags = 63;
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
							IL_15F:
							if (null == saleGoodsDataList)
							{
								saleGoodsDataList = SaleManager.GetSaleGoodsDataList(searchArgs.Type, searchArgs.ID);
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), new List<SaleGoodsData>(saleGoodsDataList));
							}
							if (orderTypeFlags > 0)
							{
								saleGoodsDataList = new List<SaleGoodsData>(saleGoodsDataList);
								saleGoodsDataList.Sort(SaleManager.GetComparerFor(true, true, (SearchOrderTypes)orderTypeFlags));
								searchArgs.OrderTypeFlags = orderTypeFlags;
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), saleGoodsDataList);
							}
							if (orderBy > 0)
							{
								saleGoodsDataList = new List<SaleGoodsData>(saleGoodsDataList);
								searchArgs.OrderBy = orderBy;
								saleGoodsDataList.Reverse();
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), saleGoodsDataList);
							}
							if (moneyFlags > 0)
							{
								searchArgs.MoneyFlags = moneyFlags;
								saleGoodsDataList = new List<SaleGoodsData>(saleGoodsDataList);
								saleGoodsDataList.RemoveAll(delegate(SaleGoodsData x)
								{
									bool result;
									if (x.SalingGoodsData.SaleMoney1 > 0)
									{
										result = ((moneyFlags & 1) == 0);
									}
									else if (x.SalingGoodsData.SaleYuanBao > 0)
									{
										result = ((moneyFlags & 2) == 0);
									}
									else
									{
										result = (x.SalingGoodsData.SaleYinPiao <= 0 || (moneyFlags & 4) == 0);
									}
									return result;
								});
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), saleGoodsDataList);
							}
							if (colorFlags > 0)
							{
								saleGoodsDataList = new List<SaleGoodsData>(saleGoodsDataList);
								searchArgs.ColorFlags = colorFlags;
								saleGoodsDataList.RemoveAll(delegate(SaleGoodsData x)
								{
									int color = Global.GetEquipColor(x.SalingGoodsData);
									return (1 << color - 1 & searchArgs.ColorFlags) == 0;
								});
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), saleGoodsDataList);
							}
							return saleGoodsDataList;
						}
						orderTypeFlags = searchArgs.OrderTypeFlags;
						searchArgs.OrderTypeFlags = 0;
					}
				}
				goto IL_15F;
			}
			return saleGoodsDataList;
		}

		// Token: 0x0600326C RID: 12908 RVA: 0x002CCB28 File Offset: 0x002CAD28
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

		// Token: 0x0600326D RID: 12909 RVA: 0x002CCB64 File Offset: 0x002CAD64
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

		// Token: 0x0600326E RID: 12910 RVA: 0x002CCD00 File Offset: 0x002CAF00
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

		// Token: 0x0600326F RID: 12911 RVA: 0x002CCDC8 File Offset: 0x002CAFC8
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

		// Token: 0x06003270 RID: 12912 RVA: 0x002CCEF4 File Offset: 0x002CB0F4
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

		// Token: 0x06003271 RID: 12913 RVA: 0x002CCFA8 File Offset: 0x002CB1A8
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

		// Token: 0x04003E81 RID: 16001
		public const int ConstAllColorFlags = 63;

		// Token: 0x04003E82 RID: 16002
		public const int ConstAllMoneyFlags = 7;

		// Token: 0x04003E83 RID: 16003
		private static SaleManager instance = new SaleManager();

		// Token: 0x04003E84 RID: 16004
		private static object Mutex_SaleGoodsDict = new object();

		// Token: 0x04003E85 RID: 16005
		private static Dictionary<int, SaleGoodsData> _SaleGoodsDict = new Dictionary<int, SaleGoodsData>();

		// Token: 0x04003E86 RID: 16006
		private static Dictionary<int, Dictionary<int, List<SaleGoodsData>>> _SaleGoodsDict2 = new Dictionary<int, Dictionary<int, List<SaleGoodsData>>>();

		// Token: 0x04003E87 RID: 16007
		private static Dictionary<SearchArgs, List<SaleGoodsData>> _OrderdSaleGoodsListDict = new Dictionary<SearchArgs, List<SaleGoodsData>>(SearchArgs.Compare);

		// Token: 0x04003E88 RID: 16008
		private static Dictionary<int, int[]> _GoodsID2TabIDDict = new Dictionary<int, int[]>();

		// Token: 0x04003E89 RID: 16009
		private static Dictionary<long, int[]> _Categoriy2TabIDDict = new Dictionary<long, int[]>();

		// Token: 0x04003E8A RID: 16010
		private static HashSet<int> _TypeHashSet = new HashSet<int>();

		// Token: 0x04003E8B RID: 16011
		private static HashSet<int> _IDHashSet = new HashSet<int>();

		// Token: 0x04003E8C RID: 16012
		private static int[] OthersGoodsTypeAndID = null;

		// Token: 0x04003E8D RID: 16013
		public static long MaxSaleGoodsTime = 43200000L;

		// Token: 0x04003E8E RID: 16014
		private static Dictionary<string, Dictionary<int, int>> _SearchText2GoodsIDDict = new Dictionary<string, Dictionary<int, int>>();

		// Token: 0x04003E8F RID: 16015
		private static Dictionary<string, List<int>> _SearchText2GoodsIDDict2 = new Dictionary<string, List<int>>();
	}
}
