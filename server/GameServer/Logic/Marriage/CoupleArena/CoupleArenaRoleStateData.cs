using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	[ProtoContract]
	public class CoupleArenaRoleStateData
	{
		
		[ProtoMember(1)]
		public int RoleId;

		
		[ProtoMember(2)]
		public int MatchState;
	}
}
