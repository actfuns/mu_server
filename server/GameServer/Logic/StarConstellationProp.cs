using System;

namespace GameServer.Logic
{
	
	public class StarConstellationProp
	{
		
		
		public double[] StarConstellationFirstProps
		{
			get
			{
				return this.m_StarConstellationFirstProps;
			}
		}

		
		
		public double[] StarConstellationSecondProps
		{
			get
			{
				return this.m_StarConstellationSecondProps;
			}
		}

		
		public StarConstellationProp()
		{
			this.ResetStarConstellationProps();
		}

		
		public void ResetStarConstellationProps()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_StarConstellationFirstProps[i] = 0.0;
			}
			for (int i = 0; i < 177; i++)
			{
				this.m_StarConstellationSecondProps[i] = 0.0;
			}
		}

		
		private double[] m_StarConstellationFirstProps = new double[4];

		
		private double[] m_StarConstellationSecondProps = new double[177];
	}
}
