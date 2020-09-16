using System;

namespace GameServer.Logic
{
	
	internal enum ActivityErrorType
	{
		
		HEFULOGIN_NOTVIP = -100,
		
		FATALERROR = -60,
		
		AWARDCFG_ERROR = -50,
		
		AWARDTIME_OUT = -40,
		
		NOTCONDITION = -30,
		
		BAG_NOTENOUGH = -20,
		
		ALREADY_GETED = -10,
		
		MINAWARDCONDIONVALUE = -5,
		
		ACTIVITY_NOTEXIST = -1,
		
		RECEIVE_SUCCEED
	}
}
