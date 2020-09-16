using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.TCP;

namespace GameServer.Logic
{
	
	public class DelayForceClosingMgr
	{
		
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

		
		public static void RemoveDelaySocket(TMSKSocket socket)
		{
			lock (DelayForceClosingMgr._Socket2UDict)
			{
				DelayForceClosingMgr._Socket2UDict.Remove(socket);
			}
		}

		
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

		
		private static Dictionary<TMSKSocket, long> _Socket2UDict = new Dictionary<TMSKSocket, long>();
	}
}
