using System;
using System.Collections.Generic;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class BanChatManager
	{
		
		public static TCPOutPacket GetBanChatDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket tcpOutPacket = null;
			lock (BanChatManager._Mutex)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, long>>(BanChatManager._BanChatDict, pool, cmdID);
			}
			return tcpOutPacket;
		}

		
		public static void AddBanRoleName(string roleName, int banHours)
		{
			lock (BanChatManager._Mutex)
			{
				if (null == BanChatManager._BanChatDict)
				{
					BanChatManager._BanChatDict = new Dictionary<string, long>();
				}
				if (banHours > 0)
				{
					BanChatManager._BanChatDict[roleName] = DateTime.Now.Ticks / 10000L + (long)(banHours * 60 * 60 * 1000);
				}
				else
				{
					BanChatManager._BanChatDict.Remove(roleName);
				}
			}
		}

		
		private static object _Mutex = new object();

		
		private static Dictionary<string, long> _BanChatDict = new Dictionary<string, long>();
	}
}
