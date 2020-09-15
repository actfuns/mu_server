using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000086 RID: 134
	[ProtoContract]
	public class PlayerMerlinRankingData : IComparable<PlayerMerlinRankingData>
	{
		// Token: 0x06000121 RID: 289 RVA: 0x00006B96 File Offset: 0x00004D96
		public PlayerMerlinRankingData()
		{
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00006BAC File Offset: 0x00004DAC
		public PlayerMerlinRankingData(MerlinRankingInfo MerlinData)
		{
			this.MerlinData = MerlinData;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00006BCC File Offset: 0x00004DCC
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.MerlinData.nRoleID;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00006BEC File Offset: 0x00004DEC
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.MerlinData.strRoleName;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000125 RID: 293 RVA: 0x00006C0C File Offset: 0x00004E0C
		[ProtoMember(3)]
		public int Occupation
		{
			get
			{
				return this.MerlinData.nOccupation;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00006C2C File Offset: 0x00004E2C
		[ProtoMember(4)]
		public string MerlinAddTime
		{
			get
			{
				return this.MerlinData.strAddTime;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000127 RID: 295 RVA: 0x00006C4C File Offset: 0x00004E4C
		[ProtoMember(5)]
		public int Level
		{
			get
			{
				return this.MerlinData.nLevel;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00006C6C File Offset: 0x00004E6C
		[ProtoMember(6)]
		public int StarNum
		{
			get
			{
				return this.MerlinData.nStarNum;
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00006C89 File Offset: 0x00004E89
		public void UpdateData(MerlinRankingInfo merlinData)
		{
			this.MerlinData = merlinData;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00006C94 File Offset: 0x00004E94
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.Level;
			this.paiHangItemData.Val2 = this.StarNum;
			this.paiHangItemData.Val3 = this.Occupation;
			return this.paiHangItemData;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00006D04 File Offset: 0x00004F04
		public int CompareTo(PlayerMerlinRankingData other)
		{
			int result;
			if (this.Level == other.Level)
			{
				if (this.StarNum == other.StarNum)
				{
					int nRet = string.Compare(this.MerlinAddTime, other.MerlinAddTime);
					result = ((nRet < 0) ? -1 : ((nRet == 0) ? 0 : 1));
				}
				else
				{
					result = ((this.StarNum < other.StarNum) ? 1 : -1);
				}
			}
			else
			{
				result = ((this.Level < other.Level) ? 1 : -1);
			}
			return result;
		}

		// Token: 0x040002DB RID: 731
		public MerlinRankingInfo MerlinData;

		// Token: 0x040002DC RID: 732
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
