using System;

namespace GameServer.Logic
{
	
	public enum BufferItemTypes
	{
		
		None,
		
		DblExperience,
		
		DblMoney,
		
		DblLingLi,
		
		LifeVReserve,
		
		MagicVReserve,
		
		AddTempAttack,
		
		AddTempDefense,
		
		UpLifeLimit,
		
		UpMagicLimit,
		
		LingLiVReserve,
		
		AntiBoss,
		
		AntiRole,
		
		MonthVIP,
		
		SheLiZhiYuan,
		
		DiWanZhiYou,
		
		JunQi,
		
		DblSkillUp,
		
		ThreeExperience,
		
		ThreeMoney,
		
		AutoFightingProtect,
		
		TimeExp,
		
		TimeAddDefense,
		
		TimeAddMDefense,
		
		TimeAddAttack,
		
		TimeAddMAttack,
		
		TimeAddDSAttack,
		
		TimeAddLifeMagic,
		
		WaWaExp,
		
		ZhuFu,
		
		FallTianSheng,
		
		ChengJiu,
		
		JingMai,
		
		WuXue,
		
		GuMuTimeLimit,
		
		MingJieMapLimit,
		
		FiveExperience,
		
		TimeAddLifeNoShow,
		
		TimeAddMagicNoShow,
		
		PKKingBuffer,
		
		DSTimeAddLifeNoShow,
		
		DSTimeHideNoShow,
		
		DSTimeShiDuNoShow,
		
		DSTimeAddDefenseNoShow,
		
		DSTimeAddMDefenseNoShow,
		
		FSAddHuDunNoShow,
		
		MutilExperience,
		
		JieRiChengHao,
		
		ErGuoTou,
		
		ZuanHuang,
		
		ZhanHun,
		
		RongYu,
		
		ADDTEMPStrength,
		
		ADDTEMPIntelligsence,
		
		ADDTEMPDexterity,
		
		ADDTEMPConstitution,
		
		ADDTEMPATTACKSPEED,
		
		ADDTEMPLUCKYATTACK,
		
		ADDTEMPFATALATTACK,
		
		ADDTEMPDOUBLEATTACK,
		
		MU_SUBDAMAGEPERCENTTIMER,
		
		MU_MAXLIFEPERCENT,
		
		MU_ADDDEFENSETIMER,
		
		MU_ADDATTACKTIMER,
		
		MU_ADDLUCKYATTACKPERCENTTIMER,
		
		MU_ADDFATALATTACKPERCENTTIMER,
		
		MU_ADDDOUBLEATTACKPERCENTTIMER,
		
		MU_ADDMAXHPVALUE,
		
		MU_ADDMAXMPVALUE,
		
		MU_ADDLIFERECOVERPERCENT,
		
		MU_FRESHPLAYERBUFF,
		
		MU_SUBDAMAGEPERCENTTIMER1,
		
		MU_SUBATTACKPERCENTTIMER,
		
		MU_ADDATTACKPERCENTTIMER,
		
		MU_SUBATTACKVALUETIMER,
		
		MU_ADDATTACKVALUETIMER,
		
		MU_SUBDEFENSEPERCENTTIMER,
		
		MU_ADDDEFENSEPERCENTTIMER,
		
		MU_SUBDEFENSEVALUETIMER,
		
		MU_ADDDEFENSEVALUETIMER,
		
		MU_SUBMOVESPEEDPERCENTTIMER,
		
		MU_ADDMAXLIFEPERCENTANDVALUE,
		
		MU_SUBHITPERCENTTIMER,
		
		MU_SUBDAMAGEPERCENTVALUETIMER,
		
		MU_ADDATTACKANDDEFENSEEPERCENTVALUETIMER,
		
		MU_ANGELTEMPLEBUFF1,
		
		MU_ANGELTEMPLEBUFF2,
		
		MU_JINGJICHANG_JUNXIAN,
		
		MU_ZHANMENGBUILD_ZHANQI,
		
		MU_ZHANMENGBUILD_JITAN,
		
