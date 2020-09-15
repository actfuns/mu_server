using System;
using GameServer.Logic;

namespace GameServer.Server
{
	// Token: 0x0200004F RID: 79
	public interface ICmdProcessor
	{
		// Token: 0x060000EF RID: 239
		bool processCmd(GameClient client, string[] cmdParams);
	}
}
