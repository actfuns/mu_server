using System;

namespace GameServer.Logic
{
	
	public enum TeamCmds
	{
		
		None,
		
		Create,
		
		Destroy,
		
		Invite,
		
		Apply,
		
		Refuse,
		
		AgreeInvite,
		
		AgreeApply,
		
		Remove,
		
		Quit,
		
		AppointLeader,
		
		GetThingOpt,
		
		Ready,
		
		QuickJoinTeam,
		
		Start,
		
		ChangeKickFlag,
		
		AutoStart
	}
}
