using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	
	public static class MagicsManyTimeDmageCachingMgr
	{
		
		public static List<ManyTimeDmageItem> GetManyTimeDmageItems(int magicCode)
		{
			List<ManyTimeDmageItem> manyTimeDmageItemList = null;
			List<ManyTimeDmageItem> result;
			if (!MagicsManyTimeDmageCachingMgr.ManyTimeDmageCachingDict.TryGetValue(magicCode, out manyTimeDmageItemList))
			{
				result = null;
			}
			else
			{
				result = manyTimeDmageItemList;
			}
			return result;
		}

		
		public static void ParseManyTimeDmageItems(SystemXmlItems systemMagicMgr)
		{
			Dictionary<int, List<ManyTimeDmageItem>> manyTimeDmageItemsDict = new Dictionary<int, List<ManyTimeDmageItem>>();
			foreach (int key in systemMagicMgr.SystemXmlItemDict.Keys)
			{
				string manyTimeDmage = systemMagicMgr.SystemXmlItemDict[key].GetStringValue("ManyTimeDmage");
				if (null != manyTimeDmage)
				{
					MagicsManyTimeDmageCachingMgr.ParseMagicManyTimeDmage(manyTimeDmageItemsDict, key, manyTimeDmage);
				}
			}
			MagicsManyTimeDmageCachingMgr.ManyTimeDmageCachingDict = manyTimeDmageItemsDict;
		}

		
		private static void ParseMagicManyTimeDmage(Dictionary<int, List<ManyTimeDmageItem>> dict, int id, string manyTimeDmage)
		{
			manyTimeDmage = manyTimeDmage.Trim();
			if (!string.IsNullOrEmpty(manyTimeDmage))
			{
				List<ManyTimeDmageItem> manyTimeDmageItemsList = MagicsManyTimeDmageCachingMgr.ParseItems(id, manyTimeDmage);
				dict[id] = manyTimeDmageItemsList;
			}
		}

		
		private static List<ManyTimeDmageItem> ParseItems(int id, string manyTimeDmage)
		{
			List<ManyTimeDmageItem> manyTimeDmageItemsList = new List<ManyTimeDmageItem>();
			string[] fields = manyTimeDmage.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < fields.Length; i++)
			{
				string[] fields2 = fields[i].Split(new char[]
				{
					','
				});
				if (fields2.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析技能项的多段伤害配置时，个数配置错误, ID={0}", id), null, true);
				}
				else
				{
					ManyTimeDmageItem manyTimeDmageItem = new ManyTimeDmageItem
					{
						InjuredSeconds = (long)Global.SafeConvertToInt32(fields2[0]),
						InjuredPercent = Global.SafeConvertToDouble(fields2[1]),
						manyRangeIndex = i
					};
					manyTimeDmageItemsList.Add(manyTimeDmageItem);
				}
			}
			return manyTimeDmageItemsList;
		}

		
		public static Dictionary<int, List<ManyTimeDmageItem>> ManyTimeDmageCachingDict = new Dictionary<int, List<ManyTimeDmageItem>>();
	}
}
