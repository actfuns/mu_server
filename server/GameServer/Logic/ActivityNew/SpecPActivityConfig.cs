using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000054 RID: 84
	public class SpecPActivityConfig
	{
		// Token: 0x040001BF RID: 447
		public int TeQuanID;

		// Token: 0x040001C0 RID: 448
		public int ActID;

		// Token: 0x040001C1 RID: 449
		public int Param1;

		// Token: 0x040001C2 RID: 450
		public int Param2;

		// Token: 0x040001C3 RID: 451
		public int ZhiGouID;

		// Token: 0x040001C4 RID: 452
		public int PurchaseNum = -1;

		// Token: 0x040001C5 RID: 453
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		// Token: 0x040001C6 RID: 454
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();

		// Token: 0x040001C7 RID: 455
		public AwardEffectTimeItem GoodsDataListThr = new AwardEffectTimeItem();

		// Token: 0x040001C8 RID: 456
		public SpecPActivityType ActType;

		// Token: 0x040001C9 RID: 457
		public double MultiNum;

		// Token: 0x040001CA RID: 458
		public int[] HongBaoRange;

		// Token: 0x040001CB RID: 459
		public HashSet<int> ChouJiangTypeSet = new HashSet<int>();
	}
}
