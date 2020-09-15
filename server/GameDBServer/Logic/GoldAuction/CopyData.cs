using System;
using System.Collections.Generic;
using System.Reflection;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	// Token: 0x02000135 RID: 309
	public class CopyData
	{
		// Token: 0x06000533 RID: 1331 RVA: 0x0002B6BC File Offset: 0x000298BC
		private static void Copy<T>(T sData, ref T rData)
		{
			try
			{
				foreach (FieldInfo info in rData.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					info.SetValue(rData, info.GetValue(sData));
				}
			}
			catch (Exception ex)
			{
				rData = default(T);
				LjlLog.WriteLog(LogTypes.Error, ex.ToString(), "");
			}
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0002B74C File Offset: 0x0002994C
		public static bool CopyAuctionDBItem(List<GoldAuctionDBItem> sData, ref List<GoldAuctionDBItem> rData)
		{
			try
			{
				foreach (GoldAuctionDBItem item in sData)
				{
					GoldAuctionDBItem tempItem = new GoldAuctionDBItem();
					CopyData.Copy<GoldAuctionDBItem>(item, ref tempItem);
					tempItem.BuyerData = new AuctionRoleData();
					CopyData.Copy<AuctionRoleData>(item.BuyerData, ref tempItem.BuyerData);
					if (tempItem != null)
					{
						tempItem.RoleList = new List<AuctionRoleData>();
						foreach (AuctionRoleData RoleData in item.RoleList)
						{
							AuctionRoleData tempRole = new AuctionRoleData();
							CopyData.Copy<AuctionRoleData>(RoleData, ref tempRole);
							tempItem.RoleList.Add(tempRole);
						}
						tempItem.OldAuctionType = tempItem.AuctionType;
						rData.Add(tempItem);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Error, ex.ToString(), "");
			}
			return false;
		}
	}
}
