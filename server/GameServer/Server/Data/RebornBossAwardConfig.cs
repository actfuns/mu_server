using System;
using GameServer.Logic;

namespace Server.Data
{
	
	public class RebornBossAwardConfig
	{
		
		public int ID;

		
		public int MonstersID;

		
		public int BeginNum;

		
		public int EndNum;

		
		public AwardsItemList AwardsItemListOne = new AwardsItemList();

		
		public AwardsItemList AwardsItemListTwo = new AwardsItemList();

		
		public int AwardType;
	}
}
