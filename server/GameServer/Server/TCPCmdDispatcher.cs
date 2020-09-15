using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Server
{
	// Token: 0x020008C1 RID: 2241
	public class TCPCmdDispatcher
	{
		// Token: 0x06003FBC RID: 16316 RVA: 0x003B8923 File Offset: 0x003B6B23
		private TCPCmdDispatcher()
		{
		}

		// Token: 0x06003FBD RID: 16317 RVA: 0x003B893C File Offset: 0x003B6B3C
		public static TCPCmdDispatcher getInstance()
		{
			return TCPCmdDispatcher.instance;
		}

		// Token: 0x06003FBE RID: 16318 RVA: 0x003B8953 File Offset: 0x003B6B53
		public void initialize()
		{
		}

		// Token: 0x06003FBF RID: 16319 RVA: 0x003B8956 File Offset: 0x003B6B56
		public void registerProcessor(int cmdId, short paramNum, ICmdProcessor processor)
		{
			this.registerProcessorEx(cmdId, paramNum, paramNum, processor, TCPCmdFlags.IsStringArrayParams);
		}

		// Token: 0x06003FC0 RID: 16320 RVA: 0x003B8968 File Offset: 0x003B6B68
		public void registerProcessorEx(int cmdId, short minParamCount, short maxParamCount, ICmdProcessor processor, TCPCmdFlags cmdFlags = TCPCmdFlags.IsStringArrayParams)
		{
			Debug.Assert(processor != null);
			CmdHandler cmdHandler = new CmdHandler
			{
				CmdFlags = (uint)cmdFlags,
				MinParamCount = minParamCount,
				MaxParamCount = maxParamCount,
				CmdProcessor = processor
			};
			this.cmdProcesserMapping.Add(cmdId, cmdHandler);
		}

		// Token: 0x06003FC1 RID: 16321 RVA: 0x003B89B8 File Offset: 0x003B6BB8
		public void registerStreamProcessorEx(int cmdId, ICmdProcessor processor)
		{
			this.registerProcessorEx(cmdId, -1, -1, processor, TCPCmdFlags.IsBinaryStreamParams);
		}

		// Token: 0x06003FC2 RID: 16322 RVA: 0x003B89C8 File Offset: 0x003B6BC8
		private CmdHandler GetCmdHandler(int cmdID)
		{
			CmdHandler cmdHandler;
			CmdHandler result;
			if (this.cmdProcesserMapping.TryGetValue(cmdID, out cmdHandler))
			{
				result = cmdHandler;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06003FC3 RID: 16323 RVA: 0x003B89F8 File Offset: 0x003B6BF8
		public TCPProcessCmdResults transmission(TMSKSocket socket, int nID, byte[] data, int count)
		{
			try
			{
				string cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				byte[] bytesData = Global.SendAndRecvData<byte[]>(nID, data, socket.ServerId, 0);
				if (null == bytesData)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("与DBServer通讯失败, CMD={0}", (TCPGameServerCmds)nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int length = BitConverter.ToInt32(bytesData, 0);
				ushort cmd = BitConverter.ToUInt16(bytesData, 4);
				TCPOutPacket tcpOutPacket = TCPOutPacketPool.getInstance().Pop();
				tcpOutPacket.PacketCmdID = cmd;
				tcpOutPacket.FinalWriteData(bytesData, 6, length - 2);
				TCPManager.getInstance().MySocketListener.SendData(socket, tcpOutPacket, true);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06003FC4 RID: 16324 RVA: 0x003B8B08 File Offset: 0x003B6D08
		public TCPProcessCmdResults dispathProcessor(TMSKSocket socket, int nID, byte[] data, int count)
		{
			try
			{
				CmdHandler tcpCmdHandler;
				if ((tcpCmdHandler = this.GetCmdHandler(nID)) == null)
				{
					return TCPProcessCmdResults.RESULT_UNREGISTERED;
				}
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (null == client)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				ICmdProcessorEx cmdProcessorEx = tcpCmdHandler.CmdProcessor as ICmdProcessorEx;
				if ((tcpCmdHandler.CmdFlags & 2U) > 0U)
				{
					string cmdData = new UTF8Encoding().GetString(data, 0, count);
					string[] cmdParams = cmdData.Split(new char[]
					{
						':'
					});
					if (cmdParams.Length < (int)tcpCmdHandler.MinParamCount || cmdParams.Length > (int)tcpCmdHandler.MaxParamCount)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), cmdParams.Length), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					if (null == cmdProcessorEx)
					{
						if (!tcpCmdHandler.CmdProcessor.processCmd(client, cmdParams))
						{
							return TCPProcessCmdResults.RESULT_FAILED;
						}
					}
					else if (!cmdProcessorEx.processCmdEx(client, nID, data, cmdParams))
					{
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					return TCPProcessCmdResults.RESULT_OK;
				}
				else
				{
					if (null == cmdProcessorEx)
					{
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					if (!cmdProcessorEx.processCmdEx(client, nID, data, null))
					{
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					return TCPProcessCmdResults.RESULT_OK;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x04004EE8 RID: 20200
		private static readonly TCPCmdDispatcher instance = new TCPCmdDispatcher();

		// Token: 0x04004EE9 RID: 20201
		private Dictionary<int, CmdHandler> cmdProcesserMapping = new Dictionary<int, CmdHandler>();
	}
}
