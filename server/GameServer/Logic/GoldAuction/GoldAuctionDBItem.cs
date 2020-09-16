using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using ProtoBuf;

namespace GameServer.Logic.GoldAuction
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
			this.AuctionTime = TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss");
			this.ProductionTime = TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss.fff");
			this.BuyerData = new AuctionRoleData();
			this.RoleList = new List<AuctionRoleData>();
		}

		
		public int GetMaxmDamageID()
		{
			try
			{
				AuctionRoleData temp = new AuctionRoleData();
				foreach (AuctionRoleData item in this.RoleList)
				{
					if (item.Value > temp.Value)
					{
						temp = item;
					}
				}
				if (null != temp)
				{
					return temp.m_RoleID;
				}
			}
			catch
			{
			}
			return 0;
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
