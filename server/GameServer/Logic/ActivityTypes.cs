using System;

namespace GameServer.Logic
{
	// Token: 0x02000663 RID: 1635
	public enum ActivityTypes
	{
		// Token: 0x04003127 RID: 12583
		None,
		// Token: 0x04003128 RID: 12584
		InputFirst,
		// Token: 0x04003129 RID: 12585
		InputFanLi,
		// Token: 0x0400312A RID: 12586
		InputJiaSong,
		// Token: 0x0400312B RID: 12587
		InputKing,
		// Token: 0x0400312C RID: 12588
		LevelKing,
		// Token: 0x0400312D RID: 12589
		EquipKing,
		// Token: 0x0400312E RID: 12590
		HorseKing,
		// Token: 0x0400312F RID: 12591
		JingMaiKing,
		// Token: 0x04003130 RID: 12592
		JieriDaLiBao,
		// Token: 0x04003131 RID: 12593
		JieriDengLuHaoLi,
		// Token: 0x04003132 RID: 12594
		JieriVIP,
		// Token: 0x04003133 RID: 12595
		JieriCZSong,
		// Token: 0x04003134 RID: 12596
		JieriLeiJiCZ,
		// Token: 0x04003135 RID: 12597
		JieriZiKa,
		// Token: 0x04003136 RID: 12598
		JieriPTXiaoFeiKing,
		// Token: 0x04003137 RID: 12599
		JieriPTCZKing,
		// Token: 0x04003138 RID: 12600
		JieriBossAttack,
		// Token: 0x04003139 RID: 12601
		HeFuLogin = 20,
		// Token: 0x0400313A RID: 12602
		HeFuTotalLogin,
		// Token: 0x0400313B RID: 12603
		HeFuShopLimit,
		// Token: 0x0400313C RID: 12604
		HeFuRecharge,
		// Token: 0x0400313D RID: 12605
		HeFuPKKing,
		// Token: 0x0400313E RID: 12606
		HeFuAwardTime,
		// Token: 0x0400313F RID: 12607
		HeFuBossAttack,
		// Token: 0x04003140 RID: 12608
		MeiRiChongZhiHaoLi,
		// Token: 0x04003141 RID: 12609
		ChongJiLingQuShenZhuang,
		// Token: 0x04003142 RID: 12610
		ShenZhuangJiQingHuiKui,
		// Token: 0x04003143 RID: 12611
		XinCZFanLi,
		// Token: 0x04003144 RID: 12612
		XingYunChouJiang,
		// Token: 0x04003145 RID: 12613
		YuDuZhuanPanChouJiang,
		// Token: 0x04003146 RID: 12614
		NewZoneUpLevelMadman,
		// Token: 0x04003147 RID: 12615
		NewZoneRechargeKing,
		// Token: 0x04003148 RID: 12616
		NewZoneConsumeKing,
		// Token: 0x04003149 RID: 12617
		NewZoneBosskillKing,
		// Token: 0x0400314A RID: 12618
		NewZoneFanli,
		// Token: 0x0400314B RID: 12619
		TotalCharge,
		// Token: 0x0400314C RID: 12620
		TotalConsume,
		// Token: 0x0400314D RID: 12621
		JieriTotalConsume,
		// Token: 0x0400314E RID: 12622
		JieriDuoBei,
		// Token: 0x0400314F RID: 12623
		JieriQiangGou,
		// Token: 0x04003150 RID: 12624
		HeFuLuoLan,
		// Token: 0x04003151 RID: 12625
		SpecActivity,
		// Token: 0x04003152 RID: 12626
		OneDollarBuy,
		// Token: 0x04003153 RID: 12627
		OneDollarChongZhi,
		// Token: 0x04003154 RID: 12628
		EverydayActivity,
		// Token: 0x04003155 RID: 12629
		InputFanLiNew,
		// Token: 0x04003156 RID: 12630
		SpecPriorityActivity,
		// Token: 0x04003157 RID: 12631
		JieriGive,
		// Token: 0x04003158 RID: 12632
		JieriGiveKing,
		// Token: 0x04003159 RID: 12633
		JieriRecvKing,
		// Token: 0x0400315A RID: 12634
		JieriWing,
		// Token: 0x0400315B RID: 12635
		JieriAddon,
		// Token: 0x0400315C RID: 12636
		JieriStrengthen,
		// Token: 0x0400315D RID: 12637
		JieriAchievement,
		// Token: 0x0400315E RID: 12638
		JieriMilitaryRank,
		// Token: 0x0400315F RID: 12639
		JieriVIPFanli,
		// Token: 0x04003160 RID: 12640
		JieriAmulet,
		// Token: 0x04003161 RID: 12641
		JieriArchangel,
		// Token: 0x04003162 RID: 12642
		JieriLianXuCharge,
		// Token: 0x04003163 RID: 12643
		JieriMarriage,
		// Token: 0x04003164 RID: 12644
		JieriRecv,
		// Token: 0x04003165 RID: 12645
		JieriInputPointsExchg,
		// Token: 0x04003166 RID: 12646
		JieriFuLi = 66,
		// Token: 0x04003167 RID: 12647
		JieriChongZhiQiangGou,
		// Token: 0x04003168 RID: 12648
		JieriVIPYouHui,
		// Token: 0x04003169 RID: 12649
		DanBiChongZhi,
		// Token: 0x0400316A RID: 12650
		JieRiMeiRiLeiJi,
		// Token: 0x0400316B RID: 12651
		JieriSuperInputFanLi,
		// Token: 0x0400316C RID: 12652
		JieRiHongBao,
		// Token: 0x0400316D RID: 12653
		JieRiChongZhiHongBao,
		// Token: 0x0400316E RID: 12654
		JieRiHongBaoKing,
		// Token: 0x0400316F RID: 12655
		JieRiHuiJi,
		// Token: 0x04003170 RID: 12656
		JieRiFuWen,
		// Token: 0x04003171 RID: 12657
		JieriPCKingEveryDay,
		// Token: 0x04003172 RID: 12658
		JieriPlatChargeKing = 100,
		// Token: 0x04003173 RID: 12659
		TriennialRegressOpen = 110,
		// Token: 0x04003174 RID: 12660
		TriennialRegressSignAward,
		// Token: 0x04003175 RID: 12661
		TriennialRegressTotalRechargeAward,
		// Token: 0x04003176 RID: 12662
		TriennialRegressDayBuy,
		// Token: 0x04003177 RID: 12663
		TriennialRegressStore,
		// Token: 0x04003178 RID: 12664
		ThemeZhiGou = 150,
		// Token: 0x04003179 RID: 12665
		ThemeDaLiBao,
		// Token: 0x0400317A RID: 12666
		ThemeJingYan,
		// Token: 0x0400317B RID: 12667
		ThemeSpec,
		// Token: 0x0400317C RID: 12668
		ThemeDuiHuan,
		// Token: 0x0400317D RID: 12669
		ThemeBoss,
		// Token: 0x0400317E RID: 12670
		ThemeMoYu,
		// Token: 0x0400317F RID: 12671
		ThemeZS,
		// Token: 0x04003180 RID: 12672
		TenReturn = 999,
		// Token: 0x04003181 RID: 12673
		PlatFuLiUC,
		// Token: 0x04003182 RID: 12674
		MaxVal
	}
}
