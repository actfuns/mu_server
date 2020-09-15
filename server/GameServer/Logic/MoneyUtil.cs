using System;
using GameServer.Logic.TuJian;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	// Token: 0x020002E3 RID: 739
	public class MoneyUtil
	{
		// Token: 0x06000BD0 RID: 3024 RVA: 0x000B82B4 File Offset: 0x000B64B4
		public static bool CheckHasMoney(GameClient client, int moneyType, int modifyValue)
		{
			if (moneyType <= 40)
			{
				if (moneyType <= 8)
				{
					if (moneyType == 1)
					{
						return modifyValue <= client.ClientData.Money1 + client.ClientData.YinLiang;
					}
					if (moneyType == 8)
					{
						return modifyValue <= client.ClientData.YinLiang;
					}
				}
				else
				{
					switch (moneyType)
					{
					case 13:
						return modifyValue <= GameManager.ClientMgr.GetTianDiJingYuanValue(client);
					case 14:
						break;
					case 15:
						return modifyValue <= Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen");
					default:
						if (moneyType == 40)
						{
							return modifyValue <= client.ClientData.UserMoney;
						}
						break;
					}
				}
			}
			else if (moneyType <= 109)
			{
				if (moneyType == 50)
				{
					return modifyValue <= client.ClientData.Gold;
				}
				switch (moneyType)
				{
				case 99:
					return modifyValue <= Global.GetRoleParamsInt32FromDB(client, "LangHunFenMo");
				case 101:
					return modifyValue <= GameManager.ClientMgr.GetZaiZaoValue(client);
				case 106:
					return modifyValue <= GameManager.ClientMgr.GetMUMoHeValue(client);
				case 107:
					return modifyValue <= Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				case 108:
					return modifyValue <= client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint;
				case 109:
					return modifyValue <= client.ClientData.FluorescentPoint;
				}
			}
			else
			{
				switch (moneyType)
				{
				case 119:
					return modifyValue <= Global.GetRoleParamsInt32FromDB(client, "10153");
				case 120:
					return modifyValue <= Global.GetRoleParamsInt32FromDB(client, "10157");
				default:
					switch (moneyType)
					{
					case 129:
						return modifyValue <= Global.GetRoleParamsInt32FromDB(client, "10187");
					case 130:
						return modifyValue <= client.ClientData.AlchemyInfo.BaseData.Element;
					default:
						if (moneyType == 163)
						{
							return modifyValue <= client.ClientData.LuckStar;
						}
						break;
					}
					break;
				}
			}
			return (long)modifyValue <= client.ClientData.MoneyData[moneyType];
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x000B8534 File Offset: 0x000B6734
		public static bool CostMoney(GameClient client, int moneyType, int modifyValue, ref string strCostList, string logMsg, bool consume = true)
		{
			bool subRes = false;
			if (moneyType <= 50)
			{
				if (moneyType <= 8)
				{
					if (moneyType == 1)
					{
						int useJinBi = modifyValue;
						int useYinLiang = 0;
						if (modifyValue > client.ClientData.Money1)
						{
							useJinBi = client.ClientData.Money1;
							useYinLiang = modifyValue - useJinBi;
						}
						subRes = GameManager.ClientMgr.SubMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, useJinBi, logMsg);
						if (subRes)
						{
							strCostList += EventLogManager.AddResPropString(strCostList, (ResLogType)moneyType, new object[]
							{
								-useJinBi
							});
							if (useYinLiang > 0)
							{
								subRes = GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, useYinLiang, logMsg, false);
								if (subRes)
								{
									strCostList += EventLogManager.AddResPropString(strCostList, ResLogType.ZuanShi, new object[]
									{
										-useYinLiang
									});
								}
							}
						}
						return subRes;
					}
					if (moneyType == 8)
					{
						subRes = GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, false);
						goto IL_425;
					}
				}
				else
				{
					switch (moneyType)
					{
					case 13:
						subRes = GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_425;
					case 14:
						break;
					case 15:
						subRes = Global.AddZaJinDanJiFen(client, -modifyValue, logMsg, false);
						goto IL_425;
					default:
						if (moneyType == 40)
						{
							subRes = GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, true, true, false, DaiBiSySType.None);
							goto IL_425;
						}
						if (moneyType == 50)
						{
							subRes = GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, false);
							goto IL_425;
						}
						break;
					}
				}
			}
			else if (moneyType <= 120)
			{
				switch (moneyType)
				{
				case 99:
					subRes = true;
					GameManager.ClientMgr.ModifyLangHunFenMoValue(client, -modifyValue, logMsg, true, true);
					goto IL_425;
				case 100:
				case 102:
				case 103:
				case 104:
				case 105:
					break;
				case 101:
					subRes = GameManager.ClientMgr.ModifyZaiZaoValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_425;
				case 106:
					subRes = GameManager.ClientMgr.ModifyMUMoHeValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_425;
				case 107:
					subRes = GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, -modifyValue, logMsg, true, false);
					goto IL_425;
				case 108:
					subRes = true;
					SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(client, -modifyValue, logMsg);
					goto IL_425;
				case 109:
					subRes = GameManager.FluorescentGemMgr.DecFluorescentPoint(client, modifyValue, logMsg, false);
					goto IL_425;
				default:
					switch (moneyType)
					{
					case 119:
						subRes = GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_425;
					case 120:
						subRes = true;
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, -modifyValue, logMsg, true, true);
						goto IL_425;
					}
					break;
				}
			}
			else
			{
				switch (moneyType)
				{
				case 129:
					subRes = GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_425;
				case 130:
					subRes = GameManager.ClientMgr.ModifyAlchemyElementValue(client, -modifyValue, logMsg, true, false);
					goto IL_425;
				case 131:
					break;
				case 132:
					subRes = GameManager.ClientMgr.ModifyJueXingValue(client, -modifyValue, logMsg, false);
					goto IL_425;
				case 133:
					subRes = GameManager.ClientMgr.ModifyJueXingZhiChenValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_425;
				default:
					switch (moneyType)
					{
					case 139:
						subRes = GameManager.ClientMgr.ModifyHunJingValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_425;
					case 140:
						subRes = GameManager.ClientMgr.ModifyMountPointValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_425;
					default:
						if (moneyType == 163)
						{
							subRes = GameManager.ClientMgr.ModifyLuckStarValue(client, -modifyValue, logMsg, false, DaiBiSySType.None);
							goto IL_425;
						}
						break;
					}
					break;
				}
			}
			LogManager.WriteLog(LogTypes.Error, " CheckHasMoney 不支持的货币类型:" + moneyType, null, true);
			IL_425:
			if (subRes)
			{
				strCostList += EventLogManager.AddResPropString(strCostList, (ResLogType)moneyType, new object[]
				{
					-modifyValue
				});
			}
			return subRes;
		}
	}
}
