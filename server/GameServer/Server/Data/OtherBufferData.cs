using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000121 RID: 289
	[ProtoContract]
	public class OtherBufferData
	{
		// Token: 0x04000644 RID: 1604
		[ProtoMember(1)]
		public int BufferID = 0;

		// Token: 0x04000645 RID: 1605
		[ProtoMember(2)]
		public long StartTime = 0L;

		// Token: 0x04000646 RID: 1606
		[ProtoMember(3)]
		public int BufferSecs = 0;

		// Token: 0x04000647 RID: 1607
		[ProtoMember(4)]
		public long BufferVal = 0L;

		// Token: 0x04000648 RID: 1608
		[ProtoMember(5)]
		public int BufferType = 0;

		// Token: 0x04000649 RID: 1609
		[ProtoMember(6)]
		public int RoleID = 0;
	}
}
