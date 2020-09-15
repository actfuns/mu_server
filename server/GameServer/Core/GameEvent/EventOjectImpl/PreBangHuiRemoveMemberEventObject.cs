using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D6 RID: 214
	public class PreBangHuiRemoveMemberEventObject : EventObjectEx
	{
		// Token: 0x0600039F RID: 927 RVA: 0x0003D019 File Offset: 0x0003B219
		public PreBangHuiRemoveMemberEventObject(GameClient player, int bhid) : base(24)
		{
			this.Player = player;
			this.BHID = bhid;
			this.Result = true;
		}

		// Token: 0x040004FB RID: 1275
		public GameClient Player;

		// Token: 0x040004FC RID: 1276
		public int BHID;
	}
}
