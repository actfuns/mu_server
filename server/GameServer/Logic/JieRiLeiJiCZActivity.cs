using System;
using System.Collections.Generic;
using System.Text;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000706 RID: 1798
	public class JieRiLeiJiCZActivity : Activity
	{
		// Token: 0x06002B33 RID: 11059 RVA: 0x0026732C File Offset: 0x0026552C
		public override AwardItem GetAward(int _params)
		{
			AwardItem myAwardItem = null;
			if (this.AwardItemDict.ContainsKey(_params))
			{
				myAwardItem = this.AwardItemDict[_params];
			}
			return myAwardItem;
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x00267364 File Offset: 0x00265564
		public AwardItem GetOccAward(int _params)
		{
			AwardItem myAwardItem = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				myAwardItem = this.OccAwardItemDict[_params];
			}
			return myAwardItem;
		}

		// Token: 0x06002B35 RID: 11061 RVA: 0x0026739C File Offset: 0x0026559C
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
					EventLogManager.AddJieriLeiJiCZEvent(client, _params, strResList);
				}
			}
			return result;
		}

		// Token: 0x06002B36 RID: 11062 RVA: 0x00267474 File Offset: 0x00265674
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

		// Token: 0x06002B37 RID: 11063 RVA: 0x002674EC File Offset: 0x002656EC
		public override string GetAwardMinConditionValues()
		{
			StringBuilder strBuilder = new StringBuilder();
			int maxPaiHang = this.AwardItemDict.Count;
			for (int paiHang = 1; paiHang <= maxPaiHang; paiHang++)
			{
				if (this.AwardItemDict.ContainsKey(paiHang))
				{
					if (strBuilder.Length > 0)
					{
						strBuilder.Append("_");
					}
					strBuilder.Append(this.AwardItemDict[paiHang].MinAwardCondionValue);
				}
			}
			return strBuilder.ToString();
		}

		// Token: 0x04003A2F RID: 14895
		public Dictionary<int, AwardItem> AwardItemDict = new Dictionary<int, AwardItem>();

		// Token: 0x04003A30 RID: 14896
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
