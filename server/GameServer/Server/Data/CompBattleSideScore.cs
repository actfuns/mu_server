using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompBattleSideScore
	{
		
		[ProtoMember(1)]
		public int CompType;

		
		[ProtoMember(2)]
		public int ZhuJiangNum;

		
		[ProtoMember(3)]
		public HashSet<int> StrongholdSet = new HashSet<int>();

		
		public int Rank;

		
		public double Rate;
	}
}
