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
	
	public class ReloadXmlManager
	{
		
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

		
		private static int ReloadXmlFile_config_gifts_activities()
		{
			Global._activitiesData = null;
			return HuodongCachingMgr.ResetSongLiItem();
		}

		
		private static int ReloadXmlFile_config_gifts_biggift()
		{
			return HuodongCachingMgr.ResetBigAwardItem();
		}

		
		private static int ReloadXmlFile_config_gifts_loginnumgift()
		{
			return HuodongCachingMgr.ResetWLoginItem();
		}

		
		public static int ReloadXmlFile_config_gifts_huodongloginnumgift()
		{
			return HuodongCachingMgr.ResetLimitTimeLoginItem();
		}

		
		private static int ReloadXmlFile_config_gifts_newrolegift()
		{
			return HuodongCachingMgr.ResetNewStepItem();
		}

		
		private static int ReloadXmlFile_config_combat_effectiveness_gift()
		{
			return HuodongCachingMgr.ResetCombatAwardItem();
		}

		
		private static int ReloadXmlFile_config_gifts_uplevelgift()
		{
			return HuodongCachingMgr.ResetUpLevelItem();
		}

		
		private static int ReloadXmlFile_config_gifts_onlietimegift()
		{
			return HuodongCachingMgr.ResetMOnlineTimeItem();
		}

		
		private static int ReloadXmlFile_config_platconfig()
		{
			return GameManager.PlatConfigMgr.ReloadPlatConfig();
		}

		
		private static int ReloadXmlFile_config_mall()
		{
			MallPriceMgr.ClearCache();
			Global._MallSaleData = null;
			return GameManager.systemMallMgr.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_monstergoodslist()
		{
			return GameManager.GoodsPackMgr.ResetCachingItems();
		}

		
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

		
		private static int ReloadXmlFile_config_specialtimes()
		{
			return SpecailTimeManager.ResetSpecialTimeLimits();
		}

		
		private static int ReloadXmlFile_config_battle()
		{
			int ret = GameManager.SystemBattle.ReloadLoadFromXMlFile();
			GameManager.BattleMgr.LoadParams();
			return ret;
		}

		
		private static int ReloadXmlFile_config_ArenaBattle()
		{
			int ret = GameManager.SystemArenaBattle.ReloadLoadFromXMlFile();
			GameManager.ArenaBattleMgr.LoadParams();
			return ret;
		}

		
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

		
		private static int ReloadXmlFile_config_systemoperations()
		{
			int ret = GameManager.SystemOperasMgr.ReloadLoadFromXMlFile();
			Global.ClearNPCOperationTimeLimits();
			return ret;
		}

		
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

		
		private static int ReloadXmlFile_config_goodsmergeitems()
		{
			return MergeNewGoods.ReloadCacheMergeItems();
		}

		
		private static int ReloadXmlFile_config_qizhengegoods()
		{
			int ret = GameManager.systemQiZhenGeGoodsMgr.ReloadLoadFromXMlFile();
			QiZhenGeManager.ClearQiZhenGeCachingItems();
			return ret;
		}

		
		private static int ReloadXmlFile_config_npcsalelist()
		{
			return GameManager.NPCSaleListMgr.ReloadSaleList() ? 0 : -1;
		}

		
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

		
		private static int ReloadXmlFile_config_systemtasks()
		{
			int ret = GameManager.SystemTasksMgr.ReloadLoadFromXMlFile();
			ProcessTask.InitBranchTasks(GameManager.SystemTasksMgr.SystemXmlItemDict);
			GameManager.NPCTasksMgr.LoadNPCTasks(GameManager.SystemTasksMgr);
			GameManager.TaskAwardsMgr.ClearAllDictionary();
			return ret;
		}

		
		public static int ReloadXmlFile_config_taskzhangjie()
		{
			int ret = GameManager.TaskZhangJie.ReloadLoadFromXMlFile();
			if (ret >= 0)
			{
				ReloadXmlManager.InitTaskZhangJieInfo();
			}
			return ret;
		}

		
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

		
		private static int ReloadXmlFile_config_equipupgrade()
		{
			EquipUpgradeCacheMgr.LoadEquipUpgradeItems();
			return 0;
		}

		
		private static int ReloadXmlFile_config_dig()
		{
			return GameManager.systemWaBaoMgr.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_battleexp()
		{
			int ret = GameManager.systemBattleExpMgr.ReloadLoadFromXMlFile();
			GameManager.BattleMgr.ClearBattleExpByLevels();
			return ret;
		}

		
		private static int ReloadXmlFile_config_bangzhanawards()
		{
			int ret = GameManager.systemBangZhanAwardsMgr.ReloadLoadFromXMlFile();
			BangZhanAwardsMgr.ClearAwardsByLevels();
			return ret;
		}

		
		private static int ReloadXmlFile_config_rebirth()
		{
			return GameManager.systemBattleRebirthMgr.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_Award()
		{
			int ret = GameManager.systemBattleAwardMgr.ReloadLoadFromXMlFile();
			GameManager.BattleMgr.ClearBattleAwardByScore();
			return ret;
		}

		
		private static int ReloadXmlFile_config_EquipBorn()
		{
			return GameManager.systemEquipBornMgr.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_BornName()
		{
			return GameManager.systemBornNameMgr.ReloadLoadFromXMlFile();
		}

		
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

		
		public static int ReloadXmlFile_config_gifts_FanLi()
		{
			return HuodongCachingMgr.ResetInputFanLiActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_ChongZhiSong()
		{
			return HuodongCachingMgr.ResetInputSongActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_ChongZhiKing()
		{
			return HuodongCachingMgr.ResetInputKingActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_LevelKing()
		{
			return HuodongCachingMgr.ResetLevelKingActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_EquipKing()
		{
			return HuodongCachingMgr.ResetEquipKingActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_HorseKing()
		{
			return HuodongCachingMgr.ResetHorseKingActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JingMaiKing()
		{
			return HuodongCachingMgr.ResetJingMaiKingActivity();
		}

		
		private static int ReloadXmlFile_config_gifts_VipDailyAwards()
		{
			GameManager.systemVipDailyAwardsMgr.LoadFromXMlFile("Config/Gifts/VipDailyAwards.xml", "", "AwardID", 1);
			return 1;
		}

		
		private static int ReloadXmlFile_config_ActivityTip()
		{
			GameManager.systemActivityTipMgr.LoadFromXMlFile("Config/Activity/ActivityTip.xml", "", "ID", 0);
			return 1;
		}

		
		private static int ReloadXmlFile_config_LuckyAward()
		{
			GameManager.systemLuckyAwardMgr.LoadFromXMlFile("Config/LuckyAward.xml", "", "ID", 0);
			GameManager.systemLuckyAward2Mgr.LoadFromXMlFile("Config/LuckyAward2.xml", "", "ID", 0);
			return 1;
		}

		
		private static int ReloadXmlFile_config_Lucky()
		{
			GameManager.systemLuckyMgr.LoadFromXMlFile("Config/Lucky.xml", "", "Number", 0);
			return 1;
		}

		
		private static int ReloadXmlFile_config_ChengJiu()
		{
			GameManager.systemChengJiu.LoadFromXMlFile("Config/ChengJiu.xml", "ChengJiu", "ChengJiuID", 0);
			ChengJiuManager.InitChengJiuConfig();
			return 1;
		}

		
		private static int ReloadXmlFile_config_MoJingAndQiFu()
		{
			GameManager.SystemExchangeMoJingAndQiFu.LoadFromXMlFile("Config/DuiHuanItems.xml", "Items", "ID", 1);
			GameManager.SystemExchangeType.LoadFromXMlFile("Config/DuiHuanType.xml", "DuiHuan", "ID", 1);
			return 1;
		}

		
		private static int ReloadXmlFile_config_TotalLoginDataInfo()
		{
			Program.LoadTotalLoginDataInfo();
			return 1;
		}

		
		private static int ReloadXmlFile_config_ChengJiuBuff()
		{
			GameManager.systemChengJiuBuffer.LoadFromXMlFile("Config/ChengJiuBuff.xml", "", "ID", 0);
			return 1;
		}

		
		private static int ReloadXmlFile_config_JingMai()
		{
			return GameManager.SystemJingMaiLevel.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_WuXue()
		{
			return GameManager.SystemWuXueLevel.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_ZuanHuang()
		{
			return GameManager.SystemZuanHuangLevel.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_SystemOpen()
		{
			return GameManager.SystemSystemOpen.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_Vip()
		{
			Global.LoadVipLevelAwardList();
			return 1;
		}

		
		private static int ReloadXmlFile_config_QiangGou()
		{
			return GameManager.SystemQiangGou.ReloadLoadFromXMlFile();
		}

		
		public static int ReloadXmlFile_config_HeFuQiangGou()
		{
			return GameManager.SystemHeFuQiangGou.ReloadLoadFromXMlFile();
		}

		
		public static int ReloadXmlFile_config_JieRiQiangGou()
		{
			return GameManager.SystemJieRiQiangGou.ReloadLoadFromXMlFile();
		}

		
		private static int ReloadXmlFile_config_DailyActive()
		{
			GameManager.systemDailyActiveInfo.LoadFromXMlFile("Config/DailyActiveInfor.xml", "DailyActive", "DailyActiveID", 0);
			return 1;
		}

		
		private static int ReloadXmlFile_config_DailyActiveAward()
		{
			GameManager.systemDailyActiveAward.LoadFromXMlFile("Config/DailyActiveAward.xml", "DailyActiveAward", "ID", 0);
			return 1;
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiType()
		{
			return HuodongCachingMgr.ResetJieriActivityConfig();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiLiBao()
		{
			return HuodongCachingMgr.ResetJieriDaLiBaoActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiDengLu()
		{
			return HuodongCachingMgr.ResetJieRiDengLuActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiVip()
		{
			return HuodongCachingMgr.ResetJieriVIPActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiChongZhiSong()
		{
			return HuodongCachingMgr.ResetJieriCZSongActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiLeiJi()
		{
			return HuodongCachingMgr.ResetJieRiLeiJiCZActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiBaoXiang()
		{
			return HuodongCachingMgr.ResetJieRiZiKaLiaBaoActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiXiaoFeiKing()
		{
			return HuodongCachingMgr.ResetJieRiXiaoFeiKingActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiChongZhiKing()
		{
			return HuodongCachingMgr.ResetJieRiCZKingActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiTotalConsume()
		{
			return HuodongCachingMgr.ResetJieRiTotalConsumeActivity();
		}

		
		public static int ReloadXmlFile_config_gifts_JieRiMultAward()
		{
			return HuodongCachingMgr.ResetJieRiMultAwardActivity();
		}
	}
}
