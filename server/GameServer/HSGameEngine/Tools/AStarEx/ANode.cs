using System;

namespace HSGameEngine.Tools.AStarEx
{
	
	public class ANode
	{
		
		public ANode(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		
		public static long GetGUID(int key1, int key2)
		{
			long lKey = (long)key1;
			long lKey2 = (long)key2;
			return lKey << 32 | lKey2;
		}

		
		public static int GetGUID_X(long val)
		{
			return (int)(val >> 32 & (long) 0xffff_ffffUL);
		}

		
		public static int GetGUID_Y(long val)
		{
			return (int)(val & (long) 0xffff_ffffUL);
		}

		
		public int x;

		
		public int y;
	}
}
