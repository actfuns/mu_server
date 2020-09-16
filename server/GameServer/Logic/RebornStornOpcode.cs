using System;

namespace GameServer.Logic
{
	
	public enum RebornStornOpcode
	{
		
		RebornHoleSucc = 1,
		
		RebornNotExist,
		
		RebornNotEquip,
		
		RebornMakeHoleErr,
		
		RebornUseMaterrislErr,
		
		RebornUpdateInfoErr,
		
		RebornNotInfo,
		
		RebornNotFindMaxQuality,
		
		RebornNotExistGoods,
		
		RebornUseMatterrislNull,
		
		RebornHoleSiteErr,
		
		RebornRandomHoleErr,
		
		RebornMaxQuality,
		
		RebornHasHole,
		
		RebornUseBind,
		
		RebornNotHasHole,
		
		RebornInlaySucc = 20,
		
		RebornInlayNotExistEquip,
		
		RebornInlayNotExistStone,
		
		RebornInlayHaveStone,
		
		RebornInlayNotEquip,
		
		RebornInlayNotMakeHole,
		
		RebornInlayNotHoleSite,
		
		RebornInlayMustEquip,
		
		RebornInlayGetInfoErr,
		
		RebornInlayNotXuanCai,
		
		RebornInlayNotChongSheng,
		
		RebornInlayUpdateInfoErr,
		
		RebornInlayCurrSiteHasStone,
		
		RebornInlayUpdateStoneInfoErr,
		
		RebornDisInlayCurrSiteNotHasXStone = 35,
		
		RebornDisInlayCurrUserNotHasXStone,
		
		RebornDisInlayCurrSiteNotHasStone,
		
		RebornDisInlayCurrUserNotHasStone,
		
		RebornDisInlayStoneInfoError,
		
		RebornDisInlayUpdateInfoErr,
		
		RebornComplexStoneNotFind = 45,
		
		RebornComplexFengYinNotEnough,
		
		RebornComplexChongShengNotEnough,
		
		RebornComplexXuanCaiNotEnough,
		
		RebornComplexNeedFengYinErr,
		
		RebornComplexNeedChongShengErr,
		
		RebornComplexNeedXuanCaiErr,
		
		RebornComplexNewStoneErr,
		
		RebornComplexStoneNotGood,
		
		RebornComplexNewStoneSucc,
		
		RebornResolveStoneSucc,
		
		RebornResolveNotFind,
		
		RebornResolveIsUsing,
		
		RebornResolveNotStone,
		
		RebornResolveDeleteErr,
		
		RebornResolveAddFengYinErr,
		
		RebornResolveAddChongShengErr,
		
		RebornResolveAddXuanCaiErr,
		
		RebornResolveStoneNotGood,
		
		RebornResolveGoodXmlErr,
		
		RebornXuanCaiComplexSucc = 70,
		
		RebornXuanCaiNotFind,
		
		RebornXuanCaiGoodXmlErr,
		
		RebornXuanCaiNotSameLevel,
		
		RebornXuanCaiMaxLevel,
		
		RebornXuanCaiUseGoodErr,
		
		RebornXuanCaiNotFindComplex,
		
		RebornXuanCaiNotUseGoodComplex,
		
		RebornXuanCaiComplexAddBagErr,
		
		RebornBatchResolveStoneSucc = 80,
		
		RebornBatchResolveAddFengYinErr,
		
		RebornBatchResolveAddChongShengErr,
		
		RebornBatchResolveAddXuanCaiErr,
		
		RebornComplexCountErr = 85,
		
		RebornResolveCountErr,
		
		RebornResolveGoodNotEnoughErr,
		
		RebornXuanCaiSuitErr = 90,
		
		RebornXuanGoodInfoErr,
		
		RebornXuanParemErr = 91,
		
		RebornNotFindBind
	}
}
