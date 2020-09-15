using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic.MUWings
{
	// Token: 0x0200076A RID: 1898
	public class WingStarCacheManager
	{
		// Token: 0x060030C5 RID: 12485 RVA: 0x002B4B94 File Offset: 0x002B2D94
		public static SystemXmlItem GetWingStarCacheItem(int occupation, int level, int starNum)
		{
			string key = string.Format("{0}_{1}_{2}", occupation, level, starNum);
			SystemXmlItem systemWingStarCacheItem = null;
			SystemXmlItem result;
			if (!WingStarCacheManager.WingStarItemsDict.TryGetValue(key, out systemWingStarCacheItem))
			{
				result = null;
			}
			else
			{
				result = systemWingStarCacheItem;
			}
			return result;
		}

		// Token: 0x060030C6 RID: 12486 RVA: 0x002B4BDC File Offset: 0x002B2DDC
		public static void LoadWingStarItemsByOccupation(int occupation)
		{
			if (occupation != 4)
			{
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/Wing/WingStar_{0}.xml", occupation);
					xml = XElement.Load(Global.GameResPath(fileName));
					if (null == xml)
					{
						throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
				}
				IEnumerable<XElement> WingStarXmlItems = xml.Elements("Wing");
				foreach (XElement WingStarItem in WingStarXmlItems)
				{
					IEnumerable<XElement> items = WingStarItem.Elements("Item");
					foreach (XElement item in items)
					{
						SystemXmlItem systemXmlItem = new SystemXmlItem
						{
							XMLNode = item
						};
						string key = string.Format("{0}_{1}_{2}", occupation, (int)Global.GetSafeAttributeLong(WingStarItem, "ID"), (int)Global.GetSafeAttributeLong(item, "Star"));
						WingStarCacheManager.WingStarItemsDict[key] = systemXmlItem;
					}
				}
			}
		}

		// Token: 0x060030C7 RID: 12487 RVA: 0x002B4D7C File Offset: 0x002B2F7C
		public static void LoadWingStarItems()
		{
			for (int i = 0; i < 6; i++)
			{
				WingStarCacheManager.LoadWingStarItemsByOccupation(i);
			}
		}

		// Token: 0x04003D62 RID: 15714
		private static Dictionary<string, SystemXmlItem> WingStarItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
