using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001EE RID: 494
	public class JingJiChallengeEndCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A4F RID: 2639 RVA: 0x00061FD5 File Offset: 0x000601D5
		private JingJiChallengeEndCmdProcessor()
		{
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x00061FE0 File Offset: 0x000601E0
		public static JingJiChallengeEndCmdProcessor getInstance()
		{
			return JingJiChallengeEndCmdProcessor.instance;
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x00061FF8 File Offset: 0x000601F8
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			JingJiChallengeResultData data = DataHelper.BytesToObject<JingJiChallengeResultData>(cmdParams, 0, count);
			int ranking = JingJiChangManager.getInstance().onChallengeEnd(data);
			client.sendCmd<int>(10144, ranking);
		}

		// Token: 0x04000C50 RID: 3152
		private static JingJiChallengeEndCmdProcessor instance = new JingJiChallengeEndCmdProcessor();
	}
}
