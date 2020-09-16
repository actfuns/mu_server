using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class FriendData
	{
		
		[ProtoMember(1)]
		public int DbID;

		
		[ProtoMember(2)]
		public int OtherRoleID;

		
		[ProtoMember(3)]
		public string OtherRoleName;

		
		[ProtoMember(4)]
		public int OtherLevel;

		
		[ProtoMember(5)]
		public int Occupation;

		
		[ProtoMember(6)]
		public int OnlineState;

		
		[ProtoMember(7)]
		public string Position;

		
		[ProtoMember(8)]
		public int FriendType;

		
		[ProtoMember(9)]
		public int FriendChangeLifeLev;

		
		[ProtoMember(10)]
		public int FriendCombatForce;

		
		[ProtoMember(11)]
		public int SpouseId;

		
		[ProtoMember(12)]
		public int YaoSaiBossState;

		
		[ProtoMember(13)]
		public int YaoSaiJianYuState;

		
		[ProtoMember(14)]
		public int ZhanDuiID;
	}
}
