using System;
using System.Collections.Generic;

namespace GameServer.Logic.TuJian
{
	
	internal class TuJianType
	{
		
		public int TypeID;

		
		public string Name;

		
		public int OpenChangeLife;

		
		public int OpenLevel;

		
		public int ItemCnt;

		
		public _AttrValue AttrValue = null;

		
		public List<int> ItemList = new List<int>();
	}
}
