using System;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic.LiXianGuaJi
{
	// Token: 0x0200073F RID: 1855
	public class LiXianGuaJiRoleItem
	{
		// Token: 0x04003C58 RID: 15448
		public int ZoneID = 0;

		// Token: 0x04003C59 RID: 15449
		public string UserID = "";

		// Token: 0x04003C5A RID: 15450
		public int RoleID = 0;

		// Token: 0x04003C5B RID: 15451
		public string RoleName = "";

		// Token: 0x04003C5C RID: 15452
		public int RoleLevel = 0;

		// Token: 0x04003C5D RID: 15453
		public Point CurrentGrid;

		// Token: 0x04003C5E RID: 15454
		public long StartTicks = 0L;

		// Token: 0x04003C5F RID: 15455
		public int FakeRoleID = 0;

		// Token: 0x04003C60 RID: 15456
		public int MeditateTime = 0;

		// Token: 0x04003C61 RID: 15457
		public int NotSafeMeditateTime = 0;

		// Token: 0x04003C62 RID: 15458
		public long MeditateTicks = TimeUtil.NOW();

		// Token: 0x04003C63 RID: 15459
		public long BiGuanTime = TimeUtil.NOW();

		// Token: 0x04003C64 RID: 15460
		public int MapCode = 0;
	}
}
