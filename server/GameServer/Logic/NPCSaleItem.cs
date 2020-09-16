using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class NPCSaleItem
	{
		
		public Dictionary<int, bool> SaleTypesDict = new Dictionary<int, bool>();

		
		public int Money1Price = 0;

		
		public int YinLiangPrice = 0;

		
		public int TianDiJingYuanPrice = 0;

		
		public int LieShaZhiPrice = 0;

		
		public int JiFenPrice = 0;

		
		public int JunGongPrice = 0;

		
		public int ZhanHunPrice = 0;

		
		public int Forge_level;

		
		public int Lucky;

		
		public int ExcellenceInfo;

		
		public int AppendPropLev;
	}
}
