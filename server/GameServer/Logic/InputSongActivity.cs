using System;

namespace GameServer.Logic
{
	
	public class InputSongActivity : Activity
	{
		
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, this.MyAwardItem);
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			return this.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList);
		}

		
		public AwardItem MyAwardItem = new AwardItem();
	}
}
