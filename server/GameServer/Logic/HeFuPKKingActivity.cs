using System;

namespace GameServer.Logic
{
	
	public class HeFuPKKingActivity : Activity
	{
		
		public override bool GiveAward(GameClient client)
		{
			return base.GiveAward(client, this.MyAwardItem);
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return this.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList);
		}

		
		public AwardItem MyAwardItem = new AwardItem();

		
		public int winerCount = 3;
	}
}
