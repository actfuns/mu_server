using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000711 RID: 1809
	public class HeFuLoginActivity : Activity
	{
		// Token: 0x06002B56 RID: 11094 RVA: 0x00267EC8 File Offset: 0x002660C8
		public override AwardItem GetAward(int _params)
		{
			AwardItem AwardList = null;
			if (this.AwardDict.ContainsKey(_params))
			{
				AwardList = this.AwardDict[_params];
			}
			return AwardList;
		}

		// Token: 0x06002B57 RID: 11095 RVA: 0x00267F00 File Offset: 0x00266100
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			AwardItem myAwardItem = this.GetAward(_params);
			return null == myAwardItem || myAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, myAwardItem.GoodsDataList);
		}

		// Token: 0x06002B58 RID: 11096 RVA: 0x00267F4C File Offset: 0x0026614C
		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem myAwardItem = this.GetAward(_params);
			return null != myAwardItem && base.GiveAward(client, myAwardItem);
		}

		// Token: 0x04003A49 RID: 14921
		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();
	}
}
