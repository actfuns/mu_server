using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public static class ExtDataManager
	{
		
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

		
		private static object Mutex = new object();

		
		private static Dictionary<int, ExtData> ExtDataDict = new Dictionary<int, ExtData>();
	}
}
