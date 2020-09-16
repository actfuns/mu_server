using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class HuanYingSiYuanAwardsData
	{
		
		[ProtoMember(1)]
		public int SuccessSide;

		
		[ProtoMember(2)]
		public long Exp;

		
		[ProtoMember(3)]
		public int ShengWang;

		
		[ProtoMember(4)]
		public int ChengJiuAward;

		
		[ProtoMember(5)]
		public int AwardsRate;

		
		[ProtoMember(6)]
		public string AwardGoods;
	}
}
