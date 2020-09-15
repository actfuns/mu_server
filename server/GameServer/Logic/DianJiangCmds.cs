using System;

namespace GameServer.Logic
{
	// Token: 0x02000642 RID: 1602
	public enum DianJiangCmds
	{
		// Token: 0x04002EF0 RID: 12016
		None,
		// Token: 0x04002EF1 RID: 12017
		CreateRoom,
		// Token: 0x04002EF2 RID: 12018
		RemoveRoom,
		// Token: 0x04002EF3 RID: 12019
		EnterRoom,
		// Token: 0x04002EF4 RID: 12020
		LeaveRoom,
		// Token: 0x04002EF5 RID: 12021
		KickRole,
		// Token: 0x04002EF6 RID: 12022
		ChangeTeam,
		// Token: 0x04002EF7 RID: 12023
		ChangeState,
		// Token: 0x04002EF8 RID: 12024
		ViewFight,
		// Token: 0x04002EF9 RID: 12025
		ToViewer,
		// Token: 0x04002EFA RID: 12026
		ToLeave
	}
}
