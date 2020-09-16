using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace GameServer.Server
{
	
	public class CacheManager
	{
		
		private static RoleMiniInfo GetRoleMiniInfo(long rid, int serverId)
		{
			RoleMiniInfo roleMiniInfo;
			lock (CacheManager.roleMiniInfoDict)
			{
				if (CacheManager.roleMiniInfoDict.TryGetValue(rid, out roleMiniInfo))
				{
					return roleMiniInfo;
				}
			}
			roleMiniInfo = Global.sendToDB<RoleMiniInfo, long>(10220, rid, serverId);
			if (roleMiniInfo != null && roleMiniInfo.roleId == rid)
			{
				lock (CacheManager.roleMiniInfoDict)
				{
					CacheManager.roleMiniInfoDict[rid] = roleMiniInfo;
				}
			}
			return roleMiniInfo;
		}

		
		public static void OnInitGame(GameClient client)
		{
			lock (CacheManager.roleMiniInfoDict)
			{
				RoleMiniInfo roleMiniInfo;
				if (!CacheManager.roleMiniInfoDict.TryGetValue((long)client.ClientData.RoleID, out roleMiniInfo))
				{
					roleMiniInfo = new RoleMiniInfo
					{
						roleId = (long)client.ClientData.RoleID,
						zoneId = client.ClientData.ZoneID,
						userId = client.strUserID
					};
				}
			}
		}

		
		public static int GetZoneIdByRoleId(long rid, int serverId)
		{
			RoleMiniInfo roleMiniInfo = CacheManager.GetRoleMiniInfo(rid, serverId);
			int result;
			if (null != roleMiniInfo)
			{
				result = roleMiniInfo.zoneId;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public static string GetUserIdByRoleId(int rid, int serverId)
		{
			RoleMiniInfo roleMiniInfo = CacheManager.GetRoleMiniInfo((long)rid, serverId);
			string result;
			if (null != roleMiniInfo)
			{
				result = roleMiniInfo.userId;
			}
			else
			{
				result = "";
			}
			return result;
		}

		
		private static Dictionary<long, RoleMiniInfo> roleMiniInfoDict = new Dictionary<long, RoleMiniInfo>();
	}
}
