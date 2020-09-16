﻿using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompBattleAwardsData
	{
		
		[ProtoMember(1)]
		public int RankNum;

		
		[ProtoMember(2)]
		public int AwardID;

		
		[ProtoMember(3)]
		public int WinNum;
	}
}
