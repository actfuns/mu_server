using System;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001F4 RID: 500
	public class JingJiRemoveCDCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A67 RID: 2663 RVA: 0x0006227E File Offset: 0x0006047E
		private JingJiRemoveCDCmdProcessor()
		{
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x0006228C File Offset: 0x0006048C
		public static JingJiRemoveCDCmdProcessor getInstance()
		{
			return JingJiRemoveCDCmdProcessor.instance;
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x000622A4 File Offset: 0x000604A4
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			bool result = JingJiChangManager.getInstance().removeCD(roleId);
			client.sendCmd<bool>(10147, result);
		}

		// Token: 0x04000C56 RID: 3158
		private static JingJiRemoveCDCmdProcessor instance = new JingJiRemoveCDCmdProcessor();
	}
}
