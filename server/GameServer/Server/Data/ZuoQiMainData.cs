using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000859 RID: 2137
	[ProtoContract]
	public class ZuoQiMainData
	{
		// Token: 0x040046B6 RID: 18102
		[ProtoMember(1)]
		public List<MountData> MountList;

		// Token: 0x040046B7 RID: 18103
		[ProtoMember(2)]
		public DateTime NextFreeTime;

		// Token: 0x040046B8 RID: 18104
		[ProtoMember(3)]
		public int MountLevel;
	}
}
