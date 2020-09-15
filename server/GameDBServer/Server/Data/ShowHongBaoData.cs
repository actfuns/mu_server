using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000F8 RID: 248
	[ProtoContract]
	public class ShowHongBaoData
	{
		// Token: 0x040006FA RID: 1786
		[ProtoMember(1)]
		public int type;

		// Token: 0x040006FB RID: 1787
		[ProtoMember(2)]
		public int hongBaoID;

		// Token: 0x040006FC RID: 1788
		[ProtoMember(3)]
		public string sender;

		// Token: 0x040006FD RID: 1789
		[ProtoMember(4)]
		public string message;

		// Token: 0x040006FE RID: 1790
		[ProtoMember(5)]
		public int yiLingNum;

		// Token: 0x040006FF RID: 1791
		[ProtoMember(6)]
		public int SumHongBaoNum;

		// Token: 0x04000700 RID: 1792
		[ProtoMember(7)]
		public int result;
	}
}
