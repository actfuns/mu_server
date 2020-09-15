using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D7 RID: 215
	public class PostBangHuiChangeEventObject : EventObjectEx
	{
		// Token: 0x060003A0 RID: 928 RVA: 0x0003D03B File Offset: 0x0003B23B
		public PostBangHuiChangeEventObject(GameClient player, int bhid) : base(26)
		{
			this.Player = player;
			this.BHID = bhid;
			this.Result = true;
		}

		// Token: 0x040004FD RID: 1277
		public GameClient Player;

		// Token: 0x040004FE RID: 1278
		public int BHID;
	}
}
