using System;
using GameDBServer.Core.GameEvent;

namespace GameServer.Core.AssemblyPatch
{
	
	internal class MethodConfig
	{
		
		public string assemblyName;

		
		public EventTypes eventType;

		
		public string fullClassName;

		
		public string methodName;

		
		public string[] methodParams;

		
		public int cmdID;
	}
}
