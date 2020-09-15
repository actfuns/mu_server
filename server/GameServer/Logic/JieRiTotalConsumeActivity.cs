using System;
using System.Collections.Generic;
using System.Text;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000707 RID: 1799
	public class JieRiTotalConsumeActivity : Activity
	{
		// Token: 0x06002B39 RID: 11065 RVA: 0x002675A0 File Offset: 0x002657A0
		public override AwardItem GetAward(int _params)
		{
			AwardItem myAwardItem = null;
			if (this.AwardItemDict.ContainsKey(_params))
			{
				myAwardItem = this.AwardItemDict[_params];
			}
			return myAwardItem;
		}

		// Token: 0x06002B3A RID: 11066 RVA: 0x002675D8 File Offset: 0x002657D8
		public AwardItem GetOccAward(int _params)
		{
			AwardItem myAwardItem = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				myAwardItem = this.OccAwardItemDict[_params];
			}
			return myAwardItem;
		}

		// Token: 0x06002B3B RID: 11067 RVA: 0x00267610 File Offset: 0x00265810
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
			}
			return result;
		}

		// Token: 0x06002B3C RID: 11068 RVA: 0x00267674 File Offset: 0x00265874
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

		// Token: 0x06002B3D RID: 11069 RVA: 0x002676EC File Offset: 0x002658EC
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

		// Token: 0x04003A31 RID: 14897
		public Dictionary<int, AwardItem> AwardItemDict = new Dictionary<int, AwardItem>();

		// Token: 0x04003A32 RID: 14898
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
