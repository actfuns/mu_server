using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A5 RID: 2213
	public class JingJiRankingRewardCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D76 RID: 15734 RVA: 0x00345562 File Offset: 0x00343762
		private JingJiRankingRewardCmdProcessor()
		{
		}

		// Token: 0x06003D77 RID: 15735 RVA: 0x00345570 File Offset: 0x00343770
		public static JingJiRankingRewardCmdProcessor getInstance()
		{
			return JingJiRankingRewardCmdProcessor.instance;
		}

		// Token: 0x06003D78 RID: 15736 RVA: 0x00345588 File Offset: 0x00343788
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int result;
			long nextRewardTime;
			JingJiChangManager.getInstance().rankingReward(client, out result, out nextRewardTime);
			client.sendCmd<long[]>(583, new long[]
			{
				(long)result,
				nextRewardTime
			}, false);
			return true;
		}

		// Token: 0x040047B5 RID: 18357
		private static JingJiRankingRewardCmdProcessor instance = new JingJiRankingRewardCmdProcessor();
	}
}
