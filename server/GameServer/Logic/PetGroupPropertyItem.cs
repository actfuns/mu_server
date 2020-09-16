using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class PetGroupPropertyItem
	{
		
		public int Id;

		
		public string Name;

		
		public List<List<int>> PetGoodsList = new List<List<int>>();

		
		public EquipPropItem PropItem;
	}
}
