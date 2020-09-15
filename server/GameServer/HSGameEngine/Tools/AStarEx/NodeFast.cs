using System;
using System.Runtime.InteropServices;

namespace HSGameEngine.Tools.AStarEx
{
	// Token: 0x020008D8 RID: 2264
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NodeFast
	{
		// Token: 0x04004F8E RID: 20366
		public double f;

		// Token: 0x04004F8F RID: 20367
		public double g;

		// Token: 0x04004F90 RID: 20368
		public double h;

		// Token: 0x04004F91 RID: 20369
		public int parentX;

		// Token: 0x04004F92 RID: 20370
		public int parentY;
	}
}
