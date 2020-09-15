using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000188 RID: 392
	[ProtoContract]
	public class SpecActInfo
	{
		// Token: 0x040008B1 RID: 2225
		[ProtoMember(1)]
		public int ActID = 0;

		// Token: 0x040008B2 RID: 2226
		[ProtoMember(2)]
		public int LeftPurNum = 0;

		// Token: 0x040008B3 RID: 2227
		[ProtoMember(3)]
		public int State = 0;

		// Token: 0x040008B4 RID: 2228
		[ProtoMember(4)]
		public int ShowNum = 0;

		// Token: 0x040008B5 RID: 2229
		[ProtoMember(5)]
		public int ShowNum2 = 0;
	}
}
