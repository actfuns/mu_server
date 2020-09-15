using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace GameServer.Server
{
	// Token: 0x02000898 RID: 2200
	public class CacheManager
	{
		// Token: 0x06003D3B RID: 15675 RVA: 0x00344898 File Offset: 0x00342A98
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

		// Token: 0x06003D3C RID: 15676 RVA: 0x0034496C File Offset: 0x00342B6C
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

		// Token: 0x06003D3D RID: 15677 RVA: 0x00344A0C File Offset: 0x00342C0C
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

		// Token: 0x06003D3E RID: 15678 RVA: 0x00344A3C File Offset: 0x00342C3C
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

		// Token: 0x040047A7 RID: 18343
		private static Dictionary<long, RoleMiniInfo> roleMiniInfoDict = new Dictionary<long, RoleMiniInfo>();
	}
}
