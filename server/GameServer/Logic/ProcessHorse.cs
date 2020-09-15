using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200077B RID: 1915
	public class ProcessHorse
	{
		// Token: 0x0600311A RID: 12570 RVA: 0x002B7C54 File Offset: 0x002B5E54
		public static int ProcessHorseEnchance(GameClient client, int horseDbID, int extPropIndex, bool allowAutoBuy)
		{
			HorseData horseData = Global.GetHorseDataByDbID(client, horseDbID);
			int result;
			if (null == horseData)
			{
				result = -1;
			}
			else if (extPropIndex < 0 || extPropIndex >= 10)
			{
				result = -10;
			}
			else
			{
				int EnchanceNeedGoodsID = (int)GameManager.systemParamsList.GetParamValueIntByName("HorseEnchancseGoodsID", -1);
				if (EnchanceNeedGoodsID <= 0)
				{
					result = -20;
				}
				else
				{
					int[] horseExtNumIntArray = Global.HorseExtStr2IntArray(horseData.PropsNum);
					int[] horseExtPropIntArray = Global.HorseExtStr2IntArray(horseData.PropsVal);
					int horseExtNum = Global.GetHorseExtFieldIntVal(horseExtNumIntArray, (HorseExtIndexes)extPropIndex);
					int horseExtVal = Global.GetHorseExtFieldIntVal(horseExtPropIntArray, (HorseExtIndexes)extPropIndex);
					SystemXmlItem systemHorseEnchance = Global.GetHorseEnchanceXmlNode(horseExtNum + 1, (HorseExtIndexes)extPropIndex);
					if (null == systemHorseEnchance)
					{
						result = -35;
					}
					else
					{
						int baseVal = Global.GetHorseBasePropVal(horseData.HorseID, (HorseExtIndexes)extPropIndex, null);
						int propLimit = Global.GetHorsePropLimitVal(horseData.HorseID, (HorseExtIndexes)extPropIndex);
						if (baseVal + horseExtVal >= propLimit)
						{
							result = -40;
						}
						else
						{
							int needYinLiang = Global.GMax(systemHorseEnchance.GetIntValue("UseMoney", -1), 0);
							if (client.ClientData.YinLiang < needYinLiang)
							{
								result = -60;
							}
							else
							{
								int needBuyGoodsNum = 0;
								int needGoodsNum = Global.GMax(systemHorseEnchance.GetIntValue("HanTie", -1), 0);
								int needSubGoodsNum = needGoodsNum;
								if (Global.GetTotalGoodsCountByID(client, EnchanceNeedGoodsID) < needGoodsNum)
								{
									if (!allowAutoBuy)
									{
										return -70;
									}
									needSubGoodsNum = Global.GetTotalGoodsCountByID(client, EnchanceNeedGoodsID);
									needBuyGoodsNum = needGoodsNum - needSubGoodsNum;
								}
								bool usedBinding = false;
								bool usedTimeLimited = false;
								if (needSubGoodsNum > 0)
								{
									if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, EnchanceNeedGoodsID, needSubGoodsNum, false, out usedBinding, out usedTimeLimited, false))
									{
										return -70;
									}
								}
								if (needBuyGoodsNum > 0)
								{
									int retAuto = Global.SubUserMoneyForGoods(client, EnchanceNeedGoodsID, needBuyGoodsNum, "坐骑强化");
									if (retAuto <= 0)
									{
										return retAuto;
									}
								}
								if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needYinLiang, "坐骑强化", false))
								{
									result = -60;
								}
								else
								{
									int successRate = Global.GMax(systemHorseEnchance.GetIntValue("SucceedRate", -1), 0);
									int randNum = Global.GetRandomNumber(0, 101);
									if (client.ClientData.TempHorseEnchanceRate != 1)
									{
										successRate *= client.ClientData.TempHorseEnchanceRate;
										successRate = Global.GMin(100, successRate);
									}
									if (randNum > successRate)
									{
										result = -1000;
									}
									else
									{
										int addPropValue = Global.GMax(systemHorseEnchance.GetIntValue("PropVal", -1), 0);
										if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
										{
											Global.UpdateHorseDataProps(client, false);
										}
										int ret = 0;
										if (Global.UpdateHorsePropsDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseData.DbID, (HorseExtIndexes)extPropIndex, addPropValue, 1) < 0)
										{
											ret = -2000;
										}
										if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
										{
											Global.UpdateHorseDataProps(client, true);
											if (0 == ret)
											{
												client.ClientData.RoleHorseJiFen = Global.CalcHorsePropsJiFen(horseData);
												GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
												GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
											}
										}
										if (0 == ret)
										{
											if (Global.IsHorsePropsFull(horseData))
											{
												Global.BroadcastHorseEnchanceOk(client, horseData.HorseID);
											}
										}
										result = ret;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600311B RID: 12571 RVA: 0x002B8060 File Offset: 0x002B6260
		public static int ProcessHorseQuickAllEnchance(GameClient client, int horseDbID)
		{
			HorseData horseData = Global.GetHorseDataByDbID(client, horseDbID);
			int result;
			if (null == horseData)
			{
				result = -1;
			}
			else if (Global.IsHorsePropsFull(horseData))
			{
				result = -10;
			}
			else
			{
				int chaoJiLianGuGoodsID = (int)GameManager.systemParamsList.GetParamValueIntByName("ChaoJiLianGuGoodsID", -1);
				if (chaoJiLianGuGoodsID <= 0)
				{
					result = -20;
				}
				else
				{
					int needYinLiang = Global.QuickHorseExtPropNeedYinLiang;
					if (client.ClientData.YinLiang < needYinLiang)
					{
						result = -30;
					}
					else if (Global.GetTotalGoodsCountByID(client, chaoJiLianGuGoodsID) < 1)
					{
						result = -40;
					}
					else
					{
						bool usedBinding = false;
						bool usedTimeLimited = false;
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, chaoJiLianGuGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
						{
							result = -60;
						}
						else if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needYinLiang, "坐骑快速全部属性强化", false))
						{
							result = -70;
						}
						else
						{
							if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
							{
								Global.UpdateHorseDataProps(client, false);
							}
							for (int i = 0; i < 10; i++)
							{
								int[] horseExtNumIntArray = Global.HorseExtStr2IntArray(horseData.PropsNum);
								int[] horseExtPropIntArray = Global.HorseExtStr2IntArray(horseData.PropsVal);
								int horseExtNum = Global.GetHorseExtFieldIntVal(horseExtNumIntArray, (HorseExtIndexes)i);
								int horseExtVal = Global.GetHorseExtFieldIntVal(horseExtPropIntArray, (HorseExtIndexes)i);
								int baseVal = Global.GetHorseBasePropVal(horseData.HorseID, (HorseExtIndexes)i, null);
								int propLimit = Global.GetHorsePropLimitVal(horseData.HorseID, (HorseExtIndexes)i);
								if (baseVal + horseExtVal < propLimit)
								{
									int maxEnchanceLevel = Global.GetHorseEnchanceNum(horseData.HorseID);
									int addNum = maxEnchanceLevel - horseExtNum;
									int addPropValue = propLimit - baseVal - horseExtVal;
									Global.UpdateHorsePropsDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseData.DbID, (HorseExtIndexes)i, addPropValue, addNum);
								}
							}
							if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
							{
								Global.UpdateHorseDataProps(client, true);
								client.ClientData.RoleHorseJiFen = Global.CalcHorsePropsJiFen(horseData);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							}
							if (Global.IsHorsePropsFull(horseData))
							{
								Global.BroadcastHorseEnchanceOk(client, horseData.HorseID);
							}
							result = 0;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600311C RID: 12572 RVA: 0x002B8334 File Offset: 0x002B6534
		public static int ProcessHorseUpgrade(GameClient client, int horseDbID, bool allowAutoBuy)
		{
			HorseData horseData = Global.GetHorseDataByDbID(client, horseDbID);
			int result;
			if (null == horseData)
			{
				result = -1;
			}
			else
			{
				SystemXmlItem horseUpXmlNode = Global.GetHorseUpXmlNode(horseData.HorseID + 1);
				if (null == horseUpXmlNode)
				{
					result = -35;
				}
				else
				{
					int horseUpgradeGoodsID = 0;
					int horseUpgradeGoodsNum = 0;
					Global.ParseHorseJinJieFu(horseData.HorseID, out horseUpgradeGoodsID, out horseUpgradeGoodsNum, horseUpXmlNode);
					if (horseUpgradeGoodsID <= 0)
					{
						result = -20;
					}
					else if (horseData.HorseID >= Global.MaxHorseID)
					{
						result = -30;
					}
					else
					{
						int needYinLiang = Global.GMax(horseUpXmlNode.GetIntValue("UseYinLiang", -1), 0);
						needYinLiang = Global.RecalcNeedYinLiang(needYinLiang);
						if (client.ClientData.YinLiang < needYinLiang)
						{
							result = -60;
						}
						else
						{
							int needBuyGoodsNum = 0;
							int needGoodsNum = Global.GMax(horseUpgradeGoodsNum, 0);
							int needSubGoodsNum = needGoodsNum;
							if (Global.GetTotalGoodsCountByID(client, horseUpgradeGoodsID) < needGoodsNum)
							{
								if (!allowAutoBuy)
								{
									return -70;
								}
								needSubGoodsNum = Global.GetTotalGoodsCountByID(client, horseUpgradeGoodsID);
								needBuyGoodsNum = needGoodsNum - needSubGoodsNum;
							}
							bool usedBinding = false;
							bool usedTimeLimited = false;
							if (needSubGoodsNum > 0)
							{
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, horseUpgradeGoodsID, needSubGoodsNum, false, out usedBinding, out usedTimeLimited, false))
								{
									return -70;
								}
							}
							if (needBuyGoodsNum > 0)
							{
								int ret = Global.SubUserMoneyForGoods(client, horseUpgradeGoodsID, needBuyGoodsNum, "坐骑进阶");
								if (ret <= 0)
								{
									return ret;
								}
							}
							if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needYinLiang, "坐骑进阶", false))
							{
								result = -60;
							}
							else
							{
								int horseOne = 110000 - horseUpXmlNode.GetIntValue("HorseOne", -1);
								int horseTwo = 110000 - horseUpXmlNode.GetIntValue("HorseTwo", -1);
								double horseThree = horseUpXmlNode.GetDoubleValue("HorseThree");
								int jinJieFailedNum = Global.GetHorseFailedNum(horseData);
								if (jinJieFailedNum < horseTwo)
								{
									Global.AddHorseFailedNum(horseData, 1);
									Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID);
									Global.AddRoleHorseUpgradeEvent(client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID, "失败");
									result = -1000;
								}
								else
								{
									if (jinJieFailedNum < horseOne - 1)
									{
										int successRate = (int)(horseThree * 10000.0);
										int randNum = Global.GetRandomNumber(1, 10001);
										if (client.ClientData.TempHorseUpLevelRate != 1)
										{
											successRate *= client.ClientData.TempHorseUpLevelRate;
											successRate = Global.GMin(10000, successRate);
										}
										if (randNum > successRate)
										{
											Global.AddHorseFailedNum(horseData, 1);
											Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID);
											Global.AddRoleHorseUpgradeEvent(client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID, "失败");
											return -1000;
										}
									}
									result = ProcessHorse.ProcessHorseUpgradeNow(client, horseDbID, horseData);
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600311D RID: 12573 RVA: 0x002B86F0 File Offset: 0x002B68F0
		private static int ProcessHorseUpgradeNow(GameClient client, int horseDbID, HorseData horseData)
		{
			if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
			{
				Global.UpdateHorseDataProps(client, false);
			}
			int oldHorseID = horseData.HorseID;
			int newHorseID = horseData.HorseID + 1;
			Global.AddHorseTempJiFen(horseData, 0);
			horseData.JinJieFailedDayID = TimeUtil.NowDateTime().DayOfYear;
			horseData.JinJieFailedNum = 0;
			int ret = 0;
			if (Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseData.DbID, newHorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID) < 0)
			{
				ret = -2000;
			}
			Global.AddRoleHorseUpgradeEvent(client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID, "成功");
			if (client.ClientData.HorseDbID > 0 && horseDbID == client.ClientData.HorseDbID)
			{
				Global.UpdateHorseDataProps(client, true);
				if (0 == ret)
				{
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
				if (0 == ret)
				{
					client.ClientData.RoleHorseJiFen = Global.CalcHorsePropsJiFen(horseData);
					List<object> objsList = Global.GetAll9Clients(client);
					GameManager.ClientMgr.NotifyHorseCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 1, horseDbID, horseData.HorseID, horseData.BodyID, objsList);
					Global.BroadcastHorseUpgradeOk(client, oldHorseID, newHorseID);
				}
			}
			return 0;
		}

		// Token: 0x0600311E RID: 12574 RVA: 0x002B88CC File Offset: 0x002B6ACC
		public static int GetCurrentHorseBlessPoint(GameClient client)
		{
			int horseDbID = client.ClientData.HorseDbID;
			int result;
			if (horseDbID <= 0)
			{
				result = 0;
			}
			else
			{
				HorseData horseData = Global.GetHorseDataByDbID(client, horseDbID);
				if (null == horseData)
				{
					result = 0;
				}
				else
				{
					SystemXmlItem horseUpXmlNode = Global.GetHorseUpXmlNode(horseData.HorseID + 1);
					if (null == horseUpXmlNode)
					{
						result = 0;
					}
					else
					{
						int horseBlessPoint = horseUpXmlNode.GetIntValue("BlessPoint", -1);
						result = horseBlessPoint;
					}
				}
			}
			return result;
		}

		// Token: 0x0600311F RID: 12575 RVA: 0x002B8948 File Offset: 0x002B6B48
		public static int ProcessAddHorseAwardLucky(GameClient client, int luckyValue, bool usedTimeLimited, string getType)
		{
			int result;
			if (0 == luckyValue)
			{
				result = 0;
			}
			else
			{
				int horseDbID = client.ClientData.HorseDbID;
				if (horseDbID <= 0)
				{
					result = -300;
				}
				else
				{
					HorseData horseData = Global.GetHorseDataByDbID(client, horseDbID);
					if (null == horseData)
					{
						result = -1;
					}
					else
					{
						SystemXmlItem horseUpXmlNode = Global.GetHorseUpXmlNode(horseData.HorseID + 1);
						if (null == horseUpXmlNode)
						{
							result = -35;
						}
						else
						{
							int horseBlessPoint = horseUpXmlNode.GetIntValue("BlessPoint", -1);
							int jinJieFailedNum = Global.GetHorseFailedNum(horseData);
							if (horseData.HorseID >= Global.MaxHorseID)
							{
								result = -10;
							}
							else
							{
								int addLuckValue = Global.GMin(luckyValue, horseBlessPoint - jinJieFailedNum);
								addLuckValue = Global.GMax(0, addLuckValue);
								if (!usedTimeLimited)
								{
									Global.AddHorseFailedNum(horseData, addLuckValue);
								}
								else
								{
									Global.AddHorseTempJiFen(horseData, addLuckValue);
								}
								Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID);
								Global.AddRoleHorseUpgradeEvent(client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID, getType);
								result = addLuckValue;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06003120 RID: 12576 RVA: 0x002B8AA8 File Offset: 0x002B6CA8
		public static int ProcessAddHorseLucky(GameClient client, int horseDbID, int luckyGoodsID)
		{
			HorseData horseData = Global.GetHorseDataByDbID(client, horseDbID);
			int result;
			if (null == horseData)
			{
				result = -1;
			}
			else
			{
				SystemXmlItem horseUpXmlNode = Global.GetHorseUpXmlNode(horseData.HorseID + 1);
				if (null == horseUpXmlNode)
				{
					result = -35;
				}
				else
				{
					int horseOne = 110000 - horseUpXmlNode.GetIntValue("HorseOne", -1);
					int horseTwo = 110000 - horseUpXmlNode.GetIntValue("HorseTwo", -1);
					int jinJieFailedNum = Global.GetHorseFailedNum(horseData);
					if (jinJieFailedNum >= horseOne - 1)
					{
						result = -100;
					}
					else
					{
						int[] allHorseLuckyGoodsIDs = GameManager.systemParamsList.GetParamValueIntArrayByName("AllHorseLuckyGoodsIDs", ',');
						int[] allHorseLuckyGoodsIDsToLucky = GameManager.systemParamsList.GetParamValueIntArrayByName("AllHorseLuckyGoodsIDsToLucky", ',');
						if (allHorseLuckyGoodsIDs == null || allHorseLuckyGoodsIDsToLucky == null || allHorseLuckyGoodsIDs.Length != allHorseLuckyGoodsIDsToLucky.Length)
						{
							result = -2;
						}
						else if (horseData.HorseID >= Global.MaxHorseID)
						{
							result = -10;
						}
						else if (Global.GetTotalGoodsCountByID(client, luckyGoodsID) <= 0)
						{
							result = -20;
						}
						else
						{
							bool usedBinding = false;
							bool usedTimeLimited = false;
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, luckyGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
							{
								result = -30;
							}
							else
							{
								int addLuckValue = 0;
								for (int i = 0; i < allHorseLuckyGoodsIDs.Length; i++)
								{
									if (allHorseLuckyGoodsIDs[i] == luckyGoodsID)
									{
										addLuckValue = allHorseLuckyGoodsIDsToLucky[i];
										break;
									}
								}
								addLuckValue = Global.GMax(0, addLuckValue);
								Global.AddHorseFailedNum(horseData, addLuckValue);
								Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID);
								Global.AddRoleHorseUpgradeEvent(client, horseData.DbID, horseData.HorseID, horseData.JinJieFailedNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID, "祝福丹");
								result = addLuckValue;
							}
						}
					}
				}
			}
			return result;
		}
	}
}
