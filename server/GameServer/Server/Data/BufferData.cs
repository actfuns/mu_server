using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000120 RID: 288
	[ProtoContract]
	public class BufferData
	{
		// Token: 0x0400063F RID: 1599
		[ProtoMember(1)]
		public int BufferID = 0;

		// Token: 0x04000640 RID: 1600
		[ProtoMember(2)]
		public long StartTime = 0L;

		// Token: 0x04000641 RID: 1601
		[ProtoMember(3)]
		public int BufferSecs = 0;

		// Token: 0x04000642 RID: 1602
		[ProtoMember(4)]
		public long BufferVal = 0L;

		// Token: 0x04000643 RID: 1603
		[ProtoMember(5)]
		public int BufferType = 0;
	}
}
