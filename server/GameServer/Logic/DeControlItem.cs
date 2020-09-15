using System;

namespace GameServer.Logic
{
	// Token: 0x020001DA RID: 474
	public class DeControlItem
	{
		// Token: 0x04000A76 RID: 2678
		public int ExtPropIndex;

		// Token: 0x04000A77 RID: 2679
		public double DeControlPercent;

		// Token: 0x04000A78 RID: 2680
		public double DeControlTime;

		// Token: 0x04000A79 RID: 2681
		public double DurationTime;

		// Token: 0x04000A7A RID: 2682
		public DeControlItem Next;

		// Token: 0x04000A7B RID: 2683
		public DeControlItem Head;
	}
}
