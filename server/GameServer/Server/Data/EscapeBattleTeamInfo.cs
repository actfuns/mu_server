using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EscapeBattleTeamInfo
	{
		
		[ProtoMember(1)]
		public int TeamID;

		
		[ProtoMember(2)]
		public string TeamName;

		
		[ProtoMember(3)]
		public int LifeSeed;

		
		public int ZhanDuiKillNum;

		
		public int RankNum;
	}
}
