using System;

namespace GameServer.Logic.KuaFuIPStatistics
{
	// Token: 0x0200034B RID: 843
	public class UserOperaData
	{
		// Token: 0x04001638 RID: 5688
		public string UserID;

		// Token: 0x04001639 RID: 5689
		public long IPAsInt;

		// Token: 0x0400163A RID: 5690
		public int[] OperaTime = new int[4];

		// Token: 0x0400163B RID: 5691
		public int[] OperaCount = new int[4];

		// Token: 0x0400163C RID: 5692
		public int[] AllCount = new int[6];

		// Token: 0x0400163D RID: 5693
		public long createTicks = 0L;
	}
}
