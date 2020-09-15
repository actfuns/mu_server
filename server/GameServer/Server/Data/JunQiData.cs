using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000566 RID: 1382
	[ProtoContract]
	public class JunQiData
	{
		// Token: 0x0400253C RID: 9532
		[ProtoMember(1)]
		public int JunQiID = 0;

		// Token: 0x0400253D RID: 9533
		[ProtoMember(2)]
		public string QiName = "";

		// Token: 0x0400253E RID: 9534
		[ProtoMember(3)]
		public int JunQiLevel = 0;

		// Token: 0x0400253F RID: 9535
		[ProtoMember(4)]
		public int ZoneID = 0;

		// Token: 0x04002540 RID: 9536
		[ProtoMember(5)]
		public int BHID = 0;

		// Token: 0x04002541 RID: 9537
		[ProtoMember(6)]
		public string BHName = "";

		// Token: 0x04002542 RID: 9538
		[ProtoMember(7)]
		public int QiZuoNPC = 0;

		// Token: 0x04002543 RID: 9539
		[ProtoMember(8)]
		public int MapCode = 0;

		// Token: 0x04002544 RID: 9540
		[ProtoMember(9)]
		public int PosX = 0;

		// Token: 0x04002545 RID: 9541
		[ProtoMember(10)]
		public int PosY = 0;

		// Token: 0x04002546 RID: 9542
		[ProtoMember(11)]
		public int Direction = 0;

		// Token: 0x04002547 RID: 9543
		[ProtoMember(12)]
		public int LifeV = 0;

		// Token: 0x04002548 RID: 9544
		[ProtoMember(13)]
		public int CutLifeV = 0;

		// Token: 0x04002549 RID: 9545
		[ProtoMember(14)]
		public long StartTime = 0L;

		// Token: 0x0400254A RID: 9546
		[ProtoMember(15)]
		public int BodyCode = 0;

		// Token: 0x0400254B RID: 9547
		[ProtoMember(16)]
		public int PicCode = 0;

		// Token: 0x0400254C RID: 9548
		[ProtoMember(17)]
		public int CurrentLifeV = 0;
	}
}
