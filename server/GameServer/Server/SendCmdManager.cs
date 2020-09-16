using System;
using GameServer.Logic;

namespace GameServer.Server
{
	
	public class SendCmdManager : IManager
	{
		
		private SendCmdManager()
		{
		}

		
		public static SendCmdManager getInstance()
		{
			return SendCmdManager.instance;
		}

		
		public void addSendCmdWrapper(SendCmdWrapper wrapper)
		{
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		private static SendCmdManager instance = new SendCmdManager();
	}
}
