using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000219 RID: 537
	[ProtoContract]
	public class BHMatchSupportData4Client
	{
		// Token: 0x04000C31 RID: 3121
		[ProtoMember(1)]
		public int round;

		// Token: 0x04000C32 RID: 3122
		[ProtoMember(2)]
		public int right;

		// Token: 0x04000C33 RID: 3123
		[ProtoMember(3)]
		public int jifen;

		// Token: 0x04000C34 RID: 3124
		public int season;
	}
}
