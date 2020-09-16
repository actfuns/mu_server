using System;

namespace Server.Data
{
	
	public enum EFluorescentGemDigErrorCode
	{
		
		NotOpen = -2,
		
		Error,
		
		Success,
		
		LevelTypeError,
		
		DigType,
		
		BagNotEnoughOne,
		
		LevelTypeDataError,
		
		PointNotEnough,
		
		DiamondNotEnough,
		
		UpdatePointError,
		
		UpdateDiamondError,
		
		DigDataError,
		
		BagNotEnoughTen,
		
		AddGoodsError,
		
		NotGem
	}
}
