using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EscapeBattleAwardsData
	{
		
		[ProtoMember(1)]
		public int Success;

		
		[ProtoMember(2)]
		public int RankNum;

		
		[ProtoMember(3)]
		public int AwardID;

		
		[ProtoMember(4)]
		public int ZhanDuiKillNum;

		
		[ProtoMember(5)]
		public int ModJiFen;

		
		[ProtoMember(6)]
		public int WinToDay;
	}
}
