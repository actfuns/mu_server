using System;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001CA RID: 458
	public class SpecActLimitData
	{
		// Token: 0x060005B2 RID: 1458 RVA: 0x000500D0 File Offset: 0x0004E2D0
		public bool IfValid()
		{
			return this.MinFirst > 0 || this.MaxFirst > 0 || this.MinSecond > 0 || this.MaxSecond > 0;
		}

		// Token: 0x04000A26 RID: 2598
		public int MinFirst = -1;

		// Token: 0x04000A27 RID: 2599
		public int MaxFirst = -1;

		// Token: 0x04000A28 RID: 2600
		public int MinSecond = -1;

		// Token: 0x04000A29 RID: 2601
		public int MaxSecond = -1;
	}
}
