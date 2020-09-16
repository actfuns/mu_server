using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Logic.Reborn;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class RebornStone
	{
		
		public static RebornStone getInstance()
		{
			return RebornStone.instance;
		}

		
		public static bool ParseRebornStoneConfig()
		{
			string fileName = Global.GameResPath(RebornStoneConst.RebornEquipDaKong);
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
			}
			try
			{
				Dictionary<int, RebornHoleStruct> RebornHoleStrHot = new Dictionary<int, RebornHoleStruct>();
				Dictionary<Dictionary<int, int>, int> ItemIDMapHot = new Dictionary<Dictionary<int, int>, int>();
				Dictionary<int, int> RebornEquipHoleMapHot = new Dictionary<int, int>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					RebornHoleStruct rebronHole = new RebornHoleStruct();
					Dictionary<int, int> UseGoods = new Dictionary<int, int>();
					Dictionary<int, int> ItemID = new Dictionary<int, int>();
					Dictionary<double, int> GaiLv = new Dictionary<double, int>();
					rebronHole.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					rebronHole.RebornEquipDengJi = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ZhuangBeiDengJie"));
					rebronHole.RebornEquipPinZhi = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ZhuangBeiPinZhi"));
					rebronHole.Count = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "DaKongShuLiang"));
					string[] UseGoodsStr = Global.GetSafeAttributeStr(xmlItem, "XiaoHaoDaoJu").Split(new char[]
					{
						','
					});
					UseGoods.Add(Convert.ToInt32(UseGoodsStr[0]), Convert.ToInt32(UseGoodsStr[1]));
					rebronHole.UseGoods = UseGoods;
					double rvalue = 0.0;
					string[] GaiLvStr = Global.GetSafeAttributeStr(xmlItem, "GaiLv").Split(new char[]
					{
						'|'
					});
					int HoleSuit = 0;
					foreach (string it in GaiLvStr)
					{
						string[] GaiLvStrOne = it.Split(new char[]
						{
							','
						});
						double rgv = Convert.ToDouble(GaiLvStrOne[1]);
						if (rgv != 0.0)
						{
							int Suit = Convert.ToInt32(GaiLvStrOne[0]);
							if (RebornEquipHoleMapHot.TryGetValue(rebronHole.RebornEquipPinZhi, out HoleSuit))
							{
								if (HoleSuit < Suit)
								{
									RebornEquipHoleMapHot[rebronHole.RebornEquipPinZhi] = Suit;
								}
							}
							else
							{
								RebornEquipHoleMapHot.Add(rebronHole.RebornEquipPinZhi, Suit);
							}
							rvalue += rgv;
							GaiLv.Add(rvalue, Suit);
						}
					}
					rebronHole.GaiLv = GaiLv;
					ItemID.Add(rebronHole.RebornEquipPinZhi, rebronHole.RebornEquipDengJi);
					ItemIDMapHot.Add(ItemID, rebronHole.ID);
					RebornHoleStrHot.Add(rebronHole.ID, rebronHole);
				}
				RebornStone.RebornHoleStr = RebornHoleStrHot;
				RebornStone.ItemIDMap = ItemIDMapHot;
				RebornStone.RebornEquipHoleMap = RebornEquipHoleMapHot;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			bool result;
			if (RebornStone.RebornHoleStr == null || RebornStone.ItemIDMap == null || RebornStone.RebornEquipHoleMap == null)
			{
				result = false;
			}
			else
			{
				Dictionary<int, double> RebornHoleExpendHot = new Dictionary<int, double>();
				List<string> SuitExpand = GameManager.systemParamsList.GetParamValueStringListByName("DaKongShuXing", '|');
				try
				{
					foreach (string it in SuitExpand)
					{
						string[] str = it.Split(new char[]
						{
							','
						});
						RebornHoleExpendHot.Add(Convert.ToInt32(str[0]), Convert.ToDouble(str[1]));
					}
					RebornStone.RebornHoleExpend = RebornHoleExpendHot;
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				if (RebornStone.RebornHoleExpend == null)
				{
					result = false;
				}
				else
				{
					fileName = Global.GameResPath(RebornStoneConst.RebornStorn);
					xml = XElement.Load(fileName);
					if (null == xml)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
					}
					try
					{
						Dictionary<int, RebornStornStruct> RebornStoneXmlHot = new Dictionary<int, RebornStornStruct>();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							RebornStornStruct rss = new RebornStornStruct();
							Dictionary<int, double> Attr = new Dictionary<int, double>();
							rss.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
							rss.StornID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "BaoShiID"));
							rss.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Type"));
							rss.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Level"));
							rss.FengYinJingShi = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "FengYinJingShi"));
							rss.ChongShengJingShi = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ChongShengJingShi"));
							rss.XuanCaiJingShi = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "XuanCaiJingShi"));
							string[] ShuXing = Global.GetSafeAttributeStr(xmlItem, "ShuXing").Split(new char[]
							{
								'|'
							});
							foreach (string it in ShuXing)
							{
								string[] str = it.Split(new char[]
								{
									','
								});
								Attr.Add((int)ConfigParser.GetPropIndexByPropName(str[0]), Convert.ToDouble(str[1]));
							}
							rss.Attr = Attr;
							RebornStoneXmlHot.Add(rss.StornID, rss);
						}
						RebornStone.RebornStoneXml = RebornStoneXmlHot;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
					if (RebornStone.RebornStoneXml == null)
					{
						result = false;
					}
					else
					{
						fileName = Global.GameResPath(RebornStoneConst.RebornStornXuanCaiCompound);
						xml = XElement.Load(fileName);
						if (null == xml)
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
						}
						try
						{
							Dictionary<int, RebornStornComp> RebornStoneComplexHot = new Dictionary<int, RebornStornComp>();
							IEnumerable<XElement> xmlItems = xml.Elements();
							foreach (XElement xmlItem in xmlItems)
							{
								RebornStornComp rsc = new RebornStornComp();
								List<int> CompNeed = new List<int>();
								rsc.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
								rsc.StornID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "BaoShiID"));
								rsc.StornAID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "HeChengBaoShiId"));
								string[] HeChengXiaoHao = Global.GetSafeAttributeStr(xmlItem, "HeChengXiaoHao").Split(new char[]
								{
									','
								});
								foreach (string it in HeChengXiaoHao)
								{
									CompNeed.Add(Convert.ToInt32(it));
								}
								rsc.CompNeed = CompNeed;
								RebornStoneComplexHot.Add(rsc.StornID, rsc);
							}
							RebornStone.RebornStoneComplex = RebornStoneComplexHot;
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
						if (RebornStone.RebornStoneComplex == null)
						{
							result = false;
						}
						else
						{
							fileName = Global.GameResPath(RebornStoneConst.RebornStornXuanCaiAttr);
							xml = XElement.Load(fileName);
							if (null == xml)
							{
								LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
							}
							try
							{
								Dictionary<int, Dictionary<int, RebornXuanCaiStorn>> RebornStoneActiveAttrHot = new Dictionary<int, Dictionary<int, RebornXuanCaiStorn>>();
								IEnumerable<XElement> xmlItems = xml.Elements();
								foreach (XElement xmlItem in xmlItems)
								{
									Dictionary<int, Dictionary<int, RebornXuanCaiStorn>> XmlData = new Dictionary<int, Dictionary<int, RebornXuanCaiStorn>>();
									Dictionary<int, double> StornAttr = new Dictionary<int, double>();
									Dictionary<int, int> StornActivity = new Dictionary<int, int>();
									RebornXuanCaiStorn rxcs = new RebornXuanCaiStorn();
									rxcs.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
									rxcs.StornID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "DaoJuID"));
									rxcs.StornLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Level"));
									string[] Attr2 = Global.GetSafeAttributeStr(xmlItem, "JiHuoShuXing").Split(new char[]
									{
										'|'
									});
									foreach (string it in Attr2)
									{
										string[] str = it.Split(new char[]
										{
											','
										});
										StornAttr.Add((int)ConfigParser.GetPropIndexByPropName(str[0]), Convert.ToDouble(str[1]));
									}
									rxcs.StornAttr = StornAttr;
									string[] Cond = Global.GetSafeAttributeStr(xmlItem, "JiHuoTiaoJian").Split(new char[]
									{
										'|'
									});
									foreach (string it in Cond)
									{
										string[] str = it.Split(new char[]
										{
											','
										});
										StornActivity.Add(Convert.ToInt32(str[0]), Convert.ToInt32(str[1]));
									}
									rxcs.StornActivity = StornActivity;
									if (RebornStoneActiveAttrHot.ContainsKey(rxcs.StornID))
									{
										RebornStoneActiveAttrHot[rxcs.StornID].Add(rxcs.ID, rxcs);
									}
									else
									{
										Dictionary<int, RebornXuanCaiStorn> XmlNumAttrMap = new Dictionary<int, RebornXuanCaiStorn>();
										XmlNumAttrMap.Add(rxcs.ID, rxcs);
										RebornStoneActiveAttrHot.Add(rxcs.StornID, XmlNumAttrMap);
									}
								}
								RebornStone.RebornStoneActiveAttr = RebornStoneActiveAttrHot;
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
							result = (RebornStone.RebornStoneActiveAttr != null);
						}
					}
				}
			}
			return result;
		}

		
		public static double GetExpandValue(int HoleSuit)
		{
			double value;
			double result;
			if (RebornStone.RebornHoleExpend.TryGetValue(HoleSuit, out value))
			{
				result = value;
			}
			else
			{
				result = 1.0;
			}
			return result;
		}

		
		public static int MakeHoleQualityOne(int Suit, int Quality)
		{
			Dictionary<int, int> Item = new Dictionary<int, int>();
			Item.Add(Quality, Suit);
			foreach (KeyValuePair<Dictionary<int, int>, int> iter in RebornStone.ItemIDMap)
			{
				foreach (KeyValuePair<int, int> it in iter.Key)
				{
					if (it.Key == Quality && it.Value == Suit)
					{
						RebornHoleStruct ItemData;
						if (RebornStone.RebornHoleStr.TryGetValue(iter.Value, out ItemData))
						{
							double rvalue = Global.GetRandom();
							foreach (KeyValuePair<double, int> itin in ItemData.GaiLv)
							{
								if (rvalue < itin.Key)
								{
									return itin.Value;
								}
							}
						}
					}
				}
			}
			return 0;
		}

		
		public Dictionary<int, Dictionary<int, int>> MakeHole(Dictionary<double, int> GaiLv, int Site)
		{
			Dictionary<int, Dictionary<int, int>> result;
			if (GaiLv == null)
			{
				result = null;
			}
			else
			{
				Dictionary<int, Dictionary<int, int>> InfoAll = new Dictionary<int, Dictionary<int, int>>();
				double rvalue = Global.GetRandom();
				Dictionary<int, int> Info = new Dictionary<int, int>();
				foreach (KeyValuePair<double, int> it in GaiLv)
				{
					if (rvalue < it.Key)
					{
						Info.Add(it.Value, 0);
						InfoAll.Add(Site, Info);
						break;
					}
				}
				result = InfoAll;
			}
			return result;
		}

		
		public static int GetGoodsHoleInfo(GoodsData gd, int Site, out Dictionary<int, Dictionary<int, int>> HoleInfo, SystemXmlItem systemGoods)
		{
			HoleInfo = null;
			int SuitID = systemGoods.GetIntValue("SuitID", -1);
			int Quality = RebornEquip.GetCurrGoodsQuality(gd);
			lock (RebornStone.RebornHoleStr)
			{
				foreach (KeyValuePair<int, RebornHoleStruct> it in RebornStone.RebornHoleStr)
				{
					if (it.Value.RebornEquipDengJi == SuitID && Quality == it.Value.RebornEquipPinZhi)
					{
						HoleInfo = RebornStone.getInstance().MakeHole(it.Value.GaiLv, Site);
						if (HoleInfo == null)
						{
							return 0;
						}
						return it.Key;
					}
				}
			}
			return 0;
		}

		
		public static string MakeHoleInfo(Dictionary<int, Dictionary<int, int>> HoleInfo, int XuanCaiID)
		{
			string HoleInfoStr = "";
			lock (HoleInfo)
			{
				Dictionary<int, int> Make;
				if (HoleInfo.TryGetValue(-1, out Make))
				{
					int XuanCaiGood;
					if (Make.TryGetValue(-1, out XuanCaiGood))
					{
						SystemXmlItem systemGoods = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(XuanCaiGood, out systemGoods))
						{
							HoleInfo.Remove(-1);
						}
					}
				}
				if (XuanCaiID != 0)
				{
					HoleInfoStr = "-1_-1_" + XuanCaiID.ToString() + "|";
				}
				foreach (KeyValuePair<int, Dictionary<int, int>> iter in HoleInfo)
				{
					foreach (KeyValuePair<int, int> it in iter.Value)
					{
						HoleInfoStr = string.Concat(new string[]
						{
							HoleInfoStr,
							iter.Key.ToString(),
							"_",
							it.Key.ToString(),
							"_",
							it.Value.ToString(),
							"|"
						});
					}
				}
			}
			return HoleInfoStr;
		}

		
		public static Dictionary<int, Dictionary<int, int>> ParessMakeHoleInfo(string HoleInfoStr)
		{
			Dictionary<int, Dictionary<int, int>> HoleInfo = new Dictionary<int, Dictionary<int, int>>();
			string[] StrArr = HoleInfoStr.Split(new char[]
			{
				'|'
			});
			foreach (string iter in StrArr)
			{
				if (iter.Length >= 3)
				{
					Dictionary<int, int> HoleStone = new Dictionary<int, int>();
					string[] Info = iter.Split(new char[]
					{
						'_'
					});
					if (Info.Length >= 3)
					{
						HoleStone.Add(Convert.ToInt32(Info[1]), Convert.ToInt32(Info[2]));
						int key = Convert.ToInt32(Info[0]);
						if (!HoleInfo.ContainsKey(key))
						{
							HoleInfo.Add(key, HoleStone);
						}
					}
				}
			}
			return HoleInfo;
		}

		
		public static bool UpdateGoodsPropsToDB(GameClient client, GoodsData goodsData)
		{
			string[] dbFields = null;
			string strDbCmd = Global.FormatUpdateDBGoodsStr(new object[]
			{
				client.ClientData.RoleID,
				goodsData.Id,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				goodsData.Props,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				goodsData.Binding,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*"
			});
			TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strDbCmd, out dbFields, client.ServerId);
			bool result;
			if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = false;
			}
			else if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
			{
				result = false;
			}
			else
			{
				GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 3, goodsData.Id, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, 1);
				result = true;
			}
			return result;
		}

		
		public static Dictionary<int, int> GetUseGoodsByEquipSuitAndQuality(int Suit, int Quality)
		{
			foreach (KeyValuePair<int, RebornHoleStruct> it in RebornStone.RebornHoleStr)
			{
				if (it.Value.RebornEquipDengJi == Suit && it.Value.RebornEquipPinZhi == Quality)
				{
					return it.Value.UseGoods;
				}
			}
			return null;
		}

		
		public static bool RebornHoleRemoveUseGoods(GameClient client, GoodsData UseGood, int Count, out bool bUsedBinding)
		{
			bUsedBinding = false;
			bool result;
			if (UseGood != null && UseGood.GCount >= Count)
			{
				bool bUsedTimeLimited = false;
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, UseGood, Count, false, out bUsedBinding, out bUsedTimeLimited, false))
				{
					result = false;
				}
				else
				{
					GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 4, UseGood.Id, UseGood.Using, UseGood.Site, UseGood.GCount, UseGood.BagIndex, 1);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public static bool RebornUseCaleMothed(Dictionary<int, Dictionary<bool, GoodsData>> GoodsDict, Dictionary<int, Dictionary<int, GoodsData>> UseGoodInfo, int Count)
		{
			int GroupUseNum = 0;
			bool flag = false;
			foreach (KeyValuePair<int, Dictionary<bool, GoodsData>> unbindGood in GoodsDict)
			{
				using (Dictionary<bool, GoodsData>.Enumerator enumerator2 = unbindGood.Value.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						KeyValuePair<bool, GoodsData> it = enumerator2.Current;
						if (it.Key)
						{
							Dictionary<int, GoodsData> GoodInfo = new Dictionary<int, GoodsData>();
							GoodInfo.Add(Count, it.Value);
							UseGoodInfo.Add(it.Value.Id, GoodInfo);
							flag = true;
						}
						else
						{
							GroupUseNum += it.Value.GCount;
							if (!flag && GroupUseNum > Count)
							{
								Dictionary<int, GoodsData> GoodInfo = new Dictionary<int, GoodsData>();
								GoodInfo.Add(it.Value.GCount - GroupUseNum + Count, it.Value);
								UseGoodInfo.Add(it.Value.Id, GoodInfo);
								flag = true;
							}
							else
							{
								Dictionary<int, GoodsData> OldGoodInfo = new Dictionary<int, GoodsData>();
								OldGoodInfo.Add(it.Value.GCount, it.Value);
								UseGoodInfo.Add(it.Value.Id, OldGoodInfo);
								if (!flag && GroupUseNum == Count)
								{
									flag = true;
								}
							}
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		
		public static bool RebornUseGoodHasBinding(GameClient client, int UseGoodID, int Count, int Binding, out bool EquipBindUse)
		{
			EquipBindUse = false;
			bool result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = false;
			}
			else if (Count == 0)
			{
				result = true;
			}
			else
			{
				Dictionary<int, Dictionary<bool, GoodsData>> BindGoodsDict = new Dictionary<int, Dictionary<bool, GoodsData>>();
				Dictionary<int, Dictionary<bool, GoodsData>> UnBindGoodsDict = new Dictionary<int, Dictionary<bool, GoodsData>>();
				int BindAllNum = 0;
				int UnBindAllNum = 0;
				for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
				{
					if (client.ClientData.RebornGoodsDataList[i].Using < 1)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == UseGoodID)
						{
							if (client.ClientData.RebornGoodsDataList[i].Binding == 1)
							{
								Dictionary<bool, GoodsData> bindNum = new Dictionary<bool, GoodsData>();
								if (client.ClientData.RebornGoodsDataList[i].GCount >= Count)
								{
									bindNum.Add(true, client.ClientData.RebornGoodsDataList[i]);
								}
								else
								{
									bindNum.Add(false, client.ClientData.RebornGoodsDataList[i]);
								}
								BindGoodsDict.Add(client.ClientData.RebornGoodsDataList[i].Id, bindNum);
								BindAllNum += client.ClientData.RebornGoodsDataList[i].GCount;
							}
							else
							{
								Dictionary<bool, GoodsData> unBindNum = new Dictionary<bool, GoodsData>();
								if (client.ClientData.RebornGoodsDataList[i].GCount >= Count)
								{
									unBindNum.Add(true, client.ClientData.RebornGoodsDataList[i]);
								}
								else
								{
									unBindNum.Add(false, client.ClientData.RebornGoodsDataList[i]);
								}
								UnBindGoodsDict.Add(client.ClientData.RebornGoodsDataList[i].Id, unBindNum);
								UnBindAllNum += client.ClientData.RebornGoodsDataList[i].GCount;
							}
						}
					}
				}
				if (BindAllNum + UnBindAllNum < Count)
				{
					result = false;
				}
				else
				{
					Dictionary<int, Dictionary<int, GoodsData>> UseGoodInfo = new Dictionary<int, Dictionary<int, GoodsData>>();
					lock (UseGoodInfo)
					{
						if (Binding == 1)
						{
							if (BindAllNum >= Count)
							{
								if (BindGoodsDict != null && BindGoodsDict.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(BindGoodsDict, UseGoodInfo, Count))
									{
										return false;
									}
								}
							}
							else
							{
								int UseBindNum = 0;
								if (BindGoodsDict != null && BindGoodsDict.Count > 0)
								{
									foreach (KeyValuePair<int, Dictionary<bool, GoodsData>> iter in BindGoodsDict)
									{
										using (Dictionary<bool, GoodsData>.Enumerator enumerator2 = iter.Value.GetEnumerator())
										{
											if (enumerator2.MoveNext())
											{
												KeyValuePair<bool, GoodsData> it = enumerator2.Current;
												Dictionary<int, GoodsData> GoodInfo = new Dictionary<int, GoodsData>();
												GoodInfo.Add(it.Value.GCount, it.Value);
												UseGoodInfo.Add(it.Value.Id, GoodInfo);
												UseBindNum += it.Value.GCount;
											}
										}
									}
								}
								int UseUnBindNum = Count - UseBindNum;
								if (UseUnBindNum <= 0)
								{
									return false;
								}
								if (UnBindGoodsDict != null && UnBindGoodsDict.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(UnBindGoodsDict, UseGoodInfo, UseUnBindNum))
									{
										return false;
									}
								}
							}
						}
						else
						{
							if (UnBindGoodsDict == null)
							{
								return false;
							}
							if (UnBindAllNum >= Count)
							{
								if (UnBindGoodsDict != null && UnBindGoodsDict.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(UnBindGoodsDict, UseGoodInfo, Count))
									{
										return false;
									}
								}
							}
							else
							{
								int UseUnBindNum = 0;
								if (UnBindGoodsDict != null && UnBindGoodsDict.Count > 0)
								{
									foreach (KeyValuePair<int, Dictionary<bool, GoodsData>> iter in UnBindGoodsDict)
									{
										using (Dictionary<bool, GoodsData>.Enumerator enumerator2 = iter.Value.GetEnumerator())
										{
											if (enumerator2.MoveNext())
											{
												KeyValuePair<bool, GoodsData> it = enumerator2.Current;
												Dictionary<int, GoodsData> GoodInfo = new Dictionary<int, GoodsData>();
												GoodInfo.Add(it.Value.GCount, it.Value);
												UseGoodInfo.Add(it.Value.Id, GoodInfo);
												UseUnBindNum += it.Value.GCount;
											}
										}
									}
								}
								int UseUnBindNumCount = Count - UseUnBindNum;
								if (UseUnBindNumCount <= 0)
								{
									return false;
								}
								if (BindGoodsDict != null && BindGoodsDict.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(BindGoodsDict, UseGoodInfo, UseUnBindNumCount))
									{
										return false;
									}
								}
							}
						}
						foreach (KeyValuePair<int, Dictionary<int, GoodsData>> iter2 in UseGoodInfo)
						{
							using (Dictionary<int, GoodsData>.Enumerator enumerator4 = iter2.Value.GetEnumerator())
							{
								if (enumerator4.MoveNext())
								{
									KeyValuePair<int, GoodsData> it2 = enumerator4.Current;
									if (!RebornStone.RebornHoleRemoveUseGoods(client, it2.Value, it2.Key, out EquipBindUse))
									{
										return false;
									}
								}
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		
		public static Dictionary<int, Dictionary<int, GoodsData>> GetDictRebornUseGoodHasBinding(GameClient client, int UseGoodID, int Count, int Binding, out bool EquipBindUse)
		{
			EquipBindUse = false;
			Dictionary<int, Dictionary<int, GoodsData>> result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = null;
			}
			else if (Count == 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, Dictionary<bool, GoodsData>> BindGoodsDict = new Dictionary<int, Dictionary<bool, GoodsData>>();
				Dictionary<int, Dictionary<bool, GoodsData>> UnBindGoodsDict = new Dictionary<int, Dictionary<bool, GoodsData>>();
				int BindAllNum = 0;
				int UnBindAllNum = 0;
				for (int i = 0; i < client.ClientData.RebornBagNum; i++)
				{
					if (i >= client.ClientData.RebornGoodsDataList.Count)
					{
						break;
					}
					if (client.ClientData.RebornGoodsDataList[i].GoodsID == UseGoodID)
					{
						if (client.ClientData.RebornGoodsDataList[i].Binding == 1)
						{
							Dictionary<bool, GoodsData> bindNum = new Dictionary<bool, GoodsData>();
							if (client.ClientData.RebornGoodsDataList[i].GCount >= Count)
							{
								bindNum.Add(true, client.ClientData.RebornGoodsDataList[i]);
							}
							else
							{
								bindNum.Add(false, client.ClientData.RebornGoodsDataList[i]);
							}
							BindGoodsDict.Add(client.ClientData.RebornGoodsDataList[i].Id, bindNum);
							BindAllNum += client.ClientData.RebornGoodsDataList[i].GCount;
						}
						else
						{
							Dictionary<bool, GoodsData> unBindNum = new Dictionary<bool, GoodsData>();
							if (client.ClientData.RebornGoodsDataList[i].GCount >= Count)
							{
								unBindNum.Add(true, client.ClientData.RebornGoodsDataList[i]);
							}
							else
							{
								unBindNum.Add(false, client.ClientData.RebornGoodsDataList[i]);
							}
							UnBindGoodsDict.Add(client.ClientData.RebornGoodsDataList[i].Id, unBindNum);
							UnBindAllNum += client.ClientData.RebornGoodsDataList[i].GCount;
						}
					}
				}
				if (BindAllNum + UnBindAllNum < Count)
				{
					result = null;
				}
				else
				{
					Dictionary<int, Dictionary<int, GoodsData>> UseGoodInfo = new Dictionary<int, Dictionary<int, GoodsData>>();
					lock (UseGoodInfo)
					{
						if (Binding == 0)
						{
							if (BindAllNum >= Count)
							{
								if (BindGoodsDict != null && BindGoodsDict.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(BindGoodsDict, UseGoodInfo, Count))
									{
										return null;
									}
								}
							}
							else
							{
								int UseBindNum = 0;
								if (BindGoodsDict != null && BindGoodsDict.Count > 0)
								{
									foreach (KeyValuePair<int, Dictionary<bool, GoodsData>> iter in BindGoodsDict)
									{
										using (Dictionary<bool, GoodsData>.Enumerator enumerator2 = iter.Value.GetEnumerator())
										{
											if (enumerator2.MoveNext())
											{
												KeyValuePair<bool, GoodsData> it = enumerator2.Current;
												Dictionary<int, GoodsData> GoodInfo = new Dictionary<int, GoodsData>();
												GoodInfo.Add(it.Value.GCount, it.Value);
												UseGoodInfo.Add(it.Value.Id, GoodInfo);
												UseBindNum += it.Value.GCount;
											}
										}
									}
								}
								int UseUnBindNum = Count - UseBindNum;
								if (UseUnBindNum <= 0)
								{
									return null;
								}
								if (UnBindGoodsDict != null && UnBindGoodsDict.Count > 0)
								{
									if (!RebornStone.RebornUseCaleMothed(UnBindGoodsDict, UseGoodInfo, UseUnBindNum))
									{
										return null;
									}
								}
							}
						}
						else
						{
							if (BindGoodsDict == null)
							{
								return null;
							}
							if (BindAllNum < Count)
							{
								return null;
							}
							if (BindGoodsDict != null && BindGoodsDict.Count > 0)
							{
								if (!RebornStone.RebornUseCaleMothed(BindGoodsDict, UseGoodInfo, Count))
								{
									return null;
								}
							}
						}
					}
					result = UseGoodInfo;
				}
			}
			return result;
		}

		
		public static void ActiveXuanCaiAttr(GameClient client, GoodsData goodsData, Dictionary<int, int> Active, int StoneID)
		{
			double[] _ExtProps = new double[177];
			Dictionary<int, RebornXuanCaiStorn> ActiveAttr;
			if (RebornStone.RebornStoneActiveAttr.TryGetValue(StoneID, out ActiveAttr))
			{
				lock (ActiveAttr)
				{
					Dictionary<int, double> StornAttr = new Dictionary<int, double>();
					Dictionary<int, double> StornAttr2 = new Dictionary<int, double>();
					int Count = 0;
					int MapNum = 0;
					foreach (RebornXuanCaiStorn iter in ActiveAttr.Values)
					{
						foreach (KeyValuePair<int, int> it in iter.StornActivity)
						{
							int value;
							if (Active.TryGetValue(it.Key, out value))
							{
								if (value >= it.Value)
								{
									bool temp;
									if (iter.StornActivity.Count >= 2)
									{
										MapNum++;
										temp = true;
									}
									else
									{
										temp = false;
									}
									int AttrCount = 0;
									foreach (KeyValuePair<int, double> it2 in iter.StornAttr)
									{
										if (!temp)
										{
											if (!StornAttr.ContainsKey(it2.Key))
											{
												StornAttr.Add(it2.Key, it2.Value);
											}
											else
											{
												Dictionary<int, double> dictionary;
												int key;
												(dictionary = StornAttr)[key = it2.Key] = dictionary[key] + it2.Value;
											}
										}
										else
										{
											AttrCount++;
											if (MapNum == AttrCount)
											{
												if (!StornAttr2.ContainsKey(it2.Key))
												{
													StornAttr2.Add(it2.Key, it2.Value);
													Count++;
												}
												else
												{
													Dictionary<int, double> dictionary;
													int key;
													(dictionary = StornAttr2)[key = it2.Key] = dictionary[key] + it2.Value;
													Count++;
												}
											}
										}
									}
								}
							}
						}
					}
					if (StornAttr != null)
					{
						foreach (KeyValuePair<int, double> it3 in StornAttr)
						{
							_ExtProps[it3.Key] = it3.Value;
						}
					}
					if (StornAttr2 != null && Count == 2)
					{
						foreach (KeyValuePair<int, double> it3 in StornAttr2)
						{
							_ExtProps[it3.Key] = it3.Value;
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.RebornXuanCaiStone,
					_ExtProps
				});
			}
		}

		
		public static bool IsXuanCaiStone(GoodsData goodsData, out GoodsData OutGoodsData, out int XuanCaiStone)
		{
			XuanCaiStone = 0;
			OutGoodsData = null;
			SystemXmlItem systemGoods;
			bool result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
			{
				result = false;
			}
			else if (37 != systemGoods.GetIntValue("Categoriy", -1))
			{
				result = false;
			}
			else
			{
				Dictionary<int, Dictionary<int, int>> StoneInfo = RebornStone.ParessMakeHoleInfo(goodsData.Props);
				if (StoneInfo == null)
				{
					result = false;
				}
				else
				{
					int GoodID = 0;
					lock (StoneInfo)
					{
						Dictionary<int, int> HoleInfo;
						if (!StoneInfo.TryGetValue(-1, out HoleInfo))
						{
							return false;
						}
						if (!HoleInfo.TryGetValue(-1, out GoodID))
						{
							return false;
						}
					}
					SystemXmlItem systemGoods2;
					if (GoodID == 0)
					{
						result = false;
					}
					else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(GoodID, out systemGoods2))
					{
						result = false;
					}
					else if (960 != systemGoods2.GetIntValue("Categoriy", -1))
					{
						result = false;
					}
					else
					{
						XuanCaiStone = GoodID;
						OutGoodsData = goodsData;
						result = true;
					}
				}
			}
			return result;
		}

		
		public static void VoidXuanCaiProps(GameClient client)
		{
			double[] _ExtProps = new double[177];
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.RebornXuanCaiStone,
				_ExtProps
			});
		}

		
		public static void RefreshProps(GameClient client, Dictionary<int, double> StoneAllAttr)
		{
			double[] _ExtProps = new double[177];
			if (StoneAllAttr != null && StoneAllAttr.Count > 0)
			{
				foreach (KeyValuePair<int, double> it in StoneAllAttr)
				{
					_ExtProps[it.Key] = it.Value;
				}
			}
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.RebornStone,
				_ExtProps
			});
		}

		
		public static void GetRefreshProps(GameClient client, GoodsData goodsData, Dictionary<int, int> Active, Dictionary<int, double> StoneAllAttr)
		{
			if (!string.IsNullOrEmpty(goodsData.Props))
			{
				Dictionary<int, Dictionary<int, int>> StoneInfo = RebornStone.ParessMakeHoleInfo(goodsData.Props);
				if (StoneInfo != null)
				{
					lock (StoneInfo)
					{
						foreach (Dictionary<int, int> iter in StoneInfo.Values)
						{
							foreach (KeyValuePair<int, int> it in iter)
							{
								RebornStornStruct OutInfo;
								if (it.Value != 0 && RebornStone.RebornStoneXml.TryGetValue(it.Value, out OutInfo))
								{
									double Rase;
									if (!RebornStone.RebornHoleExpend.TryGetValue(it.Key, out Rase))
									{
										Rase = 0.0;
									}
									int Index;
									if (RebornEquip.IsRebornEquipAttackOrDefense(goodsData.GoodsID, out Index))
									{
										int temp = 0;
										foreach (KeyValuePair<int, double> Attr in OutInfo.Attr)
										{
											if (temp == 3)
											{
												break;
											}
											if (temp == Index)
											{
												if (StoneAllAttr.ContainsKey(Attr.Key))
												{
													int key;
													StoneAllAttr[key = Attr.Key] = StoneAllAttr[key] + Attr.Value * Rase;
												}
												else
												{
													StoneAllAttr.Add(Attr.Key, Attr.Value * Rase);
												}
											}
											temp++;
										}
										if (Active.ContainsKey(OutInfo.Type))
										{
											int key;
											Active[key = OutInfo.Type] = Active[key] + OutInfo.Level;
										}
										else if (OutInfo.Type >= 1 && OutInfo.Type <= 5)
										{
											Active.Add(OutInfo.Type, OutInfo.Level);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public static RebornStornOpcode ProessMakeRebornEquipHold(GameClient client, int DBID, int Bind, int IsReset, int Number, out string prop, out int bind)
		{
			prop = "";
			bind = 0;
			RebornStornOpcode result;
			if (Bind != 0 && Bind != 1)
			{
				result = RebornStornOpcode.RebornUseBind;
			}
			else
			{
				GoodsData gd = RebornEquip.GetRebornGoodsByDbID(client, DBID);
				SystemXmlItem systemGoods;
				if (gd == null)
				{
					result = RebornStornOpcode.RebornNotExist;
				}
				else if (!RebornEquip.IsRebornEquip(gd.GoodsID))
				{
					result = RebornStornOpcode.RebornNotEquip;
				}
				else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(gd.GoodsID, out systemGoods))
				{
					result = RebornStornOpcode.RebornNotExistGoods;
				}
				else if (IsReset == 1)
				{
					if (gd.Props != null && gd.Props == "")
					{
						result = RebornStornOpcode.RebornNotInfo;
					}
					else
					{
						int CurrQuality = RebornEquip.GetCurrGoodsQuality(gd);
						int MaxHoleQuality;
						if (!RebornStone.RebornEquipHoleMap.TryGetValue(CurrQuality, out MaxHoleQuality))
						{
							result = RebornStornOpcode.RebornNotFindMaxQuality;
						}
						else
						{
							int SuitID = systemGoods.GetIntValue("SuitID", -1);
							Dictionary<int, int> UseGood = RebornStone.GetUseGoodsByEquipSuitAndQuality(SuitID, CurrQuality);
							if (UseGood == null)
							{
								result = RebornStornOpcode.RebornUseMatterrislNull;
							}
							else
							{
								bool EquipBindUse = false;
								foreach (KeyValuePair<int, int> it in UseGood)
								{
									if (!RebornStone.RebornUseGoodHasBinding(client, it.Key, it.Value, Bind, out EquipBindUse))
									{
										return RebornStornOpcode.RebornUseMaterrislErr;
									}
								}
								if (EquipBindUse)
								{
									gd.Binding = 1;
								}
								Dictionary<int, Dictionary<int, int>> DataInfo = RebornStone.ParessMakeHoleInfo(gd.Props);
								lock (DataInfo)
								{
									if (Number <= 0 || DataInfo == null || !DataInfo.ContainsKey(Number))
									{
										return RebornStornOpcode.RebornHoleSiteErr;
									}
									int Quality = RebornStone.MakeHoleQualityOne(SuitID, CurrQuality);
									if (Quality == 0)
									{
										return RebornStornOpcode.RebornRandomHoleErr;
									}
									foreach (KeyValuePair<int, int> it in DataInfo[Number])
									{
										if (it.Key == MaxHoleQuality)
										{
											return RebornStornOpcode.RebornMaxQuality;
										}
										DataInfo[Number] = new Dictionary<int, int>
										{
											{
												Quality,
												it.Value
											}
										};
									}
									gd.Props = RebornStone.MakeHoleInfo(DataInfo, 0);
								}
								if (!RebornStone.UpdateGoodsPropsToDB(client, gd))
								{
									result = RebornStornOpcode.RebornUpdateInfoErr;
								}
								else
								{
									prop = gd.Props;
									bind = gd.Binding;
									result = RebornStornOpcode.RebornHoleSucc;
								}
							}
						}
					}
				}
				else
				{
					bool flag = false;
					Dictionary<int, Dictionary<int, int>> DataInfo = null;
					if (gd.Props != null && gd.Props != "")
					{
						DataInfo = RebornStone.ParessMakeHoleInfo(gd.Props);
						lock (DataInfo)
						{
							Dictionary<int, int> Info;
							if (DataInfo.TryGetValue(Number, out Info))
							{
								return RebornStornOpcode.RebornHasHole;
							}
							flag = true;
						}
					}
					Dictionary<int, Dictionary<int, int>> HoleInfo;
					int ItemID = RebornStone.GetGoodsHoleInfo(gd, Number, out HoleInfo, systemGoods);
					if (ItemID == 0)
					{
						result = RebornStornOpcode.RebornMakeHoleErr;
					}
					else
					{
						bool EquipBindUse = false;
						RebornHoleStruct Hole;
						if (RebornStone.RebornHoleStr.TryGetValue(ItemID, out Hole))
						{
							lock (Hole)
							{
								foreach (KeyValuePair<int, int> it in Hole.UseGoods)
								{
									if (!RebornStone.RebornUseGoodHasBinding(client, it.Key, it.Value, Bind, out EquipBindUse))
									{
										return RebornStornOpcode.RebornUseMaterrislErr;
									}
								}
							}
						}
						if (EquipBindUse)
						{
							gd.Binding = 1;
						}
						if (HoleInfo != null)
						{
							lock (HoleInfo)
							{
								if (flag && DataInfo != null)
								{
									using (Dictionary<int, Dictionary<int, int>>.Enumerator enumerator2 = HoleInfo.GetEnumerator())
									{
										if (enumerator2.MoveNext())
										{
											KeyValuePair<int, Dictionary<int, int>> it2 = enumerator2.Current;
											DataInfo.Add(it2.Key, it2.Value);
										}
									}
									gd.Props = RebornStone.MakeHoleInfo(DataInfo, 0);
								}
								else if (!flag && DataInfo == null)
								{
									gd.Props = RebornStone.MakeHoleInfo(HoleInfo, 0);
								}
								if (!RebornStone.UpdateGoodsPropsToDB(client, gd))
								{
									return RebornStornOpcode.RebornUpdateInfoErr;
								}
								prop = gd.Props;
								bind = gd.Binding;
							}
						}
						result = RebornStornOpcode.RebornHoleSucc;
					}
				}
			}
			return result;
		}

		
		public static RebornStornOpcode ProessRebornStoneInlayHold(GameClient client, int EquipDBID, int StoneDBID, int Number, out string prop, out int bind)
		{
			prop = "";
			bind = 0;
			GoodsData Equip = RebornEquip.GetRebornGoodsByDbID(client, EquipDBID);
			RebornStornOpcode result;
			if (Equip == null)
			{
				result = RebornStornOpcode.RebornInlayNotExistEquip;
			}
			else
			{
				GoodsData Stone = RebornEquip.GetRebornGoodsByDbID(client, StoneDBID);
				if (Stone == null)
				{
					result = RebornStornOpcode.RebornInlayNotExistStone;
				}
				else if (!RebornEquip.IsRebornEquip(Equip.GoodsID))
				{
					result = RebornStornOpcode.RebornInlayNotEquip;
				}
				else
				{
					if (Number == -1)
					{
						Dictionary<int, Dictionary<int, int>> HoleInfo = null;
						if (Equip.Props == null || Equip.Props == "")
						{
							HoleInfo = new Dictionary<int, Dictionary<int, int>>();
						}
						else
						{
							HoleInfo = RebornStone.ParessMakeHoleInfo(Equip.Props);
						}
						if (!RebornEquip.IsRebornEquipShengWu(Equip.GoodsID))
						{
							return RebornStornOpcode.RebornInlayNotEquip;
						}
						if (!RebornStone.RebornStoneActiveAttr.ContainsKey(Stone.GoodsID))
						{
							return RebornStornOpcode.RebornInlayNotXuanCai;
						}
						Dictionary<int, int> NewInfo = new Dictionary<int, int>();
						if (HoleInfo.TryGetValue(Number, out NewInfo))
						{
							int value = 0;
							if (NewInfo.TryGetValue(Number, out value))
							{
								if (value != 0)
								{
									return RebornStornOpcode.RebornInlayCurrSiteHasStone;
								}
							}
						}
						Equip.Props = RebornStone.MakeHoleInfo(HoleInfo, Stone.GoodsID);
						if (!RebornStone.UpdateGoodsPropsToDB(client, Equip))
						{
							return RebornStornOpcode.RebornInlayUpdateInfoErr;
						}
					}
					else
					{
						if (Equip.Props == null || Equip.Props == "")
						{
							return RebornStornOpcode.RebornInlayNotMakeHole;
						}
						Dictionary<int, Dictionary<int, int>> HoleInfo = RebornStone.ParessMakeHoleInfo(Equip.Props);
						if (HoleInfo == null)
						{
							return RebornStornOpcode.RebornInlayGetInfoErr;
						}
						lock (HoleInfo)
						{
							Dictionary<int, int> Info;
							if (!HoleInfo.TryGetValue(Number, out Info))
							{
								return RebornStornOpcode.RebornInlayNotHoleSite;
							}
							if (!RebornStone.RebornStoneXml.ContainsKey(Stone.GoodsID))
							{
								return RebornStornOpcode.RebornInlayNotChongSheng;
							}
							Dictionary<int, int> NewInfo = new Dictionary<int, int>();
							using (Dictionary<int, int>.Enumerator enumerator = HoleInfo[Number].GetEnumerator())
							{
								if (enumerator.MoveNext())
								{
									KeyValuePair<int, int> it = enumerator.Current;
									if (it.Value != 0)
									{
										return RebornStornOpcode.RebornInlayCurrSiteHasStone;
									}
									NewInfo.Add(it.Key, Stone.GoodsID);
								}
							}
							HoleInfo[Number] = NewInfo;
							int StoneID = 0;
							Dictionary<int, int> value2;
							if (HoleInfo.TryGetValue(-1, out value2))
							{
								int XuanCaiID;
								if (value2.TryGetValue(-1, out XuanCaiID))
								{
									StoneID = XuanCaiID;
								}
							}
							Equip.Props = RebornStone.MakeHoleInfo(HoleInfo, StoneID);
							if (!RebornStone.UpdateGoodsPropsToDB(client, Equip))
							{
								return RebornStornOpcode.RebornInlayUpdateInfoErr;
							}
						}
					}
					prop = Equip.Props;
					bool BindUse;
					if (!RebornStone.RebornHoleRemoveUseGoods(client, Stone, 1, out BindUse))
					{
						result = RebornStornOpcode.RebornInlayUpdateStoneInfoErr;
					}
					else
					{
						if (BindUse)
						{
							Equip.Binding = 1;
						}
						bind = Equip.Binding;
						if (Equip.Using == 1)
						{
							Global.RefreshEquipProp(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						}
						GameManager.logDBCmdMgr.AddDBLogInfo(Equip.Id, "重生宝石镶嵌", "重生宝石镶嵌", client.ClientData.RoleName, "系统", "添加宝石", 0, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, Equip);
						result = RebornStornOpcode.RebornInlaySucc;
					}
				}
			}
			return result;
		}

		
		public static RebornStornOpcode ProessRebornStoneDisInlayHold(GameClient client, int EquipDBID, int Number, out string prop)
		{
			prop = "";
			GoodsData Equip = RebornEquip.GetRebornGoodsByDbID(client, EquipDBID);
			RebornStornOpcode result;
			if (Equip == null)
			{
				result = RebornStornOpcode.RebornInlayNotExistEquip;
			}
			else if (!RebornEquip.IsRebornEquip(Equip.GoodsID))
			{
				result = RebornStornOpcode.RebornInlayNotEquip;
			}
			else if (Equip.Props == null || Equip.Props == "")
			{
				result = RebornStornOpcode.RebornInlayNotMakeHole;
			}
			else
			{
				Dictionary<int, Dictionary<int, int>> HoleInfo = RebornStone.ParessMakeHoleInfo(Equip.Props);
				if (HoleInfo == null)
				{
					result = RebornStornOpcode.RebornInlayGetInfoErr;
				}
				else
				{
					lock (HoleInfo)
					{
						if (Number == -1)
						{
							if (!RebornEquip.IsRebornEquipShengWu(Equip.GoodsID))
							{
								return RebornStornOpcode.RebornInlayNotEquip;
							}
							Dictionary<int, int> Data;
							if (!HoleInfo.TryGetValue(Number, out Data))
							{
								return RebornStornOpcode.RebornDisInlayCurrSiteNotHasXStone;
							}
							int StornID;
							if (!Data.TryGetValue(Number, out StornID))
							{
								return RebornStornOpcode.RebornDisInlayCurrSiteNotHasXStone;
							}
							SystemXmlItem systemGoodsItem;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(StornID, out systemGoodsItem))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, StornID), null, true);
								GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StornID, 0);
								return RebornStornOpcode.RebornDisInlayCurrUserNotHasXStone;
							}
							int Bind = 0;
							if (Equip.Binding == 1)
							{
								Bind = 1;
							}
							Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, StornID, 1, 0, "", 0, Bind, 15000, "", true, 1, string.Format("炫彩宝石卸下", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
							HoleInfo.Remove(Number);
							Equip.Props = RebornStone.MakeHoleInfo(HoleInfo, 0);
							if (!RebornStone.UpdateGoodsPropsToDB(client, Equip))
							{
								return RebornStornOpcode.RebornDisInlayUpdateInfoErr;
							}
						}
						else
						{
							Dictionary<int, int> Info;
							if (!HoleInfo.TryGetValue(Number, out Info))
							{
								return RebornStornOpcode.RebornDisInlayCurrSiteNotHasStone;
							}
							Dictionary<int, int> NewInfo = new Dictionary<int, int>();
							using (Dictionary<int, int>.Enumerator enumerator = HoleInfo[Number].GetEnumerator())
							{
								if (enumerator.MoveNext())
								{
									KeyValuePair<int, int> it = enumerator.Current;
									if (it.Value == 0)
									{
										return RebornStornOpcode.RebornDisInlayCurrSiteNotHasStone;
									}
									SystemXmlItem systemGoodsItem;
									if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(it.Value, out systemGoodsItem))
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, it.Value), null, true);
										GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, it.Value, 0);
										return RebornStornOpcode.RebornDisInlayCurrUserNotHasStone;
									}
									int Bind = 0;
									if (Equip.Binding == 1)
									{
										Bind = 1;
									}
									Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, it.Value, 1, 0, "", 0, Bind, 15000, "", true, 1, string.Format("重生宝石卸下", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
									NewInfo.Add(it.Key, 0);
								}
							}
							HoleInfo[Number] = NewInfo;
							int StoneID = 0;
							Dictionary<int, int> value;
							if (HoleInfo.TryGetValue(-1, out value))
							{
								int XuanCaiID;
								if (value.TryGetValue(-1, out XuanCaiID))
								{
									StoneID = XuanCaiID;
								}
							}
							Equip.Props = RebornStone.MakeHoleInfo(HoleInfo, StoneID);
						}
						if (!RebornStone.UpdateGoodsPropsToDB(client, Equip))
						{
							return RebornStornOpcode.RebornDisInlayUpdateInfoErr;
						}
					}
					prop = Equip.Props;
					if (Equip.Using == 1)
					{
						Global.RefreshEquipProp(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
					result = RebornStornOpcode.RebornInlaySucc;
				}
			}
			return result;
		}

		
		public static RebornStornOpcode ProessRebornStoneComplex(GameClient client, int GoodID, int Count)
		{
			SystemXmlItem systemGoodsItem;
			RebornStornOpcode result;
			RebornStornStruct Item;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(GoodID, out systemGoodsItem))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, GoodID), null, true);
				GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GoodID, 0);
				result = RebornStornOpcode.RebornComplexStoneNotGood;
			}
			else if (Count <= 0 || Count > 999)
			{
				result = RebornStornOpcode.RebornComplexCountErr;
			}
			else if (!RebornStone.RebornStoneXml.TryGetValue(GoodID, out Item))
			{
				result = RebornStornOpcode.RebornComplexStoneNotFind;
			}
			else
			{
				lock (Item)
				{
					int RebornFengYin = Global.GetRoleParamsInt32FromDB(client, "10252");
					int RebornChongSheng = Global.GetRoleParamsInt32FromDB(client, "10253");
					int RebornXuanCai = Global.GetRoleParamsInt32FromDB(client, "10254");
					int totleFengYin = Item.FengYinJingShi * Count;
					int totleChongSheng = Item.ChongShengJingShi * Count;
					int totleXuanCai = Item.XuanCaiJingShi * Count;
					if (RebornFengYin < totleFengYin)
					{
						return RebornStornOpcode.RebornComplexFengYinNotEnough;
					}
					if (RebornChongSheng < totleChongSheng)
					{
						return RebornStornOpcode.RebornComplexChongShengNotEnough;
					}
					if (RebornXuanCai < totleXuanCai)
					{
						return RebornStornOpcode.RebornComplexXuanCaiNotEnough;
					}
					if (!GameManager.ClientMgr.ModifyRebornFengYinJinShiValue(client, -totleFengYin, "重生宝石合成消耗封印晶石", true, true, false))
					{
						return RebornStornOpcode.RebornComplexNeedFengYinErr;
					}
					if (!GameManager.ClientMgr.ModifyRebornChongShengJinShiValue(client, -totleChongSheng, "重生宝石合成消耗重生晶石", true, true, false))
					{
						return RebornStornOpcode.RebornComplexNeedChongShengErr;
					}
					if (!GameManager.ClientMgr.ModifyRebornXuanCaiJinShiValue(client, -totleXuanCai, "重生宝石合成消耗炫彩晶石", true, true, false))
					{
						return RebornStornOpcode.RebornComplexNeedXuanCaiErr;
					}
				}
				int NewDBID = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, GoodID, Count, 0, "", 0, 0, 15000, "", true, 1, string.Format("重生宝石合成", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
				result = RebornStornOpcode.RebornComplexNewStoneSucc;
			}
			return result;
		}

		
		public static RebornStornOpcode RebornStoneResolve(GameClient client, int GoodsID, int Count, int bind)
		{
			RebornStornOpcode result;
			if (bind < 0 || bind > 1)
			{
				result = RebornStornOpcode.RebornNotFindBind;
			}
			else if (Count <= 0 || Count > 9999)
			{
				result = RebornStornOpcode.RebornResolveCountErr;
			}
			else
			{
				bool EquipBindUse;
				Dictionary<int, Dictionary<int, GoodsData>> GoodsDict = RebornStone.GetDictRebornUseGoodHasBinding(client, GoodsID, Count, bind, out EquipBindUse);
				if (GoodsDict == null)
				{
					result = RebornStornOpcode.RebornResolveNotFind;
				}
				else
				{
					int TotleChangeFengYingJingShi = 0;
					int TotleChangeChongShengJingShi = 0;
					int TotleChangeXuanCaiJingShi = 0;
					lock (GoodsDict)
					{
						foreach (Dictionary<int, GoodsData> iter in GoodsDict.Values)
						{
							foreach (KeyValuePair<int, GoodsData> it in iter)
							{
								GoodsData goodsData = it.Value;
								if (goodsData == null)
								{
									return RebornStornOpcode.RebornResolveNotFind;
								}
								if (goodsData.Using == 1)
								{
									return RebornStornOpcode.RebornResolveIsUsing;
								}
								SystemXmlItem systemGoodsItem;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoodsItem))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, goodsData.GoodsID), null, true);
									GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, 0);
									return RebornStornOpcode.RebornResolveStoneNotGood;
								}
								if (it.Key < 0)
								{
									return RebornStornOpcode.RebornResolveGoodNotEnoughErr;
								}
								bool isBindUse = false;
								if (!RebornStone.RebornHoleRemoveUseGoods(client, goodsData, it.Key, out isBindUse))
								{
									return RebornStornOpcode.RebornResolveDeleteErr;
								}
								int ChangeFengYingJingShi = systemGoodsItem.GetIntValue("ChangeFengYingJingShi", -1);
								int ChangeChongShengJingShi = systemGoodsItem.GetIntValue("ChangeChongShengJingShi", -1);
								int ChangeXuanCaiJingShi = systemGoodsItem.GetIntValue("ChangeXuanCaiJingShi", -1);
								if (ChangeFengYingJingShi < 0 || ChangeChongShengJingShi < 0 || ChangeXuanCaiJingShi < 0)
								{
									return RebornStornOpcode.RebornResolveGoodXmlErr;
								}
								TotleChangeFengYingJingShi += ChangeFengYingJingShi * it.Key;
								TotleChangeChongShengJingShi += ChangeChongShengJingShi * it.Key;
								TotleChangeXuanCaiJingShi += ChangeXuanCaiJingShi * it.Key;
							}
						}
					}
					if (!GameManager.ClientMgr.ModifyRebornFengYinJinShiValue(client, TotleChangeFengYingJingShi, "重生宝石分解增加封印晶石", true, true, false))
					{
						result = RebornStornOpcode.RebornResolveAddFengYinErr;
					}
					else if (!GameManager.ClientMgr.ModifyRebornChongShengJinShiValue(client, TotleChangeChongShengJingShi, "重生宝石分解增加重生晶石", true, true, false))
					{
						result = RebornStornOpcode.RebornResolveAddChongShengErr;
					}
					else if (!GameManager.ClientMgr.ModifyRebornXuanCaiJinShiValue(client, TotleChangeXuanCaiJingShi, "重生宝石分解增加炫彩晶石", true, true, false))
					{
						result = RebornStornOpcode.RebornResolveAddXuanCaiErr;
					}
					else
					{
						result = RebornStornOpcode.RebornResolveStoneSucc;
					}
				}
			}
			return result;
		}

		
		public static RebornStornOpcode RebornXuanCaiComplexStone(GameClient client, int DBID1, int num1, int DBID2, int num2, int DBID3, int num3)
		{
			GoodsData gd = RebornEquip.GetRebornGoodsByDbID(client, DBID1);
			RebornStornOpcode result;
			if (gd == null)
			{
				result = RebornStornOpcode.RebornXuanCaiNotFind;
			}
			else
			{
				int TotleNum = 0;
				Dictionary<int, int> dict = new Dictionary<int, int>();
				lock (dict)
				{
					if (num1 > 0)
					{
						if (dict.ContainsKey(DBID1))
						{
							return RebornStornOpcode.RebornXuanGoodInfoErr;
						}
						dict.Add(DBID1, num1);
						TotleNum += num1;
					}
					if (num2 > 0)
					{
						if (dict.ContainsKey(DBID2))
						{
							return RebornStornOpcode.RebornXuanGoodInfoErr;
						}
						dict.Add(DBID2, num2);
						TotleNum += num2;
					}
					if (num3 > 0)
					{
						if (dict.ContainsKey(DBID3))
						{
							return RebornStornOpcode.RebornXuanGoodInfoErr;
						}
						dict.Add(DBID3, num3);
						TotleNum += num3;
					}
				}
				if (TotleNum != 3)
				{
					result = RebornStornOpcode.RebornXuanCaiNotFind;
				}
				else
				{
					Dictionary<int, Dictionary<int, int>> IDNumMap = null;
					List<GoodsData> goodsDataList = RebornEquip.GetRebornGoodsByDbIDDict(client, dict, out IDNumMap);
					if (goodsDataList == null)
					{
						result = RebornStornOpcode.RebornXuanCaiNotFind;
					}
					else
					{
						List<int> SuitList = new List<int>();
						lock (goodsDataList)
						{
							foreach (GoodsData it in goodsDataList)
							{
								SystemXmlItem systemGoodsItem;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(it.GoodsID, out systemGoodsItem))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("用户使用物品时，从物品配置表中查找物品ID失败, RoleID={0}, GoodsID={1}", client.ClientData.RoleID, it.GoodsID), null, true);
									GameManager.ClientMgr.NotifyUseGoodsResult(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, it.GoodsID, 0);
									return RebornStornOpcode.RebornXuanCaiGoodXmlErr;
								}
								SuitList.Add(systemGoodsItem.GetIntValue("SuitID", -1));
							}
						}
						if (SuitList.Count == 0)
						{
							result = RebornStornOpcode.RebornXuanCaiSuitErr;
						}
						else
						{
							lock (SuitList)
							{
								if (SuitList[0] >= RebornStone.XuanCaiMaxLevel)
								{
									return RebornStornOpcode.RebornXuanCaiMaxLevel;
								}
								for (int i = 0; i < SuitList.Count; i++)
								{
									if (i == SuitList.Count - 1)
									{
										break;
									}
									if (SuitList[i] != SuitList[i + 1])
									{
										return RebornStornOpcode.RebornXuanCaiNotSameLevel;
									}
								}
							}
							RebornStornComp Complex;
							if (!RebornStone.RebornStoneComplex.TryGetValue(gd.GoodsID, out Complex))
							{
								result = RebornStornOpcode.RebornXuanCaiNotFindComplex;
							}
							else
							{
								int NewGoodID = Complex.StornAID;
								if (IDNumMap == null)
								{
									result = RebornStornOpcode.RebornXuanGoodInfoErr;
								}
								else
								{
									int Bind = 0;
									lock (IDNumMap)
									{
										List<int> NumList = new List<int>();
										List<bool> FakeStateMachine = new List<bool>();
										foreach (int it2 in Complex.CompNeed)
										{
											foreach (Dictionary<int, int> git in IDNumMap.Values)
											{
												using (Dictionary<int, int>.Enumerator enumerator4 = git.GetEnumerator())
												{
													if (enumerator4.MoveNext())
													{
														KeyValuePair<int, int> iter = enumerator4.Current;
														if (iter.Key == it2)
														{
															FakeStateMachine.Add(true);
														}
														NumList.Add(iter.Value);
													}
												}
											}
										}
										if (FakeStateMachine.Count != IDNumMap.Count)
										{
											return RebornStornOpcode.RebornXuanCaiNotUseGoodComplex;
										}
										int Count = 0;
										foreach (GoodsData iter2 in goodsDataList)
										{
											bool EquipBindUse = false;
											if (!RebornStone.RebornHoleRemoveUseGoods(client, iter2, NumList[Count], out EquipBindUse))
											{
												return RebornStornOpcode.RebornXuanCaiUseGoodErr;
											}
											Count++;
											if (EquipBindUse)
											{
												Bind = 1;
											}
										}
									}
									Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, NewGoodID, 1, 0, "", 0, Bind, 15000, "", true, 1, string.Format("炫彩宝石合成", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
									result = RebornStornOpcode.RebornXuanCaiComplexSucc;
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public static RebornStornOpcode SaleRebornStoneProcess(GameClient client, string strGoodsID)
		{
			int nTotalChangeFengYing = 0;
			int nTotalChangeChongSheng = 0;
			int nTotalChangeXuanCai = 0;
			string[] idsList = strGoodsID.Split(new char[]
			{
				','
			});
			int i = 0;
			while (i < idsList.Length)
			{
				int goodsDbID = Global.SafeConvertToInt32(idsList[i]);
				GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, goodsDbID);
				if (goodsData != null && goodsData.Site == 15000 && goodsData.Using <= 0)
				{
					int category = Global.GetGoodsCatetoriy(goodsData.GoodsID);
					SystemXmlItem xmlItem = null;
					if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xmlItem) && null != xmlItem)
					{
						string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							4,
							goodsData.Id,
							goodsData.GoodsID,
							0,
							goodsData.Site,
							goodsData.GCount,
							goodsData.BagIndex,
							""
						});
						if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "重生宝石回收", null))
						{
							if (client.ClientData.RebornCount > 0)
							{
								int ChangeFengYingJingShi = xmlItem.GetIntValue("ChangeFengYingJingShi", -1);
								int ChangeChongShengJingShi = xmlItem.GetIntValue("ChangeChongShengJingShi", -1);
								int ChangeXuanCaiJingShi = xmlItem.GetIntValue("ChangeXuanCaiJingShi", -1);
								if (ChangeFengYingJingShi > 0)
								{
									nTotalChangeFengYing += ChangeFengYingJingShi * goodsData.GCount;
								}
								if (ChangeChongShengJingShi > 0)
								{
									nTotalChangeChongSheng += ChangeChongShengJingShi * goodsData.GCount;
								}
								if (ChangeXuanCaiJingShi > 0)
								{
									nTotalChangeXuanCai += ChangeXuanCaiJingShi * goodsData.GCount;
								}
							}
						}
					}
				}
				IL_1FE:
				i++;
				continue;
				goto IL_1FE;
			}
			if (nTotalChangeFengYing > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornFengYinJinShiValue(client, nTotalChangeFengYing, "重生宝石一键分解增加封印晶石", true, true, false))
				{
					return RebornStornOpcode.RebornBatchResolveAddFengYinErr;
				}
			}
			if (nTotalChangeChongSheng > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornChongShengJinShiValue(client, nTotalChangeChongSheng, "重生宝石一键分解增加重生晶石", true, true, false))
				{
					return RebornStornOpcode.RebornBatchResolveAddChongShengErr;
				}
			}
			if (nTotalChangeXuanCai > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornXuanCaiJinShiValue(client, nTotalChangeXuanCai, "重生宝石一键分解增加炫彩晶石", true, true, false))
				{
					return RebornStornOpcode.RebornBatchResolveAddXuanCaiErr;
				}
			}
			return RebornStornOpcode.RebornBatchResolveStoneSucc;
		}

		
		public static Dictionary<int, RebornHoleStruct> RebornHoleStr = new Dictionary<int, RebornHoleStruct>();

		
		public static Dictionary<int, int> RebornEquipHoleMap = new Dictionary<int, int>();

		
		public static Dictionary<Dictionary<int, int>, int> ItemIDMap = new Dictionary<Dictionary<int, int>, int>();

		
		public static Dictionary<int, double> RebornHoleExpend = new Dictionary<int, double>();

		
		public static Dictionary<int, RebornStornStruct> RebornStoneXml = new Dictionary<int, RebornStornStruct>();

		
		public static Dictionary<int, RebornStornComp> RebornStoneComplex = new Dictionary<int, RebornStornComp>();

		
		public static Dictionary<int, Dictionary<int, RebornXuanCaiStorn>> RebornStoneActiveAttr = new Dictionary<int, Dictionary<int, RebornXuanCaiStorn>>();

		
		public static int XuanCaiMaxLevel = 10;

		
		private static RebornStone instance = new RebornStone();
	}
}
