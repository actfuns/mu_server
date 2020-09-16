using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic.MUWings
{
	
	public class WingStarCacheManager
	{
		
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

		
		public static void LoadWingStarItems()
		{
			for (int i = 0; i < 6; i++)
			{
				WingStarCacheManager.LoadWingStarItemsByOccupation(i);
			}
		}

		
		private static Dictionary<string, SystemXmlItem> WingStarItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
