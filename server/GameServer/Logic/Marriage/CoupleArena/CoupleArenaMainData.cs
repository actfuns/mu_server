using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	[ProtoContract]
	public class CoupleArenaMainData
	{
		
		[ProtoMember(1)]
		public CoupleArenaCoupleJingJiData JingJiData;

		
		[ProtoMember(2)]
		public int WeekGetRongYaoTimes;

		
		[ProtoMember(3)]
		public int CanGetAwardId;
	}
}
