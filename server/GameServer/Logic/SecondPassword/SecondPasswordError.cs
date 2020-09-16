using System;

namespace GameServer.Logic.SecondPassword
{
	
	public enum SecondPasswordError
	{
		
		SecPwdVerifySuccess,
		
		SecPwdVerifyFailed,
		
		SecPwdIsNotSet,
		
		SecPwdCharInvalid,
		
		SecPwdIsNull,
		
		SecPwdIsTooShort,
		
		SecPwdIsTooLong,
		
		SecPwdSetSuccess,
		
		SecPwdDBFailed,
		
		SecPwdClearSuccess
	}
}
