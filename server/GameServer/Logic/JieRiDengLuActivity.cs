using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000703 RID: 1795
	public class JieRiDengLuActivity : Activity
	{
		// Token: 0x06002B25 RID: 11045 RVA: 0x00266ED4 File Offset: 0x002650D4
		public override AwardItem GetAward(GameClient client, int day)
		{
			AwardItem myAward = null;
			if (this.AwardItemDict.ContainsKey(day))
			{
				myAward = this.AwardItemDict[day];
			}
			return myAward;
		}

		// Token: 0x06002B26 RID: 11046 RVA: 0x00266F0C File Offset: 0x0026510C
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

		// Token: 0x06002B27 RID: 11047 RVA: 0x00266F64 File Offset: 0x00265164
		public AwardItem GetOccAward(int key)
		{
			AwardItem myOccAward = null;
			if (this.OccAwardItemDict.ContainsKey(key))
			{
				myOccAward = this.OccAwardItemDict[key];
			}
			return myOccAward;
		}

		// Token: 0x06002B28 RID: 11048 RVA: 0x00266F9C File Offset: 0x0026519C
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

		// Token: 0x06002B29 RID: 11049 RVA: 0x00267020 File Offset: 0x00265220
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

		// Token: 0x04003A2A RID: 14890
		public Dictionary<int, AwardItem> AwardItemDict = new Dictionary<int, AwardItem>();

		// Token: 0x04003A2B RID: 14891
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
