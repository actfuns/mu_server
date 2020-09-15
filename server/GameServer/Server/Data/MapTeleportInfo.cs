using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000160 RID: 352
	[ProtoContract]
	public class MapTeleportInfo
	{
		// Token: 0x040007C3 RID: 1987
		[ProtoMember(1)]
		public int MapCode = 0;

		// Token: 0x040007C4 RID: 1988
		[ProtoMember(2)]
		public List<TeleportState> TeleportStateList;
	}
}
