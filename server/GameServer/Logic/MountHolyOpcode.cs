using System;

namespace GameServer.Logic
{
	
	public enum MountHolyOpcode
	{
		
		Succ = 1,
		
		NotOpen,
		
		ParamErr,
		
		UpGradeGoodTypeErr,
		
		NotExsitGood,
		
		NotExsitPhoyGood,
		
		PhoyGoodCountErr,
		
		NotExsitGoodXml,
		
		NotExsitInfo,
		
		HasMaxLevel,
		
		CategoriyErr,
		
		UpGradeErr,
		
		UpGradeCountAttrErr,
		
		MakeAttrErr,
		
		UseGoodErr,
		
		GoodHasUsing,
		
		NotUsingType,
		
		GetHoleNumErr,
		
		CurrHoleLock,
		
		GoodHasNot,
		
		HoleInfoIsNull,
		
		HolyGoodListNotFree,
		
		DataModifyErr
	}
}
