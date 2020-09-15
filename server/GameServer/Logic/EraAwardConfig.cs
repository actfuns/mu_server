using System;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x0200029F RID: 671
	public class EraAwardConfig : EraAwardConfigBase
	{
		// Token: 0x04001081 RID: 4225
		public string EraName;

		// Token: 0x04001082 RID: 4226
		public int EraRanking;

		// Token: 0x04001083 RID: 4227
		public int Progress;

		// Token: 0x04001084 RID: 4228
		public int Contribution;

		// Token: 0x04001085 RID: 4229
		public AwardsItemList LeaderReward = new AwardsItemList();

		// Token: 0x04001086 RID: 4230
		public AwardsItemList MasterReward = new AwardsItemList();

		// Token: 0x04001087 RID: 4231
		public AwardsItemList Reward = new AwardsItemList();
	}
}
