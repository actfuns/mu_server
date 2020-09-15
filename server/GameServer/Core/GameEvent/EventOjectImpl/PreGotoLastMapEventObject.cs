using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D1 RID: 209
	public class PreGotoLastMapEventObject : EventObjectEx
	{
		// Token: 0x0600039A RID: 922 RVA: 0x0003CF66 File Offset: 0x0003B166
		public PreGotoLastMapEventObject(GameClient player, int sceneType) : base(21)
		{
			this.Player = player;
			this.SceneType = sceneType;
		}

		// Token: 0x040004EB RID: 1259
		public GameClient Player;

		// Token: 0x040004EC RID: 1260
		public int SceneType;
	}
}
