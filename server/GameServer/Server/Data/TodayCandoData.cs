using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005A7 RID: 1447
	[ProtoContract]
	internal class TodayCandoData
	{
		// Token: 0x040028D3 RID: 10451
		[ProtoMember(1)]
		public int ID;

		// Token: 0x040028D4 RID: 10452
		[ProtoMember(2)]
		public int LeftCount = 0;
	}
}
