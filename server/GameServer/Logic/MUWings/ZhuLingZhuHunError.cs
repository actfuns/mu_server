using System;

namespace GameServer.Logic.MUWings
{
	
	public enum ZhuLingZhuHunError
	{
		
		Success,
		
		ZhuLingNotOpen,
		
		ZhuHunNotOpen,
		
		ZhuLingFull,
		
		ZhuHunFull,
		
		ZhuLingMaterialNotEnough,
		
		ZhuLingJinBiNotEnough,
		
		ZhuHunMaterialNotEnough,
		
		ZhuHunJinBiNotEnough,
		
		ErrorConfig,
		
		ErrorParams,
		
		ZuanShiNotEnough,
		
		DBSERVERERROR
	}
}
