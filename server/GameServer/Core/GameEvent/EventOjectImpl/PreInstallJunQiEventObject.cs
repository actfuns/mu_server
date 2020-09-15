using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D2 RID: 210
	public class PreInstallJunQiEventObject : EventObjectEx
	{
		// Token: 0x0600039B RID: 923 RVA: 0x0003CF81 File Offset: 0x0003B181
		public PreInstallJunQiEventObject(GameClient player, int npcID, int sceneType) : base(22)
		{
			this.Player = player;
			this.NPCID = npcID;
			this.SceneType = sceneType;
		}

		// Token: 0x040004ED RID: 1261
		public GameClient Player;

		// Token: 0x040004EE RID: 1262
		public int NPCID;

		// Token: 0x040004EF RID: 1263
		public int SceneType;
	}
}
