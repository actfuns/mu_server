using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002EA RID: 746
	[ProtoContract]
	public class HongBaoDeatilsData
	{
		// Token: 0x0400132B RID: 4907
		[ProtoMember(1)]
		public int hongBaoStatus;

		// Token: 0x0400132C RID: 4908
		[ProtoMember(2)]
		public int type;

		// Token: 0x0400132D RID: 4909
		[ProtoMember(3)]
		public string sender;

		// Token: 0x0400132E RID: 4910
		[ProtoMember(4)]
		public DateTime sendTime;

		// Token: 0x0400132F RID: 4911
		[ProtoMember(5)]
		public string message;

		// Token: 0x04001330 RID: 4912
		[ProtoMember(6)]
		public int diamondNum;

		// Token: 0x04001331 RID: 4913
		[ProtoMember(7)]
		public int sumDiamondNum;

		// Token: 0x04001332 RID: 4914
		[ProtoMember(8)]
		public int leftCount;

		// Token: 0x04001333 RID: 4915
		[ProtoMember(9)]
		public int sumCount;

		// Token: 0x04001334 RID: 4916
		[ProtoMember(10)]
		public List<SingleHongBaoRankInfo> infos;

		// Token: 0x04001335 RID: 4917
		[ProtoMember(11)]
		public int hongBaoID;
	}
}
