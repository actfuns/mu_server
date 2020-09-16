using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class SystemGoodsManager
	{
		
		
		public Dictionary<string, SystemXmlItem> GoodsItemsDict
		{
			get
			{
				return this._GoodsItemsDict;
			}
		}

		
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

		
		private Dictionary<string, SystemXmlItem> _GoodsItemsDict = null;
	}
}
