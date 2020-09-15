using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200078D RID: 1933
	public class SaleGoodsMoneyPerItemCompare : IComparer<SaleGoodsData>
	{
		// Token: 0x0600324D RID: 12877 RVA: 0x002CBA7A File Offset: 0x002C9C7A
		public SaleGoodsMoneyPerItemCompare(int desc)
		{
			this.Desc = (desc != 0);
		}

		// Token: 0x0600324E RID: 12878 RVA: 0x002CBA9C File Offset: 0x002C9C9C
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			int ret = 0;
			if (x.SalingGoodsData.GCount <= 0)
			{
				if (y.SalingGoodsData.GCount > 0)
				{
					ret = -1;
				}
			}
			else
			{
				if (y.SalingGoodsData.GCount <= 0)
				{
					return 1;
				}
				ret = x.SalingGoodsData.SaleYuanBao / x.SalingGoodsData.GCount - y.SalingGoodsData.SaleYuanBao / y.SalingGoodsData.GCount;
				if (ret == 0)
				{
					ret = x.SalingGoodsData.SaleMoney1 / x.SalingGoodsData.GCount - y.SalingGoodsData.SaleMoney1 / y.SalingGoodsData.GCount;
				}
			}
			if (this.Desc)
			{
				ret = -ret;
			}
			return ret;
		}

		// Token: 0x04003E78 RID: 15992
		public static readonly SaleGoodsMoneyPerItemCompare DescInstance = new SaleGoodsMoneyPerItemCompare(1);

		// Token: 0x04003E79 RID: 15993
		public static readonly SaleGoodsMoneyPerItemCompare AscInstance = new SaleGoodsMoneyPerItemCompare(0);

		// Token: 0x04003E7A RID: 15994
		private bool Desc = true;
	}
}
