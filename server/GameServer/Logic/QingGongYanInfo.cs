using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class QingGongYanInfo
	{
		
		public bool IfBanTime(DateTime time)
		{
			int dayofweek = (int)time.DayOfWeek;
			if (dayofweek == 0)
			{
				dayofweek = 7;
			}
			foreach (string item in this.ProhibitedTimeList)
			{
				string[] strFields = this.ProhibitedTimeList[0].Split(new char[]
				{
					','
				});
				if (Convert.ToInt32(strFields[0]) == dayofweek)
				{
					DateTime beginTime = DateTime.Parse(strFields[1]);
					DateTime endTime = DateTime.Parse(strFields[2]);
					if (time >= beginTime && time <= endTime)
					{
						return true;
					}
				}
			}
			return false;
		}

		
		public int Index;

		
		public int NpcID;

		
		public int MapCode;

		
		public int X;

		
		public int Y;

		
		public int Direction;

		
		public List<string> ProhibitedTimeList = new List<string>();

		
		public string BeginTime;

		
		public string OverTime;

		
		public int FunctionID;

		
		public int HoldBindJinBi;

		
		public int TotalNum;

		
		public int SingleNum;

		
		public int JoinBindJinBi;

		
		public int ExpAward;

		
		public int XingHunAward;

		
		public int ZhanGongAward;

		
		public int ZuanShiCoe;
	}
}
