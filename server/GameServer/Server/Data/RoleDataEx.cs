using System;
using System.Collections.Generic;
using GameServer.TarotData;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000589 RID: 1417
	[ProtoContract]
	public class RoleDataEx
	{
		// Token: 0x04002706 RID: 9990
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04002707 RID: 9991
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x04002708 RID: 9992
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x04002709 RID: 9993
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x0400270A RID: 9994
		[ProtoMember(5)]
		public int Level = 1;

		// Token: 0x0400270B RID: 9995
		[ProtoMember(6)]
		public int Faction = 0;

		// Token: 0x0400270C RID: 9996
		[ProtoMember(7)]
		public int Money1 = 0;

		// Token: 0x0400270D RID: 9997
		[ProtoMember(8)]
		public int Money2 = 0;

		// Token: 0x0400270E RID: 9998
		[ProtoMember(9)]
		public long Experience = 0L;

		// Token: 0x0400270F RID: 9999
		[ProtoMember(10)]
		public int PKMode = 0;

		// Token: 0x04002710 RID: 10000
		[ProtoMember(11)]
		public int PKValue = 0;

		// Token: 0x04002711 RID: 10001
		[ProtoMember(12)]
		public int MapCode = 0;

		// Token: 0x04002712 RID: 10002
		[ProtoMember(13)]
		public int PosX = 0;

		// Token: 0x04002713 RID: 10003
		[ProtoMember(14)]
		public int PosY = 0;

		// Token: 0x04002714 RID: 10004
		[ProtoMember(15)]
		public int RoleDirection = 0;

		// Token: 0x04002715 RID: 10005
		[ProtoMember(16)]
		public int LifeV = 0;

		// Token: 0x04002716 RID: 10006
		[ProtoMember(17)]
		public int MagicV = 0;

		// Token: 0x04002717 RID: 10007
		[ProtoMember(18)]
		public List<OldTaskData> OldTasks = null;

		// Token: 0x04002718 RID: 10008
		[ProtoMember(19)]
		public int RolePic = 0;

		// Token: 0x04002719 RID: 10009
		[ProtoMember(20)]
		public int BagNum = 0;

		// Token: 0x0400271A RID: 10010
		[ProtoMember(21)]
		public List<TaskData> TaskDataList = null;

		// Token: 0x0400271B RID: 10011
		[ProtoMember(22)]
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x0400271C RID: 10012
		[ProtoMember(23)]
		public string OtherName = "";

		// Token: 0x0400271D RID: 10013
		[ProtoMember(24)]
		public string MainQuickBarKeys = "";

		// Token: 0x0400271E RID: 10014
		[ProtoMember(25)]
		public string OtherQuickBarKeys = "";

		// Token: 0x0400271F RID: 10015
		[ProtoMember(26)]
		public int LoginNum = 0;

		// Token: 0x04002720 RID: 10016
		[ProtoMember(27)]
		public int UserMoney = 0;

		// Token: 0x04002721 RID: 10017
		[ProtoMember(28)]
		public int LeftFightSeconds = 0;

		// Token: 0x04002722 RID: 10018
		[ProtoMember(29)]
		public List<FriendData> FriendDataList = null;

		// Token: 0x04002723 RID: 10019
		[ProtoMember(30)]
		public List<HorseData> HorsesDataList = null;

		// Token: 0x04002724 RID: 10020
		[ProtoMember(31)]
		public int HorseDbID = 0;

		// Token: 0x04002725 RID: 10021
		[ProtoMember(32)]
		public List<PetData> PetsDataList = null;

		// Token: 0x04002726 RID: 10022
		[ProtoMember(33)]
		public int PetDbID = 0;

		// Token: 0x04002727 RID: 10023
		[ProtoMember(34)]
		public int InterPower = 0;

		// Token: 0x04002728 RID: 10024
		[ProtoMember(35)]
		public List<JingMaiData> JingMaiDataList = null;

		// Token: 0x04002729 RID: 10025
		[ProtoMember(36)]
		public int DJPoint = 0;

		// Token: 0x0400272A RID: 10026
		[ProtoMember(37)]
		public int DJTotal = 0;

		// Token: 0x0400272B RID: 10027
		[ProtoMember(38)]
		public int DJWincnt = 0;

		// Token: 0x0400272C RID: 10028
		[ProtoMember(39)]
		public int TotalOnlineSecs = 0;

		// Token: 0x0400272D RID: 10029
		[ProtoMember(40)]
		public int AntiAddictionSecs = 0;

		// Token: 0x0400272E RID: 10030
		[ProtoMember(41)]
		public long LastOfflineTime = 0L;

		// Token: 0x0400272F RID: 10031
		[ProtoMember(42)]
		public long BiGuanTime = 0L;

		// Token: 0x04002730 RID: 10032
		[ProtoMember(43)]
		public int YinLiang = 0;

		// Token: 0x04002731 RID: 10033
		[ProtoMember(44)]
		public List<SkillData> SkillDataList = null;

		// Token: 0x04002732 RID: 10034
		[ProtoMember(45)]
		public int TotalJingMaiExp = 0;

		// Token: 0x04002733 RID: 10035
		[ProtoMember(46)]
		public int JingMaiExpNum = 0;

		// Token: 0x04002734 RID: 10036
		[ProtoMember(47)]
		public long RegTime = 0L;

		// Token: 0x04002735 RID: 10037
		[ProtoMember(48)]
		public int LastHorseID = 0;

		// Token: 0x04002736 RID: 10038
		[ProtoMember(49)]
		public List<GoodsData> SaleGoodsDataList = null;

		// Token: 0x04002737 RID: 10039
		[ProtoMember(50)]
		public int DefaultSkillID = -1;

		// Token: 0x04002738 RID: 10040
		[ProtoMember(51)]
		public int AutoLifeV = 0;

		// Token: 0x04002739 RID: 10041
		[ProtoMember(52)]
		public int AutoMagicV = 0;

		// Token: 0x0400273A RID: 10042
		[ProtoMember(53)]
		public List<BufferData> BufferDataList = null;

		// Token: 0x0400273B RID: 10043
		[ProtoMember(54)]
		public List<DailyTaskData> MyDailyTaskDataList = null;

		// Token: 0x0400273C RID: 10044
		[ProtoMember(55)]
		public DailyJingMaiData MyDailyJingMaiData = null;

		// Token: 0x0400273D RID: 10045
		[ProtoMember(56)]
		public int NumSkillID = 0;

		// Token: 0x0400273E RID: 10046
		[ProtoMember(57)]
		public PortableBagData MyPortableBagData = null;

		// Token: 0x0400273F RID: 10047
		[ProtoMember(58)]
		public HuodongData MyHuodongData = null;

		// Token: 0x04002740 RID: 10048
		[ProtoMember(59)]
		public List<FuBenData> FuBenDataList = null;

		// Token: 0x04002741 RID: 10049
		[ProtoMember(60)]
		public int MainTaskID = 0;

		// Token: 0x04002742 RID: 10050
		[ProtoMember(61)]
		public int PKPoint = 0;

		// Token: 0x04002743 RID: 10051
		[ProtoMember(62)]
		public int LianZhan = 0;

		// Token: 0x04002744 RID: 10052
		[ProtoMember(63)]
		public RoleDailyData MyRoleDailyData = null;

		// Token: 0x04002745 RID: 10053
		[ProtoMember(64)]
		public int KillBoss = 0;

		// Token: 0x04002746 RID: 10054
		[ProtoMember(65)]
		public YaBiaoData MyYaBiaoData = null;

		// Token: 0x04002747 RID: 10055
		[ProtoMember(66)]
		public long BattleNameStart = 0L;

		// Token: 0x04002748 RID: 10056
		[ProtoMember(67)]
		public int BattleNameIndex = 0;

		// Token: 0x04002749 RID: 10057
		[ProtoMember(68)]
		public int CZTaskID = 0;

		// Token: 0x0400274A RID: 10058
		[ProtoMember(69)]
		public int BattleNum = 0;

		// Token: 0x0400274B RID: 10059
		[ProtoMember(70)]
		public int HeroIndex = 0;

		// Token: 0x0400274C RID: 10060
		[ProtoMember(71)]
		public int ZoneID = 0;

		// Token: 0x0400274D RID: 10061
		[ProtoMember(72)]
		public string BHName = "";

		// Token: 0x0400274E RID: 10062
		[ProtoMember(73)]
		public int BHVerify = 0;

		// Token: 0x0400274F RID: 10063
		[ProtoMember(74)]
		public int BHZhiWu = 0;

		// Token: 0x04002750 RID: 10064
		[ProtoMember(75)]
		public int BGDayID1 = 0;

		// Token: 0x04002751 RID: 10065
		[ProtoMember(76)]
		public int BGMoney = 0;

		// Token: 0x04002752 RID: 10066
		[ProtoMember(77)]
		public int BGDayID2 = 0;

		// Token: 0x04002753 RID: 10067
		[ProtoMember(78)]
		public int BGGoods = 0;

		// Token: 0x04002754 RID: 10068
		[ProtoMember(80)]
		public int BangGong = 0;

		// Token: 0x04002755 RID: 10069
		[ProtoMember(81)]
		public int HuangHou = 0;

		// Token: 0x04002756 RID: 10070
		[ProtoMember(82)]
		public Dictionary<int, int> PaiHangPosDict = null;

		// Token: 0x04002757 RID: 10071
		[ProtoMember(83)]
		public int JieBiaoDayID = 0;

		// Token: 0x04002758 RID: 10072
		[ProtoMember(84)]
		public int JieBiaoDayNum = 0;

		// Token: 0x04002759 RID: 10073
		[ProtoMember(85)]
		public int LastMailID = 0;

		// Token: 0x0400275A RID: 10074
		[ProtoMember(86)]
		public List<VipDailyData> VipDailyDataList = null;

		// Token: 0x0400275B RID: 10075
		[ProtoMember(87)]
		public YangGongBKDailyJiFenData YangGongBKDailyJiFen = null;

		// Token: 0x0400275C RID: 10076
		[ProtoMember(88)]
		public long OnceAwardFlag = 0L;

		// Token: 0x0400275D RID: 10077
		[ProtoMember(89)]
		public int Gold = 0;

		// Token: 0x0400275E RID: 10078
		[ProtoMember(90)]
		public List<GoodsLimitData> GoodsLimitDataList = null;

		// Token: 0x0400275F RID: 10079
		[ProtoMember(91)]
		public Dictionary<string, RoleParamsData> RoleParamsDict = null;

		// Token: 0x04002760 RID: 10080
		[ProtoMember(92)]
		public int BanChat = 0;

		// Token: 0x04002761 RID: 10081
		[ProtoMember(93)]
		public int BanLogin = 0;

		// Token: 0x04002762 RID: 10082
		[ProtoMember(94)]
		public int IsFlashPlayer = 0;

		// Token: 0x04002763 RID: 10083
		[ProtoMember(95)]
		public int ChangeLifeCount = 0;

		// Token: 0x04002764 RID: 10084
		[ProtoMember(96)]
		public int AdmiredCount = 0;

		// Token: 0x04002765 RID: 10085
		[ProtoMember(97)]
		public int CombatForce = 0;

		// Token: 0x04002766 RID: 10086
		[ProtoMember(98)]
		public int AutoAssignPropertyPoint = 0;

		// Token: 0x04002767 RID: 10087
		[ProtoMember(99)]
		public string PushMessageID = "";

		// Token: 0x04002768 RID: 10088
		[ProtoMember(100)]
		public WingData MyWingData = null;

		// Token: 0x04002769 RID: 10089
		[ProtoMember(101)]
		public Dictionary<int, int> RolePictureJudgeReferInfo = null;

		// Token: 0x0400276A RID: 10090
		[ProtoMember(102)]
		public Dictionary<int, int> RoleStarConstellationInfo = null;

		// Token: 0x0400276B RID: 10091
		[ProtoMember(103)]
		public int VIPLevel = 0;

		// Token: 0x0400276C RID: 10092
		[ProtoMember(104)]
		public List<GoodsData> ElementhrtsList = null;

		// Token: 0x0400276D RID: 10093
		[ProtoMember(105)]
		public List<GoodsData> UsingElementhrtsList = null;

		// Token: 0x0400276E RID: 10094
		[ProtoMember(106)]
		public List<GoodsData> PetList = null;

		// Token: 0x0400276F RID: 10095
		[ProtoMember(107)]
		public long Store_Yinliang = 0L;

		// Token: 0x04002770 RID: 10096
		[ProtoMember(108)]
		public long Store_Money = 0L;

		// Token: 0x04002771 RID: 10097
		[ProtoMember(109)]
		public Dictionary<int, LingYuData> LingYuDict = null;

		// Token: 0x04002772 RID: 10098
		[ProtoMember(110)]
		public List<GoodsData> DamonGoodsDataList = null;

		// Token: 0x04002773 RID: 10099
		[ProtoMember(111)]
		public int MagicSwordParam = 0;

		// Token: 0x04002774 RID: 10100
		[ProtoMember(112)]
		public MarriageData MyMarriageData = null;

		// Token: 0x04002775 RID: 10101
		[ProtoMember(113)]
		public Dictionary<int, int> MyMarryPartyJoinList = null;

		// Token: 0x04002776 RID: 10102
		[ProtoMember(114)]
		public List<int> GroupMailRecordList = null;

		// Token: 0x04002777 RID: 10103
		[ProtoMember(115)]
		public GuardStatueDetail MyGuardStatueDetail = null;

		// Token: 0x04002778 RID: 10104
		[ProtoMember(116, IsRequired = true)]
		public TalentData MyTalentData = null;

		// Token: 0x04002779 RID: 10105
		[ProtoMember(117)]
		public RoleTianTiData TianTiData;

		// Token: 0x0400277A RID: 10106
		[ProtoMember(118)]
		public List<GoodsData> FashionGoodsDataList = null;

		// Token: 0x0400277B RID: 10107
		[ProtoMember(119)]
		public MerlinGrowthSaveDBData MerlinData = null;

		// Token: 0x0400277C RID: 10108
		[ProtoMember(120)]
		public Dictionary<sbyte, HolyItemData> MyHolyItemDataDic = null;

		// Token: 0x0400277D RID: 10109
		[ProtoMember(121)]
		public FluorescentGemData FluorescentGemData = null;

		// Token: 0x0400277E RID: 10110
		[ProtoMember(122)]
		public int FluorescentPoint = 0;

		// Token: 0x0400277F RID: 10111
		[ProtoMember(123)]
		public List<BuildingData> BuildingDataList = null;

		// Token: 0x04002780 RID: 10112
		[ProtoMember(124)]
		public Dictionary<int, Dictionary<int, SevenDayItemData>> SevenDayActDict = null;

		// Token: 0x04002781 RID: 10113
		[ProtoMember(125)]
		public List<GoodsData> SoulStonesInBag = null;

		// Token: 0x04002782 RID: 10114
		[ProtoMember(126)]
		public List<GoodsData> SoulStonesInUsing = null;

		// Token: 0x04002783 RID: 10115
		[ProtoMember(127)]
		public long BantTradeToTicks;

		// Token: 0x04002784 RID: 10116
		[ProtoMember(128)]
		public UserMiniData userMiniData;

		// Token: 0x04002785 RID: 10117
		[ProtoMember(129)]
		public Dictionary<int, SpecActInfoDB> SpecActInfoDict;

		// Token: 0x04002786 RID: 10118
		[ProtoMember(130)]
		public TarotSystemData TarotData;

		// Token: 0x04002787 RID: 10119
		[ProtoMember(131)]
		public List<GoodsData> OrnamentGoodsList = null;

		// Token: 0x04002788 RID: 10120
		[ProtoMember(132)]
		public Dictionary<int, OrnamentData> OrnamentDataDict = null;

		// Token: 0x04002789 RID: 10121
		[ProtoMember(133)]
		public List<int> OccupationList;

		// Token: 0x0400278A RID: 10122
		[ProtoMember(134)]
		public int JunTuanId;

		// Token: 0x0400278B RID: 10123
		[ProtoMember(135)]
		public string JunTuanName;

		// Token: 0x0400278C RID: 10124
		[ProtoMember(136)]
		public int JunTuanZhiWu;

		// Token: 0x0400278D RID: 10125
		[ProtoMember(137)]
		public List<GoodsData> PaiZhuDamonGoodsDataList = null;

		// Token: 0x0400278E RID: 10126
		[ProtoMember(138)]
		public Dictionary<int, ShenJiFuWenData> ShenJiDict = null;

		// Token: 0x0400278F RID: 10127
		[ProtoMember(139)]
		public Dictionary<int, EverydayActInfoDB> EverydayActInfoDict;

		// Token: 0x04002790 RID: 10128
		[ProtoMember(140)]
		public RoleHuiJiData HuiJiData;

		// Token: 0x04002791 RID: 10129
		[ProtoMember(141)]
		public string UserID;

		// Token: 0x04002792 RID: 10130
		[ProtoMember(142)]
		public List<FuWenTabData> FuWenTabList;

		// Token: 0x04002793 RID: 10131
		[ProtoMember(143)]
		public List<GoodsData> FuWenGoodsDataList;

		// Token: 0x04002794 RID: 10132
		[ProtoMember(144)]
		public AlchemyDataDB AlchemyInfo;

		// Token: 0x04002795 RID: 10133
		[ProtoMember(145)]
		public JueXingShiData JueXingData;

		// Token: 0x04002796 RID: 10134
		[ProtoMember(146)]
		public int CompType;

		// Token: 0x04002797 RID: 10135
		[ProtoMember(147)]
		public byte CompZhiWu;

		// Token: 0x04002798 RID: 10136
		[ProtoMember(148)]
		public List<GoodsData> MountStoreList = null;

		// Token: 0x04002799 RID: 10137
		[ProtoMember(149)]
		public List<GoodsData> MountEquipList = null;

		// Token: 0x0400279A RID: 10138
		[ProtoMember(150)]
		public int SubOccupation;

		// Token: 0x0400279B RID: 10139
		[ProtoMember(151, IsRequired = true)]
		public RoleArmorData ArmorData;

		// Token: 0x0400279C RID: 10140
		[ProtoMember(152)]
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;

		// Token: 0x0400279D RID: 10141
		[ProtoMember(153)]
		public RoleBianShenData BianShenData;

		// Token: 0x0400279E RID: 10142
		[ProtoMember(154)]
		public List<GoodsData> RebornGoodsDataList = null;

		// Token: 0x0400279F RID: 10143
		[ProtoMember(156)]
		public int RebornCombatForce = 0;

		// Token: 0x040027A0 RID: 10144
		[ProtoMember(157)]
		public int RebornCount = 0;

		// Token: 0x040027A1 RID: 10145
		[ProtoMember(158)]
		public int RebornLevel = 0;

		// Token: 0x040027A2 RID: 10146
		[ProtoMember(159)]
		public long RebornExperience = 0L;

		// Token: 0x040027A3 RID: 10147
		[ProtoMember(160)]
		public List<GoodsData> RebornGoodsStoreList;

		// Token: 0x040027A4 RID: 10148
		[ProtoMember(161)]
		public int RebornBagNum = 0;

		// Token: 0x040027A5 RID: 10149
		[ProtoMember(162)]
		public RebornPortableBagData RebornGirdData = null;

		// Token: 0x040027A6 RID: 10150
		[ProtoMember(163)]
		public int RebornShowEquip = 0;

		// Token: 0x040027A7 RID: 10151
		[ProtoMember(164)]
		public RebornStampData RebornYinJi = null;

		// Token: 0x040027A8 RID: 10152
		[ProtoMember(165)]
		public int RebornShowModel = 0;

		// Token: 0x040027A9 RID: 10153
		[ProtoMember(166)]
		public Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> SpecPriorityActInfoDict;

		// Token: 0x040027AA RID: 10154
		[ProtoMember(167)]
		public int ZhanDuiID;

		// Token: 0x040027AB RID: 10155
		[ProtoMember(168)]
		public int ZhanDuiZhiWu;

		// Token: 0x040027AC RID: 10156
		[ProtoMember(169)]
		public List<GoodsData> HolyGoodsDataList = null;

		// Token: 0x040027AD RID: 10157
		[ProtoMember(170)]
		public Dictionary<int, RebornEquipData> RebornEquipHoleData = null;

		// Token: 0x040027AE RID: 10158
		[ProtoMember(171)]
		public Dictionary<int, MazingerStoreData> MazingerStoreDataInfo = null;

		// Token: 0x040027AF RID: 10159
		[ProtoMember(249)]
		public int ServerPTID;

		// Token: 0x040027B0 RID: 10160
		[ProtoMember(250)]
		public int UserPTID;

		// Token: 0x040027B1 RID: 10161
		[ProtoMember(251)]
		public string WorldRoleID;

		// Token: 0x040027B2 RID: 10162
		[ProtoMember(252)]
		public string Channel;
	}
}
