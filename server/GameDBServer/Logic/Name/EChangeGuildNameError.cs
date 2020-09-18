using System;

namespace GameDBServer.Logic.Name
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
