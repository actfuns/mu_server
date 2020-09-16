using System;

namespace GameServer.Logic
{
	
	public class ShenZhuangHuiKuiHaoLiActivity : Activity
	{
		
		public override AwardItem GetAward(GameClient client)
		{
			return this.MyAwardItem;
		}

		
		public override bool GiveAward(GameClient client, int _params)
		{
			base.GiveAward(client, this.MyAwardItem);
			return true;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return this.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList);
		}

		
		public AwardItem MyAwardItem = new AwardItem();
	}
}
