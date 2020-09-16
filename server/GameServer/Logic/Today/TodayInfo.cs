using System;

namespace GameServer.Logic.Today
{
	
	public class TodayInfo
	{
		
		public TodayInfo()
		{
		}

		
		public TodayInfo(TodayInfo info)
		{
			this.Type = info.Type;
			this.ID = info.ID;
			this.Name = info.Name;
			this.FuBenID = info.FuBenID;
			this.HuoDongID = info.HuoDongID;
			this.LevelMin = info.LevelMin;
			this.LevelMax = info.LevelMax;
			this.TaskMin = info.TaskMin;
			this.NumMax = info.NumMax;
			this.NumEnd = info.NumEnd;
			this.AwardInfo = info.AwardInfo;
		}

		
		public int Type = 0;

		
		public int ID = 0;

		
		public string Name = "";

		
		public int FuBenID = 0;

		
		public int HuoDongID = 0;

		
		public int LevelMin = 0;

		
		public int LevelMax = 0;

		
		public int TaskMin = 0;

		
		public int NumMax = 0;

		
		public int NumEnd = 0;

		
		public TodayAwardInfo AwardInfo = new TodayAwardInfo();
	}
}
