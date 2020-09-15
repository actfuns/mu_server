using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005B4 RID: 1460
	[ProtoContract]
	public class ListRolesData
	{
		// Token: 0x0400290B RID: 10507
		[ProtoMember(1)]
		public int StartIndex = 0;

		// Token: 0x0400290C RID: 10508
		[ProtoMember(2)]
		public int TotalRolesCount = 0;

		// Token: 0x0400290D RID: 10509
		[ProtoMember(3)]
		public int PageRolesCount = 0;

		// Token: 0x0400290E RID: 10510
		[ProtoMember(4)]
		public List<SearchRoleData> SearchRoleDataList = null;
	}
}
