using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000714 RID: 1812
	public class HeFuTotalLoginActivity : Activity
	{
		// Token: 0x06002B5D RID: 11101 RVA: 0x00267FE8 File Offset: 0x002661E8
		public override AwardItem GetAward(int _params)
		{
			AwardItem AwardList = null;
			if (this.AwardDict.ContainsKey(_params))
			{
				AwardList = this.AwardDict[_params];
			}
			return AwardList;
		}

		// Token: 0x06002B5E RID: 11102 RVA: 0x00268020 File Offset: 0x00266220
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			AwardItem myAwardItem = this.GetAward(_params);
			return null == myAwardItem || myAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, myAwardItem.GoodsDataList);
		}

		// Token: 0x06002B5F RID: 11103 RVA: 0x0026806C File Offset: 0x0026626C
		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem myAwardItem = this.GetAward(_params);
			return null != myAwardItem && base.GiveAward(client, myAwardItem);
		}

		// Token: 0x04003A4E RID: 14926
		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();
	}
}
