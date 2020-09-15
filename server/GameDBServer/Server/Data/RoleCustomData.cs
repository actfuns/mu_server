using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200016B RID: 363
	[ProtoContract]
	public class RoleCustomData
	{
		// Token: 0x0400088E RID: 2190
		[ProtoMember(1, IsRequired = true)]
		public int roleId;

		// Token: 0x0400088F RID: 2191
		[ProtoMember(2, IsRequired = true)]
		public RoleData4Selector roleData4Selector;

		// Token: 0x04000890 RID: 2192
		[ProtoMember(3, IsRequired = true)]
		public List<RoleCustomDataItem> customDataList;
	}
}
