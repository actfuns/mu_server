using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020001D6 RID: 470
	public class TimedPropsData : IComparer<TimedPropsData>
	{
		// Token: 0x060005F5 RID: 1525 RVA: 0x00054560 File Offset: 0x00052760
		public TimedPropsData(long _startTicks, int _bufferTicks, int _propsType, int _propsIndex, double _propsValue, int _tag, int _color)
		{
			this.startTicks = _startTicks;
			this.bufferTicks = _bufferTicks;
			this.propsType = _propsType;
			this.propsIndex = _propsIndex;
			this.propsValue = _propsValue;
			this.tag = _tag;
			this.skillId = _color;
			this.endTicks = _startTicks + (long)_bufferTicks;
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x000545B8 File Offset: 0x000527B8
		public int Compare(TimedPropsData x, TimedPropsData y)
		{
			int result;
			if (x.endTicks > y.endTicks)
			{
				result = 1;
			}
			else if (x.endTicks < y.endTicks)
			{
				result = -1;
			}
			else
			{
				result = x.GetHashCode() - y.GetHashCode();
			}
			return result;
		}

		// Token: 0x04000A63 RID: 2659
		public long startTicks;

		// Token: 0x04000A64 RID: 2660
		public int bufferTicks;

		// Token: 0x04000A65 RID: 2661
		public int propsType;

		// Token: 0x04000A66 RID: 2662
		public int propsIndex;

		// Token: 0x04000A67 RID: 2663
		public double propsValue;

		// Token: 0x04000A68 RID: 2664
		public int tag;

		// Token: 0x04000A69 RID: 2665
		public int skillId;

		// Token: 0x04000A6A RID: 2666
		public long endTicks;
	}
}
