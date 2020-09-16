using System;

namespace GameServer.Logic.FuMo
{
	
	public enum FuMoMailOptEnum
	{
		
		FuMo_Fail = -1,
		
		FuMo_AcceptFail,
		
		FuMo_AcceptSucc,
		
		FuMo_AcceptMaxTime,
		
		FuMo_AcceptCondition,
		
		FuMo_GiveFuMoMoneyMax,
		
		FuMo_NotFriend,
		
		FuMo_GiveFuMoMoneyActive,
		
		FuMo_GiveFuMoMoneyRepeat,
		
		FuMo_RunFuMoDBError,
		
		FuMo_GetAcceptError,
		
		FuMo_GetMailListError,
		
		FuMo_GetGoodInfo,
		
		FuMo_EquipJobDiff,
		
		FuMo_LiftRightEquipDiff,
		
		FuMo_EquipNotInGoods,
		
		FuMo_MoneyError,
		
		FuMo_FuMoAttrError,
		
		FuMo_NotFuMoType,
		
		FuMo_OtherGongNengWeiKaiQi,
		
		FuMo_GongNengWeiKaiQi,
		
		FuMo_GetRandomAttrError,
		
		FuMo_SaveError,
		
		FuMo_RoleInfoError,
		
		FuMo_DBError,
		
		FuMo_GiveOtherError,
		
		FuMo_JinBiLacking,
		
		FuMo_ZuanShiLacking
	}
}
