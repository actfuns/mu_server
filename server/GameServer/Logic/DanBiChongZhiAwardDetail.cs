using System;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006FA RID: 1786
	public class DanBiChongZhiAwardDetail
	{
		// Token: 0x06002AFA RID: 11002 RVA: 0x00264D44 File Offset: 0x00262F44
		public int TotalAwardCntWithOcc(GameClient client)
		{
			int totalCnt = 0;
			if (this.AwardDict.GoodsDataList != null)
			{
				totalCnt += this.AwardDict.GoodsDataList.Count;
			}
			if (this.AwardDict2.GoodsDataList != null)
			{
				foreach (GoodsData goodsData in this.AwardDict2.GoodsDataList)
				{
					if (Global.IsCanGiveRewardByOccupation(client, goodsData.GoodsID))
					{
						totalCnt++;
					}
				}
			}
			if (this.EffectTimeAwardDict != null)
			{
				totalCnt += this.EffectTimeAwardDict.GoodsCnt();
			}
			return totalCnt;
		}

		// Token: 0x04003A10 RID: 14864
		public int ID = 0;

		// Token: 0x04003A11 RID: 14865
		public AwardItem AwardDict = new AwardItem();

		// Token: 0x04003A12 RID: 14866
		public AwardItem AwardDict2 = new AwardItem();

		// Token: 0x04003A13 RID: 14867
		public AwardEffectTimeItem EffectTimeAwardDict = new AwardEffectTimeItem();

		// Token: 0x04003A14 RID: 14868
		public int MinYuanBao = 0;

		// Token: 0x04003A15 RID: 14869
		public int MaxYuanBao = 0;

		// Token: 0x04003A16 RID: 14870
		public int SinglePurchase = 0;
	}
}
