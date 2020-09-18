using System;

namespace GameServer.Core.GameEvent
{
	
	public class GlobalEventSource4Scene : SceneEventSource
	{
		
		private GlobalEventSource4Scene()
		{
		}

		
		public static GlobalEventSource4Scene getInstance()
		{
			return GlobalEventSource4Scene.instance;
		}

		
		private static GlobalEventSource4Scene instance = new GlobalEventSource4Scene();
	}
}
