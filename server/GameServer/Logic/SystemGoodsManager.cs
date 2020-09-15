using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020007D9 RID: 2009
	public class SystemGoodsManager
	{
		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x060038CA RID: 14538 RVA: 0x0030668C File Offset: 0x0030488C
		public Dictionary<string, SystemXmlItem> GoodsItemsDict
		{
			get
			{
				return this._GoodsItemsDict;
			}
		}

		// Token: 0x060038CB RID: 14539 RVA: 0x003066A4 File Offset: 0x003048A4
		public void LoadGoodsItemsDict(SystemXmlItems systemGoodsMgr)
		{
			Dictionary<string, SystemXmlItem> goodsItemsDict = new Dictionary<string, SystemXmlItem>();
			foreach (int key in systemGoodsMgr.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemGoods = systemGoodsMgr.SystemXmlItemDict[key];
				string strKey = systemGoods.GetStringValue("Title");
				goodsItemsDict[strKey] = systemGoods;
			}
			this._GoodsItemsDict = goodsItemsDict;
		}

		// Token: 0x0400416D RID: 16749
		private Dictionary<string, SystemXmlItem> _GoodsItemsDict = null;
	}
}
