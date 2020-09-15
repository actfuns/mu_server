using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	// Token: 0x0200074F RID: 1871
	public class GridMagicHelperItemEx
	{
		// Token: 0x04003CB4 RID: 15540
		public int PosX;

		// Token: 0x04003CB5 RID: 15541
		public int PosY;

		// Token: 0x04003CB6 RID: 15542
		public int CopyMapID;

		// Token: 0x04003CB7 RID: 15543
		public MagicActionIDs MagicActionID;

		// Token: 0x04003CB8 RID: 15544
		public double[] MagicActionParams;

		// Token: 0x04003CB9 RID: 15545
		public long StartedTicks;

		// Token: 0x04003CBA RID: 15546
		public long LastTicks;

		// Token: 0x04003CBB RID: 15547
		public int ExecutedNum;

		// Token: 0x04003CBC RID: 15548
		public int MapCode;

		// Token: 0x04003CBD RID: 15549
		public int MaxNum = 8;

		// Token: 0x04003CBE RID: 15550
		public MagicActionIDs MagicActionID2;

		// Token: 0x04003CBF RID: 15551
		public double[] MagicActionParams2;

		// Token: 0x04003CC0 RID: 15552
		public List<Point> PointList = new List<Point>();

		// Token: 0x04003CC1 RID: 15553
		public int AttackerRoleId;
	}
}
