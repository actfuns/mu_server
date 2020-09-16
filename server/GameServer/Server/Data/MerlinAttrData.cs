using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MerlinAttrData
	{
		
		[ProtoMember(1)]
		public int _MinAttackV = 0;

		
		[ProtoMember(2)]
		public int _MaxAttackV = 0;

		
		[ProtoMember(3)]
		public int _MinMAttackV = 0;

		
		[ProtoMember(4)]
		public int _MaxMAttackV = 0;

		
		[ProtoMember(5)]
		public int _MinDefenseV = 0;

		
		[ProtoMember(6)]
		public int _MaxDefenseV = 0;

		
		[ProtoMember(7)]
		public int _MinMDefenseV = 0;

		
		[ProtoMember(8)]
		public int _MaxMDefenseV = 0;

		
		[ProtoMember(9)]
		public int _HitV = 0;

		
		[ProtoMember(10)]
		public int _DodgeV = 0;

		
		[ProtoMember(11)]
		public int _MaxHpV = 0;

		
		[ProtoMember(12)]
		public double _ReviveP = 0.0;

		
		[ProtoMember(13)]
		public double _MpRecoverP = 0.0;
	}
}
