using System;
using Server.Protocol;
using Server.TCP;

namespace GameServer.Server
{
	
	public class SendCmdWrapper
	{
		
		public SendCmdWrapper()
		{
			SendCmdWrapper.IncInstanceCount();
		}

		
		public void Release()
		{
			SendCmdWrapper.DecInstanceCount();
		}

		
		public static void IncInstanceCount()
		{
			lock (SendCmdWrapper.CountLock)
			{
				SendCmdWrapper.TotalInstanceCount++;
			}
		}

		
		public static void DecInstanceCount()
		{
			lock (SendCmdWrapper.CountLock)
			{
				SendCmdWrapper.TotalInstanceCount--;
			}
		}

		
		public static int GetInstanceCount()
		{
			int count = 0;
			lock (SendCmdWrapper.CountLock)
			{
				count = SendCmdWrapper.TotalInstanceCount;
			}
			return count;
		}

		
		public TMSKSocket socket = null;

		
		public TCPOutPacket tcpOutPacket = null;

		
		private static object CountLock = new object();

		
		private static int TotalInstanceCount = 0;
	}
}
