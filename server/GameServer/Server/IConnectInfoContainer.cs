using System;

namespace GameServer.Server
{
	
	public interface IConnectInfoContainer
	{
		
		void AddDBConnectInfo(int index, string info);
	}
}
