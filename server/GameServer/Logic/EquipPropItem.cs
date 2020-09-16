using System;

namespace GameServer.Logic
{
	
	public class EquipPropItem
	{
		
		public EquipPropItem()
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

		
		
		public double[] ExtProps
		{
			get
			{
				return this._ExtProps;
			}
		}

		
		public void ResetProps()
		{
			for (int i = 0; i < 5; i++)
			{
				this._BaseProps[i] = 0.0;
			}
			for (int i = 0; i < 177; i++)
			{
				this._ExtProps[i] = 0.0;
			}
		}

		
		private double[] _BaseProps = new double[5];

		
		private double[] _ExtProps = new double[177];
	}
}
