using System;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	
	public class ZhanMengBuildGetBufferCmdProcessor : ICmdProcessor
	{
		
		private ZhanMengBuildGetBufferCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(602, 4, this);
		}

		
		public static ZhanMengBuildGetBufferCmdProcessor getInstance()
		{
			return ZhanMengBuildGetBufferCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int roleID = Global.SafeConvertToInt32(cmdParams[0]);
			int bhid = Global.SafeConvertToInt32(cmdParams[1]);
			int buildType = Global.SafeConvertToInt32(cmdParams[2]);
			int level = Global.SafeConvertToInt32(cmdParams[3]);
			int nID = 602;
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
			else
			{
				SystemXmlItem systemZhanMengBuildItem = Global.GetZhanMengBuildItem(buildType, level);
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
					int buffTime = systemZhanMengBuildItem.GetIntValue("BuffTime", -1);
					int convertCost = systemZhanMengBuildItem.GetIntValue("ConvertCost", -1);
					if (client.ClientData.BangGong < convertCost)
					{
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1110,
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
						string dbcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							roleID,
							bhid,
							buildType,
							convertCost,
							level
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
							if (retCode > 0)
							{
								client.ClientData.BangGong -= Math.Abs(convertCost);
								this.installBuff(client, level - 1, buildType, buffTime);
								strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									1,
									roleID,
									bhid,
									buildType,
									convertCost
								});
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
			}
			return result;
		}

		
		private void installBuff(GameClient client, int nNewBufferGoodsIndexID, int buildType, int secs)
		{
			BufferData bufferData = Global.GetBufferDataByID(client, 88 + buildType - 1);
			double[] actionParams = new double[]
			{
				(double)secs,
				(double)nNewBufferGoodsIndexID
			};
			Global.UpdateBufferData(client, BufferItemTypes.MU_ZHANMENGBUILD_ZHANQI + buildType - 1, actionParams, 0, true);
			GameManager.ClientMgr.NotifySelfBangGongChange(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		
		private static ZhanMengBuildGetBufferCmdProcessor instance = new ZhanMengBuildGetBufferCmdProcessor();
	}
}
