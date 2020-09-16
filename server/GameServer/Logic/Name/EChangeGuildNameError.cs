using System;

namespace GameServer.Logic.Name
{
	
	public enum EChangeGuildNameError
	{
		
		Success,
		
		InvalidName,
		
		DBFailed,
		
		NameAlreadyUsed,
		
		OperatorDenied,
		
		LengthError
	}
}
