using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000136 RID: 310
	[ProtoContract]
	public class FluorescentGemEquipChangesTransferData
	{
		// Token: 0x040006C8 RID: 1736
		[ProtoMember(1)]
		public int _Position;

		// Token: 0x040006C9 RID: 1737
		[ProtoMember(2)]
		public int _GemType;

		// Token: 0x040006CA RID: 1738
		[ProtoMember(3)]
		public GoodsData _GoodsData = null;
	}
}
