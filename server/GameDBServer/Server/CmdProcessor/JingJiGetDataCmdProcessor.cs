using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001F1 RID: 497
	public class JingJiGetDataCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A5B RID: 2651 RVA: 0x00062135 File Offset: 0x00060335
		private JingJiGetDataCmdProcessor()
		{
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x00062140 File Offset: 0x00060340
		public static JingJiGetDataCmdProcessor getInstance()
		{
			return JingJiGetDataCmdProcessor.instance;
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x00062158 File Offset: 0x00060358
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			PlayerJingJiData data = JingJiChangManager.getInstance().getPlayerJingJiDataById(roleId);
			client.sendCmd<PlayerJingJiData>(10140, data);
		}

		// Token: 0x04000C53 RID: 3155
		private static JingJiGetDataCmdProcessor instance = new JingJiGetDataCmdProcessor();
	}
}
