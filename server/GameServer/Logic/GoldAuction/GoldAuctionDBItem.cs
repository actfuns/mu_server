using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using ProtoBuf;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x0200009D RID: 157
	[ProtoContract]
	public class GoldAuctionDBItem
	{
		// Token: 0x06000274 RID: 628 RVA: 0x0002A91C File Offset: 0x00028B1C
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

		// Token: 0x06000275 RID: 629 RVA: 0x0002A9A0 File Offset: 0x00028BA0
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

		// Token: 0x040003B0 RID: 944
		[ProtoMember(1)]
		public string AuctionTime;

		// Token: 0x040003B1 RID: 945
		[ProtoMember(2)]
		public int AuctionType;

		// Token: 0x040003B2 RID: 946
		[ProtoMember(3)]
		public int AuctionSource;

		// Token: 0x040003B3 RID: 947
		[ProtoMember(4)]
		public string ProductionTime;

		// Token: 0x040003B4 RID: 948
		[ProtoMember(5)]
		public string StrGoods;

		// Token: 0x040003B5 RID: 949
		[ProtoMember(6)]
		public List<AuctionRoleData> RoleList;

		// Token: 0x040003B6 RID: 950
		[ProtoMember(7)]
		public long BossLife;

		// Token: 0x040003B7 RID: 951
		[ProtoMember(8)]
		public int KillBossRoleID;

		// Token: 0x040003B8 RID: 952
		[ProtoMember(9)]
		public int UpDBWay;

		// Token: 0x040003B9 RID: 953
		[ProtoMember(10)]
		public int OldAuctionType;

		// Token: 0x040003BA RID: 954
		[ProtoMember(11)]
		public AuctionRoleData BuyerData;
	}
}
