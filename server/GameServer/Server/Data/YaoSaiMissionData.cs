using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000801 RID: 2049
	[ProtoContract]
	public class YaoSaiMissionData
	{
		// Token: 0x040043E1 RID: 17377
		[ProtoMember(1)]
		public int SiteID;

		// Token: 0x040043E2 RID: 17378
		[ProtoMember(2)]
		public int MissionID;

		// Token: 0x040043E3 RID: 17379
		[ProtoMember(3)]
		public int State;

		// Token: 0x040043E4 RID: 17380
		[ProtoMember(4)]
		public string ZhiPaiJingLing;

		// Token: 0x040043E5 RID: 17381
		[ProtoMember(5)]
		public DateTime StartTime;
	}
}
