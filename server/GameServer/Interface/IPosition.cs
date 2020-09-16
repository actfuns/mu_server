using System;
using System.Windows;

namespace GameServer.Interface
{
	// Token: 0x020005B3 RID: 1459
	public interface IPosition
	{
		// Token: 0x1700007B RID: 123
		
		
		Point Center { get; set; }

		// Token: 0x1700007C RID: 124
		
		
		Point Coordinate { get; set; }

		// Token: 0x1700007D RID: 125
		
		
		int Z { get; set; }
	}
}
