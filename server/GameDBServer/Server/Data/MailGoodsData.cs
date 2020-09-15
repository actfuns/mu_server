using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000091 RID: 145
	[ProtoContract]
	public class MailGoodsData
	{
		// Token: 0x04000344 RID: 836
		[ProtoMember(1)]
		public int Id;

		// Token: 0x04000345 RID: 837
		[ProtoMember(2)]
		public int MailID = 0;

		// Token: 0x04000346 RID: 838
		[ProtoMember(3)]
		public int GoodsID = 0;

		// Token: 0x04000347 RID: 839
		[ProtoMember(4)]
		public int Forge_level = 0;

		// Token: 0x04000348 RID: 840
		[ProtoMember(5)]
		public int Quality = 0;

		// Token: 0x04000349 RID: 841
		[ProtoMember(6)]
		public string Props = "";

		// Token: 0x0400034A RID: 842
		[ProtoMember(7)]
		public int GCount = 0;

		// Token: 0x0400034B RID: 843
		[ProtoMember(8)]
		public int Binding = 0;

		// Token: 0x0400034C RID: 844
		[ProtoMember(9)]
		public int OrigHoleNum = 0;

		// Token: 0x0400034D RID: 845
		[ProtoMember(10)]
		public int RMBHoleNum = 0;

		// Token: 0x0400034E RID: 846
		[ProtoMember(11)]
		public string Jewellist = "";

		// Token: 0x0400034F RID: 847
		[ProtoMember(12)]
		public int AddPropIndex = 0;

		// Token: 0x04000350 RID: 848
		[ProtoMember(13)]
		public int BornIndex = 0;

		// Token: 0x04000351 RID: 849
		[ProtoMember(14)]
		public int Lucky = 0;

		// Token: 0x04000352 RID: 850
		[ProtoMember(15)]
		public int Strong = 0;

		// Token: 0x04000353 RID: 851
		[ProtoMember(16)]
		public int ExcellenceInfo = 0;

		// Token: 0x04000354 RID: 852
		[ProtoMember(17)]
		public int AppendPropLev = 0;

		// Token: 0x04000355 RID: 853
		[ProtoMember(18)]
		public int EquipChangeLifeLev = 0;
	}
}
