using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	
	public class MagicsCacheManager
	{
		
		public static SystemXmlItem GetMagicCacheItem(int occupation, int skillID, int skillLevel)
		{
			string key = string.Format("{0}_{1}_{2}", occupation, skillID, skillLevel);
			SystemXmlItem systemMagicCacheItem = null;
			SystemXmlItem result;
			if (!MagicsCacheManager.MagicItemsDict.TryGetValue(key, out systemMagicCacheItem))
			{
				result = null;
			}
			else
			{
				result = systemMagicCacheItem;
			}
			return result;
		}

		
		public static void LoadMagicItemsByOccupation(int occupation)
		{
			if (occupation != 4)
			{
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/Magics/Magics_{0}.xml", occupation);
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
				IEnumerable<XElement> jiNengXmlItems = xml.Elements("Magic");
				foreach (XElement jiNengItem in jiNengXmlItems)
				{
					IEnumerable<XElement> items = jiNengItem.Elements("JiNeng");
					foreach (XElement item in items)
					{
						SystemXmlItem systemXmlItem = new SystemXmlItem
						{
							XMLNode = item
						};
						string key = string.Format("{0}_{1}_{2}", occupation, (int)Global.GetSafeAttributeLong(jiNengItem, "ID"), (int)Global.GetSafeAttributeLong(item, "Level"));
						MagicsCacheManager.MagicItemsDict[key] = systemXmlItem;
					}
				}
			}
		}

		
		public static void LoadMagicItems()
		{
			for (int i = 0; i < 6; i++)
			{
				MagicsCacheManager.LoadMagicItemsByOccupation(i);
			}
		}

		
		private static Dictionary<string, SystemXmlItem> MagicItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
