using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.Goods
{
	// Token: 0x02000473 RID: 1139
	public class CondJudger_EquipUsingAll : ICondJudger
	{
		// Token: 0x060014CB RID: 5323 RVA: 0x00145A7C File Offset: 0x00143C7C
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = true;
			int forgelev = Global.SafeConvertToInt32(arg);
			List<int> categoriyList = new List<int>();
			for (int i = 0; i <= 6; i++)
			{
				categoriyList.Add(i);
			}
			List<GoodsData> usingEquipList = client.UsingEquipMgr.GetGoodsByCategoriyList(categoriyList);
			if (usingEquipList == null || usingEquipList.Count != categoriyList.Count + 1)
			{
				bOK = false;
			}
			List<GoodsData> usingEquipList2 = null;
			if (bOK)
			{
				categoriyList.Clear();
				for (int i = 11; i <= 21; i++)
				{
					categoriyList.Add(i);
				}
				usingEquipList2 = client.UsingEquipMgr.GetGoodsByCategoriyList(categoriyList);
				if (usingEquipList2 == null || usingEquipList2.Count == 0)
				{
					bOK = false;
				}
				else if (usingEquipList2.Count == 1)
				{
					SystemXmlItem systemGoods = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(usingEquipList2[0].GoodsID, out systemGoods))
					{
						bOK = false;
					}
					else
					{
						int ActionType = systemGoods.GetIntValue("ActionType", -1);
						if (ActionType == 1 || ActionType == 4)
						{
							bOK = false;
						}
					}
				}
			}
			if (!bOK)
			{
				failedMsg = GLang.GetLang(8017, new object[0]);
			}
			else if (forgelev > 0)
			{
				bool flag;
				if (usingEquipList.TrueForAll((GoodsData x) => x.Forge_level >= forgelev))
				{
					flag = !usingEquipList2.TrueForAll((GoodsData x) => x.Forge_level >= forgelev);
				}
				else
				{
					flag = true;
				}
				if (!flag)
				{
					bOK = false;
					failedMsg = GLang.GetLang(8020, new object[0]);
				}
			}
			return bOK;
		}
	}
}
