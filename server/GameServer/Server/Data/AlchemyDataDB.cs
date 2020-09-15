using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200005C RID: 92
	[ProtoContract]
	public class AlchemyDataDB
	{
		// Token: 0x040001F7 RID: 503
		[ProtoMember(1)]
		public AlchemyData BaseData = new AlchemyData();

		// Token: 0x040001F8 RID: 504
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x040001F9 RID: 505
		[ProtoMember(3)]
		public Dictionary<int, int> HistCost = new Dictionary<int, int>();

		// Token: 0x040001FA RID: 506
		[ProtoMember(4)]
		public int ElementDayID = 0;

		// Token: 0x040001FB RID: 507
		[ProtoMember(5)]
		public string rollbackType = "";
	}
}
