using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	internal class HolyPartInfo
	{
		
		public static int GetBujianID(sbyte nType, sbyte nSlot, sbyte nSuit)
		{
			return (int)nType * 1000 + (int)((nSlot - 1) * 100) + (int)nSuit;
		}

		
		public int m_nCostBandJinBi = 0;

		
		public List<List<int>> NeedGoods;

		
		public List<List<int>> FaildNeedGoods;

		
		public int m_nNeedGoodsID = 0;

		
		public int m_nNeedGoodsCount = 0;

		
		public int m_nFailCostGoodsID = 0;

		
		public int m_nFailCostGoodsCount = 0;

		
		public sbyte m_sSuccessProbability = 0;

		
		public List<MagicActionItem> m_PropertyList = new List<MagicActionItem>();

		
		public int m_nMaxFailCount = 0;
	}
}
