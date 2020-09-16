using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HuodongData
	{
		
		[ProtoMember(1)]
		public string LastWeekID = "";

		
		[ProtoMember(2)]
		public string LastDayID = "";

		
		[ProtoMember(3)]
		public int LoginNum = 0;

		
		[ProtoMember(4)]
		public int NewStep = 0;

		
		[ProtoMember(5)]
		public long StepTime = 0L;

		
		[ProtoMember(6)]
		public int LastMTime = 0;

		
		[ProtoMember(7)]
		public string CurMID = "";

		
		[ProtoMember(8)]
		public int CurMTime = 0;

		
		[ProtoMember(9)]
		public int SongLiID = 0;

		
		[ProtoMember(10)]
		public int LoginGiftState = 0;

		
		[ProtoMember(11)]
		public int OnlineGiftState = 0;

		
		[ProtoMember(12)]
		public int LastLimitTimeHuoDongID = 0;

		
		[ProtoMember(13)]
		public int LastLimitTimeDayID = 0;

		
		[ProtoMember(14)]
		public int LimitTimeLoginNum = 0;

		
		[ProtoMember(15)]
		public int LimitTimeGiftState = 0;

		
		[ProtoMember(16)]
		public int EveryDayOnLineAwardStep = 0;

		
		[ProtoMember(17)]
		public int GetEveryDayOnLineAwardDayID = 0;

		
		[ProtoMember(18)]
		public int SeriesLoginGetAwardStep = 0;

		
		[ProtoMember(19)]
		public int SeriesLoginAwardDayID = 0;

		
		[ProtoMember(20)]
		public string SeriesLoginAwardGoodsID = "";

		
		[ProtoMember(21)]
		public string EveryDayOnLineAwardGoodsID = "";
	}
}
