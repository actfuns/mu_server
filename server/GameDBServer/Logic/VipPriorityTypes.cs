using System;

namespace GameDBServer.Logic
{
	
	public enum VipPriorityTypes
	{
		
		None,
		
		GetDailyYuanBao,
		
		GetDailyLingLi = 5,
		
		GetDailyYinLiang,
		
		GetDailyAttackFuZhou,
		
		GetDailyDefenseFuZhou,
		
		GetDailyLifeFuZhou,
		
		GetDailyTongQian,
		
		GetDailyZhenQi = 21,
		
		MaxVal = 100
	}
}
