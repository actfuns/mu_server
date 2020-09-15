using System;
using System.Collections.Generic;
using GameDBServer.Logic;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001EC RID: 492
	public class CmdRegisterTriggerManager : IManager
	{
		// Token: 0x06000A43 RID: 2627 RVA: 0x00061EB6 File Offset: 0x000600B6
		private CmdRegisterTriggerManager()
		{
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x00061ECC File Offset: 0x000600CC
		public static CmdRegisterTriggerManager getInstance()
		{
			return CmdRegisterTriggerManager.instance;
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00061EE3 File Offset: 0x000600E3
		private void TriggerProcessor(ICmdProcessor icp)
		{
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x00061EE8 File Offset: 0x000600E8
		public bool initialize()
		{
			this.TriggerProcessor(ZhanMengBuildUpLevelCmdProcessor.getInstance());
			this.TriggerProcessor(ZhanMengBuildGetBufferCmdProcessor.getInstance());
			this.TriggerProcessor(BaiTanLogAddCmdProcessor.getInstance());
			this.TriggerProcessor(BaiTanLogDetailCmdProcessor.getInstance());
			return true;
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x00061F2C File Offset: 0x0006012C
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x00061F40 File Offset: 0x00060140
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x00061F54 File Offset: 0x00060154
		public bool destroy()
		{
			return true;
		}

		// Token: 0x04000C4D RID: 3149
		private static CmdRegisterTriggerManager instance = new CmdRegisterTriggerManager();

		// Token: 0x04000C4E RID: 3150
		private List<ICmdProcessor> CmdProcessorList = new List<ICmdProcessor>();
	}
}
