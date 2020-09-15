using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200051D RID: 1309
	public static class MagicsManyTimeDmageCachingMgr
	{
		// Token: 0x060018D6 RID: 6358 RVA: 0x001846CC File Offset: 0x001828CC
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

		// Token: 0x060018D7 RID: 6359 RVA: 0x001846F8 File Offset: 0x001828F8
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

		// Token: 0x060018D8 RID: 6360 RVA: 0x00184790 File Offset: 0x00182990
		private static void ParseMagicManyTimeDmage(Dictionary<int, List<ManyTimeDmageItem>> dict, int id, string manyTimeDmage)
		{
			manyTimeDmage = manyTimeDmage.Trim();
			if (!string.IsNullOrEmpty(manyTimeDmage))
			{
				List<ManyTimeDmageItem> manyTimeDmageItemsList = MagicsManyTimeDmageCachingMgr.ParseItems(id, manyTimeDmage);
				dict[id] = manyTimeDmageItemsList;
			}
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x001847C8 File Offset: 0x001829C8
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

		// Token: 0x040022D0 RID: 8912
		public static Dictionary<int, List<ManyTimeDmageItem>> ManyTimeDmageCachingDict = new Dictionary<int, List<ManyTimeDmageItem>>();
	}
}
