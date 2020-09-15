using System;

namespace GameServer.Logic
{
	// Token: 0x02000715 RID: 1813
	public class HeFuPKKingActivity : Activity
	{
		// Token: 0x06002B61 RID: 11105 RVA: 0x002680B4 File Offset: 0x002662B4
		public override bool GiveAward(GameClient client)
		{
			return base.GiveAward(client, this.MyAwardItem);
		}

		// Token: 0x06002B62 RID: 11106 RVA: 0x002680D4 File Offset: 0x002662D4
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return this.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList);
		}

		// Token: 0x04003A4F RID: 14927
		public AwardItem MyAwardItem = new AwardItem();

		// Token: 0x04003A50 RID: 14928
		public int winerCount = 3;
	}
}
