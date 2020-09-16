using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class SaleGoodsSuitCompare : IComparer<SaleGoodsData>
	{
		
		public SaleGoodsSuitCompare(int desc)
		{
			this.Desc = (desc != 0);
		}

		
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

		
		public static readonly SaleGoodsSuitCompare DescInstance = new SaleGoodsSuitCompare(1);

		
		public static readonly SaleGoodsSuitCompare AscInstance = new SaleGoodsSuitCompare(0);

		
		private bool Desc = true;
	}
}
