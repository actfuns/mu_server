using System;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x0200072C RID: 1836
	public enum JingJiFuBenState
	{
		// Token: 0x04003B28 RID: 15144
		INITIALIZED,
		// Token: 0x04003B29 RID: 15145
		WAITING_CHANGEMAP_FINISH,
		// Token: 0x04003B2A RID: 15146
		START_CD,
		// Token: 0x04003B2B RID: 15147
		STARTED,
		// Token: 0x04003B2C RID: 15148
		STOP_CD,
		// Token: 0x04003B2D RID: 15149
		STOP_TIMEOUT_CD,
		// Token: 0x04003B2E RID: 15150
		STOPED,
		// Token: 0x04003B2F RID: 15151
		DESTROYED
	}
}
