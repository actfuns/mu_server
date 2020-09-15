using System;

namespace GameServer.Logic
{
	// Token: 0x020002A5 RID: 677
	internal enum RoleEvent
	{
		// Token: 0x040010AB RID: 4267
		Login,
		// Token: 0x040010AC RID: 4268
		InitGame,
		// Token: 0x040010AD RID: 4269
		Logout,
		// Token: 0x040010AE RID: 4270
		CreateRole,
		// Token: 0x040010AF RID: 4271
		RemoveRole,
		// Token: 0x040010B0 RID: 4272
		Upgrade,
		// Token: 0x040010B1 RID: 4273
		RoleDeath,
		// Token: 0x040010B2 RID: 4274
		RebornUpgrade,
		// Token: 0x040010B3 RID: 4275
		Resource,
		// Token: 0x040010B4 RID: 4276
		UserMoney,
		// Token: 0x040010B5 RID: 4277
		YinLiang,
		// Token: 0x040010B6 RID: 4278
		TongQian,
		// Token: 0x040010B7 RID: 4279
		BindYuanBao,
		// Token: 0x040010B8 RID: 4280
		ShengWang,
		// Token: 0x040010B9 RID: 4281
		WingStar,
		// Token: 0x040010BA RID: 4282
		WingUpgrade,
		// Token: 0x040010BB RID: 4283
		BossDied,
		// Token: 0x040010BC RID: 4284
		Purchase,
		// Token: 0x040010BD RID: 4285
		Title,
		// Token: 0x040010BE RID: 4286
		ChangeLife,
		// Token: 0x040010BF RID: 4287
		Ranking,
		// Token: 0x040010C0 RID: 4288
		JieriCZSong,
		// Token: 0x040010C1 RID: 4289
		JieRiMeiRiLeiJi,
		// Token: 0x040010C2 RID: 4290
		JieriLeiJiCZ,
		// Token: 0x040010C3 RID: 4291
		LingYuLev,
		// Token: 0x040010C4 RID: 4292
		LingYuSuit,
		// Token: 0x040010C5 RID: 4293
		WingZhuLing,
		// Token: 0x040010C6 RID: 4294
		WingZhuHun,
		// Token: 0x040010C7 RID: 4295
		ChengJiu,
		// Token: 0x040010C8 RID: 4296
		GuardStatueSuit,
		// Token: 0x040010C9 RID: 4297
		RingStarSuit,
		// Token: 0x040010CA RID: 4298
		RingBuy,
		// Token: 0x040010CB RID: 4299
		MerlinBookStar,
		// Token: 0x040010CC RID: 4300
		MerlinBookLev,
		// Token: 0x040010CD RID: 4301
		FallGoods,
		// Token: 0x040010CE RID: 4302
		BangHuiBuildUp,
		// Token: 0x040010CF RID: 4303
		BangHuiCreate,
		// Token: 0x040010D0 RID: 4304
		BangHuiDestroy,
		// Token: 0x040010D1 RID: 4305
		BangHuiQuit,
		// Token: 0x040010D2 RID: 4306
		LuoLanChengZhan,
		// Token: 0x040010D3 RID: 4307
		JunTuanCreate,
		// Token: 0x040010D4 RID: 4308
		JunTuanZhiWu,
		// Token: 0x040010D5 RID: 4309
		JunTuanDestroy,
		// Token: 0x040010D6 RID: 4310
		KarenBattle,
		// Token: 0x040010D7 RID: 4311
		KarenBattleEnter,
		// Token: 0x040010D8 RID: 4312
		LangHunLingYu,
		// Token: 0x040010D9 RID: 4313
		AchievementRune,
		// Token: 0x040010DA RID: 4314
		ChangeOccupation,
		// Token: 0x040010DB RID: 4315
		HuiJi,
		// Token: 0x040010DC RID: 4316
		Meditate,
		// Token: 0x040010DD RID: 4317
		QiFu,
		// Token: 0x040010DE RID: 4318
		Armor,
		// Token: 0x040010DF RID: 4319
		BianShen,
		// Token: 0x040010E0 RID: 4320
		Reborn,
		// Token: 0x040010E1 RID: 4321
		MazingerStore,
		// Token: 0x040010E2 RID: 4322
		KF5v5CreateZhanDui,
		// Token: 0x040010E3 RID: 4323
		KF5v5AttendZhanDui,
		// Token: 0x040010E4 RID: 4324
		KF5v5QuitZhanDui,
		// Token: 0x040010E5 RID: 4325
		KF5v5DeleteZhanDui,
		// Token: 0x040010E6 RID: 4326
		KF5v5ChangeZhanDuiLeader,
		// Token: 0x040010E7 RID: 4327
		KF5v5YaZhu,
		// Token: 0x040010E8 RID: 4328
		KF5v5YaZhuResult,
		// Token: 0x040010E9 RID: 4329
		KF5v5PiPeiSuccess,
		// Token: 0x040010EA RID: 4330
		KF5v5Enter64Qiang,
		// Token: 0x040010EB RID: 4331
		KF5v5DailyAward,
		// Token: 0x040010EC RID: 4332
		KF5v5MonthAward,
		// Token: 0x040010ED RID: 4333
		KF5v5NotifyEnter64Qiang,
		// Token: 0x040010EE RID: 4334
		KF5v5JinJi64QiangAward,
		// Token: 0x040010EF RID: 4335
		KF5v5FreeBattle,
		// Token: 0x040010F0 RID: 4336
		KF5v5MonthChengJiuAward,
		// Token: 0x040010F1 RID: 4337
		KF5v5Mail,
		// Token: 0x040010F2 RID: 4338
		OldPlayer,
		// Token: 0x040010F3 RID: 4339
		OldPlayerAward,
		// Token: 0x040010F4 RID: 4340
		MoneyEvent,
		// Token: 0x040010F5 RID: 4341
		GoodsEvent,
		// Token: 0x040010F6 RID: 4342
		GameEvent,
		// Token: 0x040010F7 RID: 4343
		OperatorEvent,
		// Token: 0x040010F8 RID: 4344
		RoleSkill,
		// Token: 0x040010F9 RID: 4345
		PetSkill,
		// Token: 0x040010FA RID: 4346
		UnionPalace,
		// Token: 0x040010FB RID: 4347
		NewGiftCode,
		// Token: 0x040010FC RID: 4348
		WriteMax,
		// Token: 0x040010FD RID: 4349
		EventMax
	}
}
