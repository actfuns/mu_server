using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D8 RID: 216
	public class PreZhanDuiChangeMemberEventObject : EventObjectEx
	{
		// Token: 0x060003A1 RID: 929 RVA: 0x0003D05D File Offset: 0x0003B25D
		public PreZhanDuiChangeMemberEventObject(GameClient player, int bhid, int _operator) : base(63)
		{
			this.Player = player;
			this.ZhanDuiID = bhid;
			this.Operator = _operator;
			this.Result = true;
		}

		// Token: 0x040004FF RID: 1279
		public GameClient Player;

		// Token: 0x04000500 RID: 1280
		public int ZhanDuiID;

		// Token: 0x04000501 RID: 1281
		public int Operator;
	}
}
