using System;

namespace GameServer.Logic
{
	// Token: 0x0200064E RID: 1614
	public enum BufferItemTypes
	{
		// Token: 0x04002F61 RID: 12129
		None,
		// Token: 0x04002F62 RID: 12130
		DblExperience,
		// Token: 0x04002F63 RID: 12131
		DblMoney,
		// Token: 0x04002F64 RID: 12132
		DblLingLi,
		// Token: 0x04002F65 RID: 12133
		LifeVReserve,
		// Token: 0x04002F66 RID: 12134
		MagicVReserve,
		// Token: 0x04002F67 RID: 12135
		AddTempAttack,
		// Token: 0x04002F68 RID: 12136
		AddTempDefense,
		// Token: 0x04002F69 RID: 12137
		UpLifeLimit,
		// Token: 0x04002F6A RID: 12138
		UpMagicLimit,
		// Token: 0x04002F6B RID: 12139
		LingLiVReserve,
		// Token: 0x04002F6C RID: 12140
		AntiBoss,
		// Token: 0x04002F6D RID: 12141
		AntiRole,
		// Token: 0x04002F6E RID: 12142
		MonthVIP,
		// Token: 0x04002F6F RID: 12143
		SheLiZhiYuan,
		// Token: 0x04002F70 RID: 12144
		DiWanZhiYou,
		// Token: 0x04002F71 RID: 12145
		JunQi,
		// Token: 0x04002F72 RID: 12146
		DblSkillUp,
		// Token: 0x04002F73 RID: 12147
		ThreeExperience,
		// Token: 0x04002F74 RID: 12148
		ThreeMoney,
		// Token: 0x04002F75 RID: 12149
		AutoFightingProtect,
		// Token: 0x04002F76 RID: 12150
		TimeExp,
		// Token: 0x04002F77 RID: 12151
		TimeAddDefense,
		// Token: 0x04002F78 RID: 12152
		TimeAddMDefense,
		// Token: 0x04002F79 RID: 12153
		TimeAddAttack,
		// Token: 0x04002F7A RID: 12154
		TimeAddMAttack,
		// Token: 0x04002F7B RID: 12155
		TimeAddDSAttack,
		// Token: 0x04002F7C RID: 12156
		TimeAddLifeMagic,
		// Token: 0x04002F7D RID: 12157
		WaWaExp,
		// Token: 0x04002F7E RID: 12158
		ZhuFu,
		// Token: 0x04002F7F RID: 12159
		FallTianSheng,
		// Token: 0x04002F80 RID: 12160
		ChengJiu,
		// Token: 0x04002F81 RID: 12161
		JingMai,
		// Token: 0x04002F82 RID: 12162
		WuXue,
		// Token: 0x04002F83 RID: 12163
		GuMuTimeLimit,
		// Token: 0x04002F84 RID: 12164
		MingJieMapLimit,
		// Token: 0x04002F85 RID: 12165
		FiveExperience,
		// Token: 0x04002F86 RID: 12166
		TimeAddLifeNoShow,
		// Token: 0x04002F87 RID: 12167
		TimeAddMagicNoShow,
		// Token: 0x04002F88 RID: 12168
		PKKingBuffer,
		// Token: 0x04002F89 RID: 12169
		DSTimeAddLifeNoShow,
		// Token: 0x04002F8A RID: 12170
		DSTimeHideNoShow,
		// Token: 0x04002F8B RID: 12171
		DSTimeShiDuNoShow,
		// Token: 0x04002F8C RID: 12172
		DSTimeAddDefenseNoShow,
		// Token: 0x04002F8D RID: 12173
		DSTimeAddMDefenseNoShow,
		// Token: 0x04002F8E RID: 12174
		FSAddHuDunNoShow,
		// Token: 0x04002F8F RID: 12175
		MutilExperience,
		// Token: 0x04002F90 RID: 12176
		JieRiChengHao,
		// Token: 0x04002F91 RID: 12177
		ErGuoTou,
		// Token: 0x04002F92 RID: 12178
		ZuanHuang,
		// Token: 0x04002F93 RID: 12179
		ZhanHun,
		// Token: 0x04002F94 RID: 12180
		RongYu,
		// Token: 0x04002F95 RID: 12181
		ADDTEMPStrength,
		// Token: 0x04002F96 RID: 12182
		ADDTEMPIntelligsence,
		// Token: 0x04002F97 RID: 12183
		ADDTEMPDexterity,
		// Token: 0x04002F98 RID: 12184
		ADDTEMPConstitution,
		// Token: 0x04002F99 RID: 12185
		ADDTEMPATTACKSPEED,
		// Token: 0x04002F9A RID: 12186
		ADDTEMPLUCKYATTACK,
		// Token: 0x04002F9B RID: 12187
		ADDTEMPFATALATTACK,
		// Token: 0x04002F9C RID: 12188
		ADDTEMPDOUBLEATTACK,
		// Token: 0x04002F9D RID: 12189
		MU_SUBDAMAGEPERCENTTIMER,
		// Token: 0x04002F9E RID: 12190
		MU_MAXLIFEPERCENT,
		// Token: 0x04002F9F RID: 12191
		MU_ADDDEFENSETIMER,
		// Token: 0x04002FA0 RID: 12192
		MU_ADDATTACKTIMER,
		// Token: 0x04002FA1 RID: 12193
		MU_ADDLUCKYATTACKPERCENTTIMER,
		// Token: 0x04002FA2 RID: 12194
		MU_ADDFATALATTACKPERCENTTIMER,
		// Token: 0x04002FA3 RID: 12195
		MU_ADDDOUBLEATTACKPERCENTTIMER,
		// Token: 0x04002FA4 RID: 12196
		MU_ADDMAXHPVALUE,
		// Token: 0x04002FA5 RID: 12197
		MU_ADDMAXMPVALUE,
		// Token: 0x04002FA6 RID: 12198
		MU_ADDLIFERECOVERPERCENT,
		// Token: 0x04002FA7 RID: 12199
		MU_FRESHPLAYERBUFF,
		// Token: 0x04002FA8 RID: 12200
		MU_SUBDAMAGEPERCENTTIMER1,
		// Token: 0x04002FA9 RID: 12201
		MU_SUBATTACKPERCENTTIMER,
		// Token: 0x04002FAA RID: 12202
		MU_ADDATTACKPERCENTTIMER,
		// Token: 0x04002FAB RID: 12203
		MU_SUBATTACKVALUETIMER,
		// Token: 0x04002FAC RID: 12204
		MU_ADDATTACKVALUETIMER,
		// Token: 0x04002FAD RID: 12205
		MU_SUBDEFENSEPERCENTTIMER,
		// Token: 0x04002FAE RID: 12206
		MU_ADDDEFENSEPERCENTTIMER,
		// Token: 0x04002FAF RID: 12207
		MU_SUBDEFENSEVALUETIMER,
		// Token: 0x04002FB0 RID: 12208
		MU_ADDDEFENSEVALUETIMER,
		// Token: 0x04002FB1 RID: 12209
		MU_SUBMOVESPEEDPERCENTTIMER,
		// Token: 0x04002FB2 RID: 12210
		MU_ADDMAXLIFEPERCENTANDVALUE,
		// Token: 0x04002FB3 RID: 12211
		MU_SUBHITPERCENTTIMER,
		// Token: 0x04002FB4 RID: 12212
		MU_SUBDAMAGEPERCENTVALUETIMER,
		// Token: 0x04002FB5 RID: 12213
		MU_ADDATTACKANDDEFENSEEPERCENTVALUETIMER,
		// Token: 0x04002FB6 RID: 12214
		MU_ANGELTEMPLEBUFF1,
		// Token: 0x04002FB7 RID: 12215
		MU_ANGELTEMPLEBUFF2,
		// Token: 0x04002FB8 RID: 12216
		MU_JINGJICHANG_JUNXIAN,
		// Token: 0x04002FB9 RID: 12217
		MU_ZHANMENGBUILD_ZHANQI,
		// Token: 0x04002FBA RID: 12218
		MU_ZHANMENGBUILD_JITAN,
		// Token: 0x04002FBB RID: 12219
		MU_ZHANMENGBUILD_JUNXIE,
		// Token: 0x04002FBC RID: 12220
		MU_ZHANMENGBUILD_GUANGHUAN,
		// Token: 0x04002FBD RID: 12221
		MU_REDNAME_DEBUFF,
		// Token: 0x04002FBE RID: 12222
		TimeFEIXUENoShow,
		// Token: 0x04002FBF RID: 12223
		TimeZHONGDUNoShow,
		// Token: 0x04002FC0 RID: 12224
		TimeLINGHUNoShow,
		// Token: 0x04002FC1 RID: 12225
		TimeRANSHAONoShow,
		// Token: 0x04002FC2 RID: 12226
		TimeHUZHAONoShow,
		// Token: 0x04002FC3 RID: 12227
		TimeWUDIHUZHAONoShow,
		// Token: 0x04002FC4 RID: 12228
		MU_WORLDLEVEL,
		// Token: 0x04002FC5 RID: 12229
		MU_SPECMACH_EXP,
		// Token: 0x04002FC6 RID: 12230
		HysyShengBei,
		// Token: 0x04002FC7 RID: 12231
		MU_ADD_HIT_DODGE_PERCENT,
		// Token: 0x04002FC8 RID: 12232
		LangHunLingYu_ChengHao,
		// Token: 0x04002FC9 RID: 12233
		MU_ADD_DAMAGE_THORN_PERCENT,
		// Token: 0x04002FCA RID: 12234
		ZhongShenZhiShen_ChengHao = 111,
		// Token: 0x04002FCB RID: 12235
		BaTiState = 113,
		// Token: 0x04002FCC RID: 12236
		FightState,
		// Token: 0x04002FCD RID: 12237
		JunTuanCaiJiBuff,
		// Token: 0x04002FCE RID: 12238
		HuiJiHuTi,
		// Token: 0x04002FCF RID: 12239
		StateXuanYun,
		// Token: 0x04002FD0 RID: 12240
		StateSpeedDown,
		// Token: 0x04002FD1 RID: 12241
		ZuZhou,
		// Token: 0x04002FD2 RID: 12242
		NoDie,
		// Token: 0x04002FD3 RID: 12243
		BianShen,
		// Token: 0x04002FD4 RID: 12244
		LianZhanBuff,
		// Token: 0x04002FD5 RID: 12245
		RebornMutilExp,
		// Token: 0x04002FD6 RID: 12246
		MaxVal,
		// Token: 0x04002FD7 RID: 12247
		CompEnemy = 8999,
		// Token: 0x04002FD8 RID: 12248
		CompBossKiller_1,
		// Token: 0x04002FD9 RID: 12249
		CompBossKiller_2,
		// Token: 0x04002FDA RID: 12250
		CompBossKiller_3,
		// Token: 0x04002FDB RID: 12251
		CompJunXian_1_1,
		// Token: 0x04002FDC RID: 12252
		CompJunXian_1_2,
		// Token: 0x04002FDD RID: 12253
		CompJunXian_1_3,
		// Token: 0x04002FDE RID: 12254
		CompJunXian_2_1,
		// Token: 0x04002FDF RID: 12255
		CompJunXian_2_2,
		// Token: 0x04002FE0 RID: 12256
		CompJunXian_2_3,
		// Token: 0x04002FE1 RID: 12257
		CompJunXian_3_1,
		// Token: 0x04002FE2 RID: 12258
		CompJunXian_3_2,
		// Token: 0x04002FE3 RID: 12259
		CompJunXian_3_3,
		// Token: 0x04002FE4 RID: 12260
		CompBattle_Self,
		// Token: 0x04002FE5 RID: 12261
		CompBattle_1,
		// Token: 0x04002FE6 RID: 12262
		CompBattle_2,
		// Token: 0x04002FE7 RID: 12263
		CompBattle_3,
		// Token: 0x04002FE8 RID: 12264
		CompBattle_4,
		// Token: 0x04002FE9 RID: 12265
		CompBattle_5,
		// Token: 0x04002FEA RID: 12266
		EscapeBattleDebuff = 9050,
		// Token: 0x04002FEB RID: 12267
		ZorkTopTeam_Title,
		// Token: 0x04002FEC RID: 12268
		ZorkTopKiller_Title,
		// Token: 0x04002FED RID: 12269
		BangHuiMatchBZ_GoldChengHao = 10001,
		// Token: 0x04002FEE RID: 12270
		BangHuiMatchCY_GoldChengHao,
		// Token: 0x04002FEF RID: 12271
		BangHuiMatchBZ_RookieChengHao,
		// Token: 0x04002FF0 RID: 12272
		BangHuiMatchCY_RookieChengHao,
		// Token: 0x04002FF1 RID: 12273
		BangHuiMatchDeHurt_Temple,
		// Token: 0x04002FF2 RID: 12274
		BangHuiMatchDeHurt_QiZhi,
		// Token: 0x04002FF3 RID: 12275
		HuangMoChenMin,
		// Token: 0x04002FF4 RID: 12276
		HuangMoLingZhu,
		// Token: 0x04002FF5 RID: 12277
		DiGongChenMin,
		// Token: 0x04002FF6 RID: 12278
		DiGongLingZhu,
		// Token: 0x04002FF7 RID: 12279
		LuoLanGuiZu_Title,
		// Token: 0x04002FF8 RID: 12280
		LuoLanChengZhu_Title,
		// Token: 0x04002FF9 RID: 12281
		ShengYuChengZhu_Title,
		// Token: 0x04002FFA RID: 12282
		KuaFuLueDuo_1_1 = 10020,
		// Token: 0x04002FFB RID: 12283
		KuaFuLueDuo_1_2 = 10022,
		// Token: 0x04002FFC RID: 12284
		KuaFuLueDuo_2_1,
		// Token: 0x04002FFD RID: 12285
		SpecialTitleBufferStart = 20000,
		// Token: 0x04002FFE RID: 12286
		SpecialTitleBufferEnd = 29999,
		// Token: 0x04002FFF RID: 12287
		ZorkBattle_1 = 2000853,
		// Token: 0x04003000 RID: 12288
		ZorkBattle_2,
		// Token: 0x04003001 RID: 12289
		ZorkBattle_3,
		// Token: 0x04003002 RID: 12290
		ZorkBattle_4,
		// Token: 0x04003003 RID: 12291
		ZorkBattle_5,
		// Token: 0x04003004 RID: 12292
		KingOfBattleCrystal = 2080001,
		// Token: 0x04003005 RID: 12293
		KarenEastCrystal,
		// Token: 0x04003006 RID: 12294
		KingOfBattleBoss_GJDZY = 2080007,
		// Token: 0x04003007 RID: 12295
		KingOfBattleBoss_GJDJX,
		// Token: 0x04003008 RID: 12296
		KingOfBattleBoss_GJDNH,
		// Token: 0x04003009 RID: 12297
		CoupleArena_ZhenAi_Buff,
		// Token: 0x0400300A RID: 12298
		CoupleArena_YongQi_Buff,
		// Token: 0x0400300B RID: 12299
		MU_LUOLANCHENGZHAN_QIZHI1 = 2000805,
		// Token: 0x0400300C RID: 12300
		MU_LUOLANCHENGZHAN_QIZHI2,
		// Token: 0x0400300D RID: 12301
		MU_LUOLANCHENGZHAN_QIZHI3,
		// Token: 0x0400300E RID: 12302
		MU_MARRIAGE_SUBDAMAGEPERCENTTIMER,
		// Token: 0x0400300F RID: 12303
		MU_LANGHUNLINGYU_QIZHI1 = 2000810,
		// Token: 0x04003010 RID: 12304
		MU_LANGHUNLINGYU_QIZHI2,
		// Token: 0x04003011 RID: 12305
		MU_LANGHUNLINGYU_QIZHI3,
		// Token: 0x04003012 RID: 12306
		EscapeBattleGod = 2090001,
		// Token: 0x04003013 RID: 12307
		EscapeBattleDevil
	}
}
