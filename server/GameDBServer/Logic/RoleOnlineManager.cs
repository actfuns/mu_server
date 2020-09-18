using System;
using System.Collections.Generic;

namespace GameDBServer.Logic
{
	
	public class RoleOnlineManager
	{
		
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

		
		public static void RemoveRoleOnlineTicks(int roleID)
		{
			lock (RoleOnlineManager._RoleOlineTicksDict)
			{
				RoleOnlineManager._RoleOlineTicksDict.Remove(roleID);
			}
		}

		
		private static Dictionary<int, long> _RoleOlineTicksDict = new Dictionary<int, long>();
	}
}
