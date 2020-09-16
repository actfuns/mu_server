using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiJunxianLevelupCmdProcessor : ICmdProcessor
	{
		
		private JingJiJunxianLevelupCmdProcessor()
		{
		}

		
		public static JingJiJunxianLevelupCmdProcessor getInstance()
		{
			return JingJiJunxianLevelupCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int result = JingJiChangManager.getInstance().upGradeJunXian(client);
			client.sendCmd<int>(586, result, false);
			return true;
		}

		
		private static JingJiJunxianLevelupCmdProcessor instance = new JingJiJunxianLevelupCmdProcessor();
	}
}
