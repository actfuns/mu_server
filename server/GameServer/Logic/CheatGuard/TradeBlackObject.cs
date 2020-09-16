using System;

namespace GameServer.Logic.CheatGuard
{
	
	internal class TradeBlackObject
	{
		
		public int RoleId;

		
		public TradeBlackHourItem[] HourItems;

		
		public DateTime LastFlushTime;

		
		public long BanTradeToTicks;

		
		public int ChangeLife;

		
		public int Level;

		
		public int VipLevel;

		
		public int ZoneId;

		
		public string RoleName;
	}
}
