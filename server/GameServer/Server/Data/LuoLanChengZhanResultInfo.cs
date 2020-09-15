using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200015D RID: 349
	[ProtoContract]
	public class LuoLanChengZhanResultInfo
	{
		// Token: 0x040007B0 RID: 1968
		[ProtoMember(1)]
		public int BHID = 0;

		// Token: 0x040007B1 RID: 1969
		[ProtoMember(2)]
		public string BHName = "";

		// Token: 0x040007B2 RID: 1970
		[ProtoMember(3)]
		public long ExpAward;

		// Token: 0x040007B3 RID: 1971
		[ProtoMember(4)]
		public int ZhanGongAward;

		// Token: 0x040007B4 RID: 1972
		[ProtoMember(5)]
		public int ZhanMengZiJin;
	}
}
