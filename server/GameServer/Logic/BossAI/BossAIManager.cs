using System;
using GameServer.Core.GameEvent;

namespace GameServer.Logic.BossAI
{
	// Token: 0x020005D7 RID: 1495
	public class BossAIManager : IManager
	{
		// Token: 0x06001BD4 RID: 7124 RVA: 0x001A2454 File Offset: 0x001A0654
		private BossAIManager()
		{
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x001A2460 File Offset: 0x001A0660
		public static BossAIManager getInstance()
		{
			return BossAIManager.instance;
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x001A2478 File Offset: 0x001A0678
		public bool initialize()
		{
			GlobalEventSource.getInstance().registerListener(16, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(11, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(17, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(19, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(18, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(20, BossAIEventListener.getInstance());
			return true;
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x001A24F8 File Offset: 0x001A06F8
		public bool startup()
		{
			return true;
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x001A250C File Offset: 0x001A070C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x001A2520 File Offset: 0x001A0720
		public bool destroy()
		{
			GlobalEventSource.getInstance().removeListener(16, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(11, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(17, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(19, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(18, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(20, BossAIEventListener.getInstance());
			return true;
		}

		// Token: 0x04002A11 RID: 10769
		private static BossAIManager instance = new BossAIManager();
	}
}
