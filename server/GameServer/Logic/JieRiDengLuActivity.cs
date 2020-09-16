using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class JieRiDengLuActivity : Activity
	{
		
		public override AwardItem GetAward(GameClient client, int day)
		{
			AwardItem myAward = null;
			if (this.AwardItemDict.ContainsKey(day))
			{
				myAward = this.AwardItemDict[day];
			}
			return myAward;
		}

		
		public AwardItem GetOccAward(GameClient client, int day)
		{
			AwardItem result;
			if (null == client)
			{
				result = null;
			}
			else
			{
				AwardItem myOccAward = null;
				int key = day * 100 + client.ClientData.Occupation;
				if (this.OccAwardItemDict.ContainsKey(key))
				{
					myOccAward = this.OccAwardItemDict[key];
				}
				result = myOccAward;
			}
			return result;
		}

		
		public AwardItem GetOccAward(int key)
		{
			AwardItem myOccAward = null;
			if (this.OccAwardItemDict.ContainsKey(key))
			{
				myOccAward = this.OccAwardItemDict[key];
			}
			return myOccAward;
		}

		
		public override bool GiveAward(GameClient client, int _params)
		{
			bool result2;
			if (null == client)
			{
				result2 = false;
			}
			else
			{
				AwardItem myAwardItem = this.GetAward(client, _params);
				bool result = true;
				if (null != myAwardItem)
				{
					result = base.GiveAward(client, myAwardItem);
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

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				AwardItem myAwardItem = this.GetAward(client, _params);
				AwardItem myOccAward = this.GetOccAward(client, _params);
				List<GoodsData> GoodsDataList = new List<GoodsData>();
				if (myAwardItem != null && null != myAwardItem.GoodsDataList)
				{
					GoodsDataList.AddRange(myAwardItem.GoodsDataList);
				}
				if (myOccAward != null && null != myOccAward.GoodsDataList)
				{
					GoodsDataList.AddRange(myOccAward.GoodsDataList);
				}
				result = (GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, GoodsDataList));
			}
			return result;
		}

		
		public Dictionary<int, AwardItem> AwardItemDict = new Dictionary<int, AwardItem>();

		
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
