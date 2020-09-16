using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class YangGongBKItem
	{
		
		public List<FallGoodsItem> FallGoodsItemList = null;

		
		public List<GoodsData> GoodsDataList = null;

		
		public List<GoodsData> TempGoodsDataList = null;

		
		public int FreeRefreshNum = 0;

		
		public int ClickBKNum = 0;

		
		public Dictionary<int, bool> PickUpDict = new Dictionary<int, bool>();

		
		public bool IsBaoWuBinding = false;
	}
}
