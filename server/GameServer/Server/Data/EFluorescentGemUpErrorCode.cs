using System;

namespace Server.Data
{
	
	public enum EFluorescentGemUpErrorCode
	{
		
		NotOpen = -2,
		
		Error,
		
		Success,
		
		GoodsNotExist,
		
		UpDataError,
		
		MaxLevel,
		
		NextLevelDataError,
		
		GoldNotEnough,
		
		PositionIndexError,
		
		GemTypeError,
		
		GemNotEnough,
		
		AddGoodsError,
		
		DecGoodsError,
		
		DecGoodsNotExist,
		
		DecGoodsNotEnough,
		
		EquipError,
		
		NotGem,
		
		BagNotEnoughOne
	}
}
