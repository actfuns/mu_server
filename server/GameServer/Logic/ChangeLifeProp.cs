using System;

namespace GameServer.Logic
{
	// Token: 0x02000539 RID: 1337
	public class ChangeLifeProp
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06001971 RID: 6513 RVA: 0x0018D414 File Offset: 0x0018B614
		public double[] ChangeLifeFirstProps
		{
			get
			{
				return this.m_ChangeLifeFirstProps;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06001972 RID: 6514 RVA: 0x0018D42C File Offset: 0x0018B62C
		public double[] ChangeLifeSecondProps
		{
			get
			{
				return this.m_ChangeLifeSecondProps;
			}
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x0018D444 File Offset: 0x0018B644
		public ChangeLifeProp()
		{
			this.ResetChangeLifeProps();
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x0018D474 File Offset: 0x0018B674
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

		// Token: 0x04002398 RID: 9112
		private double[] m_ChangeLifeFirstProps = new double[4];

		// Token: 0x04002399 RID: 9113
		private double[] m_ChangeLifeSecondProps = new double[177];
	}
}
