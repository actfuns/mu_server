using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class TalentInfo
	{
		
		public int ID = 0;

		
		public int Type = 0;

		
		public string Name = "";

		
		public int NeedTalentCount = 0;

		
		public int NeedTalentID = 0;

		
		public int NeedTalentLevel = 0;

		
		public int LevelMax = 0;

		
		public int EffectType = 0;

		
		public Dictionary<int, List<TalentEffectInfo>> EffectList = null;
	}
}
