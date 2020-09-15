using System;

namespace GameServer.Logic
{
	// Token: 0x0200071C RID: 1820
	public class ShenZhuangHuiKuiHaoLiActivity : Activity
	{
		// Token: 0x06002B7B RID: 11131 RVA: 0x00268970 File Offset: 0x00266B70
		public override AwardItem GetAward(GameClient client)
		{
			return this.MyAwardItem;
		}

		// Token: 0x06002B7C RID: 11132 RVA: 0x00268988 File Offset: 0x00266B88
		public override bool GiveAward(GameClient client, int _params)
		{
			base.GiveAward(client, this.MyAwardItem);
			return true;
		}

		// Token: 0x06002B7D RID: 11133 RVA: 0x002689AC File Offset: 0x00266BAC
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return this.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList);
		}

		// Token: 0x04003A5A RID: 14938
		public AwardItem MyAwardItem = new AwardItem();
	}
}
