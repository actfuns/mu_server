using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020007E6 RID: 2022
	public class UnregisterUserIDMgr
	{
		// Token: 0x06003936 RID: 14646 RVA: 0x00309BF4 File Offset: 0x00307DF4
		public static void AddUnRegisterUserID(string userID, int serverId)
		{
			lock (UnregisterUserIDMgr.UnRegisterUserIDsList)
			{
				UnregisterUserIDMgr.UnRegisterUserIDsList.Add(new DelayUnRegisterUserIDItem
				{
					UserID = userID,
					StartTicks = TimeUtil.NOW(),
					ServerId = serverId
				});
			}
		}

		// Token: 0x06003937 RID: 14647 RVA: 0x00309C68 File Offset: 0x00307E68
		public static void ProcessUnRegisterUserIDsQueue()
		{
			long nowTicks = TimeUtil.NOW();
			DelayUnRegisterUserIDItem item = null;
			lock (UnregisterUserIDMgr.UnRegisterUserIDsList)
			{
				if (UnregisterUserIDMgr.UnRegisterUserIDsList.Count > 0)
				{
					if (nowTicks - UnregisterUserIDMgr.UnRegisterUserIDsList[0].StartTicks >= 30000L)
					{
						item = UnregisterUserIDMgr.UnRegisterUserIDsList[0];
						UnregisterUserIDMgr.UnRegisterUserIDsList.RemoveAt(0);
					}
				}
			}
			if (null != item)
			{
				GameManager.DBCmdMgr.AddDBCmd(10025, string.Format("{0}:{1}:{2}", item.UserID, GameManager.ServerLineID, 0), null, item.ServerId);
			}
		}

		// Token: 0x04004328 RID: 17192
		private static List<DelayUnRegisterUserIDItem> UnRegisterUserIDsList = new List<DelayUnRegisterUserIDItem>();
	}
}
