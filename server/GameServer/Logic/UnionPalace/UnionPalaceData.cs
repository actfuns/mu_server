using System;
using ProtoBuf;

namespace GameServer.Logic.UnionPalace
{
	
	[ProtoContract]
	public class UnionPalaceData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int StatueID = 0;

		
		[ProtoMember(3)]
		public int StatueLevel = 0;

		
		[ProtoMember(4)]
		public int LifeAdd = 0;

		
		[ProtoMember(5)]
		public int AttackAdd = 0;

		
		[ProtoMember(6)]
		public int DefenseAdd = 0;

		
		[ProtoMember(7)]
		public int AttackInjureAdd = 0;

		
		[ProtoMember(8)]
		public int ZhanGongNeed = 0;

		
		[ProtoMember(9)]
		public int BurstType = 0;

		
		[ProtoMember(10)]
		public int ResultType = 0;

		
		[ProtoMember(11)]
		public int ZhanGongLeft = 0;

		
		[ProtoMember(12)]
		public int UnionLevel = 0;

		
		[ProtoMember(13)]
		public int StatueType = 0;
	}
}
