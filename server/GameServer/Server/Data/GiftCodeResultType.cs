using System;

namespace Server.Data
{
	
	public enum GiftCodeResultType
	{
		
		Default,
		
		Success,
		
		EnoUserOrRole = -1,
		
		EAware = -2,
		
		Fail = -3,
		
		Exception = -4
	}
}
