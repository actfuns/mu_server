using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	
	public class SaleRoleManager
	{
		
		public static void AddSaleRoleItem(GameClient client)
		{
			lock (SaleRoleManager._SaleRoleDict)
			{
				SaleRoleManager._SaleRoleDict[client.ClientData.RoleID] = client;
				SaleRoleManager._SaleRoleDataList = null;
			}
		}

		
		public static GameClient RemoveSaleRoleItem(int roleID)
		{
			GameClient result;
			lock (SaleRoleManager._SaleRoleDict)
			{
				GameClient client = null;
				if (SaleRoleManager._SaleRoleDict.TryGetValue(roleID, out client))
				{
					SaleRoleManager._SaleRoleDict.Remove(roleID);
				}
				SaleRoleManager._SaleRoleDataList = null;
				result = client;
			}
			return result;
		}

		
		public static List<SaleRoleData> GetSaleRoleDataList()
		{
			long ticks = TimeUtil.NOW();
			List<SaleRoleData> result;
			lock (SaleRoleManager._SaleRoleDict)
			{
				List<SaleRoleData> saleRoleDataList = new List<SaleRoleData>();
				foreach (GameClient client in SaleRoleManager._SaleRoleDict.Values)
				{
					int saleGoodsNum = (client.ClientData.SaleGoodsDataList == null) ? 0 : client.ClientData.SaleGoodsDataList.Count;
					if (saleGoodsNum > 0)
					{
						saleRoleDataList.Add(new SaleRoleData
						{
							RoleID = client.ClientData.RoleID,
							RoleName = Global.FormatRoleName(client, client.ClientData.RoleName),
							RoleLevel = client.ClientData.Level,
							SaleGoodsNum = saleGoodsNum
						});
						if (saleRoleDataList.Count >= 250)
						{
							break;
						}
					}
				}
				SaleRoleManager._SaleRoleDataList = saleRoleDataList;
				SaleRoleManager._SaleRoleDataListTicks = ticks;
				result = saleRoleDataList;
			}
			return result;
		}

		
		private static List<SaleRoleData> _SaleRoleDataList = null;

		
		private static long _SaleRoleDataListTicks = 0L;

		
		private static Dictionary<int, GameClient> _SaleRoleDict = new Dictionary<int, GameClient>();
	}
}
