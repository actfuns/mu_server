using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic.Marriage.CoupleWish
{
	
	[ProtoContract]
	public class CoupleWishMainData : ICompressed
	{
		
		[ProtoMember(1)]
		public List<CoupleWishCoupleData> RankList;

		
		[ProtoMember(2)]
		public int MyCoupleRank;

		
		[ProtoMember(3)]
		public int MyCoupleBeWishNum;

		
		[ProtoMember(4)]
		public int CanGetAwardId;

		
		[ProtoMember(5)]
		public RoleData4Selector MyCoupleManSelector;

		
		[ProtoMember(6)]
		public RoleData4Selector MyCoupleWifeSelector;
	}
}
