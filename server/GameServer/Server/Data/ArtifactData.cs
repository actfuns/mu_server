using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000116 RID: 278
	[ProtoContract]
	public class ArtifactData
	{
		// Token: 0x040005F1 RID: 1521
		[ProtoMember(1)]
		public int ArtifactID = 0;

		// Token: 0x040005F2 RID: 1522
		[ProtoMember(2)]
		public string ArtifactName = "";

		// Token: 0x040005F3 RID: 1523
		[ProtoMember(3)]
		public int NewEquitID = 0;

		// Token: 0x040005F4 RID: 1524
		[ProtoMember(4)]
		public int NeedEquitID = 0;

		// Token: 0x040005F5 RID: 1525
		[ProtoMember(5)]
		public Dictionary<int, int> NeedMaterial = null;

		// Token: 0x040005F6 RID: 1526
		[ProtoMember(6)]
		public int NeedGoldBind = 0;

		// Token: 0x040005F7 RID: 1527
		[ProtoMember(7)]
		public int NeedZaiZao = 0;

		// Token: 0x040005F8 RID: 1528
		[ProtoMember(8)]
		public Dictionary<int, int> FailMaterial = null;

		// Token: 0x040005F9 RID: 1529
		[ProtoMember(9)]
		public int SuccessRate = 0;
	}
}
