using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000B1 RID: 177
	[ProtoContract]
	public class SpecPriorityActInfoDB
	{
		// Token: 0x0400049D RID: 1181
		[ProtoMember(1)]
		public int TeQuanID = 0;

		// Token: 0x0400049E RID: 1182
		[ProtoMember(2)]
		public int ActID = 0;

		// Token: 0x0400049F RID: 1183
		[ProtoMember(3)]
		public int PurNum = 0;

		// Token: 0x040004A0 RID: 1184
		[ProtoMember(4)]
		public int CountNum = 0;
	}
}
