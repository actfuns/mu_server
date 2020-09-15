using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	// Token: 0x020006EA RID: 1770
	public class HorseCachingManager
	{
		// Token: 0x06002ABF RID: 10943 RVA: 0x00263348 File Offset: 0x00261548
		public static SystemXmlItem GetHorseEnchanceItem(int level, HorseExtIndexes extIndex)
		{
			string key = string.Format("{0}_{1}", level, HorseCachingManager.XmlItemNames[(int)extIndex]);
			SystemXmlItem systemHorseEnchanceItem = null;
			SystemXmlItem result;
			if (!HorseCachingManager.HorseItemsDict.TryGetValue(key, out systemHorseEnchanceItem))
			{
				result = null;
			}
			else
			{
				result = systemHorseEnchanceItem;
			}
			return result;
		}

		// Token: 0x06002AC0 RID: 10944 RVA: 0x0026338C File Offset: 0x0026158C
		public static void LoadHorseEnchanceItems()
		{
			string fileName = "";
			XElement xml = null;
			try
			{
				fileName = string.Format("Config/Horses/HorseEnchance.xml", new object[0]);
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
			IEnumerable<XElement> levelXmlItems = xml.Elements("Levels");
			foreach (XElement levelXmlItem in levelXmlItems)
			{
				IEnumerable<XElement> propItems = levelXmlItem.Elements();
				foreach (XElement propItem in propItems)
				{
					SystemXmlItem systemXmlItem = new SystemXmlItem
					{
						XMLNode = propItem
					};
					string key = string.Format("{0}_{1}", Global.GetSafeAttributeStr(levelXmlItem, "level"), propItem.Name);
					HorseCachingManager.HorseItemsDict[key] = systemXmlItem;
				}
			}
		}

		// Token: 0x040039D4 RID: 14804
		private static string[] XmlItemNames = new string[]
		{
			"WuGong",
			"WuFang",
			"MoGong",
			"MoFang",
			"BaoJi",
			"MingZong",
			"ShanBi",
			"ShengL",
			"MoFaL",
			"KangBao"
		};

		// Token: 0x040039D5 RID: 14805
		private static Dictionary<string, SystemXmlItem> HorseItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
