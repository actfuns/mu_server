using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class GoodsTypeInfo
	{
		
		public static GoodsTypeInfo Empty = new GoodsTypeInfo();

		
		public int Categriory;

		
		public int GoodsType;

		
		public bool IsEquip;

		
		public bool[] Operations = new bool[255];

		
		public List<int>[] OperationsTypeList = new List<int>[255];

		
		public bool FashionGoods;

		
		public int UsingSite;
	}
}
