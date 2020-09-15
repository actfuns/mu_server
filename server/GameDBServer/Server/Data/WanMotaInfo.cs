using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200007D RID: 125
	[ProtoContract]
	public class WanMotaInfo
	{
		// Token: 0x06000117 RID: 279 RVA: 0x00006994 File Offset: 0x00004B94
		public PlayerWanMoTaRankingData getPlayerWanMoTaRankingData()
		{
			return new PlayerWanMoTaRankingData(this);
		}

		// Token: 0x040002A2 RID: 674
		private PlayerWanMoTaRankingData rankingData;

		// Token: 0x040002A3 RID: 675
		[ProtoMember(1)]
		[DBMapping(ColumnName = "roleID")]
		public int nRoleID;

		// Token: 0x040002A4 RID: 676
		[DBMapping(ColumnName = "roleName")]
		[ProtoMember(2)]
		public string strRoleName;

		// Token: 0x040002A5 RID: 677
		[DBMapping(ColumnName = "flushTime")]
		[ProtoMember(3)]
		public long lFlushTime = 0L;

		// Token: 0x040002A6 RID: 678
		[DBMapping(ColumnName = "passLayerCount")]
		[ProtoMember(4)]
		public int nPassLayerCount = 0;

		// Token: 0x040002A7 RID: 679
		[ProtoMember(5)]
		[DBMapping(ColumnName = "sweepLayer")]
		public int nSweepLayer = 0;

		// Token: 0x040002A8 RID: 680
		[ProtoMember(6)]
		[DBMapping(ColumnName = "sweepReward")]
		public string strSweepReward = "";

		// Token: 0x040002A9 RID: 681
		[ProtoMember(7)]
		[DBMapping(ColumnName = "sweepBeginTime")]
		public long lSweepBeginTime = 0L;

		// Token: 0x040002AA RID: 682
		[ProtoMember(8)]
		public int nTopPassLayerCount;
	}
}
