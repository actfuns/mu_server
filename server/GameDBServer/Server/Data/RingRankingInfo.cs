using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000097 RID: 151
	[ProtoContract]
	public class RingRankingInfo
	{
		// Token: 0x0600014F RID: 335 RVA: 0x00007828 File Offset: 0x00005A28
		public PlayerRingRankingData getPlayerRingRankingData()
		{
			return new PlayerRingRankingData(this);
		}

		// Token: 0x0400036B RID: 875
		private PlayerRingRankingData rankingData;

		// Token: 0x0400036C RID: 876
		[ProtoMember(1)]
		[DBMapping(ColumnName = "roleid")]
		public int nRoleID;

		// Token: 0x0400036D RID: 877
		[DBMapping(ColumnName = "rolename")]
		[ProtoMember(2)]
		public string strRoleName;

		// Token: 0x0400036E RID: 878
		[DBMapping(ColumnName = "goodwilllevel")]
		[ProtoMember(3)]
		public int byGoodwilllevel = 0;

		// Token: 0x0400036F RID: 879
		[ProtoMember(4)]
		[DBMapping(ColumnName = "goodwillstar")]
		public int byGoodwillstar = 0;

		// Token: 0x04000370 RID: 880
		[DBMapping(ColumnName = "ringid")]
		[ProtoMember(5)]
		public int nRingID;

		// Token: 0x04000371 RID: 881
		[ProtoMember(6)]
		[DBMapping(ColumnName = "changtime")]
		public string strAddTime = "";
	}
}
