using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200078F RID: 1935
	public class SaleGoodsSuitCompare : IComparer<SaleGoodsData>
	{
		// Token: 0x06003253 RID: 12883 RVA: 0x002CBC34 File Offset: 0x002C9E34
		public SaleGoodsSuitCompare(int desc)
		{
			this.Desc = (desc != 0);
		}

		// Token: 0x06003254 RID: 12884 RVA: 0x002CBC54 File Offset: 0x002C9E54
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			int result;
			if (x.SalingGoodsData.GoodsID == y.SalingGoodsData.GoodsID)
			{
				result = 0;
			}
			else
			{
				int ret = Global.GetEquipGoodsSuitID(x.SalingGoodsData.GoodsID) - Global.GetEquipGoodsSuitID(y.SalingGoodsData.GoodsID);
				if (this.Desc)
				{
					ret = -ret;
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x04003E7E RID: 15998
		public static readonly SaleGoodsSuitCompare DescInstance = new SaleGoodsSuitCompare(1);

		// Token: 0x04003E7F RID: 15999
		public static readonly SaleGoodsSuitCompare AscInstance = new SaleGoodsSuitCompare(0);

		// Token: 0x04003E80 RID: 16000
		private bool Desc = true;
	}
}
