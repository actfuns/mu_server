using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_EquipUsing : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			List<int> iArgList = Global.StringToIntList(arg, '|');
			int itemCat = iArgList[0];
			int forgelev = 0;
			if (iArgList.Count >= 2)
			{
				forgelev = iArgList[1];
			}
			List<int> categoriyList = new List<int>();
			if (1 <= itemCat && itemCat <= 5)
			{
				categoriyList.Add(itemCat - 1);
			}
			else if (6 == itemCat || 7 == itemCat)
			{
				for (int i = 11; i <= 21; i++)
				{
					categoriyList.Add(i);
				}
			}
			else if (8 == itemCat || 9 == itemCat)
			{
				categoriyList.Add(6);
			}
			else if (10 == itemCat)
			{
				categoriyList.Add(5);
			}
			GoodsData goods = null;
			List<GoodsData> usingEquipList = client.UsingEquipMgr.GetGoodsByCategoriyList(categoriyList);
			if (usingEquipList == null || usingEquipList.Count == 0)
			{
				bOK = false;
			}
			else if ((1 <= itemCat && itemCat <= 5) || 10 == itemCat)
			{
				bOK = true;
				goods = usingEquipList[0];
			}
			else if (6 == itemCat || 7 == itemCat)
			{
				goods = usingEquipList.Find(delegate(GoodsData x)
				{
					int nHandType = -1;
					int ActionType = -1;
					SystemXmlItem systemGoods = null;
					if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(x.GoodsID, out systemGoods))
					{
						nHandType = systemGoods.GetIntValue("HandType", -1);
						ActionType = systemGoods.GetIntValue("ActionType", -1);
					}
					return (6 == itemCat && nHandType == 1) || (6 == itemCat && nHandType == 2 && x.BagIndex == 0) || (7 == itemCat && nHandType == 0) || (7 == itemCat && nHandType == 2 && x.BagIndex == 1) || (ActionType != 1 && ActionType != 4);
				});
				if (null != goods)
				{
					bOK = true;
				}
			}
			else if (8 == itemCat || 9 == itemCat)
			{
				if (8 == itemCat)
				{
					goods = usingEquipList.Find((GoodsData x) => x.BagIndex == 0);
				}
				else
				{
					goods = usingEquipList.Find((GoodsData x) => x.BagIndex == 1);
				}
				if (null != goods)
				{
					bOK = true;
				}
			}
			if (!bOK || null == goods)
			{
				failedMsg = GLang.GetLang(8016, new object[0]);
			}
			else if (forgelev > 0)
			{
				if (goods.Forge_level >= forgelev)
				{
					bOK = false;
					failedMsg = GLang.GetLang(8020, new object[0]);
				}
			}
			return bOK;
		}
	}
}
