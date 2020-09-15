using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020007D2 RID: 2002
	public class SpriteMultipliedBuffer
	{
		// Token: 0x06003880 RID: 14464 RVA: 0x0030334C File Offset: 0x0030154C
		public void AddTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.ExtProps[index] = value;
				this._TempProp.ExtPropsTick[index] = toTicks;
			}
		}

		// Token: 0x06003881 RID: 14465 RVA: 0x003033B0 File Offset: 0x003015B0
		public void ClearAllTempExtProps()
		{
			lock (this._TempProp)
			{
				this._TempProp.ResetProps();
			}
		}

		// Token: 0x06003882 RID: 14466 RVA: 0x00303404 File Offset: 0x00301604
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

		// Token: 0x06003883 RID: 14467 RVA: 0x003034B8 File Offset: 0x003016B8
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

		// Token: 0x0400415E RID: 16734
		private BufferPropItem _TempProp = new BufferPropItem();
	}
}
