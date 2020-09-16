using System;

namespace Server.Data
{
	
	public enum EFluorescentGemResolveErrorCode
	{
		
		NotOpen = -2,
		
		Error,
		
		Success,
		
		GoodsNotExist,
		
		ResolveCountError,
		
		ResolveError,
		
		NotGem
	}
}
