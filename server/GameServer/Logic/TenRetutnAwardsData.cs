using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class TenRetutnAwardsData
	{
		
		public int ID;

		
		public int ChongZhiZhuanShi;

		
		public AwardsItemList GoodsID1 = new AwardsItemList();

		
		public AwardsItemList GoodsID2 = new AwardsItemList();

		
		public string MailUser = "";

		
		public string MailTitle = "";

		
		public string MailContent = "";

		
		public string BeginTimeStr;

		
		public string FinishTimeStr;

		
		public DateTime BeginTime = DateTime.MaxValue;

		
		public DateTime FinishTime = DateTime.MinValue;

		
		public string UserList;

		
		public Dictionary<string, bool> _tenUserIdAwardsDict = new Dictionary<string, bool>();

		
		public bool SystemOpen;
	}
}
