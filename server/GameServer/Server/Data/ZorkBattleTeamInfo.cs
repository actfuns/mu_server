using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZorkBattleTeamInfo
	{
		
		[ProtoMember(1)]
		public int TeamID;

		
		[ProtoMember(2)]
		public string TeamName;

		
		[ProtoMember(3)]
		public int JiFen;

		
		[ProtoMember(4)]
		public int BossInjurePct;

		
		public long BossInjure;

		
		public long BossInjureTicks;
	}
}
