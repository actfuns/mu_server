using System;
using System.Collections.Generic;
using LogDBServer.Logic;
using Server.Tools;

namespace LogDBServer.Server
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

		
		public void registerProcessor(int cmdId, ICmdProcessor processor)
		{
			this.cmdProcesserMapping.Add(cmdId, processor);
		}

		
		public TCPProcessCmdResults dispathProcessor(GameServerClient client, int nID, byte[] data, int count)
		{
			try
			{
				ICmdProcessor cmdProcessor = null;
				if (!this.cmdProcesserMapping.TryGetValue(nID, out cmdProcessor))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("未注册指令, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.CurrentSocket)));
					client.sendCmd(32767, "0");
					return TCPProcessCmdResults.RESULT_DATA;
				}
				cmdProcessor.processCmd(client, data, count);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.CurrentSocket), false, false);
			}
			client.sendCmd(32767, "0");
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private static readonly TCPCmdDispatcher instance = new TCPCmdDispatcher();

		
		private Dictionary<int, ICmdProcessor> cmdProcesserMapping = new Dictionary<int, ICmdProcessor>();
	}
}
