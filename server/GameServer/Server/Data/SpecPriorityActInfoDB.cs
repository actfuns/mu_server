using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000020 RID: 32
	[ProtoContract]
	public class SpecPriorityActInfoDB
	{
		// Token: 0x040000C2 RID: 194
		[ProtoMember(1)]
		public int TeQuanID = 0;

		// Token: 0x040000C3 RID: 195
		[ProtoMember(2)]
		public int ActID = 0;

		// Token: 0x040000C4 RID: 196
		[ProtoMember(3)]
		public int PurNum = 0;

		// Token: 0x040000C5 RID: 197
		[ProtoMember(4)]
		public int CountNum = 0;
	}
}
