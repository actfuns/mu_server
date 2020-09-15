using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000194 RID: 404
	[ProtoContract]
	public class YaoSaiMissionData
	{
		// Token: 0x04000935 RID: 2357
		[ProtoMember(1)]
		public int SiteID;

		// Token: 0x04000936 RID: 2358
		[ProtoMember(2)]
		public int MissionID;

		// Token: 0x04000937 RID: 2359
		[ProtoMember(3)]
		public int State;

		// Token: 0x04000938 RID: 2360
		[ProtoMember(4)]
		public string ZhiPaiJingLing;

		// Token: 0x04000939 RID: 2361
		[ProtoMember(5)]
		public DateTime StartTime;
	}
}
