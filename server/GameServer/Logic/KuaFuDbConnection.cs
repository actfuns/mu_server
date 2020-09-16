using System;
using GameServer.Server;

namespace GameServer.Logic
{
	
	public class KuaFuDbConnection : IDisposable
	{
		
		public KuaFuDbConnection(int serverId)
		{
			this.ServerId = serverId;
		}

		
		~KuaFuDbConnection()
		{
			this.Dispose();
		}

		
		public void Dispose()
		{
			this.Pool[0].Clear();
			this.Pool[1].Clear();
		}

		
		public int ServerId;

		
		public int ErrorCount = 0;

		
		public long LastHeartTicks;

		
		public GameDbClientPool[] Pool = new GameDbClientPool[]
		{
			new GameDbClientPool(),
			new GameDbClientPool()
		};
	}
}
