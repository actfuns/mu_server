using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A1 RID: 2209
	public class JingJiJunxianLevelupCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D66 RID: 15718 RVA: 0x0034538B File Offset: 0x0034358B
		private JingJiJunxianLevelupCmdProcessor()
		{
		}

		// Token: 0x06003D67 RID: 15719 RVA: 0x00345398 File Offset: 0x00343598
		public static JingJiJunxianLevelupCmdProcessor getInstance()
		{
			return JingJiJunxianLevelupCmdProcessor.instance;
		}

		// Token: 0x06003D68 RID: 15720 RVA: 0x003453B0 File Offset: 0x003435B0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int result = JingJiChangManager.getInstance().upGradeJunXian(client);
			client.sendCmd<int>(586, result, false);
			return true;
		}

		// Token: 0x040047B1 RID: 18353
		private static JingJiJunxianLevelupCmdProcessor instance = new JingJiJunxianLevelupCmdProcessor();
	}
}
