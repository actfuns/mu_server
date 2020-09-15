using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200001E RID: 30
	[ProtoContract]
	public class SpecPriorityActInfo
	{
		// Token: 0x040000B8 RID: 184
		[ProtoMember(1)]
		public int TeQuanID = 0;

		// Token: 0x040000B9 RID: 185
		[ProtoMember(2)]
		public int ActID = 0;

		// Token: 0x040000BA RID: 186
		[ProtoMember(3)]
		public int LeftPurNum = 0;

		// Token: 0x040000BB RID: 187
		[ProtoMember(4)]
		public int State = 0;

		// Token: 0x040000BC RID: 188
		[ProtoMember(5)]
		public int ShowNum = 0;

		// Token: 0x040000BD RID: 189
		[ProtoMember(6)]
		public int ShowNum2 = 0;
	}
}
