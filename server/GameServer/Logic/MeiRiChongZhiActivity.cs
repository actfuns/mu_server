using System;
using System.Collections.Generic;
using GameServer.Logic.ActivityNew;

namespace GameServer.Logic
{
	// Token: 0x0200071A RID: 1818
	public class MeiRiChongZhiActivity : Activity
	{
		// Token: 0x06002B6F RID: 11119 RVA: 0x002682E8 File Offset: 0x002664E8
		public override AwardItem GetAward(GameClient client, int _params)
		{
			AwardItem result;
			if (this.AwardDict.ContainsKey(_params))
			{
				result = this.AwardDict[_params];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06002B70 RID: 11120 RVA: 0x00268320 File Offset: 0x00266520
		public override List<int> GetAwardMinConditionlist()
		{
			List<int> cons = new List<int>();
			int maxPaiHang = this.AwardDict.Count;
			for (int paiHang = 1; paiHang <= maxPaiHang; paiHang++)
			{
				if (this.AwardDict.ContainsKey(paiHang))
				{
					cons.Add(this.AwardDict[paiHang].MinAwardCondionValue);
				}
			}
			return cons;
		}

		// Token: 0x06002B71 RID: 11121 RVA: 0x0026838C File Offset: 0x0026658C
		public int GetIDByYuanBao(int NeedYuanbao)
		{
			foreach (KeyValuePair<int, AwardItem> kvp in this.AwardDict)
			{
				if (kvp.Value.MinAwardCondionValue == NeedYuanbao)
				{
					return kvp.Key;
				}
			}
			return -1;
		}

		// Token: 0x06002B72 RID: 11122 RVA: 0x00268408 File Offset: 0x00266608
		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem myAwardItem = null;
			if (this.AwardDict.ContainsKey(_params))
			{
				myAwardItem = this.AwardDict[_params];
			}
			bool result;
			if (null == myAwardItem)
			{
				result = false;
			}
			else
			{
				client.ClientData.ClearAwardRecord((RoleAwardMsg)this.ActivityType);
				WeedEndInputActivity act = HuodongCachingMgr.GetWeekEndInputActivity();
				if (null != act)
				{
					act.GiveAward(client, myAwardItem.MinAwardCondionValue);
				}
				bool ret = base.GiveAward(client, myAwardItem);
				GameManager.ClientMgr.NotifyGetAwardMsg(client, (RoleAwardMsg)this.ActivityType, "");
				result = ret;
			}
			return result;
		}

		// Token: 0x06002B73 RID: 11123 RVA: 0x002684A8 File Offset: 0x002666A8
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int nBtnIndex)
		{
			int needSpace = 0;
			WeedEndInputActivity act = HuodongCachingMgr.GetWeekEndInputActivity();
			if (null != act)
			{
				needSpace = act.GetNeedGoodsSpace(client, this.AwardDict[nBtnIndex].MinAwardCondionValue);
			}
			needSpace += this.AwardDict[nBtnIndex].GoodsDataList.Count;
			return Global.CanAddGoodsNum(client, needSpace);
		}

		// Token: 0x04003A59 RID: 14937
		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();
	}
}
