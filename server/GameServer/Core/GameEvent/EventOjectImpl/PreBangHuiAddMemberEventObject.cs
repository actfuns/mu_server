using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D5 RID: 213
	public class PreBangHuiAddMemberEventObject : EventObjectEx
	{
		// Token: 0x0600039E RID: 926 RVA: 0x0003CFF7 File Offset: 0x0003B1F7
		public PreBangHuiAddMemberEventObject(GameClient player, int bhid) : base(23)
		{
			this.Player = player;
			this.BHID = bhid;
			this.Result = true;
		}

		// Token: 0x040004F9 RID: 1273
		public GameClient Player;

		// Token: 0x040004FA RID: 1274
		public int BHID;
	}
}
