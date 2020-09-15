using System;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001F7 RID: 503
	public class JingJiUpdateNextRewardTimeCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A73 RID: 2675 RVA: 0x000623B3 File Offset: 0x000605B3
		private JingJiUpdateNextRewardTimeCmdProcessor()
		{
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x000623C0 File Offset: 0x000605C0
		public static JingJiUpdateNextRewardTimeCmdProcessor getInstance()
		{
			return JingJiUpdateNextRewardTimeCmdProcessor.instance;
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x000623D8 File Offset: 0x000605D8
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			long[] _cmdParams = DataHelper.BytesToObject<long[]>(cmdParams, 0, count);
			int roleId = (int)_cmdParams[0];
			long nextRewardTime = _cmdParams[1];
			bool result = JingJiChangManager.getInstance().updateNextRewardTime(roleId, nextRewardTime);
			client.sendCmd<int>(10149, result ? 1 : 0);
		}

		// Token: 0x04000C59 RID: 3161
		private static JingJiUpdateNextRewardTimeCmdProcessor instance = new JingJiUpdateNextRewardTimeCmdProcessor();
	}
}
