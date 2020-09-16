using System;


public enum ActivityTipTypes
{
	
	Root,
	
	MainHuoDongIcon = 1000,
	
	RiChangHuoDong,
	
	ShiJieBoss,
	
	VIPHuoDong,
	
	ShouFeiBoss,
	
	HuangJinBoss,
	
	RiChangHuoDongOther,
	
	AngelTemple,
	
	TianTiMonthRankAwards,
	
	MainFuBenIcon = 2000,
	
	JuQingFuBen,
	
	ZuDuiFuBen,
	
	RiChangFuBen,
	
	MainFuLiIcon = 3000,
	
	FuLiChongZhiHuiKui,
	
	ShouCiChongZhi,
	
	MeiRiChongZhi,
	
	LeiJiChongZhi,
	
	LeiJiXiaoFei,
	
	FuLiMeiRiHuoYue,
	
	FuLiLianXuDengLu,
	
	FuLiLeiJiDengLu,
	
	FuLiMeiRiZaiXian,
	
	FuLiUpLevelGift,
	
	ShouCiChongZhi_YiLingQu,
	
	MeiRiChongZhi_YiLingQu,
	
	FuLiYueKaFanLi,
	
	FuLiComBatGift,
	
	FuLiYueKaFanLi_Award,
	
	ShenYou = 3036,
	
	FuMo,
	
	MainJingJiChangIcon = 4000,
	
	JingJiChangJiangLi,
	
	JingJiChangJunXian,
	
	JingJiChangLeftTimes,
	
	MainGongNeng = 5000,
	
	MainMingXiangIcon,
	
	MainEmailIcon,
	
	MainXinFuIcon = 6000,
	
	XinFuLevel,
	
	XinFuKillBoss,
	
	XinFuChongZhiMoney,
	
	XinFuUseMoney,
	
	XinFuFreeGetMoney,
	
	MainMeiRiBiZuoIcon = 7000,
	
	ZiYuanZhaoHui,
	
	MainPaiHang = 7500,
	
	QiFuIcon = 8000,
	
	MainChengJiuIcon = 9000,
	
	VIPGongNeng = 10000,
	
	VIPGifts,
	
	BuChangIcon = 11000,
	
	ThemeActivity = 11500,
	
	ThemeZhiGou,
	
	ThemeDaLiBao,
	
	HeFuActivity = 12000,
	
	HeFuLogin,
	
	HeFuTotalLogin,
	
	HeFuRecharge,
	
	HeFuPKKing,
	
	HeFuLuoLan,
	
	ShuiJingHuangJin = 13000,
	
	JieRiActivity = 14000,
	
	JieRiLogin,
	
	JieRiTotalLogin,
	
	JieRiDayCZ,
	
	JieRiLeiJiXF,
	
	JieRiLeiJiCZ,
	
	JieRiCZKING,
	
	JieRiXFKING,
	
	JieRiGive,
	
	JieRiGiveKing,
	
	JieRiRecvKing,
	
	JieriWing,
	
	JieriAddon,
	
	JieriStrengthen,
	
	JieriAchievement,
	
	JieriMilitaryRank,
	
	JieriVIPFanli,
	
	JieriAmulet,
	
	JieriArchangel,
	
	JieriMarriage,
	
	JieRiLianXuCharge,
	
	JieRiRecv,
	
	JieRiPlatChargeKing,
	
	JieRiIPointsExchg,
	
	JieRiDanBiChongZhi = 14027,
	
	JieRiMeiriLeiChong,
	
	JieRiHongBaoBang = 14032,
	
	JieRiHuiJi,
	
	JieRiFuWen,
	
	JieRiPCKingEveryDay,
	
	UserReturnAll = 14100,
	
	UserReturnRecall,
	
	UserReturnAward,
	
	UserReturnCheck,
	
	UserReturnResult,
	
	UserReturnLeiChong = 14115,
	
	UserRegressTriennialCheck = 14120,
	
	TipSpread = 14105,
	
	FundChangeLife,
	
	FundLogin,
	
	FundMoney,
	
	Fund,
	
	ZhuanXiang,
	
	Ally,
	
	AllyAccept,
	
	AllyMsg,
	
	EveryDayAct,
	
	SpecPriorityAct,
	
	MerlinSecretAttr = 14201,
	
	GuildIcon = 15000,
	
	GuildCopyMap,
	
	LangHunLingYuIcon,
	
	LangHunLingYuFightIcon,
	
	ZhengDuoFightIcon,
	
	JunTuanRequestIcon,
	
	ZhengBaCanJoinIcon = 15010,
	
	CoupleArenaCanAward,
	
	CoupleWishCanAward,
	
	KF5V5DailyPaiHang,
	
	BuildingIcon = 15050,
	
	OneDollarChongZhi,
	
	OneDollarBuy,
	
	JunTuanEra,
	
	InputFanLiNew,
	
	TriennialRegressOpen,
	
	TriennialRegressSignAward,
	
	TriennialRegressTotalRechargeAward,
	
	TriennialRegressDayBuy,
	
	TriennialRegressStore,
	
	PetBagIcon = 16000,
	
	CallPetIcon,
	
	SevenDayActivity = 17000,
	
	SevenDayLogin,
	
	SevenDayCharge,
	
	SevenDayGoal,
	
	SevenDayBuy,
	
	SevenDayGoal_1,
	
	SevenDayGoal_2,
	
	SevenDayGoal_3,
	
	SevenDayGoal_4,
	
	SevenDayGoal_5,
	
	SevenDayGoal_6,
	
	SevenDayGoal_7,
	
	AoYunDaTiActivity = 18000,
	
	XingyunChoujiangIcon = 18002,
	
	Icon_Linyl_ZhanYong = 18099,
	
	OlympicsActivity = 20000,
	
	OlympicsMatch,
	
	OlympicsGuess,
	
	Reborn = 21000,
	
	RebornUpgrade,
	
	MainBag = 30000,
	
	BagCanUseGoods
}
