using System;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200045C RID: 1116
	public interface ICondJudger
	{
		// Token: 0x0600149E RID: 5278
		bool Judge(GameClient client, string arg, out string failedMsg);
	}
}
