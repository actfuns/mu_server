using System;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008DC RID: 2268
	public enum PathFinderNodeType
	{
		// Token: 0x04004FA3 RID: 20387
		Start = 1,
		// Token: 0x04004FA4 RID: 20388
		End,
		// Token: 0x04004FA5 RID: 20389
		Open = 4,
		// Token: 0x04004FA6 RID: 20390
		Close = 8,
		// Token: 0x04004FA7 RID: 20391
		Current = 16,
		// Token: 0x04004FA8 RID: 20392
		Path = 32
	}
}
