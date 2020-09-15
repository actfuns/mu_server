using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;
using Server.Data;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x0200089E RID: 2206
	public class JingJiDetailCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D5A RID: 15706 RVA: 0x00345133 File Offset: 0x00343333
		private JingJiDetailCmdProcessor()
		{
		}

		// Token: 0x06003D5B RID: 15707 RVA: 0x00345140 File Offset: 0x00343340
		public static JingJiDetailCmdProcessor getInstance()
		{
			return JingJiDetailCmdProcessor.instance;
		}

		// Token: 0x06003D5C RID: 15708 RVA: 0x00345158 File Offset: 0x00343358
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int type = Convert.ToInt32(cmdParams[1]);
			JingJiDetailData data = JingJiChangManager.getInstance().getDetailData(client, type);
			client.sendCmd<JingJiDetailData>(578, data, false);
			return true;
		}

		// Token: 0x040047AE RID: 18350
		private static JingJiDetailCmdProcessor instance = new JingJiDetailCmdProcessor();
	}
}
