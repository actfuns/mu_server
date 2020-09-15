using System;
using GameServer.Logic.TuJian;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	// Token: 0x020001DF RID: 479
	public class OccupationUtil
	{
		// Token: 0x0600060E RID: 1550 RVA: 0x00055368 File Offset: 0x00053568
		public static int GetOccuDamageType(int occuationIndex)
		{
			int result;
			switch (occuationIndex)
			{
			case 0:
			case 2:
			case 3:
				result = 0;
				break;
			case 1:
			case 4:
			case 5:
				result = 1;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x000553A8 File Offset: 0x000535A8
		public static bool CostMoney(GameClient client, int moneyType, int modifyValue, ref string strCostList, string logMsg)
		{
			bool subRes = false;
			if (moneyType <= 40)
			{
				if (moneyType <= 8)
				{
					if (moneyType == 1)
					{
						subRes = GameManager.ClientMgr.SubMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg);
						goto IL_321;
					}
					if (moneyType == 8)
					{
						subRes = GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, false);
						goto IL_321;
					}
				}
				else
				{
					switch (moneyType)
					{
					case 13:
						subRes = GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_321;
					case 14:
						break;
					case 15:
						subRes = Global.AddZaJinDanJiFen(client, -modifyValue, logMsg, false);
						goto IL_321;
					default:
						if (moneyType == 40)
						{
							subRes = GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, false, false, false, DaiBiSySType.None);
							goto IL_321;
						}
						break;
					}
				}
			}
			else if (moneyType <= 109)
			{
				if (moneyType == 50)
				{
					subRes = GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, false);
					goto IL_321;
				}
				switch (moneyType)
				{
				case 99:
					subRes = true;
					GameManager.ClientMgr.ModifyLangHunFenMoValue(client, -modifyValue, logMsg, true, true);
					goto IL_321;
				case 101:
					subRes = GameManager.ClientMgr.ModifyZaiZaoValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_321;
				case 106:
					subRes = GameManager.ClientMgr.ModifyMUMoHeValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_321;
				case 107:
					subRes = GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, -modifyValue, logMsg, true, false);
					goto IL_321;
				case 108:
					subRes = true;
					SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(client, -modifyValue, logMsg);
					goto IL_321;
				case 109:
					subRes = GameManager.FluorescentGemMgr.DecFluorescentPoint(client, modifyValue, logMsg, false);
					goto IL_321;
				}
			}
			else
			{
				switch (moneyType)
				{
				case 119:
					subRes = GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_321;
				case 120:
					subRes = true;
					GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, -modifyValue, logMsg, true, true);
					goto IL_321;
				default:
					switch (moneyType)
					{
					case 129:
						subRes = GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_321;
					case 130:
						subRes = GameManager.ClientMgr.ModifyAlchemyElementValue(client, -modifyValue, logMsg, true, false);
						goto IL_321;
					case 131:
						break;
					case 132:
						subRes = GameManager.ClientMgr.ModifyJueXingValue(client, -modifyValue, logMsg, false);
						goto IL_321;
					case 133:
						subRes = GameManager.ClientMgr.ModifyJueXingZhiChenValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_321;
					default:
						switch (moneyType)
						{
						case 139:
							subRes = GameManager.ClientMgr.ModifyHunJingValue(client, -modifyValue, logMsg, true, true, false);
							goto IL_321;
						case 140:
							subRes = GameManager.ClientMgr.ModifyMountPointValue(client, -modifyValue, logMsg, true, true, false);
							goto IL_321;
						}
						break;
					}
					break;
				}
			}
			LogManager.WriteLog(LogTypes.Error, " CheckHasMoney 不支持的货币类型:" + moneyType, null, true);
			IL_321:
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
