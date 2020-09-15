using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using GameDBServer.Server.CmdProcessor;
using Server.Tools;

namespace GameDBServer.Server
{
	// Token: 0x02000205 RID: 517
	public class TCPCmdDispatcher
	{
		// Token: 0x06000AB2 RID: 2738 RVA: 0x000660F4 File Offset: 0x000642F4
		private TCPCmdDispatcher()
		{
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x0006610C File Offset: 0x0006430C
		public static TCPCmdDispatcher getInstance()
		{
			return TCPCmdDispatcher.instance;
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x00066123 File Offset: 0x00064323
		public void initialize()
		{
			this.registerProcessor(13300, UpdateBuildingDataCmdProcessor.getInstance());
			this.registerProcessor(13301, UpdateBuildingDataCmdProcessor.getInstance());
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00066148 File Offset: 0x00064348
		public void registerProcessor(int cmdId, ICmdProcessor processor)
		{
			this.cmdProcesserMapping.Add(cmdId, processor);
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0006615C File Offset: 0x0006435C
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

		// Token: 0x04000C7E RID: 3198
		private static readonly TCPCmdDispatcher instance = new TCPCmdDispatcher();

		// Token: 0x04000C7F RID: 3199
		private Dictionary<int, ICmdProcessor> cmdProcesserMapping = new Dictionary<int, ICmdProcessor>();
	}
}
