using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.Data.Tarot;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000A5 RID: 165
	[ProtoContract]
	public class RoleDataEx
	{
		// Token: 0x040003BB RID: 955
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040003BC RID: 956
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x040003BD RID: 957
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x040003BE RID: 958
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x040003BF RID: 959
		[ProtoMember(5)]
		public int Level = 1;

		// Token: 0x040003C0 RID: 960
		[ProtoMember(6)]
		public int Faction = 0;

		// Token: 0x040003C1 RID: 961
		[ProtoMember(7)]
		public int Money1 = 0;

		// Token: 0x040003C2 RID: 962
		[ProtoMember(8)]
		public int Money2 = 0;

		// Token: 0x040003C3 RID: 963
		[ProtoMember(9)]
		public long Experience = 0L;

		// Token: 0x040003C4 RID: 964
		[ProtoMember(10)]
		public int PKMode = 0;

		// Token: 0x040003C5 RID: 965
		[ProtoMember(11)]
		public int PKValue = 0;

		// Token: 0x040003C6 RID: 966
		[ProtoMember(12)]
		public int MapCode = 0;

		// Token: 0x040003C7 RID: 967
		[ProtoMember(13)]
		public int PosX = 0;

		// Token: 0x040003C8 RID: 968
		[ProtoMember(14)]
		public int PosY = 0;

		// Token: 0x040003C9 RID: 969
		[ProtoMember(15)]
		public int RoleDirection = 0;

		// Token: 0x040003CA RID: 970
		[ProtoMember(16)]
		public int LifeV = 0;

		// Token: 0x040003CB RID: 971
		[ProtoMember(17)]
		public int MagicV = 0;

		// Token: 0x040003CC RID: 972
		[ProtoMember(18)]
		public List<OldTaskData> OldTasks = null;

		// Token: 0x040003CD RID: 973
		[ProtoMember(19)]
		public int RolePic = 0;

		// Token: 0x040003CE RID: 974
		[ProtoMember(20)]
		public int BagNum = 0;

		// Token: 0x040003CF RID: 975
		[ProtoMember(21)]
		public List<TaskData> TaskDataList = null;

		// Token: 0x040003D0 RID: 976
		[ProtoMember(22)]
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x040003D1 RID: 977
		[ProtoMember(23)]
		public string OtherName = "";

		// Token: 0x040003D2 RID: 978
		[ProtoMember(24)]
		public string MainQuickBarKeys = "";

		// Token: 0x040003D3 RID: 979
		[ProtoMember(25)]
		public string OtherQuickBarKeys = "";

		// Token: 0x040003D4 RID: 980
		[ProtoMember(26)]
		public int LoginNum = 0;

		// Token: 0x040003D5 RID: 981
		[ProtoMember(27)]
		public int UserMoney = 0;

		// Token: 0x040003D6 RID: 982
		[ProtoMember(28)]
		public int LeftFightSeconds = 0;

		// Token: 0x040003D7 RID: 983
		[ProtoMember(29)]
		public List<FriendData> FriendDataList = null;

		// Token: 0x040003D8 RID: 984
		[ProtoMember(30)]
		public List<HorseData> HorsesDataList = null;

		// Token: 0x040003D9 RID: 985
		[ProtoMember(31)]
		public int HorseDbID = 0;

		// Token: 0x040003DA RID: 986
		[ProtoMember(32)]
		public List<PetData> PetsDataList = null;

		// Token: 0x040003DB RID: 987
		[ProtoMember(33)]
		public int PetDbID = 0;

		// Token: 0x040003DC RID: 988
		[ProtoMember(34)]
		public int InterPower = 0;

		// Token: 0x040003DD RID: 989
		[ProtoMember(35)]
		public List<JingMaiData> JingMaiDataList = null;

		// Token: 0x040003DE RID: 990
		[ProtoMember(36)]
		public int DJPoint = 0;

		// Token: 0x040003DF RID: 991
		[ProtoMember(37)]
		public int DJTotal = 0;

		// Token: 0x040003E0 RID: 992
		[ProtoMember(38)]
		public int DJWincnt = 0;

		// Token: 0x040003E1 RID: 993
		[ProtoMember(39)]
		public int TotalOnlineSecs = 0;

		// Token: 0x040003E2 RID: 994
		[ProtoMember(40)]
		public int AntiAddictionSecs = 0;

		// Token: 0x040003E3 RID: 995
		[ProtoMember(41)]
		public long LastOfflineTime = 0L;

		// Token: 0x040003E4 RID: 996
		[ProtoMember(42)]
		public long BiGuanTime = 0L;

		// Token: 0x040003E5 RID: 997
		[ProtoMember(43)]
		public int YinLiang = 0;

		// Token: 0x040003E6 RID: 998
		[ProtoMember(44)]
		public List<SkillData> SkillDataList = null;

		// Token: 0x040003E7 RID: 999
		[ProtoMember(45)]
		public int TotalJingMaiExp = 0;

		// Token: 0x040003E8 RID: 1000
		[ProtoMember(46)]
		public int JingMaiExpNum = 0;

		// Token: 0x040003E9 RID: 1001
		[ProtoMember(47)]
		public long RegTime = 0L;

		// Token: 0x040003EA RID: 1002
		[ProtoMember(48)]
		public int LastHorseID = 0;

		// Token: 0x040003EB RID: 1003
		[ProtoMember(49)]
		public List<GoodsData> SaleGoodsDataList = null;

		// Token: 0x040003EC RID: 1004
		[ProtoMember(50)]
		public int DefaultSkillID = -1;

		// Token: 0x040003ED RID: 1005
		[ProtoMember(51)]
		public int AutoLifeV = 0;

		// Token: 0x040003EE RID: 1006
		[ProtoMember(52)]
		public int AutoMagicV = 0;

		// Token: 0x040003EF RID: 1007
		[ProtoMember(53)]
		public List<BufferData> BufferDataList = null;

		// Token: 0x040003F0 RID: 1008
		[ProtoMember(54)]
		public List<DailyTaskData> MyDailyTaskDataList = null;

		// Token: 0x040003F1 RID: 1009
		[ProtoMember(55)]
		public DailyJingMaiData MyDailyJingMaiData = null;

		// Token: 0x040003F2 RID: 1010
		[ProtoMember(56)]
		public int NumSkillID = 0;

		// Token: 0x040003F3 RID: 1011
		[ProtoMember(57)]
		public PortableBagData MyPortableBagData = null;

		// Token: 0x040003F4 RID: 1012
		[ProtoMember(58)]
		public HuodongData MyHuodongData = null;

		// Token: 0x040003F5 RID: 1013
		[ProtoMember(59)]
		public List<FuBenData> FuBenDataList = null;

		// Token: 0x040003F6 RID: 1014
		[ProtoMember(60)]
		public int MainTaskID = 0;

		// Token: 0x040003F7 RID: 1015
		[ProtoMember(61)]
		public int PKPoint = 0;

		// Token: 0x040003F8 RID: 1016
		[ProtoMember(62)]
		public int LianZhan = 0;

		// Token: 0x040003F9 RID: 1017
		[ProtoMember(63)]
		public RoleDailyData MyRoleDailyData = null;

		// Token: 0x040003FA RID: 1018
		[ProtoMember(64)]
		public int KillBoss = 0;

		// Token: 0x040003FB RID: 1019
		[ProtoMember(65)]
		public YaBiaoData MyYaBiaoData = null;

		// Token: 0x040003FC RID: 1020
		[ProtoMember(66)]
		public long BattleNameStart = 0L;

		// Token: 0x040003FD RID: 1021
		[ProtoMember(67)]
		public int BattleNameIndex = 0;

		// Token: 0x040003FE RID: 1022
		[ProtoMember(68)]
		public int CZTaskID = 0;

		// Token: 0x040003FF RID: 1023
		[ProtoMember(69)]
		public int BattleNum = 0;

		// Token: 0x04000400 RID: 1024
		[ProtoMember(70)]
		public int HeroIndex = 0;

		// Token: 0x04000401 RID: 1025
		[ProtoMember(71)]
		public int ZoneID = 0;

		// Token: 0x04000402 RID: 1026
		[ProtoMember(72)]
		public string BHName = "";

		// Token: 0x04000403 RID: 1027
		[ProtoMember(73)]
		public int BHVerify = 0;

		// Token: 0x04000404 RID: 1028
		[ProtoMember(74)]
		public int BHZhiWu = 0;

		// Token: 0x04000405 RID: 1029
		[ProtoMember(75)]
		public int BGDayID1 = 0;

		// Token: 0x04000406 RID: 1030
		[ProtoMember(76)]
		public int BGMoney = 0;

		// Token: 0x04000407 RID: 1031
		[ProtoMember(77)]
		public int BGDayID2 = 0;

		// Token: 0x04000408 RID: 1032
		[ProtoMember(78)]
		public int BGGoods = 0;

		// Token: 0x04000409 RID: 1033
		[ProtoMember(80)]
		public int BangGong = 0;

		// Token: 0x0400040A RID: 1034
		[ProtoMember(81)]
		public int HuangHou = 0;

		// Token: 0x0400040B RID: 1035
		[ProtoMember(82)]
		public Dictionary<int, int> PaiHangPosDict = null;

		// Token: 0x0400040C RID: 1036
		[ProtoMember(83)]
		public int JieBiaoDayID = 0;

		// Token: 0x0400040D RID: 1037
		[ProtoMember(84)]
		public int JieBiaoDayNum = 0;

		// Token: 0x0400040E RID: 1038
		[ProtoMember(85)]
		public int LastMailID = 0;

		// Token: 0x0400040F RID: 1039
		[ProtoMember(86)]
		public List<VipDailyData> VipDailyDataList = null;

		// Token: 0x04000410 RID: 1040
		[ProtoMember(87)]
		public YangGongBKDailyJiFenData YangGongBKDailyJiFen = null;

		// Token: 0x04000411 RID: 1041
		[ProtoMember(88)]
		public long OnceAwardFlag = 0L;

		// Token: 0x04000412 RID: 1042
		[ProtoMember(89)]
		public int Gold = 0;

		// Token: 0x04000413 RID: 1043
		[ProtoMember(90)]
		public List<GoodsLimitData> GoodsLimitDataList = null;

		// Token: 0x04000414 RID: 1044
		[ProtoMember(91)]
		public Dictionary<string, RoleParamsData> RoleParamsDict = null;

		// Token: 0x04000415 RID: 1045
		[ProtoMember(92)]
		public int BanChat = 0;

		// Token: 0x04000416 RID: 1046
		[ProtoMember(93)]
		public int BanLogin = 0;

		// Token: 0x04000417 RID: 1047
		[ProtoMember(94)]
		public int IsFlashPlayer = 0;

		// Token: 0x04000418 RID: 1048
		[ProtoMember(95)]
		public int ChangeLifeCount = 0;

		// Token: 0x04000419 RID: 1049
		[ProtoMember(96)]
		public int AdmiredCount = 0;

		// Token: 0x0400041A RID: 1050
		[ProtoMember(97)]
		public int CombatForce = 0;

		// Token: 0x0400041B RID: 1051
		[ProtoMember(98)]
		public int AutoAssignPropertyPoint = 0;

		// Token: 0x0400041C RID: 1052
		[ProtoMember(99)]
		public string PushMessageID = "";

		// Token: 0x0400041D RID: 1053
		[ProtoMember(100)]
		public WingData MyWingData = null;

		// Token: 0x0400041E RID: 1054
		[ProtoMember(101)]
		public Dictionary<int, int> RolePictureJudgeReferInfo = null;

		// Token: 0x0400041F RID: 1055
		[ProtoMember(102)]
		public Dictionary<int, int> RoleStarConstellationInfo = null;

		// Token: 0x04000420 RID: 1056
		[ProtoMember(103)]
		public int VIPLevel = 0;

		// Token: 0x04000421 RID: 1057
		[ProtoMember(104)]
		public List<GoodsData> ElementhrtsList = null;

		// Token: 0x04000422 RID: 1058
		[ProtoMember(105)]
		public List<GoodsData> UsingElementhrtsList = null;

		// Token: 0x04000423 RID: 1059
		[ProtoMember(106)]
		public List<GoodsData> PetList = null;

		// Token: 0x04000424 RID: 1060
		[ProtoMember(107)]
		public long Store_Yinliang = 0L;

		// Token: 0x04000425 RID: 1061
		[ProtoMember(108)]
		public long Store_Money = 0L;

		// Token: 0x04000426 RID: 1062
		[ProtoMember(109)]
		public Dictionary<int, LingYuData> LingYuDict = null;

		// Token: 0x04000427 RID: 1063
		[ProtoMember(110)]
		public List<GoodsData> DamonGoodsDataList = null;

		// Token: 0x04000428 RID: 1064
		[ProtoMember(111)]
		public int MagicSwordParam = 0;

		// Token: 0x04000429 RID: 1065
		[ProtoMember(112)]
		public MarriageData MyMarriageData = null;

		// Token: 0x0400042A RID: 1066
		[ProtoMember(113)]
		public Dictionary<int, int> MyMarryPartyJoinList = null;

		// Token: 0x0400042B RID: 1067
		[ProtoMember(114)]
		public List<int> GroupMailRecordList = null;

		// Token: 0x0400042C RID: 1068
		[ProtoMember(115)]
		public GuardStatueDetail MyGuardStatueDetail = null;

		// Token: 0x0400042D RID: 1069
		[ProtoMember(116, IsRequired = true)]
		public TalentData MyTalentData = null;

		// Token: 0x0400042E RID: 1070
		[ProtoMember(117, IsRequired = true)]
		public RoleTianTiData TianTiData;

		// Token: 0x0400042F RID: 1071
		[ProtoMember(118)]
		public List<GoodsData> FashionGoodsDataList = null;

		// Token: 0x04000430 RID: 1072
		[ProtoMember(119)]
		public MerlinGrowthSaveDBData MerlinData = null;

		// Token: 0x04000431 RID: 1073
		[ProtoMember(120)]
		public Dictionary<sbyte, HolyItemData> MyHolyItemDataDic = null;

		// Token: 0x04000432 RID: 1074
		[ProtoMember(121)]
		public FluorescentGemData FluorescentGemData = null;

		// Token: 0x04000433 RID: 1075
		[ProtoMember(122)]
		public int FluorescentPoint = 0;

		// Token: 0x04000434 RID: 1076
		[ProtoMember(123)]
		public List<BuildingData> BuildingDataList = null;

		// Token: 0x04000435 RID: 1077
		[ProtoMember(124)]
		public Dictionary<int, Dictionary<int, SevenDayItemData>> SevenDayActDict = null;

		// Token: 0x04000436 RID: 1078
		[ProtoMember(125)]
		public List<GoodsData> SoulStonesInBag = null;

		// Token: 0x04000437 RID: 1079
		[ProtoMember(126)]
		public List<GoodsData> SoulStonesInUsing = null;

		// Token: 0x04000438 RID: 1080
		[ProtoMember(127)]
		public long BanTradeToTicks;

		// Token: 0x04000439 RID: 1081
		[ProtoMember(128)]
		public UserMiniData userMiniData;

		// Token: 0x0400043A RID: 1082
		[ProtoMember(129)]
		public Dictionary<int, SpecActInfoDB> SpecActInfoDict;

		// Token: 0x0400043B RID: 1083
		[ProtoMember(130)]
		public TarotSystemData TarotData;

		// Token: 0x0400043C RID: 1084
		[ProtoMember(131)]
		public List<GoodsData> OrnamentGoodsList = null;

		// Token: 0x0400043D RID: 1085
		[ProtoMember(132)]
		public Dictionary<int, OrnamentData> OrnamentDataDict = null;

		// Token: 0x0400043E RID: 1086
		[ProtoMember(133)]
		public List<int> OccupationList;

		// Token: 0x0400043F RID: 1087
		[ProtoMember(136)]
		public int JunTuanZhiWu;

		// Token: 0x04000440 RID: 1088
		[ProtoMember(137)]
		public List<GoodsData> PaiZhuDamonGoodsDataList = null;

		// Token: 0x04000441 RID: 1089
		[ProtoMember(138)]
		public Dictionary<int, ShenJiFuWenData> ShenJiDict = null;

		// Token: 0x04000442 RID: 1090
		[ProtoMember(139)]
		public Dictionary<int, EverydayActInfoDB> EverydayActInfoDict;

		// Token: 0x04000443 RID: 1091
		[ProtoMember(140)]
		public RoleHuiJiData HuiJiData;

		// Token: 0x04000444 RID: 1092
		[ProtoMember(141)]
		public string UserID;

		// Token: 0x04000445 RID: 1093
		[ProtoMember(142)]
		public List<FuWenTabData> FuWenTabList;

		// Token: 0x04000446 RID: 1094
		[ProtoMember(143)]
		public List<GoodsData> FuWenGoodsDataList;

		// Token: 0x04000447 RID: 1095
		[ProtoMember(144)]
		public AlchemyDataDB AlchemyInfo;

		// Token: 0x04000448 RID: 1096
		[ProtoMember(150)]
		public int SubOccupation;

		// Token: 0x04000449 RID: 1097
		[ProtoMember(151, IsRequired = true)]
		public RoleArmorData ArmorData;

		// Token: 0x0400044A RID: 1098
		[ProtoMember(152)]
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;

		// Token: 0x0400044B RID: 1099
		[ProtoMember(153)]
		public RoleBianShenData BianShenData;

		// Token: 0x0400044C RID: 1100
		[ProtoMember(154)]
		public List<GoodsData> RebornGoodsDataList;

		// Token: 0x0400044D RID: 1101
		[ProtoMember(155)]
		public List<FuMoMailData> FumoMail;

		// Token: 0x0400044E RID: 1102
		[ProtoMember(156)]
		public int RebornCombatForce = 0;

		// Token: 0x0400044F RID: 1103
		[ProtoMember(157)]
		public int RebornCount = 0;

		// Token: 0x04000450 RID: 1104
		[ProtoMember(158)]
		public int RebornLevel = 0;

		// Token: 0x04000451 RID: 1105
		[ProtoMember(159)]
		public long RebornExperience = 0L;

		// Token: 0x04000452 RID: 1106
		[ProtoMember(160)]
		public List<GoodsData> RebornGoodsStoreList;

		// Token: 0x04000453 RID: 1107
		[ProtoMember(161)]
		public int RebornBagNum = 0;

		// Token: 0x04000454 RID: 1108
		[ProtoMember(162)]
		public RebornPortableBagData RebornGirdData = null;

		// Token: 0x04000455 RID: 1109
		[ProtoMember(163)]
		public int RebornShowEquip = 0;

		// Token: 0x04000456 RID: 1110
		[ProtoMember(164)]
		public RebornStampData RebornYinJi = null;

		// Token: 0x04000457 RID: 1111
		[ProtoMember(165)]
		public int RebornShowModel = 0;

		// Token: 0x04000458 RID: 1112
		[ProtoMember(166)]
		public Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> SpecPriorityActInfoDict;

		// Token: 0x04000459 RID: 1113
		[ProtoMember(167)]
		public int ZhanDuiID;

		// Token: 0x0400045A RID: 1114
		[ProtoMember(168)]
		public int ZhanDuiZhiWu;

		// Token: 0x0400045B RID: 1115
		[ProtoMember(169)]
		public List<GoodsData> HolyGoodsDataList = null;

		// Token: 0x0400045C RID: 1116
		[ProtoMember(170)]
		public Dictionary<int, RebornEquipData> RebornEquipHoleData = null;

		// Token: 0x0400045D RID: 1117
		[ProtoMember(171)]
		public Dictionary<int, MazingerStoreData> MazingerStoreDataInfo = null;

		// Token: 0x0400045E RID: 1118
		[ProtoMember(249)]
		public int PTID;

		// Token: 0x0400045F RID: 1119
		[ProtoMember(250)]
		public int UserPTID;

		// Token: 0x04000460 RID: 1120
		[ProtoMember(251)]
		public string WorldRoleID;

		// Token: 0x04000461 RID: 1121
		[ProtoMember(252)]
		public string Channel;
	}
}
