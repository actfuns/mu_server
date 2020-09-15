using System;

namespace GameDBServer.Server
{
	// Token: 0x02000108 RID: 264
	public interface ICmdProcessor
	{
		// Token: 0x06000470 RID: 1136
		void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count);
	}
}
