using System;
using Server.Data;

namespace GameServer.Logic
{
	
	public class CombatAwardItem
	{
		
		public int TotalAwardCnt(GameClient client)
		{
			int totalCnt = 0;
			if (this.GeneralAwardItem.GoodsDataList != null)
			{
				totalCnt += this.GeneralAwardItem.GoodsDataList.Count;
			}
			if (this.OccAwardItem.GoodsDataList != null)
			{
				foreach (GoodsData goodsData in this.OccAwardItem.GoodsDataList)
				{
					if (Global.IsCanGiveRewardByOccupation(client, goodsData.GoodsID))
					{
						totalCnt++;
					}
				}
			}
			if (this.EffectTimeAwardItem != null)
			{
				totalCnt += this.EffectTimeAwardItem.GoodsCnt();
			}
			return totalCnt;
		}

		
		public int ID = 0;

		
		public int ComBatValue = 0;

		
		public AwardItem GeneralAwardItem = new AwardItem();

		
		public AwardItem OccAwardItem = new AwardItem();

		
		public AwardEffectTimeItem EffectTimeAwardItem = new AwardEffectTimeItem();
	}
}
