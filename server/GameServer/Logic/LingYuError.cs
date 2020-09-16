using System;

namespace GameServer.Logic
{
	
	internal enum LingYuError
	{
		
		Success,
		
		NotOpen,
		
		LevelFull,
		
		NeedLevelUp,
		
		NeedSuitUp,
		
		SuitFull,
		
		LevelUpMaterialNotEnough,
		
		LevelUpJinBiNotEnough,
		
		SuitUpMaterialNotEnough,
		
		SuitUpJinBiNotEnough,
		
		ErrorConfig,
		
		ErrorParams,
		
		ZuanShiNotEnough,
		
		DBSERVERERROR
	}
}
