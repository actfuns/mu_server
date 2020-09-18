using System;
using GameServer.Logic;

namespace GameServer.Core.GameEvent
{
	
	public class MainTaskProgressEvent : EventObject
	{
		
		public MainTaskProgressEvent(GameClient client, int taskId) : base(58)
		{
			this.Client = client;
			this.MainTaskID = taskId;
		}

		
		public int MainTaskID;

		
		public GameClient Client;
	}
}
