using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020001C6 RID: 454
	[ProtoContract]
	public class SevenDayActQueryData
	{
		// Token: 0x04000A0C RID: 2572
		[ProtoMember(1)]
		public int ActivityType;

		// Token: 0x04000A0D RID: 2573
		[ProtoMember(2)]
		public Dictionary<int, SevenDayItemData> ItemDict;
	}
}
