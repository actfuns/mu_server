using System;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006F2 RID: 1778
	public class CombatAwardItem
	{
		// Token: 0x06002AE8 RID: 10984 RVA: 0x0026420C File Offset: 0x0026240C
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

		// Token: 0x040039F3 RID: 14835
		public int ID = 0;

		// Token: 0x040039F4 RID: 14836
		public int ComBatValue = 0;

		// Token: 0x040039F5 RID: 14837
		public AwardItem GeneralAwardItem = new AwardItem();

		// Token: 0x040039F6 RID: 14838
		public AwardItem OccAwardItem = new AwardItem();

		// Token: 0x040039F7 RID: 14839
		public AwardEffectTimeItem EffectTimeAwardItem = new AwardEffectTimeItem();
	}
}
