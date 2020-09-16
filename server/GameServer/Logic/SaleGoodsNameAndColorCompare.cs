using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class SaleGoodsNameAndColorCompare : IComparer<SaleGoodsData>
	{
		
		public SaleGoodsNameAndColorCompare(int desc)
		{
			this.Desc = (desc != 0);
		}

		
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

		
		public static readonly SaleGoodsNameAndColorCompare DescInstance = new SaleGoodsNameAndColorCompare(1);

		
		public static readonly SaleGoodsNameAndColorCompare AscInstance = new SaleGoodsNameAndColorCompare(0);

		
		private bool Desc = true;
	}
}
