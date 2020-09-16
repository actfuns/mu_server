using System;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	
	public class ZhanMengBuildUpLevelCmdProcessor : ICmdProcessor
	{
		
		private ZhanMengBuildUpLevelCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(601, 4, this);
		}

		
		public static ZhanMengBuildUpLevelCmdProcessor getInstance()
		{
			return ZhanMengBuildUpLevelCmdProcessor.instance;
		}

		
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

		
		private static ZhanMengBuildUpLevelCmdProcessor instance = new ZhanMengBuildUpLevelCmdProcessor();
	}
}
