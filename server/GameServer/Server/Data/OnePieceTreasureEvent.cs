using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000399 RID: 921
	[ProtoContract]
	public class OnePieceTreasureEvent
	{
		// Token: 0x04001845 RID: 6213
		[ProtoMember(1)]
		public int EventID = 0;

		// Token: 0x04001846 RID: 6214
		[ProtoMember(2)]
		public int EventValue = 0;

		// Token: 0x04001847 RID: 6215
		[ProtoMember(3)]
		public List<int> BoxIDList = null;

		// Token: 0x04001848 RID: 6216
		[ProtoMember(4)]
		public int ErrCode = 0;
	}
}
