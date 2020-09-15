using System;

namespace Server.Data
{
	// Token: 0x020002FB RID: 763
	public static class JueXingActionResultType
	{
		// Token: 0x040013B5 RID: 5045
		public const int Success = 0;

		// Token: 0x040013B6 RID: 5046
		public const int HaveActive = -1;

		// Token: 0x040013B7 RID: 5047
		public const int ErrConfig = -2;

		// Token: 0x040013B8 RID: 5048
		public const int NeedPart = -3;

		// Token: 0x040013B9 RID: 5049
		public const int ErrDB = -4;

		// Token: 0x040013BA RID: 5050
		public const int ErrParent = -5;

		// Token: 0x040013BB RID: 5051
		public const int ErrSuitType = -6;

		// Token: 0x040013BC RID: 5052
		public const int ErrNotActivite = -7;

		// Token: 0x040013BD RID: 5053
		public const int ErrLevelLimit = -8;

		// Token: 0x040013BE RID: 5054
		public const int ErrChenLimit = -9;

		// Token: 0x040013BF RID: 5055
		public const int ErrGoodsLimit = -10;
	}
}
