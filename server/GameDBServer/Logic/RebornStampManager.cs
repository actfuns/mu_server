using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	// Token: 0x02000166 RID: 358
	internal class RebornStampManager
	{
		// Token: 0x0600062C RID: 1580 RVA: 0x00037994 File Offset: 0x00035B94
		public static void InitRebornYinJi(DBManager dbMgr)
		{
			RebornStampManager.UserRebornData = DBQuery.GetRebornYinJiCached(dbMgr);
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x000379A4 File Offset: 0x00035BA4
		public static List<int> UnMakeYinJiUpdateInfo(string UpdateInfo)
		{
			List<int> list = new List<int>();
			List<int> result;
			if (UpdateInfo == "" || UpdateInfo == null)
			{
				result = list;
			}
			else
			{
				string[] strInfo = UpdateInfo.Split(new char[]
				{
					'|'
				});
				string[] strs = strInfo[0].Split(new char[]
				{
					'_'
				});
				foreach (string it in strs)
				{
					list.Add(Convert.ToInt32(it));
				}
				strs = strInfo[1].Split(new char[]
				{
					'_'
				});
				foreach (string it in strs)
				{
					list.Add(Convert.ToInt32(it));
				}
				result = list;
			}
			return result;
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x00037A94 File Offset: 0x00035C94
		public static RebornStampData GetUserRebornInfoFromCached(int RoleID)
		{
			RebornStampData result;
			if (!RebornStampManager.UserRebornData.ContainsKey(RoleID))
			{
				result = null;
			}
			else
			{
				result = RebornStampManager.UserRebornData[RoleID];
			}
			return result;
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00037AC8 File Offset: 0x00035CC8
		public static bool UpdateUserRebornInfo(int RoleID, string StampInfo, int ResetNum, int UsePoint)
		{
			bool result;
			if (RebornStampManager.UserRebornData.ContainsKey(RoleID))
			{
				List<int> list = RebornStampManager.UnMakeYinJiUpdateInfo(StampInfo);
				RebornStampManager.UserRebornData[RoleID].ResetNum = ResetNum;
				RebornStampManager.UserRebornData[RoleID].UsePoint = UsePoint;
				RebornStampManager.UserRebornData[RoleID].StampInfo = list;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00037B30 File Offset: 0x00035D30
		public static bool InsertUserRebornInfo(int RoleID, string StampInfo, int ResetNum, int UsePoint)
		{
			bool result;
			if (RebornStampManager.UserRebornData.ContainsKey(RoleID))
			{
				result = false;
			}
			else
			{
				RebornStampData data = new RebornStampData();
				data.RoleID = RoleID;
				data.ResetNum = ResetNum;
				data.UsePoint = UsePoint;
				data.StampInfo = RebornStampManager.UnMakeYinJiUpdateInfo(StampInfo);
				RebornStampManager.UserRebornData.Add(RoleID, data);
				result = true;
			}
			return result;
		}

		// Token: 0x0400087F RID: 2175
		public static Dictionary<int, RebornStampData> UserRebornData = new Dictionary<int, RebornStampData>();
	}
}
