using System;
using System.Collections.Generic;
using LogDBServer.Logic;
using Server.Tools;

namespace LogDBServer.Server
{
	// Token: 0x0200002B RID: 43
	public class TCPCmdDispatcher
	{
		// Token: 0x060000EA RID: 234 RVA: 0x0000653C File Offset: 0x0000473C
		private TCPCmdDispatcher()
		{
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00006554 File Offset: 0x00004754
		public static TCPCmdDispatcher getInstance()
		{
			return TCPCmdDispatcher.instance;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000656B File Offset: 0x0000476B
		public void initialize()
		{
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000656E File Offset: 0x0000476E
		public void registerProcessor(int cmdId, ICmdProcessor processor)
		{
			this.cmdProcesserMapping.Add(cmdId, processor);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006580 File Offset: 0x00004780
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

		// Token: 0x04000065 RID: 101
		private static readonly TCPCmdDispatcher instance = new TCPCmdDispatcher();

		// Token: 0x04000066 RID: 102
		private Dictionary<int, ICmdProcessor> cmdProcesserMapping = new Dictionary<int, ICmdProcessor>();
	}
}
