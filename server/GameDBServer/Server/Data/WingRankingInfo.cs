using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200009A RID: 154
	[ProtoContract]
	public class WingRankingInfo
	{
		// Token: 0x06000165 RID: 357 RVA: 0x00007BCC File Offset: 0x00005DCC
		public PlayerWingRankingData getPlayerWingRankingData()
		{
			return new PlayerWingRankingData(this);
		}

		// Token: 0x04000376 RID: 886
		private PlayerWingRankingData rankingData;

		// Token: 0x04000377 RID: 887
		[DBMapping(ColumnName = "rid")]
		[ProtoMember(1)]
		public int nRoleID;

		// Token: 0x04000378 RID: 888
		[DBMapping(ColumnName = "rname")]
		[ProtoMember(2)]
		public string strRoleName;

		// Token: 0x04000379 RID: 889
		[DBMapping(ColumnName = "occupation")]
		[ProtoMember(3)]
		public int nOccupation;

		// Token: 0x0400037A RID: 890
		[DBMapping(ColumnName = "wingid")]
		[ProtoMember(4)]
		public int nWingID = 0;

		// Token: 0x0400037B RID: 891
		[DBMapping(ColumnName = "forgeLevel")]
		[ProtoMember(5)]
		public int nStarNum = 0;

		// Token: 0x0400037C RID: 892
		[DBMapping(ColumnName = "addtime")]
		[ProtoMember(6)]
		public string strAddTime = "";
	}
}
