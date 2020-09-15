using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000772 RID: 1906
	public class PaoHuanTasksMgr
	{
		// Token: 0x060030F9 RID: 12537 RVA: 0x002B6848 File Offset: 0x002B4A48
		public static void SetPaoHuanHistTaskID(int roleID, int taskClass, int taskID)
		{
			lock (PaoHuanTasksMgr.PaoHuanHistDict)
			{
				PaoHuanTaskItem paoHuanTaskItem = new PaoHuanTaskItem
				{
					TaskID = taskID,
					AddDateTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd")
				};
				string key = string.Format("{0}_{1}", roleID, taskClass);
				PaoHuanTasksMgr.PaoHuanHistDict[key] = paoHuanTaskItem;
			}
		}

		// Token: 0x060030FA RID: 12538 RVA: 0x002B68DC File Offset: 0x002B4ADC
		public static int FindPaoHuanHistTaskID(int roleID, int taskClass)
		{
			string today = TimeUtil.NowDateTime().ToString("yyyy-MM-dd");
			PaoHuanTaskItem paoHuanTaskItem = null;
			int result;
			lock (PaoHuanTasksMgr.PaoHuanHistDict)
			{
				string key = string.Format("{0}_{1}", roleID, taskClass);
				if (!PaoHuanTasksMgr.PaoHuanHistDict.TryGetValue(key, out paoHuanTaskItem))
				{
					result = -1;
				}
				else if (today != paoHuanTaskItem.AddDateTime)
				{
					result = -1;
				}
				else
				{
					result = paoHuanTaskItem.TaskID;
				}
			}
			return result;
		}

		// Token: 0x04003D80 RID: 15744
		private static Dictionary<string, PaoHuanTaskItem> PaoHuanHistDict = new Dictionary<string, PaoHuanTaskItem>();
	}
}
