using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000217 RID: 535
	[ProtoContract]
	public class BHMatchKingShowData
	{
		// Token: 0x04000C28 RID: 3112
		[ProtoMember(1)]
		public int AdmireCount;

		// Token: 0x04000C29 RID: 3113
		[ProtoMember(2)]
		public RoleData4Selector RoleData4Selector;
	}
}
