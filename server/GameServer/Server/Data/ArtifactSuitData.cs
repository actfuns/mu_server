using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000118 RID: 280
	[ProtoContract]
	public class ArtifactSuitData
	{
		// Token: 0x040005FD RID: 1533
		[ProtoMember(1)]
		public int SuitID = 0;

		// Token: 0x040005FE RID: 1534
		[ProtoMember(2)]
		public string SuitName = "";

		// Token: 0x040005FF RID: 1535
		[ProtoMember(3)]
		public List<int> EquipIDList = null;

		// Token: 0x04000600 RID: 1536
		[ProtoMember(4)]
		public bool IsMulti = false;

		// Token: 0x04000601 RID: 1537
		[ProtoMember(5)]
		public Dictionary<int, Dictionary<string, string>> SuitAttr = null;
	}
}
