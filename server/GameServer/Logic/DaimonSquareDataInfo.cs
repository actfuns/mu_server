using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class DaimonSquareDataInfo
	{
		
		
		
		public int MapCode { get; set; }

		
		
		
		public int MinChangeLifeNum { get; set; }

		
		
		
		public int MaxChangeLifeNum { get; set; }

		
		
		
		public int MinLevel { get; set; }

		
		
		
		public int MaxLevel { get; set; }

		
		
		
		public int MaxEnterNum { get; set; }

		
		
		
		public int NeedGoodsID { get; set; }

		
		
		
		public int NeedGoodsNum { get; set; }

		
		
		
		public int MaxPlayerNum { get; set; }

		
		
		
		public string[] MonsterID { get; set; }

		
		
		
		public string[] MonsterNum { get; set; }

		
		
		
		public int posX { get; set; }

		
		
		
		public int posZ { get; set; }

		
		
		
		public int Radius { get; set; }

		
		
		
		public int MonsterSum { get; set; }

		
		
		
		public string[] CreateNextWaveMonsterCondition { get; set; }

		
		
		
		public int TimeModulus { get; set; }

		
		
		
		public int ExpModulus { get; set; }

		
		
		
		public int MoneyModulus { get; set; }

		
		
		
		public string[] AwardItem { get; set; }

		
		
		
		public List<string> BeginTime { get; set; }

		
		
		
		public int PrepareTime { get; set; }

		
		
		
		public int DurationTime { get; set; }

		
		
		
		public int LeaveTime { get; set; }
	}
}
