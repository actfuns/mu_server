using System;

namespace GameServer.Logic
{
	
	public class BufferPropItem
	{
		
		public BufferPropItem()
		{
			this.ResetProps();
		}

		
		
		public double[] BaseProps
		{
			get
			{
				return this._BaseProps;
			}
		}

		
		
		public long[] BasePropsTick
		{
			get
			{
				return this._BasePropsTick;
			}
		}

		
		
		public double[] ExtProps
		{
			get
			{
				return this._ExtProps;
			}
		}

		
		
		public long[] ExtPropsTick
		{
			get
			{
				return this._ExtPropsTick;
			}
		}

		
		public void ResetProps()
		{
			for (int i = 0; i < 4; i++)
			{
				this._BaseProps[i] = 0.0;
				this._BasePropsTick[i] = 0L;
			}
			for (int i = 0; i < 177; i++)
			{
				this._ExtProps[i] = 0.0;
				this._ExtPropsTick[i] = 0L;
			}
		}

		
		private double[] _BaseProps = new double[4];

		
		private long[] _BasePropsTick = new long[4];

		
		private double[] _ExtProps = new double[177];

		
		private long[] _ExtPropsTick = new long[177];
	}
}
