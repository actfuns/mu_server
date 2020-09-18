using System;

namespace GameDBServer.Logic
{
	
	public interface IManager
	{
		
		bool initialize();

		
		bool startup();

		
		bool showdown();

		
		bool destroy();
	}
}
