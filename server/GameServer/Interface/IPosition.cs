using System;
using System.Windows;

namespace GameServer.Interface
{
	// Token: 0x020005B3 RID: 1459
	public interface IPosition
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06001A6D RID: 6765
		// (set) Token: 0x06001A6E RID: 6766
		Point Center { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06001A6F RID: 6767
		// (set) Token: 0x06001A70 RID: 6768
		Point Coordinate { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06001A71 RID: 6769
		// (set) Token: 0x06001A72 RID: 6770
		int Z { get; set; }
	}
}
