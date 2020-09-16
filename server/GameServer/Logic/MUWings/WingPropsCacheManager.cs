using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic.MUWings
{
	
	public class WingPropsCacheManager
	{
		
		public static SystemXmlItem GetWingPropsCacheItem(int occupation, int level)
		{
			string key = string.Format("{0}_{1}", occupation, level);
			SystemXmlItem systemWingPropsCacheItem = null;
			SystemXmlItem result;
			if (!WingPropsCacheManager.WingPropsItemsDict.TryGetValue(key, out systemWingPropsCacheItem))
			{
				result = null;
			}
			else
			{
				result = systemWingPropsCacheItem;
			}
			return result;
		}

		
		public static void LoadWingPropsItemsByOccupation(int occupation)
		{
			if (occupation != 4)
			{
				string fileName = "";
				XElement xml = null;
				try
				{
					fileName = string.Format("Config/Wing/Wing_{0}.xml", occupation);
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
				IEnumerable<XElement> WingPropsXmlItems = xml.Elements("Level");
				foreach (XElement WingPropsItem in WingPropsXmlItems)
				{
					SystemXmlItem systemXmlItem = new SystemXmlItem
					{
						XMLNode = WingPropsItem
					};
					string key = string.Format("{0}_{1}", occupation, (int)Global.GetSafeAttributeLong(WingPropsItem, "ID"));
					WingPropsCacheManager.WingPropsItemsDict[key] = systemXmlItem;
				}
			}
		}

		
		public static void LoadWingPropsItems()
		{
			for (int i = 0; i < 6; i++)
			{
				WingPropsCacheManager.LoadWingPropsItemsByOccupation(i);
			}
		}

		
		private static Dictionary<string, SystemXmlItem> WingPropsItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
