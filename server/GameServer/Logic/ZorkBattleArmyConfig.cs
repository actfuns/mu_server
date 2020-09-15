using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Logic
{
	// Token: 0x0200083E RID: 2110
	public class ZorkBattleArmyConfig
	{
		// Token: 0x06003B65 RID: 15205 RVA: 0x00327E3C File Offset: 0x0032603C
		public int RandomGroupID()
		{
			if (this.ArmyGroupRoundList == null || this.ArmyGroupRoundList.Count == 0)
			{
				this.ArmyGroupRoundList = new List<int>(this.ArmyGroupRound);
				this.GuardGroupIDList = new List<int>(this.GuardGroupID);
			}
			int groupID = 0;
			int randnum = Global.GetRandomNumber(0, this.ArmyGroupRoundList.Sum());
			int randmax = 0;
			for (int idx = 0; idx < this.ArmyGroupRoundList.Count; idx++)
			{
				randmax += this.ArmyGroupRoundList[idx];
				if (randnum < randmax)
				{
					groupID = this.GuardGroupIDList[idx];
					this.ArmyGroupRoundList.RemoveAt(idx);
					this.GuardGroupIDList.RemoveAt(idx);
					break;
				}
			}
			return groupID;
		}

		// Token: 0x06003B66 RID: 15206 RVA: 0x00327F14 File Offset: 0x00326114
		public ZorkBattleArmyConfig Clone()
		{
			return base.MemberwiseClone() as ZorkBattleArmyConfig;
		}

		// Token: 0x040045E8 RID: 17896
		public int ID;

		// Token: 0x040045E9 RID: 17897
		public int PosX;

		// Token: 0x040045EA RID: 17898
		public int PosY;

		// Token: 0x040045EB RID: 17899
		public int PursuitRadius;

		// Token: 0x040045EC RID: 17900
		public int Range;

		// Token: 0x040045ED RID: 17901
		public ZorkBattleArmyType ArmyType;

		// Token: 0x040045EE RID: 17902
		public int[] ArmyGroupRound;

		// Token: 0x040045EF RID: 17903
		public int[] GuardGroupID;

		// Token: 0x040045F0 RID: 17904
		public int FirstArmyTime;

		// Token: 0x040045F1 RID: 17905
		public int NextArmyRefresTime;

		// Token: 0x040045F2 RID: 17906
		public List<int> ArmyGroupRoundList;

		// Token: 0x040045F3 RID: 17907
		public List<int> GuardGroupIDList;

		// Token: 0x040045F4 RID: 17908
		public int MonsterDeadNum;
	}
}
