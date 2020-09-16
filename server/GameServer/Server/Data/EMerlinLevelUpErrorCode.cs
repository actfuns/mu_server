using System;

namespace Server.Data
{
	
	public enum EMerlinLevelUpErrorCode
	{
		
		Error = -1,
		
		Success,
		
		LevelError,
		
		MaxLevelNum,
		
		NotMaxStarNum,
		
		LevelDataError,
		
		NeedGoodsIDError,
		
		NeedGoodsCountError,
		
		GoodsNotEnough,
		
		DiamondNotEnough,
		
		Fail
	}
}
