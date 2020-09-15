using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200078E RID: 1934
	public class SaleGoodsNameAndColorCompare : IComparer<SaleGoodsData>
	{
		// Token: 0x06003250 RID: 12880 RVA: 0x002CBB96 File Offset: 0x002C9D96
		public SaleGoodsNameAndColorCompare(int desc)
		{
			this.Desc = (desc != 0);
		}

		// Token: 0x06003251 RID: 12881 RVA: 0x002CBBB8 File Offset: 0x002C9DB8
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			int ret = x.SalingGoodsData.GoodsID - y.SalingGoodsData.GoodsID;
			if (ret == 0)
			{
				ret = Global.GetEquipColor(x.SalingGoodsData) - Global.GetEquipColor(y.SalingGoodsData);
			}
			if (this.Desc)
			{
				ret = -ret;
			}
			return ret;
		}

		// Token: 0x04003E7B RID: 15995
		public static readonly SaleGoodsNameAndColorCompare DescInstance = new SaleGoodsNameAndColorCompare(1);

		// Token: 0x04003E7C RID: 15996
		public static readonly SaleGoodsNameAndColorCompare AscInstance = new SaleGoodsNameAndColorCompare(0);

		// Token: 0x04003E7D RID: 15997
		private bool Desc = true;
	}
}
