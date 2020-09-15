using System;
using GameServer.Logic;
using Server.Data;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000CE RID: 206
	public class ChargeItemBaseEventObject : EventObject
	{
		// Token: 0x0600038F RID: 911 RVA: 0x0003CE8F File Offset: 0x0003B08F
		public ChargeItemBaseEventObject(GameClient player, ChargeItemData Config) : base(36)
		{
			this.Player = player;
			this.ChargeItemConfig = Config;
		}

		// Token: 0x040004E4 RID: 1252
		public GameClient Player;

		// Token: 0x040004E5 RID: 1253
		public ChargeItemData ChargeItemConfig;

		// Token: 0x040004E6 RID: 1254
		public int ChargeMoney;
	}
}
