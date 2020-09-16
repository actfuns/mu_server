using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class ThemeBossConfig
	{
		
		public const int ApplyOverTime = 180;

		
		public int ID;

		
		public int MonstersID;

		
		public int MaxUnionLevel;

		
		public int MapCode;

		
		public int PosX;

		
		public int PosY;

		
		public int Radius;

		
		public int Num;

		
		public List<TimeSpan> TimePoints = new List<TimeSpan>();

		
		public List<double> SecondsOfDay = new List<double>();
	}
}
