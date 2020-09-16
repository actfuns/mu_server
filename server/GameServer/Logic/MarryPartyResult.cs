using System;

namespace GameServer.Logic
{
	
	public enum MarryPartyResult
	{
		
		Success,
		
		PartyNotFound,
		
		InvalidParam,
		
		NotEnoughMoney,
		
		NotMarry,
		
		AlreadyRequest,
		
		AlreadyStart,
		
		NotOriginator,
		
		NotStart,
		
		ZeroPartyJoinCount,
		
		ZeroPlayerJoinCount,
		
		NotOpen
	}
}
