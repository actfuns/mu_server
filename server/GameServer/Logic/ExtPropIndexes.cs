using System;

namespace GameServer.Logic
{
	// Token: 0x0200062F RID: 1583
	public enum ExtPropIndexes
	{
		// Token: 0x04002D21 RID: 11553
		Strong,
		// Token: 0x04002D22 RID: 11554
		AttackSpeed,
		// Token: 0x04002D23 RID: 11555
		MoveSpeed,
		// Token: 0x04002D24 RID: 11556
		MinDefense,
		// Token: 0x04002D25 RID: 11557
		MaxDefense,
		// Token: 0x04002D26 RID: 11558
		MinMDefense,
		// Token: 0x04002D27 RID: 11559
		MaxMDefense,
		// Token: 0x04002D28 RID: 11560
		MinAttack,
		// Token: 0x04002D29 RID: 11561
		MaxAttack,
		// Token: 0x04002D2A RID: 11562
		MinMAttack,
		// Token: 0x04002D2B RID: 11563
		MaxMAttack,
		// Token: 0x04002D2C RID: 11564
		IncreasePhyAttack,
		// Token: 0x04002D2D RID: 11565
		IncreaseMagAttack,
		// Token: 0x04002D2E RID: 11566
		MaxLifeV,
		// Token: 0x04002D2F RID: 11567
		MaxLifePercent,
		// Token: 0x04002D30 RID: 11568
		MaxMagicV,
		// Token: 0x04002D31 RID: 11569
		MaxMagicPercent,
		// Token: 0x04002D32 RID: 11570
		Lucky,
		// Token: 0x04002D33 RID: 11571
		HitV,
		// Token: 0x04002D34 RID: 11572
		Dodge,
		// Token: 0x04002D35 RID: 11573
		LifeRecoverPercent,
		// Token: 0x04002D36 RID: 11574
		MagicRecoverPercent,
		// Token: 0x04002D37 RID: 11575
		LifeRecover,
		// Token: 0x04002D38 RID: 11576
		MagicRecover,
		// Token: 0x04002D39 RID: 11577
		SubAttackInjurePercent,
		// Token: 0x04002D3A RID: 11578
		SubAttackInjure,
		// Token: 0x04002D3B RID: 11579
		AddAttackInjurePercent,
		// Token: 0x04002D3C RID: 11580
		AddAttackInjure,
		// Token: 0x04002D3D RID: 11581
		IgnoreDefensePercent,
		// Token: 0x04002D3E RID: 11582
		DamageThornPercent,
		// Token: 0x04002D3F RID: 11583
		DamageThorn,
		// Token: 0x04002D40 RID: 11584
		PhySkillIncreasePercent,
		// Token: 0x04002D41 RID: 11585
		PhySkillIncrease,
		// Token: 0x04002D42 RID: 11586
		MagicSkillIncreasePercent,
		// Token: 0x04002D43 RID: 11587
		MagicSkillIncrease,
		// Token: 0x04002D44 RID: 11588
		FatalAttack,
		// Token: 0x04002D45 RID: 11589
		DoubleAttack,
		// Token: 0x04002D46 RID: 11590
		DecreaseInjurePercent,
		// Token: 0x04002D47 RID: 11591
		DecreaseInjureValue,
		// Token: 0x04002D48 RID: 11592
		CounteractInjurePercent,
		// Token: 0x04002D49 RID: 11593
		CounteractInjureValue,
		// Token: 0x04002D4A RID: 11594
		IgnoreDefenseRate,
		// Token: 0x04002D4B RID: 11595
		IncreasePhyDefense,
		// Token: 0x04002D4C RID: 11596
		IncreaseMagDefense,
		// Token: 0x04002D4D RID: 11597
		LifeSteal,
		// Token: 0x04002D4E RID: 11598
		AddAttack,
		// Token: 0x04002D4F RID: 11599
		AddDefense,
		// Token: 0x04002D50 RID: 11600
		StateDingShen,
		// Token: 0x04002D51 RID: 11601
		StateMoveSpeed,
		// Token: 0x04002D52 RID: 11602
		StateJiTui,
		// Token: 0x04002D53 RID: 11603
		StateHunMi,
		// Token: 0x04002D54 RID: 11604
		DeLucky,
		// Token: 0x04002D55 RID: 11605
		DeFatalAttack,
		// Token: 0x04002D56 RID: 11606
		DeDoubleAttack,
		// Token: 0x04002D57 RID: 11607
		HitPercent,
		// Token: 0x04002D58 RID: 11608
		DodgePercent,
		// Token: 0x04002D59 RID: 11609
		FrozenPercent,
		// Token: 0x04002D5A RID: 11610
		PalsyPercent,
		// Token: 0x04002D5B RID: 11611
		SpeedDownPercent,
		// Token: 0x04002D5C RID: 11612
		BlowPercent,
		// Token: 0x04002D5D RID: 11613
		AutoRevivePercent,
		// Token: 0x04002D5E RID: 11614
		SavagePercent,
		// Token: 0x04002D5F RID: 11615
		ColdPercent,
		// Token: 0x04002D60 RID: 11616
		RuthlessPercent,
		// Token: 0x04002D61 RID: 11617
		DeSavagePercent,
		// Token: 0x04002D62 RID: 11618
		DeColdPercent,
		// Token: 0x04002D63 RID: 11619
		DeRuthlessPercent,
		// Token: 0x04002D64 RID: 11620
		LifeStealPercent,
		// Token: 0x04002D65 RID: 11621
		Potion,
		// Token: 0x04002D66 RID: 11622
		FireAttack,
		// Token: 0x04002D67 RID: 11623
		WaterAttack,
		// Token: 0x04002D68 RID: 11624
		LightningAttack,
		// Token: 0x04002D69 RID: 11625
		SoilAttack,
		// Token: 0x04002D6A RID: 11626
		IceAttack,
		// Token: 0x04002D6B RID: 11627
		WindAttack,
		// Token: 0x04002D6C RID: 11628
		FirePenetration,
		// Token: 0x04002D6D RID: 11629
		WaterPenetration,
		// Token: 0x04002D6E RID: 11630
		LightningPenetration,
		// Token: 0x04002D6F RID: 11631
		SoilPenetration,
		// Token: 0x04002D70 RID: 11632
		IcePenetration,
		// Token: 0x04002D71 RID: 11633
		WindPenetration,
		// Token: 0x04002D72 RID: 11634
		DeFirePenetration,
		// Token: 0x04002D73 RID: 11635
		DeWaterPenetration,
		// Token: 0x04002D74 RID: 11636
		DeLightningPenetration,
		// Token: 0x04002D75 RID: 11637
		DeSoilPenetration,
		// Token: 0x04002D76 RID: 11638
		DeIcePenetration,
		// Token: 0x04002D77 RID: 11639
		DeWindPenetration,
		// Token: 0x04002D78 RID: 11640
		Holywater,
		// Token: 0x04002D79 RID: 11641
		RecoverLifeV,
		// Token: 0x04002D7A RID: 11642
		RecoverMagicV,
		// Token: 0x04002D7B RID: 11643
		Fatalhurt,
		// Token: 0x04002D7C RID: 11644
		AddAttackPercent,
		// Token: 0x04002D7D RID: 11645
		AddDefensePercent,
		// Token: 0x04002D7E RID: 11646
		InjurePenetrationPercent,
		// Token: 0x04002D7F RID: 11647
		ElementInjurePercent,
		// Token: 0x04002D80 RID: 11648
		IgnorePhyAttackPercent,
		// Token: 0x04002D81 RID: 11649
		IgnoreMagyAttackPercent,
		// Token: 0x04002D82 RID: 11650
		DeFrozenPercent,
		// Token: 0x04002D83 RID: 11651
		DePalsyPercent,
		// Token: 0x04002D84 RID: 11652
		DeSpeedDownPercent,
		// Token: 0x04002D85 RID: 11653
		DeBlowPercent,
		// Token: 0x04002D86 RID: 11654
		Toughness,
		// Token: 0x04002D87 RID: 11655
		SPAttackInjurePercent,
		// Token: 0x04002D88 RID: 11656
		AttackInjurePercent,
		// Token: 0x04002D89 RID: 11657
		ElementAttackInjurePercent,
		// Token: 0x04002D8A RID: 11658
		WeaponEffect,
		// Token: 0x04002D8B RID: 11659
		FireEnhance,
		// Token: 0x04002D8C RID: 11660
		WaterEnhance,
		// Token: 0x04002D8D RID: 11661
		LightningEnhance,
		// Token: 0x04002D8E RID: 11662
		SoilEnhance,
		// Token: 0x04002D8F RID: 11663
		IceEnhance,
		// Token: 0x04002D90 RID: 11664
		WindEnhance,
		// Token: 0x04002D91 RID: 11665
		FireReduce,
		// Token: 0x04002D92 RID: 11666
		WaterReduce,
		// Token: 0x04002D93 RID: 11667
		LightningReduce,
		// Token: 0x04002D94 RID: 11668
		SoilReduce,
		// Token: 0x04002D95 RID: 11669
		IceReduce,
		// Token: 0x04002D96 RID: 11670
		WindReduce,
		// Token: 0x04002D97 RID: 11671
		ElementPenetration,
		// Token: 0x04002D98 RID: 11672
		ArmorMax,
		// Token: 0x04002D99 RID: 11673
		ArmorPercent,
		// Token: 0x04002D9A RID: 11674
		ArmorRecover,
		// Token: 0x04002D9B RID: 11675
		HolyAttack,
		// Token: 0x04002D9C RID: 11676
		HolyDefense,
		// Token: 0x04002D9D RID: 11677
		HolyPenetratePercent,
		// Token: 0x04002D9E RID: 11678
		HolyAbsorbPercent,
		// Token: 0x04002D9F RID: 11679
		HolyWeakPercent,
		// Token: 0x04002DA0 RID: 11680
		HolyDoubleAttackPercent,
		// Token: 0x04002DA1 RID: 11681
		HolyDoubleAttackInjure,
		// Token: 0x04002DA2 RID: 11682
		ShadowAttack,
		// Token: 0x04002DA3 RID: 11683
		ShadowDefense,
		// Token: 0x04002DA4 RID: 11684
		ShadowPenetratePercent,
		// Token: 0x04002DA5 RID: 11685
		ShadowAbsorbPercent,
		// Token: 0x04002DA6 RID: 11686
		ShadowWeakPercent,
		// Token: 0x04002DA7 RID: 11687
		ShadowDoubleAttackPercent,
		// Token: 0x04002DA8 RID: 11688
		ShadowDoubleAttackInjure,
		// Token: 0x04002DA9 RID: 11689
		NatureAttack,
		// Token: 0x04002DAA RID: 11690
		NatureDefense,
		// Token: 0x04002DAB RID: 11691
		NaturePenetratePercent,
		// Token: 0x04002DAC RID: 11692
		NatureAbsorbPercent,
		// Token: 0x04002DAD RID: 11693
		NatureWeakPercent,
		// Token: 0x04002DAE RID: 11694
		NatureDoubleAttackPercent,
		// Token: 0x04002DAF RID: 11695
		NatureDoubleAttackInjure,
		// Token: 0x04002DB0 RID: 11696
		ChaosAttack,
		// Token: 0x04002DB1 RID: 11697
		ChaosDefense,
		// Token: 0x04002DB2 RID: 11698
		ChaosPenetratePercent,
		// Token: 0x04002DB3 RID: 11699
		ChaosAbsorbPercent,
		// Token: 0x04002DB4 RID: 11700
		ChaosWeakPercent,
		// Token: 0x04002DB5 RID: 11701
		ChaosDoubleAttackPercent,
		// Token: 0x04002DB6 RID: 11702
		ChaosDoubleAttackInjure,
		// Token: 0x04002DB7 RID: 11703
		IncubusAttack,
		// Token: 0x04002DB8 RID: 11704
		IncubusDefense,
		// Token: 0x04002DB9 RID: 11705
		IncubusPenetratePercent,
		// Token: 0x04002DBA RID: 11706
		IncubusAbsorbPercent,
		// Token: 0x04002DBB RID: 11707
		IncubusWeakPercent,
		// Token: 0x04002DBC RID: 11708
		IncubusDoubleAttackPercent,
		// Token: 0x04002DBD RID: 11709
		IncubusDoubleAttackInjure,
		// Token: 0x04002DBE RID: 11710
		RebornAttack,
		// Token: 0x04002DBF RID: 11711
		RebornDefense,
		// Token: 0x04002DC0 RID: 11712
		RebornPenetratePercent,
		// Token: 0x04002DC1 RID: 11713
		RebornAbsorbPercent,
		// Token: 0x04002DC2 RID: 11714
		RebornWeakPercent,
		// Token: 0x04002DC3 RID: 11715
		RebornDoubleAttackPercent,
		// Token: 0x04002DC4 RID: 11716
		RebornDoubleAttackInjure,
		// Token: 0x04002DC5 RID: 11717
		IgnoreDamageThornPercent,
		// Token: 0x04002DC6 RID: 11718
		HolyRebornDoubleAttackResistance,
		// Token: 0x04002DC7 RID: 11719
		HolyRebornDoubleAttackInjureResistance,
		// Token: 0x04002DC8 RID: 11720
		ShadowRebornDoubleAttackResistance,
		// Token: 0x04002DC9 RID: 11721
		ShadowRebornDoubleAttackInjureResistance,
		// Token: 0x04002DCA RID: 11722
		NatureRebornDoubleAttackResistance,
		// Token: 0x04002DCB RID: 11723
		NatureRebornDoubleAttackInjureResistance,
		// Token: 0x04002DCC RID: 11724
		ChaosRebornDoubleAttackResistance,
		// Token: 0x04002DCD RID: 11725
		ChaosRebornDoubleAttackInjureResistance,
		// Token: 0x04002DCE RID: 11726
		IncubusRebornDoubleAttackResistance,
		// Token: 0x04002DCF RID: 11727
		IncubusRebornDoubleAttackInjureResistance,
		// Token: 0x04002DD0 RID: 11728
		RebornDoubleAttackResistance,
		// Token: 0x04002DD1 RID: 11729
		RebornDoubleAttackInjureResistance,
		// Token: 0x04002DD2 RID: 11730
		Max,
		// Token: 0x04002DD3 RID: 11731
		Max_Configed = 177
	}
}
