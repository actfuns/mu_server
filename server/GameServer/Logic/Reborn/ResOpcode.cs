using System;

namespace GameServer.Logic.Reborn
{
	
	public enum ResOpcode
	{
		
		Succ = 1,
		
		Fail,
		
		GongNengWeiKaiQi,
		
		ChooseYinJiTypeErr,
		
		ChooseGetInfoYinJiNotActive,
		
		ChooseGetInfoYinJiIsActive,
		
		ChooseYinJiIsActiveErr,
		
		ResetYinJiZuanShiErr,
		
		ResetYinJiInfoErr,
		
		GetYinJiInfoErr,
		
		LevelUpYinJiPointErr,
		
		LevelUpYinJiTypeErr,
		
		LevelUpYinJiCheckErr,
		
		LevelUpYinJiMaxLv,
		
		LevelUpYinJiSaveErr,
		
		LevelUpYinJiUpNumErr,
		
		LevelUpYinJiOverUpLvErr
	}
}
