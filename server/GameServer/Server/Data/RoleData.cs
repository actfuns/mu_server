using System;
using System.Collections.Generic;
using GameServer.Logic.Olympics;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName = "";

		
		[ProtoMember(3)]
		public int RoleSex = 0;

		
		[ProtoMember(4)]
		public int Occupation = 0;

		
		[ProtoMember(5)]
		public int Level = 1;

		
		[ProtoMember(6)]
		public int Faction = 0;

		
		[ProtoMember(7)]
		public int Money1 = 0;

		
		[ProtoMember(8)]
		public int Money2 = 0;

		
		[ProtoMember(9)]
		public long Experience = 0L;

		
		[ProtoMember(10)]
		public int PKMode = 0;

		
		[ProtoMember(11)]
		public int PKValue = 0;

		
		[ProtoMember(12)]
		public int MapCode = 0;

		
		[ProtoMember(13)]
		public int PosX = 0;

		
		[ProtoMember(14)]
		public int PosY = 0;

		
		[ProtoMember(15)]
		public int RoleDirection = 0;

		
		[ProtoMember(16)]
		public int LifeV = 0;

		
		[ProtoMember(17)]
		public int MaxLifeV = 0;

		
		[ProtoMember(18)]
		public int MagicV = 0;

		
		[ProtoMember(19)]
		public int MaxMagicV = 0;

		
		[ProtoMember(20)]
		public int RolePic = 0;

		
		[ProtoMember(21)]
		public int BagNum = 0;

		
		[ProtoMember(22)]
		public List<TaskData> TaskDataList;

		
		[ProtoMember(23)]
		public List<GoodsData> GoodsDataList;

		
		[ProtoMember(24)]
		public int BodyCode;

		
		[ProtoMember(25)]
		public int WeaponCode;

		
		[ProtoMember(26)]
		public List<SkillData> SkillDataList;

		
		[ProtoMember(27)]
		public string OtherName;

		
		[ProtoMember(28)]
		public List<NPCTaskState> NPCTaskStateList;

		
		[ProtoMember(29)]
		public string MainQuickBarKeys = "";

		
		[ProtoMember(30)]
		public string OtherQuickBarKeys = "";

		
		[ProtoMember(31)]
		public int LoginNum = 0;

		
		[ProtoMember(32)]
		public int UserMoney = 0;

		
		[ProtoMember(33)]
		public string StallName;

		
		[ProtoMember(34)]
		public int TeamID;

		
		[ProtoMember(35)]
		public int LeftFightSeconds = 0;

		
		[ProtoMember(36)]
		public int TotalHorseCount = 0;

		
		[ProtoMember(37)]
		public int HorseDbID = -1;

		
		[ProtoMember(38)]
		public int TotalPetCount = 0;

		
		[ProtoMember(39)]
		public int PetDbID = -1;

		
		[ProtoMember(40)]
		public int InterPower = 0;

		
		[ProtoMember(41)]
		public int TeamLeaderRoleID = 0;

		
		[ProtoMember(42)]
		public int YinLiang = 0;

		
		[ProtoMember(43)]
		public int JingMaiBodyLevel = 0;

		
		[ProtoMember(44)]
		public int JingMaiXueWeiNum = 0;

		
		[ProtoMember(45)]
		public int LastHorseID = 0;

		
		[ProtoMember(46)]
		public int DefaultSkillID = -1;

		
		[ProtoMember(47)]
		public int AutoLifeV = 0;

		
		[ProtoMember(48)]
		public int AutoMagicV = 0;

		
		[ProtoMember(49)]
		public List<BufferData> BufferDataList = null;

		
		[ProtoMember(50)]
		public List<DailyTaskData> MyDailyTaskDataList = null;

		
		[ProtoMember(51)]
		public int JingMaiOkNum = 0;

		
		[ProtoMember(52)]
		public DailyJingMaiData MyDailyJingMaiData = null;

		
		[ProtoMember(53)]
		public int NumSkillID = 0;

		
		[ProtoMember(54)]
		public PortableBagData MyPortableBagData = null;

		
		[ProtoMember(55)]
		public int NewStep = 0;

		
		[ProtoMember(56)]
		public long StepTime = 0L;

		
		[ProtoMember(57)]
		public int BigAwardID = 0;

		
		[ProtoMember(58)]
		public int SongLiID = 0;

		
		[ProtoMember(59)]
		public List<FuBenData> FuBenDataList = null;

		
		[ProtoMember(60)]
		public int TotalLearnedSkillLevelCount = 0;

		
		[ProtoMember(61)]
		public int CompletedMainTaskID = 0;

		
		[ProtoMember(62)]
		public int PKPoint = 0;

		
		[ProtoMember(63)]
		public int LianZhan = 0;

		
		[ProtoMember(64)]
		public long StartPurpleNameTicks = 0L;

		
		[ProtoMember(65)]
		public YaBiaoData MyYaBiaoData = null;

		
		[ProtoMember(66)]
		public long BattleNameStart = 0L;

		
		[ProtoMember(67)]
		public int BattleNameIndex = 0;

		
		[ProtoMember(68)]
		public int CZTaskID = 0;

		
		[ProtoMember(69)]
		public int HeroIndex = 0;

		
		[ProtoMember(70)]
		public int AllQualityIndex = 0;

		
		[ProtoMember(71)]
		public int AllForgeLevelIndex = 0;

		
		[ProtoMember(72)]
		public int AllJewelLevelIndex = 0;

		
		[ProtoMember(73)]
		public int HalfYinLiangPeriod = 0;

		
		[ProtoMember(74)]
		public int ZoneID = 0;

		
		[ProtoMember(75)]
		public string BHName = "";

		
		[ProtoMember(76)]
		public int BHVerify = 0;

		
		[ProtoMember(77)]
		public int BHZhiWu = 0;

		
		[ProtoMember(78)]
		public int BangGong = 0;

		
		[ProtoMember(79)]
		public Dictionary<int, BangHuiLingDiItemData> BangHuiLingDiItemsDict = null;

		
		[ProtoMember(80)]
		public int HuangDiRoleID = 0;

		
		[ProtoMember(81)]
		public int HuangHou = 0;

		
		[ProtoMember(82)]
		public Dictionary<int, int> PaiHangPosDict = null;

		
		[ProtoMember(83)]
		public int AutoFightingProtect = 0;

		
		[ProtoMember(84)]
		public long FSHuDunStart = 0L;

		
		[ProtoMember(85)]
		public int BattleWhichSide = -1;

		
		[ProtoMember(86)]
		public int LastMailID = 0;

		
		[ProtoMember(87)]
		public int IsVIP = 0;

		
		[ProtoMember(88)]
		public long OnceAwardFlag = 0L;

		
		[ProtoMember(89)]
		public int Gold = 0;

		
		[ProtoMember(90)]
		public long DSHideStart = 0L;

		
		[ProtoMember(91)]
		public List<int> RoleCommonUseIntPamams = new List<int>();

		
		[ProtoMember(92)]
		public int FSHuDunSeconds = 0;

		
		[ProtoMember(93)]
		public long ZhongDuStart = 0L;

		
		[ProtoMember(94)]
		public int ZhongDuSeconds = 0;

		
		[ProtoMember(95)]
		public string KaiFuStartDay = "";

		
		[ProtoMember(96)]
		public string RegTime = "";

		
		[ProtoMember(97)]
		public string JieriStartDay = "";

		
		[ProtoMember(98)]
		public int JieriDaysNum = 0;

		
		[ProtoMember(99)]
		public string HefuStartDay = "";

		
		[ProtoMember(100)]
		public int JieriChengHao = 0;

		
		[ProtoMember(101)]
		public string BuChangStartDay = "";

		
		[ProtoMember(102)]
		public long DongJieStart = 0L;

		
		[ProtoMember(103)]
		public int DongJieSeconds = 0;

		
		[ProtoMember(104)]
		public string YueduDazhunpanStartDay = "";

		
		[ProtoMember(105)]
		public int YueduDazhunpanStartDayNum = 0;

		
		[ProtoMember(106)]
		public int RoleStrength = 0;

		
		[ProtoMember(107)]
		public int RoleIntelligence = 0;

		
		[ProtoMember(108)]
		public int RoleDexterity = 0;

		
		[ProtoMember(109)]
		public int RoleConstitution = 0;

		
		[ProtoMember(110)]
		public int ChangeLifeCount = 0;

		
		[ProtoMember(111)]
		public int TotalPropPoint = 0;

		
		[ProtoMember(112)]
		public int IsFlashPlayer = 0;

		
		[ProtoMember(113)]
		public int AdmiredCount = 0;

		
		[ProtoMember(114)]
		public int CombatForce = 0;

		
		[ProtoMember(115)]
		public int AdorationCount = 0;

		
		[ProtoMember(116)]
		public int DayOnlineSecond = 0;

		
		[ProtoMember(117)]
		public int SeriesLoginNum = 0;

		
		[ProtoMember(118)]
		public int AutoAssignPropertyPoint = 0;

		
		[ProtoMember(119)]
		public int OnLineTotalTime = 0;

		
		[ProtoMember(120)]
		public int AllZhuoYueNum = 0;

		
		[ProtoMember(121)]
		public int VIPLevel = 0;

		
		[ProtoMember(122)]
		public int OpenGridTime = 0;

		
		[ProtoMember(123)]
		public int OpenPortableGridTime = 0;

		
		[ProtoMember(124)]
		public WingData MyWingData = null;

		
		[ProtoMember(125)]
		public Dictionary<int, int> PictureJudgeReferInfo = null;

		
		[ProtoMember(126)]
		public int StarSoulValue = 0;

		
		[ProtoMember(127)]
		public long StoreYinLiang = 0L;

		
		[ProtoMember(128)]
		public long StoreMoney = 0L;

		
		[ProtoMember(129)]
		public string UserReturnTimeBegin = "";

		
		[ProtoMember(130)]
		public string UserReturnTimeEnd = "";

		
		[ProtoMember(131)]
		public TalentData MyTalentData = null;

		
		[ProtoMember(132)]
		public int TianTiRongYao;

		
		[ProtoMember(133)]
		public FluorescentGemData FluorescentGemData = null;

		
		[ProtoMember(134)]
		public int GMAuth = 0;

		
		[ProtoMember(135)]
		public SoulStoneData SoulStoneData = null;

		
		[ProtoMember(136)]
		public long SettingBitFlags;

		
		[ProtoMember(137)]
		public int SpouseId;

		
		[ProtoMember(138)]
		public List<ActivityData> ActivityList = null;

		
		[ProtoMember(139)]
		public List<int> OccupationList;

		
		[ProtoMember(140)]
		public int JunTuanId;

		
		[ProtoMember(141)]
		public string JunTuanName;

		
		[ProtoMember(142)]
		public int JunTuanZhiWu;

		
		[ProtoMember(143)]
		public int LingDi;

		
		[ProtoMember(144)]
		public Dictionary<int, ShenJiFuWenData> ShenJiDict = null;

		
		[ProtoMember(145)]
		public RoleHuiJiData HuiJiData;

		
		[ProtoMember(146)]
		public List<FuWenTabData> FuWenTabList;

		
		[ProtoMember(147)]
		public int HideGM;

		
		[ProtoMember(148)]
		public JueXingShiData JueXingData;

		
		[ProtoMember(149, IsRequired = true)]
		public LongCollection MoneyData;

		
		[ProtoMember(150)]
		public int CompType;

		
		[ProtoMember(151)]
		public byte CompZhiWu;

		
		[ProtoMember(152)]
		public List<GoodsData> MountStoreList;

		
		[ProtoMember(153)]
		public List<GoodsData> MountEquipList;

		
		[ProtoMember(154)]
		public List<GoodsLimitData> GoodsLimitDataList;

		
		[ProtoMember(155, IsRequired = true)]
		public SystemOpenData OpenData;

		
		[ProtoMember(156)]
		public int ThemeState;

		
		[ProtoMember(157)]
		public int SubOccupation;

		
		[ProtoMember(158, IsRequired = true)]
		public RoleArmorData ArmorData;

		
		[ProtoMember(159)]
		public int CurrentArmorV;

		
		[ProtoMember(160)]
		public int MaxArmorV;

		
		[ProtoMember(161)]
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;

		
		[ProtoMember(162)]
		public RoleBianShenData BianShenData;

		
		[ProtoMember(163)]
		public int RebornCombatForce = 0;

		
		[ProtoMember(164)]
		public int RebornCount = 0;

		
		[ProtoMember(165)]
		public int RebornLevel = 0;

		
		[ProtoMember(166)]
		public long RebornExperience = 0L;

		
		[ProtoMember(167)]
		public List<GoodsData> RebornGoodsDataList;

		
		[ProtoMember(168)]
		public List<GoodsData> RebornGoodsStoreList;

		
		[ProtoMember(169)]
		public int RebornBagNum = 0;

		
		[ProtoMember(170)]
		public RebornPortableBagData RebornGirdData = null;

		
		[ProtoMember(171)]
		public int OpenRebornGridTime = 0;

		
		[ProtoMember(172)]
		public int OpenRebornPortableGridTime = 0;

		
		[ProtoMember(173)]
		public int RebornShowEquip = 0;

		
		[ProtoMember(174)]
		public RebornStampData RebornYinJi = null;

		
		[ProtoMember(175)]
		public int RebornShowModel = 0;

		
		[ProtoMember(176)]
		public int ZhanDuiID;

		
		[ProtoMember(177)]
		public int ZhanDuiZhiWu;

		
		[ProtoMember(178)]
		public List<GoodsData> HolyGoodsDataList;

		
		[ProtoMember(179)]
		public Dictionary<int, RebornEquipData> RebornEquipHoleData = null;

		
		[ProtoMember(180)]
		public Dictionary<int, MazingerStoreData> MazingerStoreDataInfo = null;

		
		[ProtoMember(250)]
		public int UserPTID;

		
		[ProtoMember(251)]
		public string WorldRoleID;

		
		[ProtoMember(252)]
		public string Channel;
	}
}
