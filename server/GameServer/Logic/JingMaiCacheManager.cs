using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	
	public class JingMaiCacheManager
	{
		
		public static SystemXmlItem GetJingMaiItem(int occupation, int jingMaiID, int jingMaiBodyLevel)
		{
			string key = string.Format("{0}_{1}_{2}", occupation, jingMaiID, jingMaiBodyLevel);
			SystemXmlItem systemJingMaiItem = null;
			SystemXmlItem result;
			if (!JingMaiCacheManager.JingMaiItemsDict.TryGetValue(key, out systemJingMaiItem))
			{
				result = null;
			}
			else
			{
				result = systemJingMaiItem;
			}
			return result;
		}

		
		public static void LoadJingMaiItemsByOccupation(int occupation)
		{
			string fileName = "";
			XElement xml = null;
			try
			{
				fileName = string.Format("Config/JingMais/{0}.xml", occupation);
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
			IEnumerable<XElement> JingMaiXmlItems = xml.Elements("JingMai");
			foreach (XElement JingMaiItem in JingMaiXmlItems)
			{
				IEnumerable<XElement> ChongItems = JingMaiItem.Elements("Chong");
				foreach (XElement ChongItem in ChongItems)
				{
					SystemXmlItem systemXmlItem = new SystemXmlItem
					{
						XMLNode = ChongItem
					};
					string key = string.Format("{0}_{1}_{2}", occupation, (int)Global.GetSafeAttributeLong(JingMaiItem, "ID"), (int)Global.GetSafeAttributeLong(ChongItem, "ID"));
					JingMaiCacheManager.JingMaiItemsDict[key] = systemXmlItem;
				}
			}
		}

		
		public static void LoadJingMaiItems()
		{
			for (int i = 0; i < 3; i++)
			{
				JingMaiCacheManager.LoadJingMaiItemsByOccupation(i);
			}
		}

		
		private static Dictionary<string, SystemXmlItem> JingMaiItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
