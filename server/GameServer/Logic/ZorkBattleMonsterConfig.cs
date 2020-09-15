using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Logic
{
	// Token: 0x0200083F RID: 2111
	public class ZorkBattleMonsterConfig
	{
		// Token: 0x06003B68 RID: 15208 RVA: 0x00327F3C File Offset: 0x0032613C
		public int RandomBuffID()
		{
			if (this.BossBuffRoundList == null || this.BossBuffRoundList.Count == 0)
			{
				this.BossBuffRoundList = new List<int>(this.BossBuffRound);
				this.BossBuffGroupList = new List<int>(this.BossBuffGroup);
			}
			int buffID = 0;
			int randnum = Global.GetRandomNumber(0, this.BossBuffRoundList.Sum());
			int randmax = 0;
			for (int idx = 0; idx < this.BossBuffRoundList.Count; idx++)
			{
				randmax += this.BossBuffRound[idx];
				if (randnum < randmax)
				{
					buffID = this.BossBuffGroupList[idx];
					this.BossBuffRoundList.RemoveAt(idx);
					this.BossBuffGroupList.RemoveAt(idx);
					break;
				}
			}
			return buffID;
		}

		// Token: 0x06003B69 RID: 15209 RVA: 0x00328010 File Offset: 0x00326210
		public ZorkBattleMonsterConfig Clone()
		{
			return base.MemberwiseClone() as ZorkBattleMonsterConfig;
		}

		// Token: 0x040045F5 RID: 17909
		public int ID;

		// Token: 0x040045F6 RID: 17910
		public int GroupID;

		// Token: 0x040045F7 RID: 17911
		public ZorkBattleArmyType ArmyType;

		// Token: 0x040045F8 RID: 17912
		public int MonsterId;

		// Token: 0x040045F9 RID: 17913
		public int MonsterNum;

		// Token: 0x040045FA RID: 17914
		public int MonsterDropBuffId;

		// Token: 0x040045FB RID: 17915
		public int BuffEffictTime;

		// Token: 0x040045FC RID: 17916
		public int RewardIntegral;

		// Token: 0x040045FD RID: 17917
		public double BossBlood;

		// Token: 0x040045FE RID: 17918
		public int BuffRefreshTime;

		// Token: 0x040045FF RID: 17919
		public int[] BossBuffGroup;

		// Token: 0x04004600 RID: 17920
		public int[] BossBuffRound;

		// Token: 0x04004601 RID: 17921
		public AwardsItemList BossKillAwardsItemList = new AwardsItemList();

		// Token: 0x04004602 RID: 17922
		public List<int> BossBuffRoundList;

		// Token: 0x04004603 RID: 17923
		public List<int> BossBuffGroupList;
	}
}
