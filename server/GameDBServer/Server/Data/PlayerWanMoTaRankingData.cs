using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000099 RID: 153
	[ProtoContract]
	public class PlayerWanMoTaRankingData : IComparable<PlayerWanMoTaRankingData>
	{
		// Token: 0x0600015C RID: 348 RVA: 0x00007A5E File Offset: 0x00005C5E
		public PlayerWanMoTaRankingData()
		{
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00007A74 File Offset: 0x00005C74
		public PlayerWanMoTaRankingData(WanMotaInfo wanmotaData)
		{
			this.wanmotaData = wanmotaData;
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00007A94 File Offset: 0x00005C94
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.wanmotaData.nRoleID;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00007AB4 File Offset: 0x00005CB4
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.wanmotaData.strRoleName;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00007AD4 File Offset: 0x00005CD4
		[ProtoMember(3)]
		public long flushTime
		{
			get
			{
				return this.wanmotaData.lFlushTime;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00007AF4 File Offset: 0x00005CF4
		[ProtoMember(4)]
		public int passLayerCount
		{
			get
			{
				return this.wanmotaData.nPassLayerCount;
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00007B11 File Offset: 0x00005D11
		public void UpdateData(WanMotaInfo data)
		{
			this.wanmotaData = data;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00007B1C File Offset: 0x00005D1C
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.passLayerCount;
			return this.paiHangItemData;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00007B68 File Offset: 0x00005D68
		public int CompareTo(PlayerWanMoTaRankingData other)
		{
			int result;
			if (this.passLayerCount == other.passLayerCount)
			{
				result = ((this.flushTime < other.flushTime) ? -1 : ((this.flushTime == other.flushTime) ? 0 : 1));
			}
			else
			{
				result = ((this.passLayerCount < other.passLayerCount) ? 1 : -1);
			}
			return result;
		}

		// Token: 0x04000374 RID: 884
		public WanMotaInfo wanmotaData;

		// Token: 0x04000375 RID: 885
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
