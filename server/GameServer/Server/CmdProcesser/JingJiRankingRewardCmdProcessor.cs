using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiRankingRewardCmdProcessor : ICmdProcessor
	{
		
		private JingJiRankingRewardCmdProcessor()
		{
		}

		
		public static JingJiRankingRewardCmdProcessor getInstance()
		{
			return JingJiRankingRewardCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int result;
			long nextRewardTime;
			JingJiChangManager.getInstance().rankingReward(client, out result, out nextRewardTime);
			client.sendCmd<long[]>(583, new long[]
			{
				(long)result,
				nextRewardTime
			}, false);
			return true;
		}

		
		private static JingJiRankingRewardCmdProcessor instance = new JingJiRankingRewardCmdProcessor();
	}
}
