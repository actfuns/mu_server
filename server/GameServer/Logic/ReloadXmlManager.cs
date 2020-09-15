using System;
using System.Collections.Generic;
using GameServer.Core.AssemblyPatch;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.AoYunDaTi;
using GameServer.Logic.BocaiSys;
using GameServer.Logic.BossAI;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.ExtensionProps;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.GoldAuction;
using GameServer.Logic.KuaFuIPStatistics;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Name;
using GameServer.Logic.Olympics;
using GameServer.Logic.Ornament;
using GameServer.Logic.Talent;
using GameServer.Logic.Ten;
using GameServer.Logic.Today;
using GameServer.Logic.UserMoneyCharge;
using GameServer.Logic.UserReturn;
using GameServer.Logic.ZhuanPan;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	// Token: 0x02000787 RID: 1927
	public class ReloadXmlManager
	{
		// Token: 0x060031DA RID: 12762 RVA: 0x002C97B0 File Offset: 0x002C79B0
		public static int ReloadXmlFile(string xmlFileName)
		{
			string lowerXmlFileName = xmlFileName.ToLower();
			int result;
			if (Global.GetGiftExchangeFileName().ToLower() == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_activities();
			}
			else if ("config/gifts/biggift.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_biggift();
			}
			else if ("config/gifts/loginnumgift.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_loginnumgift();
			}
			else if ("config/gifts/huodongloginnumgift.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_huodongloginnumgift();
			}
			else if ("config/gifts/newrolegift.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_newrolegift();
			}
			else if ("config/gifts/comateffectivenessgift.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_combat_effectiveness_gift();
			}
			else if ("config/gifts/uplevelgift.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_uplevelgift();
			}
			else if ("config/gifts/onlietimegift.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_onlietimegift();
			}
			else if ("config/platconfig.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_platconfig();
			}
			else if ("config/mall.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_mall();
			}
			else if ("config/monstergoodslist.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_monstergoodslist();
			}
			else if ("config/broadcastinfos.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_broadcastinfos();
			}
			else if ("config/specialtimes.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_specialtimes();
			}
			else if ("config/battle.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_battle();
			}
			else if ("config/arenabattle.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_ArenaBattle();
			}
			else if ("config/popupwin.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_popupwin();
			}
			else if ("config/npcscripts.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_npcscripts();
			}
			else if ("config/systemoperations.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_systemoperations();
			}
			else if ("config/systemparams.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_systemparams();
			}
			else if ("config/goodsmergeitems.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_goodsmergeitems();
			}
			else if ("config/qizhengegoods.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_qizhengegoods();
			}
			else if ("config/npcsalelist.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_npcsalelist();
			}
			else if ("config/goods.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_goods();
			}
			else if ("config/goodspack.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_goodspack();
			}
			else if ("config/systemtasks.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_systemtasks();
			}
			else if ("config/taskzhangjie.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_taskzhangjie();
			}
			else if ("config/equipupgrade.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_equipupgrade();
			}
			else if ("config/dig.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_dig();
			}
			else if ("config/battleexp.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_battleexp();
			}
			else if ("config/rebirth.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_rebirth();
			}
			else if ("config/battleaward.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_Award();
			}
			else if ("config/equipborn.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_EquipBorn();
			}
			else if ("config/bornname.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_BornName();
			}
			else if ("config/gifts/fanli.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_FanLi();
			}
			else if ("config/gifts/chongzhisong.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_ChongZhiSong();
			}
			else if ("config/gifts/chongzhiking.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_ChongZhiKing();
			}
			else if ("config/gifts/levelking.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_LevelKing();
			}
			else if ("config/gifts/bossking.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_EquipKing();
			}
			else if ("config/gifts/wuxueking.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_HorseKing();
			}
			else if ("config/gifts/jingmaiking.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_JingMaiKing();
			}
			else if ("config/gifts/vipdailyawards.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_gifts_VipDailyAwards();
			}
			else if ("config/activity/activitytip.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_ActivityTip();
			}
			else if ("config/luckyaward.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_LuckyAward();
			}
			else if ("config/lucky.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_Lucky();
			}
			else if ("config/chengjiu.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_ChengJiu();
			}
			else if ("config/chengjiubuff.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_ChengJiuBuff();
			}
			else if (string.Format("Config\\JieRiGifts\\ JieRiDanBiChongZhi.xml", new object[0]).ToLower() == lowerXmlFileName)
			{
				result = HuodongCachingMgr.ResetDanBiChongZhiActivity();
			}
			else if ("config/jingmai.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_JingMai();
			}
			else if ("config/wuxue.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_WuXue();
			}
			else if ("config/zuanhuang.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_ZuanHuang();
			}
			else if ("config/vip.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_Vip();
			}
			else if ("config/qianggou.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_QiangGou();
			}
			else if ("config/hefugifts/hefuqianggou.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_HeFuQiangGou();
			}
			else if ("config/jierigifts/jirriqianggou.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_JieRiQiangGou();
			}
			else if ("config/systemopen.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_SystemOpen();
			}
			else if ("config/DailyActiveInfor.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_DailyActive();
			}
			else if ("DailyActiveAward.xml" == lowerXmlFileName)
			{
				result = ReloadXmlManager.ReloadXmlFile_config_DailyActiveAward();
			}
			else if ("config/ipwhitelist.xml" == lowerXmlFileName)
			{
				SingletonTemplate<CreateRoleLimitManager>.Instance().LoadConfig();
				result = 1;
			}
			else
			{
				if ("kuafu" == lowerXmlFileName)
				{
					if (KuaFuManager.getInstance().InitConfig())
					{
						return 1;
					}
				}
				else if ("langhunlingyu" == lowerXmlFileName)
				{
					if (LangHunLingYuManager.getInstance().InitConfig())
					{
						return 1;
					}
				}
				else if ("config/chongzhi_app.xml" == lowerXmlFileName || "config/chongzhi_andrid.xml" == lowerXmlFileName || "config/chongzhi_android.xml" == lowerXmlFileName || "config/chongzhi_yueyu.xml" == lowerXmlFileName)
				{
					UserMoneyMgr.getInstance().InitConfig();
				}
				else
				{
					if ("config/AssInfo.xml" == lowerXmlFileName || "config/AssList.xml" == lowerXmlFileName || "config/AssConfig.xml" == lowerXmlFileName)
					{
						return RobotTaskValidator.getInstance().LoadRobotTaskData() ? 1 : 0;
					}
					if ("Config/Auction.xml" == lowerXmlFileName || "Config/AngelTempleAuctionAward.xml" == lowerXmlFileName)
					{
						return GoldAuctionConfigModel.LoadConfig();
					}
					if ("Config/CaiShuZi.xml" == lowerXmlFileName || "Config/CaiDaXiao.xml" == lowerXmlFileName || "Config/DuiHuanShangCheng.xml" == lowerXmlFileName || "Config/DaiBiShiYong.xml" == lowerXmlFileName)
					{
						return BoCaiConfigMgr.LoadConfig(true);
					}
					if (lowerXmlFileName.IndexOf("Config\\Horse") > -1)
					{
						ZuoQiManager.getInstance().ReLoadConfig(false);
					}
				}
				result = -1000;
			}
			return result;
		}

		// Token: 0x060031DB RID: 12763 RVA: 0x002CA048 File Offset: 0x002C8248
		public static void ReloadAllXmlFile()
		{
			WorldLevelManager.getInstance().InitConfig();
			WorldLevelManager.getInstance().ResetWorldLevel();
			ReloadXmlManager.ReloadXmlFile_config_platconfig();
			ReloadXmlManager.ReloadXmlFile_config_gifts_activities();
			ReloadXmlManager.ReloadXmlFile_config_gifts_biggift();
			ReloadXmlManager.ReloadXmlFile_config_gifts_loginnumgift();
			ReloadXmlManager.ReloadXmlFile_config_gifts_huodongloginnumgift();
			ReloadXmlManager.ReloadXmlFile_config_gifts_newrolegift();
			ReloadXmlManager.ReloadXmlFile_config_combat_effectiveness_gift();
			ReloadXmlManager.ReloadXmlFile_config_gifts_uplevelgift();
			ReloadXmlManager.ReloadXmlFile_config_gifts_onlietimegift();
			ReloadXmlManager.ReloadXmlFile_config_mall();
			ReloadXmlManager.ReloadXmlFile_config_monstergoodslist();
			ReloadXmlManager.ReloadXmlFile_config_broadcastinfos();
			ReloadXmlManager.ReloadXmlFile_config_specialtimes();
			ReloadXmlManager.ReloadXmlFile_config_battle();
			ReloadXmlManager.ReloadXmlFile_config_ArenaBattle();
			ReloadXmlManager.ReloadXmlFile_config_popupwin();
			ReloadXmlManager.ReloadXmlFile_config_npcscripts();
			ReloadXmlManager.ReloadXmlFile_config_systemoperations();
			ReloadXmlManager.ReloadXmlFile_config_systemparams();
			ReloadXmlManager.ReloadXmlFile_config_goodsmergeitems();
			ReloadXmlManager.ReloadXmlFile_config_qizhengegoods();
			ReloadXmlManager.ReloadXmlFile_config_npcsalelist();
			ReloadXmlManager.ReloadXmlFile_config_goods();
			ReloadXmlManager.ReloadXmlFile_config_goodspack();
			ReloadXmlManager.ReloadXmlFile_config_systemtasks();
			ReloadXmlManager.ReloadXmlFile_config_equipupgrade();
			ReloadXmlManager.ReloadXmlFile_config_dig();
			ReloadXmlManager.ReloadXmlFile_config_battleexp();
			ReloadXmlManager.ReloadXmlFile_config_bangzhanawards();
			ReloadXmlManager.ReloadXmlFile_config_rebirth();
			ReloadXmlManager.ReloadXmlFile_config_Award();
			ReloadXmlManager.ReloadXmlFile_config_EquipBorn();
			ReloadXmlManager.ReloadXmlFile_config_BornName();
			ReloadXmlManager.ReloadXmlFile_config_gifts_FanLi();
			ReloadXmlManager.ReloadXmlFile_config_gifts_ChongZhiSong();
			ReloadXmlManager.ReloadXmlFile_config_gifts_ChongZhiKing();
			ReloadXmlManager.ReloadXmlFile_config_gifts_LevelKing();
			ReloadXmlManager.ReloadXmlFile_config_gifts_EquipKing();
			ReloadXmlManager.ReloadXmlFile_config_gifts_HorseKing();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JingMaiKing();
			ReloadXmlManager.ReloadXmlFile_config_gifts_VipDailyAwards();
			ReloadXmlManager.ReloadXmlFile_config_ActivityTip();
			ReloadXmlManager.ReloadXmlFile_config_LuckyAward();
			ReloadXmlManager.ReloadXmlFile_config_Lucky();
			ReloadXmlManager.ReloadXmlFile_config_ChengJiu();
			ReloadXmlManager.ReloadXmlFile_config_ChengJiuBuff();
			ReloadXmlManager.ReloadXmlFile_config_JingMai();
			ReloadXmlManager.ReloadXmlFile_config_WuXue();
			ReloadXmlManager.ReloadXmlFile_config_ZuanHuang();
			ReloadXmlManager.ReloadXmlFile_config_Vip();
			ReloadXmlManager.ReloadXmlFile_config_QiangGou();
			ReloadXmlManager.ReloadXmlFile_config_HeFuQiangGou();
			ReloadXmlManager.ReloadXmlFile_config_JieRiQiangGou();
			ReloadXmlManager.ReloadXmlFile_config_SystemOpen();
			ReloadXmlManager.ReloadXmlFile_config_DailyActive();
			ReloadXmlManager.ReloadXmlFile_config_DailyActiveAward();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiType();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiLiBao();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiDengLu();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiVip();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiChongZhiSong();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiLeiJi();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiBaoXiang();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiXiaoFeiKing();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiChongZhiKing();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiTotalConsume();
			ReloadXmlManager.ReloadXmlFile_config_gifts_JieRiMultAward();
			ReloadXmlManager.ReloadXmlFile_config_bossAI();
			ReloadXmlManager.ReloadXmlFile_config_TuoZhan();
			ReloadXmlManager.ReloadXmlFile_config_MoJingAndQiFu();
			ReloadXmlManager.ReloadXmlFile_config_TotalLoginDataInfo();
			GameManager.SystemMagicsMgr.ReloadLoadFromXMlFile();
			GameManager.SystemMagicQuickMgr.LoadMagicItemsDict(GameManager.SystemMagicsMgr);
			HuodongCachingMgr.ResetXinXiaoFeiKingActivity();
			HuodongCachingMgr.ResetHeFuActivityConfig();
			HuodongCachingMgr.ResetHeFuLoginActivity();
			HuodongCachingMgr.ResetHeFuTotalLoginActivity();
			HuodongCachingMgr.ResetHeFuRechargeActivity();
			HuodongCachingMgr.ResetHeFuPKKingActivity();
			HuodongCachingMgr.ResetHeFuAwardTimeActivity();
			HuodongCachingMgr.ResetHeFuLuoLanActivity();
			HuodongCachingMgr.ResetThemeActivityConfig();
			HuodongCachingMgr.ResetThemeDaLiBaoActivity();
			HuodongCachingMgr.ResetThemeDuiHuanActivity();
			HuodongCachingMgr.ResetThemeZhiGouActivity();
			HuodongCachingMgr.ResetJieriActivityConfig();
			HuodongCachingMgr.ResetJieriDaLiBaoActivity();
			HuodongCachingMgr.ResetJieRiDengLuActivity();
			HuodongCachingMgr.ResetJieriCZSongActivity();
			HuodongCachingMgr.ResetJieRiLeiJiCZActivity();
			HuodongCachingMgr.ResetJieRiTotalConsumeActivity();
			HuodongCachingMgr.ResetJieRiMultAwardActivity();
			HuodongCachingMgr.ResetJieRiZiKaLiaBaoActivity();
			HuodongCachingMgr.ResetJieRiXiaoFeiKingActivity();
			HuodongCachingMgr.ResetJieRiCZKingActivity();
			HuodongCachingMgr.ResetJieriGiveActivity();
			HuodongCachingMgr.ResetJieRiGiveKingActivity();
			HuodongCachingMgr.ResetJieriRecvKingActivity();
			HuodongCachingMgr.ResetJieRiFanLiAwardActivity();
			HuodongCachingMgr.ResetJieriLianXuChargeActivity();
			HuodongCachingMgr.ResetJieriRecvActivity();
			HuodongCachingMgr.ResetJieriPlatChargeKingActivity();
			HuodongCachingMgr.ResetFirstChongZhiGift();
			HuodongCachingMgr.ResetDanBiChongZhiActivity();
			HuodongCachingMgr.ResetTotalChargeActivity();
			HuodongCachingMgr.ResetTotalConsumeActivity();
			HuodongCachingMgr.ResetSeriesLoginItem();
			HuodongCachingMgr.ResetEveryDayOnLineAwardItem();
			HuodongCachingMgr.ResetJieriIPointsExchangeActivity();
			HuodongCachingMgr.ResetJieriFuLiActivity();
			HuodongCachingMgr.ResetJieriVIPYouHuiAct();
			HuodongCachingMgr.ResetJieRiMeiRiLeiJiActivity();
			HuodongCachingMgr.ResetJieriPCKingActivityEveryDay();
			OlympicsManager.getInstance().InitOlympics();
			UserReturnManager.getInstance().initConfigInfo();
			HuodongCachingMgr.ResetSpecPriorityActivity();
			HuodongCachingMgr.ResetXinFanLiActivity();
			HuodongCachingMgr.ResetWeedEndInputActivity();
			HuodongCachingMgr.ResetSpecialActivity();
			HuodongCachingMgr.ResetJieRiCZQGActivity();
			HuodongCachingMgr.ResetOneDollarBuyActivity();
			HuodongCachingMgr.ResetJieRiSuperInputFanLiActivity();
			HuodongCachingMgr.ResetOneDollarChongZhiActivity();
			HuodongCachingMgr.ResetEverydayActivity();
			HuodongCachingMgr.ResetInputFanLiNewActivity();
			HuodongCachingMgr.ResetRegressActiveOpen();
			HuodongCachingMgr.ResetRegressActiveTotalRecharge();
			HuodongCachingMgr.ResetRegressActiveStore();
			HuodongCachingMgr.ResetRegressActiveDayBuy();
			HuodongCachingMgr.ResetRegressActiveSignGift();
			WebOldPlayerManager.ReloadXml();
			TenManager.initConfig();
			Global.CachingJieriXmlData = null;
			Global.CachingSpecActXmlData = null;
			Global.CachingEverydayActXmlData = null;
			Global.CachingThemeActXmlData = null;
			Global.CachingSpecPriorityActXmlData = null;
			TodayManager.InitConfig();
			BuChangManager.ResetBuChangItemDict();
			HuodongCachingMgr.ResetMeiRiChongZhiActivity();
			HuodongCachingMgr.ResetChongJiHaoLiActivity();
			HuodongCachingMgr.ResetShenZhuangJiQiHuiKuiHaoLiActivity();
			HuodongCachingMgr.ResetYueDuZhuanPanActivity();
			GongGaoDataManager.LoadGongGaoData();
			SaleManager.InitConfig();
			GameManager.systemImpetrateByLevelMgr.ReloadLoadFromXMlFile();
			QianKunManager.LoadImpetrateItemsInfo();
			QianKunManager.LoadImpetrateItemsInfoFree();
			QianKunManager.LoadImpetrateItemsInfoHuodong();
			QianKunManager.LoadImpetrateItemsInfoFreeHuoDong();
			QianKunManager.LoadImpetrateItemsInfoTeQuan();
			QianKunManager.LoadImpetrateItemsInfoFreeTeQuan();
			GameManager.systemXingYunChouJiangMgr.ReloadLoadFromXMlFile();
			GameManager.systemYueDuZhuanPanChouJiangMgr.ReloadLoadFromXMlFile();
			Global.LoadSpecialMachineConfig();
			ElementhrtsManager.LoadRefineType();
			ElementhrtsManager.LoadElementHrtsBase();
			ElementhrtsManager.LoadElementHrtsLevelInfo();
			ElementhrtsManager.LoadSpecialElementHrtsExp();
			GameManager.QingGongYanMgr.LoadQingGongYanConfig();
			CopyTargetManager.LoadConfig();
			CallPetManager.LoadCallPetType();
			CallPetManager.LoadCallPetConfig();
			CallPetManager.LoadCallPetSystem();
			ShenShiManager.getInstance().ReloadConfig();
			Global.LoadGuWuMaps();
			Global.LoadAutoReviveMaps();
			GameManager.MonsterZoneMgr.LoadAllMonsterXml();
			GameManager.VersionSystemOpenMgr.LoadVersionSystemOpenData();
			UserMoneyMgr.getInstance().InitConfig();
			RobotTaskValidator.getInstance().LoadRobotTaskData();
			GameManager.MerlinMagicBookMgr.LoadMerlinConfigData();
			GameManager.FluorescentGemMgr.LoadFluorescentGemConfigData();
			SingletonTemplate<GetInterestingDataMgr>.Instance().LoadConfig();
			SingletonTemplate<CreateRoleLimitManager>.Instance().LoadConfig();
			TianTiManager.getInstance().InitConfig(true);
			TianTi5v5Manager.getInstance().InitConfig(true);
			YongZheZhanChangManager.getInstance().InitConfig();
			KingOfBattleManager.getInstance().InitConfig();
			BangHuiMatchManager.getInstance().InitConfig();
			ZorkBattleManager.getInstance().InitConfig();
			KarenBattleManager.getInstance().InitConfig();
			KarenBattleManager_MapWest.getInstance().InitConfig();
			KarenBattleManager_MapEast.getInstance().InitConfig();
			KuaFuBossManager.getInstance().InitConfig();
			KuaFuMapManager.getInstance().InitConfig();
			FashionManager.getInstance().InitConfig();
			OrnamentManager.getInstance().InitConfig();
			ShenJiFuWenManager.getInstance().InitConfig();
			YaoSaiJianYuManager.getInstance().InitConfig();
			AlchemyManager.getInstance().InitConfig();
			ZuoQiManager.getInstance().ReLoadConfig(false);
			RebornManager.getInstance().InitConfig(true);
			RebornBoss.getInstance().InitConfig();
			SpecPlatFuLiManager.getInstance().InitConfig();
			EraManager.getInstance().InitConfig();
			JingLingQiYuanManager.getInstance().InitConfig();
			AllThingsCalcItem.InitAllForgeLevelInfo();
			SingletonTemplate<TradeBlackManager>.Instance().LoadConfig();
			Global.LoadLangDict();
			LogFilterConfig.InitConfig();
			TenRetutnManager.getInstance().InitConfig();
			VideoLogic.getInstance().LoadVideoXml();
			Data.LoadConfig();
			GiftCodeNewManager.getInstance().initGiftCode();
			AoYunDaTiManager.getInstance().LoadConfig();
			ZhuanPanManager.getInstance().LoadConfig();
			JueXingManager.getInstance().LoadConfig();
			TalentManager.LoadTalentSpecialData();
			AssemblyPatchManager.getInstance().initialize();
			IPStatisticsManager.getInstance().LoadConfig();
			JunTuanManager.getInstance().InitConfig();
			HongBaoManager.getInstance().InitConfig();
			HuiJiManager.getInstance().InitConfig();
			DeControl.getInstance().InitConfig();
			GVoiceManager.getInstance().InitConfig();
			KuaFuLueDuoManager.getInstance().InitConfig();
			WanMoXiaGuManager.getInstance().InitConfig();
			ThemeBoss.getInstance().InitConfig();
			ArmorManager.getInstance().InitConfig();
			CompBattleManager.getInstance().InitConfig();
			CompMineManager.getInstance().InitConfig();
			JingLingYuanSuJueXingManager.getInstance().LoadConfig();
			BianShenManager.getInstance().InitConfig();
			ZhanDuiZhengBaManager.getInstance().InitConfig();
			EscapeBattleManager.getInstance().InitConfig();
			MazingerStoreManager.getInstance().InitConfig();
			BuffManager.InitConfig();
			GoldAuctionConfigModel.LoadConfig();
			BoCaiConfigMgr.LoadConfig(true);
		}

		// Token: 0x060031DC RID: 12764 RVA: 0x002CA71C File Offset: 0x002C891C
		private static int ReloadXmlFile_config_gifts_activities()
		{
			Global._activitiesData = null;
			return HuodongCachingMgr.ResetSongLiItem();
		}

		// Token: 0x060031DD RID: 12765 RVA: 0x002CA73C File Offset: 0x002C893C
		private static int ReloadXmlFile_config_gifts_biggift()
		{
			return HuodongCachingMgr.ResetBigAwardItem();
		}

		// Token: 0x060031DE RID: 12766 RVA: 0x002CA754 File Offset: 0x002C8954
		private static int ReloadXmlFile_config_gifts_loginnumgift()
		{
			return HuodongCachingMgr.ResetWLoginItem();
		}

		// Token: 0x060031DF RID: 12767 RVA: 0x002CA76C File Offset: 0x002C896C
		public static int ReloadXmlFile_config_gifts_huodongloginnumgift()
		{
			return HuodongCachingMgr.ResetLimitTimeLoginItem();
		}

		// Token: 0x060031E0 RID: 12768 RVA: 0x002CA784 File Offset: 0x002C8984
		private static int ReloadXmlFile_config_gifts_newrolegift()
		{
			return HuodongCachingMgr.ResetNewStepItem();
		}

		// Token: 0x060031E1 RID: 12769 RVA: 0x002CA79C File Offset: 0x002C899C
		private static int ReloadXmlFile_config_combat_effectiveness_gift()
		{
			return HuodongCachingMgr.ResetCombatAwardItem();
		}

		// Token: 0x060031E2 RID: 12770 RVA: 0x002CA7B4 File Offset: 0x002C89B4
		private static int ReloadXmlFile_config_gifts_uplevelgift()
		{
			return HuodongCachingMgr.ResetUpLevelItem();
		}

		// Token: 0x060031E3 RID: 12771 RVA: 0x002CA7CC File Offset: 0x002C89CC
		private static int ReloadXmlFile_config_gifts_onlietimegift()
		{
			return HuodongCachingMgr.ResetMOnlineTimeItem();
		}

		// Token: 0x060031E4 RID: 12772 RVA: 0x002CA7E4 File Offset: 0x002C89E4
		private static int ReloadXmlFile_config_platconfig()
		{
			return GameManager.PlatConfigMgr.ReloadPlatConfig();
		}

		// Token: 0x060031E5 RID: 12773 RVA: 0x002CA800 File Offset: 0x002C8A00
		private static int ReloadXmlFile_config_mall()
		{
			MallPriceMgr.ClearCache();
			Global._MallSaleData = null;
			return GameManager.systemMallMgr.ReloadLoadFromXMlFile();
		}

		// Token: 0x060031E6 RID: 12774 RVA: 0x002CA828 File Offset: 0x002C8A28
		private static int ReloadXmlFile_config_monstergoodslist()
		{
			return GameManager.GoodsPackMgr.ResetCachingItems();
		}

		// Token: 0x060031E7 RID: 12775 RVA: 0x002CA844 File Offset: 0x002C8A44
		private static int ReloadXmlFile_config_broadcastinfos()
		{
			try
			{
				BroadcastInfoMgr.LoadBroadcastInfoItemList();
			}
			catch (Exception)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x060031E8 RID: 12776 RVA: 0x002CA87C File Offset: 0x002C8A7C
		private static int ReloadXmlFile_config_specialtimes()
		{
			return SpecailTimeManager.ResetSpecialTimeLimits();
		}

		// Token: 0x060031E9 RID: 12777 RVA: 0x002CA894 File Offset: 0x002C8A94
		private static int ReloadXmlFile_config_battle()
		{
			int ret = GameManager.SystemBattle.ReloadLoadFromXMlFile();
			GameManager.BattleMgr.LoadParams();
			return ret;
		}

		// Token: 0x060031EA RID: 12778 RVA: 0x002CA8C0 File Offset: 0x002C8AC0
		private static int ReloadXmlFile_config_ArenaBattle()
		{
			int ret = GameManager.SystemArenaBattle.ReloadLoadFromXMlFile();
			GameManager.ArenaBattleMgr.LoadParams();
			return ret;
		}

		// Token: 0x060031EB RID: 12779 RVA: 0x002CA8EC File Offset: 0x002C8AEC
		private static int ReloadXmlFile_config_popupwin()
		{
			try
			{
				PopupWinMgr.LoadPopupWinItemList();
			}
			catch (Exception)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x060031EC RID: 12780 RVA: 0x002CA924 File Offset: 0x002C8B24
		private static int ReloadXmlFile_config_npcscripts()
		{
			int ret = 0;
			try
			{
				ret = GameManager.systemNPCScripts.ReloadLoadFromXMlFile();
				GameManager.SystemMagicActionMgr.ParseNPCScriptActions(GameManager.systemNPCScripts);
				Global.ClearNPCScriptTimeLimits();
			}
			catch (Exception)
			{
				return -1;
			}
			return ret;
		}

		// Token: 0x060031ED RID: 12781 RVA: 0x002CA978 File Offset: 0x002C8B78
		private static int ReloadXmlFile_config_systemoperations()
		{
			int ret = GameManager.SystemOperasMgr.ReloadLoadFromXMlFile();
			Global.ClearNPCOperationTimeLimits();
			return ret;
		}

		// Token: 0x060031EE RID: 12782 RVA: 0x002CA99C File Offset: 0x002C8B9C
		private static int ReloadXmlFile_config_systemparams()
		{
			int ret = GameManager.systemParamsList.ReloadLoadParamsList();
			JunQiManager.ParseWeekDaysTimes();
			LuoLanChengZhanManager.getInstance().InitConfig();
			Global.ResetHuangChengMapCode();
			Global.ResetHuangGongMapCode();
			Global.HorseNamesList = null;
			Global.HorseSpeedList = null;
			GameManager.ShengXiaoGuessMgr.ReloadConfig(false);
			Global.InitGuMuMapCodes();
			Global.InitVipGumuExpMultiple();
			GameManager.GoodsPackMgr.ResetLimitTimeRange();
			Global.ErGuoTouGoodsIDList = null;
			Global._VipUseBindTongQianGoodsIDNum = null;
			GameManager.AutoGiveGoodsIDList = null;
			CaiJiLogic.LoadConfig();
			GameManager.MagicSwordMgr.LoadMagicSwordData();
			GameManager.SummonerMgr.LoadSummonerData();
			GameManager.MerlinMagicBookMgr.LoadMerlinSystemParamsConfigData();
			Global.LoadItemLogMark();
			Global.LoadLogTradeGoods();
			Global.LoadForgeSystemParams();
			KuaFuManager.getInstance().InitCopyTime();
			SingletonTemplate<SoulStoneManager>.Instance().LoadJingHuaExpConfig();
			SingletonTemplate<MonsterAttackerLogManager>.Instance().LoadRecordMonsters();
			SingletonTemplate<CreateRoleLimitManager>.Instance().LoadConfig();
			SingletonTemplate<SpeedUpTickCheck>.Instance().LoadConfig();
			SingletonTemplate<NameManager>.Instance().LoadConfig();
			SingletonTemplate<CoupleArenaManager>.Instance().InitSystenParams();
			return ret;
		}

		// Token: 0x060031EF RID: 12783 RVA: 0x002CAAA4 File Offset: 0x002C8CA4
		private static int ReloadXmlFile_config_goodsmergeitems()
		{
			return MergeNewGoods.ReloadCacheMergeItems();
		}

		// Token: 0x060031F0 RID: 12784 RVA: 0x002CAABC File Offset: 0x002C8CBC
		private static int ReloadXmlFile_config_qizhengegoods()
		{
			int ret = GameManager.systemQiZhenGeGoodsMgr.ReloadLoadFromXMlFile();
			QiZhenGeManager.ClearQiZhenGeCachingItems();
			return ret;
		}

		// Token: 0x060031F1 RID: 12785 RVA: 0x002CAAE0 File Offset: 0x002C8CE0
		private static int ReloadXmlFile_config_npcsalelist()
		{
			return GameManager.NPCSaleListMgr.ReloadSaleList() ? 0 : -1;
		}

		// Token: 0x060031F2 RID: 12786 RVA: 0x002CAB04 File Offset: 0x002C8D04
		private static int ReloadXmlFile_config_goods()
		{
			int ret = GameManager.SystemGoods.ReloadLoadFromXMlFile();
			int result;
			if (ret < 0)
			{
				result = ret;
			}
			else
			{
				GameManager.SystemGoodsNamgMgr.LoadGoodsItemsDict(GameManager.SystemGoods);
				GameManager.SystemMagicActionMgr.ParseGoodsActions(GameManager.SystemGoods);
				GameManager.BattleMgr.ReloadGiveAwardsGoodsDataList(null);
				GameManager.ArenaBattleMgr.ReloadGiveAwardsGoodsDataList(null);
				Global.ClearEquipGoodsMaxStrongDict();
				GameManager.EquipPropsMgr.ClearCachedEquipPropItem();
				Global.ClearCachedGoodsShouShiSuitID();
				Global.ResetCachedGoodsQuality();
				result = ret;
			}
			return result;
		}

		// Token: 0x060031F3 RID: 12787 RVA: 0x002CAB88 File Offset: 0x002C8D88
		private static int ReloadXmlFile_config_goodspack()
		{
			int ret = GameManager.systemGoodsBaoGuoMgr.ReloadLoadFromXMlFile();
			int result;
			if (ret < 0)
			{
				result = ret;
			}
			else
			{
				result = GoodsBaoGuoCachingMgr.LoadGoodsBaoGuoDict();
			}
			return result;
		}

		// Token: 0x060031F4 RID: 12788 RVA: 0x002CABBC File Offset: 0x002C8DBC
		private static int ReloadXmlFile_config_systemtasks()
		{
			int ret = GameManager.SystemTasksMgr.ReloadLoadFromXMlFile();
			ProcessTask.InitBranchTasks(GameManager.SystemTasksMgr.SystemXmlItemDict);
			GameManager.NPCTasksMgr.LoadNPCTasks(GameManager.SystemTasksMgr);
			GameManager.TaskAwardsMgr.ClearAllDictionary();
			return ret;
		}

		// Token: 0x060031F5 RID: 12789 RVA: 0x002CAC08 File Offset: 0x002C8E08
		public static int ReloadXmlFile_config_taskzhangjie()
		{
			int ret = GameManager.TaskZhangJie.ReloadLoadFromXMlFile();
			if (ret >= 0)
			{
				ReloadXmlManager.InitTaskZhangJieInfo();
			}
			return ret;
		}

		// Token: 0x060031F6 RID: 12790 RVA: 0x002CAC38 File Offset: 0x002C8E38
		public static void InitTaskZhangJieInfo()
		{
			try
			{
				GameManager.TaskZhangJieDict.Clear();
				int startTaskID = 0;
				int endTaskID = 0;
				SystemXmlItem preXmlItem = null;
				foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.TaskZhangJie.SystemXmlItemDict)
				{
					endTaskID = kv.Value.GetIntValue("EndTaskID", -1);
					if (startTaskID != 0)
					{
						GameManager.TaskZhangJieDict.Add(new RangeKey(startTaskID, endTaskID - 1, preXmlItem));
					}
					startTaskID = endTaskID;
					preXmlItem = kv.Value;
				}
				GameManager.TaskZhangJieDict.Add(new RangeKey(endTaskID, int.MaxValue, preXmlItem));
			}
			catch (Exception)
			{
				throw new Exception(string.Format("Init xml file : {0} fail", string.Format("Config/TaskZhangJie.xml", new object[0])));
			}
		}

		// Token: 0x060031F7 RID: 12791 RVA: 0x002CAD2C File Offset: 0x002C8F2C
		private static int ReloadXmlFile_config_equipupgrade()
		{
			EquipUpgradeCacheMgr.LoadEquipUpgradeItems();
			return 0;
		}

		// Token: 0x060031F8 RID: 12792 RVA: 0x002CAD48 File Offset: 0x002C8F48
		private static int ReloadXmlFile_config_dig()
		{
			return GameManager.systemWaBaoMgr.ReloadLoadFromXMlFile();
		}

		// Token: 0x060031F9 RID: 12793 RVA: 0x002CAD68 File Offset: 0x002C8F68
		private static int ReloadXmlFile_config_battleexp()
		{
			int ret = GameManager.systemBattleExpMgr.ReloadLoadFromXMlFile();
			GameManager.BattleMgr.ClearBattleExpByLevels();
			return ret;
		}

		// Token: 0x060031FA RID: 12794 RVA: 0x002CAD94 File Offset: 0x002C8F94
		private static int ReloadXmlFile_config_bangzhanawards()
		{
			int ret = GameManager.systemBangZhanAwardsMgr.ReloadLoadFromXMlFile();
			BangZhanAwardsMgr.ClearAwardsByLevels();
			return ret;
		}

		// Token: 0x060031FB RID: 12795 RVA: 0x002CADB8 File Offset: 0x002C8FB8
		private static int ReloadXmlFile_config_rebirth()
		{
			return GameManager.systemBattleRebirthMgr.ReloadLoadFromXMlFile();
		}

		// Token: 0x060031FC RID: 12796 RVA: 0x002CADD8 File Offset: 0x002C8FD8
		private static int ReloadXmlFile_config_Award()
		{
			int ret = GameManager.systemBattleAwardMgr.ReloadLoadFromXMlFile();
			GameManager.BattleMgr.ClearBattleAwardByScore();
			return ret;
		}

		// Token: 0x060031FD RID: 12797 RVA: 0x002CAE04 File Offset: 0x002C9004
		private static int ReloadXmlFile_config_EquipBorn()
		{
			return GameManager.systemEquipBornMgr.ReloadLoadFromXMlFile();
		}

		// Token: 0x060031FE RID: 12798 RVA: 0x002CAE24 File Offset: 0x002C9024
		private static int ReloadXmlFile_config_BornName()
		{
			return GameManager.systemBornNameMgr.ReloadLoadFromXMlFile();
		}

		// Token: 0x060031FF RID: 12799 RVA: 0x002CAE44 File Offset: 0x002C9044
		private static int ReloadXmlFile_config_bossAI()
		{
			int ret = GameManager.SystemBossAI.ReloadLoadFromXMlFile();
			int result;
			if (ret < 0)
			{
				result = ret;
			}
			else
			{
				GameManager.SystemMagicActionMgr.ParseBossAIActions(GameManager.SystemBossAI);
				BossAICachingMgr.LoadBossAICachingItems(GameManager.SystemBossAI);
				result = ret;
			}
			return result;
		}

		// Token: 0x06003200 RID: 12800 RVA: 0x002CAE90 File Offset: 0x002C9090
		private static int ReloadXmlFile_config_TuoZhan()
		{
			int ret = GameManager.SystemExtensionProps.ReloadLoadFromXMlFile();
			int result;
			if (ret < 0)
			{
				result = ret;
			}
			else
			{
				GameManager.SystemMagicActionMgr.ParseExtensionPropsActions(GameManager.SystemExtensionProps);
				ExtensionPropsMgr.LoadCachingItems(GameManager.SystemExtensionProps);
				result = ret;
			}
			return result;
		}

		// Token: 0x06003201 RID: 12801 RVA: 0x002CAEDC File Offset: 0x002C90DC
		public static int ReloadXmlFile_config_gifts_FanLi()
		{
			return HuodongCachingMgr.ResetInputFanLiActivity();
		}

		// Token: 0x06003202 RID: 12802 RVA: 0x002CAEF4 File Offset: 0x002C90F4
		public static int ReloadXmlFile_config_gifts_ChongZhiSong()
		{
			return HuodongCachingMgr.ResetInputSongActivity();
		}

		// Token: 0x06003203 RID: 12803 RVA: 0x002CAF0C File Offset: 0x002C910C
		public static int ReloadXmlFile_config_gifts_ChongZhiKing()
		{
			return HuodongCachingMgr.ResetInputKingActivity();
		}

		// Token: 0x06003204 RID: 12804 RVA: 0x002CAF24 File Offset: 0x002C9124
		public static int ReloadXmlFile_config_gifts_LevelKing()
		{
			return HuodongCachingMgr.ResetLevelKingActivity();
		}

		// Token: 0x06003205 RID: 12805 RVA: 0x002CAF3C File Offset: 0x002C913C
		public static int ReloadXmlFile_config_gifts_EquipKing()
		{
			return HuodongCachingMgr.ResetEquipKingActivity();
		}

		// Token: 0x06003206 RID: 12806 RVA: 0x002CAF54 File Offset: 0x002C9154
		public static int ReloadXmlFile_config_gifts_HorseKing()
		{
			return HuodongCachingMgr.ResetHorseKingActivity();
		}

		// Token: 0x06003207 RID: 12807 RVA: 0x002CAF6C File Offset: 0x002C916C
		public static int ReloadXmlFile_config_gifts_JingMaiKing()
		{
			return HuodongCachingMgr.ResetJingMaiKingActivity();
		}

		// Token: 0x06003208 RID: 12808 RVA: 0x002CAF84 File Offset: 0x002C9184
		private static int ReloadXmlFile_config_gifts_VipDailyAwards()
		{
			GameManager.systemVipDailyAwardsMgr.LoadFromXMlFile("Config/Gifts/VipDailyAwards.xml", "", "AwardID", 1);
			return 1;
		}

		// Token: 0x06003209 RID: 12809 RVA: 0x002CAFB4 File Offset: 0x002C91B4
		private static int ReloadXmlFile_config_ActivityTip()
		{
			GameManager.systemActivityTipMgr.LoadFromXMlFile("Config/Activity/ActivityTip.xml", "", "ID", 0);
			return 1;
		}

		// Token: 0x0600320A RID: 12810 RVA: 0x002CAFE4 File Offset: 0x002C91E4
		private static int ReloadXmlFile_config_LuckyAward()
		{
			GameManager.systemLuckyAwardMgr.LoadFromXMlFile("Config/LuckyAward.xml", "", "ID", 0);
			GameManager.systemLuckyAward2Mgr.LoadFromXMlFile("Config/LuckyAward2.xml", "", "ID", 0);
			return 1;
		}

		// Token: 0x0600320B RID: 12811 RVA: 0x002CB030 File Offset: 0x002C9230
		private static int ReloadXmlFile_config_Lucky()
		{
			GameManager.systemLuckyMgr.LoadFromXMlFile("Config/Lucky.xml", "", "Number", 0);
			return 1;
		}

		// Token: 0x0600320C RID: 12812 RVA: 0x002CB060 File Offset: 0x002C9260
		private static int ReloadXmlFile_config_ChengJiu()
		{
			GameManager.systemChengJiu.LoadFromXMlFile("Config/ChengJiu.xml", "ChengJiu", "ChengJiuID", 0);
			ChengJiuManager.InitChengJiuConfig();
			return 1;
		}

		// Token: 0x0600320D RID: 12813 RVA: 0x002CB094 File Offset: 0x002C9294
		private static int ReloadXmlFile_config_MoJingAndQiFu()
		{
			GameManager.SystemExchangeMoJingAndQiFu.LoadFromXMlFile("Config/DuiHuanItems.xml", "Items", "ID", 1);
			GameManager.SystemExchangeType.LoadFromXMlFile("Config/DuiHuanType.xml", "DuiHuan", "ID", 1);
			return 1;
		}

		// Token: 0x0600320E RID: 12814 RVA: 0x002CB0E0 File Offset: 0x002C92E0
		private static int ReloadXmlFile_config_TotalLoginDataInfo()
		{
			Program.LoadTotalLoginDataInfo();
			return 1;
		}

		// Token: 0x0600320F RID: 12815 RVA: 0x002CB0FC File Offset: 0x002C92FC
		private static int ReloadXmlFile_config_ChengJiuBuff()
		{
			GameManager.systemChengJiuBuffer.LoadFromXMlFile("Config/ChengJiuBuff.xml", "", "ID", 0);
			return 1;
		}

		// Token: 0x06003210 RID: 12816 RVA: 0x002CB12C File Offset: 0x002C932C
		private static int ReloadXmlFile_config_JingMai()
		{
			return GameManager.SystemJingMaiLevel.ReloadLoadFromXMlFile();
		}

		// Token: 0x06003211 RID: 12817 RVA: 0x002CB14C File Offset: 0x002C934C
		private static int ReloadXmlFile_config_WuXue()
		{
			return GameManager.SystemWuXueLevel.ReloadLoadFromXMlFile();
		}

		// Token: 0x06003212 RID: 12818 RVA: 0x002CB16C File Offset: 0x002C936C
		private static int ReloadXmlFile_config_ZuanHuang()
		{
			return GameManager.SystemZuanHuangLevel.ReloadLoadFromXMlFile();
		}

		// Token: 0x06003213 RID: 12819 RVA: 0x002CB18C File Offset: 0x002C938C
		private static int ReloadXmlFile_config_SystemOpen()
		{
			return GameManager.SystemSystemOpen.ReloadLoadFromXMlFile();
		}

		// Token: 0x06003214 RID: 12820 RVA: 0x002CB1AC File Offset: 0x002C93AC
		private static int ReloadXmlFile_config_Vip()
		{
			Global.LoadVipLevelAwardList();
			return 1;
		}

		// Token: 0x06003215 RID: 12821 RVA: 0x002CB1C8 File Offset: 0x002C93C8
		private static int ReloadXmlFile_config_QiangGou()
		{
			return GameManager.SystemQiangGou.ReloadLoadFromXMlFile();
		}

		// Token: 0x06003216 RID: 12822 RVA: 0x002CB1E8 File Offset: 0x002C93E8
		public static int ReloadXmlFile_config_HeFuQiangGou()
		{
			return GameManager.SystemHeFuQiangGou.ReloadLoadFromXMlFile();
		}

		// Token: 0x06003217 RID: 12823 RVA: 0x002CB208 File Offset: 0x002C9408
		public static int ReloadXmlFile_config_JieRiQiangGou()
		{
			return GameManager.SystemJieRiQiangGou.ReloadLoadFromXMlFile();
		}

		// Token: 0x06003218 RID: 12824 RVA: 0x002CB228 File Offset: 0x002C9428
		private static int ReloadXmlFile_config_DailyActive()
		{
			GameManager.systemDailyActiveInfo.LoadFromXMlFile("Config/DailyActiveInfor.xml", "DailyActive", "DailyActiveID", 0);
			return 1;
		}

		// Token: 0x06003219 RID: 12825 RVA: 0x002CB258 File Offset: 0x002C9458
		private static int ReloadXmlFile_config_DailyActiveAward()
		{
			GameManager.systemDailyActiveAward.LoadFromXMlFile("Config/DailyActiveAward.xml", "DailyActiveAward", "ID", 0);
			return 1;
		}

		// Token: 0x0600321A RID: 12826 RVA: 0x002CB288 File Offset: 0x002C9488
		public static int ReloadXmlFile_config_gifts_JieRiType()
		{
			return HuodongCachingMgr.ResetJieriActivityConfig();
		}

		// Token: 0x0600321B RID: 12827 RVA: 0x002CB2A0 File Offset: 0x002C94A0
		public static int ReloadXmlFile_config_gifts_JieRiLiBao()
		{
			return HuodongCachingMgr.ResetJieriDaLiBaoActivity();
		}

		// Token: 0x0600321C RID: 12828 RVA: 0x002CB2B8 File Offset: 0x002C94B8
		public static int ReloadXmlFile_config_gifts_JieRiDengLu()
		{
			return HuodongCachingMgr.ResetJieRiDengLuActivity();
		}

		// Token: 0x0600321D RID: 12829 RVA: 0x002CB2D0 File Offset: 0x002C94D0
		public static int ReloadXmlFile_config_gifts_JieRiVip()
		{
			return HuodongCachingMgr.ResetJieriVIPActivity();
		}

		// Token: 0x0600321E RID: 12830 RVA: 0x002CB2E8 File Offset: 0x002C94E8
		public static int ReloadXmlFile_config_gifts_JieRiChongZhiSong()
		{
			return HuodongCachingMgr.ResetJieriCZSongActivity();
		}

		// Token: 0x0600321F RID: 12831 RVA: 0x002CB300 File Offset: 0x002C9500
		public static int ReloadXmlFile_config_gifts_JieRiLeiJi()
		{
			return HuodongCachingMgr.ResetJieRiLeiJiCZActivity();
		}

		// Token: 0x06003220 RID: 12832 RVA: 0x002CB318 File Offset: 0x002C9518
		public static int ReloadXmlFile_config_gifts_JieRiBaoXiang()
		{
			return HuodongCachingMgr.ResetJieRiZiKaLiaBaoActivity();
		}

		// Token: 0x06003221 RID: 12833 RVA: 0x002CB330 File Offset: 0x002C9530
		public static int ReloadXmlFile_config_gifts_JieRiXiaoFeiKing()
		{
			return HuodongCachingMgr.ResetJieRiXiaoFeiKingActivity();
		}

		// Token: 0x06003222 RID: 12834 RVA: 0x002CB348 File Offset: 0x002C9548
		public static int ReloadXmlFile_config_gifts_JieRiChongZhiKing()
		{
			return HuodongCachingMgr.ResetJieRiCZKingActivity();
		}

		// Token: 0x06003223 RID: 12835 RVA: 0x002CB360 File Offset: 0x002C9560
		public static int ReloadXmlFile_config_gifts_JieRiTotalConsume()
		{
			return HuodongCachingMgr.ResetJieRiTotalConsumeActivity();
		}

		// Token: 0x06003224 RID: 12836 RVA: 0x002CB378 File Offset: 0x002C9578
		public static int ReloadXmlFile_config_gifts_JieRiMultAward()
		{
			return HuodongCachingMgr.ResetJieRiMultAwardActivity();
		}
	}
}
