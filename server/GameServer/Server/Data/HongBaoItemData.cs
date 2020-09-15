using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020002E8 RID: 744
	[ProtoContract]
	public class HongBaoItemData
	{
		// Token: 0x0400131B RID: 4891
		[ProtoMember(1)]
		public int hongBaoStatus;

		// Token: 0x0400131C RID: 4892
		[ProtoMember(2)]
		public int hongBaoID;

		// Token: 0x0400131D RID: 4893
		[ProtoMember(3)]
		public string sender;

		// Token: 0x0400131E RID: 4894
		[ProtoMember(4)]
		public DateTime beginTime;

		// Token: 0x0400131F RID: 4895
		[ProtoMember(5)]
		public DateTime endTime;

		// Token: 0x04001320 RID: 4896
		[ProtoMember(6)]
		public int diamondCount;

		// Token: 0x04001321 RID: 4897
		[ProtoMember(7)]
		public int diamondSumCount;

		// Token: 0x04001322 RID: 4898
		[ProtoMember(8)]
		public int type;
	}
}
