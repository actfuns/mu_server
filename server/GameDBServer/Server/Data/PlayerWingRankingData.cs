using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200009B RID: 155
	[ProtoContract]
	public class PlayerWingRankingData : IComparable<PlayerWingRankingData>
	{
		// Token: 0x06000167 RID: 359 RVA: 0x00007C06 File Offset: 0x00005E06
		public PlayerWingRankingData()
		{
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00007C1C File Offset: 0x00005E1C
		public PlayerWingRankingData(WingRankingInfo wingData)
		{
			this.wingData = wingData;
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00007C3C File Offset: 0x00005E3C
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.wingData.nRoleID;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600016A RID: 362 RVA: 0x00007C5C File Offset: 0x00005E5C
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.wingData.strRoleName;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600016B RID: 363 RVA: 0x00007C7C File Offset: 0x00005E7C
		[ProtoMember(3)]
		public int Occupation
		{
			get
			{
				return this.wingData.nOccupation;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00007C9C File Offset: 0x00005E9C
		[ProtoMember(4)]
		public string WingAddTime
		{
			get
			{
				return this.wingData.strAddTime;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00007CBC File Offset: 0x00005EBC
		[ProtoMember(5)]
		public int WingID
		{
			get
			{
				return this.wingData.nWingID;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00007CDC File Offset: 0x00005EDC
		[ProtoMember(6)]
		public int WingStarNum
		{
			get
			{
				return this.wingData.nStarNum;
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00007CFC File Offset: 0x00005EFC
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.WingID;
			this.paiHangItemData.Val2 = this.WingStarNum;
			this.paiHangItemData.Val3 = this.Occupation;
			return this.paiHangItemData;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00007D69 File Offset: 0x00005F69
		public void UpdateData(WingRankingInfo data)
		{
			this.wingData = data;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00007D74 File Offset: 0x00005F74
		public int CompareTo(PlayerWingRankingData other)
		{
			int result;
			if (this.WingID == other.WingID)
			{
				if (this.WingStarNum == other.WingStarNum)
				{
					int nRet = string.Compare(this.WingAddTime, other.WingAddTime);
					result = ((nRet < 0) ? -1 : ((nRet == 0) ? 0 : 1));
				}
				else
				{
					result = ((this.WingStarNum < other.WingStarNum) ? 1 : -1);
				}
			}
			else
			{
				result = ((this.WingID < other.WingID) ? 1 : -1);
			}
			return result;
		}

		// Token: 0x0400037D RID: 893
		public WingRankingInfo wingData;

		// Token: 0x0400037E RID: 894
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
