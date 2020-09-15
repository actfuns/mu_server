using System;
using GameServer.Logic;

namespace GameServer.Server
{
	// Token: 0x020008B5 RID: 2229
	public class SendCmdManager : IManager
	{
		// Token: 0x06003DC0 RID: 15808 RVA: 0x0034C316 File Offset: 0x0034A516
		private SendCmdManager()
		{
		}

		// Token: 0x06003DC1 RID: 15809 RVA: 0x0034C324 File Offset: 0x0034A524
		public static SendCmdManager getInstance()
		{
			return SendCmdManager.instance;
		}

		// Token: 0x06003DC2 RID: 15810 RVA: 0x0034C33B File Offset: 0x0034A53B
		public void addSendCmdWrapper(SendCmdWrapper wrapper)
		{
		}

		// Token: 0x06003DC3 RID: 15811 RVA: 0x0034C340 File Offset: 0x0034A540
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06003DC4 RID: 15812 RVA: 0x0034C354 File Offset: 0x0034A554
		public bool startup()
		{
			return true;
		}

		// Token: 0x06003DC5 RID: 15813 RVA: 0x0034C368 File Offset: 0x0034A568
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06003DC6 RID: 15814 RVA: 0x0034C37C File Offset: 0x0034A57C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x040047D7 RID: 18391
		private static SendCmdManager instance = new SendCmdManager();
	}
}
