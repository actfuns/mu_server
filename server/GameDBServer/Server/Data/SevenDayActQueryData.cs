using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200010B RID: 267
	[ProtoContract]
	public class SevenDayActQueryData
	{
		// Token: 0x0400073F RID: 1855
		[ProtoMember(1)]
		public int ActivityType;

		// Token: 0x04000740 RID: 1856
		[ProtoMember(2)]
		public Dictionary<int, SevenDayItemData> ItemDict;
	}
}
