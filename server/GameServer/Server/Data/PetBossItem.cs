using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class PetBossItem
	{
		
		public int ID;

		
		public int MonsterID;

		
		public int Star;

		
		public int FreeStartValue;

		
		public int FreeEndValue;

		
		public int ZuanShiStartValue;

		
		public int ZuanShiEndValue;

		
		public int Time;

		
		public string FightAward;

		
		public string KillAward;

		
		public string KillExtraAward;

		
		public int PetLevelStep;

		
		public int[] PetLevelStepNum;

		
		public int ExcellentStep;

		
		public int[] ExcellentStepNum;

		
		public List<int> PetSuit;

		
		public List<int> PetRate;

		
		public int SuitRate;
	}
}
