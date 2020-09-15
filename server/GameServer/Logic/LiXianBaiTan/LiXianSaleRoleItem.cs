using System;
using System.Windows;

namespace GameServer.Logic.LiXianBaiTan
{
	// Token: 0x0200073D RID: 1853
	public class LiXianSaleRoleItem
	{
		// Token: 0x04003C4B RID: 15435
		public int ZoneID = 0;

		// Token: 0x04003C4C RID: 15436
		public string UserID = "";

		// Token: 0x04003C4D RID: 15437
		public int RoleID = 0;

		// Token: 0x04003C4E RID: 15438
		public string RoleName = "";

		// Token: 0x04003C4F RID: 15439
		public int RoleLevel = 0;

		// Token: 0x04003C50 RID: 15440
		public Point CurrentGrid;

		// Token: 0x04003C51 RID: 15441
		public int LiXianBaiTanMaxTicks = 0;

		// Token: 0x04003C52 RID: 15442
		public long StartTicks = 0L;

		// Token: 0x04003C53 RID: 15443
		public int FakeRoleID = 0;
	}
}
