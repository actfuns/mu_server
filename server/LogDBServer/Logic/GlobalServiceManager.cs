using System;
using LogDBServer.Server.CmdProcessor;

namespace LogDBServer.Logic
{
	
	public class GlobalServiceManager
	{
		
		public static void initialize()
		{
			CmdRegisterTriggerManager.getInstance().initialize();
		}

		
		public static void startup()
		{
			CmdRegisterTriggerManager.getInstance().startup();
		}

		
		public static void showdown()
		{
			CmdRegisterTriggerManager.getInstance().showdown();
		}

		
		public static void destroy()
		{
			CmdRegisterTriggerManager.getInstance().destroy();
		}
	}
}
