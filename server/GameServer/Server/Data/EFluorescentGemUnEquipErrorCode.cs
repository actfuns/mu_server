using System;

namespace Server.Data
{
	
	public enum EFluorescentGemUnEquipErrorCode
	{
		
		NotOpen = -2,
		
		Error,
		
		Success,
		
		GoodsNotExist,
		
		PositionIndexError,
		
		GemTypeError,
		
		UnEquipError,
		
		BagNotEnoughOne,
		
		BagNotEnoughThree
	}
}
