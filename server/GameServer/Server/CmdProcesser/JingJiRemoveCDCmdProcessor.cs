using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A3 RID: 2211
	public class JingJiRemoveCDCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D6E RID: 15726 RVA: 0x00345437 File Offset: 0x00343637
		private JingJiRemoveCDCmdProcessor()
		{
		}

		// Token: 0x06003D6F RID: 15727 RVA: 0x00345444 File Offset: 0x00343644
		public static JingJiRemoveCDCmdProcessor getInstance()
		{
			return JingJiRemoveCDCmdProcessor.instance;
		}

		// Token: 0x06003D70 RID: 15728 RVA: 0x0034545C File Offset: 0x0034365C
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int result = JingJiChangManager.getInstance().removeCD(client);
			client.sendCmd<int>(584, result, false);
			return true;
		}

		// Token: 0x040047B3 RID: 18355
		private static JingJiRemoveCDCmdProcessor instance = new JingJiRemoveCDCmdProcessor();
	}
}
