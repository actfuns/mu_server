using System;

namespace GameDBServer.Logic.UnionAlly
{
	
	public enum EAlly
	{
		
		EUnionLevel = -9,
		
		EAllyMax,
		
		EAllyRequestMax,
		
		ENotLeader,
		
		EIsSelf,
		
		EZoneID,
		
		EName,
		
		EMoney,
		
		EMore,
		
		SuccRequest = 1,
		
		EAlly = 10,
		
		AllyRefuse,
		
		AllyAgree,
		
		AllyRefuseOther = 20,
		
		AllyAgreeOther,
		
		EAllyCancel = 30,
		
		AllyCancelSucc,
		
		EAllyRemove = 40,
		
		AllyRemoveSucc,
		
		AllyRemoveSuccOther,
		
		Default = 0
	}
}
