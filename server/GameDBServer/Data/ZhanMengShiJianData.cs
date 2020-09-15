using System;
using GameDBServer.DB;
using ProtoBuf;

namespace GameDBServer.Data
{
	// Token: 0x020000D6 RID: 214
	[ProtoContract]
	public class ZhanMengShiJianData
	{
		// Token: 0x040005DC RID: 1500
		[DBMapping(ColumnName = "pkId")]
		public int PKId = 0;

		// Token: 0x040005DD RID: 1501
		[ProtoMember(1)]
		[DBMapping(ColumnName = "bhId")]
		public int BHID = 0;

		// Token: 0x040005DE RID: 1502
		[ProtoMember(2)]
		[DBMapping(ColumnName = "shijianType")]
		public int ShiJianType = 0;

		// Token: 0x040005DF RID: 1503
		[DBMapping(ColumnName = "roleName")]
		[ProtoMember(3)]
		public string RoleName = "";

		// Token: 0x040005E0 RID: 1504
		[DBMapping(ColumnName = "createTime")]
		[ProtoMember(4)]
		public string CreateTime = "";

		// Token: 0x040005E1 RID: 1505
		[DBMapping(ColumnName = "subValue1")]
		[ProtoMember(5)]
		public int SubValue1 = -1;

		// Token: 0x040005E2 RID: 1506
		[ProtoMember(6)]
		[DBMapping(ColumnName = "subValue2")]
		public int SubValue2 = -1;

		// Token: 0x040005E3 RID: 1507
		[ProtoMember(7)]
		[DBMapping(ColumnName = "subValue3")]
		public int SubValue3 = -1;

		// Token: 0x040005E4 RID: 1508
		[DBMapping(ColumnName = "subSzValue1")]
		[ProtoMember(8)]
		public string SubSzValue1 = "";
	}
}
