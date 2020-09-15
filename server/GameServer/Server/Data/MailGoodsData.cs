using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000570 RID: 1392
	[ProtoContract]
	public class MailGoodsData
	{
		// Token: 0x0400257B RID: 9595
		[ProtoMember(1)]
		public int Id;

		// Token: 0x0400257C RID: 9596
		[ProtoMember(2)]
		public int MailID = 0;

		// Token: 0x0400257D RID: 9597
		[ProtoMember(3)]
		public int GoodsID = 0;

		// Token: 0x0400257E RID: 9598
		[ProtoMember(4)]
		public int Forge_level;

		// Token: 0x0400257F RID: 9599
		[ProtoMember(5)]
		public int Quality;

		// Token: 0x04002580 RID: 9600
		[ProtoMember(6)]
		public string Props;

		// Token: 0x04002581 RID: 9601
		[ProtoMember(7)]
		public int GCount;

		// Token: 0x04002582 RID: 9602
		[ProtoMember(8)]
		public int Binding;

		// Token: 0x04002583 RID: 9603
		[ProtoMember(9)]
		public int OrigHoleNum = 0;

		// Token: 0x04002584 RID: 9604
		[ProtoMember(10)]
		public int RMBHoleNum = 0;

		// Token: 0x04002585 RID: 9605
		[ProtoMember(11)]
		public string Jewellist;

		// Token: 0x04002586 RID: 9606
		[ProtoMember(12)]
		public int AddPropIndex;

		// Token: 0x04002587 RID: 9607
		[ProtoMember(13)]
		public int BornIndex;

		// Token: 0x04002588 RID: 9608
		[ProtoMember(14)]
		public int Lucky;

		// Token: 0x04002589 RID: 9609
		[ProtoMember(15)]
		public int Strong;

		// Token: 0x0400258A RID: 9610
		[ProtoMember(16)]
		public int ExcellenceInfo;

		// Token: 0x0400258B RID: 9611
		[ProtoMember(17)]
		public int AppendPropLev;

		// Token: 0x0400258C RID: 9612
		[ProtoMember(18)]
		public int EquipChangeLifeLev;

		// Token: 0x0400258D RID: 9613
		public int Site;
	}
}
