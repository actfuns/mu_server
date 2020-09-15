using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200008D RID: 141
	[ProtoContract]
	public class JingMaiData
	{
		// Token: 0x04000327 RID: 807
		[ProtoMember(1)]
		public int DbID = 0;

		// Token: 0x04000328 RID: 808
		[ProtoMember(2)]
		public int JingMaiID = 0;

		// Token: 0x04000329 RID: 809
		[ProtoMember(3)]
		public int JingMaiLevel = 0;

		// Token: 0x0400032A RID: 810
		[ProtoMember(4)]
		public int JingMaiBodyLevel = 0;
	}
}
