using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000095 RID: 149
	[ProtoContract]
	public class PetData
	{
		// Token: 0x04000360 RID: 864
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x04000361 RID: 865
		[ProtoMember(2)]
		public int PetID = 0;

		// Token: 0x04000362 RID: 866
		[ProtoMember(3)]
		public string PetName = "";

		// Token: 0x04000363 RID: 867
		[ProtoMember(4)]
		public int PetType = 0;

		// Token: 0x04000364 RID: 868
		[ProtoMember(5)]
		public int FeedNum = 0;

		// Token: 0x04000365 RID: 869
		[ProtoMember(6)]
		public int ReAliveNum = 0;

		// Token: 0x04000366 RID: 870
		[ProtoMember(7)]
		public long AddDateTime = 0L;

		// Token: 0x04000367 RID: 871
		[ProtoMember(8)]
		public string PetProps = "";

		// Token: 0x04000368 RID: 872
		[ProtoMember(9)]
		public int Level = 1;
	}
}
