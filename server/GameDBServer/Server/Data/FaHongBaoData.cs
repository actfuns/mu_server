using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000F3 RID: 243
	[ProtoContract]
	public class FaHongBaoData
	{
		// Token: 0x040006E3 RID: 1763
		[ProtoMember(1)]
		public int roleID;

		// Token: 0x040006E4 RID: 1764
		[ProtoMember(2)]
		public int type;

		// Token: 0x040006E5 RID: 1765
		[ProtoMember(3)]
		public int count;

		// Token: 0x040006E6 RID: 1766
		[ProtoMember(4)]
		public int diamondNum;

		// Token: 0x040006E7 RID: 1767
		[ProtoMember(5)]
		public string message;
	}
}
