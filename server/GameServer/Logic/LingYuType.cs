using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	internal class LingYuType
	{
		
		public int Type;

		
		public string Name;

		
		public double LifeScale;

		
		public double AttackScale;

		
		public double DefenseScale;

		
		public double MAttackScale;

		
		public double MDefenseScale;

		
		public double HitScale;

		
		public Dictionary<int, LingYuLevel> LevelDict = new Dictionary<int, LingYuLevel>();

		
		public Dictionary<int, LingYuSuit> SuitDict = new Dictionary<int, LingYuSuit>();
	}
}
