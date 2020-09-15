using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000085 RID: 133
	[ProtoContract]
	public class MerlinRankingInfo
	{
		// Token: 0x0600011F RID: 287 RVA: 0x00006B5C File Offset: 0x00004D5C
		public PlayerMerlinRankingData getPlayerMerlinRankingData()
		{
			return new PlayerMerlinRankingData(this);
		}

		// Token: 0x040002D4 RID: 724
		private PlayerMerlinRankingData rankingData;

		// Token: 0x040002D5 RID: 725
		[ProtoMember(1)]
		[DBMapping(ColumnName = "roleID")]
		public int nRoleID;

		// Token: 0x040002D6 RID: 726
		[ProtoMember(2)]
		[DBMapping(ColumnName = "roleName")]
		public string strRoleName;

		// Token: 0x040002D7 RID: 727
		[DBMapping(ColumnName = "occupation")]
		[ProtoMember(3)]
		public int nOccupation;

		// Token: 0x040002D8 RID: 728
		[ProtoMember(4)]
		[DBMapping(ColumnName = "level")]
		public int nLevel = 0;

		// Token: 0x040002D9 RID: 729
		[ProtoMember(5)]
		[DBMapping(ColumnName = "starNum")]
		public int nStarNum = 0;

		// Token: 0x040002DA RID: 730
		[DBMapping(ColumnName = "addTime")]
		[ProtoMember(6)]
		public string strAddTime = "";
	}
}
