using System;

namespace GameDBServer.Logic
{
	// Token: 0x020001B7 RID: 439
	public enum ActivityTypes
	{
		// Token: 0x04000A27 RID: 2599
		None,
		// Token: 0x04000A28 RID: 2600
		InputFirst,
		// Token: 0x04000A29 RID: 2601
		InputFanLi,
		// Token: 0x04000A2A RID: 2602
		InputJiaSong,
		// Token: 0x04000A2B RID: 2603
		InputKing,
		// Token: 0x04000A2C RID: 2604
		LevelKing,
		// Token: 0x04000A2D RID: 2605
		EquipKing,
		// Token: 0x04000A2E RID: 2606
		HorseKing,
		// Token: 0x04000A2F RID: 2607
		JingMaiKing,
		// Token: 0x04000A30 RID: 2608
		JieriDaLiBao,
		// Token: 0x04000A31 RID: 2609
		JieriDengLuHaoLi,
		// Token: 0x04000A32 RID: 2610
		JieriVIP,
		// Token: 0x04000A33 RID: 2611
		JieriCZSong,
		// Token: 0x04000A34 RID: 2612
		JieriLeiJiCZ,
		// Token: 0x04000A35 RID: 2613
		JieriZiKa,
		// Token: 0x04000A36 RID: 2614
		JieriPTXiaoFeiKing,
		// Token: 0x04000A37 RID: 2615
		JieriPTCZKing,
		// Token: 0x04000A38 RID: 2616
		JieriBossAttack,
		// Token: 0x04000A39 RID: 2617
		HeFuLogin = 20,
		// Token: 0x04000A3A RID: 2618
		HeFuTotalLogin,
		// Token: 0x04000A3B RID: 2619
		HeFuShopLimit,
		// Token: 0x04000A3C RID: 2620
		HeFuRecharge,
		// Token: 0x04000A3D RID: 2621
		HeFuPKKing,
		// Token: 0x04000A3E RID: 2622
		HeFuAwardTime,
		// Token: 0x04000A3F RID: 2623
		HeFuBossAttack,
		// Token: 0x04000A40 RID: 2624
		MeiRiChongZhiHaoLi,
		// Token: 0x04000A41 RID: 2625
		ChongJiLingQuShenZhuang,
		// Token: 0x04000A42 RID: 2626
		ShenZhuangJiQingHuiKui,
		// Token: 0x04000A43 RID: 2627
		XinCZFanLi,
		// Token: 0x04000A44 RID: 2628
		XingYunChouJiang,
		// Token: 0x04000A45 RID: 2629
		YuDuZhuanPanChouJiang,
		// Token: 0x04000A46 RID: 2630
		NewZoneUpLevelMadman,
		// Token: 0x04000A47 RID: 2631
		NewZoneRechargeKing,
		// Token: 0x04000A48 RID: 2632
		NewZoneConsumeKing,
		// Token: 0x04000A49 RID: 2633
		NewZoneBosskillKing,
		// Token: 0x04000A4A RID: 2634
		NewZoneFanli,
		// Token: 0x04000A4B RID: 2635
		TotalCharge,
		// Token: 0x04000A4C RID: 2636
		TotalConsume,
		// Token: 0x04000A4D RID: 2637
		JieriTotalConsume,
		// Token: 0x04000A4E RID: 2638
		JieriDuoBei,
		// Token: 0x04000A4F RID: 2639
		JieriQiangGou,
		// Token: 0x04000A50 RID: 2640
		HeFuLuoLan,
		// Token: 0x04000A51 RID: 2641
		SpecActivity,
		// Token: 0x04000A52 RID: 2642
		OneDollarBuy,
		// Token: 0x04000A53 RID: 2643
		OneDollarChongZhi,
		// Token: 0x04000A54 RID: 2644
		EverydayActivity,
		// Token: 0x04000A55 RID: 2645
		InputFanLiNew,
		// Token: 0x04000A56 RID: 2646
		SpecPriorityActivity,
		// Token: 0x04000A57 RID: 2647
		JieriGive,
		// Token: 0x04000A58 RID: 2648
		JieriGiveKing,
		// Token: 0x04000A59 RID: 2649
		JieriRecvKing,
		// Token: 0x04000A5A RID: 2650
		JieriWing,
		// Token: 0x04000A5B RID: 2651
		JieriAddon,
		// Token: 0x04000A5C RID: 2652
		JieriStrengthen,
		// Token: 0x04000A5D RID: 2653
		JieriAchievement,
		// Token: 0x04000A5E RID: 2654
		JieriMilitaryRank,
		// Token: 0x04000A5F RID: 2655
		JieriVIPFanli,
		// Token: 0x04000A60 RID: 2656
		JieriAmulet,
		// Token: 0x04000A61 RID: 2657
		JieriArchangel,
		// Token: 0x04000A62 RID: 2658
		JieriLianXuCharge,
		// Token: 0x04000A63 RID: 2659
		JieriMarriage,
		// Token: 0x04000A64 RID: 2660
		JieriRecv,
		// Token: 0x04000A65 RID: 2661
		JieriInputPointsExchg,
		// Token: 0x04000A66 RID: 2662
		JieriFuLi = 66,
		// Token: 0x04000A67 RID: 2663
		JieriChongZhiQiangGou,
		// Token: 0x04000A68 RID: 2664
		JieriVIPYouHui,
		// Token: 0x04000A69 RID: 2665
		DanBiChongZhi,
		// Token: 0x04000A6A RID: 2666
		JieRiMeiRiLeiJi,
		// Token: 0x04000A6B RID: 2667
		JieriSuperInputFanLi,
		// Token: 0x04000A6C RID: 2668
		JieRiHongBao,
		// Token: 0x04000A6D RID: 2669
		JieRiChongZhiHongBao,
		// Token: 0x04000A6E RID: 2670
		JieRiHongBaoKing,
		// Token: 0x04000A6F RID: 2671
		JieRiHuiJi,
		// Token: 0x04000A70 RID: 2672
		JieRiFuWen,
		// Token: 0x04000A71 RID: 2673
		JieriPCKingEveryDay,
		// Token: 0x04000A72 RID: 2674
		JieriPlatChargeKing = 100,
		// Token: 0x04000A73 RID: 2675
		TriennialRegressOpen = 110,
		// Token: 0x04000A74 RID: 2676
		TriennialRegressSignAward,
		// Token: 0x04000A75 RID: 2677
		TriennialRegressTotalRechargeAward,
		// Token: 0x04000A76 RID: 2678
		TriennialRegressDayBuy,
		// Token: 0x04000A77 RID: 2679
		TriennialRegressStore,
		// Token: 0x04000A78 RID: 2680
		ThemeZhiGou = 150,
		// Token: 0x04000A79 RID: 2681
		ThemeDaLiBao,
		// Token: 0x04000A7A RID: 2682
		ThemeJingYan,
		// Token: 0x04000A7B RID: 2683
		ThemeSpec,
		// Token: 0x04000A7C RID: 2684
		ThemeDuiHuan,
		// Token: 0x04000A7D RID: 2685
		ThemeBoss,
		// Token: 0x04000A7E RID: 2686
		ThemeMoYu,
		// Token: 0x04000A7F RID: 2687
		ThemeZS,
		// Token: 0x04000A80 RID: 2688
		TenReturn = 999,
		// Token: 0x04000A81 RID: 2689
		MaxVal
	}
}
