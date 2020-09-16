using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiRemoveCDCmdProcessor : ICmdProcessor
	{
		
		private JingJiRemoveCDCmdProcessor()
		{
		}

		
		public static JingJiRemoveCDCmdProcessor getInstance()
		{
			return JingJiRemoveCDCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int result = JingJiChangManager.getInstance().removeCD(client);
			client.sendCmd<int>(584, result, false);
			return true;
		}

		
		private static JingJiRemoveCDCmdProcessor instance = new JingJiRemoveCDCmdProcessor();
	}
}
