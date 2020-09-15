using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000122 RID: 290
	[ProtoContract]
	public class BufferDataMini
	{
		// Token: 0x0400064A RID: 1610
		[ProtoMember(1)]
		public int BufferID = 0;

		// Token: 0x0400064B RID: 1611
		[ProtoMember(2)]
		public long StartTime = 0L;

		// Token: 0x0400064C RID: 1612
		[ProtoMember(3)]
		public int BufferSecs = 0;

		// Token: 0x0400064D RID: 1613
		[ProtoMember(4)]
		public long BufferVal = 0L;

		// Token: 0x0400064E RID: 1614
		[ProtoMember(5)]
		public int BufferType = 0;
	}
}
