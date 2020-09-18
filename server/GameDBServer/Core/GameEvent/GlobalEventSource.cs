using System;

namespace GameDBServer.Core.GameEvent
{
	
	public class GlobalEventSource : EventSource
	{
		
		private GlobalEventSource()
		{
		}

		
		public static GlobalEventSource getInstance()
		{
			return GlobalEventSource.instance;
		}

		
		private static GlobalEventSource instance = new GlobalEventSource();
	}
}
