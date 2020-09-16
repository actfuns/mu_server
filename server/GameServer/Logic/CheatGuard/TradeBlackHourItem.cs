using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace GameServer.Logic.CheatGuard
{
	
	[ProtoContract]
	internal class TradeBlackHourItem
	{
		
		public TradeBlackHourItem SimpleClone()
		{
			return new TradeBlackHourItem
			{
				RoleId = this.RoleId,
				Day = this.Day,
				Hour = this.Hour,
				MarketTimes = this.MarketTimes,
				MarketInPrice = this.MarketInPrice,
				MarketOutPrice = this.MarketOutPrice,
				TradeTimes = this.TradeTimes,
				TradeInPrice = this.TradeInPrice,
				TradeOutPrice = this.TradeOutPrice,
				TradeDistinctRoleCount = ((this.TradeRoles != null) ? this.TradeRoles.Count<int>() : 0)
			};
		}

		
		[ProtoMember(1)]
		public int RoleId;

		
		[ProtoMember(2)]
		public string Day;

		
		[ProtoMember(3)]
		public int Hour;

		
		[ProtoMember(4)]
		public int MarketTimes;

		
		[ProtoMember(5)]
		public long MarketInPrice;

		
		[ProtoMember(6)]
		public long MarketOutPrice;

		
		[ProtoMember(7)]
		public int TradeTimes;

		
		[ProtoMember(8)]
		public long TradeInPrice;

		
		[ProtoMember(9)]
		public long TradeOutPrice;

		
		[ProtoMember(10)]
		public HashSet<int> TradeRoles;

		
		[ProtoMember(11)]
		public int TradeDistinctRoleCount;
	}
}
