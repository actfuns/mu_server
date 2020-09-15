using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.Data;

namespace Server.Data
{
	// Token: 0x0200014C RID: 332
	[ProtoContract]
	public class JieriPlatChargeKingEverydayData
	{
		// Token: 0x0400076A RID: 1898
		[ProtoMember(1)]
		public long hasgettimes;

		// Token: 0x0400076B RID: 1899
		[ProtoMember(2)]
		public Dictionary<int, List<InputKingPaiHangData>> PaiHangDict;
	}
}
