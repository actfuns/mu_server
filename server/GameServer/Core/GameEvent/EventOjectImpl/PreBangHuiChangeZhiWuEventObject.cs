using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000DE RID: 222
	public class PreBangHuiChangeZhiWuEventObject : EventObjectEx
	{
		// Token: 0x060003A7 RID: 935 RVA: 0x0003D12C File Offset: 0x0003B32C
		public PreBangHuiChangeZhiWuEventObject(GameClient player, int bhid, int targetRoleId, int targetZhiWu) : base(25)
		{
			this.Player = player;
			this.BHID = bhid;
			this.TargetRoleId = targetRoleId;
			this.TargetZhiWu = targetZhiWu;
		}

		// Token: 0x04000510 RID: 1296
		public GameClient Player;

		// Token: 0x04000511 RID: 1297
		public int BHID;

		// Token: 0x04000512 RID: 1298
		public int TargetRoleId;

		// Token: 0x04000513 RID: 1299
		public int TargetZhiWu;

		// Token: 0x04000514 RID: 1300
		public int ErrorCode;
	}
}
