using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	// Token: 0x020006B8 RID: 1720
	public class EquipUpgradeCacheMgr
	{
		// Token: 0x0600205A RID: 8282 RVA: 0x001BDB8C File Offset: 0x001BBD8C
		public static SystemXmlItem GetEquipUpgradeCacheItem(int categoriy, int suitID)
		{
			SystemXmlItem result;
			if (null == EquipUpgradeCacheMgr._EquipUpgradeItemsDict)
			{
				result = null;
			}
			else
			{
				string key = string.Format("{0}_{1}", categoriy, suitID);
				SystemXmlItem systemEquipUpgradeItem = null;
				if (!EquipUpgradeCacheMgr._EquipUpgradeItemsDict.TryGetValue(key, out systemEquipUpgradeItem))
				{
					result = null;
				}
				else
				{
					result = systemEquipUpgradeItem;
				}
			}
			return result;
		}

		// Token: 0x0600205B RID: 8283 RVA: 0x001BDBE4 File Offset: 0x001BBDE4
		public static SystemXmlItem GetEquipUpgradeItemByGoodsID(int goodsID, int maxSuiItID)
		{
			SystemXmlItem systemGoods = null;
			SystemXmlItem result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
			{
				result = null;
			}
			else
			{
				int categoriy = systemGoods.GetIntValue("Categoriy", -1);
				if (categoriy < 0 || categoriy >= 49)
				{
					result = null;
				}
				else
				{
					int suitID = systemGoods.GetIntValue("SuitID", -1);
					if (suitID < 1 || suitID > maxSuiItID)
					{
						suitID = 1;
					}
					result = EquipUpgradeCacheMgr.GetEquipUpgradeCacheItem(categoriy, suitID);
				}
			}
			return result;
		}

		// Token: 0x0600205C RID: 8284 RVA: 0x001BDC68 File Offset: 0x001BBE68
		public static void LoadEquipUpgradeItems()
		{
			string fileName = "";
			XElement xml = null;
			try
			{
				fileName = string.Format("Config/EquipUpgrade.xml", new object[0]);
				xml = XElement.Load(Global.GameResPath(fileName));
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			Dictionary<string, SystemXmlItem> equipUpgradeItemsDict = new Dictionary<string, SystemXmlItem>();
			IEnumerable<XElement> jiNengXmlItems = xml.Elements("Equip");
			foreach (XElement jiNengItem in jiNengXmlItems)
			{
				IEnumerable<XElement> items = jiNengItem.Elements("Item");
				foreach (XElement item in items)
				{
					SystemXmlItem systemXmlItem = new SystemXmlItem
					{
						XMLNode = item
					};
					string key = string.Format("{0}_{1}", (int)Global.GetSafeAttributeLong(jiNengItem, "Categoriy"), (int)Global.GetSafeAttributeLong(item, "SuitID"));
					equipUpgradeItemsDict[key] = systemXmlItem;
				}
			}
			EquipUpgradeCacheMgr._EquipUpgradeItemsDict = equipUpgradeItemsDict;
		}

		// Token: 0x0400365E RID: 13918
		private static Dictionary<string, SystemXmlItem> _EquipUpgradeItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
