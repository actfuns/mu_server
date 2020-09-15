using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000257 RID: 599
	public class QiZhiConfig : ICloneable
	{
		// Token: 0x06000861 RID: 2145 RVA: 0x00080DA0 File Offset: 0x0007EFA0
		public object Clone()
		{
			return base.MemberwiseClone() as QiZhiConfig;
		}

		// Token: 0x04000E72 RID: 3698
		public int NPCID;

		// Token: 0x04000E73 RID: 3699
		public int BufferID;

		// Token: 0x04000E74 RID: 3700
		public int PosX;

		// Token: 0x04000E75 RID: 3701
		public int PosY;

		// Token: 0x04000E76 RID: 3702
		public HashSet<int> UseAuthority = new HashSet<int>();

		// Token: 0x04000E77 RID: 3703
		public int MonsterId;

		// Token: 0x04000E78 RID: 3704
		public int Injure;

		// Token: 0x04000E79 RID: 3705
		public int RebirthSiteX;

		// Token: 0x04000E7A RID: 3706
		public int RebirthSiteY;

		// Token: 0x04000E7B RID: 3707
		public int RebirthRadius;

		// Token: 0x04000E7C RID: 3708
		public int ProduceTime;

		// Token: 0x04000E7D RID: 3709
		public int ProduceNum;

		// Token: 0x04000E7E RID: 3710
		public int BattleWhichSide;

		// Token: 0x04000E7F RID: 3711
		public bool Alive;

		// Token: 0x04000E80 RID: 3712
		public long DeadTicks;

		// Token: 0x04000E81 RID: 3713
		public long KillerBhid;

		// Token: 0x04000E82 RID: 3714
		public long InstallBhid;

		// Token: 0x04000E83 RID: 3715
		public string InstallBhName;
	}
}
