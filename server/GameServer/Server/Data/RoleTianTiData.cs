using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleTianTiData
	{
		
		[ProtoMember(1)]
		public int RoleId;

		
		[ProtoMember(2)]
		public int DuanWeiId;

		
		[ProtoMember(3)]
		public int DuanWeiJiFen;

		
		[ProtoMember(4)]
		public int DuanWeiRank;

		
		[ProtoMember(5)]
		public int LianSheng;

		
		[ProtoMember(6)]
		public int SuccessCount;

		
		[ProtoMember(7)]
		public int FightCount;

		
		[ProtoMember(8)]
		public int TodayFightCount;

		
		[ProtoMember(9)]
		public int MonthDuanWeiRank;

		
		[ProtoMember(10)]
		public DateTime FetchMonthDuanWeiRankAwardsTime;

		
		[ProtoMember(11)]
		public int RongYao;

		
		[ProtoMember(12)]
		public int LastFightDayId;

		
		[ProtoMember(13)]
		public DateTime RankUpdateTime;

		
		[ProtoMember(14)]
		public int DayDuanWeiJiFen;
	}
}
