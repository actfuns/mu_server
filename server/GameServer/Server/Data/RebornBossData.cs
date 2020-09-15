using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003DF RID: 991
	[ProtoContract]
	public class RebornBossData
	{
		// Token: 0x04001A61 RID: 6753
		[ProtoMember(1)]
		public int ExtensionID = 0;

		// Token: 0x04001A62 RID: 6754
		[ProtoMember(2)]
		public string NextTime = "";

		// Token: 0x04001A63 RID: 6755
		[ProtoMember(3)]
		public int AwardExtensionID = 0;

		// Token: 0x04001A64 RID: 6756
		[ProtoMember(4)]
		public int RankNum = 0;

		// Token: 0x04001A65 RID: 6757
		[ProtoMember(5)]
		public int BossKill = 0;
	}
}
