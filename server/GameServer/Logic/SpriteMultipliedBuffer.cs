using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class SpriteMultipliedBuffer
	{
		
		public void AddTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.ExtProps[index] = value;
				this._TempProp.ExtPropsTick[index] = toTicks;
			}
		}

		
		public void ClearAllTempExtProps()
		{
			lock (this._TempProp)
			{
				this._TempProp.ResetProps();
			}
		}

		
		public double GetExtProp(int index, double baseValue)
		{
			double tempProp = 0.0;
			lock (this._TempProp)
			{
				long nowTicks = TimeUtil.NOW() * 10000L;
				if (this._TempProp.ExtPropsTick[index] <= 0L || nowTicks - this._TempProp.ExtPropsTick[index] < 0L)
				{
					tempProp = this._TempProp.ExtProps[index];
				}
			}
			return (1.0 + tempProp) * baseValue;
		}

		
		public double GetExtProp(int index)
		{
			double tempProp = 0.0;
			lock (this._TempProp)
			{
				long nowTicks = TimeUtil.NOW() * 10000L;
				if (this._TempProp.ExtPropsTick[index] <= 0L || nowTicks - this._TempProp.ExtPropsTick[index] < 0L)
				{
					tempProp = this._TempProp.ExtProps[index];
				}
			}
			return tempProp;
		}

		
		private BufferPropItem _TempProp = new BufferPropItem();
	}
}
