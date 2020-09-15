using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Goods;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	// Token: 0x020004F3 RID: 1267
	public static class WashPropsManager
	{
		// Token: 0x06001786 RID: 6022 RVA: 0x0017012C File Offset: 0x0016E32C
		public static bool initialize()
		{
			WashPropsManager.InitConfig();
			TCPCmdDispatcher.getInstance().registerProcessor(645, 5, WashPropsCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS));
			TCPCmdDispatcher.getInstance().registerProcessor(646, 4, WashPropsCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPSINHERIT));
			return true;
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x0017017C File Offset: 0x0016E37C
		public static bool startup()
		{
			return true;
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x00170190 File Offset: 0x0016E390
		public static bool showdown()
		{
			return true;
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x001701A4 File Offset: 0x0016E3A4
		public static bool destroy()
		{
			return true;
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x001701B8 File Offset: 0x0016E3B8
		public static void InitConfig()
		{
			WashPropsManager.XiLianTypeDict.Clear();
			WashPropsManager.XiLianShuXingDict.Clear();
			WashPropsManager.PropsIds.Clear();
			WashPropsManager.PropsIds.Add(13);
			WashPropsManager.PropsIds.Add(27);
			WashPropsManager.PropsIds.Add(38);
			WashPropsManager.PropsIds.Add(45);
			WashPropsManager.PropsIds.Add(46);
			WashPropsManager.PropsIds.Add(18);
			WashPropsManager.PropsIds.Add(19);
			WashPropsManager.PropsIds.Add(44);
			string fileName = "Config/XiLianType.xml";
			try
			{
				string fullPathFileName = Global.GameResPath("Config/XiLianType.xml");
				XElement xml = XElement.Load(fullPathFileName);
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fullPathFileName));
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement node in nodes)
				{
					XiLianType xiLianType = new XiLianType();
					xiLianType.ID = (int)Global.GetSafeAttributeLong(node, "ID");
					xiLianType.Color = Global.GetSafeAttributeStr(node, "Color");
					xiLianType.ShuXingNum = (int)Global.GetSafeAttributeLong(node, "ShuXingNum");
					xiLianType.Text = Global.GetSafeAttributeStr(node, "Text");
					xiLianType.FirstShuXing = 0.0;
					xiLianType.ShuXingLimitMultiplying = Global.GetSafeAttributeDouble(node, "Multiplying");
					WashPropsManager.XiLianTypeDict.Add(xiLianType.ID, xiLianType);
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。", fileName));
			}
			fileName = "Config/XiLianShuXing.xml";
			try
			{
				string fullPathFileName = Global.GameResPath(fileName);
				XElement xml = XElement.Load(fullPathFileName);
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fullPathFileName));
				}
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement node in nodes)
				{
					WashPropsManager.ParseXiLianShuXing(node);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
			}
			try
			{
				WashPropsManager.XiLianChuanChengGoodsRates = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengGoodsRates", ',');
				WashPropsManager.XiLianChuanChengXiaoHaoJinBi = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengXiaoHaoJinBi", ',');
				WashPropsManager.XiLianChuanChengXiaoHaoZhuanShi = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengXiaoHaoZhuanShi", ',');
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
			}
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x001704F8 File Offset: 0x0016E6F8
		public static void ParseXiLianShuXing(XElement node)
		{
			XiLianShuXing xiLianShuXing = new XiLianShuXing();
			xiLianShuXing.ID = (int)Global.GetSafeAttributeLong(node, "ID");
			xiLianShuXing.Name = Global.GetSafeAttributeStr(node, "Name");
			xiLianShuXing.NeedJinBi = (int)Global.GetSafeAttributeLong(node, "NeedJinBi");
			xiLianShuXing.NeedZuanShi = (int)Global.GetSafeAttributeLong(node, "NeedZuanShi");
			long[] args = Global.GetSafeAttributeLongArray(node, "NeedGoods", 2);
			if (null != args)
			{
				xiLianShuXing.NeedGoodsIDs.Add((int)args[0]);
				xiLianShuXing.NeedGoodsCounts.Add((int)args[1]);
			}
			foreach (int propsID in WashPropsManager.PropsIds)
			{
				ExtPropIndexes propIndex = (ExtPropIndexes)propsID;
				string attributeName = string.Format("JinBi{0}", propIndex.ToString());
				args = Global.GetSafeAttributeLongArray(node, attributeName, -1);
				if (args != null && args.Length > 0)
				{
					xiLianShuXing.PromoteJinBiRange.Add(propsID, new List<long>(args));
				}
				attributeName = string.Format("ZuanShi{0}", propIndex.ToString());
				args = Global.GetSafeAttributeLongArray(node, attributeName, -1);
				if (args != null && args.Length > 0)
				{
					xiLianShuXing.PromoteZuanShiRange.Add(propsID, new List<long>(args));
				}
				xiLianShuXing.PromotePropLimit.Add((int)propIndex, (int)Global.GetSafeAttributeLong(node, propIndex.ToString()));
			}
			WashPropsManager.XiLianShuXingDict.Add(xiLianShuXing.ID, xiLianShuXing);
		}

		// Token: 0x0600178C RID: 6028 RVA: 0x001706A8 File Offset: 0x0016E8A8
		public static bool WashProps(GameClient client, int dbid, int washIndex, bool firstUseBinding, int moneyType)
		{
			int nID = 645;
			List<int> result = new List<int>();
			result.Add(washIndex);
			result.Add(1);
			result.Add(dbid);
			result.Add(0);
			int needMoney = 0;
			double Discount = 1.0;
			object arg = null;
			if (HuodongCachingMgr.GetJieriFuLiActivity().IsOpened(EJieRiFuLiType.OneDiscountDiamond, out arg))
			{
				List<double> argList = (List<double>)arg;
				if (argList.Count != 2)
				{
					result[1] = -3;
					client.sendCmd<List<int>>(nID, result, false);
					return true;
				}
				if (moneyType == 0)
				{
					Discount = argList[0];
				}
				else if (moneyType == 1)
				{
					Discount = argList[1];
				}
			}
			bool result2;
			if (washIndex > -2 || washIndex < -5)
			{
				result[1] = -5;
				client.sendCmd<List<int>>(nID, result, false);
				result2 = true;
			}
			else if (moneyType < 0 || moneyType > 1)
			{
				result[1] = -14;
				client.sendCmd<List<int>>(nID, result, false);
				result2 = true;
			}
			else
			{
				GoodsData goodsData = Global.GetGoodsByDbID(client, dbid);
				SystemXmlItem xml;
				if (null == goodsData)
				{
					result[1] = -1;
					client.sendCmd<List<int>>(nID, result, false);
					result2 = true;
				}
				else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xml))
				{
					result[1] = -3;
					client.sendCmd<List<int>>(nID, result, false);
					result2 = true;
				}
				else
				{
					int id = xml.GetIntValue("XiLian", -1);
					XiLianShuXing xiLianShuXing;
					if (!WashPropsManager.XiLianShuXingDict.TryGetValue(id, out xiLianShuXing))
					{
						result[1] = -3;
						client.sendCmd<List<int>>(nID, result, false);
						result2 = true;
					}
					else
					{
						if (washIndex == -2 || washIndex == -1)
						{
							if (moneyType == 0)
							{
								if (xiLianShuXing.NeedJinBi < 1)
								{
									result[1] = -3;
									client.sendCmd<List<int>>(nID, result, false);
									return true;
								}
								needMoney = Math.Max((int)((double)xiLianShuXing.NeedJinBi * Discount), 1);
								if (client.ClientData.Money1 + client.ClientData.YinLiang < needMoney)
								{
									result[1] = -9;
									client.sendCmd<List<int>>(nID, result, false);
									return true;
								}
							}
							else if (moneyType == 1)
							{
								if (xiLianShuXing.NeedZuanShi < 1)
								{
									result[1] = -3;
									client.sendCmd<List<int>>(nID, result, false);
									return true;
								}
								needMoney = Math.Max((int)((double)xiLianShuXing.NeedZuanShi * Discount), 1);
								if (client.ClientData.UserMoney < needMoney && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, needMoney))
								{
									result[1] = -10;
									client.sendCmd<List<int>>(nID, result, false);
									return true;
								}
							}
						}
						if (washIndex == -1)
						{
							if (goodsData.WashProps != null && goodsData.WashProps.Count > 0)
							{
								result[1] = -5;
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
							else
							{
								int color = Global.GetEquipColor(goodsData);
								XiLianType xiLianType;
								if (color <= 0 || !WashPropsManager.XiLianTypeDict.TryGetValue(color, out xiLianType) || xiLianType.ShuXingNum <= 0)
								{
									result[1] = -5;
									client.sendCmd<List<int>>(nID, result, false);
									result2 = true;
								}
								else
								{
									UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = dbid
									};
									updateGoodsArgs.WashProps = new List<int>();
									if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
									{
										client.ClientData._ReplaceExtArg.Reset();
										if (SingletonTemplate<GoodsReplaceManager>.Instance().NeedCheckSuit(Global.GetGoodsCatetoriy(goodsData.GoodsID)))
										{
											client.ClientData._ReplaceExtArg.CurrEquipSuit = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
										}
										GoodsReplaceResult replaceRet = SingletonTemplate<GoodsReplaceManager>.Instance().GetReplaceResult(client, xiLianShuXing.NeedGoodsIDs[0]);
										if (replaceRet == null || replaceRet.TotalGoodsCnt() < xiLianShuXing.NeedGoodsCounts[0])
										{
											result[1] = -6;
											client.sendCmd<List<int>>(nID, result, false);
											return true;
										}
										List<GoodsReplaceResult.ReplaceItem> realCostList = new List<GoodsReplaceResult.ReplaceItem>();
										if (firstUseBinding)
										{
											realCostList.AddRange(replaceRet.BindList);
											realCostList.Add(replaceRet.OriginBindGoods);
											realCostList.AddRange(replaceRet.UnBindList);
											realCostList.Add(replaceRet.OriginUnBindGoods);
										}
										else
										{
											realCostList.AddRange(replaceRet.UnBindList);
											realCostList.Add(replaceRet.OriginUnBindGoods);
											realCostList.AddRange(replaceRet.BindList);
											realCostList.Add(replaceRet.OriginBindGoods);
										}
										int stillNeedGoodsCnt = xiLianShuXing.NeedGoodsCounts[0];
										foreach (GoodsReplaceResult.ReplaceItem item in realCostList)
										{
											if (item.GoodsCnt > 0)
											{
												int realCostCnt = Math.Min(stillNeedGoodsCnt, item.GoodsCnt);
												if (realCostCnt <= 0)
												{
													break;
												}
												bool usedBinding_just_placeholder = false;
												bool usedTimeLimited_just_placeholder = false;
												bool bFailed = false;
												if (item.IsBind)
												{
													if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, realCostCnt, false, out usedBinding_just_placeholder, out usedTimeLimited_just_placeholder, false))
													{
														bFailed = true;
													}
													updateGoodsArgs.Binding = 1;
												}
												else if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, realCostCnt, false, out usedBinding_just_placeholder, out usedTimeLimited_just_placeholder, false))
												{
													bFailed = true;
												}
												if (bFailed)
												{
													result[1] = -6;
													client.sendCmd<List<int>>(nID, result, false);
													return true;
												}
												stillNeedGoodsCnt -= realCostCnt;
											}
										}
									}
									for (int i = 0; i < xiLianType.ShuXingNum; i++)
									{
										int rand = Global.GetRandomNumber(0, WashPropsManager.PropsIds.Count);
										int propID = WashPropsManager.PropsIds[rand];
										int propLimit = xiLianShuXing.PromotePropLimit[propID];
										int propValue = (int)Math.Ceiling((double)propLimit * xiLianType.FirstShuXing * xiLianType.ShuXingLimitMultiplying);
										updateGoodsArgs.WashProps.Add(propID);
										updateGoodsArgs.WashProps.Add(propValue);
									}
									Global.UpdateGoodsProp(client, goodsData, updateGoodsArgs, true);
									Global.ModRoleGoodsEvent(client, goodsData, 0, "装备洗炼激活", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "装备洗炼激活");
									result[3] = ((goodsData.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0);
									result.AddRange(goodsData.WashProps);
									client.sendCmd<List<int>>(nID, result, false);
									result2 = true;
								}
							}
						}
						else if (washIndex == -2)
						{
							UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
							{
								RoleID = client.ClientData.RoleID,
								DbID = dbid
							};
							if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
							{
								client.ClientData._ReplaceExtArg.Reset();
								if (SingletonTemplate<GoodsReplaceManager>.Instance().NeedCheckSuit(Global.GetGoodsCatetoriy(goodsData.GoodsID)))
								{
									client.ClientData._ReplaceExtArg.CurrEquipSuit = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
								}
								GoodsReplaceResult replaceRet = SingletonTemplate<GoodsReplaceManager>.Instance().GetReplaceResult(client, xiLianShuXing.NeedGoodsIDs[0]);
								if (replaceRet == null || replaceRet.TotalGoodsCnt() < xiLianShuXing.NeedGoodsCounts[0])
								{
									result[1] = -6;
									client.sendCmd<List<int>>(nID, result, false);
									return true;
								}
								List<GoodsReplaceResult.ReplaceItem> realCostList = new List<GoodsReplaceResult.ReplaceItem>();
								if (firstUseBinding)
								{
									realCostList.AddRange(replaceRet.BindList);
									realCostList.Add(replaceRet.OriginBindGoods);
									realCostList.AddRange(replaceRet.UnBindList);
									realCostList.Add(replaceRet.OriginUnBindGoods);
								}
								else
								{
									realCostList.AddRange(replaceRet.UnBindList);
									realCostList.Add(replaceRet.OriginUnBindGoods);
									realCostList.AddRange(replaceRet.BindList);
									realCostList.Add(replaceRet.OriginBindGoods);
								}
								int stillNeedGoodsCnt = xiLianShuXing.NeedGoodsCounts[0];
								foreach (GoodsReplaceResult.ReplaceItem item in realCostList)
								{
									if (item.GoodsCnt > 0)
									{
										int realCostCnt = Math.Min(stillNeedGoodsCnt, item.GoodsCnt);
										if (realCostCnt <= 0)
										{
											break;
										}
										bool usedBinding_just_placeholder = false;
										bool usedTimeLimited_just_placeholder = false;
										bool bFailed = false;
										if (item.IsBind)
										{
											if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, realCostCnt, false, out usedBinding_just_placeholder, out usedTimeLimited_just_placeholder, false))
											{
												bFailed = true;
											}
											updateGoodsArgs.Binding = 1;
										}
										else if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, realCostCnt, false, out usedBinding_just_placeholder, out usedTimeLimited_just_placeholder, false))
										{
											bFailed = true;
										}
										if (bFailed)
										{
											result[1] = -6;
											client.sendCmd<List<int>>(nID, result, false);
											return true;
										}
										stillNeedGoodsCnt -= realCostCnt;
									}
								}
							}
							if (moneyType == 0)
							{
								Global.SubBindTongQianAndTongQian(client, needMoney, "洗炼");
							}
							else if (moneyType == 1)
							{
								GameManager.ClientMgr.SubUserMoney(client, needMoney, "洗炼", true, true, true, true, DaiBiSySType.ZhuangBeiPeiYang);
							}
							int color = Global.GetEquipColor(goodsData);
							XiLianType xiLianType;
							if (color <= 0 || !WashPropsManager.XiLianTypeDict.TryGetValue(color, out xiLianType) || xiLianType.ShuXingNum <= 0)
							{
								result[1] = -5;
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
							else
							{
								if (goodsData.WashProps == null || goodsData.WashProps.Count == 0)
								{
									List<int> washProps = new List<int>(xiLianType.ShuXingNum * 2);
									int maxCount = xiLianType.ShuXingNum;
									foreach (KeyValuePair<int, int> kv in xiLianShuXing.PromotePropLimit)
									{
										if (kv.Value > 0)
										{
											washProps.Add(kv.Key);
											washProps.Add(0);
											if (--maxCount <= 0)
											{
												break;
											}
										}
									}
									updateGoodsArgs.WashProps = washProps;
								}
								else
								{
									updateGoodsArgs.WashProps = new List<int>(goodsData.WashProps);
								}
								for (int i = 0; i < updateGoodsArgs.WashProps.Count; i += 2)
								{
									int propID = updateGoodsArgs.WashProps[i];
									if (!xiLianShuXing.PromotePropLimit.ContainsKey(propID))
									{
										result[1] = -3;
										client.sendCmd<List<int>>(nID, result, false);
										return true;
									}
									int propValue = updateGoodsArgs.WashProps[i + 1];
									int propLimit = (int)((double)xiLianShuXing.PromotePropLimit[propID] * xiLianType.ShuXingLimitMultiplying);
									if (moneyType == 0)
									{
										int nRandNum = Global.GetRandomNumber(0, xiLianShuXing.PromoteJinBiRange[propID].Count);
										propValue += (int)xiLianShuXing.PromoteJinBiRange[propID][nRandNum];
									}
									else if (moneyType == 1)
									{
										int nRandNum = Global.GetRandomNumber(0, xiLianShuXing.PromoteZuanShiRange[propID].Count);
										propValue += (int)xiLianShuXing.PromoteZuanShiRange[propID][nRandNum];
									}
									propValue = Global.Clamp(propValue, 0, propLimit);
									updateGoodsArgs.WashProps[i + 1] = propValue;
								}
								client.ClientData.TempWashPropsDict[updateGoodsArgs.DbID] = updateGoodsArgs;
								client.ClientData.TempWashPropOperationIndex = washIndex;
								result[3] = ((goodsData.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0);
								result.AddRange(updateGoodsArgs.WashProps);
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
						}
						else if (washIndex == -3)
						{
							UpdateGoodsArgs tempWashProps;
							if (!client.ClientData.TempWashPropsDict.TryGetValue(goodsData.Id, out tempWashProps))
							{
								result[1] = -2;
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
							else
							{
								Global.UpdateGoodsProp(client, goodsData, tempWashProps, true);
								Global.ModRoleGoodsEvent(client, goodsData, 0, "装备洗炼", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "装备洗炼");
								client.ClientData.TempWashPropsDict.Remove(goodsData.Id);
								result[3] = ((goodsData.Binding > 0) ? 1 : 0);
								result.AddRange(goodsData.WashProps);
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
						}
						else if (washIndex == -4)
						{
							client.ClientData.TempWashPropsDict.Remove(dbid);
							client.sendCmd<List<int>>(nID, result, false);
							result2 = true;
						}
						else if (washIndex == -5)
						{
							UpdateGoodsArgs tempWashProps;
							if (!client.ClientData.TempWashPropsDict.TryGetValue(goodsData.Id, out tempWashProps))
							{
								result[1] = -4;
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
							else
							{
								result[0] = client.ClientData.TempWashPropOperationIndex;
								result[2] = tempWashProps.DbID;
								result[3] = (tempWashProps.Binding | goodsData.Binding);
								result.AddRange(tempWashProps.WashProps);
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
						}
						else if (washIndex >= 0)
						{
							if (washIndex < 0 || goodsData.WashProps == null || goodsData.WashProps.Count / 2 <= washIndex)
							{
								result[1] = -2;
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
							else
							{
								int color = Global.GetEquipColor(goodsData);
								XiLianType xiLianType;
								if (color <= 0 || !WashPropsManager.XiLianTypeDict.TryGetValue(color, out xiLianType) || xiLianType.ShuXingNum <= washIndex)
								{
									result[1] = -5;
									client.sendCmd<List<int>>(nID, result, false);
									result2 = true;
								}
								else
								{
									UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = dbid
									};
									updateGoodsArgs.WashProps = new List<int>(goodsData.WashProps);
									if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
									{
										bool bUsedBinding = firstUseBinding;
										bool bUsedTimeLimited = false;
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, xiLianShuXing.NeedGoodsIDs[0], xiLianShuXing.NeedGoodsCounts[0], false, out bUsedBinding, out bUsedTimeLimited, false))
										{
											result[1] = -6;
											client.sendCmd<List<int>>(nID, result, false);
											return true;
										}
										if (goodsData.Binding == 0 && bUsedBinding)
										{
											updateGoodsArgs.Binding = 1;
										}
									}
									int rand = Global.GetRandomNumber(0, WashPropsManager.PropsIds.Count);
									int propID = WashPropsManager.PropsIds[rand];
									int propLimit = xiLianShuXing.PromotePropLimit[propID];
									int propValue = (int)Math.Ceiling((double)propLimit * xiLianType.FirstShuXing * xiLianType.ShuXingLimitMultiplying);
									updateGoodsArgs.WashProps[washIndex * 2] = propID;
									updateGoodsArgs.WashProps[washIndex * 2 + 1] = propValue;
									client.ClientData.TempWashPropsDict[updateGoodsArgs.DbID] = updateGoodsArgs;
									client.ClientData.TempWashPropOperationIndex = washIndex;
									result[3] = ((goodsData.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0);
									result.Add(propID);
									result.Add(propValue);
									client.sendCmd<List<int>>(nID, result, false);
									result2 = true;
								}
							}
						}
						else
						{
							result[1] = -2;
							client.sendCmd<List<int>>(nID, result, false);
							result2 = true;
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x00171948 File Offset: 0x0016FB48
		public static bool WashPropsInherit(GameClient client, int leftGoodsDbID, int rightGoodsDbID, int nSubMoneyType)
		{
			int roleID = client.ClientData.RoleID;
			int nID = 646;
			List<int> result = new List<int>();
			result.Add(1);
			result.Add(leftGoodsDbID);
			result.Add(rightGoodsDbID);
			result.Add(0);
			GoodsData leftGoodsData = Global.GetGoodsByDbID(client, leftGoodsDbID);
			bool result2;
			if (null == leftGoodsData)
			{
				result[0] = -1;
				client.sendCmd<List<int>>(nID, result, false);
				result2 = true;
			}
			else
			{
				GoodsData rightGoodsData = Global.GetGoodsByDbID(client, rightGoodsDbID);
				SystemXmlItem xml;
				if (null == rightGoodsData)
				{
					result[0] = -1;
					client.sendCmd<List<int>>(nID, result, false);
					result2 = true;
				}
				else if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(rightGoodsData.GoodsID, out xml))
				{
					result.Add(-3);
					client.sendCmd<List<int>>(nID, result, false);
					result2 = true;
				}
				else
				{
					int id = xml.GetIntValue("XiLian", -1);
					XiLianShuXing xiLianShuXing;
					if (!WashPropsManager.XiLianShuXingDict.TryGetValue(id, out xiLianShuXing))
					{
						result.Add(-3);
						client.sendCmd<List<int>>(nID, result, false);
						result2 = true;
					}
					else
					{
						int nLeftColor = Global.GetEquipColor(leftGoodsData);
						int nRigthColor = Global.GetEquipColor(rightGoodsData);
						if (nLeftColor < 2 || nRigthColor < 2 || null == leftGoodsData.WashProps)
						{
							result[0] = -12;
							client.sendCmd<List<int>>(nID, result, false);
							result2 = true;
						}
						else
						{
							XiLianType xiLianType = null;
							if (!WashPropsManager.XiLianTypeDict.TryGetValue(nRigthColor, out xiLianType))
							{
								result.Add(-3);
								client.sendCmd<List<int>>(nID, result, false);
								result2 = true;
							}
							else
							{
								int OccupationLeft = Global.GetMainOccupationByGoodsID(leftGoodsData.GoodsID);
								int OccupationRight = Global.GetMainOccupationByGoodsID(rightGoodsData.GoodsID);
								if (OccupationLeft != OccupationRight)
								{
									result[0] = -12;
									client.sendCmd<List<int>>(nID, result, false);
									result2 = true;
								}
								else
								{
									int categoryLeft = Global.GetGoodsCatetoriy(leftGoodsData.GoodsID);
									int categoryRight = Global.GetGoodsCatetoriy(rightGoodsData.GoodsID);
									int ret = GoodsUtil.CanUpgradeInhert(categoryLeft, categoryRight, 8);
									if (ret < 0)
									{
										result[0] = -13;
										client.sendCmd<List<int>>(nID, result, false);
										result2 = true;
									}
									else if (leftGoodsData.Site != 0 || rightGoodsData.Site != 0)
									{
										result[0] = -8;
										client.sendCmd<List<int>>(nID, result, false);
										result2 = true;
									}
									else if (nSubMoneyType < 1 || nSubMoneyType > 2)
									{
										result[0] = -14;
										client.sendCmd<List<int>>(nID, result, false);
										result2 = true;
									}
									else
									{
										if (nSubMoneyType == 1)
										{
											if (WashPropsManager.XiLianChuanChengXiaoHaoJinBi[0] > 0 && !Global.SubBindTongQianAndTongQian(client, WashPropsManager.XiLianChuanChengXiaoHaoJinBi[0], "洗练属性传承"))
											{
												result[0] = -9;
												client.sendCmd<List<int>>(nID, result, false);
												return true;
											}
										}
										else if (nSubMoneyType == 2)
										{
											if (WashPropsManager.XiLianChuanChengXiaoHaoZhuanShi[0] > 0 && !GameManager.ClientMgr.SubUserMoney(client, WashPropsManager.XiLianChuanChengXiaoHaoZhuanShi[0], "洗练属性传承", true, true, true, true, DaiBiSySType.ZhuangBeiChuanCheng))
											{
												result[0] = -10;
												client.sendCmd<List<int>>(nID, result, false);
												return true;
											}
										}
										int nBinding = 0;
										if (rightGoodsData.Binding == 1 || leftGoodsData.Binding == 1)
										{
											nBinding = 1;
										}
										int rnd = Global.GetRandomNumber(0, 101);
										if (WashPropsManager.XiLianChuanChengGoodsRates != null && rnd > WashPropsManager.XiLianChuanChengGoodsRates[nLeftColor])
										{
											result[0] = -11;
											client.sendCmd<List<int>>(nID, result, false);
											result2 = true;
										}
										else
										{
											UpdateGoodsArgs argsLeft = new UpdateGoodsArgs
											{
												RoleID = roleID,
												DbID = leftGoodsDbID
											};
											argsLeft.WashProps = new List<int>(leftGoodsData.WashProps);
											UpdateGoodsArgs argsRight = new UpdateGoodsArgs
											{
												RoleID = roleID,
												DbID = rightGoodsDbID
											};
											if (rightGoodsData.WashProps == null || rightGoodsData.WashProps.Count == 0)
											{
												argsRight.WashProps = new List<int>(xiLianType.ShuXingNum * 2);
												int maxCount = 0;
												foreach (KeyValuePair<int, int> kv in xiLianShuXing.PromotePropLimit)
												{
													if (kv.Value > 0)
													{
														argsRight.WashProps.Add(kv.Key);
														argsRight.WashProps.Add(0);
														if (++maxCount >= xiLianType.ShuXingNum)
														{
															break;
														}
													}
												}
											}
											else
											{
												argsRight.WashProps = new List<int>(rightGoodsData.WashProps);
											}
											List<int> correctPropsList = new List<int>();
											for (int i = 0; i < argsRight.WashProps.Count - 1; i += 2)
											{
												int propID = argsRight.WashProps[i];
												int propLimit = 0;
												if (correctPropsList.Contains(propID) || !xiLianShuXing.PromotePropLimit.TryGetValue(propID, out propLimit) || propLimit <= 0)
												{
													foreach (KeyValuePair<int, int> kv in xiLianShuXing.PromotePropLimit)
													{
														if (kv.Value > 0)
														{
															argsRight.WashProps[i] = kv.Key;
															argsRight.WashProps[i + 1] = 0;
															correctPropsList.Add(kv.Key);
														}
													}
												}
												else
												{
													correctPropsList.Add(propID);
												}
											}
											List<int> inhertPropsList = new List<int>();
											for (int i = 0; i < argsLeft.WashProps.Count - 1; i += 2)
											{
												for (int j = 0; j < argsRight.WashProps.Count - 1; j += 2)
												{
													if (argsLeft.WashProps[i] == argsRight.WashProps[j])
													{
														int propID = argsLeft.WashProps[i];
														int propLimit = 0;
														inhertPropsList.Add(propID);
														argsRight.WashProps[j] = propID;
														if (xiLianShuXing.PromotePropLimit.TryGetValue(propID, out propLimit))
														{
															argsRight.WashProps[j + 1] = (int)Math.Round(Global.Clamp((double)argsLeft.WashProps[i + 1], 0.0, (double)propLimit * xiLianType.ShuXingLimitMultiplying));
														}
														else
														{
															argsRight.WashProps[j + 1] = 0;
														}
													}
												}
											}
											for (int i = 0; i < argsLeft.WashProps.Count - 1; i += 2)
											{
												if (!inhertPropsList.Contains(argsLeft.WashProps[i]))
												{
													inhertPropsList.Add(argsLeft.WashProps[i]);
													for (int j = 0; j < argsRight.WashProps.Count - 1; j += 2)
													{
														if (!inhertPropsList.Contains(argsRight.WashProps[j]))
														{
															inhertPropsList.Add(argsRight.WashProps[j]);
															int propID = argsRight.WashProps[j];
															int propLimit = 0;
															argsRight.WashProps[i] = propID;
															if (xiLianShuXing.PromotePropLimit.TryGetValue(propID, out propLimit))
															{
																if (argsLeft.WashProps[i] == 44 && argsRight.WashProps[j] == 45)
																{
																	argsRight.WashProps[j + 1] = (int)Math.Floor(Global.Clamp((double)(argsLeft.WashProps[i + 1] * 10), 0.0, (double)propLimit * xiLianType.ShuXingLimitMultiplying));
																}
																else if (argsLeft.WashProps[i] == 45 && argsRight.WashProps[j] == 44)
																{
																	argsRight.WashProps[j + 1] = (int)Math.Floor(Global.Clamp((double)(argsLeft.WashProps[i + 1] / 10), 0.0, (double)propLimit * xiLianType.ShuXingLimitMultiplying));
																}
																else
																{
																	argsRight.WashProps[j + 1] = 0;
																}
															}
															else
															{
																argsRight.WashProps[j + 1] = 0;
															}
														}
													}
												}
											}
											argsLeft.WashProps = null;
											argsRight.Binding = nBinding;
											client.ClientData.TempWashPropsDict.Remove(argsLeft.DbID);
											client.ClientData.TempWashPropsDict.Remove(argsRight.DbID);
											if (Global.UpdateGoodsProp(client, leftGoodsData, argsLeft, true) < 0)
											{
												result[0] = -15;
												client.sendCmd<List<int>>(nID, result, false);
												result2 = true;
											}
											else if (Global.UpdateGoodsProp(client, rightGoodsData, argsRight, true) < 0)
											{
												result[0] = -15;
												client.sendCmd<List<int>>(nID, result, false);
												result2 = true;
											}
											else
											{
												Global.ModRoleGoodsEvent(client, leftGoodsData, 0, "装备洗炼传承_提供方", false);
												Global.ModRoleGoodsEvent(client, rightGoodsData, 0, "装备洗炼传承_接受方", false);
												EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, leftGoodsData.GoodsID, (long)leftGoodsData.Id, 0, leftGoodsData.GCount, "装备洗炼传承_提供方");
												EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, rightGoodsData.GoodsID, (long)rightGoodsData.Id, 0, rightGoodsData.GCount, "装备洗炼传承_接受方");
												if (leftGoodsData.Using > 0 || rightGoodsData.Using > 0)
												{
													Global.RefreshEquipPropAndNotify(client);
												}
												GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.EquipChuanChengTimes));
												ProcessTask.ProcessAddTaskVal(client, TaskTypes.EquipChuanCheng, -1, 1, new object[0]);
												result[3] = nBinding;
												result.AddRange(rightGoodsData.WashProps);
												client.sendCmd<List<int>>(nID, result, false);
												result2 = true;
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

		// Token: 0x0600178E RID: 6030 RVA: 0x0017243C File Offset: 0x0017063C
		public static List<int> WashPropsMax(params List<int>[] args)
		{
			List<int> washProps = null;
			if (null != args)
			{
				foreach (List<int> list in args)
				{
					if (null != list)
					{
						if (null == washProps)
						{
							washProps = list.GetRange(0, list.Count);
						}
						else
						{
							for (int j = 0; j < washProps.Count - 1; j += 2)
							{
								for (int k = 0; k < list.Count - 1; k += 2)
								{
									if (washProps[j] == list[k] && list[k + 1] > washProps[j + 1])
									{
										washProps[j + 1] = list[k + 1];
									}
								}
							}
						}
					}
				}
			}
			return washProps;
		}

		// Token: 0x04002194 RID: 8596
		private static int[] XiLianChuanChengGoodsRates = new int[5];

		// Token: 0x04002195 RID: 8597
		private static int[] XiLianChuanChengXiaoHaoJinBi = new int[16];

		// Token: 0x04002196 RID: 8598
		private static int[] XiLianChuanChengXiaoHaoZhuanShi = new int[16];

		// Token: 0x04002197 RID: 8599
		private static List<int> PropsIds = new List<int>();

		// Token: 0x04002198 RID: 8600
		private static Dictionary<int, XiLianType> XiLianTypeDict = new Dictionary<int, XiLianType>();

		// Token: 0x04002199 RID: 8601
		private static Dictionary<int, XiLianShuXing> XiLianShuXingDict = new Dictionary<int, XiLianShuXing>();

		// Token: 0x020004F4 RID: 1268
		public static class WashOperations
		{
			// Token: 0x0400219A RID: 8602
			public const int WashProps = 0;

			// Token: 0x0400219B RID: 8603
			public const int WashPropsActive = -1;

			// Token: 0x0400219C RID: 8604
			public const int WashPropsQuantity = -2;

			// Token: 0x0400219D RID: 8605
			public const int WashPropsCommit = -3;

			// Token: 0x0400219E RID: 8606
			public const int WashPropsCancle = -4;

			// Token: 0x0400219F RID: 8607
			public const int WashPropsQuery = -5;
		}
	}
}
