using System;

namespace GameServer.Logic
{
	
	public enum EHolyResult
	{
		
		Error = -1,
		
		Success,
		
		Fail,
		
		NeedGold,
		
		NeedHolyItemPart,
		
		PartSuitIsMax,
		
		NotOpen,
		
		NeedGoods
	}
}
