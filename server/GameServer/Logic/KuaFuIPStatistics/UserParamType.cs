using System;

namespace GameServer.Logic.KuaFuIPStatistics
{
	
	internal enum UserParamType
	{
		
		Begin,
		
		LoginFailByDataCheckCnt = 0,
		
		LoginFailByBanCnt,
		
		LoginFailByLoginTimeOutCnt,
		
		InitGameFailByBanCnt,
		
		createRoleFailByCnt,
		
		createRoleFailByBan,
		
		Max
	}
}
