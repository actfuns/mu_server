using System;

namespace LogDBServer.Server
{
	
	public interface ICmdProcessor
	{
		
		void processCmd(GameServerClient client, byte[] cmdParams, int count);
	}
}
