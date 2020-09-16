using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Logic
{
	
	public class ZorkBattleArmyConfig
	{
		
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

		
		public ZorkBattleArmyConfig Clone()
		{
			return base.MemberwiseClone() as ZorkBattleArmyConfig;
		}

		
		public int ID;

		
		public int PosX;

		
		public int PosY;

		
		public int PursuitRadius;

		
		public int Range;

		
		public ZorkBattleArmyType ArmyType;

		
		public int[] ArmyGroupRound;

		
		public int[] GuardGroupID;

		
		public int FirstArmyTime;

		
		public int NextArmyRefresTime;

		
		public List<int> ArmyGroupRoundList;

		
		public List<int> GuardGroupIDList;

		
		public int MonsterDeadNum;
	}
}
