using System;
using Server.Data;

namespace GameServer.Logic
{
	
	public class KingOfBattleStoreConfig
	{
		
		public int ID;

		
		public GoodsData SaleData = null;

		
		public int JiFen;

		
		public int SinglePurchase;

		
		public int BeginNum;

		
		public int EndNum;

		
		public int RandNumMinus = 0;

		
		public bool RandSkip = false;
	}
}
