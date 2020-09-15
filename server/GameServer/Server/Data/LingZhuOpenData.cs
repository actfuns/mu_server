using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200030E RID: 782
	[ProtoContract]
	public class LingZhuOpenData
	{
		// Token: 0x04001413 RID: 5139
		[ProtoMember(1)]
		public TimeSpan BeginTime;

		// Token: 0x04001414 RID: 5140
		[ProtoMember(2)]
		public TimeSpan EndTime;

		// Token: 0x04001415 RID: 5141
		[ProtoMember(3)]
		public int OpenType;

		// Token: 0x04001416 RID: 5142
		[ProtoMember(4)]
		public DateTime DoubleOpenEndTime;

		// Token: 0x04001417 RID: 5143
		[ProtoMember(5)]
		public int LeftCount;
	}
}
