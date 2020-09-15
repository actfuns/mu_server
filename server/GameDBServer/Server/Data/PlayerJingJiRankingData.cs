using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000A2 RID: 162
	[ProtoContract]
	public class PlayerJingJiRankingData : IComparable<PlayerJingJiRankingData>
	{
		// Token: 0x0600017A RID: 378 RVA: 0x00007F9A File Offset: 0x0000619A
		public PlayerJingJiRankingData()
		{
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00007FB0 File Offset: 0x000061B0
		public PlayerJingJiRankingData(PlayerJingJiData jingjiData)
		{
			this.jingjiData = jingjiData;
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00007FD0 File Offset: 0x000061D0
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.jingjiData.roleId;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00007FF0 File Offset: 0x000061F0
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.jingjiData.roleName;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600017E RID: 382 RVA: 0x00008010 File Offset: 0x00006210
		[ProtoMember(3)]
		public int combatForce
		{
			get
			{
				return this.jingjiData.combatForce;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00008030 File Offset: 0x00006230
		// (set) Token: 0x06000180 RID: 384 RVA: 0x0000804D File Offset: 0x0000624D
		[ProtoMember(4)]
		public int ranking
		{
			get
			{
				return this.jingjiData.ranking;
			}
			set
			{
				this.jingjiData.ranking = value;
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000805C File Offset: 0x0000625C
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.ranking;
			this.paiHangItemData.Val2 = this.combatForce;
			return this.paiHangItemData;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x000080B8 File Offset: 0x000062B8
		public int CompareTo(PlayerJingJiRankingData other)
		{
			return (this.ranking < other.ranking) ? -1 : ((this.ranking == other.ranking) ? 0 : 1);
		}

		// Token: 0x04000394 RID: 916
		public PlayerJingJiData jingjiData;

		// Token: 0x04000395 RID: 917
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
