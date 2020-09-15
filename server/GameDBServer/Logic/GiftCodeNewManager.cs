using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000131 RID: 305
	internal class GiftCodeNewManager
	{
		// Token: 0x06000525 RID: 1317 RVA: 0x0002A874 File Offset: 0x00028A74
		public static void ScanLastGroup(DBManager dbMgr)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			if (nowTicks - GiftCodeNewManager.LastScanTicks >= 10000L)
			{
				GiftCodeNewManager.LastScanTicks = nowTicks;
				List<LineItem> itemList = LineManager.GetLineItemList();
				if (itemList != null && itemList.Count != 0)
				{
					bool bExistLocalServer = false;
					for (int i = 0; i < itemList.Count; i++)
					{
						if (itemList[i].LineID > 0 && (itemList[i].LineID < 9000 || itemList[i].LineID == GameDBManager.ZoneID))
						{
							bExistLocalServer = true;
							break;
						}
					}
					if (bExistLocalServer)
					{
						List<GiftCodeAwardData> groupList = DBQuery.ScanNewGiftCodeFromTable(dbMgr);
						if (groupList != null && groupList.Count != 0)
						{
							string nowtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
							List<string> GiftData = new List<string>();
							foreach (GiftCodeAwardData item in groupList)
							{
								if (item.RoleID > 0 && !string.IsNullOrEmpty(item.UserId) && !string.IsNullOrEmpty(item.GiftId) && !string.IsNullOrEmpty(item.CodeNo))
								{
									bool isSucc = DBWriter.UpdateGiftCodeState(dbMgr, item.Dbid, 1, nowtime);
									if (isSucc)
									{
										string szCmd = string.Format("{0},{1},{2},{3}", new object[]
										{
											item.UserId,
											item.RoleID,
											item.GiftId,
											item.CodeNo
										});
										GiftData.Add(szCmd);
									}
								}
								else
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("[GiftCodeNew]数据表t_giftcode相关配置DBID:{0},RoleId:{1},UserId:{2}错误!", item.Dbid, item.RoleID, item.UserId), null, true);
								}
							}
							if (GiftData.Count > 0)
							{
								string szCmds = string.Join("#", GiftData);
								string gmCmdData = string.Format("-giftcodecmd {0}", szCmds);
								ChatMsgManager.AddGMCmdChatMsgToOneClient(gmCmdData);
							}
							groupList.Clear();
							GiftData.Clear();
						}
					}
				}
			}
		}

		// Token: 0x040007D8 RID: 2008
		private static long LastScanTicks = DateTime.Now.Ticks / 10000L;
	}
}
