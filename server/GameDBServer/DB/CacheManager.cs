using System;
using System.Collections.Generic;

namespace GameDBServer.DB
{
	
	public static class CacheManager
	{
		
		public static RoleMiniInfo GetRoleMiniInfo(long rid)
		{
			RoleMiniInfo roleMiniInfo;
			lock (CacheManager.roleMiniInfoDict)
			{
				if (CacheManager.roleMiniInfoDict.TryGetValue(rid, out roleMiniInfo))
				{
					return roleMiniInfo;
				}
			}
			roleMiniInfo = DBQuery.QueryRoleMiniInfo(rid);
			if (null != roleMiniInfo)
			{
				lock (CacheManager.roleMiniInfoDict)
				{
					CacheManager.roleMiniInfoDict[rid] = roleMiniInfo;
				}
			}
			return roleMiniInfo;
		}

		
		public static void AddRoleMiniInfo(long rid, int zoneId, string userId)
		{
			lock (CacheManager.roleMiniInfoDict)
			{
				RoleMiniInfo roleMiniInfo;
				if (!CacheManager.roleMiniInfoDict.TryGetValue(rid, out roleMiniInfo))
				{
					roleMiniInfo = new RoleMiniInfo
					{
						roleId = rid,
						zoneId = zoneId,
						userId = userId
					};
				}
			}
		}

		
		private static Dictionary<long, RoleMiniInfo> roleMiniInfoDict = new Dictionary<long, RoleMiniInfo>();
	}
}
