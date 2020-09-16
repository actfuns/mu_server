using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiGetBuffCmdProcessor : ICmdProcessor
	{
		
		private JingJiGetBuffCmdProcessor()
		{
		}

		
		public static JingJiGetBuffCmdProcessor getInstance()
		{
			return JingJiGetBuffCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int _replace = Convert.ToInt32(cmdParams[1]);
			int result;
			if (_replace != 0 && _replace != 1)
			{
				result = ResultCode.Illegal;
			}
			else
			{
				result = JingJiChangManager.getInstance().activeJunXianBuff(client, _replace == 1);
			}
			client.sendCmd<int>(585, result, false);
			return true;
		}

		
		private static JingJiGetBuffCmdProcessor instance = new JingJiGetBuffCmdProcessor();
	}
}
