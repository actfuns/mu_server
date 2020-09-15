using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	// Token: 0x02000737 RID: 1847
	public class JingMaiCacheManager
	{
		// Token: 0x06002E02 RID: 11778 RVA: 0x002862B0 File Offset: 0x002844B0
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

		// Token: 0x06002E03 RID: 11779 RVA: 0x002862F8 File Offset: 0x002844F8
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

		// Token: 0x06002E04 RID: 11780 RVA: 0x00286480 File Offset: 0x00284680
		public static void LoadJingMaiItems()
		{
			for (int i = 0; i < 3; i++)
			{
				JingMaiCacheManager.LoadJingMaiItemsByOccupation(i);
			}
		}

		// Token: 0x04003C1E RID: 15390
		private static Dictionary<string, SystemXmlItem> JingMaiItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
