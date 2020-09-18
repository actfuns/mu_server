using System;
using System.Collections.Generic;
using System.Reflection;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	
	public class CopyData
	{
		
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
