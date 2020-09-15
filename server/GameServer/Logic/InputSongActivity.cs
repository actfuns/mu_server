using System;

namespace GameServer.Logic
{
	// Token: 0x020006FF RID: 1791
	public class InputSongActivity : Activity
	{
		// Token: 0x06002B13 RID: 11027 RVA: 0x00266A18 File Offset: 0x00264C18
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, this.MyAwardItem);
		}

		// Token: 0x06002B14 RID: 11028 RVA: 0x00266A38 File Offset: 0x00264C38
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return this.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList);
		}

		// Token: 0x04003A1E RID: 14878
		public AwardItem MyAwardItem = new AwardItem();
	}
}
