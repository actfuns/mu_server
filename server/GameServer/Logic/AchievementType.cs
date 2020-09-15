using System;

namespace GameServer.Logic
{
	// Token: 0x02000674 RID: 1652
	public enum AchievementType
	{
		// Token: 0x0400338D RID: 13197
		None,
		// Token: 0x0400338E RID: 13198
		First = 10,
		// Token: 0x0400338F RID: 13199
		PlayerLevel = 20,
		// Token: 0x04003390 RID: 13200
		ShenGe = 30,
		// Token: 0x04003391 RID: 13201
		SkillLevel = 35,
		// Token: 0x04003392 RID: 13202
		LoginContinue = 40,
		// Token: 0x04003393 RID: 13203
		LoginTotal = 50,
		// Token: 0x04003394 RID: 13204
		BindGoldCoin = 60,
		// Token: 0x04003395 RID: 13205
		Monster = 70,
		// Token: 0x04003396 RID: 13206
		Boss = 80,
		// Token: 0x04003397 RID: 13207
		CopyNormal = 90,
		// Token: 0x04003398 RID: 13208
		CopyHard = 100,
		// Token: 0x04003399 RID: 13209
		CopyDifficlt = 110,
		// Token: 0x0400339A RID: 13210
		EquipForge = 120,
		// Token: 0x0400339B RID: 13211
		EquipZhuLing = 130,
		// Token: 0x0400339C RID: 13212
		Merge = 140,
		// Token: 0x0400339D RID: 13213
		ZhanGong = 200,
		// Token: 0x0400339E RID: 13214
		ShengWangLevel = 205,
		// Token: 0x0400339F RID: 13215
		MainLineTask = 210,
		// Token: 0x040033A0 RID: 13216
		Wing,
		// Token: 0x040033A1 RID: 13217
		Prestige,
		// Token: 0x040033A2 RID: 13218
		Gem,
		// Token: 0x040033A3 RID: 13219
		Shield,
		// Token: 0x040033A4 RID: 13220
		DailyTask,
		// Token: 0x040033A5 RID: 13221
		ZhanLi,
		// Token: 0x040033A6 RID: 13222
		Tower,
		// Token: 0x040033A7 RID: 13223
		GoldIngot,
		// Token: 0x040033A8 RID: 13224
		Friend,
		// Token: 0x040033A9 RID: 13225
		GuardStar,
		// Token: 0x040033AA RID: 13226
		StarUp,
		// Token: 0x040033AB RID: 13227
		ShenQi,
		// Token: 0x040033AC RID: 13228
		TuJian,
		// Token: 0x040033AD RID: 13229
		BangHuiJuanXian,
		// Token: 0x040033AE RID: 13230
		BangHuiBuy,
		// Token: 0x040033AF RID: 13231
		JiShouSale,
		// Token: 0x040033B0 RID: 13232
		JiShouBuy,
		// Token: 0x040033B1 RID: 13233
		ShenQiPart,
		// Token: 0x040033B2 RID: 13234
		GetEquipOrange,
		// Token: 0x040033B3 RID: 13235
		GetEquip3Star,
		// Token: 0x040033B4 RID: 13236
		JingJiWIN,
		// Token: 0x040033B5 RID: 13237
		TianTangDaoTime,
		// Token: 0x040033B6 RID: 13238
		JoinBangHui,
		// Token: 0x040033B7 RID: 13239
		KillEliteMonster,
		// Token: 0x040033B8 RID: 13240
		KillXueMengBoss,
		// Token: 0x040033B9 RID: 13241
		GuardUpLevel,
		// Token: 0x040033BA RID: 13242
		GuardUpStar,
		// Token: 0x040033BB RID: 13243
		WanShouChang,
		// Token: 0x040033BC RID: 13244
		Battle,
		// Token: 0x040033BD RID: 13245
		CaiJi,
		// Token: 0x040033BE RID: 13246
		ShenGeDian,
		// Token: 0x040033BF RID: 13247
		ShenGeDianBoss,
		// Token: 0x040033C0 RID: 13248
		WangLingJiTan,
		// Token: 0x040033C1 RID: 13249
		ShouHuFengYin,
		// Token: 0x040033C2 RID: 13250
		DigNum,
		// Token: 0x040033C3 RID: 13251
		LianZhan,
		// Token: 0x040033C4 RID: 13252
		XuanShang,
		// Token: 0x040033C5 RID: 13253
		CangBaoTu,
		// Token: 0x040033C6 RID: 13254
		Max,
		// Token: 0x040033C7 RID: 13255
		BuyGoods,
		// Token: 0x040033C8 RID: 13256
		MeiRiDengLu,
		// Token: 0x040033C9 RID: 13257
		XSTaskCompleteCount,
		// Token: 0x040033CA RID: 13258
		ChargeCurrentDay,
		// Token: 0x040033CB RID: 13259
		OrageGuardCount,
		// Token: 0x040033CC RID: 13260
		GatherGoods,
		// Token: 0x040033CD RID: 13261
		EnterEmoLingZhu,
		// Token: 0x040033CE RID: 13262
		ZBXuYuanCnt,
		// Token: 0x040033CF RID: 13263
		WangLingJiTan__NORMAL,
		// Token: 0x040033D0 RID: 13264
		WangLingJiTan_HARD,
		// Token: 0x040033D1 RID: 13265
		WangLingJiTan__DIFFICLT,
		// Token: 0x040033D2 RID: 13266
		GetOrangePurple,
		// Token: 0x040033D3 RID: 13267
		XuYuanCnt,
		// Token: 0x040033D4 RID: 13268
		ShouHuFengYin_NORMAL,
		// Token: 0x040033D5 RID: 13269
		ShouHuFengYin_HARD,
		// Token: 0x040033D6 RID: 13270
		GuardShopBuy,
		// Token: 0x040033D7 RID: 13271
		LongWangBaoZang__NORMAL,
		// Token: 0x040033D8 RID: 13272
		LongWangBaoZang_HARD,
		// Token: 0x040033D9 RID: 13273
		LongWangBaoZang__DIFFICLT,
		// Token: 0x040033DA RID: 13274
		TianShiTaoZhuang,
		// Token: 0x040033DB RID: 13275
		EnterJingJiChang
	}
}
