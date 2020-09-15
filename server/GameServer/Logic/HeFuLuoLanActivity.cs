using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000717 RID: 1815
	public class HeFuLuoLanActivity : Activity
	{
		// Token: 0x06002B65 RID: 11109 RVA: 0x00268150 File Offset: 0x00266350
		public HeFuLuoLanAward GetHeFuLuoLanAward(int _param)
		{
			HeFuLuoLanAward result;
			if (this.HeFuLuoLanAwardDict.ContainsKey(_param))
			{
				result = this.HeFuLuoLanAwardDict[_param];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06002B66 RID: 11110 RVA: 0x00268188 File Offset: 0x00266388
		public new AwardItem GetAward(int _param)
		{
			AwardItem result;
			if (this.HeFuLuoLanAwardDict.ContainsKey(_param))
			{
				result = this.HeFuLuoLanAwardDict[_param].awardData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06002B67 RID: 11111 RVA: 0x002681C4 File Offset: 0x002663C4
		public override bool GiveAward(GameClient client, int _param)
		{
			AwardItem awardData = this.GetAward(_param);
			return base.GiveAward(client, awardData);
		}

		// Token: 0x06002B68 RID: 11112 RVA: 0x002681E8 File Offset: 0x002663E8
		public new bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _param)
		{
			AwardItem awardData = this.GetAward(_param);
			return awardData == null || awardData.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, awardData.GoodsDataList);
		}

		// Token: 0x04003A54 RID: 14932
		public Dictionary<int, HeFuLuoLanAward> HeFuLuoLanAwardDict = new Dictionary<int, HeFuLuoLanAward>();
	}
}
