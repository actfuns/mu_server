using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using GameDBServer.Server.CmdProcessor;
using Server.Tools;

namespace GameDBServer.Server
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
			this.registerProcessor(13300, UpdateBuildingDataCmdProcessor.getInstance());
			this.registerProcessor(13301, UpdateBuildingDataCmdProcessor.getInstance());
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
					LogManager.WriteLog(LogTypes.Error, string.Format("未注册指令, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.CurrentSocket)), null, true);
					client.sendCmd(30767, "0");
					return TCPProcessCmdResults.RESULT_DATA;
				}
				cmdProcessor.processCmd(client, nID, data, count);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.CurrentSocket), false, false);
			}
			client.sendCmd(30767, "0");
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private static readonly TCPCmdDispatcher instance = new TCPCmdDispatcher();

		
		private Dictionary<int, ICmdProcessor> cmdProcesserMapping = new Dictionary<int, ICmdProcessor>();
	}
}
