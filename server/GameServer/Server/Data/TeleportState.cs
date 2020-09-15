using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000161 RID: 353
	[ProtoContract]
	public class TeleportState
	{
		// Token: 0x040007C5 RID: 1989
		[ProtoMember(1)]
		public int ToMapCode;

		// Token: 0x040007C6 RID: 1990
		[ProtoMember(2)]
		public int State;
	}
}
