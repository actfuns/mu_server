using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000705 RID: 1797
	public class JieriCZSongActivity : Activity
	{
		// Token: 0x06002B2E RID: 11054 RVA: 0x00267150 File Offset: 0x00265350
		public override AwardItem GetAward(int _params)
		{
			AwardItem myAwardItem = null;
			if (this.AwardItemDict.ContainsKey(_params))
			{
				myAwardItem = this.AwardItemDict[_params];
			}
			return myAwardItem;
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x00267188 File Offset: 0x00265388
		public AwardItem GetOccAward(int _params)
		{
			AwardItem myAwardItem = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				myAwardItem = this.OccAwardItemDict[_params];
			}
			return myAwardItem;
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x002671C0 File Offset: 0x002653C0
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

		// Token: 0x06002B31 RID: 11057 RVA: 0x00267298 File Offset: 0x00265498
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

		// Token: 0x04003A2D RID: 14893
		public Dictionary<int, AwardItem> AwardItemDict = new Dictionary<int, AwardItem>();

		// Token: 0x04003A2E RID: 14894
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
