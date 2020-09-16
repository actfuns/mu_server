using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class RoleManagerData
	{
		
		public object Mutex = new object();

		
		public int PurchaseOccupationNeedZuanShi = 1500;

		
		public HashSet<int> CanChangeOccupationMapCodes = new HashSet<int>();

		
		public HashSet<int> CanChangeOccupationGoodsTypes = new HashSet<int>();
	}
}
