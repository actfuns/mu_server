using System;
using System.Collections.Generic;

namespace GameDBServer.Logic
{
	// Token: 0x020001D6 RID: 470
	public class RoleOnlineManager
	{
		// Token: 0x060009E0 RID: 2528 RVA: 0x0005EF1C File Offset: 0x0005D11C
		public static long GetRoleOnlineTicks(int roleID)
		{
			long ticks = 0L;
			lock (RoleOnlineManager._RoleOlineTicksDict)
			{
				if (!RoleOnlineManager._RoleOlineTicksDict.TryGetValue(roleID, out ticks))
				{
					ticks = 0L;
				}
			}
			return ticks;
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x0005EF84 File Offset: 0x0005D184
		public static void UpdateRoleOnlineTicks(int roleID)
		{
			long ticks = DateTime.Now.Ticks / 10000L;
			lock (RoleOnlineManager._RoleOlineTicksDict)
			{
				if (RoleOnlineManager._RoleOlineTicksDict.ContainsKey(roleID))
				{
					RoleOnlineManager._RoleOlineTicksDict[roleID] = ticks;
				}
				else
				{
					RoleOnlineManager._RoleOlineTicksDict.Add(roleID, ticks);
				}
			}
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x0005F014 File Offset: 0x0005D214
		public static void RemoveRoleOnlineTicks(int roleID)
		{
			lock (RoleOnlineManager._RoleOlineTicksDict)
			{
				RoleOnlineManager._RoleOlineTicksDict.Remove(roleID);
			}
		}

		// Token: 0x04000C04 RID: 3076
		private static Dictionary<int, long> _RoleOlineTicksDict = new Dictionary<int, long>();
	}
}
