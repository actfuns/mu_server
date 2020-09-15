using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000DC RID: 220
	public class OnClientChangeMapEventObject : EventObjectEx
	{
		// Token: 0x060003A5 RID: 933 RVA: 0x0003D0E6 File Offset: 0x0003B2E6
		public OnClientChangeMapEventObject(GameClient client, int teleportID, int toMapCode, int toPosX, int toPosY) : base(29)
		{
			this.Client = client;
			this.TeleportID = teleportID;
			this.ToMapCode = toMapCode;
			this.ToPosX = toPosX;
			this.ToPosY = toPosY;
		}

		// Token: 0x0400050A RID: 1290
		public GameClient Client;

		// Token: 0x0400050B RID: 1291
		public int TeleportID;

		// Token: 0x0400050C RID: 1292
		public int ToMapCode;

		// Token: 0x0400050D RID: 1293
		public int ToPosX;

		// Token: 0x0400050E RID: 1294
		public int ToPosY;
	}
}
