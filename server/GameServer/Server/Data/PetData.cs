using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000579 RID: 1401
	[ProtoContract]
	public class PetData
	{
		// Token: 0x040025C9 RID: 9673
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x040025CA RID: 9674
		[ProtoMember(2)]
		public int PetID = 0;

		// Token: 0x040025CB RID: 9675
		[ProtoMember(3)]
		public string PetName = "";

		// Token: 0x040025CC RID: 9676
		[ProtoMember(4)]
		public int PetType = 0;

		// Token: 0x040025CD RID: 9677
		[ProtoMember(5)]
		public int FeedNum = 0;

		// Token: 0x040025CE RID: 9678
		[ProtoMember(6)]
		public int ReAliveNum = 0;

		// Token: 0x040025CF RID: 9679
		[ProtoMember(7)]
		public long AddDateTime = 0L;

		// Token: 0x040025D0 RID: 9680
		[ProtoMember(8)]
		public string PetProps = "";

		// Token: 0x040025D1 RID: 9681
		[ProtoMember(9)]
		public int Level = 1;
	}
}
