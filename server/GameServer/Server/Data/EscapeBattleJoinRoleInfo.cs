using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class EscapeBattleJoinRoleInfo
	{
		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2)]
		public string RoleName;

		
		[ProtoMember(3)]
		public int Level = 0;

		
		[ProtoMember(4)]
		public int ChangeLevel = 0;

		
		[ProtoMember(5)]
		public long CombatForce;

		
		[ProtoMember(6)]
		public bool Join;

		
		[ProtoMember(7)]
		public bool IsLeader;
	}
}
