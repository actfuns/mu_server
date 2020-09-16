using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class JieriCZSongActivity : Activity
	{
		
		public override AwardItem GetAward(int _params)
		{
			AwardItem myAwardItem = null;
			if (this.AwardItemDict.ContainsKey(_params))
			{
				myAwardItem = this.AwardItemDict[_params];
			}
			return myAwardItem;
		}

		
		public AwardItem GetOccAward(int _params)
		{
			AwardItem myAwardItem = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				myAwardItem = this.OccAwardItemDict[_params];
			}
			return myAwardItem;
		}

		
		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem myAwardItem = this.GetAward(_params);
			bool result = true;
			if (null != myAwardItem)
			{
				result = base.GiveAward(client, myAwardItem);
			}
			if (result)
			{
				AwardItem myOccAward = this.GetOccAward(_params);
				if (null != myOccAward)
				{
					result = base.GiveAward(client, myOccAward);
				}
				if (result)
				{
					string strResList = "";
					if (null != myAwardItem)
					{
						strResList = EventLogManager.MakeGoodsDataPropString(myAwardItem.GoodsDataList);
					}
					if (!string.IsNullOrEmpty(strResList))
					{
						strResList += "@";
					}
					if (null != myOccAward)
					{
						strResList += EventLogManager.MakeGoodsDataPropString(myOccAward.GoodsDataList);
					}
					EventLogManager.AddJieriCZSongEvent(client, _params, strResList);
				}
			}
			return result;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			AwardItem myAwardItem = this.GetAward(_params);
			AwardItem myOccAwardItem = this.GetOccAward(_params);
			List<GoodsData> GoodsDataList = new List<GoodsData>();
			if (null != myAwardItem)
			{
				GoodsDataList.AddRange(myAwardItem.GoodsDataList);
			}
			if (null != myOccAwardItem)
			{
				GoodsDataList.AddRange(myOccAwardItem.GoodsDataList);
			}
			return GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, GoodsDataList);
		}

		
		public Dictionary<int, AwardItem> AwardItemDict = new Dictionary<int, AwardItem>();

		
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
