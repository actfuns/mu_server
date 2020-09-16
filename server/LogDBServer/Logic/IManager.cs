using System;

namespace LogDBServer.Logic
{
	
	public interface IManager
	{
		
		bool initialize();

		
		bool startup();

		
		bool showdown();

		
		bool destroy();
	}
}
