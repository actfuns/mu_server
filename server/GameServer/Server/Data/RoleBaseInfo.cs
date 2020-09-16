using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleBaseInfo
	{
		
		[ProtoMember(1)]
		public string RoleName;

		
		[ProtoMember(2)]
		public string UserID;

		
		[ProtoMember(3)]
		public string Position;

		
		[ProtoMember(4)]
		public string Feeling;

		
		[ProtoMember(5)]
		public int RoleID;

		
		[ProtoMember(6)]
		public int Level;

		
		[ProtoMember(7)]
		public int CombatForce;

		
		[ProtoMember(8)]
		public int Admiredcount;

		
		[ProtoMember(9)]
		public long LoginTime;

		
		[ProtoMember(10)]
		public long LogoutTime;

		
		[ProtoMember(11)]
		public short ZoneID;

		
		[ProtoMember(12)]
		public byte ChangeLifeCount;

		
		[ProtoMember(13)]
		public byte Occupation;

		
		[ProtoMember(14)]
		public byte Sex;

		
		[ProtoMember(15)]
		public int RealMoney;
	}
}
