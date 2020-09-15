using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020003C1 RID: 961
	[ProtoContract]
	public class LangHunLingYuQiZhiBuffOwnerData
	{
		// Token: 0x0400191E RID: 6430
		[ProtoMember(1)]
		public int NPCID;

		// Token: 0x0400191F RID: 6431
		[ProtoMember(2)]
		public int OwnerBHID;

		// Token: 0x04001920 RID: 6432
		[ProtoMember(3)]
		public string OwnerBHName;

		// Token: 0x04001921 RID: 6433
		[ProtoMember(4)]
		public int OwnerBHZoneId = 0;
	}
}
