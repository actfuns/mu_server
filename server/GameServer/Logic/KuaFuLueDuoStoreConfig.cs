using System;
using Server.Data;

namespace GameServer.Logic
{
	
	public class KuaFuLueDuoStoreConfig
	{
		
		public int ID;

		
		public int Type;

		
		public GoodsData SaleData = null;

		
		public int ZuanShi;

		
		public int JueXingNum;

		
		public int SinglePurchase;

		
		public int BeginNum;

		
		public int EndNum;

		
		public int RandNumMinus = 0;

		
		public bool RandSkip = false;
	}
}
