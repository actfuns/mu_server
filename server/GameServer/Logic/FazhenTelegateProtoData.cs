using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x020004C5 RID: 1221
	[ProtoContract]
	public class FazhenTelegateProtoData
	{
		// Token: 0x04002056 RID: 8278
		[ProtoMember(1)]
		public int gateId = 0;

		// Token: 0x04002057 RID: 8279
		[ProtoMember(2)]
		public int DestMapCode = 0;
	}
}
