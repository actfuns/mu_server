using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200021A RID: 538
	[ProtoContract]
	public class BangHuiMatchScoreData
	{
		// Token: 0x04000C35 RID: 3125
		[ProtoMember(1)]
		public int PlayerNum1;

		// Token: 0x04000C36 RID: 3126
		[ProtoMember(2)]
		public int PlayerNum2;

		// Token: 0x04000C37 RID: 3127
		[ProtoMember(3)]
		public string LT_BHName;

		// Token: 0x04000C38 RID: 3128
		[ProtoMember(4)]
		public int QiZhi1;

		// Token: 0x04000C39 RID: 3129
		[ProtoMember(5)]
		public int QiZhi2;

		// Token: 0x04000C3A RID: 3130
		[ProtoMember(6)]
		public int Score1;

		// Token: 0x04000C3B RID: 3131
		[ProtoMember(7)]
		public int Score2;
	}
}
