using System;

namespace Server.Data
{
	
	public enum ShenQiActionResultType
	{
		
		End = 3,
		
		Next = 2,
		
		Success = 1,
		
		Efail = 0,
		
		EnoOpen = -1,
		
		EnoShenLiJingHua = -2,
		
		EnoDiamond = -3,
		
		EnoJinBi = -4,
		
		None = -100
	}
}
