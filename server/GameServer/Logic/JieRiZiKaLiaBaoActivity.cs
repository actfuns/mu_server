using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200070C RID: 1804
	public class JieRiZiKaLiaBaoActivity : Activity
	{
		// Token: 0x06002B4A RID: 11082 RVA: 0x00267BB0 File Offset: 0x00265DB0
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

		// Token: 0x06002B4B RID: 11083 RVA: 0x00267C3C File Offset: 0x00265E3C
		public new JieRiZiKa GetAward(int id)
		{
			JieRiZiKa config = null;
			if (this.JieRiZiKaDict.ContainsKey(id))
			{
				config = this.JieRiZiKaDict[id];
			}
			return config;
		}

		// Token: 0x06002B4C RID: 11084 RVA: 0x00267C74 File Offset: 0x00265E74
		public override bool GiveAward(GameClient client, int _params)
		{
			JieRiZiKa config = this.GetAward(_params);
			return null != config && base.GiveAward(client, config.MyAwardItem);
		}

		// Token: 0x06002B4D RID: 11085 RVA: 0x00267CAC File Offset: 0x00265EAC
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			JieRiZiKa config = this.GetAward(_params);
			return null != config && null != config.MyAwardItem && (config.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, config.MyAwardItem.GoodsDataList));
		}

		// Token: 0x04003A44 RID: 14916
		public Dictionary<int, JieRiZiKa> JieRiZiKaDict = new Dictionary<int, JieRiZiKa>();
	}
}
