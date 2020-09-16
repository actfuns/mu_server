using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class FuBenMapItem
	{
		
		public int FuBenID = 0;

		
		public int MapCode = 0;

		
		public int MaxTime = -1;

		
		public int MinSaoDangTimer = -1;

		
		public int Money1 = 0;

		
		public int Experience = 0;

		
		public int nFirstGold = 0;

		
		public int nFirstExp = 0;

		
		public List<GoodsData> GoodsDataList = null;

		
		public List<GoodsData> FirstGoodsDataList = null;

		
		public int nXingHunAward = 0;

		
		public int nFirstXingHunAward = 0;

		
		public int nZhanGongaward = 0;

		
		public int YuanSuFenMoaward = 0;

		
		public int LightAward = 0;

		
		public int WolfMoney = 0;
	}
}
