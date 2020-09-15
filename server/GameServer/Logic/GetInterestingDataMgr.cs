using System;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	// Token: 0x02000247 RID: 583
	internal class GetInterestingDataMgr : SingletonTemplate<GetInterestingDataMgr>
	{
		// Token: 0x06000805 RID: 2053 RVA: 0x0007A189 File Offset: 0x00078389
		private GetInterestingDataMgr()
		{
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0007A194 File Offset: 0x00078394
		public void LoadConfig()
		{
			GetInterestingDataMgr.GetIntervalMs = GameManager.systemParamsList.GetParamValueIntByName("GetIntervalMs", -1);
			if (GetInterestingDataMgr.GetIntervalMs < 30000L)
			{
				GetInterestingDataMgr.GetIntervalMs = 180000L;
			}
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0007A1D8 File Offset: 0x000783D8
		public void Update(GameClient client)
		{
			if (GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("GetInterestingData"))
			{
				if (client != null)
				{
					int totalUnRspCount = 0;
					int totalInvalidCount = 0;
					long nowMs = TimeUtil.NOW();
					lock (client.InterestingData)
					{
						if (client.ClientData.FirstPlayStart)
						{
							return;
						}
						for (int i = 0; i < client.InterestingData.itemArray.Length; i++)
						{
							InterestingData.Item item = client.InterestingData.itemArray[i];
							if (item != null)
							{
								totalUnRspCount += item.RequestCount - item.ResponseCount;
								totalInvalidCount += item.InvalidCount;
								if (item.RequestCount >= 2 || item.LastRequestMs + GetInterestingDataMgr._FirstGetIntervalMs <= nowMs)
								{
									if (item.RequestCount < 2 || item.LastRequestMs + GetInterestingDataMgr.GetIntervalMs <= nowMs)
									{
										if (!client.ClientSocket.session.IsGM)
										{
											if (i == 1)
											{
												RobotTaskValidator.getInstance().SendTaskListKey(client);
											}
											client.sendCmd(14004, string.Format("{0}", i), false);
											item.LastRequestMs = nowMs;
											item.RequestCount++;
										}
									}
								}
							}
						}
					}
					if (totalUnRspCount > 10)
					{
					}
					if (totalInvalidCount > 10)
					{
					}
				}
			}
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0007A3C4 File Offset: 0x000785C4
		public TCPProcessCmdResults OnResponse(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				string[] fields = new UTF8Encoding().GetString(data, 0, count).Split(new char[]
				{
					':'
				});
				if (fields == null || fields.Length <= 2)
				{
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleId = Convert.ToInt32(fields[0]);
				int index = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleId))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleId), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (index < 0 || index >= 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色返回敏感数据索引错误,roleid={0}, rolename={1}, index={2}", roleId, client.ClientData.RoleName, index), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				lock (client.InterestingData)
				{
					InterestingData.Item item = client.InterestingData.itemArray[index];
					if (item != null)
					{
						item.LastResponseMs = TimeUtil.NOW();
						item.ResponseCount++;
						if (index == 0)
						{
							this._CheckSpeed(client, item, fields);
						}
					}
				}
			}
			catch
			{
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0007A58C File Offset: 0x0007878C
		private void _CheckSpeed(GameClient client, InterestingData.Item item, string[] fields)
		{
			double clientSpeed = Convert.ToDouble(fields[2]);
			client.InterestingData.Speed = clientSpeed;
			double calcSpeed = RoleAlgorithm.GetMoveSpeed(client);
			if (client.ClientData.HorseDbID > 0)
			{
				calcSpeed += Global.GetHorseSpeed(client);
			}
			if (clientSpeed > calcSpeed * 1.4)
			{
				item.InvalidCount++;
			}
		}

		// Token: 0x04000DE4 RID: 3556
		private static long GetIntervalMs = 180000L;

		// Token: 0x04000DE5 RID: 3557
		private static long _FirstGetIntervalMs = 30000L;
	}
}
