using System;

namespace GameServer.Logic
{
	
	public enum MarryFubenResult
	{
		
		Error = -1,
		
		Success,
		
		ResultRoomInfo,
		
		NotMarriaged,
		
		InFuben,
		
		SelfOrOtherLimit,
		
		IsReaday,
		
		NotOpen,
		
		Error_Denied_For_Minor_Occupation = -35
	}
}
