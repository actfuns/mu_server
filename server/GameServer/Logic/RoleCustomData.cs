using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020003FB RID: 1019
	[ProtoContract]
	public class RoleCustomData
	{
		// Token: 0x04001B32 RID: 6962
		[ProtoMember(1, IsRequired = true)]
		public int roleId;

		// Token: 0x04001B33 RID: 6963
		[ProtoMember(2, IsRequired = true)]
		public RoleData4Selector roleData4Selector;

		// Token: 0x04001B34 RID: 6964
		[ProtoMember(3, IsRequired = true)]
		public List<RoleCustomDataItem> customDataList = new List<RoleCustomDataItem>();
	}
}
