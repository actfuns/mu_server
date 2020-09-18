using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	
	internal class RebornStampManager
	{
		
		public static void InitRebornYinJi(DBManager dbMgr)
		{
			RebornStampManager.UserRebornData = DBQuery.GetRebornYinJiCached(dbMgr);
		}

		
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

		
		public static Dictionary<int, RebornStampData> UserRebornData = new Dictionary<int, RebornStampData>();
	}
}
