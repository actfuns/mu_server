using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class SpriteBuffer
	{
		
		public void ResetForeverProps()
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ResetProps();
			}
		}

		
		public void AddForeverBaseProp(int index, double value)
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.BaseProps[index] = value;
			}
		}

		
		public void AddForeverExtProp(int index, double value)
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ExtProps[index] = value;
			}
		}

		
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

		
		public void AddTempBaseProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.BaseProps[index] = value;
				this._TempProp.BasePropsTick[index] = toTicks;
			}
		}

		
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

		
		public void SetTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.ExtProps[index] = value;
				this._TempProp.ExtPropsTick[index] = toTicks;
			}
		}

		
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

		
		public void ClearAllTempProps()
		{
			lock (this._TempProp)
			{
				this._TempProp.ResetProps();
			}
		}

		
		public void ClearAllForeverProps()
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ResetProps();
			}
		}

		
		private BufferPropItem _ForeverProp = new BufferPropItem();

		
		private BufferPropItem _TempProp = new BufferPropItem();
	}
}
