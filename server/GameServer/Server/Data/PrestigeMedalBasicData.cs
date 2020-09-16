using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PrestigeMedalBasicData
	{
		
		[ProtoMember(1)]
		public int MedalID = 0;

		
		[ProtoMember(2)]
		public string MedalName = "";

		
		[ProtoMember(3)]
		public int LifeMax = 0;

		
		[ProtoMember(4)]
		public int AttackMax = 0;

		
		[ProtoMember(5)]
		public int DefenseMax = 0;

		
		[ProtoMember(6)]
		public int HitMax = 0;

		
		[ProtoMember(7)]
		public int PrestigeCost = 0;

		
		[ProtoMember(8)]
		public List<int> RateList;

		
		public List<int[]> AddNumList;
	}
}
