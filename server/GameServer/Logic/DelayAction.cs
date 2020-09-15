using System;

namespace GameServer.Logic
{
	// Token: 0x02000624 RID: 1572
	internal class DelayAction
	{
		// Token: 0x04002CD4 RID: 11476
		public GameClient m_Client = null;

		// Token: 0x04002CD5 RID: 11477
		public long m_StartTime = 0L;

		// Token: 0x04002CD6 RID: 11478
		public long m_DelayTime = 0L;

		// Token: 0x04002CD7 RID: 11479
		public int[] m_Params = new int[2];

		// Token: 0x04002CD8 RID: 11480
		public DelayActionType m_DelayActionType = DelayActionType.DB_NULL;
	}
}
