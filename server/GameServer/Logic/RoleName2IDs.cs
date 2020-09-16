using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	internal class RoleName2IDs
	{
		
		public static void AddRoleName(string roleName, int roleID)
		{
			lock (RoleName2IDs._S2UDict)
			{
				RoleName2IDs._S2UDict[roleName] = roleID;
			}
		}

		
		public static void RemoveRoleName(string roleName)
		{
			lock (RoleName2IDs._S2UDict)
			{
				if (RoleName2IDs._S2UDict.ContainsKey(roleName))
				{
					RoleName2IDs._S2UDict.Remove(roleName);
				}
			}
		}

		
		public static int FindRoleIDByName(string roleName, bool queryFromDB = false)
		{
			int roleID = -1;
			lock (RoleName2IDs._S2UDict)
			{
				if (!RoleName2IDs._S2UDict.TryGetValue(roleName, out roleID))
				{
					roleID = -1;
				}
			}
			if (roleID < 0 && queryFromDB)
			{
				string[] dbFields = Global.ExecuteDBCmd(195, string.Format("{0}:{1}:0", 0, roleName), 0);
				if (dbFields != null || dbFields.Length >= 5)
				{
					roleID = Global.SafeConvertToInt32(dbFields[3]);
				}
			}
			return roleID;
		}

		
		public static void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (RoleName2IDs._S2UDict)
				{
					if (RoleName2IDs._S2UDict.ContainsKey(oldName))
					{
						RoleName2IDs._S2UDict.Remove(oldName);
						RoleName2IDs._S2UDict.Add(newName, roleId);
					}
				}
			}
		}

		
		private static Dictionary<string, int> _S2UDict = new Dictionary<string, int>(1000);
	}
}
