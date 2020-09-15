using System;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008DB RID: 2267
	public struct PathFinderNode
	{
		// Token: 0x04004F9B RID: 20379
		public int F;

		// Token: 0x04004F9C RID: 20380
		public int G;

		// Token: 0x04004F9D RID: 20381
		public int H;

		// Token: 0x04004F9E RID: 20382
		public int X;

		// Token: 0x04004F9F RID: 20383
		public int Y;

		// Token: 0x04004FA0 RID: 20384
		public int PX;

		// Token: 0x04004FA1 RID: 20385
		public int PY;
	}
}
