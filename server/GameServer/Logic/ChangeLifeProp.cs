using System;

namespace GameServer.Logic
{
	
	public class ChangeLifeProp
	{
		
		
		public double[] ChangeLifeFirstProps
		{
			get
			{
				return this.m_ChangeLifeFirstProps;
			}
		}

		
		
		public double[] ChangeLifeSecondProps
		{
			get
			{
				return this.m_ChangeLifeSecondProps;
			}
		}

		
		public ChangeLifeProp()
		{
			this.ResetChangeLifeProps();
		}

		
		public void ResetChangeLifeProps()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_ChangeLifeFirstProps[i] = 0.0;
			}
			for (int i = 0; i < 177; i++)
			{
				this.m_ChangeLifeSecondProps[i] = 0.0;
			}
		}

		
		private double[] m_ChangeLifeFirstProps = new double[4];

		
		private double[] m_ChangeLifeSecondProps = new double[177];
	}
}
