using System;

namespace GameServer.Logic
{
	// Token: 0x020007D3 RID: 2003
	public class SpriteOnceBuffer
	{
		// Token: 0x06003885 RID: 14469 RVA: 0x00303574 File Offset: 0x00301774
		public void AddTempBaseProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.BaseProps[index] = value;
				this._TempProp.BasePropsTick[index] = toTicks;
			}
		}

		// Token: 0x06003886 RID: 14470 RVA: 0x003035D8 File Offset: 0x003017D8
		public void AddTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.ExtProps[index] = value;
				this._TempProp.ExtPropsTick[index] = toTicks;
			}
		}

		// Token: 0x06003887 RID: 14471 RVA: 0x0030363C File Offset: 0x0030183C
		public double GetBaseProp(int index)
		{
			double tempProp = 0.0;
			lock (this._TempProp)
			{
				tempProp = this._TempProp.BaseProps[index];
				this._TempProp.BaseProps[index] = 0.0;
			}
			return tempProp;
		}

		// Token: 0x06003888 RID: 14472 RVA: 0x003036B8 File Offset: 0x003018B8
		public double GetExtProp(int index)
		{
			double tempProp = 0.0;
			lock (this._TempProp)
			{
				tempProp = this._TempProp.ExtProps[index];
				this._TempProp.ExtProps[index] = 0.0;
			}
			return tempProp;
		}

		// Token: 0x0400415F RID: 16735
		private BufferPropItem _TempProp = new BufferPropItem();
	}
}
