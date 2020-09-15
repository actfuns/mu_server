using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000781 RID: 1921
	public class QiZhenGeManager
	{
		// Token: 0x06003157 RID: 12631 RVA: 0x002C2BBC File Offset: 0x002C0DBC
		private static void InitQiZhenGeCachingItems()
		{
			if (QiZhenGeManager.QiZhenGeItemDataList.Count <= 0)
			{
				int basePercent = 0;
				foreach (SystemXmlItem val in GameManager.systemQiZhenGeGoodsMgr.SystemXmlItemDict.Values)
				{
					int percent = (int)(val.GetDoubleValue("Probability") * 10000.0);
					QiZhenGeManager.QiZhenGeItemDataList.Add(new QiZhenGeItemData
					{
						ItemID = val.GetIntValue("ID", -1),
						GoodsID = val.GetIntValue("GoodsID", -1),
						OrigPrice = val.GetIntValue("OrigPrice", -1),
						Price = val.GetIntValue("Price", -1),
						Description = val.GetStringValue("Description"),
						BaseProbability = basePercent,
						SelfProbability = percent
					});
					basePercent += percent;
				}
				if (basePercent > 10000)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析奇珍阁配置项时发生概率溢出10000错误", new object[0]), null, true);
				}
			}
		}

		// Token: 0x06003158 RID: 12632 RVA: 0x002C2D00 File Offset: 0x002C0F00
		public static void ClearQiZhenGeCachingItems()
		{
			lock (QiZhenGeManager.QiZhenMutex)
			{
				QiZhenGeManager.QiZhenGeItemDataList.Clear();
			}
		}

		// Token: 0x06003159 RID: 12633 RVA: 0x002C2D50 File Offset: 0x002C0F50
		private static QiZhenGeItemData PickUpQiZhenGeItemDataByPercent(List<QiZhenGeItemData> qiZhenGeItemDataList, int randPercent)
		{
			QiZhenGeItemData qiZhenGeItemData = null;
			for (int i = 0; i < qiZhenGeItemDataList.Count; i++)
			{
				if (randPercent > qiZhenGeItemDataList[i].BaseProbability && randPercent <= qiZhenGeItemDataList[i].BaseProbability + qiZhenGeItemDataList[i].SelfProbability)
				{
					qiZhenGeItemData = qiZhenGeItemDataList[i];
					break;
				}
			}
			return qiZhenGeItemData;
		}

		// Token: 0x0600315A RID: 12634 RVA: 0x002C2DBC File Offset: 0x002C0FBC
		public static List<QiZhenGeItemData> GetRandomQiZhenGeCachingItems(int maxNum)
		{
			List<QiZhenGeItemData> qiZhenGeItemDataList = null;
			lock (QiZhenGeManager.QiZhenMutex)
			{
				QiZhenGeManager.InitQiZhenGeCachingItems();
				qiZhenGeItemDataList = Global.RandomSortList<QiZhenGeItemData>(QiZhenGeManager.QiZhenGeItemDataList);
				QiZhenGeManager.QiZhenGeItemDataList = qiZhenGeItemDataList;
			}
			List<QiZhenGeItemData> result;
			if (null == qiZhenGeItemDataList)
			{
				result = null;
			}
			else
			{
				List<QiZhenGeItemData> list = new List<QiZhenGeItemData>();
				for (int i = 0; i < maxNum; i++)
				{
					int randNum = Global.GetRandomNumber(1, 10001);
					QiZhenGeItemData qiZhenGeItemData = QiZhenGeManager.PickUpQiZhenGeItemDataByPercent(qiZhenGeItemDataList, randNum);
					list.Add(qiZhenGeItemData);
				}
				result = list;
			}
			return result;
		}

		// Token: 0x0600315B RID: 12635 RVA: 0x002C2E70 File Offset: 0x002C1070
		public static List<QiZhenGeItemData> GetQiZhenGeGoodsList(GameClient client)
		{
			return QiZhenGeManager.GetRandomQiZhenGeCachingItems(Global.MaxNumPerRefreshQiZhenGe);
		}

		// Token: 0x04003DB3 RID: 15795
		public static object QiZhenMutex = new object();

		// Token: 0x04003DB4 RID: 15796
		private static List<QiZhenGeItemData> QiZhenGeItemDataList = new List<QiZhenGeItemData>();
	}
}
