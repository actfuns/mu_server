using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000030 RID: 48
	[ProtoContract]
	public class EverydayActInfo
	{
		// Token: 0x04000104 RID: 260
		[ProtoMember(1)]
		public int ActID = 0;

		// Token: 0x04000105 RID: 261
		[ProtoMember(2)]
		public int LeftPurNum = 0;

		// Token: 0x04000106 RID: 262
		[ProtoMember(3)]
		public int State = 0;

		// Token: 0x04000107 RID: 263
		[ProtoMember(4)]
		public int ShowNum = 0;

		// Token: 0x04000108 RID: 264
		[ProtoMember(5)]
		public int ShowNum2 = 0;
	}
}
