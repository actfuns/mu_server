using System;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008AF RID: 2223
	public class ZhanMengBuildGetBufferCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003DAB RID: 15787 RVA: 0x0034B684 File Offset: 0x00349884
		private ZhanMengBuildGetBufferCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(602, 4, this);
		}

		// Token: 0x06003DAC RID: 15788 RVA: 0x0034B6A4 File Offset: 0x003498A4
		public static ZhanMengBuildGetBufferCmdProcessor getInstance()
		{
			return ZhanMengBuildGetBufferCmdProcessor.instance;
		}

		// Token: 0x06003DAD RID: 15789 RVA: 0x0034B6BC File Offset: 0x003498BC
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

		// Token: 0x06003DAE RID: 15790 RVA: 0x0034BA0C File Offset: 0x00349C0C
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

		// Token: 0x040047C0 RID: 18368
		private static ZhanMengBuildGetBufferCmdProcessor instance = new ZhanMengBuildGetBufferCmdProcessor();
	}
}
