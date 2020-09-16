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
	
	public class TCPCmdDispatcher
	{
		
		private TCPCmdDispatcher()
		{
		}

		
		public static TCPCmdDispatcher getInstance()
		{
			return TCPCmdDispatcher.instance;
		}

		
		public void initialize()
		{
		}

		
		public void registerProcessor(int cmdId, short paramNum, ICmdProcessor processor)
		{
			this.registerProcessorEx(cmdId, paramNum, paramNum, processor, TCPCmdFlags.IsStringArrayParams);
		}

		
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

		
		public void registerStreamProcessorEx(int cmdId, ICmdProcessor processor)
		{
			this.registerProcessorEx(cmdId, -1, -1, processor, TCPCmdFlags.IsBinaryStreamParams);
		}

		
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

		
		private static readonly TCPCmdDispatcher instance = new TCPCmdDispatcher();

		
		private Dictionary<int, CmdHandler> cmdProcesserMapping = new Dictionary<int, CmdHandler>();
	}
}
