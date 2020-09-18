using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class JingJiSaveDataCmdProcessor : ICmdProcessor
	{
		
		private JingJiSaveDataCmdProcessor()
		{
		}

		
		public static JingJiSaveDataCmdProcessor getInstance()
		{
			return JingJiSaveDataCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			JingJiSaveData data = DataHelper.BytesToObject<JingJiSaveData>(cmdParams, 0, count);
			int winCount;
			JingJiChangManager.getInstance().saveData(data, out winCount);
			client.sendCmd<int>(10145, winCount);
		}

		
		private static JingJiSaveDataCmdProcessor instance = new JingJiSaveDataCmdProcessor();
	}
}
