using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001F5 RID: 501
	public class JingJiRequestChallengeCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A6B RID: 2667 RVA: 0x000622E1 File Offset: 0x000604E1
		private JingJiRequestChallengeCmdProcessor()
		{
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x000622EC File Offset: 0x000604EC
		public static JingJiRequestChallengeCmdProcessor getInstance()
		{
			return JingJiRequestChallengeCmdProcessor.instance;
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00062304 File Offset: 0x00060504
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] _cmdParams = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			int challengerId = _cmdParams[0];
			int beChallengerId = _cmdParams[1];
			int beChallengerRanking = _cmdParams[2];
			JingJiBeChallengeData result = JingJiChangManager.getInstance().requestChallenge(challengerId, beChallengerId, beChallengerRanking);
			client.sendCmd<JingJiBeChallengeData>(10143, result);
		}

		// Token: 0x04000C57 RID: 3159
		private static JingJiRequestChallengeCmdProcessor instance = new JingJiRequestChallengeCmdProcessor();
	}
}
