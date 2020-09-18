using System;

namespace GameServer.Logic
{
	
	public enum CallSpriteResult
	{
		
		Success,
		
		ErrorParams,
		
		ErrorConfig,
		
		ErrorLevel,
		
		ZuanShiNotEnough,
		
		BagIsFull,
		
		SpriteBagIsFull,
		
		GoodsNotExist,
		
		DBSERVERERROR
	}
}
