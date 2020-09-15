using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.Reborn
{
	// Token: 0x020003F1 RID: 1009
	public class RebornEquip
	{
		// Token: 0x06001153 RID: 4435 RVA: 0x0010F48C File Offset: 0x0010D68C
		public static RebornEquip getInstance()
		{
			return RebornEquip.instance;
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x0010F4A4 File Offset: 0x0010D6A4
		public static bool ParseRebornEquipConfig()
		{
			Dictionary<int, RebornEquipXmlStruct> EquipSQSWHot = new Dictionary<int, RebornEquipXmlStruct>();
			string fileName = Global.GameResPath(RebornEquipConst.RebornEquip);
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
			}
			try
			{
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					int GoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "GoodsID"));
					if (RebornEquip.IsRebornEquipShengWu(GoodID) || RebornEquip.IsRebornEquipShengQi(GoodID))
					{
						EquipSQSWHot.Add(GoodID, new RebornEquipXmlStruct
						{
							ZSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ZSMod")),
							FSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "FSMod")),
							GJSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "GSMOd")),
							LMJSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "LMJSMod")),
							FMJSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "FMJSMod")),
							ZHSModGoodID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ZHSMod"))
						});
					}
				}
				RebornEquip.EquipSQSW = EquipSQSWHot;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			bool result;
			if (RebornEquip.EquipSQSW == null)
			{
				result = false;
			}
			else
			{
				fileName = Global.GameResPath(RebornEquipConst.RebornEquipEvolution);
				xml = XElement.Load(fileName);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
				}
				try
				{
					Dictionary<int, RebornEquipEvolution> EvolutionHot = new Dictionary<int, RebornEquipEvolution>();
					Dictionary<int, RebornEquipEvolution> RebornEquipUpHot = new Dictionary<int, RebornEquipEvolution>();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						RebornEquipEvolution rev = new RebornEquipEvolution();
						rev.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
						rev.NewEquitID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "NewEquitID"));
						rev.NeedEquitID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "NeedEquitID"));
						rev.NeedCuiLian = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "NeedCuiLian"));
						rev.NeedDuanZao = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "NeedDuanZao"));
						rev.NeedNiePan = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "NeedNiePan"));
						rev.SuccessRate = Convert.ToDouble(Global.GetSafeAttributeStr(xmlItem, "SuccessRate"));
						EvolutionHot.Add(rev.ID, rev);
						RebornEquipUpHot.Add(rev.NeedEquitID, rev);
					}
					RebornEquip.Evolution = EvolutionHot;
					RebornEquip.RebornEquipUp = RebornEquipUpHot;
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				if (RebornEquip.Evolution == null && RebornEquip.RebornEquipUp == null)
				{
					result = false;
				}
				else
				{
					fileName = Global.GameResPath(RebornEquipConst.RebornSuperiorDrop);
					xml = XElement.Load(fileName);
					if (null == xml)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
					}
					try
					{
						Dictionary<int, RebornSuperiorDrop> SuperiorDropHot = new Dictionary<int, RebornSuperiorDrop>();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							RebornSuperiorDrop rsd = new RebornSuperiorDrop();
							List<int> RebornSuperiorInherentNum = new List<int>();
							List<int> RebornSuperiorInherentBank = new List<int>();
							Dictionary<double, int> RebornSuperiorRate = new Dictionary<double, int>();
							Dictionary<double, int> RebornSuperiorRate2 = new Dictionary<double, int>();
							Dictionary<double, int> RebornSuperiorRate3 = new Dictionary<double, int>();
							List<int> RebornSuperiorBank = new List<int>();
							List<int> RebornSuperiorBank2 = new List<int>();
							List<int> RebornSuperiorBank3 = new List<int>();
							rsd.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
							string RebornSuperiorInherentNumStr = Global.GetSafeAttributeStr(xmlItem, "RebornSuperiorInherentNum");
							string[] RSINS = RebornSuperiorInherentNumStr.Split(new char[]
							{
								','
							});
							foreach (string it in RSINS)
							{
								RebornSuperiorInherentNum.Add(Convert.ToInt32(it));
								rsd.RebornSuperiorInherentNum = RebornSuperiorInherentNum;
							}
							string RebornSuperiorInherentBankStr = Global.GetSafeAttributeStr(xmlItem, "RebornSuperiorInherentBank");
							string[] RSIBS = RebornSuperiorInherentBankStr.Split(new char[]
							{
								','
							});
							foreach (string it in RSIBS)
							{
								RebornSuperiorInherentBank.Add(Convert.ToInt32(it));
								rsd.RebornSuperiorInherentBank = RebornSuperiorInherentBank;
							}
							string RebornSuperiorRateStr = Global.GetSafeAttributeStr(xmlItem, "RebornSuperiorRate1");
							double rvalue = 0.0;
							string[] RSRS = RebornSuperiorRateStr.Split(new char[]
							{
								'|'
							});
							foreach (string it in RSRS)
							{
								rvalue += Convert.ToDouble(it.Split(new char[]
								{
									','
								})[1]);
								if (rvalue != 0.0)
								{
									RebornSuperiorRate.Add(rvalue, Convert.ToInt32(it.Split(new char[]
									{
										','
									})[0]));
									rsd.RebornSuperiorRate1 = RebornSuperiorRate;
								}
							}
							string RebornSuperiorBankStr = Global.GetSafeAttributeStr(xmlItem, "RebornSuperiorBank1");
							string[] RSBS = RebornSuperiorBankStr.Split(new char[]
							{
								','
							});
							foreach (string it in RSBS)
							{
								RebornSuperiorBank.Add(Convert.ToInt32(it));
								rsd.RebornSuperiorBank1 = RebornSuperiorBank;
							}
							string RebornSuperiorRateStr2 = Global.GetSafeAttributeStr(xmlItem, "RebornSuperiorRate2");
							double rvalue2 = 0.0;
							string[] RSRS2 = RebornSuperiorRateStr2.Split(new char[]
							{
								'|'
							});
							foreach (string it in RSRS2)
							{
								rvalue2 += Convert.ToDouble(it.Split(new char[]
								{
									','
								})[1]);
								if (rvalue2 != 0.0)
								{
									RebornSuperiorRate2.Add(rvalue2, Convert.ToInt32(it.Split(new char[]
									{
										','
									})[0]));
									rsd.RebornSuperiorRate2 = RebornSuperiorRate2;
								}
							}
							string RebornSuperiorBankStr2 = Global.GetSafeAttributeStr(xmlItem, "RebornSuperiorBank2");
							string[] RSBS2 = RebornSuperiorBankStr2.Split(new char[]
							{
								','
							});
							foreach (string it in RSBS2)
							{
								RebornSuperiorBank2.Add(Convert.ToInt32(it));
								rsd.RebornSuperiorBank2 = RebornSuperiorBank2;
							}
							string RebornSuperiorRateStr3 = Global.GetSafeAttributeStr(xmlItem, "RebornSuperiorRate3");
							double rvalue3 = 0.0;
							string[] RSRS3 = RebornSuperiorRateStr3.Split(new char[]
							{
								'|'
							});
							foreach (string it in RSRS3)
							{
								rvalue3 += Convert.ToDouble(it.Split(new char[]
								{
									','
								})[1]);
								if (rvalue3 != 0.0)
								{
									RebornSuperiorRate3.Add(rvalue3, Convert.ToInt32(it.Split(new char[]
									{
										','
									})[0]));
									rsd.RebornSuperiorRate3 = RebornSuperiorRate3;
								}
							}
							string RebornSuperiorBankStr3 = Global.GetSafeAttributeStr(xmlItem, "RebornSuperiorBank3");
							string[] RSBS3 = RebornSuperiorBankStr3.Split(new char[]
							{
								','
							});
							foreach (string it in RSBS3)
							{
								RebornSuperiorBank3.Add(Convert.ToInt32(it));
								rsd.RebornSuperiorBank3 = RebornSuperiorBank3;
							}
							rsd.ShowColor = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ShowColor"));
							SuperiorDropHot.Add(rsd.ID, rsd);
						}
						RebornEquip.SuperiorDrop = SuperiorDropHot;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
					if (RebornEquip.SuperiorDrop == null)
					{
						result = false;
					}
					else
					{
						Dictionary<int, RebornSuperiorType> SuperiorTypeHot = new Dictionary<int, RebornSuperiorType>();
						fileName = Global.GameResPath(RebornEquipConst.RebornSuperiorType);
						xml = XElement.Load(fileName);
						if (null == xml)
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
						}
						try
						{
							IEnumerable<XElement> xmlItems = xml.Elements();
							foreach (XElement xmlItem in xmlItems)
							{
								RebornSuperiorType rst = new RebornSuperiorType();
								Dictionary<double, double> Parameter = new Dictionary<double, double>();
								rst.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
								rst.Type = (int)ConfigParser.GetPropIndexByPropName(Global.GetSafeAttributeStr(xmlItem, "Type"));
								string[] ParameterStrList = Global.GetSafeAttributeStr(xmlItem, "Parameter").Split(new char[]
								{
									'|'
								});
								double rvalue4 = 0.0;
								foreach (string it in ParameterStrList)
								{
									rvalue4 += Convert.ToDouble(it.Split(new char[]
									{
										','
									})[1]);
									Parameter.Add(Convert.ToDouble(it.Split(new char[]
									{
										','
									})[0]), rvalue4);
									rst.Parameter = Parameter;
								}
								SuperiorTypeHot.Add(rst.ID, rst);
							}
							RebornEquip.SuperiorType = SuperiorTypeHot;
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
						if (RebornEquip.SuperiorType == null)
						{
							result = false;
						}
						else
						{
							Dictionary<int, Dictionary<int, RebornQuenching>> RebornEquipHoleHot = new Dictionary<int, Dictionary<int, RebornQuenching>>();
							Dictionary<int, int> RebornEquipHoleLevelMapHot = new Dictionary<int, int>();
							fileName = Global.GameResPath(RebornEquipConst.RebornEquipQuenching);
							xml = XElement.Load(fileName);
							if (null == xml)
							{
								LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
							}
							try
							{
								IEnumerable<XElement> xmlItems = xml.Elements();
								foreach (XElement xmlItem in xmlItems)
								{
									RebornQuenching rst2 = new RebornQuenching();
									rst2.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
									rst2.Perfusion = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "PerfusionName"));
									rst2.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "PerfusionLevel"));
									Dictionary<int, double> attrTemp = new Dictionary<int, double>();
									string attr = Global.GetSafeAttributeStr(xmlItem, "EverLelAttribute");
									if (attr.Equals("-1"))
									{
										attrTemp.Add(0, 0.0);
									}
									else
									{
										string[] AttrStrs = Global.GetSafeAttributeStr(xmlItem, "EverLelAttribute").Split(new char[]
										{
											'|'
										});
										foreach (string it in AttrStrs)
										{
											string[] str = it.Split(new char[]
											{
												','
											});
											int AttrID = (int)ConfigParser.GetPropIndexByPropName(str[0]);
											if (!attrTemp.ContainsKey(AttrID))
											{
												attrTemp.Add(AttrID, Convert.ToDouble(str[1]));
											}
										}
									}
									rst2.Attr = attrTemp;
									Dictionary<int, int> goodTemp = new Dictionary<int, int>();
									string[] LossItemStrs = Global.GetSafeAttributeStr(xmlItem, "LossItem").Split(new char[]
									{
										','
									});
									if (LossItemStrs.Length != 1)
									{
										int goodid = Convert.ToInt32(LossItemStrs[0]);
										if (!goodTemp.ContainsKey(goodid))
										{
											goodTemp.Add(goodid, Convert.ToInt32(LossItemStrs[1]));
										}
									}
									rst2.UseGoods = goodTemp;
									string[] IncreaseProbability = Global.GetSafeAttributeStr(xmlItem, "IncreaseProbability").Split(new char[]
									{
										'|'
									});
									if (IncreaseProbability.Length == 1)
									{
										rst2.AddStart = 0.0;
										rst2.AddEnd = 0.0;
									}
									else
									{
										rst2.AddStart = Convert.ToDouble(IncreaseProbability[0]);
										rst2.AddEnd = Convert.ToDouble(IncreaseProbability[1]);
									}
									string[] ReduceProbability = Global.GetSafeAttributeStr(xmlItem, "ReduceProbability").Split(new char[]
									{
										'|'
									});
									if (ReduceProbability.Length == 1)
									{
										rst2.SubStart = 0.0;
										rst2.SubEnd = 0.0;
									}
									else
									{
										rst2.SubStart = Convert.ToDouble(ReduceProbability[0]);
										rst2.SubEnd = Convert.ToDouble(ReduceProbability[1]);
									}
									string[] UpProbability = Global.GetSafeAttributeStr(xmlItem, "UpProbability").Split(new char[]
									{
										'|'
									});
									if (UpProbability.Length == 1)
									{
										rst2.SturmGaiLv = 0.0;
										rst2.SturmLevel = 0;
									}
									else
									{
										rst2.SturmGaiLv = Convert.ToDouble(UpProbability[0]);
										rst2.SturmLevel = Convert.ToInt32(UpProbability[1]);
									}
									rst2.AbschreckenUnterwerfen = Global.GetSafeAttributeDouble(xmlItem, "LelInterval");
									if (RebornEquipHoleHot.ContainsKey(rst2.Perfusion))
									{
										if (RebornEquipHoleHot[rst2.Perfusion].ContainsKey(rst2.Level))
										{
											RebornEquipHoleHot[rst2.Perfusion][rst2.Level] = rst2;
										}
										else
										{
											RebornEquipHoleHot[rst2.Perfusion].Add(rst2.Level, rst2);
										}
									}
									else
									{
										Dictionary<int, RebornQuenching> newDict = new Dictionary<int, RebornQuenching>();
										newDict.Add(rst2.Level, rst2);
										RebornEquipHoleHot.Add(rst2.Perfusion, newDict);
									}
									if (RebornEquipHoleLevelMapHot.ContainsKey(rst2.Perfusion))
									{
										if (RebornEquipHoleLevelMapHot[rst2.Perfusion] < rst2.Level)
										{
											RebornEquipHoleLevelMapHot[rst2.Perfusion] = rst2.Level;
										}
									}
									else
									{
										RebornEquipHoleLevelMapHot.Add(rst2.Perfusion, rst2.Level);
									}
								}
								RebornEquip.RebornEquipHole = RebornEquipHoleHot;
								RebornEquip.RebornEquipHoleLevelMap = RebornEquipHoleLevelMapHot;
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
							if (RebornEquip.RebornEquipHole == null || RebornEquip.RebornEquipHoleLevelMap == null)
							{
								result = false;
							}
							else
							{
								int Distance = 0;
								for (int idx = 122; idx <= 150; idx += 7)
								{
									int index = 165 + Distance;
									List<int> list = new List<int>();
									list.Add(index);
									list.Add(index + 1);
									if (!RebornEquip.ExtPropIndexMap.ContainsKey(idx))
									{
										RebornEquip.ExtPropIndexMap.Add(idx, list);
									}
									Distance += 2;
								}
								result = (RebornEquip.ExtPropIndexMap != null);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x001105C8 File Offset: 0x0010E7C8
		public void InitRoleRebornGoodsData(GameClient client)
		{
			if (GlobalNew.IsGongNengOpened(client, GongNengIDs.Reborn, false))
			{
				if (null == client.ClientData.RebornGoodsStoreList)
				{
					client.ClientData.RebornGoodsStoreList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 15001), client.ServerId);
					if (null == client.ClientData.RebornGoodsStoreList)
					{
						client.ClientData.RebornGoodsStoreList = new List<GoodsData>();
					}
				}
				if (null == client.ClientData.RebornGoodsDataList)
				{
					client.ClientData.RebornGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 15000), client.ServerId);
					if (null == client.ClientData.RebornGoodsDataList)
					{
						client.ClientData.RebornGoodsDataList = new List<GoodsData>();
					}
				}
				RebornEquip.ResetBagAllGoods(client, true);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
			}
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x00110704 File Offset: 0x0010E904
		public static void RefreshOneEquipProp(GameClient client, GoodsData goodsData, ref AllThingsCalcItem allThingsCalcItem)
		{
			SystemXmlItem systemGoods = null;
			systemGoods = null;
			if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
			{
				if (RebornEquip.IsRebornEquip(goodsData.GoodsID))
				{
					int nRet = client.UsingEquipMgr.EquipFirstPropCondition(client, systemGoods);
					EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(goodsData.GoodsID);
					if (null != item)
					{
						if (nRet == 1)
						{
							if (goodsData.Quality >= 4)
							{
								allThingsCalcItem.TotalGoldQualityNum++;
							}
							else if (goodsData.Quality >= 3)
							{
								allThingsCalcItem.TotalPurpleQualityNum++;
							}
						}
						if (nRet == 1)
						{
							allThingsCalcItem.ChangeTotalForgeLevel(goodsData.Forge_level, true);
						}
						Global.CalcExcellenceEquipNum(allThingsCalcItem, goodsData, nRet, true);
						if (!string.IsNullOrEmpty(goodsData.Jewellist) && nRet == 1)
						{
							AllThingsCalcItem singleEquipJewelItems = new AllThingsCalcItem();
							string[] jewelFields = goodsData.Jewellist.Split(new char[]
							{
								','
							});
							for (int x = 0; x < jewelFields.Length; x++)
							{
								int jewelGoodsID = Convert.ToInt32(jewelFields[x]);
								EquipPropItem jewelItem = GameManager.EquipPropsMgr.FindEquipPropItem(jewelGoodsID);
								if (null != jewelItem)
								{
									int jewelLevel = Global.GetJewelLevel(jewelGoodsID);
									if (jewelLevel >= 8)
									{
										allThingsCalcItem.TotalJewel8LevelNum++;
										singleEquipJewelItems.TotalJewel8LevelNum++;
									}
									else if (jewelLevel >= 7)
									{
										allThingsCalcItem.TotalJewel7LevelNum++;
										singleEquipJewelItems.TotalJewel7LevelNum++;
									}
									else if (jewelLevel >= 6)
									{
										allThingsCalcItem.TotalJewel6LevelNum++;
										singleEquipJewelItems.TotalJewel6LevelNum++;
									}
									else if (jewelLevel >= 5)
									{
										allThingsCalcItem.TotalJewel5LevelNum++;
										singleEquipJewelItems.TotalJewel5LevelNum++;
									}
									else if (jewelLevel >= 4)
									{
										allThingsCalcItem.TotalJewel4LevelNum++;
										singleEquipJewelItems.TotalJewel4LevelNum++;
									}
								}
							}
						}
						int maxStrong = (int)item.ExtProps[0];
						if (goodsData.Strong < maxStrong)
						{
							bool bRet = true;
							if (nRet != 1)
							{
								bRet = false;
							}
							if (goodsData.ExcellenceInfo != 0)
							{
								Global.ProcessEquipExcellenceProp(client, goodsData, bRet, systemGoods);
							}
							if (bRet && goodsData.Lucky > 0)
							{
								Global.ProcessEquipLuckProp(client, goodsData, bRet, systemGoods);
							}
							if (nRet == 1)
							{
								for (int i = 0; i < 5; i++)
								{
									client.ClientData.EquipProp.BaseProps[i] += Global.GetEquipBasePropsItemVal(goodsData, item, i);
								}
								for (int i = 0; i < 177; i++)
								{
									client.ClientData.EquipProp.ExtProps[i] += Global.GetEquipExtPropsItemVal(client, goodsData, item, i, systemGoods);
								}
							}
							int nOcc = Global.CalcOriginalOccupationID(client);
							ChuanQiQianHua.ApplayEquipQianHuaProps(client.ClientData.EquipProp.ExtProps, nOcc, goodsData, systemGoods, true);
							if (nRet > 0)
							{
								if (goodsData.WashProps != null && goodsData.WashProps.Count >= 2)
								{
									for (int i = 0; i < goodsData.WashProps.Count; i += 2)
									{
										int idx = goodsData.WashProps[i];
										if (0 < idx && idx < 177)
										{
											client.ClientData.EquipProp.ExtProps[idx] += (double)goodsData.WashProps[i + 1] * 0.001;
										}
									}
								}
								if (goodsData.ElementhrtsProps != null && goodsData.ElementhrtsProps.Count >= 2 && GoodsUtil.IsEquip(goodsData.GoodsID))
								{
									double zuoQiPercent = 0.001;
									for (int i = 0; i <= goodsData.ElementhrtsProps.Count - 2; i += 2)
									{
										int idx = goodsData.ElementhrtsProps[i];
										if (0 < idx && idx < 177)
										{
											client.ClientData.EquipProp.ExtProps[idx] += (double)goodsData.ElementhrtsProps[i + 1] * zuoQiPercent;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x00110C44 File Offset: 0x0010EE44
		public static List<GoodsData> GetUsingGoodsList(SafeClientData clientData)
		{
			List<GoodsData> result;
			if (null == clientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				List<GoodsData> RebornGoodsDataList = new List<GoodsData>();
				lock (clientData.RebornGoodsDataList)
				{
					for (int i = 0; i < clientData.RebornGoodsDataList.Count; i++)
					{
						if (clientData.RebornGoodsDataList[i].Using > 0 && GoodsUtil.IsVisiableEquip(clientData.RebornGoodsDataList[i].GoodsID))
						{
							RebornGoodsDataList.Add(clientData.RebornGoodsDataList[i]);
						}
					}
				}
				result = RebornGoodsDataList;
			}
			return result;
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x00110D1C File Offset: 0x0010EF1C
		public static int GetCurrGoodsQuality(GoodsData gd)
		{
			int result;
			if (!RebornEquip.IsRebornEquip(gd.GoodsID))
			{
				result = 0;
			}
			else if (gd.WashProps == null)
			{
				result = 0;
			}
			else
			{
				int Quality = gd.WashProps.Count / 2;
				if (Quality >= 6)
				{
					result = 5;
				}
				else if (Quality > 4)
				{
					result = 4;
				}
				else if (Quality >= 3)
				{
					result = 3;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x00110D90 File Offset: 0x0010EF90
		public static int CalcFixAllEquipsStrongMoney(GameClient client)
		{
			int result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = 0;
			}
			else if (client.ClientData.RebornGoodsDataList.Count <= 0)
			{
				result = 0;
			}
			else
			{
				int totalYinLiang = 0;
				List<GoodsData> RebornGoodsDataList = null;
				lock (client.ClientData.RebornGoodsDataList)
				{
					RebornGoodsDataList = client.ClientData.RebornGoodsDataList.GetRange(0, client.ClientData.RebornGoodsDataList.Count);
				}
				for (int i = 0; i < RebornGoodsDataList.Count; i++)
				{
					GoodsData goodsData = RebornGoodsDataList[i];
					if (goodsData.Using > 0)
					{
						int category = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (category >= 30 && category <= 38)
						{
							SystemXmlItem systemGoods = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods) && null != systemGoods)
							{
								int priceTwo = systemGoods.GetIntValue("PriceTwo", -1);
								int[] equipProps = systemGoods.GetIntArrayValue("EquipProps", ',');
								if (equipProps != null && equipProps.Length >= 2 && priceTwo > 0)
								{
									double fMaxStrong = (double)equipProps[0];
									if (fMaxStrong > 0.0 && (double)goodsData.Strong > 0.0)
									{
										int needYinLiang = (int)((double)priceTwo / 3.0 * (double)goodsData.Strong / fMaxStrong);
										needYinLiang = Global.RecalcNeedYinLiang(needYinLiang);
										totalYinLiang += needYinLiang;
									}
								}
							}
						}
					}
				}
				result = totalYinLiang;
			}
			return result;
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x00110F84 File Offset: 0x0010F184
		public static int VerifyWeaponCanEquip(Dictionary<int, List<GoodsData>> EquipDict)
		{
			int nWeaponCount = 0;
			List<GoodsData> listGood = null;
			int i = 37;
			while (i <= 38)
			{
				listGood = null;
				lock (EquipDict)
				{
					if (!EquipDict.TryGetValue(i, out listGood))
					{
						goto IL_CB;
					}
					if (listGood != null && listGood.Count > 0)
					{
						nWeaponCount += listGood.Count;
						for (int nCount = 0; nCount < listGood.Count; nCount++)
						{
							SystemXmlItem systemGoods = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(listGood[nCount].GoodsID, out systemGoods))
							{
								return -1;
							}
						}
					}
				}
				goto IL_B5;
				IL_CB:
				i++;
				continue;
				IL_B5:
				if (nWeaponCount <= 1)
				{
					goto IL_CB;
				}
				return -3;
			}
			return 0;
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x0011108C File Offset: 0x0010F28C
		public static bool isWarnRebornEquip(GameClient client, SystemXmlItem systemGoods)
		{
			int toReborn = systemGoods.GetIntValue("ToReborn", -1);
			int toRebornLevel = systemGoods.GetIntValue("ToRebornLevel", -1);
			return client.ClientData.RebornCount >= toReborn && client.ClientData.RebornLevel >= toRebornLevel;
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x001110E4 File Offset: 0x0010F2E4
		public static bool IsRebornEquip(int goodsID)
		{
			int type = Global.GetGoodsCatetoriy(goodsID);
			return type >= 30 && type <= 38;
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x00111118 File Offset: 0x0010F318
		public static bool IsRebornEquipShengWu(int goodsID)
		{
			int type = Global.GetGoodsCatetoriy(goodsID);
			return type == 37;
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x00111144 File Offset: 0x0010F344
		public static bool IsRebornEquipShengQi(int goodsID)
		{
			int type = Global.GetGoodsCatetoriy(goodsID);
			return type == 38;
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x00111170 File Offset: 0x0010F370
		public static bool IsRebornEquipAttackOrDefense(int goodsID, out int Index)
		{
			Index = -1;
			int type = Global.GetGoodsCatetoriy(goodsID);
			bool result;
			if (type == 37 || type == 38 || type == 36 || type == 35)
			{
				Index = 0;
				result = true;
			}
			else if (type == 34 || type == 32 || type == 33 || type == 31 || type == 30)
			{
				Index = 1;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x001111E0 File Offset: 0x0010F3E0
		public static bool IsRebornType(int goodsID)
		{
			return RebornEquip.GetGoodsRebornEquip(goodsID) == 1;
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x00111208 File Offset: 0x0010F408
		public static bool OneIsCanIntoRebornOrBaseBag(GameClient client, GoodsData goodsData, out int BagInt)
		{
			BagInt = 0;
			bool result;
			if (Global.GetGoodsRebornEquip(goodsData.GoodsID) == 1)
			{
				if (!RebornEquip.CanAddGoodsToReborn(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, "1900-01-01 12:00:00", true, false))
				{
					BagInt = 1;
					result = false;
				}
				else
				{
					result = true;
				}
			}
			else if (!Global.CanAddGoods2(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, "1900-01-01 12:00:00", true))
			{
				BagInt = 2;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x00111290 File Offset: 0x0010F490
		public static bool MoreIsCanIntoRebornOrBaseBag(GameClient client, List<GoodsData> goodsData, out int BagInt)
		{
			BagInt = 0;
			int RebornBagCount = 0;
			foreach (GoodsData it in goodsData)
			{
				if (Global.GetGoodsRebornEquip(it.GoodsID) == 1)
				{
					RebornBagCount++;
					if (!RebornEquip.CanAddGoodsToReborn(client, it.GoodsID, it.GCount, it.Binding, "1900-01-01 12:00:00", true, false) || !RebornEquip.CanAddGoodsNum(client, RebornBagCount))
					{
						BagInt = 1;
						return false;
					}
				}
				else if (!Global.CanAddGoods2(client, it.GoodsID, it.GCount, it.Binding, "1900-01-01 12:00:00", true))
				{
					BagInt = 2;
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x0011137C File Offset: 0x0010F57C
		public static bool MoreIsCanIntoRebornOrBaseBagAward(GameClient client, List<AwardsItemData> goodsData, out int BagInt)
		{
			BagInt = 0;
			int BagCount = 0;
			int RebornBagCount = 0;
			foreach (AwardsItemData it in goodsData)
			{
				if (Global.GetGoodsRebornEquip(it.GoodsID) == 1)
				{
					RebornBagCount++;
					if (!RebornEquip.CanAddGoodsToReborn(client, it.GoodsID, it.GoodsNum, it.Binding, "1900-01-01 12:00:00", true, false) || !RebornEquip.CanAddGoodsNum(client, RebornBagCount))
					{
						BagInt = 1;
						return false;
					}
				}
				else
				{
					BagCount++;
					if (!Global.CanAddGoods2(client, it.GoodsID, it.GoodsNum, it.Binding, "1900-01-01 12:00:00", true) || !Global.CanAddGoodsNum(client, BagCount))
					{
						BagInt = 2;
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0011147C File Offset: 0x0010F67C
		public static bool MoreIsCanIntoRebornOrBaseBagAward(GameClient client, List<GoodsData> goodsData, out int BagInt)
		{
			BagInt = 0;
			int BagCount = 0;
			int RebornBagCount = 0;
			foreach (GoodsData it in goodsData)
			{
				if (Global.GetGoodsRebornEquip(it.GoodsID) == 1)
				{
					RebornBagCount++;
					if (!RebornEquip.CanAddGoodsToReborn(client, it.GoodsID, it.GCount, it.Binding, "1900-01-01 12:00:00", true, false) || !RebornEquip.CanAddGoodsNum(client, RebornBagCount))
					{
						BagInt = 1;
						return false;
					}
				}
				else
				{
					BagCount++;
					if (!Global.CanAddGoods2(client, it.GoodsID, it.GCount, it.Binding, "1900-01-01 12:00:00", true) || !Global.CanAddGoodsNum(client, BagCount))
					{
						BagInt = 2;
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0011157C File Offset: 0x0010F77C
		public static int GetGoodsRebornEquip(int goodsID)
		{
			SystemXmlItem systemGoods = null;
			int result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
			{
				result = 0;
			}
			else
			{
				result = systemGoods.GetIntValue("RebornEquip", -1);
			}
			return result;
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x001115B8 File Offset: 0x0010F7B8
		public static bool CanAddGoodsDataList(GameClient client, List<GoodsData> goodsDataList)
		{
			bool result;
			if (null == goodsDataList)
			{
				result = true;
			}
			else
			{
				int needGridNum = 0;
				foreach (GoodsData item in goodsDataList)
				{
					needGridNum += RebornEquip.CalGoodsGridNum(client, item.GoodsID, item.GCount, item.Binding, item.Endtime, true);
				}
				result = RebornEquip.CanAddGoodsNum(client, needGridNum);
			}
			return result;
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x00111648 File Offset: 0x0010F848
		public static bool CanAddGoodsDataList2(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int needGrid = RebornEquip.CalGoodsGridNum(client, goodsID, newGoodsNum, binding, endTime, canUseOld);
				int haveGoodsCount = RebornEquip.GetGoodsUsedGrid(client);
				int totalMaxGridCount = RebornEquip.GetSelfBagCapacity(client);
				result = (haveGoodsCount + needGrid <= totalMaxGridCount);
			}
			return result;
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0011169C File Offset: 0x0010F89C
		private static int CalGoodsGridNum(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			int gridNum = Global.GetGoodsGridNumByID(goodsID);
			gridNum = Global.GMax(gridNum, 1);
			int result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = (newGoodsNum - 1) / gridNum + 1;
			}
			else
			{
				int totalGridNum = 0;
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							totalGridNum++;
							if (canUseOld && gridNum > 1)
							{
								if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.RebornGoodsDataList[i].Endtime, endTime))
								{
									if (client.ClientData.RebornGoodsDataList[i].GCount < gridNum)
									{
										newGoodsNum -= Global.GMin(newGoodsNum, gridNum - client.ClientData.RebornGoodsDataList[i].GCount);
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

		// Token: 0x06001169 RID: 4457 RVA: 0x00111860 File Offset: 0x0010FA60
		public static int GetTotalGoodsCountByID(GameClient client, int goodsID)
		{
			int result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = 0;
			}
			else
			{
				int ret = 0;
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID)
						{
							if (!Global.IsGoodsTimeOver(client.ClientData.RebornGoodsDataList[i]) && !Global.IsGoodsNotReachStartTime(client.ClientData.RebornGoodsDataList[i]))
							{
								ret += client.ClientData.RebornGoodsDataList[i].GCount;
							}
						}
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x00111970 File Offset: 0x0010FB70
		public static bool CanAddGoodsNum(GameClient client, int newGoodsCount)
		{
			bool result;
			if (newGoodsCount <= 0)
			{
				result = false;
			}
			else
			{
				int haveGoodsCount = RebornEquip.GetGoodsUsedGrid(client);
				result = (newGoodsCount + haveGoodsCount <= client.ClientData.RebornBagNum);
			}
			return result;
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x001119AC File Offset: 0x0010FBAC
		public static int GetGoodsUsedGrid(GameClient client)
		{
			int ret = 0;
			int result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = ret;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							ret++;
						}
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x00111A68 File Offset: 0x0010FC68
		public static bool RemoveGoodsData(GameClient client, GoodsData gd)
		{
			bool result;
			if (null == gd)
			{
				result = false;
			}
			else if (client.ClientData.RebornGoodsDataList == null)
			{
				result = false;
			}
			else
			{
				bool ret = false;
				lock (client.ClientData.RebornGoodsDataList)
				{
					ret = client.ClientData.RebornGoodsDataList.Remove(gd);
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x00111AF8 File Offset: 0x0010FCF8
		public static bool RemoveGoodsDataToDb(GameClient client, GoodsData goodsData)
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
			return TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null);
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x00111BAC File Offset: 0x0010FDAC
		public static bool RemoveGoodsDataToDb(GameClient client, GoodsData goodsData, int Num)
		{
			bool result;
			if (goodsData.GCount < Num)
			{
				result = false;
			}
			else
			{
				string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					client.ClientData.RoleID,
					4,
					goodsData.Id,
					goodsData.GoodsID,
					0,
					goodsData.Site,
					Num,
					goodsData.BagIndex,
					""
				});
				result = (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null));
			}
			return result;
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x00111C74 File Offset: 0x0010FE74
		public static void AddPortableGoodsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 15001)
			{
				RebornEquip.UpdatePortableGoodsNum(client, 1);
				if (null == client.ClientData.RebornGoodsStoreList)
				{
					client.ClientData.RebornGoodsStoreList = new List<GoodsData>();
				}
				lock (client.ClientData.RebornGoodsStoreList)
				{
					client.ClientData.RebornGoodsStoreList.Add(goodsData);
				}
			}
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x00111D14 File Offset: 0x0010FF14
		public static void AddGoodsData(GameClient client, GoodsData gd)
		{
			if (null != gd)
			{
				if (client.ClientData.RebornGoodsDataList == null)
				{
					client.ClientData.RebornGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.RebornGoodsDataList)
				{
					client.ClientData.RebornGoodsDataList.Add(gd);
				}
			}
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00111DA8 File Offset: 0x0010FFA8
		public static int GetMaxMountCount()
		{
			return 240;
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x00111DC0 File Offset: 0x0010FFC0
		public static int GetIdleSlotOfRebornGoods(GameClient client)
		{
			int idelPos = 0;
			int result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = idelPos;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
				{
					if (client.ClientData.RebornGoodsDataList[i].Site == 15000 && client.ClientData.RebornGoodsDataList[i].Using <= 0)
					{
						if (usedBagIndex.IndexOf(client.ClientData.RebornGoodsDataList[i].BagIndex) < 0)
						{
							usedBagIndex.Add(client.ClientData.RebornGoodsDataList[i].BagIndex);
						}
					}
				}
				for (int j = 0; j < client.ClientData.RebornBagNum; j++)
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

		// Token: 0x06001173 RID: 4467 RVA: 0x00111EE4 File Offset: 0x001100E4
		public static int GetIdleSlotOfPortableGoods(GameClient client)
		{
			int idelPos = -1;
			int result;
			if (null == client.ClientData.RebornGoodsStoreList)
			{
				result = 0;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.RebornGoodsStoreList.Count; i++)
				{
					if (client.ClientData.RebornGoodsStoreList[i].Site == 15001 && client.ClientData.RebornGoodsStoreList[i].Using <= 0)
					{
						if (usedBagIndex.IndexOf(client.ClientData.RebornGoodsStoreList[i].BagIndex) < 0)
						{
							usedBagIndex.Add(client.ClientData.RebornGoodsStoreList[i].BagIndex);
						}
					}
				}
				for (int j = 0; j < RebornEquip.GetRebornStoreCapacity(client); j++)
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

		// Token: 0x06001174 RID: 4468 RVA: 0x00112003 File Offset: 0x00110203
		public static void UpdatePortableGoodsNum(GameClient client, int addNum)
		{
			client.ClientData.RebornGirdData.GoodsUsedGridNum += addNum;
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x00112020 File Offset: 0x00110220
		public static void RemovePortableGoodsData(GameClient client, GoodsData goodsData)
		{
			RebornEquip.UpdatePortableGoodsNum(client, -1);
			if (null != client.ClientData.RebornGoodsStoreList)
			{
				lock (client.ClientData.RebornGoodsStoreList)
				{
					client.ClientData.RebornGoodsStoreList.Remove(goodsData);
				}
			}
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x0011209C File Offset: 0x0011029C
		public static bool CanOpenPortableBag(GameClient client)
		{
			VIPDataInfo data;
			if (Data.VIPDataInfoList.TryGetValue(5001, out data))
			{
				if (client.ClientData.VipLevel < data.VIPlev && Global.GetTwoPointDistanceSquare(client.CurrentPos, client.ClientData.OpenPortableBagPoint) > 810000.0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x00112110 File Offset: 0x00110310
		public static bool CanPortableAddGoods(GameClient client)
		{
			return client.ClientData.RebornGirdData.GoodsUsedGridNum < client.ClientData.RebornGirdData.ExtGridNum;
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x00112144 File Offset: 0x00110344
		public static bool CanAddGoodsToReborn(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true, bool bLeftGrid = false)
		{
			bool result;
			if (client.ClientData.RebornGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int gridNum = Global.GetGoodsGridNumByID(goodsID);
				gridNum = Global.GMax(gridNum, 1);
				bool findOldGrid = false;
				int totalGridNum = 0;
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							totalGridNum++;
							if (canUseOld && gridNum > 1)
							{
								if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.RebornGoodsDataList[i].Endtime, endTime))
								{
									if (client.ClientData.RebornGoodsDataList[i].GCount + newGoodsNum <= gridNum)
									{
										findOldGrid = true;
										break;
									}
								}
							}
						}
					}
				}
				result = ((findOldGrid && !bLeftGrid) || totalGridNum < client.ClientData.RebornBagNum);
			}
			return result;
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x001122F0 File Offset: 0x001104F0
		public static GoodsData AddRebornGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
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
			if (null == client.ClientData.RebornGoodsDataList)
			{
				client.ClientData.RebornGoodsDataList = new List<GoodsData>();
			}
			lock (client.ClientData.RebornGoodsDataList)
			{
				client.ClientData.RebornGoodsDataList.Add(gd);
			}
			return gd;
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x0011242C File Offset: 0x0011062C
		public static List<int> GetRandomSuperior(List<int> bankListSource, int num)
		{
			List<int> ret = new List<int>();
			List<int> bankList = new List<int>();
			bankList.AddRange(bankListSource);
			List<int> result;
			if (num <= 0)
			{
				result = ret;
			}
			else
			{
				lock (RebornEquip.Mutex)
				{
					for (int i = 0; i < num; i++)
					{
						if (bankList.Count < 1)
						{
							break;
						}
						int random = Global.GetRandomNumber(0, bankList.Count);
						RebornSuperiorType typeItem;
						if (!RebornEquip.SuperiorType.TryGetValue(bankList[random], out typeItem))
						{
							break;
						}
						double random2 = Global.GetRandom();
						foreach (KeyValuePair<double, double> paramterRate in typeItem.Parameter)
						{
							if (random2 <= paramterRate.Value)
							{
								ret.Add(typeItem.Type);
								ret.Add(Convert.ToInt32(paramterRate.Key * 1000.0));
								break;
							}
						}
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x0011258C File Offset: 0x0011078C
		public static List<int> GetFixationSuperior(RebornSuperiorDrop superiorDrop)
		{
			List<int> ret = new List<int>();
			lock (ret)
			{
				foreach (int it in superiorDrop.RebornSuperiorInherentNum)
				{
					if (it < superiorDrop.RebornSuperiorInherentBank.Count)
					{
						RebornSuperiorType typeItem;
						if (RebornEquip.SuperiorType.TryGetValue(superiorDrop.RebornSuperiorInherentBank[it], out typeItem))
						{
							double random2 = Global.GetRandom();
							foreach (KeyValuePair<double, double> paramterRate in typeItem.Parameter)
							{
								if (random2 <= paramterRate.Value)
								{
									ret.Add(typeItem.Type);
									ret.Add(Convert.ToInt32(paramterRate.Key * 1000.0));
									break;
								}
							}
						}
					}
				}
			}
			return ret;
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00112714 File Offset: 0x00110914
		public static List<int> CalZhuoYueAttrByID(int code)
		{
			List<int> ret = new List<int>();
			try
			{
				RebornSuperiorDrop superiorDrop;
				if (!RebornEquip.SuperiorDrop.TryGetValue(code, out superiorDrop))
				{
					return ret;
				}
				int commonSuperiorNum = 0;
				int commonSuperiorNum2 = 0;
				int commonSuperiorNum3 = 0;
				lock (superiorDrop)
				{
					double superiorRandom = Global.GetRandom();
					foreach (KeyValuePair<double, int> commonSuperiorRate in superiorDrop.RebornSuperiorRate1)
					{
						if (superiorRandom < commonSuperiorRate.Key)
						{
							commonSuperiorNum = Convert.ToInt32(commonSuperiorRate.Value);
							break;
						}
					}
					double superiorRandom2 = Global.GetRandom();
					foreach (KeyValuePair<double, int> commonSuperiorRate in superiorDrop.RebornSuperiorRate2)
					{
						if (superiorRandom2 < commonSuperiorRate.Key)
						{
							commonSuperiorNum2 = Convert.ToInt32(commonSuperiorRate.Value);
							break;
						}
					}
					double superiorRandom3 = Global.GetRandom();
					foreach (KeyValuePair<double, int> commonSuperiorRate in superiorDrop.RebornSuperiorRate3)
					{
						if (superiorRandom3 < commonSuperiorRate.Key)
						{
							commonSuperiorNum3 = Convert.ToInt32(commonSuperiorRate.Value);
							break;
						}
					}
				}
				lock (ret)
				{
					ret.AddRange(RebornEquip.GetFixationSuperior(superiorDrop));
					ret.AddRange(RebornEquip.GetRandomSuperior(superiorDrop.RebornSuperiorBank1, commonSuperiorNum));
					ret.AddRange(RebornEquip.GetRandomSuperior(superiorDrop.RebornSuperiorBank2, commonSuperiorNum2));
					ret.AddRange(RebornEquip.GetRandomSuperior(superiorDrop.RebornSuperiorBank3, commonSuperiorNum3));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("重生装备 :: 根据卓越ID计算随机卓越属性，code={0}。", code), ex, true);
			}
			return ret;
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x001129D4 File Offset: 0x00110BD4
		public static GoodsData GetRebornGoodsByDbID(GameClient client, int dbID)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Id == dbID)
						{
							return client.ClientData.RebornGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x00112A98 File Offset: 0x00110C98
		public static List<GoodsData> GetRebornGoodsByDbIDDict(GameClient client, Dictionary<int, int> DBidList, out Dictionary<int, Dictionary<int, int>> IDNumMap)
		{
			IDNumMap = new Dictionary<int, Dictionary<int, int>>();
			List<GoodsData> resGoodList = new List<GoodsData>();
			List<GoodsData> result;
			if (null == DBidList)
			{
				result = null;
			}
			else if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						foreach (KeyValuePair<int, int> it in DBidList)
						{
							if (client.ClientData.RebornGoodsDataList[i].Id == it.Key && client.ClientData.RebornGoodsDataList[i].GCount >= it.Value)
							{
								Dictionary<int, int> dict = new Dictionary<int, int>();
								resGoodList.Add(client.ClientData.RebornGoodsDataList[i]);
								dict.Add(client.ClientData.RebornGoodsDataList[i].GoodsID, it.Value);
								IDNumMap.Add(i, dict);
							}
						}
					}
				}
				if (resGoodList.Count != DBidList.Count)
				{
					result = null;
				}
				else if (resGoodList.Count > 0)
				{
					result = resGoodList;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x00112C80 File Offset: 0x00110E80
		public static GoodsData GetRebornStoreGoodsByDbID(GameClient client, int dbID)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsStoreList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsStoreList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsStoreList.Count; i++)
					{
						if (client.ClientData.RebornGoodsStoreList[i].Id == dbID)
						{
							return client.ClientData.RebornGoodsStoreList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x00112D44 File Offset: 0x00110F44
		public static TCPProcessCmdResults SaleRebornEquipProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			int nTotalExp = 0;
			int totalYinLiangPrice = 0;
			int totalTongQianPrice = 0;
			int totalCuiLian = 0;
			int totalDuanZao = 0;
			int totalNiePan = 0;
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
						if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "重生装备回收", null))
						{
							if (client.ClientData.RebornCount > 0)
							{
								SystemXmlItem xmlItems = null;
								GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xmlItems);
								if (null != xmlItems)
								{
									int goodsPrice = Global.GetGoodsDataPrice(goodsData);
									if (goodsPrice < 0)
									{
										goodsPrice = 0;
									}
									if (RebornEquip.IsRebornEquip(goodsData.GoodsID))
									{
										if (goodsData.Props != null || goodsData.Props != "")
										{
											Dictionary<int, Dictionary<int, int>> HoleInfo = RebornStone.ParessMakeHoleInfo(goodsData.Props);
											if (HoleInfo != null || HoleInfo.Count != 0)
											{
												foreach (Dictionary<int, int> iter in HoleInfo.Values)
												{
													foreach (int it in iter.Values)
													{
														SystemXmlItem xml = null;
														if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(it, out xml))
														{
															break;
														}
														int Bind = 0;
														if (goodsData.Binding == 1)
														{
															Bind = 1;
														}
														Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, it, 1, 0, "", 0, Bind, 15000, "", true, 1, string.Format("装备回收宝石卸下", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
													}
												}
											}
										}
										if (xmlItem.GetIntValue("SuitID", -1) >= 2)
										{
											int NiePan = 0;
											int DuanZao = 0;
											int CuiLian = 0;
											if (RebornEquip.GetRebornEquipUseGoods(goodsData.GoodsID, out NiePan, out DuanZao, out CuiLian))
											{
												totalNiePan += NiePan;
												totalDuanZao += DuanZao;
												totalCuiLian += CuiLian;
											}
										}
										int nExp = xmlItems.GetIntValue("ChangeRebornExp", -1);
										if (nExp > 0)
										{
											nTotalExp += nExp;
										}
									}
									else if (goodsData.Binding > 0)
									{
										totalTongQianPrice += goodsPrice;
									}
									else
									{
										totalYinLiangPrice += goodsPrice;
									}
								}
							}
						}
					}
				}
				IL_3E5:
				i++;
				continue;
				goto IL_3E5;
			}
			if (nTotalExp > 0)
			{
				RebornManager.getInstance().ProcessRoleExperience(client, (long)nTotalExp, MoneyTypes.RebornExpSale, true, true, false, "none");
			}
			GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, totalYinLiangPrice, "重生背包出售材料", false);
			GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, totalTongQianPrice, "重生背包出售材料", true);
			RebornEquip.SetRebornEquipUseGoods(client, totalNiePan, totalDuanZao, totalCuiLian);
			if (totalYinLiangPrice > 0 && totalTongQianPrice > 0)
			{
				GameManager.LuaMgr.Hot(client, StringUtil.substitute(GLang.GetLang(655, new object[0]), new object[]
				{
					totalYinLiangPrice,
					totalTongQianPrice
				}), 0);
			}
			else if (totalYinLiangPrice > 0)
			{
				GameManager.LuaMgr.Hot(client, StringUtil.substitute(GLang.GetLang(656, new object[0]), new object[]
				{
					totalYinLiangPrice
				}), 0);
			}
			else if (totalTongQianPrice > 0)
			{
				GameManager.LuaMgr.Hot(client, StringUtil.substitute(GLang.GetLang(657, new object[0]), new object[]
				{
					totalTongQianPrice
				}), 0);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x001132EC File Offset: 0x001114EC
		public static TCPProcessCmdResults SaleStoreRebornEquipProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			return TCPProcessCmdResults.RESULT_OK;
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00113320 File Offset: 0x00111520
		public static void ResetBagAllGoods(GameClient client, bool notifyClient = true)
		{
			byte[] bytesCmd = null;
			if (client.ClientData.RebornGoodsDataList != null)
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					Dictionary<string, GoodsData> oldGoodsDict = new Dictionary<string, GoodsData>();
					List<GoodsData> toRemovedGoodsDataList = new List<GoodsData>();
					List<GoodsData> MoveGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							if (!RebornEquip.IsRebornType(client.ClientData.RebornGoodsDataList[i].GoodsID))
							{
								MoveGoodsDataList.Add(client.ClientData.RebornGoodsDataList[i]);
							}
							client.ClientData.RebornGoodsDataList[i].BagIndex = 0;
							int gridNum = Global.GetGoodsGridNumByID(client.ClientData.RebornGoodsDataList[i].GoodsID);
							if (gridNum > 1)
							{
								GoodsData oldGoodsData = null;
								string key = string.Format("{0}_{1}_{2}_{3}", new object[]
								{
									client.ClientData.RebornGoodsDataList[i].GoodsID,
									client.ClientData.RebornGoodsDataList[i].Binding,
									Global.DateTimeTicks(client.ClientData.RebornGoodsDataList[i].Starttime),
									Global.DateTimeTicks(client.ClientData.RebornGoodsDataList[i].Endtime)
								});
								if (oldGoodsDict.TryGetValue(key, out oldGoodsData))
								{
									int toAddNum = Global.GMin(gridNum - oldGoodsData.GCount, client.ClientData.RebornGoodsDataList[i].GCount);
									oldGoodsData.GCount += toAddNum;
									client.ClientData.RebornGoodsDataList[i].GCount -= toAddNum;
									client.ClientData.RebornGoodsDataList[i].BagIndex = 1;
									oldGoodsData.BagIndex = 1;
									if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsDataList[i]))
									{
										break;
									}
									EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, oldGoodsData.GoodsID, (long)oldGoodsData.Id, toAddNum, oldGoodsData.GCount, "整理背包");
									EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, client.ClientData.RebornGoodsDataList[i].GoodsID, (long)client.ClientData.RebornGoodsDataList[i].Id, -toAddNum, client.ClientData.RebornGoodsDataList[i].GCount, "整理背包");
									if (oldGoodsData.GCount >= gridNum)
									{
										if (client.ClientData.RebornGoodsDataList[i].GCount > 0)
										{
											oldGoodsDict[key] = client.ClientData.RebornGoodsDataList[i];
										}
										else
										{
											oldGoodsDict.Remove(key);
											toRemovedGoodsDataList.Add(client.ClientData.RebornGoodsDataList[i]);
										}
									}
									else if (client.ClientData.RebornGoodsDataList[i].GCount <= 0)
									{
										toRemovedGoodsDataList.Add(client.ClientData.RebornGoodsDataList[i]);
									}
								}
								else
								{
									oldGoodsDict[key] = client.ClientData.RebornGoodsDataList[i];
								}
							}
						}
					}
					for (int i = 0; i < toRemovedGoodsDataList.Count; i++)
					{
						client.ClientData.RebornGoodsDataList.Remove(toRemovedGoodsDataList[i]);
					}
					for (int i = 0; i < MoveGoodsDataList.Count; i++)
					{
						client.ClientData.RebornGoodsDataList.Remove(MoveGoodsDataList[i]);
						int BaseIndex = Global.GetIdleSlotOfBagGoods(client);
						MoveGoodsDataList[i].BagIndex = BaseIndex;
						MoveGoodsDataList[i].Site = 0;
						if (client.ClientData.GoodsDataList == null)
						{
							List<GoodsData> GoodsDataList = new List<GoodsData>();
							GoodsDataList.Add(MoveGoodsDataList[i]);
							client.ClientData.GoodsDataList = GoodsDataList;
						}
						else
						{
							client.ClientData.GoodsDataList.Add(MoveGoodsDataList[i]);
						}
						string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							3,
							MoveGoodsDataList[i].Id,
							MoveGoodsDataList[i].GoodsID,
							MoveGoodsDataList[i].Using,
							MoveGoodsDataList[i].Site,
							MoveGoodsDataList[i].GCount,
							MoveGoodsDataList[i].BagIndex,
							""
						});
						if (TCPProcessCmdResults.RESULT_OK != Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null))
						{
						}
					}
					client.ClientData.RebornGoodsDataList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
					int index = 0;
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using <= 0)
						{
							if (GameManager.Flag_OptimizationBagReset)
							{
								bool godosCountChanged = client.ClientData.RebornGoodsDataList[i].BagIndex > 0;
								client.ClientData.RebornGoodsDataList[i].BagIndex = index++;
								if (godosCountChanged)
								{
									if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsDataList[i]))
									{
										break;
									}
								}
							}
							else
							{
								client.ClientData.RebornGoodsDataList[i].BagIndex = index++;
								if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsDataList[i]))
								{
									break;
								}
							}
						}
					}
					bytesCmd = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.RebornGoodsDataList);
				}
				if (notifyClient)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytesCmd, 0, bytesCmd.Length, 2041);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x00113AB8 File Offset: 0x00111CB8
		public static void ResetStoreRebormGoods(GameClient client)
		{
			if (null != client.ClientData.RebornGoodsStoreList)
			{
				lock (client.ClientData.RebornGoodsStoreList)
				{
					Dictionary<string, GoodsData> oldGoodsDict = new Dictionary<string, GoodsData>();
					List<GoodsData> toRemovedGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.RebornGoodsStoreList.Count; i++)
					{
						if (client.ClientData.RebornGoodsStoreList[i].Using <= 0)
						{
							client.ClientData.RebornGoodsStoreList[i].BagIndex = 1;
							int gridNum = Global.GetGoodsGridNumByID(client.ClientData.RebornGoodsStoreList[i].GoodsID);
							if (gridNum > 1)
							{
								GoodsData oldGoodsData = null;
								string key = string.Format("{0}_{1}_{2}", client.ClientData.RebornGoodsStoreList[i].GoodsID, client.ClientData.RebornGoodsStoreList[i].Binding, Global.DateTimeTicks(client.ClientData.RebornGoodsStoreList[i].Endtime));
								if (oldGoodsDict.TryGetValue(key, out oldGoodsData))
								{
									int toAddNum = Global.GMin(gridNum - oldGoodsData.GCount, client.ClientData.RebornGoodsStoreList[i].GCount);
									oldGoodsData.GCount += toAddNum;
									client.ClientData.RebornGoodsStoreList[i].GCount -= toAddNum;
									client.ClientData.RebornGoodsStoreList[i].BagIndex = 1;
									oldGoodsData.BagIndex = 1;
									if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsStoreList[i]))
									{
										break;
									}
									if (oldGoodsData.GCount >= gridNum)
									{
										if (client.ClientData.RebornGoodsStoreList[i].GCount > 0)
										{
											oldGoodsDict[key] = client.ClientData.RebornGoodsStoreList[i];
										}
										else
										{
											oldGoodsDict.Remove(key);
											toRemovedGoodsDataList.Add(client.ClientData.RebornGoodsStoreList[i]);
										}
									}
									else if (client.ClientData.RebornGoodsStoreList[i].GCount <= 0)
									{
										toRemovedGoodsDataList.Add(client.ClientData.RebornGoodsStoreList[i]);
									}
								}
								else
								{
									oldGoodsDict[key] = client.ClientData.RebornGoodsStoreList[i];
								}
							}
						}
					}
					for (int i = 0; i < toRemovedGoodsDataList.Count; i++)
					{
						client.ClientData.RebornGoodsStoreList.Remove(toRemovedGoodsDataList[i]);
					}
					client.ClientData.RebornGoodsStoreList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
					int index = 0;
					for (int i = 0; i < client.ClientData.RebornGoodsStoreList.Count; i++)
					{
						if (client.ClientData.RebornGoodsStoreList[i].Using <= 0)
						{
							bool flag2 = 0 == 0;
							client.ClientData.RebornGoodsStoreList[i].BagIndex = index++;
							if (!RebornEquip.ResetRebornBagGoodsData(client, client.ClientData.RebornGoodsStoreList[i]))
							{
								break;
							}
						}
					}
				}
			}
			TCPOutPacket tcpOutPacket = null;
			if (null != client.ClientData.RebornGoodsStoreList)
			{
				lock (client.ClientData.RebornGoodsStoreList)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.RebornGoodsStoreList, Global._TCPManager.TcpOutPacketPool, 2042);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.RebornGoodsStoreList, Global._TCPManager.TcpOutPacketPool, 2042);
			}
			Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00113F6C File Offset: 0x0011216C
		public static int GetRebornStoreCapacity(GameClient client)
		{
			return client.ClientData.RebornGirdData.ExtGridNum;
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x00113F90 File Offset: 0x00112190
		public static int GetSelfBagCapacity(GameClient client)
		{
			return client.ClientData.RebornBagNum;
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x00113FB0 File Offset: 0x001121B0
		private static void GetNormalAndExNum(GameClient client, int addGridNum, out int normalNum, out int exNum)
		{
			normalNum = 0;
			exNum = 0;
			addGridNum = Global.GMax(0, addGridNum);
			int bagCapacity = RebornEquip.GetRebornStoreCapacity(client);
			if (bagCapacity >= Global.MaxPortableGridNum)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
				{
					Global.MaxPortableGridNum
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
			else
			{
				addGridNum = Global.GMin(addGridNum, Global.MaxPortableGridNum - bagCapacity);
				normalNum = ((bagCapacity + addGridNum < Global.ExtraBagGridPriceStartPos) ? addGridNum : Global.GMax(0, Global.ExtraBagGridPriceStartPos - bagCapacity - 1));
				exNum = Global.GMax(0, addGridNum - normalNum);
			}
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x0011406C File Offset: 0x0011226C
		public static int GetOneRebronBagExtendNeedYuanBaoForStorage(int extendPos)
		{
			int needYuanBao = (extendPos - Global.DefaultPortableGridNum) * Global.OnePortableGridYuanBao;
			if (needYuanBao > 10 * Global.OnePortableGridYuanBao)
			{
				needYuanBao = 10 * Global.OnePortableGridYuanBao;
			}
			return needYuanBao;
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x001140AC File Offset: 0x001122AC
		public static int GetExtRebornNeedYuanBaoForStorage(GameClient client, int addNum, int hasTime)
		{
			int bagCapacity = RebornEquip.GetRebornStoreCapacity(client);
			int extStartGrid = bagCapacity + 1;
			int extEndGrid = bagCapacity + addNum;
			int needYuanBao = 0;
			for (int pos = extStartGrid; pos <= extEndGrid; pos++)
			{
				needYuanBao += RebornEquip.GetOneRebronBagExtendNeedYuanBaoForStorage(pos);
				if (pos == extStartGrid)
				{
					double hasTimePercent = (double)hasTime / (double)((extStartGrid - Global.DefaultPortableGridNum) * 1500);
					needYuanBao = (int)((double)needYuanBao * Math.Max(0.0, 1.0 - hasTimePercent));
				}
			}
			return needYuanBao;
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x0011413C File Offset: 0x0011233C
		public static int GetOneBagGridExtendNeedYuanBao(int extendPos)
		{
			int needYuanBao = (extendPos - Global.DefaultBagGridNum) * Global.OneBagGridYuanBao;
			if (needYuanBao > 10 * Global.OneBagGridYuanBao)
			{
				needYuanBao = 10 * Global.OneBagGridYuanBao;
			}
			return needYuanBao;
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x0011417C File Offset: 0x0011237C
		public static int GetExtBagGridNeedYuanBao(GameClient client, int addNum, int hasTime)
		{
			int bagCapacity = RebornEquip.GetSelfBagCapacity(client);
			int extStartGrid = bagCapacity + 1;
			int extEndGrid = bagCapacity + addNum;
			int needYuanBao = 0;
			for (int pos = extStartGrid; pos <= extEndGrid; pos++)
			{
				needYuanBao += RebornEquip.GetOneBagGridExtendNeedYuanBao(pos);
				if (pos == extStartGrid)
				{
					double hasTimePercent = (double)hasTime / (double)((bagCapacity + 1 - Global.DefaultBagGridNum) * 3000);
					needYuanBao = (int)((double)needYuanBao * Math.Max(0.0, 1.0 - hasTimePercent));
				}
			}
			return needYuanBao;
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x0011420C File Offset: 0x0011240C
		public static int ExtRoleRebornBagNumWithYuanBao(TCPOutPacketPool pool, GameClient client, int addGridNum)
		{
			int bagCapacity = RebornEquip.GetSelfBagCapacity(client);
			int result;
			if (bagCapacity >= Global.MaxBagGridNum)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
				{
					Global.MaxBagGridNum
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = -1;
			}
			else
			{
				addGridNum = Global.Clamp(addGridNum, 0, Global.MaxBagGridNum - bagCapacity);
				if (addGridNum <= 0)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(159, new object[0]), new object[]
					{
						addGridNum
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = -2;
				}
				else
				{
					int[] YuanBaoBase = GameManager.systemParamsList.GetParamValueIntArrayByName("RebornBagGridParams", ',');
					if (YuanBaoBase == null || YuanBaoBase.Length != 4 || YuanBaoBase[1] == 0)
					{
						result = -2;
					}
					else
					{
						int Index = bagCapacity - Global.DefaultBagGridNum + 1;
						if (Index == 0)
						{
							result = -2;
						}
						else
						{
							int needYuanBao = 0;
							for (int i = 0; i < addGridNum; i++)
							{
								int temp = YuanBaoBase[1] * (Index + i);
								if (temp > YuanBaoBase[3])
								{
									temp = YuanBaoBase[3];
								}
								needYuanBao += temp;
							}
							if (needYuanBao > 0)
							{
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needYuanBao, "扩充重生背包", true, true, false, DaiBiSySType.None))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(161, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
									return -170;
								}
							}
							client.ClientData.RebornBagNum += addGridNum;
							GameManager.DBCmdMgr.AddDBCmd(14118, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornBagNum), null, client.ServerId);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x00114468 File Offset: 0x00112668
		public static int ExtRebornStoreWithYuanBao(TCPOutPacketPool pool, GameClient client, int addGridNum)
		{
			int bagCapacity = RebornEquip.GetRebornStoreCapacity(client);
			int result;
			if (bagCapacity >= Global.MaxPortableGridNum)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
				{
					Global.MaxPortableGridNum
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = -1;
			}
			else
			{
				addGridNum = Global.Clamp(addGridNum, 0, Global.MaxPortableGridNum - bagCapacity);
				if (addGridNum <= 0)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(159, new object[0]), new object[]
					{
						addGridNum
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = -2;
				}
				else
				{
					int[] YuanBaoBase = GameManager.systemParamsList.GetParamValueIntArrayByName("RebornBagGridParams", ',');
					if (YuanBaoBase == null || YuanBaoBase.Length != 4 || YuanBaoBase[0] == 0)
					{
						result = -2;
					}
					else
					{
						int Index = bagCapacity - Global.DefaultPortableGridNum + 1;
						if (Index == 0)
						{
							result = -2;
						}
						else
						{
							int needYuanBao = 0;
							for (int i = 0; i < addGridNum; i++)
							{
								int temp = YuanBaoBase[0] * (Index + i);
								if (temp > YuanBaoBase[2])
								{
									temp = YuanBaoBase[2];
								}
								needYuanBao += temp;
							}
							if (needYuanBao > 0)
							{
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needYuanBao, "扩充移动重生仓库", true, true, false, DaiBiSySType.None))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(160, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
									return -170;
								}
							}
							client.ClientData.RebornGirdData.ExtGridNum += addGridNum;
							GameManager.DBCmdMgr.AddDBCmd(14119, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornGirdData.ExtGridNum), null, client.ServerId);
							RebornEquip.NotifyRebornBagData(client);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x001146D4 File Offset: 0x001128D4
		public static bool ResetRebornBagGoodsData(GameClient client, GoodsData goodsData)
		{
			string[] dbFields = null;
			string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
			{
				client.ClientData.RoleID,
				goodsData.Id,
				goodsData.Using,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*",
				goodsData.GCount,
				"*",
				goodsData.BagIndex,
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
				Global.NoDBLogModRoleGoodsEvent(client, goodsData, 0, "重置重生背包索引", false);
				EventLogManager.AddGoodsEvent(client, OpTypes.Sort, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "重置重生背包索引");
				result = true;
			}
			return result;
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00114890 File Offset: 0x00112A90
		public static void NotifyRebornBagData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RebornPortableBagData>(client.ClientData.RebornGirdData, Global._TCPManager.TcpOutPacketPool, 2045);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x001148DC File Offset: 0x00112ADC
		public static void NotifyRebornShowEquipData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<int>(client.ClientData.RebornShowEquip, Global._TCPManager.TcpOutPacketPool, 2051);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00114928 File Offset: 0x00112B28
		public static TCPProcessCmdResults ProcessSpriteGetRebornGoodsListCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int site = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != roleID || site != 15000)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (null == client.ClientData.RebornGoodsDataList)
				{
					TCPProcessCmdResults result = Global.TransferRequestToDBServer(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 204, data, count, out tcpOutPacket, client.ServerId);
					if (TCPProcessCmdResults.RESULT_FAILED != result && null != tcpOutPacket)
					{
						List<GoodsData> goodsDataList = DataHelper.BytesToObject<List<GoodsData>>(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
						client.ClientData.RebornGoodsDataList = goodsDataList;
						Global.PushBackTcpOutPacket(tcpOutPacket);
					}
					if (null == client.ClientData.RebornGoodsDataList)
					{
						client.ClientData.RebornGoodsDataList = new List<GoodsData>();
					}
				}
				byte[] bytesData = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.RebornGoodsDataList);
				GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				Global.RefreshEquipProp(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(tcpMgr.MySocketListener, pool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(tcpMgr.MySocketListener, pool, client, true, false, 7);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x00114BA8 File Offset: 0x00112DA8
		public static TCPProcessCmdResults ProcessSpriteRebornResetBagCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (client == null || client.ClientData.RoleID != roleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (SingletonTemplate<CreateRoleLimitManager>.Instance().ResetBagSlotTicks > 0 && TimeUtil.NOW() - client.ClientData._ResetBagTicks < (long)SingletonTemplate<CreateRoleLimitManager>.Instance().ResetBagSlotTicks)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(129, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return TCPProcessCmdResults.RESULT_OK;
				}
				client.ClientData._ResetBagTicks = TimeUtil.NOW();
				RebornEquip.ResetBagAllGoods(client, true);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00114DA0 File Offset: 0x00112FA0
		public static GoodsData GetGoodsByID(GameClient client, int goodsID)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID)
						{
							return client.ClientData.RebornGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x00114E64 File Offset: 0x00113064
		public static GoodsData GetBindingGoodsByID(GameClient client, int goodsID, int Binding)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				GoodsData gd = null;
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == Binding)
						{
							if (gd == null)
							{
								gd = client.ClientData.RebornGoodsDataList[i];
							}
							else
							{
								gd.GCount += client.ClientData.RebornGoodsDataList[i].GCount;
							}
						}
					}
				}
				result = gd;
			}
			return result;
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x00114F8C File Offset: 0x0011318C
		public static List<GoodsData> GetBindingNotCountGoodsByID(GameClient client, int goodsID, int Binding)
		{
			List<GoodsData> result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				List<GoodsData> gd = new List<GoodsData>();
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == Binding)
						{
							gd.Add(client.ClientData.RebornGoodsDataList[i]);
						}
					}
				}
				result = gd;
			}
			return result;
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00115080 File Offset: 0x00113280
		public static GoodsData GetGoodsByID(GameClient client, int goodsID, int bingding, string endTime, ref int startIndex)
		{
			GoodsData result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					if (startIndex >= client.ClientData.RebornGoodsDataList.Count)
					{
						return null;
					}
					for (int i = startIndex; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].GoodsID == goodsID && client.ClientData.RebornGoodsDataList[i].Binding == bingding && Global.DateTimeEqual(client.ClientData.RebornGoodsDataList[i].Endtime, endTime))
						{
							startIndex = i + 1;
							return client.ClientData.RebornGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x001151A8 File Offset: 0x001133A8
		public static TCPProcessCmdResults ProcessSpriteResetRebornStoreCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (client == null || client.ClientData.RoleID != roleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				RebornEquip.ResetStoreRebormGoods(client);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x001152F8 File Offset: 0x001134F8
		public static TCPProcessCmdResults ProcessExtRebornStoreByYuanBaoCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addGridNum = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != roleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int bagCapacity = RebornEquip.GetRebornStoreCapacity(client);
				if (bagCapacity >= Global.MaxPortableGridNum)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
					{
						Global.MaxPortableGridNum
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return TCPProcessCmdResults.RESULT_OK;
				}
				RebornEquip.ExtRebornStoreWithYuanBao(pool, client, addGridNum);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x001154D8 File Offset: 0x001136D8
		public static TCPProcessCmdResults ProcessExtRebornBagNumByYuanBaoCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addGridNum = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != roleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int bagCapacity = RebornEquip.GetSelfBagCapacity(client);
				if (bagCapacity >= Global.MaxBagGridNum)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(156, new object[0]), new object[]
					{
						Global.MaxBagGridNum
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int result = RebornEquip.ExtRoleRebornBagNumWithYuanBao(pool, client, addGridNum);
				string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.RebornBagNum, result);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strCmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x00115704 File Offset: 0x00113904
		public static TCPProcessCmdResults ProcessQueryRebornOpenGridCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 1 && fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleAID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != roleAID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleAID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string strcmd = string.Format("{0}", client.ClientData.OpenRebornBagTime);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x00115888 File Offset: 0x00113A88
		public static TCPProcessCmdResults ProcessQueryOpenRebornPortableGridCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int roleAID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != roleAID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleAID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string strcmd = string.Format("{0}", client.ClientData.OpenRebornGridTime);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00115A04 File Offset: 0x00113C04
		public static RebornEquipOpcode RebornEquipShowProcess(GameClient client, int nRoleID)
		{
			RebornEquipOpcode result;
			if (client.ClientData.RoleID != nRoleID)
			{
				result = RebornEquipOpcode.RebornShowErr;
			}
			else
			{
				if (client.ClientData.RebornShowEquip == 0)
				{
					client.ClientData.RebornShowEquip = 1;
				}
				else
				{
					client.ClientData.RebornShowEquip = 0;
				}
				GameManager.DBCmdMgr.AddDBCmd(14120, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornShowEquip), null, client.ServerId);
				GameManager.ClientMgr.NotifyOthersRebornEquipChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = RebornEquipOpcode.RebornShowSucc;
			}
			return result;
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x00115ACC File Offset: 0x00113CCC
		public static RebornEquipOpcode RebornEquipShowModelProcess(GameClient client, int nRoleID)
		{
			RebornEquipOpcode result;
			if (client.ClientData.RoleID != nRoleID)
			{
				result = RebornEquipOpcode.RebornShowErr;
			}
			else
			{
				if (client.ClientData.RebornShowModel == 0)
				{
					client.ClientData.RebornShowModel = 1;
				}
				else
				{
					client.ClientData.RebornShowModel = 0;
				}
				GameManager.DBCmdMgr.AddDBCmd(14122, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornShowModel), null, client.ServerId);
				GameManager.ClientMgr.NotifyOthersRebornModelChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = RebornEquipOpcode.RebornShowSucc;
			}
			return result;
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00115B94 File Offset: 0x00113D94
		public static void NotifyRebornEquipUp(GameClient client, GoodsData gd)
		{
			GameManager.ClientMgr.NotifySelfAddGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, gd.Id, gd.GoodsID, gd.Forge_level, gd.Quality, gd.GCount, gd.Binding, gd.Site, gd.Jewellist, 0, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, gd.BagIndex, gd.WashProps, null, 0, "");
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00115C38 File Offset: 0x00113E38
		public static List<GoodsData> GetRebornUsingAttackWeaponGoods(GameClient client)
		{
			List<GoodsData> result;
			if (null == client.ClientData.RebornGoodsDataList)
			{
				result = null;
			}
			else
			{
				List<GoodsData> lGood = new List<GoodsData>();
				lock (client.ClientData.RebornGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using > 0)
						{
							int goodsCatetoriy = Global.GetGoodsCatetoriy(client.ClientData.RebornGoodsDataList[i].GoodsID);
							if (38 == goodsCatetoriy || 37 == goodsCatetoriy)
							{
								lGood.Add(client.ClientData.RebornGoodsDataList[i]);
							}
						}
					}
				}
				if (lGood.Count<GoodsData>() > 0)
				{
					result = lGood;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00115D60 File Offset: 0x00113F60
		public static GoodsData GetMagicWeaponGoods(List<GoodsData> BaseWeap, bool IsBase)
		{
			GoodsData goods = null;
			GoodsData result;
			if (BaseWeap == null || BaseWeap.Count <= 0)
			{
				result = null;
			}
			else
			{
				if (BaseWeap.Count == 1)
				{
					goods = BaseWeap[0];
				}
				else if (BaseWeap.Count > 1)
				{
					for (int i = 0; i < BaseWeap.Count; i++)
					{
						if (BaseWeap[i].BagIndex == 0 && IsBase)
						{
							goods = BaseWeap[i];
							break;
						}
						if (!IsBase && (RebornEquip.IsRebornEquipShengQi(BaseWeap[i].GoodsID) || RebornEquip.IsRebornEquipShengWu(BaseWeap[i].GoodsID)))
						{
							goods = BaseWeap[i];
							break;
						}
					}
					if (null == goods)
					{
						for (int i = 0; i < BaseWeap.Count; i++)
						{
							if (BaseWeap[i].BagIndex == 1 && IsBase)
							{
								goods = BaseWeap[i];
								break;
							}
							if (!IsBase && (RebornEquip.IsRebornEquipShengQi(BaseWeap[i].GoodsID) || RebornEquip.IsRebornEquipShengWu(BaseWeap[i].GoodsID)))
							{
								goods = BaseWeap[i];
								break;
							}
						}
					}
				}
				if (null == goods)
				{
					result = null;
				}
				else
				{
					result = goods;
				}
			}
			return result;
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00115EE0 File Offset: 0x001140E0
		public static bool IsWeaponCanAttackOrActtion(GameClient client, out int Code)
		{
			Code = 0;
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			List<GoodsData> bGoods = Global.GetUsingAttackWeaponGoods(client);
			List<GoodsData> rGoods = RebornEquip.GetRebornUsingAttackWeaponGoods(client);
			bool result;
			if (SceneUIClasses.ChongShengMap != sceneType && client.ClientData.RebornShowEquip == 0 && (bGoods == null || bGoods.Count<GoodsData>() < 0))
			{
				Code = 1;
				result = false;
			}
			else if (SceneUIClasses.ChongShengMap != sceneType && client.ClientData.RebornShowEquip == 1 && (rGoods == null || rGoods.Count<GoodsData>() < 0))
			{
				Code = 2;
				result = false;
			}
			else if (SceneUIClasses.ChongShengMap == sceneType && (rGoods == null || rGoods.Count<GoodsData>() < 0))
			{
				Code = 3;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x00115FB4 File Offset: 0x001141B4
		public static RebornEquipOpcode RebornEquipAdvanceProcess(GameClient client, int nRoleID, int DBID)
		{
			GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, DBID);
			RebornEquipOpcode result;
			RebornEquipEvolution Item;
			if (goodsData == null)
			{
				result = RebornEquipOpcode.NotExistRebornEquip;
			}
			else if (goodsData.Site != 15000)
			{
				result = RebornEquipOpcode.NotInRebornBag;
			}
			else if (!RebornEquip.RebornEquipUp.TryGetValue(goodsData.GoodsID, out Item))
			{
				result = RebornEquipOpcode.NotFindRebornLow;
			}
			else
			{
				int RebornNiePan = Global.GetRoleParamsInt32FromDB(client, "10251");
				int RebornDuanZao = Global.GetRoleParamsInt32FromDB(client, "10250");
				int RebornCuiLian = Global.GetRoleParamsInt32FromDB(client, "10249");
				lock (Item)
				{
					if (RebornNiePan < Item.NeedNiePan)
					{
						return RebornEquipOpcode.NeedNiePanNotEnough;
					}
					if (RebornDuanZao < Item.NeedDuanZao)
					{
						return RebornEquipOpcode.NeedDuanZaoNotEnough;
					}
					if (RebornCuiLian < Item.NeedCuiLian)
					{
						return RebornEquipOpcode.NeedCuiLianNotEnough;
					}
					SystemXmlItem systemGoods = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(Item.NeedEquitID, out systemGoods))
					{
						return RebornEquipOpcode.NotHaveUpEquip;
					}
					if (!GameManager.ClientMgr.ModifyRebornNiePanPointValue(client, -Item.NeedNiePan, "装备进阶消耗涅槃点", true, true, false))
					{
						return RebornEquipOpcode.NeedNiePanErr;
					}
					if (!GameManager.ClientMgr.ModifyRebornDuanZaoPointValue(client, -Item.NeedDuanZao, "装备进阶消耗锻造点", true, true, false))
					{
						return RebornEquipOpcode.NeedDuanZaoErr;
					}
					if (!GameManager.ClientMgr.ModifyRebornCuiLianPointValue(client, -Item.NeedCuiLian, "装备进阶消耗淬炼点", true, true, false))
					{
						return RebornEquipOpcode.NeedCuiLianErr;
					}
					double rand = Global.GetRandom();
					if (rand > Item.SuccessRate)
					{
						return RebornEquipOpcode.NotEnoughProb;
					}
					int NewDBID = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, Item.NewEquitID, 1, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 15000, goodsData.Jewellist, true, 1, string.Format("重生装备晋升", new object[0]), false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, goodsData.WashProps, goodsData.ElementhrtsProps, goodsData.Starttime, goodsData.JuHunID, true);
					if (NewDBID == DBID)
					{
						return RebornEquipOpcode.RebornNewEquipErr;
					}
					string prop = goodsData.Props;
					if (!RebornEquip.RemoveGoodsDataToDb(client, goodsData))
					{
						return RebornEquipOpcode.RebornLowError;
					}
					GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 4, goodsData.Id, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, 1);
					GoodsData NewGd = RebornEquip.GetRebornGoodsByDbID(client, NewDBID);
					NewGd.Props = prop;
					if (NewGd.Using > 0)
					{
						Global.RefreshEquipProp(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
				}
				result = RebornEquipOpcode.RebornUpSucc;
			}
			return result;
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x0011630C File Offset: 0x0011450C
		public static int GetGoodDataCategoriyByRebornPerfusion(int Item, int JieZhiHand = 0)
		{
			int result;
			switch (Item)
			{
			case 30:
				result = 1;
				break;
			case 31:
				result = 2;
				break;
			case 32:
				result = 3;
				break;
			case 33:
				result = 4;
				break;
			case 34:
				result = 5;
				break;
			case 35:
				result = 8;
				break;
			case 36:
				if (JieZhiHand == 0)
				{
					result = 6;
				}
				else
				{
					result = 7;
				}
				break;
			case 37:
				result = 9;
				break;
			case 38:
				result = 10;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x00116388 File Offset: 0x00114588
		public static long GetFreeTime()
		{
			return GameManager.systemParamsList.GetParamValueIntByName("EquipQuenchingFreeNum", 0);
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x001163AC File Offset: 0x001145AC
		public static int UseGoodAblePerfusion(GameClient client, int HoleSite, int Level, out int Able, out bool isFree)
		{
			Able = 0;
			isFree = false;
			int result;
			if (!RebornEquip.RebornEquipHole.ContainsKey(HoleSite))
			{
				result = -1;
			}
			else if (!RebornEquip.RebornEquipHole[HoleSite].ContainsKey(Level))
			{
				result = -2;
			}
			else
			{
				int Start = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][Level].AddStart * 100.0);
				int End = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][Level].AddEnd * 100.0);
				if (Start < 0 || End < 0)
				{
					result = -3;
				}
				else
				{
					if ((long)Global.GetRoleParamsInt32FromDB(client, "10255") < RebornEquip.GetFreeTime())
					{
						isFree = true;
					}
					else
					{
						Dictionary<int, Dictionary<int, GoodsData>> dict = new Dictionary<int, Dictionary<int, GoodsData>>();
						lock (dict)
						{
							int Count = 0;
							foreach (KeyValuePair<int, int> it in RebornEquip.RebornEquipHole[HoleSite][Level].UseGoods)
							{
								SystemXmlItem systemGoods = null;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(it.Key, out systemGoods))
								{
									return -4;
								}
								GoodsData good = RebornEquip.GetGoodsByID(client, it.Key);
								if (good == null)
								{
									return -5;
								}
								Dictionary<int, GoodsData> gdict = new Dictionary<int, GoodsData>();
								gdict.Add(it.Value, good);
								Count++;
								dict.Add(Count, gdict);
							}
							foreach (Dictionary<int, GoodsData> iter in dict.Values)
							{
								foreach (KeyValuePair<int, GoodsData> it2 in iter)
								{
									bool bind;
									if (!RebornStone.RebornUseGoodHasBinding(client, it2.Value.GoodsID, it2.Key, 1, out bind))
									{
										return -6;
									}
								}
							}
						}
					}
					Able = Global.GetRandomNumber(Start, End);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x00116698 File Offset: 0x00114898
		public static RebornEquipData GetNewInfo(GameClient client, int HoleSite)
		{
			return new RebornEquipData
			{
				RoleID = client.ClientData.RebornEquipHoleInfo[HoleSite].RoleID,
				HoleID = client.ClientData.RebornEquipHoleInfo[HoleSite].HoleID,
				Able = client.ClientData.RebornEquipHoleInfo[HoleSite].Able,
				Level = client.ClientData.RebornEquipHoleInfo[HoleSite].Level
			};
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x00116724 File Offset: 0x00114924
		public static void RefreshProps(GameClient client)
		{
			if (client.ClientData.RebornEquipHoleInfo != null && client.ClientData.RebornEquipHoleInfo.Count != 0)
			{
				double[] _ExtProps = new double[177];
				try
				{
					List<int> holeIDList = new List<int>();
					foreach (GoodsData gd in client.ClientData.RebornGoodsDataList)
					{
						if (gd.Using != 0)
						{
							SystemXmlItem systemGoods = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(gd.GoodsID, out systemGoods))
							{
								int Categoriy = systemGoods.GetIntValue("Categoriy", -1);
								if (Categoriy >= 30 && Categoriy <= 38)
								{
									if (Categoriy == 36)
									{
										holeIDList.Add(RebornEquip.GetGoodDataCategoriyByRebornPerfusion(Categoriy, gd.BagIndex));
									}
									else
									{
										holeIDList.Add(RebornEquip.GetGoodDataCategoriyByRebornPerfusion(Categoriy, 0));
									}
								}
							}
						}
					}
					if (holeIDList != null && holeIDList.Count != 0)
					{
						foreach (KeyValuePair<int, RebornEquipData> it in client.ClientData.RebornEquipHoleInfo)
						{
							if (it.Value.Level != 0)
							{
								if (holeIDList.IndexOf(it.Value.HoleID) != -1)
								{
									if (RebornEquip.RebornEquipHole.ContainsKey(it.Value.HoleID))
									{
										if (RebornEquip.RebornEquipHole[it.Value.HoleID].ContainsKey(it.Value.Level))
										{
											foreach (KeyValuePair<int, double> iter in RebornEquip.RebornEquipHole[it.Value.HoleID][it.Value.Level].Attr)
											{
												_ExtProps[iter.Key] += iter.Value;
											}
										}
									}
								}
							}
						}
					}
				}
				finally
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.RebornEquipHole,
						_ExtProps
					});
				}
			}
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x00116A60 File Offset: 0x00114C60
		public static RebornPerfusionOpcode RebornEquipHolePerfusionProcess(GameClient client, int HoleSite, out int ClientAble)
		{
			ClientAble = 0;
			RebornPerfusionOpcode result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.RebornEquipHole, false))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("灌注功能未开启, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
				result = RebornPerfusionOpcode.NotStart;
			}
			else if (!RebornEquip.RebornEquipHole.ContainsKey(HoleSite))
			{
				result = RebornPerfusionOpcode.NotExsit;
			}
			else
			{
				if (client.ClientData.RebornEquipHoleInfo == null || !client.ClientData.RebornEquipHoleInfo.ContainsKey(HoleSite))
				{
					if (client.ClientData.RebornEquipHoleInfo == null)
					{
						client.ClientData.RebornEquipHoleInfo = new Dictionary<int, RebornEquipData>();
					}
					int Able = 0;
					bool isFree = false;
					if (0 != RebornEquip.UseGoodAblePerfusion(client, HoleSite, 0, out Able, out isFree))
					{
						return RebornPerfusionOpcode.MakeAbleErr;
					}
					if (isFree)
					{
						if (!GameManager.ClientMgr.ModifyRebornEquipHoleValue(client, 1, "重生装备槽灌注免费次数减少", true, true, false))
						{
							return RebornPerfusionOpcode.PerfusionNumErr;
						}
						client.ClientData.RebornEquipHole += 1L;
					}
					RebornEquipData newData = new RebornEquipData();
					newData.RoleID = client.ClientData.RoleID;
					newData.HoleID = HoleSite;
					newData.Level = 0;
					newData.Able = Able;
					int ret = Global.sendToDB<int, RebornEquipData>(14123, newData, client.ServerId);
					if (ret < 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("灌注插入数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
						return RebornPerfusionOpcode.InsertDataErr;
					}
					client.ClientData.RebornEquipHoleInfo.Add(newData.HoleID, newData);
					ClientAble = client.ClientData.RebornEquipHoleInfo[newData.HoleID].Able;
				}
				else
				{
					if (client.ClientData.RebornEquipHoleInfo[HoleSite].RoleID != client.ClientData.RoleID)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("灌注ID校验出错, 玩家id RoleID={0}, 灌注结构 RoleID={1} 灌注部位 HoleID={2}", client.ClientData.RoleID, client.ClientData.RebornEquipHoleInfo[HoleSite].RoleID, client.ClientData.RebornEquipHoleInfo[HoleSite].HoleID), null, true);
						return RebornPerfusionOpcode.PerfusionInfoErr;
					}
					RebornEquipData newData = RebornEquip.GetNewInfo(client, HoleSite);
					if (newData == null)
					{
						return RebornPerfusionOpcode.InsertDataErr;
					}
					if (newData.Able >= 100)
					{
						return RebornPerfusionOpcode.MaxAble;
					}
					int Able = 0;
					bool isFree = false;
					if (0 != RebornEquip.UseGoodAblePerfusion(client, HoleSite, client.ClientData.RebornEquipHoleInfo[HoleSite].Level, out Able, out isFree))
					{
						return RebornPerfusionOpcode.MakeAbleErr;
					}
					newData.Able += Able;
					if (newData.Able > 100)
					{
						newData.Able = 100;
					}
					if (isFree)
					{
						if (!GameManager.ClientMgr.ModifyRebornEquipHoleValue(client, 1, "重生装备槽灌注免费次数减少", true, true, false))
						{
							return RebornPerfusionOpcode.PerfusionNumErr;
						}
						client.ClientData.RebornEquipHole += 1L;
					}
					int ret = Global.sendToDB<int, RebornEquipData>(14124, newData, client.ServerId);
					if (ret <= 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("灌注更新数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
						return RebornPerfusionOpcode.UpdateDataErr;
					}
					client.ClientData.RebornEquipHoleInfo[newData.HoleID].Able = newData.Able;
					ClientAble = client.ClientData.RebornEquipHoleInfo[newData.HoleID].Able;
				}
				result = RebornPerfusionOpcode.Succ;
			}
			return result;
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x00116E30 File Offset: 0x00115030
		public static RebornPerfusionOpcode RebornEquipHoleAbschreckenProcess(GameClient client, int HoleSite, out int ClientLevel, out int ClientAble)
		{
			ClientLevel = 0;
			ClientAble = 0;
			RebornPerfusionOpcode result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.RebornEquipHole, false))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("灌注功能未开启, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
				result = RebornPerfusionOpcode.NotStart;
			}
			else if (client.ClientData.RebornEquipHoleInfo == null || !client.ClientData.RebornEquipHoleInfo.ContainsKey(HoleSite) || RebornEquip.RebornEquipHole == null || !RebornEquip.RebornEquipHole.ContainsKey(HoleSite) || !RebornEquip.RebornEquipHole[HoleSite].ContainsKey(client.ClientData.RebornEquipHoleInfo[HoleSite].Level))
			{
				result = RebornPerfusionOpcode.NotExsit;
			}
			else if (!RebornEquip.RebornEquipHoleLevelMap.ContainsKey(HoleSite))
			{
				result = RebornPerfusionOpcode.NotExsit;
			}
			else if (client.ClientData.RebornEquipHoleInfo[HoleSite].Level >= RebornEquip.RebornEquipHoleLevelMap[HoleSite])
			{
				result = RebornPerfusionOpcode.MaxLevel;
			}
			else
			{
				int SysAble = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][client.ClientData.RebornEquipHoleInfo[HoleSite].Level].AbschreckenUnterwerfen * 100.0);
				if (SysAble < 0 || client.ClientData.RebornEquipHoleInfo[HoleSite].Able < SysAble)
				{
					result = RebornPerfusionOpcode.SuccNotVoll;
				}
				else
				{
					RebornEquipData newData = RebornEquip.GetNewInfo(client, HoleSite);
					if (newData == null)
					{
						result = RebornPerfusionOpcode.InsertDataErr;
					}
					else
					{
						int UpAble = Global.GetRandomNumber(1, 101);
						if (UpAble > client.ClientData.RebornEquipHoleInfo[HoleSite].Able)
						{
							int SubStart = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][client.ClientData.RebornEquipHoleInfo[HoleSite].Level].SubStart * 100.0);
							int SubEnd = Convert.ToInt32(RebornEquip.RebornEquipHole[HoleSite][client.ClientData.RebornEquipHoleInfo[HoleSite].Level].SubEnd * 100.0);
							if (SubStart < 0 || SubEnd < 0)
							{
								result = RebornPerfusionOpcode.AbschreckenXmlErr;
							}
							else
							{
								int Rand = Global.GetRandomNumber(SubStart, SubEnd);
								if (Rand < 0)
								{
									result = RebornPerfusionOpcode.AbschreckenXmlErr;
								}
								else
								{
									if (newData.Able == 0 || newData.Able <= Rand)
									{
										newData.Able = 0;
									}
									else
									{
										newData.Able -= Rand;
									}
									int ret = Global.sendToDB<int, RebornEquipData>(14124, newData, client.ServerId);
									if (ret <= 0)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("淬炼失败更新数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
										result = RebornPerfusionOpcode.UpdateDataErr;
									}
									else
									{
										client.ClientData.RebornEquipHoleInfo[HoleSite].Able = newData.Able;
										ClientAble = client.ClientData.RebornEquipHoleInfo[HoleSite].Able;
										ClientLevel = client.ClientData.RebornEquipHoleInfo[HoleSite].Level;
										result = RebornPerfusionOpcode.AbschreckenFail;
									}
								}
							}
						}
						else
						{
							double UpBangBangLevel = Global.GetRandom();
							if (UpBangBangLevel <= RebornEquip.RebornEquipHole[HoleSite][newData.Level].SturmGaiLv)
							{
								newData.Level += RebornEquip.RebornEquipHole[HoleSite][newData.Level].SturmLevel;
								if (newData.Level > RebornEquip.RebornEquipHoleLevelMap[HoleSite])
								{
									newData.Level = RebornEquip.RebornEquipHoleLevelMap[HoleSite];
								}
							}
							else
							{
								newData.Level++;
							}
							newData.Able = 0;
							int ret = Global.sendToDB<int, RebornEquipData>(14124, newData, client.ServerId);
							if (ret <= 0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("淬炼成功更新数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
								result = RebornPerfusionOpcode.UpdateDataErr;
							}
							else
							{
								client.ClientData.RebornEquipHoleInfo[HoleSite].Level = newData.Level;
								client.ClientData.RebornEquipHoleInfo[HoleSite].Able = 0;
								ClientAble = client.ClientData.RebornEquipHoleInfo[HoleSite].Able;
								ClientLevel = client.ClientData.RebornEquipHoleInfo[HoleSite].Level;
								Global.RefreshEquipProp(client);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								result = RebornPerfusionOpcode.Succ;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x00117308 File Offset: 0x00115508
		public static double GetRebornEquipRate()
		{
			return Math.Min(GameManager.systemParamsList.GetParamValueDoubleByName("ChongShengReturnNum", 0.0), 1.0);
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x00117340 File Offset: 0x00115540
		public static bool GetRebornEquipUseGoods(int goodsId, out int NiePan, out int DuanZao, out int CuiLian)
		{
			NiePan = 0;
			DuanZao = 0;
			CuiLian = 0;
			bool result;
			if (RebornEquip.Evolution == null)
			{
				result = false;
			}
			else
			{
				foreach (KeyValuePair<int, RebornEquipEvolution> it in RebornEquip.Evolution)
				{
					if (it.Value.NewEquitID == goodsId)
					{
						double rate = RebornEquip.GetRebornEquipRate();
						NiePan = (int)Math.Floor((double)it.Value.NeedNiePan * rate);
						DuanZao = (int)Math.Floor((double)it.Value.NeedDuanZao * rate);
						CuiLian = (int)Math.Floor((double)it.Value.NeedCuiLian * rate);
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x0011741C File Offset: 0x0011561C
		public static bool SetRebornEquipUseGoods(GameClient client, int NiePan, int DuanZao, int Cuiian)
		{
			if (NiePan > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornNiePanPointValue(client, NiePan, "装备回收获得涅槃点", true, true, false))
				{
					return false;
				}
			}
			if (DuanZao > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornDuanZaoPointValue(client, DuanZao, "装备回收获得锻造点", true, true, false))
				{
					return false;
				}
			}
			if (Cuiian > 0)
			{
				if (!GameManager.ClientMgr.ModifyRebornCuiLianPointValue(client, Cuiian, "装备回收获得淬炼点", true, true, false))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04001AE1 RID: 6881
		public static Dictionary<int, RebornEquipEvolution> Evolution = new Dictionary<int, RebornEquipEvolution>();

		// Token: 0x04001AE2 RID: 6882
		public static Dictionary<int, RebornSuperiorDrop> SuperiorDrop = new Dictionary<int, RebornSuperiorDrop>();

		// Token: 0x04001AE3 RID: 6883
		public static Dictionary<int, RebornSuperiorType> SuperiorType = new Dictionary<int, RebornSuperiorType>();

		// Token: 0x04001AE4 RID: 6884
		public static Dictionary<int, RebornEquipXmlStruct> EquipSQSW = new Dictionary<int, RebornEquipXmlStruct>();

		// Token: 0x04001AE5 RID: 6885
		public static Dictionary<int, RebornEquipEvolution> RebornEquipUp = new Dictionary<int, RebornEquipEvolution>();

		// Token: 0x04001AE6 RID: 6886
		public static Dictionary<int, Dictionary<int, RebornQuenching>> RebornEquipHole = new Dictionary<int, Dictionary<int, RebornQuenching>>();

		// Token: 0x04001AE7 RID: 6887
		public static Dictionary<int, int> RebornEquipHoleLevelMap = new Dictionary<int, int>();

		// Token: 0x04001AE8 RID: 6888
		public static Dictionary<int, List<int>> ExtPropIndexMap = new Dictionary<int, List<int>>();

		// Token: 0x04001AE9 RID: 6889
		public static object Mutex = new object();

		// Token: 0x04001AEA RID: 6890
		private static RebornEquip instance = new RebornEquip();
	}
}
