using System;
using System.Collections.Generic;
using GameServer.Logic.ActivityNew;

namespace GameServer.Logic
{
	
	public class MeiRiChongZhiActivity : Activity
	{
		
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

		
		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();
	}
}
