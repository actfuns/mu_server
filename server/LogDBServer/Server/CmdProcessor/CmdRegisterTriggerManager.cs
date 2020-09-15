using System;
using System.Collections.Generic;
using LogDBServer.Logic;

namespace LogDBServer.Server.CmdProcessor
{
	// Token: 0x02000025 RID: 37
	public class CmdRegisterTriggerManager : IManager
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00006060 File Offset: 0x00004260
		private CmdRegisterTriggerManager()
		{
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00006078 File Offset: 0x00004278
		public static CmdRegisterTriggerManager getInstance()
		{
			return CmdRegisterTriggerManager.instance;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000608F File Offset: 0x0000428F
		private void TriggerProcessor(ICmdProcessor icp)
		{
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00006094 File Offset: 0x00004294
		public bool initialize()
		{
			this.TriggerProcessor(AddItemLogCmdProcessor.getInstance());
			this.TriggerProcessor(TradeMoneyFreqLogCmdProcessor.getInstance());
			this.TriggerProcessor(TradeMoneyNumLogCmdProcessor.getInstance());
			return true;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000060CC File Offset: 0x000042CC
		public bool startup()
		{
			return true;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000060E0 File Offset: 0x000042E0
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000060F4 File Offset: 0x000042F4
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0400005F RID: 95
		private static CmdRegisterTriggerManager instance = new CmdRegisterTriggerManager();

		// Token: 0x04000060 RID: 96
		private List<ICmdProcessor> CmdProcessorList = new List<ICmdProcessor>();
	}
}
