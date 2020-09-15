using System;

namespace GameServer.Logic
{
	// Token: 0x02000719 RID: 1817
	public class HeFuWCKingActivity : Activity
	{
		// Token: 0x06002B6C RID: 11116 RVA: 0x00268274 File Offset: 0x00266474
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, this.MyAwardItem);
		}

		// Token: 0x06002B6D RID: 11117 RVA: 0x00268294 File Offset: 0x00266494
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return this.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList);
		}

		// Token: 0x04003A58 RID: 14936
		public AwardItem MyAwardItem = new AwardItem();
	}
}
