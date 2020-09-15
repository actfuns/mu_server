using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000233 RID: 563
	[ProtoContract]
	public class OldCaiJiData
	{
		// Token: 0x04000D43 RID: 3395
		[ProtoMember(1)]
		public int OldDay = -1;

		// Token: 0x04000D44 RID: 3396
		[ProtoMember(2)]
		public int OldNum = -1;
	}
}
