using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003E0 RID: 992
	[ProtoContract]
	public class RebornRankAdmireData
	{
		// Token: 0x04001A66 RID: 6758
		[ProtoMember(1)]
		public int AdmireCount;

		// Token: 0x04001A67 RID: 6759
		[ProtoMember(2)]
		public RoleData4Selector RoleData4Selector;

		// Token: 0x04001A68 RID: 6760
		[ProtoMember(3)]
		public int Value;

		// Token: 0x04001A69 RID: 6761
		[ProtoMember(4)]
		public int PtID;

		// Token: 0x04001A6A RID: 6762
		[ProtoMember(5)]
		public string Param;
	}
}
