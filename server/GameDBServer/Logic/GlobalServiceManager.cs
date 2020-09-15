using System;
using GameDBServer.Logic.Activity.SevenDay;
using GameDBServer.Logic.AoYunDaTi;
using GameDBServer.Logic.BoCai;
using GameDBServer.Logic.CoupleArena;
using GameDBServer.Logic.Fund;
using GameDBServer.Logic.GoldAuction;
using GameDBServer.Logic.MerlinMagicBook;
using GameDBServer.Logic.UnionAlly;
using GameDBServer.Logic.UserReturn;
using GameDBServer.Logic.WanMoTa;
using GameDBServer.Logic.Wing;
using GameDBServer.Logic.ZhengBa;
using GameDBServer.Server.CmdProcessor;
using GameServer.Core.AssemblyPatch;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000134 RID: 308
	public class GlobalServiceManager
	{
		// Token: 0x0600052E RID: 1326 RVA: 0x0002B178 File Offset: 0x00029378
		public static void initialize()
		{
			ZhanMengShiJianManager.getInstance().initialize();
			JingJiChangManager.getInstance().initialize();
			WanMoTaManager.getInstance().initialize();
			WingPaiHangManager.getInstance().initialize();
			RingPaiHangManager.getInstance().initialize();
			MerlinRankManager.getInstance().initialize();
			CmdRegisterTriggerManager.getInstance().initialize();
			TianTiDbCmdProcessor.getInstance().registerProcessor();
			SingletonTemplate<SevenDayActivityManager>.Instance().initialize();
			SingletonTemplate<FundManager>.Instance().initialize();
			SingletonTemplate<UserReturnManager>.Instance().initialize();
			SingletonTemplate<OlympicsManager>.Instance().initialize();
			SingletonTemplate<ShenJiManager>.Instance().initialize();
			SingletonTemplate<TradeBlackManager>.Instance().initialize();
			SingletonTemplate<KingRoleDataManager>.Instance().initialize();
			SingletonTemplate<AlchemyManager>.Instance().initialize();
			SingletonTemplate<ZhengBaManager>.Instance().initialize();
			SingletonTemplate<CoupleArenaDbManager>.Instance().initialize();
			SingletonTemplate<AllyManager>.Instance().initialize();
			SingletonTemplate<AoYunDaTiManager>.Instance().initialize();
			SingletonTemplate<YaoSaiBossManager>.Instance().initialize();
			SingletonTemplate<RoleManager>.Instance().initialize();
			SingletonTemplate<YaoSaiMissionManager>.Instance().initialize();
			SingletonTemplate<HongBaoManager>.Instance().initialize();
			SingletonTemplate<HuiJiManager>.Instance().initialize();
			SingletonTemplate<ShenShiManager>.Instance().initialize();
			SingletonTemplate<JueXingManager>.Instance().initialize();
			SingletonTemplate<ZuoQiManager>.Instance().initialize();
			SingletonTemplate<ArmorManager>.Instance().initialize();
			SingletonTemplate<JingLingYuanSuJueXingManager>.Instance().initialize();
			SingletonTemplate<BianShenManager>.Instance().initialize();
			AssemblyPatchManager.getInstance().initialize();
			GlodAuctionMsgProcess.getInstance().initialize();
			BoCaiManager.getInstance().initialize();
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0002B2FC File Offset: 0x000294FC
		public static void startup()
		{
			ZhanMengShiJianManager.getInstance().startup();
			JingJiChangManager.getInstance().startup();
			WanMoTaManager.getInstance().startup();
			WingPaiHangManager.getInstance().startup();
			MerlinRankManager.getInstance().startup();
			CmdRegisterTriggerManager.getInstance().startup();
			SingletonTemplate<SevenDayActivityManager>.Instance().startup();
			SingletonTemplate<FundManager>.Instance().startup();
			SingletonTemplate<UserReturnManager>.Instance().startup();
			SingletonTemplate<OlympicsManager>.Instance().startup();
			SingletonTemplate<ShenJiManager>.Instance().startup();
			SingletonTemplate<AlchemyManager>.Instance().startup();
			SingletonTemplate<TradeBlackManager>.Instance().startup();
			SingletonTemplate<KingRoleDataManager>.Instance().startup();
			SingletonTemplate<ZhengBaManager>.Instance().startup();
			SingletonTemplate<CoupleArenaDbManager>.Instance().startup();
			SingletonTemplate<AllyManager>.Instance().startup();
			SingletonTemplate<AoYunDaTiManager>.Instance().startup();
			SingletonTemplate<YaoSaiBossManager>.Instance().startup();
			SingletonTemplate<RoleManager>.Instance().startup();
			SingletonTemplate<YaoSaiMissionManager>.Instance().startup();
			SingletonTemplate<HongBaoManager>.Instance().startup();
			SingletonTemplate<HuiJiManager>.Instance().startup();
			SingletonTemplate<ShenShiManager>.Instance().startup();
			SingletonTemplate<JueXingManager>.Instance().startup();
			SingletonTemplate<ZuoQiManager>.Instance().startup();
			SingletonTemplate<ArmorManager>.Instance().startup();
			SingletonTemplate<JingLingYuanSuJueXingManager>.Instance().startup();
			SingletonTemplate<BianShenManager>.Instance().startup();
			GlodAuctionMsgProcess.getInstance().startup();
			BoCaiManager.getInstance().startup();
			SingletonTemplate<RebornEquip>.Instance().startup();
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x0002B46C File Offset: 0x0002966C
		public static void showdown()
		{
			ZhanMengShiJianManager.getInstance().showdown();
			JingJiChangManager.getInstance().showdown();
			WanMoTaManager.getInstance().showdown();
			WingPaiHangManager.getInstance().showdown();
			MerlinRankManager.getInstance().showdown();
			CmdRegisterTriggerManager.getInstance().showdown();
			SingletonTemplate<SevenDayActivityManager>.Instance().showdown();
			SingletonTemplate<FundManager>.Instance().showdown();
			SingletonTemplate<UserReturnManager>.Instance().showdown();
			SingletonTemplate<OlympicsManager>.Instance().showdown();
			SingletonTemplate<ShenJiManager>.Instance().showdown();
			SingletonTemplate<TradeBlackManager>.Instance().showdown();
			SingletonTemplate<AlchemyManager>.Instance().showdown();
			SingletonTemplate<KingRoleDataManager>.Instance().showdown();
			SingletonTemplate<ZhengBaManager>.Instance().showdown();
			SingletonTemplate<CoupleArenaDbManager>.Instance().showdown();
			SingletonTemplate<AllyManager>.Instance().showdown();
			SingletonTemplate<RoleManager>.Instance().showdown();
			SingletonTemplate<HongBaoManager>.Instance().showdown();
			SingletonTemplate<HuiJiManager>.Instance().showdown();
			SingletonTemplate<ArmorManager>.Instance().showdown();
			SingletonTemplate<JingLingYuanSuJueXingManager>.Instance().showdown();
			SingletonTemplate<BianShenManager>.Instance().showdown();
			GlodAuctionMsgProcess.getInstance().showdown();
			BoCaiManager.getInstance().showdown();
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0002B590 File Offset: 0x00029790
		public static void destroy()
		{
			ZhanMengShiJianManager.getInstance().destroy();
			JingJiChangManager.getInstance().destroy();
			WanMoTaManager.getInstance().destroy();
			WingPaiHangManager.getInstance().destroy();
			MerlinRankManager.getInstance().destroy();
			CmdRegisterTriggerManager.getInstance().destroy();
			SingletonTemplate<SevenDayActivityManager>.Instance().destroy();
			SingletonTemplate<FundManager>.Instance().destroy();
			SingletonTemplate<UserReturnManager>.Instance().destroy();
			SingletonTemplate<OlympicsManager>.Instance().destroy();
			SingletonTemplate<ShenJiManager>.Instance().destroy();
			SingletonTemplate<TradeBlackManager>.Instance().showdown();
			SingletonTemplate<AlchemyManager>.Instance().showdown();
			SingletonTemplate<KingRoleDataManager>.Instance().destroy();
			SingletonTemplate<ZhengBaManager>.Instance().destroy();
			SingletonTemplate<CoupleArenaDbManager>.Instance().destroy();
			SingletonTemplate<AllyManager>.Instance().destroy();
			SingletonTemplate<RoleManager>.Instance().destroy();
			SingletonTemplate<HongBaoManager>.Instance().destroy();
			SingletonTemplate<HuiJiManager>.Instance().destroy();
			SingletonTemplate<ArmorManager>.Instance().destroy();
			SingletonTemplate<JingLingYuanSuJueXingManager>.Instance().destroy();
			SingletonTemplate<BianShenManager>.Instance().destroy();
			GlodAuctionMsgProcess.getInstance().destroy();
			BoCaiManager.getInstance().destroy();
		}
	}
}
