using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class RoleManagerData
	{
		
		public object Mutex = new object();

		
		public int[] PurchaseOccupationNeedZuanShi;

		
		public int PurchaseOccupationGoods = 2100;

		
		public HashSet<int> CanChangeOccupationMapCodes = new HashSet<int>();

		
		public HashSet<int> DisableChangeOccupationGoodsTypes = new HashSet<int>();
	}
}
