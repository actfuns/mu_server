using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class JingJiGetChallengeDataCmdProcessor : ICmdProcessor
	{
		
		private JingJiGetChallengeDataCmdProcessor()
		{
		}

		
		public static JingJiGetChallengeDataCmdProcessor getInstance()
		{
			return JingJiGetChallengeDataCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] challengeRankings = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			List<PlayerJingJiMiniData> miniDatas = JingJiChangManager.getInstance().getChallengeData(challengeRankings);
			client.sendCmd<List<PlayerJingJiMiniData>>(10141, miniDatas);
		}

		
		private static JingJiGetChallengeDataCmdProcessor instance = new JingJiGetChallengeDataCmdProcessor();
	}
}
