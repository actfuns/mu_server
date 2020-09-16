using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class HeFuLoginActivity : Activity
	{
		
		public override AwardItem GetAward(int _params)
		{
			AwardItem AwardList = null;
			if (this.AwardDict.ContainsKey(_params))
			{
				AwardList = this.AwardDict[_params];
			}
			return AwardList;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			AwardItem myAwardItem = this.GetAward(_params);
			return null == myAwardItem || myAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, myAwardItem.GoodsDataList);
		}

		
		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem myAwardItem = this.GetAward(_params);
			return null != myAwardItem && base.GiveAward(client, myAwardItem);
		}

		
		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();
	}
}
