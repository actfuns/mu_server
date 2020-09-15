using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200078B RID: 1931
	public class SaleGoodsMoneyCompare : IComparer<SaleGoodsData>
	{
		// Token: 0x06003247 RID: 12871 RVA: 0x002CB9B4 File Offset: 0x002C9BB4
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			int ret = x.SalingGoodsData.SaleYuanBao - y.SalingGoodsData.SaleYuanBao;
			int result;
			if (ret != 0)
			{
				result = ret;
			}
			else
			{
				result = x.SalingGoodsData.SaleMoney1 - y.SalingGoodsData.SaleMoney1;
			}
			return result;
		}

		// Token: 0x04003E76 RID: 15990
		public static readonly SaleGoodsMoneyCompare Instance = new SaleGoodsMoneyCompare();
	}
}
