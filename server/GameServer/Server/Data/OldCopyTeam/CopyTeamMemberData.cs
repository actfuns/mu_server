using System;
using ProtoBuf;

namespace Server.Data.OldCopyTeam
{
	
	[ProtoContract]
	public class CopyTeamMemberData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName;

		
		[ProtoMember(3)]
		public int RoleSex = 0;

		
		[ProtoMember(4)]
		public int Level = 0;

		
		[ProtoMember(5)]
		public int Occupation = 0;

		
		[ProtoMember(6)]
		public int RolePic = 0;

		
		[ProtoMember(7)]
		public int MapCode = 0;

		
		[ProtoMember(8)]
		public int OnlineState = 0;

		
		[ProtoMember(9)]
		public int MaxLifeV = 0;

		
		[ProtoMember(10)]
		public int CurrentLifeV = 0;

		
		[ProtoMember(11)]
		public int MaxMagicV = 0;

		
		[ProtoMember(12)]
		public int CurrentMagicV = 0;

		
		[ProtoMember(13)]
		public int PosX = 0;

		
		[ProtoMember(14)]
		public int PosY = 0;

		
		[ProtoMember(15)]
		public int CombatForce = 0;

		
		[ProtoMember(16)]
		public int ChangeLifeLev = 0;

		
		[ProtoMember(17)]
		public bool IsReady = false;
	}
}
