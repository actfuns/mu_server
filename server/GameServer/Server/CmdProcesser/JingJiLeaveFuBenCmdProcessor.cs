using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiLeaveFuBenCmdProcessor : ICmdProcessor
	{
		
		private JingJiLeaveFuBenCmdProcessor()
		{
		}

		
		public static JingJiLeaveFuBenCmdProcessor getInstance()
		{
			return JingJiLeaveFuBenCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			JingJiChangManager.getInstance().onLeaveFuBenForStopCD(client);
			return true;
		}

		
		private static JingJiLeaveFuBenCmdProcessor instance = new JingJiLeaveFuBenCmdProcessor();
	}
}
