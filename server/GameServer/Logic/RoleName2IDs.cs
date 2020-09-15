using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020007C6 RID: 1990
	internal class RoleName2IDs
	{
		// Token: 0x060034A9 RID: 13481 RVA: 0x002EB680 File Offset: 0x002E9880
		public static void AddRoleName(string roleName, int roleID)
		{
			lock (RoleName2IDs._S2UDict)
			{
				RoleName2IDs._S2UDict[roleName] = roleID;
			}
		}

		// Token: 0x060034AA RID: 13482 RVA: 0x002EB6D4 File Offset: 0x002E98D4
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

		// Token: 0x060034AB RID: 13483 RVA: 0x002EB738 File Offset: 0x002E9938
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

		// Token: 0x060034AC RID: 13484 RVA: 0x002EB7F0 File Offset: 0x002E99F0
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

		// Token: 0x04003FC1 RID: 16321
		private static Dictionary<string, int> _S2UDict = new Dictionary<string, int>(1000);
	}
}
