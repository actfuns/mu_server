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
	// Token: 0x020006C9 RID: 1737
	public class GameManager
	{
		// Token: 0x060023B2 RID: 9138 RVA: 0x001E7084 File Offset: 0x001E5284
		static GameManager()
		{
			GameManager.ResetHideFlagsMaps(true, 1);
		}

		// Token: 0x060023B3 RID: 9139 RVA: 0x001E7FA0 File Offset: 0x001E61A0
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

		// Token: 0x060023B4 RID: 9140 RVA: 0x001E8098 File Offset: 0x001E6298
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

		// Token: 0x060023B5 RID: 9141 RVA: 0x001E811C File Offset: 0x001E631C
		public static void LoadGameConfigFlags()
		{
			GameManager.FlagRecalcRolePropsTicks = (long)GameManager.GameConfigMgr.GetGameConfigItemInt("recalcrolepropsticks", 700);
			GameManager.FlagCheckCmdPosition = GameManager.GameConfigMgr.GetGameConfigItemInt("check_cmd_position", 1);
			GameManager.ConstCheckServerTimeDiffMinutes = GameManager.GameConfigMgr.GetGameConfigItemInt("checkservertime", 5);
		}

		// Token: 0x040036F1 RID: 14065
		public const int ServerLineIdAllIncludeKuaFu = 0;

		// Token: 0x040036F2 RID: 14066
		public const int ServerLineIdAllLineExcludeSelf = -1000;

		// Token: 0x040036F3 RID: 14067
		public const int ClientViewSlot_Direct = 0;

		// Token: 0x040036F4 RID: 14068
		public const int ClientViewSlot_Pos = 1;

		// Token: 0x040036F5 RID: 14069
		public const int MaxSlotCount = 5;

		// Token: 0x040036F6 RID: 14070
		public const int Update9GridUsingNewMode = 0;

		// Token: 0x040036F7 RID: 14071
		public const int Update9GridDelayFlag = 1;

		// Token: 0x040036F8 RID: 14072
		public const int Update9GridBoostFlag = 2;

		// Token: 0x040036F9 RID: 14073
		public const bool FlagManyAttack = true;

		// Token: 0x040036FA RID: 14074
		public const bool FlagManyAttackOp = true;

		// Token: 0x040036FB RID: 14075
		public const bool FlagOptimizeLock = true;

		// Token: 0x040036FC RID: 14076
		public const bool FlagOptimizeLock2 = true;

		// Token: 0x040036FD RID: 14077
		public const bool FlagOptimizeLock3 = true;

		// Token: 0x040036FE RID: 14078
		public const bool FlagOptimizePathString = false;

		// Token: 0x040036FF RID: 14079
		public const bool FlagOptimizeLockTrace = false;

		// Token: 0x04003700 RID: 14080
		public const bool FlagOptimizeAlgorithm = true;

		// Token: 0x04003701 RID: 14081
		public const bool FlagOptimizeThreadPool = true;

		// Token: 0x04003702 RID: 14082
		public const bool FlagOptimizeThreadPool2 = false;

		// Token: 0x04003703 RID: 14083
		public const bool FlagOptimizeThreadPool3 = true;

		// Token: 0x04003704 RID: 14084
		public const bool FlagOptimizeThreadPool4 = true;

		// Token: 0x04003705 RID: 14085
		public const bool FlagOptimizeThreadPool5 = true;

		// Token: 0x04003706 RID: 14086
		public const bool FlagSkipSendDataCall = false;

		// Token: 0x04003707 RID: 14087
		public const bool FlagSkipAddBuffCall = false;

		// Token: 0x04003708 RID: 14088
		public const bool FlagSkipTrySendCall = false;

		// Token: 0x04003709 RID: 14089
		public const bool FlagSkipSocketSend = false;

		// Token: 0x0400370A RID: 14090
		public const bool FlagTraceMemoryPool = false;

		// Token: 0x0400370B RID: 14091
		public const bool FlagTraceTCPEvent = false;

		// Token: 0x0400370C RID: 14092
		public const bool FlagTracePropsValues = false;

		// Token: 0x0400370D RID: 14093
		public const bool FlagDisableNameServer = true;

		// Token: 0x0400370E RID: 14094
		public const int CostSkipSendCount = 900;

		// Token: 0x0400370F RID: 14095
		public const bool Flag_Cache_VisiableObject = false;

		// Token: 0x04003710 RID: 14096
		public const bool FlagOptimizeAlgorithm_Props = true;

		// Token: 0x04003711 RID: 14097
		public const bool FlagEnableMultiLineServer = false;

		// Token: 0x04003712 RID: 14098
		public const int LocalServerId = 0;

		// Token: 0x04003713 RID: 14099
		public const int LocalServerIdForNotImplement = 0;

		// Token: 0x04003714 RID: 14100
		public const int OPT_ChengZhanType = 1;

		// Token: 0x04003715 RID: 14101
		public const bool OPT_OldJuQiConfig = true;

		// Token: 0x04003716 RID: 14102
		public static int MapGridWidth = 100;

		// Token: 0x04003717 RID: 14103
		public static int MapGridHeight = 100;

		// Token: 0x04003718 RID: 14104
		public static Program AppMainWnd = null;

		// Token: 0x04003719 RID: 14105
		public static int DefaultMapCode = 1;

		// Token: 0x0400371A RID: 14106
		public static int MainMapCode = 2;

		// Token: 0x0400371B RID: 14107
		public static int NewDefaultMapCode = 1;

		// Token: 0x0400371C RID: 14108
		public static int ServerLineID = 1;

		// Token: 0x0400371D RID: 14109
		public static int ServerId = 1;

		// Token: 0x0400371E RID: 14110
		public static int KuaFuServerId = 1;

		// Token: 0x0400371F RID: 14111
		public static int PTID = 0;

		// Token: 0x04003720 RID: 14112
		public static PlatformTypes PlatformType = PlatformTypes.Tmsk;

		// Token: 0x04003721 RID: 14113
		public static List<int> AutoGiveGoodsIDPortableList = null;

		// Token: 0x04003722 RID: 14114
		public static List<int> AutoGiveGoodsIDList = null;

		// Token: 0x04003723 RID: 14115
		public static int MaxSlotOnUpdate9GridsTicks = 1000;

		// Token: 0x04003724 RID: 14116
		public static int MaxSleepOnDoMapGridMoveTicks = 5;

		// Token: 0x04003725 RID: 14117
		public static int MaxCachingMonsterToClientBytesDataTicks = 30000;

		// Token: 0x04003726 RID: 14118
		public static int MaxCachingClientToClientBytesDataTicks = 30000;

		// Token: 0x04003727 RID: 14119
		public static int Update9GridUsingPosition = 1;

		// Token: 0x04003728 RID: 14120
		public static int MaxSlotOnPositionUpdate9GridsTicks = 2000;

		// Token: 0x04003729 RID: 14121
		public static int RoleDataMiniMode = 1;

		// Token: 0x0400372A RID: 14122
		public static int FlagSleepTime = 0;

		// Token: 0x0400372B RID: 14123
		public static int FlagLiXianGuaJi = 0;

		// Token: 0x0400372C RID: 14124
		public static int ConstCheckServerTimeDiffMinutes = 3;

		// Token: 0x0400372D RID: 14125
		public static int StatisticsMode = 1;

		// Token: 0x0400372E RID: 14126
		public static bool Flag_OptimizationBagReset = true;

		// Token: 0x0400372F RID: 14127
		public static Dictionary<int, int> MemoryPoolConfigDict = new Dictionary<int, int>();

		// Token: 0x04003730 RID: 14128
		public static bool TestGamePerformanceMode = false;

		// Token: 0x04003731 RID: 14129
		public static List<Point> TestBirthPointList1 = new List<Point>();

		// Token: 0x04003732 RID: 14130
		public static List<Point> TestBirthPointList2 = new List<Point>();

		// Token: 0x04003733 RID: 14131
		public static int TestGamePerformanceMapCode = 1;

		// Token: 0x04003734 RID: 14132
		public static int TestGamePerformanceMapMode = 0;

		// Token: 0x04003735 RID: 14133
		public static bool TestGamePerformanceAllPK = false;

		// Token: 0x04003736 RID: 14134
		public static bool TestGamePerformanceLockLifeV = true;

		// Token: 0x04003737 RID: 14135
		public static bool TestGamePerformanceForAllUser = false;

		// Token: 0x04003738 RID: 14136
		public static bool FlagUseWin32Decrypt = true;

		// Token: 0x04003739 RID: 14137
		public static bool TestGameShowFakeRoleForUser = false;

		// Token: 0x0400373A RID: 14138
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

		// Token: 0x0400373B RID: 14139
		public static long GM_NoCheckTokenTimeRemainMS = 0L;

		// Token: 0x0400373C RID: 14140
		public static bool FlagAlowUnRegistedCmd = false;

		// Token: 0x0400373D RID: 14141
		public static bool CheckMismatchMapCode = true;

		// Token: 0x0400373E RID: 14142
		public static bool CheckCheatPosition = true;

		// Token: 0x0400373F RID: 14143
		public static double CheckPositionInterval = 5000.0;

		// Token: 0x04003740 RID: 14144
		public static double CheckCheatMaxDistance = 600.0;

		// Token: 0x04003741 RID: 14145
		public static int MaxAttackDistance = 1500;

		// Token: 0x04003742 RID: 14146
		public static bool ServerStarting = true;

		// Token: 0x04003743 RID: 14147
		public static bool FlagDisableMovingOnAttack = true;

		// Token: 0x04003744 RID: 14148
		public static bool FlaDisablegFilterMonsterDeadEvent = false;

		// Token: 0x04003745 RID: 14149
		public static bool FlagKuaFuServerExplicit = true;

		// Token: 0x04003746 RID: 14150
		public static bool IsKuaFuServer = false;

		// Token: 0x04003747 RID: 14151
		public static ConcurrentDictionary<int, int> HideFlagsMapDict = new ConcurrentDictionary<int, int>();

		// Token: 0x04003748 RID: 14152
		public static bool FlagEnableHideFlags = false;

		// Token: 0x04003749 RID: 14153
		public static int FlagHideFlagsType = 0;

		// Token: 0x0400374A RID: 14154
		public static int OnceDestroyCopyMapNum = 100;

		// Token: 0x0400374B RID: 14155
		public static bool FlagEnableMoneyEventLog = true;

		// Token: 0x0400374C RID: 14156
		public static bool FlagEnableGoodsEventLog = true;

		// Token: 0x0400374D RID: 14157
		public static bool FlagEnableGameEventLog = true;

		// Token: 0x0400374E RID: 14158
		public static bool FlagEnableOperatorEventLog = true;

		// Token: 0x0400374F RID: 14159
		public static bool FlagEnableRoleSkillLog = true;

		// Token: 0x04003750 RID: 14160
		public static bool FlagEnablePetSkillLog = true;

		// Token: 0x04003751 RID: 14161
		public static bool FlagEnableUnionPalaceLog = true;

		// Token: 0x04003752 RID: 14162
		public static long FlagRecalcRolePropsTicks = 700L;

		// Token: 0x04003753 RID: 14163
		public static int FlagCheckCmdPosition = 1;

		// Token: 0x04003754 RID: 14164
		public static LuaManager LuaMgr = new LuaManager();

		// Token: 0x04003755 RID: 14165
		public static UserSession OnlineUserSession = new UserSession();

		// Token: 0x04003756 RID: 14166
		public static KuaFuManager KuaFuMgr = KuaFuManager.getInstance();

		// Token: 0x04003757 RID: 14167
		public static MonsterIDManager MonsterIDMgr = new MonsterIDManager();

		// Token: 0x04003758 RID: 14168
		public static PetIDManager PetIDMgr = new PetIDManager();

		// Token: 0x04003759 RID: 14169
		public static BiaoCheIDManager BiaoCheIDMgr = new BiaoCheIDManager();

		// Token: 0x0400375A RID: 14170
		public static JunQiIDManager JunQiIDMgr = new JunQiIDManager();

		// Token: 0x0400375B RID: 14171
		public static FakeRoleIDManager FakeRoleIDMgr = new FakeRoleIDManager();

		// Token: 0x0400375C RID: 14172
		public static MapManager MapMgr = new MapManager();

		// Token: 0x0400375D RID: 14173
		public static MapGridManager MapGridMgr = new MapGridManager();

		// Token: 0x0400375E RID: 14174
		public static ClientManager ClientMgr = new ClientManager();

		// Token: 0x0400375F RID: 14175
		public static MonsterZoneManager MonsterZoneMgr = new MonsterZoneManager();

		// Token: 0x04003760 RID: 14176
		public static MonsterManager MonsterMgr = new MonsterManager();

		// Token: 0x04003761 RID: 14177
		public static DBCmdManager DBCmdMgr = new DBCmdManager();

		// Token: 0x04003762 RID: 14178
		public static LogDBCmdManager logDBCmdMgr = new LogDBCmdManager();

		// Token: 0x04003763 RID: 14179
		public static NPCSaleList NPCSaleListMgr = new NPCSaleList();

		// Token: 0x04003764 RID: 14180
		public static SystemGoodsManager SystemGoodsNamgMgr = new SystemGoodsManager();

		// Token: 0x04003765 RID: 14181
		public static TaskAwards TaskAwardsMgr = new TaskAwards();

		// Token: 0x04003766 RID: 14182
		public static EquipProps EquipPropsMgr = new EquipProps();

		// Token: 0x04003767 RID: 14183
		public static SystemMagicManager SystemMagicQuickMgr = new SystemMagicManager();

		// Token: 0x04003768 RID: 14184
		public static SystemMagicAction SystemMagicActionMgr = new SystemMagicAction();

		// Token: 0x04003769 RID: 14185
		public static SystemMagicAction SystemMagicActionMgr2 = new SystemMagicAction();

		// Token: 0x0400376A RID: 14186
		public static SystemMagicAction SystemMagicScanTypeMgr = new SystemMagicAction();

		// Token: 0x0400376B RID: 14187
		public static SystemMagicAction SystemPassiveEffectMgr = new SystemMagicAction();

		// Token: 0x0400376C RID: 14188
		public static NPCTasksManager NPCTasksMgr = new NPCTasksManager();

		// Token: 0x0400376D RID: 14189
		public static GoodsPackManager GoodsPackMgr = new GoodsPackManager();

		// Token: 0x0400376E RID: 14190
		public static GoodsExchangeManager GoodsExchangeMgr = new GoodsExchangeManager();

		// Token: 0x0400376F RID: 14191
		public static TeamManager TeamMgr = new TeamManager();

		// Token: 0x04003770 RID: 14192
		public static BattleManager BattleMgr = new BattleManager();

		// Token: 0x04003771 RID: 14193
		public static ArenaBattleManager ArenaBattleMgr = new ArenaBattleManager();

		// Token: 0x04003772 RID: 14194
		public static DJRoomManager DJRoomMgr = new DJRoomManager();

		// Token: 0x04003773 RID: 14195
		public static CopyMapManager CopyMapMgr = new CopyMapManager();

		// Token: 0x04003774 RID: 14196
		public static GMCommands systemGMCommands = new GMCommands();

		// Token: 0x04003775 RID: 14197
		public static BulletinMsgManager BulletinMsgMgr = new BulletinMsgManager();

		// Token: 0x04003776 RID: 14198
		public static GameConfig GameConfigMgr = new GameConfig();

		// Token: 0x04003777 RID: 14199
		public static PlatConfig PlatConfigMgr = new PlatConfig();

		// Token: 0x04003778 RID: 14200
		public static SystemParamsList systemParamsList = new SystemParamsList();

		// Token: 0x04003779 RID: 14201
		public static JunQiManager JunQiMgr = new JunQiManager();

		// Token: 0x0400377A RID: 14202
		public static ShengXiaoGuessManager ShengXiaoGuessMgr = new ShengXiaoGuessManager();

		// Token: 0x0400377B RID: 14203
		public static MapGridMagicHelper GridMagicHelperMgr = new MapGridMagicHelper();

		// Token: 0x0400377C RID: 14204
		public static MapGridMagicHelper GridMagicHelperMgrEx = new MapGridMagicHelper();

		// Token: 0x0400377D RID: 14205
		public static AngelTempleManager AngelTempleMgr = new AngelTempleManager();

		// Token: 0x0400377E RID: 14206
		public static BossHomeManager BosshomeMgr = new BossHomeManager();

		// Token: 0x0400377F RID: 14207
		public static GoldTempleManager GoldTempleMgr = new GoldTempleManager();

		// Token: 0x04003780 RID: 14208
		public static BloodCastleCopySceneManager BloodCastleCopySceneMgr = new BloodCastleCopySceneManager();

		// Token: 0x04003781 RID: 14209
		public static DaimonSquareCopySceneManager DaimonSquareCopySceneMgr = new DaimonSquareCopySceneManager();

		// Token: 0x04003782 RID: 14210
		public static StarConstellationManager StarConstellationMgr = new StarConstellationManager();

		// Token: 0x04003783 RID: 14211
		public static ChangeLifeManager ChangeLifeMgr = new ChangeLifeManager();

		// Token: 0x04003784 RID: 14212
		public static GuildCopyMapManager GuildCopyMapMgr = new GuildCopyMapManager();

		// Token: 0x04003785 RID: 14213
		public static GuildCopyMapDBManager GuildCopyMapDBMgr = new GuildCopyMapDBManager();

		// Token: 0x04003786 RID: 14214
		public static QingGongYanManager QingGongYanMgr = new QingGongYanManager();

		// Token: 0x04003787 RID: 14215
		public static VersionSystemOpenManager VersionSystemOpenMgr = new VersionSystemOpenManager();

		// Token: 0x04003788 RID: 14216
		public static MagicSwordManager MagicSwordMgr = new MagicSwordManager();

		// Token: 0x04003789 RID: 14217
		public static SummonerManager SummonerMgr = new SummonerManager();

		// Token: 0x0400378A RID: 14218
		public static MerlinMagicBookManager MerlinMagicBookMgr = new MerlinMagicBookManager();

		// Token: 0x0400378B RID: 14219
		public static FluorescentGemManager FluorescentGemMgr = new FluorescentGemManager();

		// Token: 0x0400378C RID: 14220
		public static MerlinInjureManager MerlinInjureMgr = new MerlinInjureManager();

		// Token: 0x0400378D RID: 14221
		public static LoginWaitLogic loginWaitLogic = new LoginWaitLogic();

		// Token: 0x0400378E RID: 14222
		public static ElementsAttackManager ElementsAttackMgr = new ElementsAttackManager();

		// Token: 0x0400378F RID: 14223
		public static DamageMonitor damageMonitor = new DamageMonitor();

		// Token: 0x04003790 RID: 14224
		public static ServerMonitorManager ServerMonitor = new ServerMonitorManager();

		// Token: 0x04003791 RID: 14225
		public static long LastFlushMonsterMs;

		// Token: 0x04003792 RID: 14226
		public static SystemXmlItems SystemTasksMgr = new SystemXmlItems();

		// Token: 0x04003793 RID: 14227
		public static SystemXmlItems SystemNPCsMgr = new SystemXmlItems();

		// Token: 0x04003794 RID: 14228
		public static SystemXmlItems SystemOperasMgr = new SystemXmlItems();

		// Token: 0x04003795 RID: 14229
		public static SystemXmlItems SystemMagicsMgr = new SystemXmlItems();

		// Token: 0x04003796 RID: 14230
		public static SystemXmlItems SystemPassiveMgr = new SystemXmlItems();

		// Token: 0x04003797 RID: 14231
		public static SystemXmlItems SystemGoods = new SystemXmlItems();

		// Token: 0x04003798 RID: 14232
		public static SystemXmlItems SystemMonsterGoodsList = new SystemXmlItems();

		// Token: 0x04003799 RID: 14233
		public static SystemXmlItems SystemLimitTimeMonsterGoodsList = new SystemXmlItems();

		// Token: 0x0400379A RID: 14234
		public static SystemXmlItems SystemGoodsQuality = new SystemXmlItems();

		// Token: 0x0400379B RID: 14235
		public static SystemXmlItems SystemGoodsLevel = new SystemXmlItems();

		// Token: 0x0400379C RID: 14236
		public static SystemXmlItems SystemGoodsBornIndex = new SystemXmlItems();

		// Token: 0x0400379D RID: 14237
		public static SystemXmlItems SystemGoodsZhuiJia = new SystemXmlItems();

		// Token: 0x0400379E RID: 14238
		public static SystemXmlItems SystemGoodsExcellenceProperty = new SystemXmlItems();

		// Token: 0x0400379F RID: 14239
		public static SystemXmlItems SystemBattle = new SystemXmlItems();

		// Token: 0x040037A0 RID: 14240
		public static SystemXmlItems SystemBattlePaiMingAwards = new SystemXmlItems();

		// Token: 0x040037A1 RID: 14241
		public static SystemXmlItems SystemArenaBattle = new SystemXmlItems();

		// Token: 0x040037A2 RID: 14242
		public static SystemXmlItems systemNPCScripts = new SystemXmlItems();

		// Token: 0x040037A3 RID: 14243
		public static SystemXmlItems systemPets = new SystemXmlItems();

		// Token: 0x040037A4 RID: 14244
		public static Dictionary<int, SystemXmlItems> SystemHorseDataDict = new Dictionary<int, SystemXmlItems>();

		// Token: 0x040037A5 RID: 14245
		public static SystemXmlItems systemGoodsMergeTypes = new SystemXmlItems();

		// Token: 0x040037A6 RID: 14246
		public static SystemXmlItems systemGoodsMergeItems = new SystemXmlItems();

		// Token: 0x040037A7 RID: 14247
		public static SystemXmlItems systemBiGuanMgr = new SystemXmlItems();

		// Token: 0x040037A8 RID: 14248
		public static SystemXmlItems systemMallMgr = new SystemXmlItems();

		// Token: 0x040037A9 RID: 14249
		public static SystemXmlItems systemJingMaiExpMgr = new SystemXmlItems();

		// Token: 0x040037AA RID: 14250
		public static SystemXmlItems systemGoodsBaoGuoMgr = new SystemXmlItems();

		// Token: 0x040037AB RID: 14251
		public static SystemXmlItems systemWaBaoMgr = new SystemXmlItems();

		// Token: 0x040037AC RID: 14252
		public static SystemXmlItems systemWeekLoginGiftMgr = new SystemXmlItems();

		// Token: 0x040037AD RID: 14253
		public static SystemXmlItems systemMOnlineTimeGiftMgr = new SystemXmlItems();

		// Token: 0x040037AE RID: 14254
		public static SystemXmlItems systemNewRoleGiftMgr = new SystemXmlItems();

		// Token: 0x040037AF RID: 14255
		public static SystemXmlItems systemCombatAwardMgr = new SystemXmlItems();

		// Token: 0x040037B0 RID: 14256
		public static SystemXmlItems systemUpLevelGiftMgr = new SystemXmlItems();

		// Token: 0x040037B1 RID: 14257
		public static SystemXmlItems systemFuBenMgr = new SystemXmlItems();

		// Token: 0x040037B2 RID: 14258
		public static SystemXmlItems systemYaBiaoMgr = new SystemXmlItems();

		// Token: 0x040037B3 RID: 14259
		public static SystemXmlItems systemSpecialTimeMgr = new SystemXmlItems();

		// Token: 0x040037B4 RID: 14260
		public static SystemXmlItems systemHeroConfigMgr = new SystemXmlItems();

		// Token: 0x040037B5 RID: 14261
		public static SystemXmlItems systemBangHuiFlagUpLevelMgr = new SystemXmlItems();

		// Token: 0x040037B6 RID: 14262
		public static SystemXmlItems systemJunQiMgr = new SystemXmlItems();

		// Token: 0x040037B7 RID: 14263
		public static SystemXmlItems systemQiZuoMgr = new SystemXmlItems();

		// Token: 0x040037B8 RID: 14264
		public static SystemXmlItems systemLingQiMapQiZhiMgr = new SystemXmlItems();

		// Token: 0x040037B9 RID: 14265
		public static SystemXmlItems systemQiZhenGeGoodsMgr = new SystemXmlItems();

		// Token: 0x040037BA RID: 14266
		public static SystemXmlItems systemHuangChengFuHuoMgr = new SystemXmlItems();

		// Token: 0x040037BB RID: 14267
		public static SystemXmlItems systemBattleExpMgr = new SystemXmlItems();

		// Token: 0x040037BC RID: 14268
		public static SystemXmlItems systemBangZhanAwardsMgr = new SystemXmlItems();

		// Token: 0x040037BD RID: 14269
		public static SystemXmlItems systemBattleRebirthMgr = new SystemXmlItems();

		// Token: 0x040037BE RID: 14270
		public static SystemXmlItems systemBattleAwardMgr = new SystemXmlItems();

		// Token: 0x040037BF RID: 14271
		public static SystemXmlItems systemEquipBornMgr = new SystemXmlItems();

		// Token: 0x040037C0 RID: 14272
		public static SystemXmlItems systemBornNameMgr = new SystemXmlItems();

		// Token: 0x040037C1 RID: 14273
		public static SystemXmlItems systemVipDailyAwardsMgr = new SystemXmlItems();

		// Token: 0x040037C2 RID: 14274
		public static SystemXmlItems systemActivityTipMgr = new SystemXmlItems();

		// Token: 0x040037C3 RID: 14275
		public static SystemXmlItems systemLuckyAwardMgr = new SystemXmlItems();

		// Token: 0x040037C4 RID: 14276
		public static SystemXmlItems systemLuckyAward2Mgr = new SystemXmlItems();

		// Token: 0x040037C5 RID: 14277
		public static SystemXmlItems systemLuckyMgr = new SystemXmlItems();

		// Token: 0x040037C6 RID: 14278
		public static SystemXmlItems systemChengJiu = new SystemXmlItems();

		// Token: 0x040037C7 RID: 14279
		public static SystemXmlItems systemChengJiuBuffer = new SystemXmlItems();

		// Token: 0x040037C8 RID: 14280
		public static SystemXmlItems systemWeaponTongLing = new SystemXmlItems();

		// Token: 0x040037C9 RID: 14281
		public static SystemXmlItems systemImpetrateByLevelMgr = new SystemXmlItems();

		// Token: 0x040037CA RID: 14282
		public static SystemXmlItems systemXingYunChouJiangMgr = new SystemXmlItems();

		// Token: 0x040037CB RID: 14283
		public static SystemXmlItems systemYueDuZhuanPanChouJiangMgr = new SystemXmlItems();

		// Token: 0x040037CC RID: 14284
		public static SystemXmlItems systemEveryDayOnLineAwardMgr = new SystemXmlItems();

		// Token: 0x040037CD RID: 14285
		public static SystemXmlItems systemSeriesLoginAwardMgr = new SystemXmlItems();

		// Token: 0x040037CE RID: 14286
		public static SystemXmlItems systemMonsterMgr = new SystemXmlItems();

		// Token: 0x040037CF RID: 14287
		public static SystemXmlItems SystemJingMaiLevel = new SystemXmlItems();

		// Token: 0x040037D0 RID: 14288
		public static SystemXmlItems SystemWuXueLevel = new SystemXmlItems();

		// Token: 0x040037D1 RID: 14289
		public static SystemXmlItems SystemTaskPlots = new SystemXmlItems();

		// Token: 0x040037D2 RID: 14290
		public static SystemXmlItems SystemQiangGou = new SystemXmlItems();

		// Token: 0x040037D3 RID: 14291
		public static SystemXmlItems SystemHeFuQiangGou = new SystemXmlItems();

		// Token: 0x040037D4 RID: 14292
		public static SystemXmlItems SystemJieRiQiangGou = new SystemXmlItems();

		// Token: 0x040037D5 RID: 14293
		public static SystemXmlItems SystemZuanHuangLevel = new SystemXmlItems();

		// Token: 0x040037D6 RID: 14294
		public static SystemXmlItems SystemSystemOpen = new SystemXmlItems();

		// Token: 0x040037D7 RID: 14295
		public static SystemXmlItems SystemDropMoney = new SystemXmlItems();

		// Token: 0x040037D8 RID: 14296
		public static SystemXmlItems SystemDengLuDali = new SystemXmlItems();

		// Token: 0x040037D9 RID: 14297
		public static SystemXmlItems SystemBuChang = new SystemXmlItems();

		// Token: 0x040037DA RID: 14298
		public static SystemXmlItems SystemZhanHunLevel = new SystemXmlItems();

		// Token: 0x040037DB RID: 14299
		public static SystemXmlItems SystemRongYuLevel = new SystemXmlItems();

		// Token: 0x040037DC RID: 14300
		public static SystemXmlItems SystemExchangeMoJingAndQiFu = new SystemXmlItems();

		// Token: 0x040037DD RID: 14301
		public static SystemXmlItems SystemExchangeType = new SystemXmlItems();

		// Token: 0x040037DE RID: 14302
		public static SystemXmlItems systemDailyActiveInfo = new SystemXmlItems();

		// Token: 0x040037DF RID: 14303
		public static SystemXmlItems systemDailyActiveAward = new SystemXmlItems();

		// Token: 0x040037E0 RID: 14304
		public static SystemXmlItems systemAngelTempleData = new SystemXmlItems();

		// Token: 0x040037E1 RID: 14305
		public static SystemXmlItems AngelTempleAward = new SystemXmlItems();

		// Token: 0x040037E2 RID: 14306
		public static SystemXmlItems AngelTempleLuckyAward = new SystemXmlItems();

		// Token: 0x040037E3 RID: 14307
		public static SystemXmlItems TaskZhangJie = new SystemXmlItems();

		// Token: 0x040037E4 RID: 14308
		public static List<RangeKey> TaskZhangJieDict = new List<RangeKey>();

		// Token: 0x040037E5 RID: 14309
		public static SystemXmlItems JiaoYiTab = new SystemXmlItems();

		// Token: 0x040037E6 RID: 14310
		public static SystemXmlItems JiaoYiType = new SystemXmlItems();

		// Token: 0x040037E7 RID: 14311
		public static SystemXmlItems SystemZhanMengBuild = new SystemXmlItems();

		// Token: 0x040037E8 RID: 14312
		public static SystemXmlItems SystemWingsUp = new SystemXmlItems();

		// Token: 0x040037E9 RID: 14313
		public static SystemXmlItems SystemBossAI = new SystemXmlItems();

		// Token: 0x040037EA RID: 14314
		public static SystemXmlItems SystemExtensionProps = new SystemXmlItems();

		// Token: 0x040037EB RID: 14315
		public static SystemXmlItems systemCaiJiMonsterMgr = new SystemXmlItems();

		// Token: 0x040037EC RID: 14316
		public static SystemXmlItems SystemDamonUpgrade = new SystemXmlItems();

		// Token: 0x040037ED RID: 14317
		public static ServerEvents SystemServerEvents = new ServerEvents
		{
			EventRootPath = "Events",
			EventPreFileName = "Event"
		};

		// Token: 0x040037EE RID: 14318
		public static ServerEvents SystemRoleTaskEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Task"
		};

		// Token: 0x040037EF RID: 14319
		public static ServerEvents SystemRoleBuyWithTongQianEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "TongQianBuy"
		};

		// Token: 0x040037F0 RID: 14320
		public static ServerEvents SystemRoleBuyWithYinLiangEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "YinLiangBuy"
		};

		// Token: 0x040037F1 RID: 14321
		public static ServerEvents SystemRoleBuyWithJunGongEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "JunGongBuy"
		};

		// Token: 0x040037F2 RID: 14322
		public static ServerEvents SystemRoleBuyWithYinPiaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "YinPiaoBuy"
		};

		// Token: 0x040037F3 RID: 14323
		public static ServerEvents SystemRoleBuyWithYuanBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "YuanBaoBuy"
		};

		// Token: 0x040037F4 RID: 14324
		public static ServerEvents SystemRoleQiZhenGeBuyWithYuanBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "QiZhenGeBuy"
		};

		// Token: 0x040037F5 RID: 14325
		public static ServerEvents SystemRoleQiangGouBuyWithYuanBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "QiangGouBuy"
		};

		// Token: 0x040037F6 RID: 14326
		public static ServerEvents SystemRoleSaleEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Sale"
		};

		// Token: 0x040037F7 RID: 14327
		public static ServerEvents SystemRoleExchangeEvents1 = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Exchange1"
		};

		// Token: 0x040037F8 RID: 14328
		public static ServerEvents SystemRoleExchangeEvents2 = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Exchange2"
		};

		// Token: 0x040037F9 RID: 14329
		public static ServerEvents SystemRoleExchangeEvents3 = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Exchange3"
		};

		// Token: 0x040037FA RID: 14330
		public static ServerEvents SystemRoleGoodsEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Goods"
		};

		// Token: 0x040037FB RID: 14331
		public static ServerEvents SystemRoleStoreYinLiangEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "StoreYinLiang"
		};

		// Token: 0x040037FC RID: 14332
		public static ServerEvents SystemRoleStoreMoneyEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "StoreMoney"
		};

		// Token: 0x040037FD RID: 14333
		public static ServerEvents SystemRoleHorseEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Horse"
		};

		// Token: 0x040037FE RID: 14334
		public static ServerEvents SystemRoleBangGongEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "BangGong"
		};

		// Token: 0x040037FF RID: 14335
		public static ServerEvents SystemRoleJingMaiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "JingMai"
		};

		// Token: 0x04003800 RID: 14336
		public static ServerEvents SystemRoleRefreshQiZhenGeEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "RefreshQiZhenGe"
		};

		// Token: 0x04003801 RID: 14337
		public static ServerEvents SystemRoleWaBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "WaBao"
		};

		// Token: 0x04003802 RID: 14338
		public static ServerEvents SystemRoleMapEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Map"
		};

		// Token: 0x04003803 RID: 14339
		public static ServerEvents SystemRoleFuBenAwardEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "FuBenAward"
		};

		// Token: 0x04003804 RID: 14340
		public static ServerEvents SystemRoleWuXingAwardEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "WuXingAward"
		};

		// Token: 0x04003805 RID: 14341
		public static ServerEvents SystemRolePaoHuanOkEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "PaoHuanOk"
		};

		// Token: 0x04003806 RID: 14342
		public static ServerEvents SystemRoleYaBiaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "YaBiao"
		};

		// Token: 0x04003807 RID: 14343
		public static ServerEvents SystemRoleLianZhanEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "LianZhan"
		};

		// Token: 0x04003808 RID: 14344
		public static ServerEvents SystemRoleHuoDongMonsterEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "HuoDongMonster"
		};

		// Token: 0x04003809 RID: 14345
		public static ServerEvents SystemRoleDigTreasureWithYaoShiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "DigTreasureWithYaoShi"
		};

		// Token: 0x0400380A RID: 14346
		public static ServerEvents SystemRoleAutoSubYuanBaoEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "AutoSubYuanBao"
		};

		// Token: 0x0400380B RID: 14347
		public static ServerEvents SystemRoleAutoSubGoldEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "AutoSubGold"
		};

		// Token: 0x0400380C RID: 14348
		public static ServerEvents SystemRoleAutoSubEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "AutoSub"
		};

		// Token: 0x0400380D RID: 14349
		public static ServerEvents SystemRoleFetchMailMoneyEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "MailMoneyFetch"
		};

		// Token: 0x0400380E RID: 14350
		public static ServerEvents SystemRoleBuyWithTianDiJingYuanEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "TianDiJingYuanBuy"
		};

		// Token: 0x0400380F RID: 14351
		public static ServerEvents SystemRoleFetchVipAwardEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "VipAwardGet"
		};

		// Token: 0x04003810 RID: 14352
		public static ServerEvents SystemRoleBuyWithGoldEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "GoldBuy"
		};

		// Token: 0x04003811 RID: 14353
		public static ServerEvents SystemRoleBuyWithJingYuanZhiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "JingYuanZhiBuy"
		};

		// Token: 0x04003812 RID: 14354
		public static ServerEvents SystemRoleBuyWithLieShaZhiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "LieShaZhiBuy"
		};

		// Token: 0x04003813 RID: 14355
		public static ServerEvents SystemRoleBuyWithZhuangBeiJiFenEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "ZhuangBeiJiFenBuy"
		};

		// Token: 0x04003814 RID: 14356
		public static ServerEvents SystemRoleBuyWithJunGongZhiEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "JunGongZhiBuy"
		};

		// Token: 0x04003815 RID: 14357
		public static ServerEvents SystemRoleBuyWithZhanHunEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "ZhanHunBuy"
		};

		// Token: 0x04003816 RID: 14358
		public static ServerEvents SystemRoleGameEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "RoleGame"
		};

		// Token: 0x04003817 RID: 14359
		public static ServerEvents SystemGlobalGameEvents = new ServerEvents
		{
			EventRootPath = "Events",
			EventPreFileName = "GameLog"
		};

		// Token: 0x04003818 RID: 14360
		public static ServerEvents SystemClientLogsEvents = new ServerEvents
		{
			EventRootPath = "Events",
			EventPreFileName = "ClientLogs"
		};

		// Token: 0x04003819 RID: 14361
		public static ServerEvents SystemRoleConsumeEvents = new ServerEvents
		{
			EventRootPath = "RoleEvents",
			EventPreFileName = "Consume"
		};

		// Token: 0x0400381A RID: 14362
		public static Dictionary<int, SafeClientData> RoleDataExDictForTestMem = new Dictionary<int, SafeClientData>();
	}
}
