using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.TCP;

namespace GameServer.Logic
{
	// Token: 0x02000626 RID: 1574
	public class DelayForceClosingMgr
	{
		// Token: 0x06002026 RID: 8230 RVA: 0x001BC0BC File Offset: 0x001BA2BC
		public static void AddDelaySocket(TMSKSocket socket)
		{
			long ticks = TimeUtil.NOW();
			lock (DelayForceClosingMgr._Socket2UDict)
			{
				if (!DelayForceClosingMgr._Socket2UDict.ContainsKey(socket))
				{
					DelayForceClosingMgr._Socket2UDict[socket] = ticks;
				}
			}
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x001BC124 File Offset: 0x001BA324
		public static void RemoveDelaySocket(TMSKSocket socket)
		{
			lock (DelayForceClosingMgr._Socket2UDict)
			{
				DelayForceClosingMgr._Socket2UDict.Remove(socket);
			}
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x001BC174 File Offset: 0x001BA374
		private static List<TMSKSocket> GetDelaySockets()
		{
			List<TMSKSocket> socketsList = new List<TMSKSocket>();
			long ticks = TimeUtil.NOW();
			lock (DelayForceClosingMgr._Socket2UDict)
			{
				foreach (TMSKSocket socket in DelayForceClosingMgr._Socket2UDict.Keys)
				{
					long lastTicks = DelayForceClosingMgr._Socket2UDict[socket];
					if (ticks - lastTicks >= 3000L)
					{
						socketsList.Add(socket);
					}
				}
			}
			return socketsList;
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x001BC244 File Offset: 0x001BA444
		public static void ProcessDelaySockets()
		{
			List<TMSKSocket> socketsList = DelayForceClosingMgr.GetDelaySockets();
			if (socketsList.Count > 0)
			{
				for (int i = 0; i < socketsList.Count; i++)
				{
					GameClient client = GameManager.ClientMgr.FindClient(socketsList[i]);
					if (null != client)
					{
						Global.ForceCloseClient(client, "", true);
					}
					else
					{
						Global.ForceCloseSocket(socketsList[i], "", true);
					}
				}
			}
		}

		// Token: 0x04002CDA RID: 11482
		private static Dictionary<TMSKSocket, long> _Socket2UDict = new Dictionary<TMSKSocket, long>();
	}
}