		MU_ZHANMENGBUILD_JUNXIE,
		
		MU_ZHANMENGBUILD_GUANGHUAN,
		
		MU_REDNAME_DEBUFF,
		
		TimeFEIXUENoShow,
		
		TimeZHONGDUNoShow,
		
		TimeLINGHUNoShow,
		
		TimeRANSHAONoShow,
		
		TimeHUZHAONoShow,
		
		TimeWUDIHUZHAONoShow,
		
		MU_WORLDLEVEL,
		
		MU_SPECMACH_EXP,
		
		HysyShengBei,
		
		MU_ADD_HIT_DODGE_PERCENT,
		
		LangHunLingYu_ChengHao,
		
		MU_ADD_DAMAGE_THORN_PERCENT,
		
		ZhongShenZhiShen_ChengHao = 111,
		
		BaTiState = 113,
		
		FightState,
		
		JunTuanCaiJiBuff,
		
		HuiJiHuTi,
		
		StateXuanYun,
		
		StateSpeedDown,
		
		ZuZhou,
		
		NoDie,
		
		BianShen,
		
		LianZhanBuff,
		
		RebornMutilExp,
		
		MaxVal,
		
		CompEnemy = 8999,
		
		CompBossKiller_1,
		
		CompBossKiller_2,
		
		CompBossKiller_3,
		
		CompJunXian_1_1,
		
		CompJunXian_1_2,
		
		CompJunXian_1_3,
		
		CompJunXian_2_1,
		
		CompJunXian_2_2,
		
		CompJunXian_2_3,
		
		CompJunXian_3_1,
		
		CompJunXian_3_2,
		
		CompJunXian_3_3,
		
		CompBattle_Self,
		
		CompBattle_1,
		
		CompBattle_2,
		
		CompBattle_3,
		
		CompBattle_4,
		
		CompBattle_5,
		
		EscapeBattleDebuff = 9050,
		
		ZorkTopTeam_Title,
		
		ZorkTopKiller_Title,
		
		BangHuiMatchBZ_GoldChengHao = 10001,
		
		BangHuiMatchCY_GoldChengHao,
		
		BangHuiMatchBZ_RookieChengHao,
		
		BangHuiMatchCY_RookieChengHao,
		
		BangHuiMatchDeHurt_Temple,
		
		BangHuiMatchDeHurt_QiZhi,
		
		HuangMoChenMin,
		
		HuangMoLingZhu,
		
		DiGongChenMin,
		
		DiGongLingZhu,
		
		LuoLanGuiZu_Title,
		
		LuoLanChengZhu_Title,
		
		ShengYuChengZhu_Title,
		
		KuaFuLueDuo_1_1 = 10020,
		
		KuaFuLueDuo_1_2 = 10022,
		
		KuaFuLueDuo_2_1,
		
		SpecialTitleBufferStart = 20000,
		
		SpecialTitleBufferEnd = 29999,
		
		ZorkBattle_1 = 2000853,
		
		ZorkBattle_2,
		
		ZorkBattle_3,
		
		ZorkBattle_4,
		
		ZorkBattle_5,
		
		KingOfBattleCrystal = 2080001,
		
		KarenEastCrystal,
		
		KingOfBattleBoss_GJDZY = 2080007,
		
		KingOfBattleBoss_GJDJX,
		
		KingOfBattleBoss_GJDNH,
		
		CoupleArena_ZhenAi_Buff,
		
		CoupleArena_YongQi_Buff,
		
		MU_LUOLANCHENGZHAN_QIZHI1 = 2000805,
		
		MU_LUOLANCHENGZHAN_QIZHI2,
		
		MU_LUOLANCHENGZHAN_QIZHI3,
		
		MU_MARRIAGE_SUBDAMAGEPERCENTTIMER,
		
		MU_LANGHUNLINGYU_QIZHI1 = 2000810,
		
		MU_LANGHUNLINGYU_QIZHI2,
		
		MU_LANGHUNLINGYU_QIZHI3,
		
		EscapeBattleGod = 2090001,
		
		EscapeBattleDevil
	}
}
