using System;
using System.Collections.Generic;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x02000273 RID: 627
	public class CompMineRuntimeData
	{
		// Token: 0x04000F9B RID: 3995
		public const string CompMine_FileName = "Config/CompMineWar.xml";

		// Token: 0x04000F9C RID: 3996
		public const string CompMineTruck_FileName = "Config/CompMineTruck.xml";

		// Token: 0x04000F9D RID: 3997
		public const string CompMineLink_FileName = "Config/CompMineLink.xml";

		// Token: 0x04000F9E RID: 3998
		public const string CompMineBirth_FileName = "Config/CompMineBirthPoint.xml";

		// Token: 0x04000F9F RID: 3999
		public const string CompMineReward_FileName = "Config/CompMineAward.xml";

		// Token: 0x04000FA0 RID: 4000
		public object Mutex = new object();

		// Token: 0x04000FA1 RID: 4001
		public Dictionary<int, CompMineConfig> CompMineConfigDict = new Dictionary<int, CompMineConfig>();

		// Token: 0x04000FA2 RID: 4002
		public List<CompMineTruckConfig> CompMineTruckConfigList = new List<CompMineTruckConfig>();

		// Token: 0x04000FA3 RID: 4003
		public List<CompMineLinkConfig> CompMineLinkConfigList = new List<CompMineLinkConfig>();

		// Token: 0x04000FA4 RID: 4004
		public Dictionary<int, CompMineBirthConfig> CompMineBirthConfigDict = new Dictionary<int, CompMineBirthConfig>();

		// Token: 0x04000FA5 RID: 4005
		public List<CompMineRewardConfig> CompMineRewardConfigList = new List<CompMineRewardConfig>();

		// Token: 0x04000FA6 RID: 4006
		public int[] CompMineAttackKill;

		// Token: 0x04000FA7 RID: 4007
		public int[] CompMineAttackShutDown;

		// Token: 0x04000FA8 RID: 4008
		public int CompMineDie;

		// Token: 0x04000FA9 RID: 4009
		public double[] CompMineAwardNum;

		// Token: 0x04000FAA RID: 4010
		public Dictionary<int, CompFuBenData> FuBenItemData = new Dictionary<int, CompFuBenData>();
	}
}
