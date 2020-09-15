using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000135 RID: 309
	[ProtoContract]
	public class FluorescentGemUpTransferData
	{
		// Token: 0x040006C2 RID: 1730
		[ProtoMember(1)]
		public int _RoleID;

		// Token: 0x040006C3 RID: 1731
		[ProtoMember(2)]
		public int _UpType;

		// Token: 0x040006C4 RID: 1732
		[ProtoMember(3)]
		public int _BagIndex;

		// Token: 0x040006C5 RID: 1733
		[ProtoMember(4)]
		public int _Position;

		// Token: 0x040006C6 RID: 1734
		[ProtoMember(5)]
		public int _GemType;

		// Token: 0x040006C7 RID: 1735
		[ProtoMember(6)]
		public Dictionary<int, int> _DecGoodsDict;
	}
}
