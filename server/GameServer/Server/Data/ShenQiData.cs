using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ShenQiData
	{
		
		[ProtoMember(1)]
		public int ShenQiID;

		
		[ProtoMember(2)]
		public int LifeAdd;

		
		[ProtoMember(3)]
		public int AttackAdd;

		
		[ProtoMember(4)]
		public int DefenseAdd;

		
		[ProtoMember(5)]
		public int ToughnessAdd;

		
		[ProtoMember(6)]
		public int BurstType;

		
		[ProtoMember(7)]
		public int UpResultType;

		
		[ProtoMember(8)]
		public int ShenLiJingHuaLeft;
	}
}
