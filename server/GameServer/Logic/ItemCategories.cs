using System;

namespace GameServer.Logic
{
	// Token: 0x02000631 RID: 1585
	public enum ItemCategories
	{
		// Token: 0x04002DFD RID: 11773
		ItemTask = 50,
		// Token: 0x04002DFE RID: 11774
		ItemHorsePet = 60,
		// Token: 0x04002DFF RID: 11775
		ItemBook = 70,
		// Token: 0x04002E00 RID: 11776
		ItemOther = 80,
		// Token: 0x04002E01 RID: 11777
		ItemJewel = 90,
		// Token: 0x04002E02 RID: 11778
		ItemMagic = 100,
		// Token: 0x04002E03 RID: 11779
		ItemMakings = 110,
		// Token: 0x04002E04 RID: 11780
		ItemMaterial = 120,
		// Token: 0x04002E05 RID: 11781
		ItemLargeGridNum,
		// Token: 0x04002E06 RID: 11782
		BaoXiang_GongXiHuoDe,
		// Token: 0x04002E07 RID: 11783
		ItemDrug = 180,
		// Token: 0x04002E08 RID: 11784
		ItemAddVal = 230,
		// Token: 0x04002E09 RID: 11785
		ItemBuffer = 250,
		// Token: 0x04002E0A RID: 11786
		JingMai,
		// Token: 0x04002E0B RID: 11787
		WuXue,
		// Token: 0x04002E0C RID: 11788
		ChengJiu,
		// Token: 0x04002E0D RID: 11789
		RongYuHuTi = 255,
		// Token: 0x04002E0E RID: 11790
		ZhanHunHuTi,
		// Token: 0x04002E0F RID: 11791
		BangQiHuTi = 258,
		// Token: 0x04002E10 RID: 11792
		ItemNormalPack = 301,
		// Token: 0x04002E11 RID: 11793
		ItemUpPack,
		// Token: 0x04002E12 RID: 11794
		YinLiangPack = 401,
		// Token: 0x04002E13 RID: 11795
		MoneyPack = 501,
		// Token: 0x04002E14 RID: 11796
		BindMoneyFu = 601,
		// Token: 0x04002E15 RID: 11797
		TreasureBox = 701,
		// Token: 0x04002E16 RID: 11798
		ChenHaoEquip,
		// Token: 0x04002E17 RID: 11799
		TouKui = 0,
		// Token: 0x04002E18 RID: 11800
		KaiJia,
		// Token: 0x04002E19 RID: 11801
		HuShou,
		// Token: 0x04002E1A RID: 11802
		HuTui,
		// Token: 0x04002E1B RID: 11803
		XueZi,
		// Token: 0x04002E1C RID: 11804
		XiangLian,
		// Token: 0x04002E1D RID: 11805
		JieZhi,
		// Token: 0x04002E1E RID: 11806
		ZuoJi,
		// Token: 0x04002E1F RID: 11807
		ChiBang,
		// Token: 0x04002E20 RID: 11808
		ShouHuChong,
		// Token: 0x04002E21 RID: 11809
		ChongWu,
		// Token: 0x04002E22 RID: 11810
		WuQi_Jian,
		// Token: 0x04002E23 RID: 11811
		WuQi_Fu,
		// Token: 0x04002E24 RID: 11812
		WuQi_Chui,
		// Token: 0x04002E25 RID: 11813
		WuQi_Gong,
		// Token: 0x04002E26 RID: 11814
		WuQi_Nu,
		// Token: 0x04002E27 RID: 11815
		WuQi_Mao,
		// Token: 0x04002E28 RID: 11816
		WuQi_Zhang,
		// Token: 0x04002E29 RID: 11817
		WuQi_Dun,
		// Token: 0x04002E2A RID: 11818
		WuQi_Dao,
		// Token: 0x04002E2B RID: 11819
		WuQi_GongJianTong,
		// Token: 0x04002E2C RID: 11820
		WuQi_NuJianTong,
		// Token: 0x04002E2D RID: 11821
		HuFu,
		// Token: 0x04002E2E RID: 11822
		HuFu_2,
		// Token: 0x04002E2F RID: 11823
		ShiZhuang,
		// Token: 0x04002E30 RID: 11824
		ShiZhuang_WuQi,
		// Token: 0x04002E31 RID: 11825
		ShiZhuang_JiaoYin,
		// Token: 0x04002E32 RID: 11826
		ShiZhuang_ZuoQi,
		// Token: 0x04002E33 RID: 11827
		ShiZhuang_BianShen,
		// Token: 0x04002E34 RID: 11828
		TouKui_ChongSheng = 30,
		// Token: 0x04002E35 RID: 11829
		KaiJia_ChongSheng,
		// Token: 0x04002E36 RID: 11830
		HuShou_ChongSheng,
		// Token: 0x04002E37 RID: 11831
		HuTui_ChongSheng,
		// Token: 0x04002E38 RID: 11832
		XueZi_ChongSheng,
		// Token: 0x04002E39 RID: 11833
		XiangLian_ChongSheng,
		// Token: 0x04002E3A RID: 11834
		JieZhi_ChongSheng,
		// Token: 0x04002E3B RID: 11835
		ShengWu_ChongSheng,
		// Token: 0x04002E3C RID: 11836
		ShengQi_ChongSheng,
		// Token: 0x04002E3D RID: 11837
		ZuoQi_MaBian = 40,
		// Token: 0x04002E3E RID: 11838
		ZuoQi_HuJu,
		// Token: 0x04002E3F RID: 11839
		ZuoQi_JiangSheng,
		// Token: 0x04002E40 RID: 11840
		ZuoQi_MaAn,
		// Token: 0x04002E41 RID: 11841
		ZuoQi_MaCi,
		// Token: 0x04002E42 RID: 11842
		ZuoQi_MaZhang,
		// Token: 0x04002E43 RID: 11843
		EquipMax = 49,
		// Token: 0x04002E44 RID: 11844
		TarotCard = 703,
		// Token: 0x04002E45 RID: 11845
		ElementHrtBegin = 800,
		// Token: 0x04002E46 RID: 11846
		SpecialElementHrt = 810,
		// Token: 0x04002E47 RID: 11847
		ElementHrtEnd = 816,
		// Token: 0x04002E48 RID: 11848
		FluorescentGem = 901,
		// Token: 0x04002E49 RID: 11849
		SoulStoneJingHua = 910,
		// Token: 0x04002E4A RID: 11850
		SoulStoneFire,
		// Token: 0x04002E4B RID: 11851
		SoulStoneThunder,
		// Token: 0x04002E4C RID: 11852
		SoulStoneWind,
		// Token: 0x04002E4D RID: 11853
		SoulStoneWater,
		// Token: 0x04002E4E RID: 11854
		SoulStoneIce,
		// Token: 0x04002E4F RID: 11855
		SoulStoneSoil,
		// Token: 0x04002E50 RID: 11856
		SoulStoneLight,
		// Token: 0x04002E51 RID: 11857
		SoulStonePower,
		// Token: 0x04002E52 RID: 11858
		SoulStonePole,
		// Token: 0x04002E53 RID: 11859
		SoulStoneCold,
		// Token: 0x04002E54 RID: 11860
		SoulStoneFrost,
		// Token: 0x04002E55 RID: 11861
		SoulStoneHot,
		// Token: 0x04002E56 RID: 11862
		SoulStoneExplode,
		// Token: 0x04002E57 RID: 11863
		SoulStoneCloud,
		// Token: 0x04002E58 RID: 11864
		SoulStoneRomantic,
		// Token: 0x04002E59 RID: 11865
		SoulStoneSnow,
		// Token: 0x04002E5A RID: 11866
		SoulStoneSeal,
		// Token: 0x04002E5B RID: 11867
		SoulStoneRed,
		// Token: 0x04002E5C RID: 11868
		ShenShiFuWen = 940,
		// Token: 0x04002E5D RID: 11869
		JueXingSuiPian = 330,
		// Token: 0x04002E5E RID: 11870
		ZuoQi = 340,
		// Token: 0x04002E5F RID: 11871
		RebornBaoShi = 950,
		// Token: 0x04002E60 RID: 11872
		RebornXuanCaiBaoShi = 960,
		// Token: 0x04002E61 RID: 11873
		MountHolyStamp = 980,
		// Token: 0x04002E62 RID: 11874
		MountHolyStampTun
	}
}
