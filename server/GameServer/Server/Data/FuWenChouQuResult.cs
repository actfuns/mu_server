using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000416 RID: 1046
	[ProtoContract]
	public class FuWenChouQuResult
	{
		// Token: 0x04001BE5 RID: 7141
		[ProtoMember(1)]
		public int Result;

		// Token: 0x04001BE6 RID: 7142
		[ProtoMember(2)]
		public string GoodsList;

		// Token: 0x04001BE7 RID: 7143
		[ProtoMember(3)]
		public DateTime FreeTime;

		// Token: 0x04001BE8 RID: 7144
		[ProtoMember(4)]
		public int ChouQuCount;
	}
}
