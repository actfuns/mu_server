using System;

namespace LogDBServer.Server
{
	// Token: 0x02000026 RID: 38
	public interface ICmdProcessor
	{
		// Token: 0x060000D8 RID: 216
		void processCmd(GameServerClient client, byte[] cmdParams, int count);
	}
}
