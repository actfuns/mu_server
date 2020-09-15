using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000544 RID: 1348
	public class RangeKey : IComparable<RangeKey>, IEqualityComparer<RangeKey>
	{
		// Token: 0x060019B8 RID: 6584 RVA: 0x00190C81 File Offset: 0x0018EE81
		public RangeKey(int value)
		{
			this.Left = value;
			this.Right = value;
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x00190C9A File Offset: 0x0018EE9A
		public RangeKey(int left, int right, object obj = null)
		{
			this.Left = left;
			this.Right = right;
			this.tag = obj;
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x00190CBC File Offset: 0x0018EEBC
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

		// Token: 0x060019BB RID: 6587 RVA: 0x00190D04 File Offset: 0x0018EF04
		public bool Equals(RangeKey x, RangeKey y)
		{
			return 0 == x.CompareTo(y);
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x00190D20 File Offset: 0x0018EF20
		public int GetHashCode(RangeKey obj)
		{
			return 0;
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x00190D34 File Offset: 0x0018EF34
		public static implicit operator RangeKey(int key)
		{
			return new RangeKey(key, key, null);
		}

		// Token: 0x0400241D RID: 9245
		public static RangeKey Comparer = new RangeKey(-1, -1, null);

		// Token: 0x0400241E RID: 9246
		private int Left;

		// Token: 0x0400241F RID: 9247
		private int Right;

		// Token: 0x04002420 RID: 9248
		public object tag;
	}
}
