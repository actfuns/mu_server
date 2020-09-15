using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003BD RID: 957
	[ProtoContract]
	public class LangHunLingYuKingShowData
	{
		// Token: 0x04001912 RID: 6418
		[ProtoMember(1)]
		public int AdmireCount;

		// Token: 0x04001913 RID: 6419
		[ProtoMember(2)]
		public RoleData4Selector RoleData4Selector;
	}
}
