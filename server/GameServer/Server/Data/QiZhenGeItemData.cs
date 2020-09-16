using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class QiZhenGeItemData
	{
		
		[ProtoMember(1)]
		public int ItemID = 0;

		
		[ProtoMember(2)]
		public int GoodsID = 0;

		
		[ProtoMember(3)]
		public int OrigPrice = 0;

		
		[ProtoMember(4)]
		public int Price = 0;

		
		[ProtoMember(5)]
		public string Description = "";

		
		[ProtoMember(6)]
		public int BaseProbability = 0;

		
		[ProtoMember(7)]
		public int SelfProbability = 0;
	}
}
