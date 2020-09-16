using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class JieRiZiKa
	{
		
		public int type;

		
		public int id;

		
		public int DayMaxTimes = 0;

		
		public List<GoodsData> NeedGoodsList = null;

		
		public int NeedMoJing;

		
		public int NeedQiFuJiFen;

		
		public int NeedPetJiFen;

		
		public AwardItem MyAwardItem = new AwardItem();
	}
}
