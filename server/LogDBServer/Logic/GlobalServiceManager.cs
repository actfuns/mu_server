using System;
using LogDBServer.Server.CmdProcessor;

namespace LogDBServer.Logic
{
	// Token: 0x02000014 RID: 20
	public class GlobalServiceManager
	{
		// Token: 0x0600004B RID: 75 RVA: 0x000038B0 File Offset: 0x00001AB0
		public static void initialize()
		{
			CmdRegisterTriggerManager.getInstance().initialize();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000038BE File Offset: 0x00001ABE
		public static void startup()
		{
			CmdRegisterTriggerManager.getInstance().startup();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000038CC File Offset: 0x00001ACC
		public static void showdown()
		{
			CmdRegisterTriggerManager.getInstance().showdown();
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000038DA File Offset: 0x00001ADA
		public static void destroy()
		{
			CmdRegisterTriggerManager.getInstance().destroy();
		}
	}
}
