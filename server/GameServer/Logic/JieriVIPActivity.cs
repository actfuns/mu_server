using System;

namespace GameServer.Logic
{
	// Token: 0x02000704 RID: 1796
	public class JieriVIPActivity : Activity
	{
		// Token: 0x06002B2B RID: 11051 RVA: 0x002670DC File Offset: 0x002652DC
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, this.MyAwardItem);
		}

		// Token: 0x06002B2C RID: 11052 RVA: 0x002670FC File Offset: 0x002652FC
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return this.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList);
		}

		// Token: 0x04003A2C RID: 14892
		public AwardItem MyAwardItem = new AwardItem();
	}
}
