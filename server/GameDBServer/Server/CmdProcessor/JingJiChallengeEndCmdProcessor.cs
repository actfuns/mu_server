using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class JingJiChallengeEndCmdProcessor : ICmdProcessor
	{
		
		private JingJiChallengeEndCmdProcessor()
		{
		}

		
		public static JingJiChallengeEndCmdProcessor getInstance()
		{
			return JingJiChallengeEndCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			JingJiChallengeResultData data = DataHelper.BytesToObject<JingJiChallengeResultData>(cmdParams, 0, count);
			int ranking = JingJiChangManager.getInstance().onChallengeEnd(data);
			client.sendCmd<int>(10144, ranking);
		}

		
		private static JingJiChallengeEndCmdProcessor instance = new JingJiChallengeEndCmdProcessor();
	}
}
