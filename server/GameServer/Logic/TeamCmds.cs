using System;

namespace GameServer.Logic
{
	// Token: 0x0200063D RID: 1597
	public enum TeamCmds
	{
		// Token: 0x04002EC9 RID: 11977
		None,
		// Token: 0x04002ECA RID: 11978
		Create,
		// Token: 0x04002ECB RID: 11979
		Destroy,
		// Token: 0x04002ECC RID: 11980
		Invite,
		// Token: 0x04002ECD RID: 11981
		Apply,
		// Token: 0x04002ECE RID: 11982
		Refuse,
		// Token: 0x04002ECF RID: 11983
		AgreeInvite,
		// Token: 0x04002ED0 RID: 11984
		AgreeApply,
		// Token: 0x04002ED1 RID: 11985
		Remove,
		// Token: 0x04002ED2 RID: 11986
		Quit,
		// Token: 0x04002ED3 RID: 11987
		AppointLeader,
		// Token: 0x04002ED4 RID: 11988
		GetThingOpt,
		// Token: 0x04002ED5 RID: 11989
		Ready,
		// Token: 0x04002ED6 RID: 11990
		QuickJoinTeam,
		// Token: 0x04002ED7 RID: 11991
		Start,
		// Token: 0x04002ED8 RID: 11992
		ChangeKickFlag,
		// Token: 0x04002ED9 RID: 11993
		AutoStart
	}
}
