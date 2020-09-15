using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000F7 RID: 247
	[ProtoContract]
	public class HongBaoItemData
	{
		// Token: 0x040006F2 RID: 1778
		[ProtoMember(1)]
		public int hongBaoStatus;

		// Token: 0x040006F3 RID: 1779
		[ProtoMember(2)]
		public int hongBaoID;

		// Token: 0x040006F4 RID: 1780
		[ProtoMember(3)]
		public string sender;

		// Token: 0x040006F5 RID: 1781
		[ProtoMember(4)]
		public DateTime beginTime;

		// Token: 0x040006F6 RID: 1782
		[ProtoMember(5)]
		public DateTime endTime;

		// Token: 0x040006F7 RID: 1783
		[ProtoMember(6)]
		public int diamondCount;

		// Token: 0x040006F8 RID: 1784
		[ProtoMember(7)]
		public int diamondSumCount;

		// Token: 0x040006F9 RID: 1785
		[ProtoMember(8)]
		public int type;
	}
}
