using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class UnregisterUserIDMgr
	{
		
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

		
		private static List<DelayUnRegisterUserIDItem> UnRegisterUserIDsList = new List<DelayUnRegisterUserIDItem>();
	}
}
