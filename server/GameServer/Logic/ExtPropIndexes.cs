using System;

namespace GameServer.Logic
{
	
	public enum ExtPropIndexes
	{
		
		Strong,
		
		AttackSpeed,
		
		MoveSpeed,
		
		MinDefense,
		
		MaxDefense,
		
		MinMDefense,
		
		MaxMDefense,
		
		MinAttack,
		
		MaxAttack,
		
		MinMAttack,
		
		MaxMAttack,
		
		IncreasePhyAttack,
		
		IncreaseMagAttack,
		
		MaxLifeV,
		
		MaxLifePercent,
		
		MaxMagicV,
		
		MaxMagicPercent,
		
		Lucky,
		
		HitV,
		
		Dodge,
		
		LifeRecoverPercent,
		
		MagicRecoverPercent,
		
		LifeRecover,
		
		MagicRecover,
		
		SubAttackInjurePercent,
		
		SubAttackInjure,
		
		AddAttackInjurePercent,
		
		AddAttackInjure,
		
		IgnoreDefensePercent,
		
		DamageThornPercent,
		
		DamageThorn,
		
		PhySkillIncreasePercent,
		
		PhySkillIncrease,
		
		MagicSkillIncreasePercent,
		
		MagicSkillIncrease,
		
		FatalAttack,
		
		DoubleAttack,
		
		DecreaseInjurePercent,
		
		DecreaseInjureValue,
		
		CounteractInjurePercent,
		
		CounteractInjureValue,
		
		IgnoreDefenseRate,
		
		IncreasePhyDefense,
		
		IncreaseMagDefense,
		
		LifeSteal,
		
		AddAttack,
		
		AddDefense,
		
		StateDingShen,
		
		StateMoveSpeed,
		
		StateJiTui,
		
		StateHunMi,
		
		DeLucky,
		
		DeFatalAttack,
		
		DeDoubleAttack,
		
		HitPercent,
		
		DodgePercent,
		
		FrozenPercent,
		
		PalsyPercent,
		
		SpeedDownPercent,
		
		BlowPercent,
		
		AutoRevivePercent,
		
		SavagePercent,
		
		ColdPercent,
		
		RuthlessPercent,
		
		DeSavagePercent,
		
		DeColdPercent,
		
		DeRuthlessPercent,
		
		LifeStealPercent,
		
		Potion,
		
		FireAttack,
		
		WaterAttack,
		
		LightningAttack,
		
		SoilAttack,
		
		IceAttack,
		
		WindAttack,
		
		FirePenetration,
		
		WaterPenetration,
		
		LightningPenetration,
		
		SoilPenetration,
		
		IcePenetration,
		
		WindPenetration,
		
		DeFirePenetration,
		
		DeWaterPenetration,
		
		DeLightningPenetration,
		
		DeSoilPenetration,
		
		DeIcePenetration,
		
		DeWindPenetration,
		
		Holywater,
		
		RecoverLifeV,
		
		RecoverMagicV,
		
		Fatalhurt,
		
		AddAttackPercent,
		
		AddDefensePercent,
		
		InjurePenetrationPercent,
		
		ElementInjurePercent,
		
		IgnorePhyAttackPercent,
		
		IgnoreMagyAttackPercent,
		
		DeFrozenPercent,
		
		DePalsyPercent,
		
		DeSpeedDownPercent,
		
		DeBlowPercent,
		
		Toughness,
		
		SPAttackInjurePercent,
		
		AttackInjurePercent,
		
		ElementAttackInjurePercent,
		
		WeaponEffect,
		
		FireEnhance,
		
		WaterEnhance,
		
		LightningEnhance,
		
		SoilEnhance,
		
		IceEnhance,
		
		WindEnhance,
		
		FireReduce,
		
		WaterReduce,
		
		LightningReduce,
		
		SoilReduce,
		
		IceReduce,
		
		WindReduce,
		
		ElementPenetration,
		
		ArmorMax,
		
		ArmorPercent,
		
		ArmorRecover,
		
		HolyAttack,
		
		HolyDefense,
		
		HolyPenetratePercent,
		
		HolyAbsorbPercent,
		
		HolyWeakPercent,
		
		HolyDoubleAttackPercent,
		
		HolyDoubleAttackInjure,
		
		ShadowAttack,
		
		ShadowDefense,
		
		ShadowPenetratePercent,
		
		ShadowAbsorbPercent,
		
		ShadowWeakPercent,
		
		ShadowDoubleAttackPercent,
		
		ShadowDoubleAttackInjure,
		
		NatureAttack,
		
		NatureDefense,
		
		NaturePenetratePercent,
		
		NatureAbsorbPercent,
		
		NatureWeakPercent,
		
		NatureDoubleAttackPercent,
		
		NatureDoubleAttackInjure,
		
		ChaosAttack,
		
		ChaosDefense,
		
		ChaosPenetratePercent,
		
		ChaosAbsorbPercent,
		
		ChaosWeakPercent,
		
		ChaosDoubleAttackPercent,
		
		ChaosDoubleAttackInjure,
		
		IncubusAttack,
		
		IncubusDefense,
		
		IncubusPenetratePercent,
		
		IncubusAbsorbPercent,
		
		IncubusWeakPercent,
		
		IncubusDoubleAttackPercent,
		
		IncubusDoubleAttackInjure,
		
		RebornAttack,
		
		RebornDefense,
		
		RebornPenetratePercent,
		
		RebornAbsorbPercent,
		
		RebornWeakPercent,
		
		RebornDoubleAttackPercent,
		
		RebornDoubleAttackInjure,
		
		IgnoreDamageThornPercent,
		
		HolyRebornDoubleAttackResistance,
		
		HolyRebornDoubleAttackInjureResistance,
		
		ShadowRebornDoubleAttackResistance,
		
		ShadowRebornDoubleAttackInjureResistance,
		
		NatureRebornDoubleAttackResistance,
		
		NatureRebornDoubleAttackInjureResistance,
		
		ChaosRebornDoubleAttackResistance,
		
		ChaosRebornDoubleAttackInjureResistance,
		
		IncubusRebornDoubleAttackResistance,
		
		IncubusRebornDoubleAttackInjureResistance,
		
		RebornDoubleAttackResistance,
		
		RebornDoubleAttackInjureResistance,
		
		Max,
		
		Max_Configed = 177
	}
}
