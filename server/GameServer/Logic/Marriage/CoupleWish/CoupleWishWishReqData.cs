using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleWish
{
	
	[ProtoContract]
	public class CoupleWishWishReqData
	{
		
		[ProtoMember(1)]
		public bool IsWishRankRole;

		
		[ProtoMember(2)]
		public int ToRankCoupleId;

		
		[ProtoMember(3)]
		public string ToLocalRoleName;

		
		[ProtoMember(4)]
		public int WishType;

		
		[ProtoMember(5)]
		public string WishTxt;

		
		[ProtoMember(6)]
		public int CostType;

		
		public enum ECostType
		{
			
			Goods = 1,
			
			ZuanShi
		}
	}
}
