using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000038 RID: 56
	[ProtoContract]
	public class BufferData
	{
		// Token: 0x04000122 RID: 290
		[ProtoMember(1)]
		public int BufferID = 0;

		// Token: 0x04000123 RID: 291
		[ProtoMember(2)]
		public long StartTime = 0L;

		// Token: 0x04000124 RID: 292
		[ProtoMember(3)]
		public int BufferSecs = 0;

		// Token: 0x04000125 RID: 293
		[ProtoMember(4)]
		public long BufferVal = 0L;

		// Token: 0x04000126 RID: 294
		[ProtoMember(5)]
		public int BufferType = 0;
	}
}
