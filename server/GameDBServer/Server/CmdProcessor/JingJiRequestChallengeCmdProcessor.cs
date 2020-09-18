using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
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

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] _cmdParams = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			int challengerId = _cmdParams[0];
			int beChallengerId = _cmdParams[1];
			int beChallengerRanking = _cmdParams[2];
			JingJiBeChallengeData result = JingJiChangManager.getInstance().requestChallenge(challengerId, beChallengerId, beChallengerRanking);
			client.sendCmd<JingJiBeChallengeData>(10143, result);
		}

		
		private static JingJiRequestChallengeCmdProcessor instance = new JingJiRequestChallengeCmdProcessor();
	}
}
