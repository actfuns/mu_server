using System;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "Map", "Code")]
	public class MapSettingItem
	{
		
		public int Code;

		
		public string Name;

		
		public int MapType;

		
		public int PicCode;

		
		public int AutoStart;

		
		public int RealiveType;

		
		public int Transfer;

		
		public int MoveType;

		
		public int Horse;

		
		public int ElsePeople;

		
		public int Transfiguration;

		
		public double NormalHuntNum = 1.0;

		
		public double RebornHuntNum = 0.0;
	}
}
