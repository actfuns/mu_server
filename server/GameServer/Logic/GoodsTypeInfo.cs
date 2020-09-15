using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020002DA RID: 730
	public class GoodsTypeInfo
	{
		// Token: 0x040012C9 RID: 4809
		public static GoodsTypeInfo Empty = new GoodsTypeInfo();

		// Token: 0x040012CA RID: 4810
		public int Categriory;

		// Token: 0x040012CB RID: 4811
		public int GoodsType;

		// Token: 0x040012CC RID: 4812
		public bool IsEquip;

		// Token: 0x040012CD RID: 4813
		public bool[] Operations = new bool[255];

		// Token: 0x040012CE RID: 4814
		public List<int>[] OperationsTypeList = new List<int>[255];

		// Token: 0x040012CF RID: 4815
		public bool FashionGoods;

		// Token: 0x040012D0 RID: 4816
		public int UsingSite;
	}
}
