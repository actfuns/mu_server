using System;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class JingJiUpdateNextRewardTimeCmdProcessor : ICmdProcessor
	{
		
		private JingJiUpdateNextRewardTimeCmdProcessor()
		{
		}

		
		public static JingJiUpdateNextRewardTimeCmdProcessor getInstance()
		{
			return JingJiUpdateNextRewardTimeCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			long[] _cmdParams = DataHelper.BytesToObject<long[]>(cmdParams, 0, count);
			int roleId = (int)_cmdParams[0];
			long nextRewardTime = _cmdParams[1];
			bool result = JingJiChangManager.getInstance().updateNextRewardTime(roleId, nextRewardTime);
			client.sendCmd<int>(10149, result ? 1 : 0);
		}

		
		private static JingJiUpdateNextRewardTimeCmdProcessor instance = new JingJiUpdateNextRewardTimeCmdProcessor();
	}
}
