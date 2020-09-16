using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Logic
{
	
	public class ZorkBattleMonsterConfig
	{
		
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

		
		public ZorkBattleMonsterConfig Clone()
		{
			return base.MemberwiseClone() as ZorkBattleMonsterConfig;
		}

		
		public int ID;

		
		public int GroupID;

		
		public ZorkBattleArmyType ArmyType;

		
		public int MonsterId;

		
		public int MonsterNum;

		
		public int MonsterDropBuffId;

		
		public int BuffEffictTime;

		
		public int RewardIntegral;

		
		public double BossBlood;

		
		public int BuffRefreshTime;

		
		public int[] BossBuffGroup;

		
		public int[] BossBuffRound;

		
		public AwardsItemList BossKillAwardsItemList = new AwardsItemList();

		
		public List<int> BossBuffRoundList;

		
		public List<int> BossBuffGroupList;
	}
}
