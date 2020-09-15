using System;
using GameServer.Logic;

namespace GameServer.Server
{
	// Token: 0x02000050 RID: 80
	public interface ICmdProcessorEx : ICmdProcessor
	{
		// Token: 0x060000F0 RID: 240
		bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams);
	}
}
