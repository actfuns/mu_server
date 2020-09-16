using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class JieriDaLiBaoActivity : Activity
	{
		
		public override bool GiveAward(GameClient client, int _params)
		{
			bool result2;
			if (null == client)
			{
				result2 = false;
			}
			else
			{
				bool result = true;
				if (null != this.MyAwardItem)
				{
					result = base.GiveAward(client, this.MyAwardItem);
				}
				if (result)
				{
					int occupation = client.ClientData.Occupation;
					AwardItem myOccAward = this.GetOccAward(occupation);
					if (null != myOccAward)
					{
						result = base.GiveAward(client, myOccAward);
					}
				}
				result2 = result;
			}
			return result2;
		}

		
		public AwardItem GetOccAward(int _params)
		{
			AwardItem myOccAward = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				myOccAward = this.OccAwardItemDict[_params];
			}
			return myOccAward;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				int occupation = client.ClientData.Occupation;
				AwardItem myOccAward = this.GetOccAward(occupation);
				result = ((this.MyAwardItem.GoodsDataList.Count <= 0 && (myOccAward == null || myOccAward.GoodsDataList.Count <= 0)) || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList));
			}
			return result;
		}

		
		public AwardItem MyAwardItem = new AwardItem();

		
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
