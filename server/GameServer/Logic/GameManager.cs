using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using GameServer.Logic.ElementsAttack;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.LoginWaiting;
using GameServer.Logic.MagicSword;
using GameServer.Logic.MerlinMagicBook;
using GameServer.Logic.Summoner;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class GameManager
	{
		
		static GameManager()
		{
			GameManager.ResetHideFlagsMaps(true, 1);
		}

		
		public static void ResetHideFlagsMaps(bool enable, int type)
		{
			try
			{
				GameManager.HideFlagsMapDict.Clear();
				GameManager.FlagEnableHideFlags = enable;
				GameManager.FlagHideFlagsType = type;
				GameManager.HideFlagsMapDict.TryAdd(3000, 1);
				if (GameManager.FlagHideFlagsType == 1)
				{
					GameManager.HideFlagsMapDict.TryAdd(5300, 1);
					GameManager.HideFlagsMapDict.TryAdd(5301, 1);
					GameManager.HideFlagsMapDict.TryAdd(5302, 1);
					GameManager.HideFlagsMapDict.TryAdd(5303, 1);
					GameManager.HideFlagsMapDict.TryAdd(7000, 1);
					GameManager.HideFlagsMapDict.TryAdd(7001, 1);
					GameManager.HideFlagsMapDict.TryAdd(7002, 1);
					GameManager.HideFlagsMapDict.TryAdd(7003, 1);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public static void SetLogFlags(long flags)
		{
			GameManager.FlagEnableMoneyEventLog = ((flags & 1L) != 0L);
			GameManager.FlagEnableGoodsEventLog = ((flags & 2L) != 0L);
			GameManager.FlagEnableGameEventLog = ((flags & 4L) != 0L);
			GameManager.FlagEnableOperatorEventLog = ((flags & 8L) != 0L);
			GameManager.FlagEnableRoleSkillLog = ((flags & 16L) != 0L);
			GameManager.FlagEnablePetSkillLog = ((flags & 32L) != 0L);
			GameManager.FlagEnableUnionPalaceLog = ((flags & 64L) != 0L);
		}

		
		public static void LoadGameConfigFlags()
		{
			GameManager.FlagRecalcRolePropsTicks = (long)GameManager.GameConfigMgr.GetGameConfigItemInt("recalcrolepropsticks", 700);
			GameManager.FlagCheckCmdPosition = GameManager.GameConfigMgr.GetGameConfigItemInt("check_cmd_position", 1);
			GameManager.ConstCheckServerTimeDiffMinutes = GameManager.GameConfigMgr.GetGameConfigItemInt("checkservertime", 5);
		}

		
		public const int ServerLineIdAllIncludeKuaFu = 0;

		
		public const int ServerLineIdAllLineExcludeSelf = -1000;

		
		public const int ClientViewSlot_Direct = 0;

		
		public const int ClientViewSlot_Pos = 1;

		
		public const int MaxSlotCount = 5;

		
		public const int Update9GridUsingNewMode = 0;

		
		public const int Update9GridDelayFlag = 1;

		
		public const int Update9GridBoostFlag = 2;

		
		public const bool FlagManyAttack = true;

		
		public const bool FlagManyAttackOp = true;

		
		public const bool FlagOptimizeLock = true;

		
		public const bool FlagOptimizeLock2 = true;

		
		public const bool FlagOptimizeLock3 = true;

		
		public const bool FlagOptimizePathString = false;

		
		public const bool FlagOptimizeLockTrace = false;

		
		public const bool FlagOptimizeAlgorithm = true;

		
		public const bool FlagOptimizeThreadPool = true;

		
		public const bool FlagOptimizeThreadPool2 = false;

		
		public const bool FlagOptimizeThreadPool3 = true;

		
		public const bool FlagOptimizeThreadPool4 = true;

		
		public const bool FlagOptimizeThreadPool5 = true;

		
		public const bool FlagSkipSendDataCall = false;

		
		public const bool FlagSkipAddBuffCall = false;

		
		public const bool FlagSkipTrySendCall = false;

		
		public const bool FlagSkipSocketSend = false;

		
		public const bool FlagTraceMemoryPool = false;

		
		public const bool FlagTraceTCPEvent = false;

		
		public const bool FlagTracePropsValues = false;

		
		public const bool FlagDisableNameServer = true;

		
		public const int CostSkipSendCount = 900;

		
		public const bool Flag_Cache_VisiableObject = false;

		
		public const bool FlagOptimizeAlgorithm_Props = true;

		
		public const bool FlagEnableMultiLineServer = false;

		
		public const int LocalServerId = 0;

		
		public const int LocalServerIdForNotImplement = 0;

		
		public const int OPT_ChengZhanType = 1;

		
		public const bool OPT_OldJuQiConfig = true;

		
		public static int MapGridWidth = 100;

		
		public static int MapGridHeight = 100;

		
		public static Program AppMainWnd = null;

		
		public static int DefaultMapCode = 1;

		
		public static int MainMapCode = 2;

		
		public static int NewDefaultMapCode = 1;

		
		public static int ServerLineID = 1;

		
		public static int ServerId = 1;

		
		public static int KuaFuServerId = 1;

		
		public static int PTID = 0;

		
		public static PlatformTypes PlatformType = PlatformTypes.Tmsk;

		
		public static List<int> AutoGiveGoodsIDPortableList = null;

		
		public static List<int> AutoGiveGoodsIDList = null;

		
		public static int MaxSlotOnUpdate9GridsTicks = 1000;

		
		public static int MaxSleepOnDoMapGridMoveTicks = 5;

		
		public static int MaxCachingMonsterToClientBytesDataTicks = 30000;

		
		public static int MaxCachingClientToClientBytesDataTicks = 30000;

		
		public static int Update9GridUsingPosition = 1;

		
		public static int MaxSlotOnPositionUpdate9GridsTicks = 2000;

		
		public static int RoleDataMiniMode = 1;

		
		public static int FlagSleepTime = 0;

		
		public static int FlagLiXianGuaJi = 0;

		
		public static int ConstCheckServerTimeDiffMinutes = 3;

		
		public static int StatisticsMode = 1;

		
		public static bool Flag_OptimizationBagReset = true;

		
		public static Dictionary<int, int> MemoryPoolConfigDict = new Dictionary<int, int>();

		
		public static bool TestGamePerformanceMode = false;

		
		public static List<Point> TestBirthPointList1 = new List<Point>();

		
		public static List<Point> TestBirthPointList2 = new List<Point>();

		
		public static int TestGamePerformanceMapCode = 1;

		
		public static int TestGamePerformanceMapMode = 0;

		
		public static bool TestGamePerformanceAllPK = false;

		
		public static bool TestGamePerformanceLockLifeV = true;

		
		public static bool TestGamePerformanceForAllUser = false;

		
		public static bool FlagUseWin32Decrypt = true;

		
		public static bool TestGameShowFakeRoleForUser = false;

		
		public static int[][] TestRoleEquipsArrays = new int[][]
		{
			new int[]
			{
				1005005,
				1005005,
				1000105,
				1000005,
				1000505,
				1000205,
				1000605,
				1000605,
				1000305,
				1000405,
				1032212
			},
			new int[]
			{
				1015105,
				1000105,
				1010005,
				1010505,
				1010205,
				1010605,
				1010605,
				1010305,
				1010405,
				1032212
			},
			new int[]
			{
				1025405,
				1025505,
				1020105,
				1020005,
				1020505,
				1020205,
				1020605,
				1020605,
				1020305,
				1020405,
				1032212
			},
			new int[]
			{
				1005006,
				1005006,
				1000106,
				1000006,
				1000506,
				1000206,
				1000606,
				1000606,
				1000306,
				1000406,
				1032212
			}
		};

		
		public static long GM_NoCheckTokenTimeRemainMS = 0L;

		
		public static bool FlagAlowUnRegistedCmd = false;

		
		public static bool CheckMismatchMapCode = true;

		
		public static bool CheckCheatPosition = true;

		
		public static double CheckPositionInterval = 5000.0;

		
		public static double CheckCheatMaxDistance = 600.0;

		
		public static int MaxAttackDistance = 1500;

		
		public static bool ServerStarting = true;

		
		public static bool FlagDisableMovingOnAttack = true;

		
		public static bool FlaDisablegFilterMonsterDeadEvent = false;

		
		public static bool FlagKuaFuServerExplicit = true;

		
		public static bool IsKuaFuServer = false;

		
		public static ConcurrentDictionary<int, int> HideFlagsMapDict = new ConcurrentDictionary<int, int>();

		
		public static bool FlagEnableHideFlags = false;

		
		public static int FlagHideFlagsType = 0;

		
		public static int OnceDestroyCopyMapNum = 100;

		
		public static bool FlagEnableMoneyEventLog = true;

		
		public static bool FlagEnableGoodsEventLog = true;

		
		public static bool FlagEnableGameEventLog = true;

		
		public static bool FlagEnableOperatorEventLog = true;

		
		public static bool FlagEnableRoleSkillLog = true;

		
		public static bool FlagEnablePetSkillLog = true;

		
		public static bool FlagEnableUnionPalaceLog = true;

		
		public static long FlagRecalcRolePropsTicks = 700L;

		
		public static int FlagCheckCmdPosition = 1;

		
		public static LuaManager LuaMgr = new LuaManager();

		
		public static UserSession OnlineUserSession = new UserSession();

		
		public static KuaFuManager KuaFuMgr = KuaFuManager.getInstance();

		
		public static MonsterIDManager MonsterIDMgr = new MonsterIDManager();

		
		public static PetIDManager PetIDMgr = new PetIDManager();

		
		public static BiaoCheIDManager BiaoCheIDMgr = new BiaoCheIDManager();

		
		public static JunQiIDManager JunQiIDMgr = new JunQiIDManager();

		
		public static FakeRoleIDManager FakeRoleIDMgr = new FakeRoleIDManager();

		
		public static MapManager MapMgr = new MapManager();

		
		public static MapGridManager MapGridMgr = new MapGridManager();

		
		public static ClientManager ClientMgr = new ClientManager();

		
		public static MonsterZoneManager MonsterZoneMgr = new MonsterZoneManager();

		
		public static MonsterManager MonsterMgr = new MonsterManager();

		
		public static DBCmdManager DBCmdMgr = new DBCmdManager();

		
		public static LogDBCmdManager logDBCmdMgr = new LogDBCmdManager();

		
		public static NPCSaleList NPCSaleListMgr = new NPCSaleList();

		
		public static SystemGoodsManager SystemGoodsNamgMgr = new SystemGoodsManager();

		
		public static TaskAwards TaskAwardsMgr = new TaskAwards();

		
		public static EquipProps EquipPropsMgr = new EquipProps();

		
		public static SystemMagicManager SystemMagicQuickMgr = new SystemMagicManager();

		
		public static SystemMagicAction SystemMagicActionMgr = new SystemMagicAction();

		
		public static SystemMagicAction SystemMagicActionMgr2 = new SystemMagicAction();

		
		public static SystemMagicAction SystemMagicScanTypeMgr = new SystemMagicAction();

		
		public static SystemMagicAction SystemPassiveEffectMgr = new SystemMagicAction();

		
		public static NPCTasksManager NPCTasksMgr = new NPCTasksManager();

		
		public static GoodsPackManager GoodsPackMgr = new GoodsPackManager();

		
		public static GoodsExchangeManager GoodsExchangeMgr = new GoodsExchangeManager();

		
		public static TeamManager TeamMgr = new TeamManager();

		
		public static BattleManager BattleMgr = new BattleManager();

		
		public static ArenaBattleManager ArenaBattleMgr = new ArenaBattleManager();

		
		public static DJRoomManager DJRoomMgr = new DJRoomManager();

		
		public static CopyMapManager CopyMapMgr = new CopyMapManager();

		
		public static GMCommands systemGMCommands = new GMCommands();

		
		public static BulletinMsgManager BulletinMsgMgr = new BulletinMsgManager();

		
		public static GameConfig GameConfigMgr = new GameConfig();

		
		public static PlatConfig PlatConfigMgr = new PlatConfig();

		
		public static SystemParamsList systemParamsList = new SystemParamsList();

		
		public static JunQiManager JunQiMgr = new JunQiManager();

		
		public static ShengXiaoGuessManager ShengXiaoGuessMgr = new ShengXiaoGuessManager();

		
		public static MapGridMagicHelper GridMagicHelperMgr = new MapGridMagicHelper();

		
		public static MapGridMagicHelper GridMagicHelperMgrEx = new MapGridMagicHelper();

		
		public static AngelTempleManager AngelTempleMgr = new AngelTempleManager();

		
		public static BossHomeManager BosshomeMgr = new BossHomeManager();

		
		public static GoldTempleManager GoldTempleMgr = new GoldTempleManager();

		
		public static BloodCastleCopySceneManager BloodCastleCopySceneMgr = new BloodCastleCopySceneManager();

		
		public static DaimonSquareCopySceneManager DaimonSquareCopySceneMgr = new DaimonSquareCopySceneManager();

		
		public static StarConstellationManager StarConstellationMgr = new StarConstellationManager();

		
		public static ChangeLifeManager ChangeLifeMgr = new ChangeLifeManager();

		
		public static GuildCopyMapManager GuildCopyMapMgr = new GuildCopyMapManager();

		
		public static GuildCopyMapDBManager GuildCopyMapDBMgr = new GuildCopyMapDBManager();

		
		public static QingGongYanManager QingGongYanMgr = new QingGongYanManager();

		
		public static VersionSystemOpenManager VersionSystemOpenMgr = new VersionSystemOpenManager();

		
		public static MagicSwordManager MagicSwordMgr = new MagicSwordManager();

		
		public static SummonerManager SummonerMgr = new SummonerManager();

		
		public static MerlinMagicBookManager MerlinMagicBookMgr = new MerlinMagicBookManager();

		
		public static FluorescentGemManager FluorescentGemMgr = new FluorescentGemManager();

		
		public static MerlinInjureManager MerlinInjureMgr = new MerlinInjureManager();

		
		public static LoginWaitLogic loginWaitLogic = new LoginWaitLogic();

		
		public static ElementsAttackManager ElementsAttackMgr = new ElementsAttackManager();

		
		public static DamageMonitor damageMonitor = new DamageMonitor();

		
		public static ServerMonitorManager ServerMonitor = new ServerMonitorManager();

		
		public static long LastFlushMonsterMs;

		
		public static SystemXmlItems SystemTasksMgr = new SystemXmlItems();

		
		public static SystemXmlItems SystemNPCsMgr = new SystemXmlItems();

		
		public static SystemXmlItems SystemOperasMgr = new SystemXmlItems();

		
		public static SystemXmlItems SystemMagicsMgr = new SystemXmlItems();

		
		public static SystemXmlItems SystemPassiveMgr = new SystemXmlItems();

		
		public static SystemXmlItems SystemGoods = new SystemXmlItems();

		
		public static SystemXmlItems SystemMonsterGoodsList = new SystemXmlItems();

		
		public static SystemXmlItems SystemLimitTimeMonsterGoodsList = new SystemXmlItems();

		
		public static SystemXmlItems SystemGoodsQuality = new SystemXmlItems();

		
		public static SystemXmlItems SystemGoodsLevel = new SystemXmlItems();

		
		public static SystemXmlItems SystemGoodsBornIndex = new SystemXmlItems();

		
		public static SystemXmlItems SystemGoodsZhuiJia = new SystemXmlItems();

		
		public static SystemXmlItems SystemGoodsExcellenceProperty = new SystemXmlItems();

		
		public static SystemXmlItems SystemBattle = new SystemXmlItems();

		
		public static SystemXmlItems SystemBattlePaiMingAwards = new SystemXmlItems();

		
		public static SystemXmlItems SystemArenaBattle = new SystemXmlItems();

		
		public static SystemXmlItems systemNPCScripts = new SystemXmlItems();

		
		public static SystemXmlItems systemPets = new SystemXmlItems();

		
		public static Dictionary<int, SystemXmlItems> SystemHorseDataDict = new Dictionary<int, SystemXmlItems>();

		
		public static SystemXmlItems systemGoodsMergeTypes = new SystemXmlItems();

		
		public static SystemXmlItems systemGoodsMergeItems = new SystemXmlItems();

		
		public static SystemXmlItems systemBiGuanMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemMallMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemJingMaiExpMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemGoodsBaoGuoMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemWaBaoMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemWeekLoginGiftMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemMOnlineTimeGiftMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemNewRoleGiftMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemCombatAwardMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemUpLevelGiftMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemFuBenMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemYaBiaoMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemSpecialTimeMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemHeroConfigMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemBangHuiFlagUpLevelMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemJunQiMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemQiZuoMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemLingQiMapQiZhiMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemQiZhenGeGoodsMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemHuangChengFuHuoMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemBattleExpMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemBangZhanAwardsMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemBattleRebirthMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemBattleAwardMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemEquipBornMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemBornNameMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemVipDailyAwardsMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemActivityTipMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemLuckyAwardMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemLuckyAward2Mgr = new SystemXmlItems();

		
		public static SystemXmlItems systemLuckyMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemChengJiu = new SystemXmlItems();

		
		public static SystemXmlItems systemChengJiuBuffer = new SystemXmlItems();

		
		public static SystemXmlItems systemWeaponTongLing = new SystemXmlItems();

		
		public static SystemXmlItems systemImpetrateByLevelMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemXingYunChouJiangMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemYueDuZhuanPanChouJiangMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemEveryDayOnLineAwardMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemSeriesLoginAwardMgr = new SystemXmlItems();

		
		public static SystemXmlItems systemMonsterMgr = new SystemXmlItems();

		
		public static SystemXmlItems SystemJingMaiLevel = new SystemXmlItems();

		
		public static SystemXmlItems SystemWuXueLevel = new SystemXmlItems();

		
		public static SystemXmlItems SystemTaskPlots = new SystemXmlItems();

		
		public static SystemXmlItems SystemQiangGou = new SystemXmlItems();

		
		public static SystemXmlItems SystemHeFuQiangGou = new SystemXmlItems();

		
		public static SystemXmlItems SystemJieRiQiangGou = new SystemXmlItems();

		
		public static SystemXmlItems SystemZuanHuangLevel = new SystemXmlItems();

		
		public static SystemXmlItems SystemSystemOpen = new SystemXmlItems();

		
		public static SystemXmlItems SystemDropMoney = new SystemXmlItems();

		
		public static SystemXmlItems SystemDengLuDali = new SystemXmlItems();

		
		public static SystemXmlItems SystemBuChang = new SystemXmlItems();

		
		public static SystemXmlItems SystemZhanHunLevel = new SystemXmlItems();

		
		public static SystemXmlItems SystemRongYuLevel = new SystemXmlItems();

		
		public static SystemXmlItems SystemExchangeMoJingAndQiFu = new SystemXmlItems();

		
		public static SystemXmlItems SystemExchangeType = new SystemXmlItems();

		
		public static SystemXmlItems systemDailyActiveInfo = new SystemXmlItems();

		
		public static SystemXmlItems systemDailyActiveAward = new SystemXmlItems();

		
		public static SystemXmlItems systemAngelTempleData = new SystemXmlItems();

		
		public static SystemXmlItems AngelTempleAward = new SystemXmlItems();

		
		public static SystemXmlItems AngelTempleLuckyAward = new SystemXmlItems();

		
		public static SystemXmlItems TaskZhangJie = new SystemXmlItems();

		
		public static List<RangeKey> TaskZhangJieDict = new List<RangeKey>();

		
		public static SystemXmlItems JiaoYiTab = new SystemXmlItems();

		
		public static SystemXmlItems JiaoYiType = new SystemXmlItems();

		
		public static SystemXmlItems SystemZhanMengBuild = new SystemXmlItems();

		
		public static SystemXmlItems SystemWingsUp = new SystemXmlItems();

		
		public static SystemXmlItems SystemBossAI = new SystemXmlItems();

		
		public static SystemXmlItems SystemExtensionProps = new SystemXmlItems();

		
		public static SystemXmlItems systemCaiJiMonsterMgr = new SystemXmlItems();

		
		public static SystemXmlItems SystemDamonUpgrade = new SystemXmlItems();

		
		public static ServerEvents SystemServerEvents = new ServerEvents
		{
			EventRootPath = "Events",
			EventPreFileName = "Event"
		};

		
		public static ServerEvents SystemRoleTaskEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Task"
		};

		
		public static ServerEvents SystemRoleBuyWithTongQianEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "TongQianBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithYinLiangEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "YinLiangBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithJunGongEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "JunGongBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithYinPiaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "YinPiaoBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithYuanBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "YuanBaoBuy"
		};

		
		public static ServerEvents SystemRoleQiZhenGeBuyWithYuanBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "QiZhenGeBuy"
		};

		
		public static ServerEvents SystemRoleQiangGouBuyWithYuanBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "QiangGouBuy"
		};

		
		public static ServerEvents SystemRoleSaleEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Sale"
		};

		
		public static ServerEvents SystemRoleExchangeEvents1 = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Exchange1"
		};

		
		public static ServerEvents SystemRoleExchangeEvents2 = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Exchange2"
		};

		
		public static ServerEvents SystemRoleExchangeEvents3 = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Exchange3"
		};

		
		public static ServerEvents SystemRoleGoodsEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Goods"
		};

		
		public static ServerEvents SystemRoleStoreYinLiangEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "StoreYinLiang"
		};

		
		public static ServerEvents SystemRoleStoreMoneyEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "StoreMoney"
		};

		
		public static ServerEvents SystemRoleHorseEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Horse"
		};

		
		public static ServerEvents SystemRoleBangGongEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "BangGong"
		};

		
		public static ServerEvents SystemRoleJingMaiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "JingMai"
		};

		
		public static ServerEvents SystemRoleRefreshQiZhenGeEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "RefreshQiZhenGe"
		};

		
		public static ServerEvents SystemRoleWaBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "WaBao"
		};

		
		public static ServerEvents SystemRoleMapEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Map"
		};

		
		public static ServerEvents SystemRoleFuBenAwardEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "FuBenAward"
		};

		
		public static ServerEvents SystemRoleWuXingAwardEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "WuXingAward"
		};

		
		public static ServerEvents SystemRolePaoHuanOkEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "PaoHuanOk"
		};

		
		public static ServerEvents SystemRoleYaBiaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "YaBiao"
		};

		
		public static ServerEvents SystemRoleLianZhanEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "LianZhan"
		};

		
		public static ServerEvents SystemRoleHuoDongMonsterEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "HuoDongMonster"
		};

		
		public static ServerEvents SystemRoleDigTreasureWithYaoShiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "DigTreasureWithYaoShi"
		};

		
		public static ServerEvents SystemRoleAutoSubYuanBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "AutoSubYuanBao"
		};

		
		public static ServerEvents SystemRoleAutoSubGoldEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "AutoSubGold"
		};

		
		public static ServerEvents SystemRoleAutoSubEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "AutoSub"
		};

		
		public static ServerEvents SystemRoleFetchMailMoneyEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "MailMoneyFetch"
		};

		
		public static ServerEvents SystemRoleBuyWithTianDiJingYuanEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "TianDiJingYuanBuy"
		};

		
		public static ServerEvents SystemRoleFetchVipAwardEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "VipAwardGet"
		};

		
		public static ServerEvents SystemRoleBuyWithGoldEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "GoldBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithJingYuanZhiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "JingYuanZhiBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithLieShaZhiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "LieShaZhiBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithZhuangBeiJiFenEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "ZhuangBeiJiFenBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithJunGongZhiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "JunGongZhiBuy"
		};

		
		public static ServerEvents SystemRoleBuyWithZhanHunEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "ZhanHunBuy"
		};

		
		public static ServerEvents SystemRoleGameEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "RoleGame"
		};

		
		public static ServerEvents SystemGlobalGameEvents = new ServerEvents
		{
			EventRootPath = "Events",
			EventPreFileName = "GameLog"
		};

		
		public static ServerEvents SystemClientLogsEvents = new ServerEvents
		{
			EventRootPath = "Events",
			EventPreFileName = "ClientLogs"
		};

		
		public static ServerEvents SystemRoleConsumeEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Consume"
		};

		
		public static Dictionary<int, SafeClientData> RoleDataExDictForTestMem = new Dictionary<int, SafeClientData>();
	}
}
