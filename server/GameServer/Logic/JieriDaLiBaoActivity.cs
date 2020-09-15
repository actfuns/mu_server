using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000702 RID: 1794
	public class JieriDaLiBaoActivity : Activity
	{
		// Token: 0x06002B21 RID: 11041 RVA: 0x00266D7C File Offset: 0x00264F7C
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

		// Token: 0x06002B22 RID: 11042 RVA: 0x00266E00 File Offset: 0x00265000
		public AwardItem GetOccAward(int _params)
		{
			AwardItem myOccAward = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				myOccAward = this.OccAwardItemDict[_params];
			}
			return myOccAward;
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x00266E38 File Offset: 0x00265038
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

		// Token: 0x04003A28 RID: 14888
		public AwardItem MyAwardItem = new AwardItem();

		// Token: 0x04003A29 RID: 14889
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
