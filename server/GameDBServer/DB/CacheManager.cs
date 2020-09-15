using System;
using System.Collections.Generic;

namespace GameDBServer.DB
{
	// Token: 0x020001E8 RID: 488
	public static class CacheManager
	{
		// Token: 0x06000A31 RID: 2609 RVA: 0x00061600 File Offset: 0x0005F800
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

		// Token: 0x06000A32 RID: 2610 RVA: 0x000616C0 File Offset: 0x0005F8C0
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

		// Token: 0x04000C49 RID: 3145
		private static Dictionary<long, RoleMiniInfo> roleMiniInfoDict = new Dictionary<long, RoleMiniInfo>();
	}
}
