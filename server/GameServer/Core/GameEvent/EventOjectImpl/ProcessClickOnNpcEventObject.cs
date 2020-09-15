using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D9 RID: 217
	public class ProcessClickOnNpcEventObject : EventObjectEx
	{
		// Token: 0x060003A2 RID: 930 RVA: 0x0003D086 File Offset: 0x0003B286
		public ProcessClickOnNpcEventObject(GameClient client, NPC npc, int npcId, int extensionID) : base(27)
		{
			this.Client = client;
			this.Npc = npc;
			this.NpcId = npcId;
			this.ExtensionID = extensionID;
		}

		// Token: 0x04000502 RID: 1282
		public GameClient Client;

		// Token: 0x04000503 RID: 1283
		public NPC Npc;

		// Token: 0x04000504 RID: 1284
		public int NpcId;

		// Token: 0x04000505 RID: 1285
		public int ExtensionID;
	}
}
