using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x0200089F RID: 2207
	public class JingJiGetBuffCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D5E RID: 15710 RVA: 0x0034519B File Offset: 0x0034339B
		private JingJiGetBuffCmdProcessor()
		{
		}

		// Token: 0x06003D5F RID: 15711 RVA: 0x003451A8 File Offset: 0x003433A8
		public static JingJiGetBuffCmdProcessor getInstance()
		{
			return JingJiGetBuffCmdProcessor.instance;
		}

		// Token: 0x06003D60 RID: 15712 RVA: 0x003451C0 File Offset: 0x003433C0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int _replace = Convert.ToInt32(cmdParams[1]);
			int result;
			if (_replace != 0 && _replace != 1)
			{
				result = ResultCode.Illegal;
			}
			else
			{
				result = JingJiChangManager.getInstance().activeJunXianBuff(client, _replace == 1);
			}
			client.sendCmd<int>(585, result, false);
			return true;
		}

		// Token: 0x040047AF RID: 18351
		private static JingJiGetBuffCmdProcessor instance = new JingJiGetBuffCmdProcessor();
	}
}
