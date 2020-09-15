using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020001ED RID: 493
	public class ExtData
	{
		// Token: 0x04000AD8 RID: 2776
		public int OffsetDay;

		// Token: 0x04000AD9 RID: 2777
		public long ZhanLiLogNextWriteTicks;

		// Token: 0x04000ADA RID: 2778
		public long LastZhanLi;

		// Token: 0x04000ADB RID: 2779
		public long ZhanLiWriteten;

		// Token: 0x04000ADC RID: 2780
		public HashSet<long> ZhanLiLogged = new HashSet<long>();

		// Token: 0x04000ADD RID: 2781
		public long HuiJiCDTicks;

		// Token: 0x04000ADE RID: 2782
		public long HuiJiCdTime;

		// Token: 0x04000ADF RID: 2783
		public long ZuoQiSkillCDTicks;

		// Token: 0x04000AE0 RID: 2784
		public long ZuoQiSkillCdTime;

		// Token: 0x04000AE1 RID: 2785
		public int ArmorCurrentV;

		// Token: 0x04000AE2 RID: 2786
		public int ArmorMaxV;

		// Token: 0x04000AE3 RID: 2787
		public double ArmorPercent;

		// Token: 0x04000AE4 RID: 2788
		public long BianShenCDTicks;

		// Token: 0x04000AE5 RID: 2789
		public long BianShenCdTime;

		// Token: 0x04000AE6 RID: 2790
		public long BianShenToTicks;

		// Token: 0x04000AE7 RID: 2791
		public List<int> skillIDList;
	}
}
