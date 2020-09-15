using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200030B RID: 779
	[ProtoContract]
	public class LingDiCaiJiData
	{
		// Token: 0x04001409 RID: 5129
		[ProtoMember(1)]
		public int LingDiType;

		// Token: 0x0400140A RID: 5130
		[ProtoMember(2)]
		public DateTime BeginTime;

		// Token: 0x0400140B RID: 5131
		[ProtoMember(3)]
		public DateTime EndTime;

		// Token: 0x0400140C RID: 5132
		[ProtoMember(4)]
		public bool HaveJunTuan;

		// Token: 0x0400140D RID: 5133
		[ProtoMember(5)]
		public string ZhanLingName;
	}
}
