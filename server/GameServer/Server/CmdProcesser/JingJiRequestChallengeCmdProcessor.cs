using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A4 RID: 2212
	public class JingJiRequestChallengeCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D72 RID: 15730 RVA: 0x00345495 File Offset: 0x00343695
		private JingJiRequestChallengeCmdProcessor()
		{
		}

		// Token: 0x06003D73 RID: 15731 RVA: 0x003454A0 File Offset: 0x003436A0
		public static JingJiRequestChallengeCmdProcessor getInstance()
		{
			return JingJiRequestChallengeCmdProcessor.instance;
		}

		// Token: 0x06003D74 RID: 15732 RVA: 0x003454B8 File Offset: 0x003436B8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int roleId = Convert.ToInt32(cmdParams[0]);
			int beChallengerId = Convert.ToInt32(cmdParams[1]);
			int beChallengeRanking = Convert.ToInt32(cmdParams[2]);
			int enterType = Convert.ToInt32(cmdParams[3]);
			int result = 0;
			bool result2;
			if (beChallengerId < 0 || beChallengeRanking < 1 || beChallengeRanking > JingJiChangConstants.RankingListMaxNum || (enterType != JingJiChangConstants.Enter_Type_Free && enterType != JingJiChangConstants.Enter_Type_Vip))
			{
				client.sendCmd<int>(579, result, false);
				result2 = true;
			}
			else
			{
				result = JingJiChangManager.getInstance().requestChallenge(client, beChallengerId, beChallengeRanking, enterType);
				client.sendCmd<int>(579, result, false);
				result2 = true;
			}
			return result2;
		}

		// Token: 0x040047B4 RID: 18356
		private static JingJiRequestChallengeCmdProcessor instance = new JingJiRequestChallengeCmdProcessor();
	}
}
