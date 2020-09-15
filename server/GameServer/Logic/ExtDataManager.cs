using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020001EE RID: 494
	public static class ExtDataManager
	{
		// Token: 0x0600063A RID: 1594 RVA: 0x00057614 File Offset: 0x00055814
		public static ExtData GetClientExtData(GameClient client)
		{
			ExtData result;
			if (client.extData != null)
			{
				result = client.extData;
			}
			else
			{
				int roleId = client.ClientData.RoleID;
				lock (ExtDataManager.Mutex)
				{
					ExtData extData;
					if (!ExtDataManager.ExtDataDict.TryGetValue(roleId, out extData))
					{
						extData = new ExtData();
						ExtDataManager.ExtDataDict[roleId] = extData;
					}
					client.extData = extData;
					result = extData;
				}
			}
			return result;
		}

		// Token: 0x04000AE8 RID: 2792
		private static object Mutex = new object();

		// Token: 0x04000AE9 RID: 2793
		private static Dictionary<int, ExtData> ExtDataDict = new Dictionary<int, ExtData>();
	}
}
