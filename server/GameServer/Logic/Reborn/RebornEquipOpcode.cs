using System;

namespace GameServer.Logic.Reborn
{
	
	public enum RebornEquipOpcode
	{
		
		NotExistRebornEquip = 1,
		
		NotInRebornBag,
		
		NotFindRebornLow,
		
		NotEnoughProb,
		
		NeedNiePanNotEnough,
		
		NeedDuanZaoNotEnough,
		
		NeedCuiLianNotEnough,
		
		NotHaveUpEquip,
		
		RebornLowError,
		
		RebornShowErr,
		
		RebornShowSucc,
		
		RebornNewEquipErr,
		
		RebornUpSucc,
		
		NeedNiePanErr,
		
		NeedDuanZaoErr,
		
		NeedCuiLianErr
	}
}
