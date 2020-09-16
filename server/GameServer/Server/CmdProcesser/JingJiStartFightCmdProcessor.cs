using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	
	public class JingJiStartFightCmdProcessor : ICmdProcessor
	{
		
		private JingJiStartFightCmdProcessor()
		{
		}

		
		public static JingJiStartFightCmdProcessor getInstance()
		{
			return JingJiStartFightCmdProcessor.instance;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int nID = 634;
			int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
			bool result;
			if (-1 == JingJiChangManager.getInstance().JingJiChangStartFight(client))
			{
				string strCmd = string.Format("{0}:{1}", -1, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			else
			{
				string strCmd = string.Format("{0}:{1}", 0, nRoleID);
				client.sendCmd(nID, strCmd, false);
				result = true;
			}
			return result;
		}

		
		private static JingJiStartFightCmdProcessor instance = new JingJiStartFightCmdProcessor();
	}
}
