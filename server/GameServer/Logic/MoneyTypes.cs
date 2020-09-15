using System;

namespace GameServer.Logic
{
	// Token: 0x0200067B RID: 1659
	public enum MoneyTypes
	{
		// Token: 0x04003406 RID: 13318
		None,
		// Token: 0x04003407 RID: 13319
		TongQian,
		// Token: 0x04003408 RID: 13320
		YinLiang = 8,
		// Token: 0x04003409 RID: 13321
		JingYuanZhi = 13,
		// Token: 0x0400340A RID: 13322
		JunGongZhi,
		// Token: 0x0400340B RID: 13323
		ImpetratePoint,
		// Token: 0x0400340C RID: 13324
		LieShaZhi = 20,
		// Token: 0x0400340D RID: 13325
		JiFenZhi = 30,
		// Token: 0x0400340E RID: 13326
		YuanBao = 40,
		// Token: 0x0400340F RID: 13327
		YuanBaoByInput,
		// Token: 0x04003410 RID: 13328
		BindYuanBao = 50,
		// Token: 0x04003411 RID: 13329
		ZhanHun = 90,
		// Token: 0x04003412 RID: 13330
		BangGong = 95,
		// Token: 0x04003413 RID: 13331
		LangHunFenMo = 99,
		// Token: 0x04003414 RID: 13332
		PetJiFen,
		// Token: 0x04003415 RID: 13333
		ZaiZao,
		// Token: 0x04003416 RID: 13334
		TianTiRongYao,
		// Token: 0x04003417 RID: 13335
		XingHun,
		// Token: 0x04003418 RID: 13336
		ShengWang,
		// Token: 0x04003419 RID: 13337
		ChengJiu,
		// Token: 0x0400341A RID: 13338
		MUMoHe,
		// Token: 0x0400341B RID: 13339
		YuanSuFenMo,
		// Token: 0x0400341C RID: 13340
		GuardPoint,
		// Token: 0x0400341D RID: 13341
		Fluorescent,
		// Token: 0x0400341E RID: 13342
		BaoZangJiFen,
		// Token: 0x0400341F RID: 13343
		BaoZangXueZuan,
		// Token: 0x04003420 RID: 13344
		KingOfBattlePoint,
		// Token: 0x04003421 RID: 13345
		StoreTongQian,
		// Token: 0x04003422 RID: 13346
		StoreYinLiang,
		// Token: 0x04003423 RID: 13347
		Exp,
		// Token: 0x04003424 RID: 13348
		RongYu,
		// Token: 0x04003425 RID: 13349
		ZhengBaPoint,
		// Token: 0x04003426 RID: 13350
		OrnamentCharmPoint = 119,
		// Token: 0x04003427 RID: 13351
		ShenLiJingHua,
		// Token: 0x04003428 RID: 13352
		Toughness,
		// Token: 0x04003429 RID: 13353
		NengLiangSmall,
		// Token: 0x0400342A RID: 13354
		NengLiangMedium,
		// Token: 0x0400342B RID: 13355
		NengLiangBig,
		// Token: 0x0400342C RID: 13356
		NengLiangSuper,
		// Token: 0x0400342D RID: 13357
		ShenJiPoints,
		// Token: 0x0400342E RID: 13358
		ShenJiJiFen,
		// Token: 0x0400342F RID: 13359
		ShenJiJiFenAdd,
		// Token: 0x04003430 RID: 13360
		FuWenZhiChen,
		// Token: 0x04003431 RID: 13361
		AlchemyElement,
		// Token: 0x04003432 RID: 13362
		BHMatchGuessJiFen,
		// Token: 0x04003433 RID: 13363
		JueXing,
		// Token: 0x04003434 RID: 13364
		JueXingZhiChen,
		// Token: 0x04003435 RID: 13365
		EraDonate = 133,
		// Token: 0x04003436 RID: 13366
		KuaFuLueDuoEnterNum,
		// Token: 0x04003437 RID: 13367
		KuaFuLueDuoEnterNumBuyNum,
		// Token: 0x04003438 RID: 13368
		KuaFuLueDuoEnterNumDayID,
		// Token: 0x04003439 RID: 13369
		TotalLoginNum,
		// Token: 0x0400343A RID: 13370
		CompDonate,
		// Token: 0x0400343B RID: 13371
		HunJing,
		// Token: 0x0400343C RID: 13372
		MountPoint,
		// Token: 0x0400343D RID: 13373
		MoBi,
		// Token: 0x0400343E RID: 13374
		YinPiao,
		// Token: 0x0400343F RID: 13375
		CompBattleJiFen,
		// Token: 0x04003440 RID: 13376
		YuanSuJueXingShi,
		// Token: 0x04003441 RID: 13377
		CompMineJiFen,
		// Token: 0x04003442 RID: 13378
		FuMoMoney,
		// Token: 0x04003443 RID: 13379
		BianShenFreeNum,
		// Token: 0x04003444 RID: 13380
		RebornExpMonster,
		// Token: 0x04003445 RID: 13381
		RebornExpSale,
		// Token: 0x04003446 RID: 13382
		RebornExp,
		// Token: 0x04003447 RID: 13383
		RebornLevelUpPoint,
		// Token: 0x04003448 RID: 13384
		RebornCuiLian,
		// Token: 0x04003449 RID: 13385
		RebornDuanZao,
		// Token: 0x0400344A RID: 13386
		RebornNiePan,
		// Token: 0x0400344B RID: 13387
		RebornFengYin,
		// Token: 0x0400344C RID: 13388
		RebornChongSheng,
		// Token: 0x0400344D RID: 13389
		RebornXuanCai,
		// Token: 0x0400344E RID: 13390
		RebornExpMonsterMax,
		// Token: 0x0400344F RID: 13391
		RebornExpSaleMax,
		// Token: 0x04003450 RID: 13392
		TeamRongYao,
		// Token: 0x04003451 RID: 13393
		RebornEquipHole,
		// Token: 0x04003452 RID: 13394
		TeamPoint,
		// Token: 0x04003453 RID: 13395
		LuckStar
	}
}
