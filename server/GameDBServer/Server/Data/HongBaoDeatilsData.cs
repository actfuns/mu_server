using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000F9 RID: 249
	[ProtoContract]
	public class HongBaoDeatilsData
	{
		// Token: 0x04000701 RID: 1793
		[ProtoMember(1)]
		public int hongBaoStatus;

		// Token: 0x04000702 RID: 1794
		[ProtoMember(2)]
		public int type;

		// Token: 0x04000703 RID: 1795
		[ProtoMember(3)]
		public string sender;

		// Token: 0x04000704 RID: 1796
		[ProtoMember(4)]
		public DateTime sendTime;

		// Token: 0x04000705 RID: 1797
		[ProtoMember(5)]
		public string message;

		// Token: 0x04000706 RID: 1798
		[ProtoMember(6)]
		public int diamondNum;

		// Token: 0x04000707 RID: 1799
		[ProtoMember(7)]
		public int sumDiamondNum;

		// Token: 0x04000708 RID: 1800
		[ProtoMember(8)]
		public int leftCount;

		// Token: 0x04000709 RID: 1801
		[ProtoMember(9)]
		public int sumCount;

		// Token: 0x0400070A RID: 1802
		[ProtoMember(10)]
		public List<SingleHongBaoRankInfo> infos;

		// Token: 0x0400070B RID: 1803
		[ProtoMember(11)]
		public int hongBaoID;
	}
}
