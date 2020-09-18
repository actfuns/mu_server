using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class JingJiGetChallengeInfoDataCmdProcessor : ICmdProcessor
	{
		
		private JingJiGetChallengeInfoDataCmdProcessor()
		{
		}

		
		public static JingJiGetChallengeInfoDataCmdProcessor getInstance()
		{
			return JingJiGetChallengeInfoDataCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] _cmdParams = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			int roleId = _cmdParams[0];
			int pageIndex = _cmdParams[1];
			List<JingJiChallengeInfoData> rankingListData = JingJiChangManager.getInstance().getChallengeInfoDataList(roleId, pageIndex);
			client.sendCmd<List<JingJiChallengeInfoData>>(10146, rankingListData);
		}

		
		private static JingJiGetChallengeInfoDataCmdProcessor instance = new JingJiGetChallengeInfoDataCmdProcessor();
	}
}
