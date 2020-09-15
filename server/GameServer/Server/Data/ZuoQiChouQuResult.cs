using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200085B RID: 2139
	[ProtoContract]
	public class ZuoQiChouQuResult
	{
		// Token: 0x040046BC RID: 18108
		[ProtoMember(1)]
		public int Result;

		// Token: 0x040046BD RID: 18109
		[ProtoMember(2)]
		public string GoodsList;

		// Token: 0x040046BE RID: 18110
		[ProtoMember(3)]
		public DateTime FreeTime;
	}
}
