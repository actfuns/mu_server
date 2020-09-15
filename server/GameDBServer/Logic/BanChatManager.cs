using System;
using System.Collections.Generic;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001A2 RID: 418
	public class BanChatManager
	{
		// Token: 0x060008D7 RID: 2263 RVA: 0x000529BC File Offset: 0x00050BBC
		public static TCPOutPacket GetBanChatDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket tcpOutPacket = null;
			lock (BanChatManager._Mutex)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, long>>(BanChatManager._BanChatDict, pool, cmdID);
			}
			return tcpOutPacket;
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x00052A18 File Offset: 0x00050C18
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

		// Token: 0x0400099E RID: 2462
		private static object _Mutex = new object();

		// Token: 0x0400099F RID: 2463
		private static Dictionary<string, long> _BanChatDict = new Dictionary<string, long>();
	}
}
