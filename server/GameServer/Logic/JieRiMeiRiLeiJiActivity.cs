using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class JieRiMeiRiLeiJiActivity : Activity
	{
		
		public override AwardItem GetAward(int _params)
		{
			AwardItem myAwardItem = null;
			int day = _params / 1000;
			int index = _params % 1000;
			AwardItem result;
			if (index < 1)
			{
				result = null;
			}
			else
			{
				if (this.DayAwardItemDict.ContainsKey(day))
				{
					if (this.DayAwardItemDict[day].Count < index)
					{
						return null;
					}
					myAwardItem = this.DayAwardItemDict[day][index - 1];
				}
				result = myAwardItem;
			}
			return result;
		}

		
		public AwardItem GetOccAward(int _params)
		{
			AwardItem myAwardItem = null;
			int day = _params / 1000;
			int key = _params % 1000;
			if (this.DayOccAwardItemDict.ContainsKey(day))
			{
				if (this.DayOccAwardItemDict[day].ContainsKey(key))
				{
					myAwardItem = this.DayOccAwardItemDict[day][key];
				}
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
					EventLogManager.AddJieRiMeiRiLeiJiEvent(client, _params, strResList);
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

		
		public Dictionary<int, List<AwardItem>> DayAwardItemDict = new Dictionary<int, List<AwardItem>>();

		
		public Dictionary<int, Dictionary<int, AwardItem>> DayOccAwardItemDict = new Dictionary<int, Dictionary<int, AwardItem>>();
	}
}
