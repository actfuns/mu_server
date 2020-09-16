using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiRequestChallengeCmdProcessor : ICmdProcessor
	{
		
		private JingJiRequestChallengeCmdProcessor()
		{
		}

		
		public static JingJiRequestChallengeCmdProcessor getInstance()
		{
			return JingJiRequestChallengeCmdProcessor.instance;
		}

		
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

		
		private static JingJiRequestChallengeCmdProcessor instance = new JingJiRequestChallengeCmdProcessor();
	}
}
