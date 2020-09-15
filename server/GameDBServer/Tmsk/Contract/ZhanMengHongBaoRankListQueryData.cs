using System;
using ProtoBuf;

namespace Tmsk.Contract
{
	// Token: 0x020000FE RID: 254
	[ProtoContract]
	public class ZhanMengHongBaoRankListQueryData
	{
		// Token: 0x0400071D RID: 1821
		[ProtoMember(1)]
		public int Bhid;

		// Token: 0x0400071E RID: 1822
		[ProtoMember(2)]
		public int Type;

		// Token: 0x0400071F RID: 1823
		[ProtoMember(3)]
		public DateTime StartTime;

		// Token: 0x04000720 RID: 1824
		[ProtoMember(4)]
		public int Count;
	}
}
