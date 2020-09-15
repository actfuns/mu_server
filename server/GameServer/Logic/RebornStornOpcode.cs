using System;

namespace GameServer.Logic
{
	// Token: 0x020003D8 RID: 984
	public enum RebornStornOpcode
	{
		// Token: 0x040019D7 RID: 6615
		RebornHoleSucc = 1,
		// Token: 0x040019D8 RID: 6616
		RebornNotExist,
		// Token: 0x040019D9 RID: 6617
		RebornNotEquip,
		// Token: 0x040019DA RID: 6618
		RebornMakeHoleErr,
		// Token: 0x040019DB RID: 6619
		RebornUseMaterrislErr,
		// Token: 0x040019DC RID: 6620
		RebornUpdateInfoErr,
		// Token: 0x040019DD RID: 6621
		RebornNotInfo,
		// Token: 0x040019DE RID: 6622
		RebornNotFindMaxQuality,
		// Token: 0x040019DF RID: 6623
		RebornNotExistGoods,
		// Token: 0x040019E0 RID: 6624
		RebornUseMatterrislNull,
		// Token: 0x040019E1 RID: 6625
		RebornHoleSiteErr,
		// Token: 0x040019E2 RID: 6626
		RebornRandomHoleErr,
		// Token: 0x040019E3 RID: 6627
		RebornMaxQuality,
		// Token: 0x040019E4 RID: 6628
		RebornHasHole,
		// Token: 0x040019E5 RID: 6629
		RebornUseBind,
		// Token: 0x040019E6 RID: 6630
		RebornNotHasHole,
		// Token: 0x040019E7 RID: 6631
		RebornInlaySucc = 20,
		// Token: 0x040019E8 RID: 6632
		RebornInlayNotExistEquip,
		// Token: 0x040019E9 RID: 6633
		RebornInlayNotExistStone,
		// Token: 0x040019EA RID: 6634
		RebornInlayHaveStone,
		// Token: 0x040019EB RID: 6635
		RebornInlayNotEquip,
		// Token: 0x040019EC RID: 6636
		RebornInlayNotMakeHole,
		// Token: 0x040019ED RID: 6637
		RebornInlayNotHoleSite,
		// Token: 0x040019EE RID: 6638
		RebornInlayMustEquip,
		// Token: 0x040019EF RID: 6639
		RebornInlayGetInfoErr,
		// Token: 0x040019F0 RID: 6640
		RebornInlayNotXuanCai,
		// Token: 0x040019F1 RID: 6641
		RebornInlayNotChongSheng,
		// Token: 0x040019F2 RID: 6642
		RebornInlayUpdateInfoErr,
		// Token: 0x040019F3 RID: 6643
		RebornInlayCurrSiteHasStone,
		// Token: 0x040019F4 RID: 6644
		RebornInlayUpdateStoneInfoErr,
		// Token: 0x040019F5 RID: 6645
		RebornDisInlayCurrSiteNotHasXStone = 35,
		// Token: 0x040019F6 RID: 6646
		RebornDisInlayCurrUserNotHasXStone,
		// Token: 0x040019F7 RID: 6647
		RebornDisInlayCurrSiteNotHasStone,
		// Token: 0x040019F8 RID: 6648
		RebornDisInlayCurrUserNotHasStone,
		// Token: 0x040019F9 RID: 6649
		RebornDisInlayStoneInfoError,
		// Token: 0x040019FA RID: 6650
		RebornDisInlayUpdateInfoErr,
		// Token: 0x040019FB RID: 6651
		RebornComplexStoneNotFind = 45,
		// Token: 0x040019FC RID: 6652
		RebornComplexFengYinNotEnough,
		// Token: 0x040019FD RID: 6653
		RebornComplexChongShengNotEnough,
		// Token: 0x040019FE RID: 6654
		RebornComplexXuanCaiNotEnough,
		// Token: 0x040019FF RID: 6655
		RebornComplexNeedFengYinErr,
		// Token: 0x04001A00 RID: 6656
		RebornComplexNeedChongShengErr,
		// Token: 0x04001A01 RID: 6657
		RebornComplexNeedXuanCaiErr,
		// Token: 0x04001A02 RID: 6658
		RebornComplexNewStoneErr,
		// Token: 0x04001A03 RID: 6659
		RebornComplexStoneNotGood,
		// Token: 0x04001A04 RID: 6660
		RebornComplexNewStoneSucc,
		// Token: 0x04001A05 RID: 6661
		RebornResolveStoneSucc,
		// Token: 0x04001A06 RID: 6662
		RebornResolveNotFind,
		// Token: 0x04001A07 RID: 6663
		RebornResolveIsUsing,
		// Token: 0x04001A08 RID: 6664
		RebornResolveNotStone,
		// Token: 0x04001A09 RID: 6665
		RebornResolveDeleteErr,
		// Token: 0x04001A0A RID: 6666
		RebornResolveAddFengYinErr,
		// Token: 0x04001A0B RID: 6667
		RebornResolveAddChongShengErr,
		// Token: 0x04001A0C RID: 6668
		RebornResolveAddXuanCaiErr,
		// Token: 0x04001A0D RID: 6669
		RebornResolveStoneNotGood,
		// Token: 0x04001A0E RID: 6670
		RebornResolveGoodXmlErr,
		// Token: 0x04001A0F RID: 6671
		RebornXuanCaiComplexSucc = 70,
		// Token: 0x04001A10 RID: 6672
		RebornXuanCaiNotFind,
		// Token: 0x04001A11 RID: 6673
		RebornXuanCaiGoodXmlErr,
		// Token: 0x04001A12 RID: 6674
		RebornXuanCaiNotSameLevel,
		// Token: 0x04001A13 RID: 6675
		RebornXuanCaiMaxLevel,
		// Token: 0x04001A14 RID: 6676
		RebornXuanCaiUseGoodErr,
		// Token: 0x04001A15 RID: 6677
		RebornXuanCaiNotFindComplex,
		// Token: 0x04001A16 RID: 6678
		RebornXuanCaiNotUseGoodComplex,
		// Token: 0x04001A17 RID: 6679
		RebornXuanCaiComplexAddBagErr,
		// Token: 0x04001A18 RID: 6680
		RebornBatchResolveStoneSucc = 80,
		// Token: 0x04001A19 RID: 6681
		RebornBatchResolveAddFengYinErr,
		// Token: 0x04001A1A RID: 6682
		RebornBatchResolveAddChongShengErr,
		// Token: 0x04001A1B RID: 6683
		RebornBatchResolveAddXuanCaiErr,
		// Token: 0x04001A1C RID: 6684
		RebornComplexCountErr = 85,
		// Token: 0x04001A1D RID: 6685
		RebornResolveCountErr,
		// Token: 0x04001A1E RID: 6686
		RebornResolveGoodNotEnoughErr,
		// Token: 0x04001A1F RID: 6687
		RebornXuanCaiSuitErr = 90,
		// Token: 0x04001A20 RID: 6688
		RebornXuanGoodInfoErr,
		// Token: 0x04001A21 RID: 6689
		RebornXuanParemErr = 91,
		// Token: 0x04001A22 RID: 6690
		RebornNotFindBind
	}
}
