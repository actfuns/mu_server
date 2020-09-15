using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A2 RID: 2210
	public class JingJiLeaveFuBenCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D6A RID: 15722 RVA: 0x003453E9 File Offset: 0x003435E9
		private JingJiLeaveFuBenCmdProcessor()
		{
		}

		// Token: 0x06003D6B RID: 15723 RVA: 0x003453F4 File Offset: 0x003435F4
		public static JingJiLeaveFuBenCmdProcessor getInstance()
		{
			return JingJiLeaveFuBenCmdProcessor.instance;
		}

		// Token: 0x06003D6C RID: 15724 RVA: 0x0034540C File Offset: 0x0034360C
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			JingJiChangManager.getInstance().onLeaveFuBenForStopCD(client);
			return true;
		}

		// Token: 0x040047B2 RID: 18354
		private static JingJiLeaveFuBenCmdProcessor instance = new JingJiLeaveFuBenCmdProcessor();
	}
}
