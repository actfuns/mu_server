using System;

namespace Server.Data
{
	
	public enum BanType
	{
		
		Old,
		
		BanLog,
		
		BanKick,
		
		BanClose,
		
		BanKickBreak,
		
		BanCloseBreak,
		
		BanRate,
		
		BanRateNum,
		
		BanLogBig = 10,
		
		BanLogNormal,
		
		BanLogInvalid,
		
		BanLogAppVM = 20,
		
		BanKickAppVM,
		
		BanCloseAppVM,
		
		BanRateAppVM,
		
		BanLogAppPlatformCount = 30,
		
		BanKickAppPlatformCount,
		
		BanCloseAppPlatformCount,
		
		BanRateAppPlatformCount,
		
		BanLogDeviceNull = 40,
		
		BanKickDeviceNull,
		
		BanCloseDeviceNull,
		
		BanRateDeviceNull,
		
		BanLogSpecialApp = 50,
		
		BanKickSpecialApp,
		
		BanCloseSpecialApp,
		
		BanRateSpecialApp,
		
		BanLogAppCount = 60,
		
		BanKickAppCount,
		
		BanCloseAppCount,
		
		BanRateAppCount,
		
		BanLogMulti = 70,
		
		BanKickMulti,
		
		BanCloseMulti,
		
		BanRateMulti,
		
		BanLogTimeOut = 80,
		
		BanKickTimeOut,
		
		BanCloseTimeOut,
		
		BanRateTimeOut,
		
		BanLogDecrypt = 90,
		
		BanKickDecrypt,
		
		BanCloseDecrypt,
		
		BanRateDecrypt,
		
		VmLog = 110,
		
		VmKick,
		
		VmClose,
		
		VmRate,
		
		VmLogSign = 120,
		
		VmKickSign,
		
		VmCloseSign,
		
		VmRateSign,
		
		Max = 10000
	}
}
