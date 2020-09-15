using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020004C6 RID: 1222
	[ProtoContract]
	public class FazhenMapProtoData
	{
		// Token: 0x04002058 RID: 8280
		[ProtoMember(1)]
		public int SrcMapCode = 0;

		// Token: 0x04002059 RID: 8281
		[ProtoMember(2)]
		public List<FazhenTelegateProtoData> listTelegate = null;
	}
}
