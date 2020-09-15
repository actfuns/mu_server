using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000560 RID: 1376
	[ProtoContract]
	public class JingJiChallengeInfoData
	{
		// Token: 0x0400250B RID: 9483
		[ProtoMember(1)]
		public int pkId;

		// Token: 0x0400250C RID: 9484
		[ProtoMember(2)]
		public int roleId;

		// Token: 0x0400250D RID: 9485
		[ProtoMember(3)]
		public int zhanbaoType;

		// Token: 0x0400250E RID: 9486
		[ProtoMember(4)]
		public string challengeName;

		// Token: 0x0400250F RID: 9487
		[ProtoMember(5)]
		public int value;
	}
}
