using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001F3 RID: 499
	public class JingJiGetChallengeInfoDataCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A63 RID: 2659 RVA: 0x00062215 File Offset: 0x00060415
		private JingJiGetChallengeInfoDataCmdProcessor()
		{
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x00062220 File Offset: 0x00060420
		public static JingJiGetChallengeInfoDataCmdProcessor getInstance()
		{
			return JingJiGetChallengeInfoDataCmdProcessor.instance;
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x00062238 File Offset: 0x00060438
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] _cmdParams = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			int roleId = _cmdParams[0];
			int pageIndex = _cmdParams[1];
			List<JingJiChallengeInfoData> rankingListData = JingJiChangManager.getInstance().getChallengeInfoDataList(roleId, pageIndex);
			client.sendCmd<List<JingJiChallengeInfoData>>(10146, rankingListData);
		}

		// Token: 0x04000C55 RID: 3157
		private static JingJiGetChallengeInfoDataCmdProcessor instance = new JingJiGetChallengeInfoDataCmdProcessor();
	}
}
