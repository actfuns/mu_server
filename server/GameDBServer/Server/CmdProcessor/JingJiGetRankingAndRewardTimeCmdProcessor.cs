using System;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001F2 RID: 498
	public class JingJiGetRankingAndRewardTimeCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A5F RID: 2655 RVA: 0x00062195 File Offset: 0x00060395
		private JingJiGetRankingAndRewardTimeCmdProcessor()
		{
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x000621A0 File Offset: 0x000603A0
		public static JingJiGetRankingAndRewardTimeCmdProcessor getInstance()
		{
			return JingJiGetRankingAndRewardTimeCmdProcessor.instance;
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x000621B8 File Offset: 0x000603B8
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			int ranking = -2;
			long nextRewardTime = 0L;
			JingJiChangManager.getInstance().getRankingAndNextRewardTimeById(roleId, out ranking, out nextRewardTime);
			long[] resultParams = new long[]
			{
				(long)ranking,
				nextRewardTime
			};
			client.sendCmd<long[]>(10148, resultParams);
		}

		// Token: 0x04000C54 RID: 3156
		private static JingJiGetRankingAndRewardTimeCmdProcessor instance = new JingJiGetRankingAndRewardTimeCmdProcessor();
	}
}
