using System;
using System.Collections.Generic;
using GameDBServer.Core;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001DE RID: 478
	public class UserOnlineManager
	{
		// Token: 0x06000A03 RID: 2563 RVA: 0x0006088C File Offset: 0x0005EA8C
		public static int GetUserOnlineState(string userID)
		{
			lock (UserOnlineManager._RegUserIDDict)
			{
				UserOnlineManager.UserOnlineData data;
				if (UserOnlineManager._RegUserIDDict.TryGetValue(userID, out data) && TimeUtil.NOW() < data.MaxActiveTicks)
				{
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00060908 File Offset: 0x0005EB08
		public static bool RegisterUserID(string userID, int serverLineID, int state)
		{
			long nowTicks = TimeUtil.NOW();
			lock (UserOnlineManager._RegUserIDDict)
			{
				if (state <= 0)
				{
					UserOnlineManager.UserOnlineData data;
					if (UserOnlineManager._RegUserIDDict.TryGetValue(userID, out data))
					{
						if (data.ServerId == serverLineID)
						{
							UserOnlineManager._RegUserIDDict.Remove(userID);
						}
					}
				}
				else
				{
					UserOnlineManager.UserOnlineData data;
					if (!UserOnlineManager._RegUserIDDict.TryGetValue(userID, out data))
					{
						data = new UserOnlineManager.UserOnlineData();
						data.ServerId = serverLineID;
						data.UserId = userID;
						UserOnlineManager._RegUserIDDict[userID] = data;
					}
					if (nowTicks < data.MaxActiveTicks)
					{
						if (data.ServerId != serverLineID)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("账号 {0} 请求注册登录到 {1} 线，但是该账号已经被注册到 {2} 线", userID, serverLineID, data.ServerId), null, true);
							return false;
						}
					}
					else if (data.ServerId != serverLineID)
					{
						data.ServerId = serverLineID;
						LogManager.WriteLog(LogTypes.Error, string.Format("账号 {0} 注册登录到 {1} 线，该账号在{2} 线注册心跳已超时 ", userID, serverLineID, data.ServerId), null, true);
					}
					data.MaxActiveTicks = nowTicks + 600000L;
				}
			}
			return true;
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00060A88 File Offset: 0x0005EC88
		public static void ClearUserIDsByServerLineID(int serverLineID)
		{
			lock (UserOnlineManager._RegUserIDDict)
			{
				List<string> userIDsList = new List<string>();
				foreach (UserOnlineManager.UserOnlineData data in UserOnlineManager._RegUserIDDict.Values)
				{
					if (data.ServerId == serverLineID)
					{
						userIDsList.Add(data.UserId);
					}
				}
				for (int i = 0; i < userIDsList.Count; i++)
				{
					UserOnlineManager._RegUserIDDict.Remove(userIDsList[i]);
				}
			}
		}

		// Token: 0x04000C2C RID: 3116
		public const long MaxActiveTicks = 600000L;

		// Token: 0x04000C2D RID: 3117
		private static Dictionary<string, UserOnlineManager.UserOnlineData> _RegUserIDDict = new Dictionary<string, UserOnlineManager.UserOnlineData>();

		// Token: 0x020001DF RID: 479
		private class UserOnlineData
		{
			// Token: 0x04000C2E RID: 3118
			public string UserId;

			// Token: 0x04000C2F RID: 3119
			public int ServerId;

			// Token: 0x04000C30 RID: 3120
			public long MaxActiveTicks;
		}
	}
}
