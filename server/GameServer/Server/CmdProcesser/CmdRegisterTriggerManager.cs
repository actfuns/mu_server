using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x02000899 RID: 2201
	public class CmdRegisterTriggerManager : IManager
	{
		// Token: 0x06003D41 RID: 15681 RVA: 0x00344A82 File Offset: 0x00342C82
		private CmdRegisterTriggerManager()
		{
		}

		// Token: 0x06003D42 RID: 15682 RVA: 0x00344A98 File Offset: 0x00342C98
		public static CmdRegisterTriggerManager getInstance()
		{
			return CmdRegisterTriggerManager.instance;
		}

		// Token: 0x06003D43 RID: 15683 RVA: 0x00344AAF File Offset: 0x00342CAF
		private void TriggerProcessor(ICmdProcessor icp)
		{
		}

		// Token: 0x06003D44 RID: 15684 RVA: 0x00344AB4 File Offset: 0x00342CB4
		public bool initialize()
		{
			this.TriggerProcessor(ZhanMengBuildUpLevelCmdProcessor.getInstance());
			this.TriggerProcessor(ZhanMengBuildGetBufferCmdProcessor.getInstance());
			this.TriggerProcessor(WingOffOnCmdProcessor.getInstance());
			this.TriggerProcessor(WingUpgradeCmdProcessor.getInstance());
			this.TriggerProcessor(WingUpStarCmdProcessor.getInstance());
			this.TriggerProcessor(GetWingInfoCmdProcessor.getInstance());
			this.TriggerProcessor(GetSweepWanMoTaAwardCmdProcessor.getInstance());
			this.TriggerProcessor(GetWanMoTaDetailCmdProcessor.getInstance());
			this.TriggerProcessor(SweepWanMoTaCmdProcessor.getInstance());
			return true;
		}

		// Token: 0x06003D45 RID: 15685 RVA: 0x00344B34 File Offset: 0x00342D34
		public bool startup()
		{
			return true;
		}

		// Token: 0x06003D46 RID: 15686 RVA: 0x00344B48 File Offset: 0x00342D48
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06003D47 RID: 15687 RVA: 0x00344B5C File Offset: 0x00342D5C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x040047A8 RID: 18344
		private static CmdRegisterTriggerManager instance = new CmdRegisterTriggerManager();

		// Token: 0x040047A9 RID: 18345
		private List<ICmdProcessor> CmdProcessorList = new List<ICmdProcessor>();
	}
}
