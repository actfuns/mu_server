using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000098 RID: 152
	[ProtoContract]
	public class PlayerRingRankingData : IComparable<PlayerRingRankingData>
	{
		// Token: 0x06000151 RID: 337 RVA: 0x00007862 File Offset: 0x00005A62
		public PlayerRingRankingData()
		{
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00007878 File Offset: 0x00005A78
		public PlayerRingRankingData(RingRankingInfo ringData)
		{
			this.ringData = ringData;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00007898 File Offset: 0x00005A98
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.ringData.nRoleID;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000154 RID: 340 RVA: 0x000078B8 File Offset: 0x00005AB8
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.ringData.strRoleName;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000155 RID: 341 RVA: 0x000078D8 File Offset: 0x00005AD8
		[ProtoMember(3)]
		public int RingID
		{
			get
			{
				return this.ringData.nRingID;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000156 RID: 342 RVA: 0x000078F8 File Offset: 0x00005AF8
		[ProtoMember(4)]
		public string RingAddTime
		{
			get
			{
				return this.ringData.strAddTime;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00007918 File Offset: 0x00005B18
		[ProtoMember(5)]
		public int GoodWillLevel
		{
			get
			{
				return this.ringData.byGoodwilllevel;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00007938 File Offset: 0x00005B38
		[ProtoMember(6)]
		public int GoodWillStar
		{
			get
			{
				return this.ringData.byGoodwillstar;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00007955 File Offset: 0x00005B55
		public void UpdateData(RingRankingInfo data)
		{
			this.ringData = data;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007960 File Offset: 0x00005B60
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.GoodWillLevel;
			this.paiHangItemData.Val2 = this.GoodWillStar;
			this.paiHangItemData.Val3 = this.RingID;
			return this.paiHangItemData;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000079D0 File Offset: 0x00005BD0
		public int CompareTo(PlayerRingRankingData other)
		{
			int result;
			if (this.GoodWillLevel == other.GoodWillLevel)
			{
				if (this.GoodWillStar == other.GoodWillStar)
				{
					int nRet = string.Compare(this.RingAddTime, other.RingAddTime);
					result = ((nRet < 0) ? -1 : ((nRet == 0) ? 0 : 1));
				}
				else
				{
					result = ((this.GoodWillStar < other.GoodWillStar) ? 1 : -1);
				}
			}
			else
			{
				result = ((this.GoodWillLevel < other.GoodWillLevel) ? 1 : -1);
			}
			return result;
		}

		// Token: 0x04000372 RID: 882
		public RingRankingInfo ringData;

		// Token: 0x04000373 RID: 883
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
