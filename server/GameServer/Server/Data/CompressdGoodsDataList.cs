using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000558 RID: 1368
	public class CompressdGoodsDataList : List<GoodsData>, ICompressed
	{
		// Token: 0x060019D5 RID: 6613 RVA: 0x001913D4 File Offset: 0x0018F5D4
		public CompressdGoodsDataList(List<GoodsData> list) : base(list)
		{
		}
	}
}
