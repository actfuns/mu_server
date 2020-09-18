using System;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class JingJiGetRankingAndRewardTimeCmdProcessor : ICmdProcessor
	{
		
		private JingJiGetRankingAndRewardTimeCmdProcessor()
		{
		}

		
		public static JingJiGetRankingAndRewardTimeCmdProcessor getInstance()
		{
			return JingJiGetRankingAndRewardTimeCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			int ranking = -2;
			long nextRewardTime = 0L;
			JingJiChangManager.getInstance().getRankingAndNextRewardTimeById(roleId, out ranking, out nextRewardTime);
			long[] resultParams = new long[]
			{
				(long)ranking,
				nextRewardTime
			};
			client.sendCmd<long[]>(10148, resultParams);
		}

		
		private static JingJiGetRankingAndRewardTimeCmdProcessor instance = new JingJiGetRankingAndRewardTimeCmdProcessor();
	}
}
