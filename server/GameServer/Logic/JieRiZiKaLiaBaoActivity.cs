using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class JieRiZiKaLiaBaoActivity : Activity
	{
		
		public List<int> GetIndexByType(int type)
		{
			List<int> IndexList = new List<int>();
			foreach (KeyValuePair<int, JieRiZiKa> item in this.JieRiZiKaDict)
			{
				if (type == item.Value.type)
				{
					IndexList.Add(item.Key);
				}
			}
			return IndexList;
		}

		
		public new JieRiZiKa GetAward(int id)
		{
			JieRiZiKa config = null;
			if (this.JieRiZiKaDict.ContainsKey(id))
			{
				config = this.JieRiZiKaDict[id];
			}
			return config;
		}

		
		public override bool GiveAward(GameClient client, int _params)
		{
			JieRiZiKa config = this.GetAward(_params);
			return null != config && base.GiveAward(client, config.MyAwardItem);
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			JieRiZiKa config = this.GetAward(_params);
			return null != config && null != config.MyAwardItem && (config.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, config.MyAwardItem.GoodsDataList));
		}

		
		public Dictionary<int, JieRiZiKa> JieRiZiKaDict = new Dictionary<int, JieRiZiKa>();
	}
}
