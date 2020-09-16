using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.Data.Tarot;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleDataEx
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
		public int MagicV = 0;

		
		[ProtoMember(18)]
		public List<OldTaskData> OldTasks = null;

		
		[ProtoMember(19)]
		public int RolePic = 0;

		
		[ProtoMember(20)]
		public int BagNum = 0;

		
		[ProtoMember(21)]
		public List<TaskData> TaskDataList = null;

		
		[ProtoMember(22)]
		public List<GoodsData> GoodsDataList = null;

		
		[ProtoMember(23)]
		public string OtherName = "";

		
		[ProtoMember(24)]
		public string MainQuickBarKeys = "";

		
		[ProtoMember(25)]
		public string OtherQuickBarKeys = "";

		
		[ProtoMember(26)]
		public int LoginNum = 0;

		
		[ProtoMember(27)]
		public int UserMoney = 0;

		
		[ProtoMember(28)]
		public int LeftFightSeconds = 0;

		
		[ProtoMember(29)]
		public List<FriendData> FriendDataList = null;

		
		[ProtoMember(30)]
		public List<HorseData> HorsesDataList = null;

		
		[ProtoMember(31)]
		public int HorseDbID = 0;

		
		[ProtoMember(32)]
		public List<PetData> PetsDataList = null;

		
		[ProtoMember(33)]
		public int PetDbID = 0;

		
		[ProtoMember(34)]
		public int InterPower = 0;

		
		[ProtoMember(35)]
		public List<JingMaiData> JingMaiDataList = null;

		
		[ProtoMember(36)]
		public int DJPoint = 0;

		
		[ProtoMember(37)]
		public int DJTotal = 0;

		
		[ProtoMember(38)]
		public int DJWincnt = 0;

		
		[ProtoMember(39)]
		public int TotalOnlineSecs = 0;

		
		[ProtoMember(40)]
		public int AntiAddictionSecs = 0;

		
		[ProtoMember(41)]
		public long LastOfflineTime = 0L;

		
		[ProtoMember(42)]
		public long BiGuanTime = 0L;

		
		[ProtoMember(43)]
		public int YinLiang = 0;

		
		[ProtoMember(44)]
		public List<SkillData> SkillDataList = null;

		
		[ProtoMember(45)]
		public int TotalJingMaiExp = 0;

		
		[ProtoMember(46)]
		public int JingMaiExpNum = 0;

		
		[ProtoMember(47)]
		public long RegTime = 0L;

		
		[ProtoMember(48)]
		public int LastHorseID = 0;

		
		[ProtoMember(49)]
		public List<GoodsData> SaleGoodsDataList = null;

		
		[ProtoMember(50)]
		public int DefaultSkillID = -1;

		
		[ProtoMember(51)]
		public int AutoLifeV = 0;

		
		[ProtoMember(52)]
		public int AutoMagicV = 0;

		
		[ProtoMember(53)]
		public List<BufferData> BufferDataList = null;

		
		[ProtoMember(54)]
		public List<DailyTaskData> MyDailyTaskDataList = null;

		
		[ProtoMember(55)]
		public DailyJingMaiData MyDailyJingMaiData = null;

		
		[ProtoMember(56)]
		public int NumSkillID = 0;

		
		[ProtoMember(57)]
		public PortableBagData MyPortableBagData = null;

		
		[ProtoMember(58)]
		public HuodongData MyHuodongData = null;

		
		[ProtoMember(59)]
		public List<FuBenData> FuBenDataList = null;

		
		[ProtoMember(60)]
		public int MainTaskID = 0;

		
		[ProtoMember(61)]
		public int PKPoint = 0;

		
		[ProtoMember(62)]
		public int LianZhan = 0;

		
		[ProtoMember(63)]
		public RoleDailyData MyRoleDailyData = null;

		
		[ProtoMember(64)]
		public int KillBoss = 0;

		
		[ProtoMember(65)]
		public YaBiaoData MyYaBiaoData = null;

		
		[ProtoMember(66)]
		public long BattleNameStart = 0L;

		
		[ProtoMember(67)]
		public int BattleNameIndex = 0;

		
		[ProtoMember(68)]
		public int CZTaskID = 0;

		
		[ProtoMember(69)]
		public int BattleNum = 0;

		
		[ProtoMember(70)]
		public int HeroIndex = 0;

		
		[ProtoMember(71)]
		public int ZoneID = 0;

		
		[ProtoMember(72)]
		public string BHName = "";

		
		[ProtoMember(73)]
		public int BHVerify = 0;

		
		[ProtoMember(74)]
		public int BHZhiWu = 0;

		
		[ProtoMember(75)]
		public int BGDayID1 = 0;

		
		[ProtoMember(76)]
		public int BGMoney = 0;

		
		[ProtoMember(77)]
		public int BGDayID2 = 0;

		
		[ProtoMember(78)]
		public int BGGoods = 0;

		
		[ProtoMember(80)]
		public int BangGong = 0;

		
		[ProtoMember(81)]
		public int HuangHou = 0;

		
		[ProtoMember(82)]
		public Dictionary<int, int> PaiHangPosDict = null;

		
		[ProtoMember(83)]
		public int JieBiaoDayID = 0;

		
		[ProtoMember(84)]
		public int JieBiaoDayNum = 0;

		
		[ProtoMember(85)]
		public int LastMailID = 0;

		
		[ProtoMember(86)]
		public List<VipDailyData> VipDailyDataList = null;

		
		[ProtoMember(87)]
		public YangGongBKDailyJiFenData YangGongBKDailyJiFen = null;

		
		[ProtoMember(88)]
		public long OnceAwardFlag = 0L;

		
		[ProtoMember(89)]
		public int Gold = 0;

		
		[ProtoMember(90)]
		public List<GoodsLimitData> GoodsLimitDataList = null;

		
		[ProtoMember(91)]
		public Dictionary<string, RoleParamsData> RoleParamsDict = null;

		
		[ProtoMember(92)]
		public int BanChat = 0;

		
		[ProtoMember(93)]
		public int BanLogin = 0;

		
		[ProtoMember(94)]
		public int IsFlashPlayer = 0;

		
		[ProtoMember(95)]
		public int ChangeLifeCount = 0;

		
		[ProtoMember(96)]
		public int AdmiredCount = 0;

		
		[ProtoMember(97)]
		public int CombatForce = 0;

		
		[ProtoMember(98)]
		public int AutoAssignPropertyPoint = 0;

		
		[ProtoMember(99)]
		public string PushMessageID = "";

		
		[ProtoMember(100)]
		public WingData MyWingData = null;

		
		[ProtoMember(101)]
		public Dictionary<int, int> RolePictureJudgeReferInfo = null;

		
		[ProtoMember(102)]
		public Dictionary<int, int> RoleStarConstellationInfo = null;

		
		[ProtoMember(103)]
		public int VIPLevel = 0;

		
		[ProtoMember(104)]
		public List<GoodsData> ElementhrtsList = null;

		
		[ProtoMember(105)]
		public List<GoodsData> UsingElementhrtsList = null;

		
		[ProtoMember(106)]
		public List<GoodsData> PetList = null;

		
		[ProtoMember(107)]
		public long Store_Yinliang = 0L;

		
		[ProtoMember(108)]
		public long Store_Money = 0L;

		
		[ProtoMember(109)]
		public Dictionary<int, LingYuData> LingYuDict = null;

		
		[ProtoMember(110)]
		public List<GoodsData> DamonGoodsDataList = null;

		
		[ProtoMember(111)]
		public int MagicSwordParam = 0;

		
		[ProtoMember(112)]
		public MarriageData MyMarriageData = null;

		
		[ProtoMember(113)]
		public Dictionary<int, int> MyMarryPartyJoinList = null;

		
		[ProtoMember(114)]
		public List<int> GroupMailRecordList = null;

		
		[ProtoMember(115)]
		public GuardStatueDetail MyGuardStatueDetail = null;

		
		[ProtoMember(116, IsRequired = true)]
		public TalentData MyTalentData = null;

		
		[ProtoMember(117, IsRequired = true)]
		public RoleTianTiData TianTiData;

		
		[ProtoMember(118)]
		public List<GoodsData> FashionGoodsDataList = null;

		
		[ProtoMember(119)]
		public MerlinGrowthSaveDBData MerlinData = null;

		
		[ProtoMember(120)]
		public Dictionary<sbyte, HolyItemData> MyHolyItemDataDic = null;

		
		[ProtoMember(121)]
		public FluorescentGemData FluorescentGemData = null;

		
		[ProtoMember(122)]
		public int FluorescentPoint = 0;

		
		[ProtoMember(123)]
		public List<BuildingData> BuildingDataList = null;

		
		[ProtoMember(124)]
		public Dictionary<int, Dictionary<int, SevenDayItemData>> SevenDayActDict = null;

		
		[ProtoMember(125)]
		public List<GoodsData> SoulStonesInBag = null;

		
		[ProtoMember(126)]
		public List<GoodsData> SoulStonesInUsing = null;

		
		[ProtoMember(127)]
		public long BanTradeToTicks;

		
		[ProtoMember(128)]
		public UserMiniData userMiniData;

		
		[ProtoMember(129)]
		public Dictionary<int, SpecActInfoDB> SpecActInfoDict;

		
		[ProtoMember(130)]
		public TarotSystemData TarotData;

		
		[ProtoMember(131)]
		public List<GoodsData> OrnamentGoodsList = null;

		
		[ProtoMember(132)]
		public Dictionary<int, OrnamentData> OrnamentDataDict = null;

		
		[ProtoMember(133)]
		public List<int> OccupationList;

		
		[ProtoMember(136)]
		public int JunTuanZhiWu;

		
		[ProtoMember(137)]
		public List<GoodsData> PaiZhuDamonGoodsDataList = null;

		
		[ProtoMember(138)]
		public Dictionary<int, ShenJiFuWenData> ShenJiDict = null;

		
		[ProtoMember(139)]
		public Dictionary<int, EverydayActInfoDB> EverydayActInfoDict;

		
		[ProtoMember(140)]
		public RoleHuiJiData HuiJiData;

		
		[ProtoMember(141)]
		public string UserID;

		
		[ProtoMember(142)]
		public List<FuWenTabData> FuWenTabList;

		
		[ProtoMember(143)]
		public List<GoodsData> FuWenGoodsDataList;

		
		[ProtoMember(144)]
		public AlchemyDataDB AlchemyInfo;

		
		[ProtoMember(150)]
		public int SubOccupation;

		
		[ProtoMember(151, IsRequired = true)]
		public RoleArmorData ArmorData;

		
		[ProtoMember(152)]
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;

		
		[ProtoMember(153)]
		public RoleBianShenData BianShenData;

		
		[ProtoMember(154)]
		public List<GoodsData> RebornGoodsDataList;

		
		[ProtoMember(155)]
		public List<FuMoMailData> FumoMail;

		
		[ProtoMember(156)]
		public int RebornCombatForce = 0;

		
		[ProtoMember(157)]
		public int RebornCount = 0;

		
		[ProtoMember(158)]
		public int RebornLevel = 0;

		
		[ProtoMember(159)]
		public long RebornExperience = 0L;

		
		[ProtoMember(160)]
		public List<GoodsData> RebornGoodsStoreList;

		
		[ProtoMember(161)]
		public int RebornBagNum = 0;

		
		[ProtoMember(162)]
		public RebornPortableBagData RebornGirdData = null;

		
		[ProtoMember(163)]
		public int RebornShowEquip = 0;

		
		[ProtoMember(164)]
		public RebornStampData RebornYinJi = null;

		
		[ProtoMember(165)]
		public int RebornShowModel = 0;

		
		[ProtoMember(166)]
		public Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> SpecPriorityActInfoDict;

		
		[ProtoMember(167)]
		public int ZhanDuiID;

		
		[ProtoMember(168)]
		public int ZhanDuiZhiWu;

		
		[ProtoMember(169)]
		public List<GoodsData> HolyGoodsDataList = null;

		
		[ProtoMember(170)]
		public Dictionary<int, RebornEquipData> RebornEquipHoleData = null;

		
		[ProtoMember(171)]
		public Dictionary<int, MazingerStoreData> MazingerStoreDataInfo = null;

		
		[ProtoMember(249)]
		public int PTID;

		
		[ProtoMember(250)]
		public int UserPTID;

		
		[ProtoMember(251)]
		public string WorldRoleID;

		
		[ProtoMember(252)]
		public string Channel;
	}
}
