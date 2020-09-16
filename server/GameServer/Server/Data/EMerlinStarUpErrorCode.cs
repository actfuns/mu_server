using System;

namespace Server.Data
{
	
	public enum EMerlinStarUpErrorCode
	{
		
		Error = -1,
		
		Success,
		
		MaxStarNum,
		
		StarDataError,
		
		NeedGoodsIDError,
		
		NeedGoodsCountError,
		
		GoodsNotEnough,
		
		DiamondNotEnough,
		
		LevelError,
		
		StarError
	}
}
