using System;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008B0 RID: 2224
	public class ZhanMengBuildUpLevelCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003DB0 RID: 15792 RVA: 0x0034BAB8 File Offset: 0x00349CB8
		private ZhanMengBuildUpLevelCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(601, 4, this);
		}

		// Token: 0x06003DB1 RID: 15793 RVA: 0x0034BAD8 File Offset: 0x00349CD8
		public static ZhanMengBuildUpLevelCmdProcessor getInstance()
		{
			return ZhanMengBuildUpLevelCmdProcessor.instance;
		}

		// Token: 0x06003DB2 RID: 15794 RVA: 0x0034BAF0 File Offset: 0x00349CF0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int roleID = Global.SafeConvertToInt32(cmdParams[0]);
			int bhid = Global.SafeConvertToInt32(cmdParams[1]);
			int buildType = Global.SafeConvertToInt32(cmdParams[2]);
			int toLevel = Math.Max(2, Global.SafeConvertToInt32(cmdParams[3]));
			int nID = 601;
			bool result;
			if (client.ClientData.Faction != bhid)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-1,
					roleID,
					bhid,
					buildType,
					0
				});
				client.sendCmd(nID, strcmd, false);
				result = true;
			}
			else if (toLevel > Global.MaxBangHuiFlagLevel)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-2,
					roleID,
					bhid,
					buildType,
					0
				});
				client.sendCmd(nID, strcmd, false);
				result = true;
			}
			else
			{
				SystemXmlItem systemZhanMengBuildItem = Global.GetZhanMengBuildItem(buildType, toLevel);
				if (null == systemZhanMengBuildItem)
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-3,
						roleID,
						bhid,
						buildType,
						0
					});
					client.sendCmd(nID, strcmd, false);
					result = true;
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, 0);
					int oldBanHuiMoney = bangHuiDetailData.TotalMoney;
					int levelupCost = systemZhanMengBuildItem.GetIntValue("LevelupCost", -1);
					string strReqGoods = systemZhanMengBuildItem.GetStringValue("NeedGoods");
					string dbcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
					{
						roleID,
						bhid,
						buildType,
						levelupCost,
						toLevel,
						Global.GetZhanMengInitCoin(),
						strReqGoods
					});
					string[] fields = Global.ExecuteDBCmd(nID, dbcmd, client.ServerId);
					if (fields == null || fields.Length != 1)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("升级帮旗等级时失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), roleID), null, true);
						result = false;
					}
					else
					{
						int retCode = Global.SafeConvertToInt32(fields[0]);
						string strcmd;
						if (retCode >= 0)
						{
							Global.BroadcastZhanMengBuildUpLevelHint(client, buildType, toLevel);
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								toLevel,
								roleID,
								bhid,
								buildType,
								levelupCost
							});
							JunQiManager.NotifySyncBangHuiJunQiItemsDict(client);
							GameManager.ClientMgr.NotifyBangHuiUpLevel(bhid, client.ServerId, toLevel, client.ClientSocket.IsKuaFuLogin);
							BangHuiDetailData bangHuiDetailDataNew = Global.GetBangHuiDetailData(roleID, bhid, 0);
							int newBanHuiMoney = bangHuiDetailDataNew.TotalMoney;
							string strCostList = EventLogManager.NewResPropString(ResLogType.BangHuiMoney, new object[]
							{
								-levelupCost,
								oldBanHuiMoney,
								newBanHuiMoney
							});
							EventLogManager.AddBangHuiBuildUpEvent(client, bhid, buildType, toLevel, strCostList);
						}
						else
						{
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								retCode,
								roleID,
								bhid,
								buildType,
								0
							});
						}
						client.sendCmd(nID, strcmd, false);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x040047C1 RID: 18369
		private static ZhanMengBuildUpLevelCmdProcessor instance = new ZhanMengBuildUpLevelCmdProcessor();
	}
}
