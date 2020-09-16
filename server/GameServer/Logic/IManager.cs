using System;

namespace GameServer.Logic
{
	
	public interface IManager
	{
		
		bool initialize();

		
		bool startup();

		
		bool showdown();

		
		bool destroy();
	}
}
