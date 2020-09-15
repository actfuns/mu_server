using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	// Token: 0x02000361 RID: 865
	[ProtoContract]
	public class CoupleArenaMainData
	{
		// Token: 0x040016E5 RID: 5861
		[ProtoMember(1)]
		public CoupleArenaCoupleJingJiData JingJiData;

		// Token: 0x040016E6 RID: 5862
		[ProtoMember(2)]
		public int WeekGetRongYaoTimes;

		// Token: 0x040016E7 RID: 5863
		[ProtoMember(3)]
		public int CanGetAwardId;
	}
}
