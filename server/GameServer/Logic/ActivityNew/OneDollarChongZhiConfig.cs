using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.ActivityNew
{
	
	public class OneDollarChongZhiConfig
	{
		
		public int ID = 0;

		
		public DateTime FromDate;

		
		public DateTime ToDate;

		
		public string UserListFile = "";

		
		public HashSet<string> UserListSet = new HashSet<string>();

		
		public int MinZuanShi = 0;

		
		public List<GoodsData> GoodsDataListOne = new List<GoodsData>();

		
		public List<GoodsData> GoodsDataListTwo = new List<GoodsData>();
	}
}
