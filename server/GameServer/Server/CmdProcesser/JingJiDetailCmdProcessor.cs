using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;
using Server.Data;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiDetailCmdProcessor : ICmdProcessor
	{
		
		private JingJiDetailCmdProcessor()
		{
		}

		
		public static JingJiDetailCmdProcessor getInstance()
		{
			return JingJiDetailCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int type = Convert.ToInt32(cmdParams[1]);
			JingJiDetailData data = JingJiChangManager.getInstance().getDetailData(client, type);
			client.sendCmd<JingJiDetailData>(578, data, false);
			return true;
		}

		
		private static JingJiDetailCmdProcessor instance = new JingJiDetailCmdProcessor();
	}
}
