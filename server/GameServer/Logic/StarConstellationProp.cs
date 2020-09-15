using System;

namespace GameServer.Logic
{
	// Token: 0x0200079B RID: 1947
	public class StarConstellationProp
	{
		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x060032C8 RID: 13000 RVA: 0x002D02E4 File Offset: 0x002CE4E4
		public double[] StarConstellationFirstProps
		{
			get
			{
				return this.m_StarConstellationFirstProps;
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x060032C9 RID: 13001 RVA: 0x002D02FC File Offset: 0x002CE4FC
		public double[] StarConstellationSecondProps
		{
			get
			{
				return this.m_StarConstellationSecondProps;
			}
		}

		// Token: 0x060032CA RID: 13002 RVA: 0x002D0314 File Offset: 0x002CE514
		public StarConstellationProp()
		{
			this.ResetStarConstellationProps();
		}

		// Token: 0x060032CB RID: 13003 RVA: 0x002D0344 File Offset: 0x002CE544
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

		// Token: 0x04003EC2 RID: 16066
		private double[] m_StarConstellationFirstProps = new double[4];

		// Token: 0x04003EC3 RID: 16067
		private double[] m_StarConstellationSecondProps = new double[177];
	}
}
