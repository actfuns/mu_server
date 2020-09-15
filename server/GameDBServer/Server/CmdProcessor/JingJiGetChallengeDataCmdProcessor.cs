using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001F0 RID: 496
	public class JingJiGetChallengeDataCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A57 RID: 2647 RVA: 0x000620D4 File Offset: 0x000602D4
		private JingJiGetChallengeDataCmdProcessor()
		{
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x000620E0 File Offset: 0x000602E0
		public static JingJiGetChallengeDataCmdProcessor getInstance()
		{
			return JingJiGetChallengeDataCmdProcessor.instance;
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x000620F8 File Offset: 0x000602F8
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] challengeRankings = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			List<PlayerJingJiMiniData> miniDatas = JingJiChangManager.getInstance().getChallengeData(challengeRankings);
			client.sendCmd<List<PlayerJingJiMiniData>>(10141, miniDatas);
		}

		// Token: 0x04000C52 RID: 3154
		private static JingJiGetChallengeDataCmdProcessor instance = new JingJiGetChallengeDataCmdProcessor();
	}
}
