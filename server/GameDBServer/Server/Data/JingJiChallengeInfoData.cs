using System;
using GameDBServer.DB;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000076 RID: 118
	[ProtoContract]
	public class JingJiChallengeInfoData
	{
		// Token: 0x0400028E RID: 654
		[DBMapping(ColumnName = "pkId")]
		[ProtoMember(1)]
		public int pkId;

		// Token: 0x0400028F RID: 655
		[ProtoMember(2)]
		[DBMapping(ColumnName = "roleId")]
		public int roleId;

		// Token: 0x04000290 RID: 656
		[ProtoMember(3)]
		[DBMapping(ColumnName = "zhanbaoType")]
		public int zhanbaoType;

		// Token: 0x04000291 RID: 657
		[ProtoMember(4)]
		[DBMapping(ColumnName = "challengeName")]
		public string challengeName;

		// Token: 0x04000292 RID: 658
		[DBMapping(ColumnName = "value")]
		[ProtoMember(5)]
		public int value;

		// Token: 0x04000293 RID: 659
		[DBMapping(ColumnName = "createTime")]
		public string createTime;
	}
}
