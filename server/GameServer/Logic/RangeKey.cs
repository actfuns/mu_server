using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class RangeKey : IComparable<RangeKey>, IEqualityComparer<RangeKey>
	{
		
		public RangeKey(int value)
		{
			this.Left = value;
			this.Right = value;
		}

		
		public RangeKey(int left, int right, object obj = null)
		{
			this.Left = left;
			this.Right = right;
			this.tag = obj;
		}

		
		public int CompareTo(RangeKey obj)
		{
			int result;
			if (this.Right < obj.Left)
			{
				result = -1;
			}
			else if (this.Left > obj.Right)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public bool Equals(RangeKey x, RangeKey y)
		{
			return 0 == x.CompareTo(y);
		}

		
		public int GetHashCode(RangeKey obj)
		{
			return 0;
		}

		
		public static implicit operator RangeKey(int key)
		{
			return new RangeKey(key, key, null);
		}

		
		public static RangeKey Comparer = new RangeKey(-1, -1, null);

		
		private int Left;

		
		private int Right;

		
		public object tag;
	}
}
