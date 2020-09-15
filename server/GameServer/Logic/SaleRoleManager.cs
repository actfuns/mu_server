using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020007CC RID: 1996
	public class SaleRoleManager
	{
		// Token: 0x06003838 RID: 14392 RVA: 0x002FADDC File Offset: 0x002F8FDC
		public static void AddSaleRoleItem(GameClient client)
		{
			lock (SaleRoleManager._SaleRoleDict)
			{
				SaleRoleManager._SaleRoleDict[client.ClientData.RoleID] = client;
				SaleRoleManager._SaleRoleDataList = null;
			}
		}

		// Token: 0x06003839 RID: 14393 RVA: 0x002FAE40 File Offset: 0x002F9040
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

		// Token: 0x0600383A RID: 14394 RVA: 0x002FAEB4 File Offset: 0x002F90B4
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

		// Token: 0x04004149 RID: 16713
		private static List<SaleRoleData> _SaleRoleDataList = null;

		// Token: 0x0400414A RID: 16714
		private static long _SaleRoleDataListTicks = 0L;

		// Token: 0x0400414B RID: 16715
		private static Dictionary<int, GameClient> _SaleRoleDict = new Dictionary<int, GameClient>();
	}
}
