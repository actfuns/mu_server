using System;
using GameServer.Server;

namespace GameServer.Logic
{
	// Token: 0x0200050E RID: 1294
	public class KuaFuDbConnection : IDisposable
	{
		// Token: 0x0600180D RID: 6157 RVA: 0x00178248 File Offset: 0x00176448
		public KuaFuDbConnection(int serverId)
		{
			this.ServerId = serverId;
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x0017828C File Offset: 0x0017648C
		~KuaFuDbConnection()
		{
			this.Dispose();
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x001782C0 File Offset: 0x001764C0
		public void Dispose()
		{
			this.Pool[0].Clear();
			this.Pool[1].Clear();
		}

		// Token: 0x04002250 RID: 8784
		public int ServerId;

		// Token: 0x04002251 RID: 8785
		public int ErrorCount = 0;

		// Token: 0x04002252 RID: 8786
		public long LastHeartTicks;

		// Token: 0x04002253 RID: 8787
		public GameDbClientPool[] Pool = new GameDbClientPool[]
		{
			new GameDbClientPool(),
			new GameDbClientPool()
		};
	}
}
