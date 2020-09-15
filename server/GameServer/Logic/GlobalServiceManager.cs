using System;
using System.Collections.Generic;
using GameServer.Core.AssemblyPatch;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.AoYunDaTi;
using GameServer.Logic.BangHui.ZhanMengShiJian;
using GameServer.Logic.BocaiSys;
using GameServer.Logic.BossAI;
using GameServer.Logic.Building;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.Copy;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.FuMo;
using GameServer.Logic.GoldAuction;
using GameServer.Logic.Goods;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.KuaFuIPStatistics;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Logic.LiXianGuaJi;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Marriage.CoupleWish;
using GameServer.Logic.MoRi;
using GameServer.Logic.Olympics;
using GameServer.Logic.OnePiece;
using GameServer.Logic.Ornament;
using GameServer.Logic.Spread;
using GameServer.Logic.Talent;
using GameServer.Logic.Tarot;
using GameServer.Logic.Ten;
using GameServer.Logic.Today;
using GameServer.Logic.UnionAlly;
using GameServer.Logic.UnionPalace;
using GameServer.Logic.UserActivate;
using GameServer.Logic.UserReturn;
using GameServer.Logic.ZhuanPan;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using GameServer.Tools;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020006D3 RID: 1747
	public class GlobalServiceManager
	{
		// Token: 0x06002996 RID: 10646 RVA: 0x00238538 File Offset: 0x00236738
		public static bool RegisterManager4Scene(int ManagerType, IManager manager)
		{
			lock (GlobalServiceManager.Scene2ManagerDict)
			{
				List<IManager> list;
				if (!GlobalServiceManager.Scene2ManagerDict.TryGetValue(ManagerType, out list))
				{
					list = new List<IManager>();
					GlobalServiceManager.Scene2ManagerDict[ManagerType] = list;
				}
				if (!list.Contains(manager))
				{
					list.Add(manager);
				}
			}
			return true;
		}

		// Token: 0x06002997 RID: 10647 RVA: 0x002385C4 File Offset: 0x002367C4
		public static void initialize()
		{
			ZhanMengShiJianManager.getInstance().initialize();
			JingJiChangManager.getInstance().initialize();
			LiXianBaiTanManager.getInstance().initialize();
			LiXianGuaJiManager.getInstance().initialize();
			CmdRegisterTriggerManager.getInstance().initialize();
			SendCmdManager.getInstance().initialize();
			BossAIManager.getInstance().initialize();
			WashPropsManager.initialize();
			SaleManager.getInstance().initialize();
			LianZhiManager.GetInstance().initialize();
			ChengJiuManager.GetInstance().initialize();
			PrestigeMedalManager.getInstance().initialize();
			UnionPalaceManager.getInstance().initialize();
			UserActivateManager.getInstance().initialize();
			PetSkillManager.getInstance().initialize();
			UserReturnManager.getInstance().initialize();
			OlympicsManager.getInstance().initialize();
			TalentManager.getInstance().initialize();
			TodayManager.getInstance().initialize();
			FundManager.getInstance().initialize();
			WarnManager.getInstance().initialize();
			EMoLaiXiCopySceneManager.LoadEMoLaiXiCopySceneInfo();
			LuoLanFaZhenCopySceneManager.initialize();
			MarryFuBenMgr.getInstance().initialize();
			MarryLogic.LoadMarryBaseConfig();
			MarryPartyLogic.getInstance().LoadMarryPartyConfig();
			BuildingManager.getInstance().initialize();
			OnePieceManager.getInstance().initialize();
			GlobalServiceManager.RegisterManager4Scene(0, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalServiceManager.RegisterManager4Scene(0, KuaFuManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(35, LangHunLingYuManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, RebornManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, RebornBoss.getInstance());
			GlobalServiceManager.RegisterManager4Scene(24, LuoLanChengZhanManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, FashionManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, OrnamentManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, ShenJiFuWenManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, YaoSaiJianYuManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, AlchemyManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, EraManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, VideoLogic.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, SpecPlatFuLiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(25, HuanYingSiYuanManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(10000, JingLingQiYuanManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(26, TianTiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(27, YongZheZhanChangManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(39, KingOfBattleManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(45, BangHuiMatchManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(48, CompManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(52, CompBattleManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(53, CompMineManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(57, ZorkBattleManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(47, KuaFuLueDuoManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, KarenBattleManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(41, KarenBattleManager_MapWest.getInstance());
			GlobalServiceManager.RegisterManager4Scene(42, KarenBattleManager_MapEast.getInstance());
			GlobalServiceManager.RegisterManager4Scene(29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalServiceManager.RegisterManager4Scene(28, ElementWarManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(49, WanMoXiaGuManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(34, CopyWolfManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(31, KuaFuBossManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(10003, KuaFuMapManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(10002, SpreadManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalServiceManager.RegisterManager4Scene(38, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalServiceManager.RegisterManager4Scene(10004, AllyManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(10005, SingletonTemplate<CoupleWishManager>.Instance());
			GlobalServiceManager.RegisterManager4Scene(40, ZhengDuoManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, AoYunDaTiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, RoleManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, ZhuanPanManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, ShenQiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, JunTuanManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, LingDiCaiJiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, HongBaoManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, YaoSaiBossManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, YaoSaiMissionManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, HuiJiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, DeControl.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, GVoiceManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, ShenShiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, JueXingManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, ZuoQiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, ThemeBoss.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, ArmorManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, JingLingYuanSuJueXingManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, BianShenManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(55, TianTi5v5Manager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, UserRegressActiveManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, MountHolyStampManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, MazingerStoreManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, GlodAuctionProcessCmdEx.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, BoCaiManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, FunctionSendManager.GetInstance());
			GlobalServiceManager.RegisterManager4Scene(0, HuanLeDaiBiManager.GetInstance());
			GlobalServiceManager.RegisterManager4Scene(0, NewTimerProc.GetInstance());
			GlobalServiceManager.RegisterManager4Scene(56, ZhanDuiZhengBaManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(59, EscapeBattleManager.getInstance());
			GlobalServiceManager.RegisterManager4Scene(0, TestReceiveModel.getInstance());
			RobotTaskValidator.getInstance().Initialize(false, 0, 0, "");
			HolyItemManager.getInstance().Initialize();
			TarotManager.getInstance().Initialize();
			SingletonTemplate<SevenDayActivityMgr>.Instance().initialize();
			SingletonTemplate<SoulStoneManager>.Instance().initialize();
			SingletonTemplate<TradeBlackManager>.Instance().LoadConfig();
			lock (GlobalServiceManager.Scene2ManagerDict)
			{
				foreach (List<IManager> list in GlobalServiceManager.Scene2ManagerDict.Values)
				{
					foreach (IManager i in list)
					{
						bool success = true;
						try
						{
							success = (success && i.initialize());
							IManager2 m2 = i as IManager2;
							if (null != m2)
							{
								success = (success && m2.initialize(GameCoreInterface.getinstance()));
							}
						}
						catch (Exception ex)
						{
							success = false;
							LogManager.WriteException(ex.ToString());
						}
						if (GameManager.ServerStarting && !success)
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("执行{0}.initialize()失败,按任意键继续启动!", i.GetType()), null, true);
							Console.ReadKey();
						}
					}
				}
			}
			TenManager.getInstance().initialize();
			TenRetutnManager.getInstance().initialize();
			GiftCodeNewManager.getInstance().initialize();
			FaceBookManager.getInstance().initialize();
			AssemblyPatchManager.getInstance().initialize();
			IPStatisticsManager.getInstance().initialize();
			FuMoManager.getInstance().Initialize();
		}

		// Token: 0x06002998 RID: 10648 RVA: 0x00238CA0 File Offset: 0x00236EA0
		public static void startup()
		{
			ZhanMengShiJianManager.getInstance().startup();
			JingJiChangManager.getInstance().startup();
			LiXianBaiTanManager.getInstance().startup();
			LiXianGuaJiManager.getInstance().startup();
			CmdRegisterTriggerManager.getInstance().startup();
			SendCmdManager.getInstance().startup();
			BossAIManager.getInstance().startup();
			SaleManager.getInstance().startup();
			LianZhiManager.GetInstance().startup();
			ChengJiuManager.GetInstance().startup();
			UserReturnManager.getInstance().startup();
			OlympicsManager.getInstance().startup();
			TalentManager.getInstance().startup();
			TodayManager.getInstance().startup();
			FundManager.getInstance().startup();
			WarnManager.getInstance().startup();
			PrestigeMedalManager.getInstance().startup();
			UnionPalaceManager.getInstance().startup();
			UserActivateManager.getInstance().startup();
			PetSkillManager.getInstance().startup();
			BuildingManager.getInstance().startup();
			OnePieceManager.getInstance().startup();
			TenManager.getInstance().startup();
			SingletonTemplate<SevenDayActivityMgr>.Instance().startup();
			SingletonTemplate<SoulStoneManager>.Instance().startup();
			lock (GlobalServiceManager.Scene2ManagerDict)
			{
				foreach (List<IManager> list in GlobalServiceManager.Scene2ManagerDict.Values)
				{
					foreach (IManager i in list)
					{
						try
						{
							bool success = i.startup();
							if (GameManager.ServerStarting && !success)
							{
								LogManager.WriteLog(LogTypes.Fatal, string.Format("初始化{0}.startup()失败,按任意键忽略此错误并继续启动服务器!", i.GetType()), null, true);
								Console.ReadKey();
							}
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
					}
				}
			}
			FaceBookManager.getInstance().startup();
		}

		// Token: 0x06002999 RID: 10649 RVA: 0x00238EF8 File Offset: 0x002370F8
		public static void showdown()
		{
			ZhanMengShiJianManager.getInstance().showdown();
			JingJiChangManager.getInstance().showdown();
			LiXianBaiTanManager.getInstance().showdown();
			LiXianGuaJiManager.getInstance().showdown();
			CmdRegisterTriggerManager.getInstance().showdown();
			SendCmdManager.getInstance().showdown();
			BossAIManager.getInstance().showdown();
			SaleManager.getInstance().showdown();
			LianZhiManager.GetInstance().showdown();
			ChengJiuManager.GetInstance().showdown();
			PrestigeMedalManager.getInstance().showdown();
			UnionPalaceManager.getInstance().showdown();
			UserActivateManager.getInstance().showdown();
			PetSkillManager.getInstance().showdown();
			UserReturnManager.getInstance().showdown();
			OlympicsManager.getInstance().showdown();
			TalentManager.getInstance().showdown();
			TodayManager.getInstance().showdown();
			FundManager.getInstance().showdown();
			WarnManager.getInstance().showdown();
			BuildingManager.getInstance().showdown();
			OnePieceManager.getInstance().showdown();
			MarryLogic.ApplyShutdownClear();
			TenManager.getInstance().showdown();
			SingletonTemplate<SevenDayActivityMgr>.Instance().showdown();
			SingletonTemplate<SoulStoneManager>.Instance().showdown();
			lock (GlobalServiceManager.Scene2ManagerDict)
			{
				foreach (List<IManager> list in GlobalServiceManager.Scene2ManagerDict.Values)
				{
					foreach (IManager i in list)
					{
						try
						{
							i.showdown();
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
					}
				}
			}
			FaceBookManager.getInstance().showdown();
		}

		// Token: 0x0600299A RID: 10650 RVA: 0x00239118 File Offset: 0x00237318
		public static void destroy()
		{
			ZhanMengShiJianManager.getInstance().destroy();
			JingJiChangManager.getInstance().destroy();
			LiXianBaiTanManager.getInstance().destroy();
			LiXianGuaJiManager.getInstance().destroy();
			CmdRegisterTriggerManager.getInstance().destroy();
			SendCmdManager.getInstance().destroy();
			BossAIManager.getInstance().destroy();
			SaleManager.getInstance().destroy();
			LianZhiManager.GetInstance().destroy();
			ChengJiuManager.GetInstance().destroy();
			PrestigeMedalManager.getInstance().destroy();
			UnionPalaceManager.getInstance().destroy();
			UserActivateManager.getInstance().destroy();
			PetSkillManager.getInstance().destroy();
			UserReturnManager.getInstance().destroy();
			OlympicsManager.getInstance().destroy();
			TalentManager.getInstance().destroy();
			TodayManager.getInstance().destroy();
			FundManager.getInstance().destroy();
			WarnManager.getInstance().destroy();
			MarryFuBenMgr.getInstance().destroy();
			BuildingManager.getInstance().destroy();
			OnePieceManager.getInstance().destroy();
			TenManager.getInstance().destroy();
			SingletonTemplate<SevenDayActivityMgr>.Instance().destroy();
			SingletonTemplate<SoulStoneManager>.Instance().destroy();
			lock (GlobalServiceManager.Scene2ManagerDict)
			{
				foreach (List<IManager> list in GlobalServiceManager.Scene2ManagerDict.Values)
				{
					foreach (IManager i in list)
					{
						try
						{
							i.destroy();
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
					}
				}
			}
			FaceBookManager.getInstance().destroy();
		}

		// Token: 0x0600299B RID: 10651 RVA: 0x0023933C File Offset: 0x0023753C
		public static bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool success = true;
			lock (GlobalServiceManager.Scene2ManagerDict)
			{
				List<IManager> list;
				if (GlobalServiceManager.Scene2ManagerDict.TryGetValue((int)sceneType, out list))
				{
					foreach (IManager i in list)
					{
						ICopySceneManager m2 = i as ICopySceneManager;
						if (null != m2)
						{
							try
							{
								success = (success && m2.AddCopyScenes(client, copyMap, sceneType));
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
						}
					}
				}
			}
			return success;
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x00239430 File Offset: 0x00237630
		public static bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool success = true;
			lock (GlobalServiceManager.Scene2ManagerDict)
			{
				List<IManager> list;
				if (GlobalServiceManager.Scene2ManagerDict.TryGetValue((int)sceneType, out list))
				{
					foreach (IManager i in list)
					{
						ICopySceneManager m2 = i as ICopySceneManager;
						if (null != m2)
						{
							try
							{
								success = (success && m2.RemoveCopyScene(copyMap, sceneType));
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
						}
					}
				}
			}
			return success;
		}

		// Token: 0x0600299D RID: 10653 RVA: 0x00239524 File Offset: 0x00237724
		public static void TimerProc()
		{
			lock (GlobalServiceManager.Scene2ManagerDict)
			{
				foreach (List<IManager> list in GlobalServiceManager.Scene2ManagerDict.Values)
				{
					foreach (IManager i in list)
					{
						ICopySceneManager m2 = i as ICopySceneManager;
						if (null != m2)
						{
							try
							{
								m2.TimerProc();
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
						}
					}
				}
			}
		}

		// Token: 0x0400394F RID: 14671
		private static Dictionary<int, List<IManager>> Scene2ManagerDict = new Dictionary<int, List<IManager>>();
	}
}
