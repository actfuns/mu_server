using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020007D1 RID: 2001
	public class SpriteBuffer
	{
		// Token: 0x06003873 RID: 14451 RVA: 0x00302DE8 File Offset: 0x00300FE8
		public void ResetForeverProps()
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ResetProps();
			}
		}

		// Token: 0x06003874 RID: 14452 RVA: 0x00302E3C File Offset: 0x0030103C
		public void AddForeverBaseProp(int index, double value)
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.BaseProps[index] = value;
			}
		}

		// Token: 0x06003875 RID: 14453 RVA: 0x00302E90 File Offset: 0x00301090
		public void AddForeverExtProp(int index, double value)
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ExtProps[index] = value;
			}
		}

		// Token: 0x06003876 RID: 14454 RVA: 0x00302EE4 File Offset: 0x003010E4
		public double[] getCopyBaseProp()
		{
			double[] baseProps = this._TempProp.BaseProps;
			double[] copyBaseProps = new double[baseProps.Length];
			for (int i = 0; i < baseProps.Length; i++)
			{
				copyBaseProps[i] = baseProps[i];
			}
			return copyBaseProps;
		}

		// Token: 0x06003877 RID: 14455 RVA: 0x00302F28 File Offset: 0x00301128
		public double[] getCopyExtProp()
		{
			double[] extProps = this._TempProp.ExtProps;
			double[] copyExtProps = new double[extProps.Length];
			for (int i = 0; i < extProps.Length; i++)
			{
				copyExtProps[i] = extProps[i];
			}
			return copyExtProps;
		}

		// Token: 0x06003878 RID: 14456 RVA: 0x00302F6C File Offset: 0x0030116C
		public void AddTempBaseProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.BaseProps[index] = value;
				this._TempProp.BasePropsTick[index] = toTicks;
			}
		}

		// Token: 0x06003879 RID: 14457 RVA: 0x00302FD0 File Offset: 0x003011D0
		public void AddTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				if (TimeUtil.NOW() * 10000L > this._TempProp.ExtPropsTick[index] || Math.Abs(value) >= Math.Abs(this._TempProp.ExtProps[index]))
				{
					this._TempProp.ExtProps[index] = value;
					this._TempProp.ExtPropsTick[index] = toTicks;
				}
			}
		}

		// Token: 0x0600387A RID: 14458 RVA: 0x00303070 File Offset: 0x00301270
		public void SetTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.ExtProps[index] = value;
				this._TempProp.ExtPropsTick[index] = toTicks;
			}
		}

		// Token: 0x0600387B RID: 14459 RVA: 0x003030D4 File Offset: 0x003012D4
		public double GetBaseProp(int index)
		{
			double tempProp = 0.0;
			lock (this._TempProp)
			{
				long nowTicks = TimeUtil.NOW() * 10000L;
				if (nowTicks - this._TempProp.BasePropsTick[index] < 0L)
				{
					tempProp = this._TempProp.BaseProps[index];
				}
			}
			double result;
			lock (this._ForeverProp)
			{
				result = this._ForeverProp.BaseProps[index] + tempProp;
			}
			return result;
		}

		// Token: 0x0600387C RID: 14460 RVA: 0x003031AC File Offset: 0x003013AC
		public double GetExtProp(int index)
		{
			double tempProp = 0.0;
			lock (this._TempProp)
			{
				long nowTicks = TimeUtil.NOW() * 10000L;
				if (nowTicks - this._TempProp.ExtPropsTick[index] < 0L)
				{
					tempProp = this._TempProp.ExtProps[index];
				}
			}
			double result;
			lock (this._ForeverProp)
			{
				result = this._ForeverProp.ExtProps[index] + tempProp;
			}
			return result;
		}

		// Token: 0x0600387D RID: 14461 RVA: 0x00303284 File Offset: 0x00301484
		public void ClearAllTempProps()
		{
			lock (this._TempProp)
			{
				this._TempProp.ResetProps();
			}
		}

		// Token: 0x0600387E RID: 14462 RVA: 0x003032D8 File Offset: 0x003014D8
		public void ClearAllForeverProps()
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ResetProps();
			}
		}

		// Token: 0x0400415C RID: 16732
		private BufferPropItem _ForeverProp = new BufferPropItem();

		// Token: 0x0400415D RID: 16733
		private BufferPropItem _TempProp = new BufferPropItem();
	}
}
