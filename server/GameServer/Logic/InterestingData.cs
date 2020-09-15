using System;

namespace GameServer.Logic
{
	// Token: 0x02000245 RID: 581
	public class InterestingData
	{
		// Token: 0x06000803 RID: 2051 RVA: 0x0007A104 File Offset: 0x00078304
		public InterestingData()
		{
			this.itemArray = new InterestingData.Item[2];
			for (int i = 0; i < 2; i++)
			{
				this.itemArray[i] = new InterestingData.Item();
			}
		}

		// Token: 0x04000DDD RID: 3549
		public InterestingData.Item[] itemArray = null;

		// Token: 0x04000DDE RID: 3550
		public double Speed = 0.0;

		// Token: 0x02000246 RID: 582
		public class Item
		{
			// Token: 0x04000DDF RID: 3551
			public int RequestCount = 0;

			// Token: 0x04000DE0 RID: 3552
			public int ResponseCount = 0;

			// Token: 0x04000DE1 RID: 3553
			public long LastRequestMs = 0L;

			// Token: 0x04000DE2 RID: 3554
			public long LastResponseMs = 0L;

			// Token: 0x04000DE3 RID: 3555
			public int InvalidCount = 0;
		}
	}
}
