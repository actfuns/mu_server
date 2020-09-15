using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200014D RID: 333
	[ProtoContract]
	internal class JieRiFanBeiItemData
	{
		// Token: 0x0400076C RID: 1900
		[ProtoMember(1)]
		public int Type;

		// Token: 0x0400076D RID: 1901
		[ProtoMember(2)]
		public int IsOpen;

		// Token: 0x0400076E RID: 1902
		[ProtoMember(3)]
		public string ExtArg1;

		// Token: 0x0400076F RID: 1903
		[ProtoMember(4)]
		public string ExtArg2;
	}
}
