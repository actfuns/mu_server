using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JingJiDetailData
	{
		
		[ProtoMember(1)]
		public int state;

		
		[ProtoMember(2)]
		public int freeChallengeNum;

		
		[ProtoMember(3)]
		public int useFreeChallengeNum;

		
		[ProtoMember(4)]
		public int vipChallengeNum;

		
		[ProtoMember(5)]
		public int useVipChallengeNum;

		
		[ProtoMember(6)]
		public int winCount = 0;

		
		[ProtoMember(7)]
		public int ranking = -1;

		
		[ProtoMember(8)]
		public long nextRewardTime = 0L;

		
		[ProtoMember(9)]
		public long nextChallengeTime = 0L;

		
		[ProtoMember(10)]
		public List<PlayerJingJiMiniData> beChallengerData;

		
		[ProtoMember(11)]
		public int maxwincount;
	}
}
