using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class UpLevelItem
	{
		
		public int ID = 0;

		
		public int ToLevel = 0;

		
		public List<GoodsData> GoodsDataList = null;

		
		public int BindYuanBao = 0;

		
		public int BindMoney = 0;

		
		public int MoJing = 0;

		
		public int Occupation = -1;
	}
}
