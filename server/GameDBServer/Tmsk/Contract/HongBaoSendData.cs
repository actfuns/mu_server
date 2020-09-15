using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Tmsk.Contract
{
	// Token: 0x02000100 RID: 256
	[ProtoContract]
	public class HongBaoSendData
	{
		// Token: 0x04000725 RID: 1829
		[ProtoMember(1)]
		public int hongBaoStatus;

		// Token: 0x04000726 RID: 1830
		[ProtoMember(2)]
		public int type;

		// Token: 0x04000727 RID: 1831
		[ProtoMember(3)]
		public string sender;

		// Token: 0x04000728 RID: 1832
		[ProtoMember(4)]
		public DateTime sendTime;

		// Token: 0x04000729 RID: 1833
		[ProtoMember(5)]
		public string message;

		// Token: 0x0400072A RID: 1834
		[ProtoMember(6)]
		public int leftZuanShi;

		// Token: 0x0400072B RID: 1835
		[ProtoMember(7)]
		public int sumDiamondNum;

		// Token: 0x0400072C RID: 1836
		[ProtoMember(8)]
		public int leftCount;

		// Token: 0x0400072D RID: 1837
		[ProtoMember(9)]
		public int sumCount;

		// Token: 0x0400072E RID: 1838
		[ProtoMember(10)]
		public DateTime endTime;

		// Token: 0x0400072F RID: 1839
		[ProtoMember(11)]
		public int hongBaoID;

		// Token: 0x04000730 RID: 1840
		[ProtoMember(12)]
		public List<HongBaoRecvData> RecvList;

		// Token: 0x04000731 RID: 1841
		[ProtoMember(13)]
		public int senderID;

		// Token: 0x04000732 RID: 1842
		[ProtoMember(14)]
		public int bhid;
	}
}
