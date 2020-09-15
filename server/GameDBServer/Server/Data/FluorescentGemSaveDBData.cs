using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000052 RID: 82
	[ProtoContract]
	public class FluorescentGemSaveDBData
	{
		// Token: 0x040001AA RID: 426
		[ProtoMember(1)]
		public int _RoleID;

		// Token: 0x040001AB RID: 427
		[ProtoMember(2)]
		public int _GoodsID;

		// Token: 0x040001AC RID: 428
		[ProtoMember(3)]
		public int _Position;

		// Token: 0x040001AD RID: 429
		[ProtoMember(4)]
		public int _GemType;

		// Token: 0x040001AE RID: 430
		[ProtoMember(5)]
		public int _Bind;
	}
}
