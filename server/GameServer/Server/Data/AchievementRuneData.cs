using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class AchievementRuneData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int RuneID = 0;

		
		[ProtoMember(3)]
		public int LifeAdd = 0;

		
		[ProtoMember(4)]
		public int AttackAdd = 0;

		
		[ProtoMember(5)]
		public int DefenseAdd = 0;

		
		[ProtoMember(6)]
		public int DodgeAdd = 0;

		
		[ProtoMember(7)]
		public int Achievement = 0;

		
		[ProtoMember(8)]
		public int Diamond = 0;

		
		[ProtoMember(9)]
		public int BurstType = 0;

		
		[ProtoMember(10)]
		public int UpResultType = 0;

		
		[ProtoMember(11)]
		public int AchievementLeft = 0;
	}
}
