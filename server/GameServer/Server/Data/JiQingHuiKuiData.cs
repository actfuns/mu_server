using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000565 RID: 1381
	[ProtoContract]
	public class JiQingHuiKuiData
	{
		// Token: 0x04002534 RID: 9524
		[ProtoMember(1)]
		public List<int> ChongJiQingQuShenZhuangQuota;

		// Token: 0x04002535 RID: 9525
		[ProtoMember(2)]
		public int ShenZhuangHuiZengQuoto;

		// Token: 0x04002536 RID: 9526
		[ProtoMember(3)]
		public int XingYunChouJiangCount;

		// Token: 0x04002537 RID: 9527
		[ProtoMember(4)]
		public int TodayYuanBao;

		// Token: 0x04002538 RID: 9528
		[ProtoMember(5)]
		public int TodayYuanBaoState;

		// Token: 0x04002539 RID: 9529
		[ProtoMember(6)]
		public int ChongJiLingQuShenZhuangState;

		// Token: 0x0400253A RID: 9530
		[ProtoMember(7)]
		public int XingYunChouJiangYuanBao;

		// Token: 0x0400253B RID: 9531
		[ProtoMember(8)]
		public int ShenZhuangHuiZengState;
	}
}
