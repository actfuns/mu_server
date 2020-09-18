using System;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	
	public enum ESevenDayActErrorCode
	{
		
		Success,
		
		NotInActivityTime,
		
		ServerConfigError,
		
		NoBagSpace,
		
		DBFailed,
		
		NotReachCondition,
		
		VisitParamsWrong,
		
		ZuanShiNotEnough,
		
		NoEnoughGoodsCanBuy
	}
}
