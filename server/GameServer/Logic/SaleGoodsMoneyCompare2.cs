using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200078C RID: 1932
	public class SaleGoodsMoneyCompare2 : IComparer<SaleGoodsData>
	{
		// Token: 0x0600324A RID: 12874 RVA: 0x002CBA18 File Offset: 0x002C9C18
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			int ret = y.SalingGoodsData.SaleYuanBao - x.SalingGoodsData.SaleYuanBao;
			int result;
			if (ret != 0)
			{
				result = ret;
			}
			else
			{
				result = y.SalingGoodsData.SaleMoney1 - x.SalingGoodsData.SaleMoney1;
			}
			return result;
		}

		// Token: 0x04003E77 RID: 15991
		public static readonly SaleGoodsMoneyCompare2 Instance = new SaleGoodsMoneyCompare2();
	}
}
