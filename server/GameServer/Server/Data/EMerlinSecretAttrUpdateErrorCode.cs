using System;

namespace Server.Data
{
	
	public enum EMerlinSecretAttrUpdateErrorCode
	{
		
		Error = -1,
		
		Success,
		
		LevelError,
		
		SecretDataError,
		
		NeedGoodsIDError,
		
		NeedGoodsCountError,
		
		GoodsNotEnough
	}
}
