using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Logic.GoldAuction
{
	
	[ProtoContract]
	public class GoldAuctionDBItem
	{
		
		public GoldAuctionDBItem()
		{
			this.AuctionType = 0;
			this.OldAuctionType = 0;
			this.AuctionSource = 0;
			this.StrGoods = "";
			this.BossLife = 0L;
			this.KillBossRoleID = 0;
			this.UpDBWay = 0;
			this.AuctionTime = "";
			this.ProductionTime = "";
			this.BuyerData = new AuctionRoleData();
			this.RoleList = new List<AuctionRoleData>();
		}

		
		[ProtoMember(1)]
		public string AuctionTime;

		
		[ProtoMember(2)]
		public int AuctionType;

		
		[ProtoMember(3)]
		public int AuctionSource;

		
		[ProtoMember(4)]
		public string ProductionTime;

		
		[ProtoMember(5)]
		public string StrGoods;

		
		[ProtoMember(6)]
		public List<AuctionRoleData> RoleList;

		
		[ProtoMember(7)]
		public long BossLife;

		
		[ProtoMember(8)]
		public int KillBossRoleID;

		
		[ProtoMember(9)]
		public int UpDBWay;

		
		[ProtoMember(10)]
		public int OldAuctionType;

		
		[ProtoMember(11)]
		public AuctionRoleData BuyerData;
	}
}
