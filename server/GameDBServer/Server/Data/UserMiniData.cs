using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class UserMiniData
	{
		
		[ProtoMember(1)]
		public string UserId;

		
		[ProtoMember(2)]
		public int LastRoleId;

		
		[ProtoMember(3)]
		public int RealMoney;

		
		[ProtoMember(4)]
		public DateTime MinCreateRoleTime;

		
		[ProtoMember(5)]
		public DateTime LastLoginTime;

		
		[ProtoMember(6)]
		public DateTime LastLogoutTime;

		
		[ProtoMember(7)]
		public DateTime RoleCreateTime;

		
		[ProtoMember(8)]
		public DateTime RoleLastLoginTime;

		
		[ProtoMember(9)]
		public DateTime RoleLastLogoutTime;

		
		[ProtoMember(10)]
		public int MaxLevel;

		
		[ProtoMember(11)]
		public int MaxChangeLifeCount;
	}
}
