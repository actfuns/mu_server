using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class TimedPropsData : IComparer<TimedPropsData>
	{
		
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

		
		public long startTicks;

		
		public int bufferTicks;

		
		public int propsType;

		
		public int propsIndex;

		
		public double propsValue;

		
		public int tag;

		
		public int skillId;

		
		public long endTicks;
	}
}
