using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class KingOfBattleDynamicMonsterItem
	{
		
		public int Id;

		
		public int MapCode;

		
		public int MonsterID;

		
		public int PosX;

		
		public int PosY;

		
		public int Num;

		
		public int Radius;

		
		public int DelayBirthMs;

		
		public int PursuitRadius;

		
		public int MonsterType;

		
		public bool RebornBirth;

		
		public int RebornID;

		
		public int JiFenDamage;

		
		public int JiFenKill;

		
		public List<KingOfBattleRandomBuff> RandomBuffList = new List<KingOfBattleRandomBuff>();

		
		public int BuffTime;
	}
}
