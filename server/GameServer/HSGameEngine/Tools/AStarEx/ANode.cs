using System;

namespace HSGameEngine.Tools.AStarEx
{
	// Token: 0x020008D5 RID: 2261
	public class ANode
	{
		// Token: 0x06004133 RID: 16691 RVA: 0x003BF459 File Offset: 0x003BD659
		public ANode(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x06004134 RID: 16692 RVA: 0x003BF474 File Offset: 0x003BD674
		public static long GetGUID(int key1, int key2)
		{
			long lKey = (long)key1;
			long lKey2 = (long)key2;
			return lKey << 32 | lKey2;
		}

		// Token: 0x06004135 RID: 16693 RVA: 0x003BF494 File Offset: 0x003BD694
		public static int GetGUID_X(long val)
		{
			return (int)(val >> 32 & (long) 0xffff_ffffUL);
		}

		// Token: 0x06004136 RID: 16694 RVA: 0x003BF4B0 File Offset: 0x003BD6B0
		public static int GetGUID_Y(long val)
		{
			return (int)(val & (long) 0xffff_ffffUL);
		}

		// Token: 0x04004F7D RID: 20349
		public int x;

		// Token: 0x04004F7E RID: 20350
		public int y;
	}
}
