using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class BanChatManager
	{
		
		public static void GetBanChatDictFromDBServer()
		{
			long ticks = TimeUtil.NOW();
			if (ticks - BanChatManager.LastGetBanChatDictTicks >= 30000L)
			{
				BanChatManager.LastGetBanChatDictTicks = ticks;
				byte[] bytesData = null;
				if (TCPProcessCmdResults.RESULT_FAILED != Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10028, string.Format("{0}", GameManager.ServerLineID), out bytesData, 0))
				{
					if (bytesData != null && bytesData.Length > 6)
					{
						int length = BitConverter.ToInt32(bytesData, 0);
						Dictionary<string, long> banChatDict = DataHelper.BytesToObject<Dictionary<string, long>>(bytesData, 6, length - 2);
						lock (BanChatManager._Mutex)
						{
							BanChatManager._BanChatDict = banChatDict;
						}
					}
				}
			}
		}

		
		public static void AddBanRoleName(string roleName, int banHours)
		{
			lock (BanChatManager._Mutex)
			{
				if (null == BanChatManager._BanChatDict)
				{
					BanChatManager._BanChatDict = new Dictionary<string, long>();
				}
				BanChatManager._BanChatDict[roleName] = TimeUtil.NOW() + (long)(banHours * 60 * 60 * 1000);
			}
		}

		
		public static bool IsBanRoleName(string roleName, long roleId)
		{
			lock (BanChatManager._Mutex)
			{
				if (null == BanChatManager._BanChatDict)
				{
					return false;
				}
				long banTicks = 0L;
				BanChatManager._BanChatDict.TryGetValue(roleName, out banTicks);
				long nowTicks = TimeUtil.NOW();
				if (nowTicks < banTicks)
				{
					return true;
				}
				BanChatManager._BanChatDict.TryGetValue(roleId + "$rid", out banTicks);
				if (nowTicks < banTicks)
				{
					return true;
				}
			}
			return false;
		}

		
		private static object _Mutex = new object();

		
		private static Dictionary<string, long> _BanChatDict = null;

		
		private static long LastGetBanChatDictTicks = 0L;
	}
}
