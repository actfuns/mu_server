using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020000B7 RID: 183
	internal class MountHolyStampManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2
	{
		// Token: 0x060002CD RID: 717 RVA: 0x0002F7A8 File Offset: 0x0002D9A8
		public static MountHolyStampManager getInstance()
		{
			return MountHolyStampManager.instance;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0002F7C0 File Offset: 0x0002D9C0
		public bool InitConfig()
		{
			string fileName = Global.GameResPath(MountHolyStampConst.ShengYinShengJi);
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
			}
			try
			{
				Dictionary<int, List<HolyStampUpLeve>> holyStampList = new Dictionary<int, List<HolyStampUpLeve>>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				int AttrCount = 0;
				foreach (XElement xmlItem in xmlItems)
				{
					HolyStampUpLeve temp = new HolyStampUpLeve();
					Dictionary<int, double> AttrList = new Dictionary<int, double>();
					temp.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					temp.GoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "DaoJuID"));
					temp.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "LeiXing"));
					temp.Site = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "BuWei"));
					temp.Quality = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "PinZhi"));
					temp.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "DengJi"));
					temp.UpExp = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ShengJiJingYan"));
					temp.PhagocytosisExp = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "TunShiJingYan"));
					temp.AttrNum = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ZhuoYueShuXingTiaoShu"));
					if (temp.AttrNum < 0)
					{
						temp.AttrNum = 0;
					}
					string[] arrAttr = Global.GetSafeAttributeStr(xmlItem, "JiChuShuXing").Split(new char[]
					{
						'|'
					});
					if (arrAttr != null && arrAttr.Length == 1)
					{
						AttrList.Add(0, 0.0);
					}
					else
					{
						for (int i = 0; i < arrAttr.Length; i++)
						{
							AttrList.Add((int)ConfigParser.GetPropIndexByPropName(arrAttr[i].Split(new char[]
							{
								','
							})[0]), Convert.ToDouble(arrAttr[i].Split(new char[]
							{
								','
							})[1]));
						}
					}
					temp.AttrList = AttrList;
					if (holyStampList.ContainsKey(temp.GoodID))
					{
						holyStampList[temp.GoodID].Add(temp);
					}
					else
					{
						List<HolyStampUpLeve> list = new List<HolyStampUpLeve>();
						list.Add(temp);
						holyStampList.Add(temp.GoodID, list);
					}
					if (this.GoodsLvDict.ContainsKey(temp.GoodID))
					{
						if (this.GoodsLvDict[temp.GoodID] < temp.Level)
						{
							this.GoodsLvDict[temp.GoodID] = temp.Level;
						}
					}
					else
					{
						this.GoodsLvDict.Add(temp.GoodID, temp.Level);
					}
					AttrCount += temp.AttrNum;
					if (this.GoodsLvAttrCount.ContainsKey(temp.GoodID))
					{
						if (this.GoodsLvAttrCount[temp.GoodID].ContainsKey(temp.Level))
						{
							this.GoodsLvAttrCount[temp.GoodID][temp.Level] = AttrCount;
						}
						else
						{
							this.GoodsLvAttrCount[temp.GoodID].Add(temp.Level, AttrCount);
						}
					}
					else
					{
						AttrCount = temp.AttrNum;
						Dictionary<int, int> lvTemp = new Dictionary<int, int>();
						lvTemp.Add(temp.Level, AttrCount);
						this.GoodsLvAttrCount.Add(temp.GoodID, lvTemp);
					}
				}
				this.holyStampUpLeveL = holyStampList;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			bool result;
			if (this.holyStampUpLeveL == null || this.GoodsLvDict == null || this.GoodsLvAttrCount == null)
			{
				result = false;
			}
			else
			{
				fileName = Global.GameResPath(MountHolyStampConst.ShengYinZhuoYue);
				xml = XElement.Load(fileName);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
				}
				try
				{
					Dictionary<int, Dictionary<int, List<HolyStampAttr>>> holystampAttr = new Dictionary<int, Dictionary<int, List<HolyStampAttr>>>();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						HolyStampAttr temp2 = new HolyStampAttr();
						Dictionary<int, double> AttrList = new Dictionary<int, double>();
						temp2.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
						temp2.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ShengYinLeiXing"));
						temp2.Quality = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "PinZhi"));
						string[] arrAttr = Global.GetSafeAttributeStr(xmlItem, "ShuXingLeiXing").Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < arrAttr.Length; i++)
						{
							AttrList.Add((int)ConfigParser.GetPropIndexByPropName(arrAttr[i].Split(new char[]
							{
								','
							})[0]), Convert.ToDouble(arrAttr[i].Split(new char[]
							{
								','
							})[1]));
						}
						temp2.AttrList = AttrList;
						if (holystampAttr.ContainsKey(temp2.Type))
						{
							if (holystampAttr[temp2.Type].ContainsKey(temp2.Quality))
							{
								holystampAttr[temp2.Type][temp2.Quality].Add(temp2);
							}
							else
							{
								List<HolyStampAttr> list2 = new List<HolyStampAttr>();
								list2.Add(temp2);
								holystampAttr[temp2.Type].Add(temp2.Quality, list2);
							}
						}
						else
						{
							Dictionary<int, List<HolyStampAttr>> QualityStamp = new Dictionary<int, List<HolyStampAttr>>();
							List<HolyStampAttr> list2 = new List<HolyStampAttr>();
							list2.Add(temp2);
							QualityStamp.Add(temp2.Quality, list2);
							holystampAttr.Add(temp2.Type, QualityStamp);
						}
					}
					this.holyStampAttr = holystampAttr;
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				if (this.holyStampAttr == null)
				{
					result = false;
				}
				else
				{
					fileName = Global.GameResPath(MountHolyStampConst.ShengYinTaoZhuang);
					xml = XElement.Load(fileName);
					if (null == xml)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
					}
					try
					{
						Dictionary<int, HolyStampSuit> holyStampSuitHot = new Dictionary<int, HolyStampSuit>();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							HolyStampSuit temp3 = new HolyStampSuit();
							Dictionary<int, double> AttrList2 = new Dictionary<int, double>();
							Dictionary<int, double> AttrList3 = new Dictionary<int, double>();
							Dictionary<int, double> AttrList4 = new Dictionary<int, double>();
							temp3.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
							temp3.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "LeiXing"));
							string[] arrAttr2 = Global.GetSafeAttributeStr(xmlItem, "TaoZhuangShuXingTwo").Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < arrAttr2.Length; i++)
							{
								AttrList2.Add((int)ConfigParser.GetPropIndexByPropName(arrAttr2[i].Split(new char[]
								{
									','
								})[0]), Convert.ToDouble(arrAttr2[i].Split(new char[]
								{
									','
								})[1]));
							}
							temp3.AttrListTwo = AttrList2;
							string[] arrAttr3 = Global.GetSafeAttributeStr(xmlItem, "TaoZhuangShuXingFour").Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < arrAttr3.Length; i++)
							{
								AttrList3.Add((int)ConfigParser.GetPropIndexByPropName(arrAttr3[i].Split(new char[]
								{
									','
								})[0]), Convert.ToDouble(arrAttr3[i].Split(new char[]
								{
									','
								})[1]));
							}
							temp3.AttrListFour = AttrList3;
							string[] arrAttr4 = Global.GetSafeAttributeStr(xmlItem, "TaoZhuangShuXingSix").Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < arrAttr4.Length; i++)
							{
								AttrList4.Add((int)ConfigParser.GetPropIndexByPropName(arrAttr4[i].Split(new char[]
								{
									','
								})[0]), Convert.ToDouble(arrAttr4[i].Split(new char[]
								{
									','
								})[1]));
							}
							temp3.AttrListSix = AttrList4;
							holyStampSuitHot.Add(temp3.Type, temp3);
						}
						this.holyStampSuit = holyStampSuitHot;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
					if (this.holyStampSuit == null)
					{
						result = false;
					}
					else
					{
						List<string> SuitExpand = GameManager.systemParamsList.GetParamValueStringListByName("ShengYinJieSuo", '|');
						try
						{
							Dictionary<int, int> holyStampDesbloquearHot = new Dictionary<int, int>();
							foreach (string it in SuitExpand)
							{
								string[] str = it.Split(new char[]
								{
									','
								});
								holyStampDesbloquearHot.Add(Convert.ToInt32(str[0]), Convert.ToInt32(str[1]));
							}
							this.holyStampDesbloquear = holyStampDesbloquearHot;
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
						result = (this.holyStampDesbloquear != null);
					}
				}
			}
			return result;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x000302DC File Offset: 0x0002E4DC
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00030300 File Offset: 0x0002E500
		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00030314 File Offset: 0x0002E514
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2090, 2, 2, MountHolyStampManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2091, 1, 1, MountHolyStampManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2092, 1, 1, MountHolyStampManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2093, 1, 1, MountHolyStampManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00030388 File Offset: 0x0002E588
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0003039C File Offset: 0x0002E59C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000303B0 File Offset: 0x0002E5B0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000303C4 File Offset: 0x0002E5C4
		public int GetGolaNum(GameClient client)
		{
			int result;
			if (client == null)
			{
				result = 0;
			}
			else if (client.ClientData.ZuoQiMainData == null)
			{
				result = 0;
			}
			else
			{
				int Gola = 0;
				lock (this.holyStampDesbloquear)
				{
					foreach (KeyValuePair<int, int> it in this.holyStampDesbloquear)
					{
						if (client.ClientData.ZuoQiMainData.MountLevel + 1 < it.Value)
						{
							break;
						}
						Gola = it.Key;
					}
				}
				result = Gola;
			}
			return result;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x000304BC File Offset: 0x0002E6BC
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 2090:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int DBid = Convert.ToInt32(cmdParams[0]);
					string PhagocytosisDBids = cmdParams[1];
					this.ProcessHolyStampUpGrade(client, DBid, PhagocytosisDBids);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_HOLYSTAMP_UPGRADE", false, false);
				}
				break;
			case 2091:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int DBid = Convert.ToInt32(cmdParams[0]);
					int result = Convert.ToInt32(this.ProcessHolyStampUsing(client, DBid));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_HOLYSTAMP_INLAY", false, false);
				}
				break;
			case 2092:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int DBid = Convert.ToInt32(cmdParams[0]);
					int result = Convert.ToInt32(this.ProcessHolyStampDisUsing(client, DBid));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_HOLYSTAMP_DEMOUNT", false, false);
				}
				break;
			case 2093:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int roleid = Convert.ToInt32(cmdParams[0]);
					this.ResetHolyStampBag(client, true);
					client.sendCmd<List<GoodsData>>(nID, client.ClientData.HolyGoodsDataList, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_HOLYSTAMP_BAGRESET", false, false);
				}
				break;
			}
			return true;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x000306F8 File Offset: 0x0002E8F8
		public static int GetIdleSlotOfGoods(GameClient client)
		{
			int idelPos = 0;
			int result;
			if (null == client.ClientData.HolyGoodsDataList)
			{
				result = idelPos;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
				{
					if (client.ClientData.HolyGoodsDataList[i].Site == 16000 && client.ClientData.HolyGoodsDataList[i].Using <= 0)
					{
						if (usedBagIndex.IndexOf(client.ClientData.HolyGoodsDataList[i].BagIndex) < 0)
						{
							usedBagIndex.Add(client.ClientData.HolyGoodsDataList[i].BagIndex);
						}
					}
				}
				for (int j = 0; j < MountHolyStampManager.HolyBagNum; j++)
				{
					if (usedBagIndex.IndexOf(j) < 0)
					{
						idelPos = j;
						break;
					}
				}
				result = idelPos;
			}
			return result;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00030840 File Offset: 0x0002EA40
		public GoodsData GetGoodsByDbID(GameClient client, int dbID)
		{
			GoodsData result;
			if (null == client.ClientData.HolyGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.HolyGoodsDataList)
				{
					result = client.ClientData.HolyGoodsDataList.Find((GoodsData _g) => _g.Id == dbID);
				}
			}
			return result;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x000308DC File Offset: 0x0002EADC
		public static bool CheckIsMountBagByGoodsID(int goodsID)
		{
			SystemXmlItem systemGoods = null;
			return GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods) && (systemGoods.GetIntValue("Categoriy", -1) == 980 || systemGoods.GetIntValue("Categoriy", -1) == 981);
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00030938 File Offset: 0x0002EB38
		public static GoodsData AddGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
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
			if (null == client.ClientData.HolyGoodsDataList)
			{
				client.ClientData.HolyGoodsDataList = new List<GoodsData>();
			}
			lock (client.ClientData.HolyGoodsDataList)
			{
				client.ClientData.HolyGoodsDataList.Add(gd);
			}
			return gd;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00030A74 File Offset: 0x0002EC74
		public HolyStampUpLeve GetHolyUpGradeInfo(GoodsData goodData)
		{
			HolyStampUpLeve result;
			if (goodData == null || this.holyStampUpLeveL == null || !this.holyStampUpLeveL.ContainsKey(goodData.GoodsID) || goodData.ElementhrtsProps == null || goodData.ElementhrtsProps.Count != 2)
			{
				result = null;
			}
			else
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodData.GoodsID, out systemGoods))
				{
					result = null;
				}
				else
				{
					lock (this.holyStampUpLeveL[goodData.GoodsID])
					{
						foreach (HolyStampUpLeve it in this.holyStampUpLeveL[goodData.GoodsID])
						{
							if (980 == systemGoods.GetIntValue("Categoriy", -1) && it.Level == goodData.ElementhrtsProps[0])
							{
								return it;
							}
							if (981 == systemGoods.GetIntValue("Categoriy", -1) && it.Level == -1)
							{
								return it;
							}
						}
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00030BF4 File Offset: 0x0002EDF4
		public HolyStampUpLeve GetLevelUpCount(GoodsData goodData, HolyStampUpLeve CurrInfo, int TunExp, out int TotleExp)
		{
			TotleExp = 0;
			HolyStampUpLeve result;
			if (TunExp < 0 || TunExp > 2147483647)
			{
				result = null;
			}
			else if (CurrInfo == null || goodData == null || goodData.ElementhrtsProps == null || goodData.ElementhrtsProps.Count != 2)
			{
				result = null;
			}
			else if (!this.holyStampUpLeveL.ContainsKey(goodData.GoodsID))
			{
				result = null;
			}
			else if (CurrInfo.GoodID != goodData.GoodsID)
			{
				result = null;
			}
			else
			{
				TotleExp = goodData.ElementhrtsProps[1] + TunExp;
				if (CurrInfo.UpExp > TotleExp)
				{
					result = CurrInfo;
				}
				else
				{
					lock (this.holyStampUpLeveL)
					{
						foreach (HolyStampUpLeve it in this.holyStampUpLeveL[goodData.GoodsID])
						{
							if (it.UpExp > TotleExp)
							{
								return it;
							}
							if (this.GoodsLvDict[it.GoodID] <= it.Level)
							{
								TotleExp = it.UpExp;
								return it;
							}
						}
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00030D84 File Offset: 0x0002EF84
		public HolyStampAttr GetAttrByQuality(HolyStampUpLeve StampInfo, GoodsData goodsData, List<int> newList = null)
		{
			if (this.holyStampAttr.ContainsKey(StampInfo.Type))
			{
				if (this.holyStampAttr[StampInfo.Type].ContainsKey(StampInfo.Quality))
				{
					if (newList == null)
					{
						if (goodsData.WashProps == null || goodsData.WashProps.Count == 0)
						{
							int MaxV = this.holyStampAttr[StampInfo.Type][StampInfo.Quality].Count;
							if (MaxV < 1)
							{
								return null;
							}
							int index = Global.GetRandomNumber(1, MaxV);
							return this.holyStampAttr[StampInfo.Type][StampInfo.Quality][index];
						}
						else
						{
							List<HolyStampAttr> newAttrList = new List<HolyStampAttr>();
							bool flag = false;
							foreach (HolyStampAttr it in this.holyStampAttr[StampInfo.Type][StampInfo.Quality])
							{
								for (int i = 0; i < goodsData.WashProps.Count; i += 3)
								{
									if (it.ID == goodsData.WashProps[i])
									{
										flag = true;
										break;
									}
								}
								if (!flag && newAttrList.IndexOf(it) == -1)
								{
									newAttrList.Add(it);
								}
								flag = false;
							}
							int MaxV = newAttrList.Count;
							if (MaxV < 1)
							{
								return null;
							}
							int index = Global.GetRandomNumber(0, MaxV);
							return newAttrList[index];
						}
					}
					else
					{
						List<HolyStampAttr> newAttrList = new List<HolyStampAttr>();
						bool flag = false;
						foreach (HolyStampAttr it in this.holyStampAttr[StampInfo.Type][StampInfo.Quality])
						{
							for (int i = 0; i < newList.Count; i += 3)
							{
								if (it.ID == newList[i])
								{
									flag = true;
									break;
								}
							}
							if (!flag && newAttrList.IndexOf(it) == -1)
							{
								newAttrList.Add(it);
							}
							flag = false;
						}
						int MaxV = newAttrList.Count;
						if (MaxV < 1)
						{
							return null;
						}
						int index = Global.GetRandomNumber(0, MaxV);
						return newAttrList[index];
					}
				}
			}
			return null;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00031084 File Offset: 0x0002F284
		public void UpdateProps(GameClient client)
		{
			double[] _ExtProps = new double[177];
			try
			{
				if (client != null && client.ClientData.HolyGoodsDataList != null)
				{
					double Percent = 0.001;
					int Count = 0;
					Dictionary<int, int> Dict = new Dictionary<int, int>();
					foreach (GoodsData it in client.ClientData.HolyGoodsDataList)
					{
						if (it.Using == 1)
						{
							HolyStampUpLeve info = this.GetHolyUpGradeInfo(it);
							if (info != null)
							{
								if (Dict.ContainsKey(info.Type))
								{
									Dictionary<int, int> dictionary;
									int key;
									(dictionary = Dict)[key = info.Type] = dictionary[key] + 1;
								}
								else
								{
									Dict.Add(info.Type, 1);
								}
								foreach (KeyValuePair<int, double> iter in info.AttrList)
								{
									_ExtProps[iter.Key] += iter.Value;
								}
								if (it.WashProps != null && it.WashProps.Count >= 3)
								{
									for (int i = 1; i < it.WashProps.Count; i += 3)
									{
										if (i + 1 < it.WashProps.Count)
										{
											_ExtProps[it.WashProps[i]] += (double)it.WashProps[i + 1] * Percent;
										}
									}
								}
								Count++;
							}
						}
					}
					if (Count <= 6)
					{
						Dictionary<int, double> SuitAttrList = new Dictionary<int, double>();
						lock (SuitAttrList)
						{
							if (Dict != null)
							{
								foreach (KeyValuePair<int, int> it2 in Dict)
								{
									if (this.holyStampSuit.ContainsKey(it2.Key))
									{
										if (it2.Value >= 2)
										{
											foreach (KeyValuePair<int, double> iter in this.holyStampSuit[it2.Key].AttrListTwo)
											{
												if (SuitAttrList.ContainsKey(iter.Key))
												{
													int key;
													Dictionary<int, double> dictionary2;
													(dictionary2 = SuitAttrList)[key = iter.Key] = dictionary2[key] + iter.Value;
												}
												else
												{
													SuitAttrList.Add(iter.Key, iter.Value);
												}
											}
											if (it2.Value >= 4)
											{
												foreach (KeyValuePair<int, double> iter in this.holyStampSuit[it2.Key].AttrListFour)
												{
													if (SuitAttrList.ContainsKey(iter.Key))
													{
														int key;
														Dictionary<int, double> dictionary2;
														(dictionary2 = SuitAttrList)[key = iter.Key] = dictionary2[key] + iter.Value;
													}
													else
													{
														SuitAttrList.Add(iter.Key, iter.Value);
													}
												}
												if (it2.Value >= 6)
												{
													foreach (KeyValuePair<int, double> iter in this.holyStampSuit[it2.Key].AttrListSix)
													{
														if (SuitAttrList.ContainsKey(iter.Key))
														{
															int key;
															Dictionary<int, double> dictionary2;
															(dictionary2 = SuitAttrList)[key = iter.Key] = dictionary2[key] + iter.Value;
														}
														else
														{
															SuitAttrList.Add(iter.Key, iter.Value);
														}
													}
												}
											}
										}
									}
								}
							}
							foreach (KeyValuePair<int, double> it3 in SuitAttrList)
							{
								_ExtProps[it3.Key] += it3.Value;
							}
						}
					}
				}
			}
			finally
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.HolyStamp,
					_ExtProps
				});
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000316D8 File Offset: 0x0002F8D8
		public GoodsData GetUsingGoodDataByBagindex(GameClient client, int HolySite)
		{
			GoodsData result;
			if (client.ClientData.HolyGoodsDataList == null)
			{
				result = null;
			}
			else
			{
				foreach (GoodsData goodsData in client.ClientData.HolyGoodsDataList)
				{
					if (goodsData.Using == 1 && goodsData.BagIndex == HolySite)
					{
						return goodsData;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00031774 File Offset: 0x0002F974
		public static bool RemoveGoodsData(GameClient client, GoodsData gd)
		{
			bool result;
			if (null == gd)
			{
				result = false;
			}
			else if (client.ClientData.HolyGoodsDataList == null)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				lock (client.ClientData.HolyGoodsDataList)
				{
					ret = client.ClientData.HolyGoodsDataList.Remove(gd);
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00031804 File Offset: 0x0002FA04
		public static int GetGoodsGridNumByID(int goodsID)
		{
			SystemXmlItem systemGoods = null;
			int result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
			{
				result = 1;
			}
			else
			{
				result = systemGoods.GetIntValue("GridNum", -1);
			}
			return result;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00031840 File Offset: 0x0002FA40
		private static int CalGoodsGridNum(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			int gridNum = MountHolyStampManager.GetGoodsGridNumByID(goodsID);
			gridNum = Global.GMax(gridNum, 1);
			int result;
			if (client.ClientData.HolyGoodsDataList == null)
			{
				result = (newGoodsNum - 1) / gridNum + 1;
			}
			else
			{
				int totalGridNum = 0;
				lock (client.ClientData.HolyGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Using <= 0)
						{
							totalGridNum++;
							if (canUseOld && gridNum > 1)
							{
								if (client.ClientData.HolyGoodsDataList[i].GoodsID == goodsID && client.ClientData.HolyGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.HolyGoodsDataList[i].Endtime, endTime))
								{
									if (client.ClientData.HolyGoodsDataList[i].GCount < gridNum)
									{
										newGoodsNum -= Global.GMin(newGoodsNum, gridNum - client.ClientData.HolyGoodsDataList[i].GCount);
									}
								}
							}
						}
					}
				}
				if (newGoodsNum <= 0)
				{
					result = 0;
				}
				else
				{
					result = (newGoodsNum - 1) / gridNum + 1;
				}
			}
			return result;
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00031A04 File Offset: 0x0002FC04
		public static int GetGoodsUsedGrid(GameClient client)
		{
			int ret = 0;
			int result;
			if (client.ClientData.HolyGoodsDataList == null)
			{
				result = ret;
			}
			else
			{
				lock (client.ClientData.HolyGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Using <= 0)
						{
							ret++;
						}
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00031AC0 File Offset: 0x0002FCC0
		public static bool CanAddGoods(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.HolyGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int needGrid = MountHolyStampManager.CalGoodsGridNum(client, goodsID, newGoodsNum, binding, endTime, canUseOld);
				int haveGoodsCount = MountHolyStampManager.GetGoodsUsedGrid(client);
				result = (haveGoodsCount + needGrid <= MountHolyStampManager.HolyBagNum);
			}
			return result;
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00031B10 File Offset: 0x0002FD10
		public static bool CanAddGoodsNum(GameClient client, int newGoodsCount)
		{
			bool result;
			if (newGoodsCount <= 0)
			{
				result = false;
			}
			else
			{
				int haveGoodsCount = MountHolyStampManager.GetGoodsUsedGrid(client);
				result = (newGoodsCount + haveGoodsCount <= MountHolyStampManager.HolyBagNum);
			}
			return result;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00031B44 File Offset: 0x0002FD44
		public List<int> SetHolyStampAttr(GoodsData goodsData)
		{
			List<int> result;
			if (goodsData == null || this.holyStampUpLeveL == null || !this.holyStampUpLeveL.ContainsKey(goodsData.GoodsID) || goodsData.ElementhrtsProps == null || goodsData.ElementhrtsProps.Count != 2)
			{
				result = null;
			}
			else
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
				{
					result = null;
				}
				else
				{
					lock (this.holyStampUpLeveL[goodsData.GoodsID])
					{
						foreach (HolyStampUpLeve it in this.holyStampUpLeveL[goodsData.GoodsID])
						{
							if (980 == systemGoods.GetIntValue("Categoriy", -1) && it.Level == goodsData.ElementhrtsProps[0])
							{
								if (this.GoodsLvAttrCount.ContainsKey(goodsData.GoodsID) && this.GoodsLvAttrCount[goodsData.GoodsID].ContainsKey(it.Level))
								{
									int Count = this.GoodsLvAttrCount[goodsData.GoodsID][it.Level];
									if (goodsData.WashProps == null)
									{
										if (this.holyStampAttr.ContainsKey(it.Type) && this.holyStampAttr[it.Type].ContainsKey(it.Quality))
										{
											goodsData.WashProps = new List<int>();
											for (int i = 0; i < Count; i++)
											{
												int rand = Global.GetRandomNumber(0, this.holyStampAttr[it.Type][it.Quality].Count);
												if (this.holyStampAttr[it.Type][it.Quality][rand] != null)
												{
													HolyStampAttr Attr = this.GetAttrByQuality(it, goodsData, null);
													if (Attr != null)
													{
														using (Dictionary<int, double>.Enumerator enumerator2 = Attr.AttrList.GetEnumerator())
														{
															if (enumerator2.MoveNext())
															{
																KeyValuePair<int, double> iter = enumerator2.Current;
																goodsData.WashProps.Add(Attr.ID);
																goodsData.WashProps.Add(iter.Key);
																goodsData.WashProps.Add(Convert.ToInt32(iter.Value * 1000.0));
															}
														}
													}
												}
											}
											return goodsData.WashProps;
										}
									}
									else if (goodsData.WashProps.Count > 0)
									{
										int num = goodsData.WashProps.Count / 3;
										int res = Count - num;
										if (res > 0)
										{
											for (int i = 0; i < res; i++)
											{
												int rand = Global.GetRandomNumber(0, this.holyStampAttr[it.Type][it.Quality].Count);
												if (this.holyStampAttr[it.Type][it.Quality][rand] != null)
												{
													HolyStampAttr Attr = this.GetAttrByQuality(it, goodsData, null);
													if (Attr != null)
													{
														using (Dictionary<int, double>.Enumerator enumerator2 = Attr.AttrList.GetEnumerator())
														{
															if (enumerator2.MoveNext())
															{
																KeyValuePair<int, double> iter = enumerator2.Current;
																goodsData.WashProps.Add(Attr.ID);
																goodsData.WashProps.Add(iter.Key);
																goodsData.WashProps.Add(Convert.ToInt32(iter.Value * 1000.0));
															}
														}
													}
												}
											}
											return goodsData.WashProps;
										}
									}
								}
							}
							else if (981 == systemGoods.GetIntValue("Categoriy", -1) && it.Level == -1)
							{
								return null;
							}
						}
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00032064 File Offset: 0x00030264
		public int GetCurrHolyMaxLevelExp(HolyStampUpLeve data, int currLevel)
		{
			int result;
			lock (this.Mutex)
			{
				if (data == null)
				{
					result = -1;
				}
				else if (!this.GoodsLvDict.ContainsKey(data.GoodID))
				{
					result = -1;
				}
				else if (!this.holyStampUpLeveL.ContainsKey(data.GoodID))
				{
					result = -1;
				}
				else
				{
					int temp = this.GoodsLvDict[data.GoodID] - 1;
					foreach (HolyStampUpLeve it in this.holyStampUpLeveL[data.GoodID])
					{
						if (it.Level == temp)
						{
							return it.UpExp;
						}
					}
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00032184 File Offset: 0x00030384
		public MountHolyOpcode ProcessHolyStampUpGrade(GameClient client, int Dbid, string PhagocytosisDBids)
		{
			int result = 1;
			bool breakFlag = false;
			GoodsData goodData = null;
			goodData = this.GetGoodsByDbID(client, Dbid);
			if (goodData == null)
			{
				result = 5;
			}
			else
			{
				HolyStampUpLeve info = this.GetHolyUpGradeInfo(goodData);
				if (info == null)
				{
					result = 9;
				}
				else if (!this.GoodsLvDict.ContainsKey(goodData.GoodsID))
				{
					result = 9;
				}
				else if (info.Level == this.GoodsLvDict[goodData.GoodsID])
				{
					result = 10;
				}
				else
				{
					SystemXmlItem systemGoodsXml = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodData.GoodsID, out systemGoodsXml))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("圣印升级类型错误{0}", goodData.GoodsID), null, true);
						result = 5;
					}
					else if (980 != systemGoodsXml.GetIntValue("Categoriy", -1))
					{
						result = 4;
					}
					else if (string.IsNullOrEmpty(PhagocytosisDBids))
					{
						result = 3;
					}
					else
					{
						Dictionary<int, int> GoodDict = new Dictionary<int, int>();
						string[] Goods = PhagocytosisDBids.Split(new char[]
						{
							'|'
						});
						foreach (string it in Goods)
						{
							if (!string.IsNullOrEmpty(it))
							{
								string[] GoodAll = it.Split(new char[]
								{
									','
								});
								if (GoodAll != null && GoodAll.Length == 2)
								{
									int GoodDbid = Convert.ToInt32(GoodAll[0]);
									int GoodNum = Convert.ToInt32(GoodAll[1]);
									if (GoodDbid == Dbid)
									{
										result = 11;
										break;
									}
									if (GoodDict.ContainsKey(GoodDbid))
									{
										Dictionary<int, int> dictionary;
										int key;
										(dictionary = GoodDict)[key = GoodDbid] = dictionary[key] + GoodNum;
									}
									else
									{
										GoodDict.Add(GoodDbid, GoodNum);
									}
								}
							}
						}
						if (result != 11)
						{
							int MaxExp = this.GetCurrHolyMaxLevelExp(info, goodData.ElementhrtsProps[0]);
							if (MaxExp <= 0)
							{
								result = 9;
							}
							else
							{
								MaxExp -= goodData.ElementhrtsProps[1];
								MaxExp = Math.Max(0, MaxExp);
								int TotleExp = 0;
								int i = 0;
								Dictionary<int, Dictionary<int, GoodsData>> dict = new Dictionary<int, Dictionary<int, GoodsData>>();
								lock (GoodDict)
								{
									foreach (KeyValuePair<int, int> it2 in GoodDict)
									{
										GoodsData goodTemp = this.GetGoodsByDbID(client, it2.Key);
										if (goodTemp == null)
										{
											result = 6;
											breakFlag = true;
											break;
										}
										if (goodTemp.GCount < it2.Value || it2.Value <= 0)
										{
											result = 7;
											breakFlag = true;
											break;
										}
										SystemXmlItem systemGoods = null;
										if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodTemp.GoodsID, out systemGoods))
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("系统中不存在{0}", goodTemp.GoodsID), null, true);
											result = 8;
											breakFlag = true;
											break;
										}
										HolyStampUpLeve tempinfo = this.GetHolyUpGradeInfo(goodTemp);
										if (tempinfo == null)
										{
											result = 8;
											breakFlag = true;
											break;
										}
										bool expFlag = false;
										int num = 0;
										if (981 == systemGoods.GetIntValue("Categoriy", -1))
										{
											for (int j = 0; j < it2.Value; j++)
											{
												TotleExp += tempinfo.PhagocytosisExp;
												num++;
												if (TotleExp > MaxExp)
												{
													expFlag = true;
													break;
												}
											}
										}
										else
										{
											if (980 != systemGoods.GetIntValue("Categoriy", -1))
											{
												result = 11;
												breakFlag = true;
												break;
											}
											TotleExp += tempinfo.PhagocytosisExp;
										}
										i++;
										Dictionary<int, GoodsData> inDict = new Dictionary<int, GoodsData>();
										if (981 == systemGoods.GetIntValue("Categoriy", -1))
										{
											inDict.Add(num, goodTemp);
										}
										else
										{
											inDict.Add(it2.Value, goodTemp);
										}
										if (dict.ContainsKey(i))
										{
											dict[i] = inDict;
										}
										else
										{
											dict.Add(i, inDict);
										}
										if (TotleExp > MaxExp || expFlag)
										{
											break;
										}
									}
								}
								if (!breakFlag)
								{
									int UpAfterExp = 0;
									HolyStampUpLeve UpAfter = this.GetLevelUpCount(goodData, info, TotleExp, out UpAfterExp);
									if (UpAfter == null)
									{
										result = 12;
									}
									else if (!this.GoodsLvAttrCount.ContainsKey(goodData.GoodsID))
									{
										result = 13;
									}
									else if (!this.GoodsLvAttrCount[goodData.GoodsID].ContainsKey(UpAfter.Level))
									{
										result = 13;
									}
									else
									{
										List<int> newList = new List<int>();
										if (goodData.WashProps == null)
										{
											if (this.GoodsLvAttrCount[goodData.GoodsID][UpAfter.Level] > 0)
											{
												int k = 0;
												while (i < this.GoodsLvAttrCount[goodData.GoodsID][UpAfter.Level])
												{
													HolyStampAttr Attr = this.GetAttrByQuality(UpAfter, goodData, newList);
													if (Attr != null)
													{
														newList.Add(Attr.ID);
														using (Dictionary<int, double>.Enumerator enumerator2 = Attr.AttrList.GetEnumerator())
														{
															if (enumerator2.MoveNext())
															{
																KeyValuePair<int, double> it3 = enumerator2.Current;
																newList.Add(it3.Key);
																int value = (int)(it3.Value * 1000.0);
																newList.Add(value);
															}
														}
													}
													k++;
												}
											}
										}
										else if (goodData.WashProps.Count >= 0)
										{
											int num = goodData.WashProps.Count / 3;
											num = this.GoodsLvAttrCount[goodData.GoodsID][UpAfter.Level] - num;
											if (num > 0)
											{
												newList.AddRange(goodData.WashProps);
												for (int k = 0; k < num; k++)
												{
													HolyStampAttr Attr = this.GetAttrByQuality(UpAfter, goodData, newList);
													if (Attr != null)
													{
														newList.Add(Attr.ID);
														using (Dictionary<int, double>.Enumerator enumerator2 = Attr.AttrList.GetEnumerator())
														{
															if (enumerator2.MoveNext())
															{
																KeyValuePair<int, double> it3 = enumerator2.Current;
																newList.Add(it3.Key);
																int value = (int)(it3.Value * 1000.0);
																newList.Add(value);
															}
														}
													}
												}
											}
										}
										breakFlag = false;
										foreach (KeyValuePair<int, Dictionary<int, GoodsData>> goodDict in dict)
										{
											foreach (KeyValuePair<int, GoodsData> iter in goodDict.Value)
											{
												bool bind;
												if (!RebornStone.RebornHoleRemoveUseGoods(client, iter.Value, iter.Key, out bind))
												{
													result = 15;
													breakFlag = true;
													break;
												}
											}
										}
										if (!breakFlag)
										{
											goodData.ElementhrtsProps[0] = UpAfter.Level;
											if (UpAfter.Level >= this.GoodsLvDict[goodData.GoodsID])
											{
												goodData.ElementhrtsProps[1] = 0;
											}
											else
											{
												goodData.ElementhrtsProps[1] = UpAfterExp;
											}
											UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
											{
												RoleID = client.ClientData.RoleID,
												DbID = goodData.Id
											};
											updateGoodsArgs.ElementhrtsProps = new List<int>();
											updateGoodsArgs.ElementhrtsProps.Add(goodData.ElementhrtsProps[0]);
											updateGoodsArgs.ElementhrtsProps.Add(goodData.ElementhrtsProps[1]);
											int old = 3;
											if (goodData.WashProps != null)
											{
												old = goodData.WashProps.Count;
											}
											if (newList.Count >= old)
											{
												updateGoodsArgs.WashProps = new List<int>();
												updateGoodsArgs.WashProps.AddRange(newList);
											}
											if (goodData.Using == 1)
											{
												Global.RefreshEquipProp(client);
												GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
												GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
											}
											if (Global.UpdateGoodsProp(client, goodData, updateGoodsArgs, true) < 0)
											{
												result = 12;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			byte[] bytesCmd;
			if (result == 1)
			{
				bytesCmd = DataHelper.ObjectToBytes<GoodsData>(goodData);
			}
			else
			{
				bytesCmd = DataHelper.ObjectToBytes<GoodsData>(new GoodsData
				{
					Id = -1,
					GoodsID = result
				});
			}
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytesCmd, 0, bytesCmd.Length, 2090);
			Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
			return MountHolyOpcode.Succ;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00032C6C File Offset: 0x00030E6C
		public bool ModifyHolyStampState(GameClient client, int DBid, int IsUsing, int Site, int Bagindex)
		{
			string[] dbFields = null;
			string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
			{
				client.ClientData.RoleID,
				DBid,
				IsUsing,
				"*",
				"*",
				"*",
				Site,
				"*",
				"*",
				"*",
				"*",
				Bagindex,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				"*"
			});
			TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out dbFields, client.ServerId);
			return dbRequestResult != TCPProcessCmdResults.RESULT_FAILED && dbFields.Length > 0 && Convert.ToInt32(dbFields[1]) >= 0;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00032DE0 File Offset: 0x00030FE0
		public MountHolyOpcode ProcessHolyStampUsing(GameClient client, int Dbid)
		{
			GoodsData goodData = this.GetGoodsByDbID(client, Dbid);
			MountHolyOpcode result;
			if (goodData == null)
			{
				result = MountHolyOpcode.NotExsitGood;
			}
			else if (goodData.Using == 1)
			{
				result = MountHolyOpcode.GoodHasUsing;
			}
			else
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodData.GoodsID, out systemGoods))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("系统中不存在{0}", goodData.GoodsID), null, true);
					result = MountHolyOpcode.NotExsitGoodXml;
				}
				else if (980 != systemGoods.GetIntValue("Categoriy", -1))
				{
					result = MountHolyOpcode.NotUsingType;
				}
				else
				{
					int MaxSite = this.GetGolaNum(client);
					if (MaxSite <= 0)
					{
						result = MountHolyOpcode.GetHoleNumErr;
					}
					else
					{
						HolyStampUpLeve info = this.GetHolyUpGradeInfo(goodData);
						if (info == null)
						{
							result = MountHolyOpcode.NotExsitInfo;
						}
						else if (info.Site > MaxSite)
						{
							result = MountHolyOpcode.CurrHoleLock;
						}
						else
						{
							GoodsData good = this.GetUsingGoodDataByBagindex(client, info.Site);
							if (good != null)
							{
								int bagindex;
								if (MountHolyStampManager.CanAddGoods(client, good.GoodsID, 1, good.Binding, "1900-01-01 12:00:00", true))
								{
									bagindex = MountHolyStampManager.GetIdleSlotOfGoods(client);
								}
								else
								{
									if (client.ClientData.HolyGoodsDataList == null || client.ClientData.HolyGoodsDataList.Count != MountHolyStampManager.HolyBagNum)
									{
										return MountHolyOpcode.HolyGoodListNotFree;
									}
									bagindex = goodData.BagIndex;
								}
								if (!this.ModifyHolyStampState(client, good.Id, 0, 16000, bagindex))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("卸下圣印数据出错 GoodsID={0} DBID={1}", goodData.GoodsID, good.Id), null, true);
									return MountHolyOpcode.DataModifyErr;
								}
								good.Using = 0;
								good.BagIndex = bagindex;
								GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 3, good.Id, good.Using, good.Site, good.GCount, good.BagIndex, 1);
							}
							if (!this.ModifyHolyStampState(client, goodData.Id, 1, 16000, info.Site))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("佩戴圣印数据出错 GoodsID={0} DBID={1}", goodData.GoodsID, good.Id), null, true);
								result = MountHolyOpcode.DataModifyErr;
							}
							else
							{
								goodData.Using = 1;
								goodData.BagIndex = info.Site;
								GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 3, goodData.Id, goodData.Using, goodData.Site, goodData.GCount, goodData.BagIndex, 1);
								if (goodData.Using == 1)
								{
									Global.RefreshEquipProp(client);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
								}
								result = MountHolyOpcode.Succ;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00033134 File Offset: 0x00031334
		public MountHolyOpcode ProcessHolyStampDisUsing(GameClient client, int Dbid)
		{
			GoodsData goodData = this.GetGoodsByDbID(client, Dbid);
			MountHolyOpcode result;
			if (goodData == null)
			{
				result = MountHolyOpcode.NotExsitGood;
			}
			else if (goodData.Using <= 0)
			{
				result = MountHolyOpcode.GoodHasNot;
			}
			else
			{
				HolyStampUpLeve info = this.GetHolyUpGradeInfo(goodData);
				if (info == null)
				{
					result = MountHolyOpcode.NotExsitInfo;
				}
				else
				{
					GoodsData good = this.GetUsingGoodDataByBagindex(client, info.Site);
					if (good == null)
					{
						result = MountHolyOpcode.HoleInfoIsNull;
					}
					else if (!MountHolyStampManager.CanAddGoods(client, good.GoodsID, 1, good.Binding, "1900-01-01 12:00:00", true))
					{
						result = MountHolyOpcode.HolyGoodListNotFree;
					}
					else
					{
						int bagIndex = MountHolyStampManager.GetIdleSlotOfGoods(client);
						if (!this.ModifyHolyStampState(client, good.Id, 0, 16000, bagIndex))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("卸下圣印数据出错 GoodsID={0} DBID={1}", goodData.GoodsID, good.Id), null, true);
							result = MountHolyOpcode.DataModifyErr;
						}
						else
						{
							good.Using = 0;
							good.BagIndex = bagIndex;
							GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 3, good.Id, good.Using, good.Site, good.GCount, good.BagIndex, 1);
							Global.RefreshEquipProp(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							result = MountHolyOpcode.Succ;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x000332DC File Offset: 0x000314DC
		public static GoodsData GetMountHolyGoodsByDbID(GameClient client, int dbID)
		{
			GoodsData result;
			if (null == client.ClientData.HolyGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.HolyGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Id == dbID)
						{
							return client.ClientData.HolyGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00033490 File Offset: 0x00031690
		private void ResetHolyStampBag(GameClient client, bool notifyClient = true)
		{
			byte[] bytesCmd = null;
			if (client.ClientData.HolyGoodsDataList != null)
			{
				lock (client.ClientData.HolyGoodsDataList)
				{
					Dictionary<string, GoodsData> oldGoodsDict = new Dictionary<string, GoodsData>();
					List<GoodsData> toRemovedGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Using <= 0)
						{
							client.ClientData.HolyGoodsDataList[i].BagIndex = 0;
							int gridNum = Global.GetGoodsGridNumByID(client.ClientData.HolyGoodsDataList[i].GoodsID);
							if (gridNum > 1)
							{
								GoodsData oldGoodsData = null;
								string key = string.Format("{0}_{1}_{2}_{3}", new object[]
								{
									client.ClientData.HolyGoodsDataList[i].GoodsID,
									client.ClientData.HolyGoodsDataList[i].Binding,
									Global.DateTimeTicks(client.ClientData.HolyGoodsDataList[i].Starttime),
									Global.DateTimeTicks(client.ClientData.HolyGoodsDataList[i].Endtime)
								});
								if (oldGoodsDict.TryGetValue(key, out oldGoodsData))
								{
									int toAddNum = Global.GMin(gridNum - oldGoodsData.GCount, client.ClientData.HolyGoodsDataList[i].GCount);
									oldGoodsData.GCount += toAddNum;
									client.ClientData.HolyGoodsDataList[i].GCount -= toAddNum;
									client.ClientData.HolyGoodsDataList[i].BagIndex = 1;
									oldGoodsData.BagIndex = 1;
									if (!Global.ResetBagGoodsData(client, client.ClientData.HolyGoodsDataList[i]))
									{
										break;
									}
									EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, oldGoodsData.GoodsID, (long)oldGoodsData.Id, toAddNum, oldGoodsData.GCount, "整理圣印背包");
									EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, client.ClientData.HolyGoodsDataList[i].GoodsID, (long)client.ClientData.HolyGoodsDataList[i].Id, -toAddNum, client.ClientData.HolyGoodsDataList[i].GCount, "整理圣印背包");
									if (oldGoodsData.GCount >= gridNum)
									{
										if (client.ClientData.HolyGoodsDataList[i].GCount > 0)
										{
											oldGoodsDict[key] = client.ClientData.HolyGoodsDataList[i];
										}
										else
										{
											oldGoodsDict.Remove(key);
											toRemovedGoodsDataList.Add(client.ClientData.HolyGoodsDataList[i]);
										}
									}
									else if (client.ClientData.HolyGoodsDataList[i].GCount <= 0)
									{
										toRemovedGoodsDataList.Add(client.ClientData.HolyGoodsDataList[i]);
									}
								}
								else
								{
									oldGoodsDict[key] = client.ClientData.HolyGoodsDataList[i];
								}
							}
						}
					}
					for (int i = 0; i < toRemovedGoodsDataList.Count; i++)
					{
						client.ClientData.HolyGoodsDataList.Remove(toRemovedGoodsDataList[i]);
					}
					client.ClientData.HolyGoodsDataList.Sort(delegate(GoodsData _left, GoodsData _right)
					{
						int lvlDiff = 0;
						if (_left.ElementhrtsProps != null && _right.ElementhrtsProps != null)
						{
							lvlDiff = _left.ElementhrtsProps[0] - _right.ElementhrtsProps[0];
						}
						int result;
						if (lvlDiff > 0)
						{
							result = -1;
						}
						else if (lvlDiff < 0)
						{
							result = 1;
						}
						else
						{
							HolyStampUpLeve linfo = this.GetHolyUpGradeInfo(_left);
							if (linfo == null)
							{
								result = -1;
							}
							else
							{
								HolyStampUpLeve rinfo = this.GetHolyUpGradeInfo(_right);
								if (rinfo == null)
								{
									result = -1;
								}
								else
								{
									int l_suit = linfo.Quality;
									int r_suit = rinfo.Quality;
									if (l_suit > r_suit)
									{
										result = -1;
									}
									else if (l_suit < r_suit)
									{
										result = 1;
									}
									else
									{
										result = _right.GoodsID - _left.GoodsID;
									}
								}
							}
						}
						return result;
					});
					int index = 0;
					for (int i = 0; i < client.ClientData.HolyGoodsDataList.Count; i++)
					{
						if (client.ClientData.HolyGoodsDataList[i].Using <= 0)
						{
							if (GameManager.Flag_OptimizationBagReset)
							{
								bool godosCountChanged = client.ClientData.HolyGoodsDataList[i].BagIndex > 0;
								client.ClientData.HolyGoodsDataList[i].BagIndex = index++;
								if (godosCountChanged)
								{
									if (!Global.ResetBagGoodsData(client, client.ClientData.HolyGoodsDataList[i]))
									{
										break;
									}
								}
							}
							else
							{
								client.ClientData.HolyGoodsDataList[i].BagIndex = index++;
								if (!Global.ResetBagGoodsData(client, client.ClientData.HolyGoodsDataList[i]))
								{
									break;
								}
							}
						}
					}
					bytesCmd = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.HolyGoodsDataList);
				}
				if (notifyClient)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytesCmd, 0, bytesCmd.Length, 2093);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		// Token: 0x060002EE RID: 750 RVA: 0x00033A04 File Offset: 0x00031C04
		public void InitRoleHolyStampGoodsData(GameClient client)
		{
			if (null == client.ClientData.HolyGoodsDataList)
			{
				client.ClientData.HolyGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 16000), client.ServerId);
				if (null == client.ClientData.HolyGoodsDataList)
				{
					client.ClientData.HolyGoodsDataList = new List<GoodsData>();
				}
			}
			if (null == client.ClientData.HolyGoodsDataList)
			{
				client.ClientData.HolyGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 16000), client.ServerId);
				if (null == client.ClientData.HolyGoodsDataList)
				{
					client.ClientData.HolyGoodsDataList = new List<GoodsData>();
				}
			}
			this.ResetHolyStampBag(client, true);
			client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
			{
				DelayExecProcIds.RecalcProps,
				DelayExecProcIds.NotifyRefreshProps
			});
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00033B30 File Offset: 0x00031D30
		public static bool IsHolyStamp(int goodID)
		{
			int type = Global.GetGoodsCatetoriy(goodID);
			return type == 980;
		}

		// Token: 0x04000451 RID: 1105
		public const int DefaultLevel = 1;

		// Token: 0x04000452 RID: 1106
		public Dictionary<int, HolyStampSuit> holyStampSuit = new Dictionary<int, HolyStampSuit>();

		// Token: 0x04000453 RID: 1107
		public Dictionary<int, Dictionary<int, List<HolyStampAttr>>> holyStampAttr = new Dictionary<int, Dictionary<int, List<HolyStampAttr>>>();

		// Token: 0x04000454 RID: 1108
		public Dictionary<int, List<HolyStampUpLeve>> holyStampUpLeveL = new Dictionary<int, List<HolyStampUpLeve>>();

		// Token: 0x04000455 RID: 1109
		public Dictionary<int, int> holyStampDesbloquear = new Dictionary<int, int>();

		// Token: 0x04000456 RID: 1110
		public Dictionary<int, int> GoodsLvDict = new Dictionary<int, int>();

		// Token: 0x04000457 RID: 1111
		public Dictionary<int, Dictionary<int, int>> GoodsLvAttrCount = new Dictionary<int, Dictionary<int, int>>();

		// Token: 0x04000458 RID: 1112
		protected object Mutex = new object();

		// Token: 0x04000459 RID: 1113
		public static int HolyBagNum = 200;

		// Token: 0x0400045A RID: 1114
		private static MountHolyStampManager instance = new MountHolyStampManager();
	}
}
