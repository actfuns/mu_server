using System;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
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

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			bool result = JingJiChangManager.getInstance().removeCD(roleId);
			client.sendCmd<bool>(10147, result);
		}

		
		private static JingJiRemoveCDCmdProcessor instance = new JingJiRemoveCDCmdProcessor();
	}
}
