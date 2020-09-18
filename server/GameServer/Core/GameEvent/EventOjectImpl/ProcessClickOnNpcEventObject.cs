using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class ProcessClickOnNpcEventObject : EventObjectEx
	{
		
		public ProcessClickOnNpcEventObject(GameClient client, NPC npc, int npcId, int extensionID) : base(27)
		{
			this.Client = client;
			this.Npc = npc;
			this.NpcId = npcId;
			this.ExtensionID = extensionID;
		}

		
		public GameClient Client;

		
		public NPC Npc;

		
		public int NpcId;

		
		public int ExtensionID;
	}
}
