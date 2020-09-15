using System;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008E5 RID: 2277
	public struct Point3D
	{
		// Token: 0x060041C6 RID: 16838 RVA: 0x003C28FA File Offset: 0x003C0AFA
		public Point3D(int x, int y, int z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		// Token: 0x04004FF2 RID: 20466
		public int X;

		// Token: 0x04004FF3 RID: 20467
		public int Y;

		// Token: 0x04004FF4 RID: 20468
		public int Z;
	}
}
