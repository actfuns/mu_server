using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public static class Data
	{
		
		public static int GetTotalLoginInfoNum()
		{
			int count;
			lock (Data.TotalLoginDataInfoListLock)
			{
				count = Data.TotalLoginDataInfoList.Count;
			}
			return count;
		}

		
		public static TotalLoginDataInfo GetTotalLoginDataInfo(int nIdnex)
		{
			lock (Data.TotalLoginDataInfoListLock)
			{
				if (Data.TotalLoginDataInfoList.ContainsKey(nIdnex))
				{
					return Data.TotalLoginDataInfoList[nIdnex];
				}
			}
			return null;
		}

		
		
		
		public static SingleChargeData ChargeData
		{
			get
			{
				SingleChargeData chargeData;
				lock (Data.SingleChargeDataMutex)
				{
					chargeData = Data._ChargeData;
				}
				return chargeData;
			}
			set
			{
				lock (Data.SingleChargeDataMutex)
				{
					Data._ChargeData = value;
				}
			}
		}

		
		
		
		public static Dictionary<int, ChargeItemData> ChargeItemDict
		{
			get
			{
				Dictionary<int, ChargeItemData> chargeItemDict;
				lock (Data.ChargeItemDataMutex)
				{
					chargeItemDict = Data._ChargeItemDict;
				}
				return chargeItemDict;
			}
			set
			{
				lock (Data.ChargeItemDataMutex)
				{
					Data._ChargeItemDict = value;
				}
			}
		}

		
		
		public static List<ChannelName> ChannelNameConfigList
		{
			get
			{
				return Data._ChannelNameConfigList.Value;
			}
		}

		
		
		public static List<LianZhanConfig> LianZhanConfigList
		{
			get
			{
				return Data._LianZhanConfigList.Value;
			}
		}

		
		public static void LoadConfig()
		{
			try
			{
				double[] args0 = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhanDouLiAlertLog", ',');
				if (args0 != null && args0.Length == 2 && args0[0] >= 100000.0 && args0[1] >= 0.1)
				{
					Data.CombatForceLogMinValue = (long)args0[0];
					Data.CombatForceLogPercent = args0[1];
				}
				Data.LoadUseridWhiteList();
				Data.LoadSystemParamsValues();
				Data.SettingsDict.Load(Global.GameResPath("Config/Settings.xml"), "Maps");
				int iWithRname = (int)GameManager.systemParamsList.GetParamValueIntByName("LogWithRname", -1);
				Global.WithRname = (iWithRname == 1);
				Data.ZhuTiID = GameManager.GameConfigMgr.GetGameConfigItemInt("zhuti", 0);
				Data.FightStateTime = (long)((int)GameManager.systemParamsList.GetParamValueIntByName("FightStateTime", 6000));
				Data.CheckTimeBoost = (GameManager.systemParamsList.GetParamValueIntByName("CheckTimeBoost", 1) > 0L);
				Data.CheckPositionCheat = ((GameManager.systemParamsList.GetParamValueIntByName("CheckPositionCheat", 1) & 1L) != 0L);
				Data.CheckPositionCheatSpeed = ((GameManager.systemParamsList.GetParamValueIntByName("CheckPositionCheat", 1) & 2L) != 0L);
				Data.IgnoreClientPos = ((GameManager.systemParamsList.GetParamValueIntByName("CheckPositionCheat", 1) & 4L) != 0L);
				LogManager.DisableLogTypes(GameManager.systemParamsList.GetParamValueIntArrayByName("DisableLogTypes", ','));
				Data.SyncTimeByClient = GameManager.systemParamsList.GetParamValueIntByName("SyncTimeByClient", 750);
				Data.MaxServerClientTimeDiff = GameManager.systemParamsList.GetParamValueIntByName("MaxServerClientTimeDiff", 300);
				Data.RoleOccupationMaxCount = (int)GameManager.systemParamsList.GetParamValueIntByName("PurchaseOccupationNum", 1);
				Data.OpChangeLifeCount = (int)GameManager.systemParamsList.GetParamValueIntByName("OpChangeLifeCount", 100);
				Data.NotifyLiXianAwardMin = (int)GameManager.systemParamsList.GetParamValueIntByName("OfflineRW_Auto", 1440);
				Data.OfflineRW_ItemLimit = (int)GameManager.systemParamsList.GetParamValueIntByName("OfflineRW_ItemLimit", 30);
				Data.OpenData.paimaihangjinbi = (int)GameManager.systemParamsList.GetParamValueIntByName("paimaihangjinbi", 1);
				Data.OpenData.paimaihangzuanshi = (int)GameManager.systemParamsList.GetParamValueIntByName("paimaihangzuanshi", 1);
				Data.OpenData.paimaihangmobi = (int)GameManager.systemParamsList.GetParamValueIntByName("paimaihangmobi", 0);
				Data.OpenData.bangzuan = (int)GameManager.systemParamsList.GetParamValueIntByName("bangzuan", 1);
				Data.OpenData.zuanshi = (int)GameManager.systemParamsList.GetParamValueIntByName("zuanshi", 1);
				Data.OpenData.mobi = (int)GameManager.systemParamsList.GetParamValueIntByName("mobi", 0);
				Data.OpenData.paimaijiemianmobi = (int)GameManager.systemParamsList.GetParamValueIntByName("paimaijiemianmobi", 0);
				Data.LuoLanKingGongGaoCD = (long)((int)GameManager.systemParamsList.GetParamValueIntByName("LuoLanKingGongGaoCD", 120));
				int plat = (int)GameCoreInterface.getinstance().GetPlatformType();
				List<string> p0 = GameManager.systemParamsList.GetParamValueStringListByName("LogLifeRecoverOpen", '|');
				if (p0 != null)
				{
					ClientCmdCheck.MinLogAddLifeV = 2147483647L;
					ClientCmdCheck.MinLogAddLifePercent = 100L;
					ClientCmdCheck.MapCodes.Clear();
					foreach (string p in p0)
					{
						int[] p2 = Global.String2IntArray(p, ',');
						lock (ClientCmdCheck.MapCodes)
						{
							if (p2[0] == plat && p2[1] > 0)
							{
								double[] p3 = GameManager.systemParamsList.GetParamValueDoubleArrayByName("LogLifeRecoverNum", ',');
								int[] p4 = GameManager.systemParamsList.GetParamValueIntArrayByName("LogLifeRecoverMap", ',');
								if (p3 != null && p3.Length >= 2)
								{
									ClientCmdCheck.MinLogAddLifeV = (long)((int)Math.Max(p3[0], 10000.0));
									ClientCmdCheck.MinLogAddLifePercent = (long)((int)Math.Max(p3[1] * 100.0, 16.0));
								}
								if (p4 != null && p4.Length >= 1)
								{
									foreach (int mapCode in p4)
									{
										ClientCmdCheck.MapCodes.Add(mapCode);
									}
								}
								break;
							}
						}
					}
				}
				Data.LoadEquipDelay = (GameManager.systemParamsList.GetParamValueIntByName("LoadEquipDelay", 1) != 0L);
				Data.LoadExtPropThreshold();
				GoodsUtil.LoadConfig();
				Data.LoadChannelNameConfig();
				Data.LoadKuaFuWorldCmds();
				Data.LoadLianZhanConfig();
				Data.LoadMapOptimizeFlags();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			GameManager.OnceDestroyCopyMapNum = (int)GameManager.systemParamsList.GetParamValueIntByName("OnceDestroyCopyMapNum", 100);
		}

		
		public static void LoadMapOptimizeFlags()
		{
			string mapCodes = GameManager.systemParamsList.GetParamValueByName("MapOptimizeFlags");
			if (null == mapCodes)
			{
				mapCodes = "96000";
			}
			int[] arr = ConfigHelper.String2IntArray(mapCodes, ',');
			foreach (MapGrid mapGrid in GameManager.MapGridMgr.DictGrids.Values)
			{
				mapGrid.FlagOptimizeFindObjects = arr.Contains(mapGrid._GameMap.MapCode);
			}
		}

		
		public static void LoadExtPropThreshold()
		{
			string xmlFileName = Global.GameResPath("Config/ExtPropThreshold.xml");
			try
			{
				Data.ExtPropThreshold[24] = new Tuple<double, double>(0.0, 0.8);
				XElement xmlFile = ConfigHelper.Load(xmlFileName);
				if (null != xmlFile)
				{
					IEnumerable<XElement> xmls = ConfigHelper.GetXElements(xmlFile, "ExtPropThreshold");
					if (null != xmls)
					{
						foreach (XElement xml in xmls)
						{
							int expPropId = (int)ConfigHelper.GetElementAttributeValueLong(xml, "ExtPropID1", 0L);
							double min = ConfigHelper.GetElementAttributeValueDouble(xml, "Min", 0.0);
							double max = ConfigHelper.GetElementAttributeValueDouble(xml, "Max", 0.8);
							if (expPropId > 0 && expPropId < 177)
							{
								Data.ExtPropThreshold[expPropId] = new Tuple<double, double>(min, max);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void LoadUseridWhiteList()
		{
			string xmlFileName = Global.IsolateResPath("Config/LoginUserWhiteList.xml");
			try
			{
				Data.readWriteLock.EnterWriteLock();
				Data.UseridWhiteList.Clear();
				XElement xmlFile = ConfigHelper.Load(xmlFileName);
				if (null != xmlFile)
				{
					IEnumerable<XElement> xmls = ConfigHelper.GetXElements(xmlFile, "WhiteList");
					if (null != xmls)
					{
						foreach (XElement xml in xmls)
						{
							string platform = ConfigHelper.GetElementAttributeValue(xml, "PinTai", "");
							if (0 == string.Compare(platform, GameCoreInterface.getinstance().GetPlatformType().ToString(), true))
							{
								string userId = ConfigHelper.GetElementAttributeValue(xml, "UserID", "");
								Data.UseridWhiteList.Add(userId.ToLower());
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			finally
			{
				Data.readWriteLock.ExitWriteLock();
			}
		}

		
		public static bool? InUserWriteList(string userId)
		{
			try
			{
				Data.readWriteLock.EnterReadLock();
				if (Data.UseridWhiteList.Count > 0)
				{
					return new bool?(Data.UseridWhiteList.Contains(userId.ToLower()));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
				return new bool?(false);
			}
			finally
			{
				Data.readWriteLock.ExitReadLock();
			}
			return null;
		}

		
		private static void LoadSystemParamsValues()
		{
			try
			{
				Data.readWriteLock.EnterWriteLock();
				Data.DaTianShiGoodsIdList.Clear();
				int[] argsInt = GameManager.systemParamsList.GetParamValueIntArrayByName("DaTianShi", ',');
				if (argsInt != null && argsInt.Length > 0)
				{
					foreach (int id in argsInt)
					{
						Data.DaTianShiGoodsIdList.Add(id);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			finally
			{
				Data.readWriteLock.ExitWriteLock();
			}
		}

		
		public static bool IsDaTianShiGoods(int goodsId)
		{
			bool result;
			try
			{
				Data.readWriteLock.EnterReadLock();
				result = Data.DaTianShiGoodsIdList.Contains(goodsId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
				result = false;
			}
			finally
			{
				Data.readWriteLock.ExitReadLock();
			}
			return result;
		}

		
		private static void LoadChannelNameConfig()
		{
			try
			{
				string fileName = Global.GameResPath("Config/ChannelName.xml");
				Data._ChannelNameConfigList.Load(fileName, null);
				if (null != Data.ChannelNameConfigList)
				{
					Data.ChannelNameConfigList.Sort((ChannelName x, ChannelName y) => StringComparer.Ordinal.Compare(y.Channel, x.Channel));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public static string GetChannelNameByUserID(string userid)
		{
			string channelName = "IOS";
			List<ChannelName> list = Data.ChannelNameConfigList;
			if (null != list)
			{
				foreach (ChannelName item in list)
				{
					if (userid.StartsWith(item.Channel))
					{
						channelName = item.Name;
						break;
					}
				}
			}
			return channelName;
		}

		
		public static int GetUserPtIDByUserID(string userid)
		{
			PlatformTypes userPtID = PlatformTypes.APP;
			List<ChannelName> list = Data.ChannelNameConfigList;
			if (null != list)
			{
				foreach (ChannelName item in list)
				{
					if (userid.StartsWith(item.Channel))
					{
						userPtID = (PlatformTypes)item.PTID;
						break;
					}
				}
			}
			return (int)userPtID;
		}

		
		public static string GetPtNameByPtID(int ptId)
		{
			string ptName = "";
			List<ChannelName> list = Data.ChannelNameConfigList;
			if (null != list)
			{
				ChannelName findItem = list.Find((ChannelName x) => x.PTID == ptId);
				if (null != findItem)
				{
					ptName = findItem.PTName;
				}
			}
			return ptName;
		}

		
		private static void LoadLianZhanConfig()
		{
			try
			{
				Data.LianZhanTimes = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("RebornLianZhan"), true, '|', ',');
				Data.LianZhanMaps = GameManager.systemParamsList.GetParamValueIntArrayByName("RebornLianZhanMap", '|');
				string fileName = Global.GameResPath("Config/RebornLianZhan.xml");
				Data._LianZhanConfigList.Load(fileName, null);
				if (Data._LianZhanConfigList.Value.Count > 0)
				{
					Data.MinLianZhanNum = Data.LianZhanConfigList.Min((LianZhanConfig x) => x.Num);
					Data.MaxLianZhanNum = Data.LianZhanConfigList.Max((LianZhanConfig x) => x.Num);
					Data.MinLianZhanNum = MathEx.GCD(Data.MinLianZhanNum, Data.MaxLianZhanNum);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public static bool IsLianZhanMap(int mapCode)
		{
			int[] arr = Data.LianZhanMaps;
			if (null != arr)
			{
				for (int i = 0; i < arr.Length; i++)
				{
					if (arr[i] == mapCode)
					{
						return true;
					}
				}
			}
			return false;
		}

		
		public static int LianZhanContinueTime(int num)
		{
			int continueTime = 5;
			List<List<int>> arr = Data.LianZhanTimes;
			int result;
			if (arr == null || arr.Count == 0)
			{
				result = continueTime;
			}
			else
			{
				continueTime = arr[arr.Count - 1][1];
				for (int i = 0; i < arr.Count; i++)
				{
					if (num <= arr[i][0])
					{
						continueTime = arr[i][1];
						break;
					}
				}
				result = continueTime;
			}
			return result;
		}

		
		public static MapSettingItem GetMapSettingInfo(int mapCode)
		{
			Dictionary<int, MapSettingItem> dict = Data.SettingsDict.Value;
			MapSettingItem info;
			if (dict == null || !dict.TryGetValue(mapCode, out info))
			{
				info = new MapSettingItem();
			}
			return info;
		}

		
		public static void ClearMiniBufferDataIds()
		{
			lock (Data.MiniBufferDataIds)
			{
				List<KeyValuePair<int, int>> list = Data.MiniBufferDataIds.ToList<KeyValuePair<int, int>>();
				foreach (KeyValuePair<int, int> kv in list)
				{
					if (kv.Value != 0)
					{
						Data.MiniBufferDataIds.Remove(kv.Key);
					}
				}
			}
		}

		
		public static void AddMiniBufferDataIds(params int[] args)
		{
			lock (Data.MiniBufferDataIds)
			{
				foreach (int id in args)
				{
					if (!Data.MiniBufferDataIds.ContainsKey(id))
					{
						Data.MiniBufferDataIds[id] = 1;
					}
				}
			}
		}

		
		public static bool IsMiniBufferDataId(int id)
		{
			bool result;
			lock (Data.MiniBufferDataIds)
			{
				result = Data.MiniBufferDataIds.ContainsKey(id);
			}
			return result;
		}

		
		public static bool KuaFuWorldCmdEnabled(int cmd)
		{
			HashSet<int> hashset = Data.KuaFuWorldCmds;
			return hashset == null || !hashset.Contains(cmd);
		}

		
		private static void LoadKuaFuWorldCmds()
		{
			HashSet<int> cmds = new HashSet<int>(Data.DisabledKuaFuWorldCmds);
			try
			{
				string fileName = Global.GameResPath("Config/KuaFuWorldCmds.xml");
				XElement xmlFile = ConfigHelper.Load(fileName);
				if (null != xmlFile)
				{
					foreach (XElement xml in ConfigHelper.GetXElements(xmlFile, "KuaFuWorldCmd"))
					{
						int cmdID = (int)ConfigHelper.GetElementAttributeValueLong(xml, "ID", 0L);
						int enable = (int)ConfigHelper.GetElementAttributeValueLong(xml, "Enable", 0L);
						if (cmdID > 0)
						{
							if (enable > 0)
							{
								cmds.Add(cmdID);
							}
							else
							{
								cmds.Remove(cmdID);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			finally
			{
				Data.KuaFuWorldCmds = cmds;
			}
		}

		
		public const int ConstSubPKPointPerMin = 10;

		
		public const string GAME_CONFIG_USERWHITELIST_FILE = "Config/LoginUserWhiteList.xml";

		
		public const int RoleMaxLevel = 100;

		
		public const int GlobalData = 0;

		
		public static int WalkUnitCost;

		
		public static int RunUnitCost;

		
		public static int[] SpeedTickList;

		
		public static int WalkStepWidth;

		
		public static int RunStepWidth;

		
		public static int MaxAttackDistance;

		
		public static int MinAttackDistance;

		
		public static int MaxMagicDistance;

		
		public static int MaxAttackSlotTick;

		
		public static int LifeTotalWidth;

		
		public static int HoldWidth;

		
		public static int HoldHeight;

		
		public static int GoodsPackOvertimeTick = 90;

		
		public static int PackDestroyTimeTick = 90;

		
		public static int TaskMaxFocusCount = 400;

		
		public static int AliveGoodsID = -1;

		
		public static int AliveMaxLevel = 10;

		
		public static int AutoGetThing = 0;

		
		public static long[] LevelUpExperienceList = null;

		
		public static RoleSitExpItem[] RoleSitExpList = null;

		
		public static List<RoleBasePropItem[]> RoleBasePropList = new List<RoleBasePropItem[]>();

		
		public static List<MapStallItem> MapStallList = new List<MapStallItem>(10);

		
		public static Dictionary<int, string> MapNamesDict = new Dictionary<int, string>(100);

		
		public static Dictionary<int, ChangeOccupInfo> ChangeOccupInfoList = new Dictionary<int, ChangeOccupInfo>();

		
		public static Dictionary<int, double> ChangeLifeEverydayExpRate = new Dictionary<int, double>();

		
		public static Dictionary<int, OccupationAddPointInfo> OccupationAddPointInfoList = new Dictionary<int, OccupationAddPointInfo>();

		
		public static Dictionary<int, ChangeLifeAddPointInfo> ChangeLifeAddPointInfoList = new Dictionary<int, ChangeLifeAddPointInfo>();

		
		public static Dictionary<int, BloodCastleDataInfo> BloodCastleDataInfoList = new Dictionary<int, BloodCastleDataInfo>();

		
		public static Dictionary<int, MoBaiData> MoBaiDataInfoList = new Dictionary<int, MoBaiData>();

		
		public static Dictionary<int, List<CopyScoreDataInfo>> CopyScoreDataInfoList = new Dictionary<int, List<CopyScoreDataInfo>>();

		
		public static FreshPlayerCopySceneInfo FreshPlayerSceneInfo = new FreshPlayerCopySceneInfo();

		
		public static List<TaskStarDataInfo> TaskStarInfo = new List<TaskStarDataInfo>();

		
		public static List<DailyCircleTaskAwardInfo> DailyCircleTaskAward = new List<DailyCircleTaskAwardInfo>();

		
		public static TaofaTaskAwardInfo TaofaTaskExAward = new TaofaTaskAwardInfo();

		
		public static Dictionary<int, CombatForceInfo> CombatForceDataInfo = new Dictionary<int, CombatForceInfo>();

		
		public static Dictionary<int, DaimonSquareDataInfo> DaimonSquareDataInfoList = new Dictionary<int, DaimonSquareDataInfo>();

		
		public static double[] WingForgeLevelAddShangHaiJiaCheng = null;

		
		public static double[] WingForgeLevelAddDefenseRates = null;

		
		public static double[] WingForgeLevelAddShangHaiXiShou = null;

		
		public static double[] WingZhuiJiaLevelAddDefenseRates = null;

		
		public static double[] ForgeLevelAddAttackRates = null;

		
		public static double[] ForgeLevelAddDefenseRates = null;

		
		public static double[] ZhuiJiaLevelAddAttackRates = null;

		
		public static double[] ZhuiJiaLevelAddDefenseRates = null;

		
		public static double[] ForgeLevelAddMaxLifeVRates = null;

		
		public static double[] ZhuoYueAddAttackRates = null;

		
		public static double[] ZhuoYueAddDefenseRates = null;

		
		public static double[] RebornZhuoYueAddRates = null;

		
		public static int[] ForgeProtectStoneGoodsID = null;

		
		public static int[] ForgeProtectStoneGoodsNum = null;

		
		public static int DiamondToVipExpValue = 0;

		
		public static double[] RedNameDebuffInfo = null;

		
		public static string[] ForgeNeedGoodsID = new string[21];

		
		public static string[] ForgeNeedGoodsNum = new string[21];

		
		public static Dictionary<int, int> MapTransNeedMoneyDict = new Dictionary<int, int>();

		
		public static double[] EquipChangeLifeAddAttackRates = null;

		
		public static double[] EquipChangeLifeAddDefenseRates = null;

		
		public static int[] KillBossCountForChengJiu = null;

		
		public static int InsertAwardtPortableBagTaskID = 0;

		
		public static string InsertAwardtPortableBagGoodsInfo = null;

		
		public static int PaihangbangAdration = 0;

		
		public static int[] StoryCopyMapID = null;

		
		public static int FreeImpetrateIntervalTime = 0;

		
		public static Dictionary<int, TotalLoginDataInfo> TotalLoginDataInfoList = new Dictionary<int, TotalLoginDataInfo>();

		
		public static object TotalLoginDataInfoListLock = new object();

		
		public static Dictionary<int, VIPDataInfo> VIPDataInfoList = new Dictionary<int, VIPDataInfo>();

		
		public static Dictionary<int, VIPLevAwardAndExpInfo> VIPLevAwardAndExpInfoList = new Dictionary<int, VIPLevAwardAndExpInfo>();

		
		public static Dictionary<int, MeditateData> MeditateInfoList = new Dictionary<int, MeditateData>();

		
		public static Dictionary<int, ExperienceCopyMapDataInfo> ExperienceCopyMapDataInfoList = new Dictionary<int, ExperienceCopyMapDataInfo>();

		
		public static PKKingAdrationData PKkingadrationData = new PKKingAdrationData();

		
		public static PKKingAdrationData LLCZadrationData = new PKKingAdrationData();

		
		public static BossHomeData BosshomeData = new BossHomeData();

		
		public static GoldTempleData GoldtempleData = new GoldTempleData();

		
		public static Dictionary<int, PictureJudgeData> PicturejudgeData = new Dictionary<int, PictureJudgeData>();

		
		public static Dictionary<int, PictureJudgeTypeData> PicturejudgeTypeData = new Dictionary<int, PictureJudgeTypeData>();

		
		public static Dictionary<int, Dictionary<int, MuEquipUpgradeData>> EquipUpgradeData = new Dictionary<int, Dictionary<int, MuEquipUpgradeData>>();

		
		public static GoldCopySceneData Goldcopyscenedata = new GoldCopySceneData();

		
		public static Dictionary<int, EquipJuHunXmlData> EquipJuHunDataDict = new Dictionary<int, EquipJuHunXmlData>();

		
		public static Dictionary<int, BagTypeXmlData> BagTypeDict = new Dictionary<int, BagTypeXmlData>();

		
		private static SingleChargeData _ChargeData = null;

		
		private static object SingleChargeDataMutex = new object();

		
		private static object ChargeItemDataMutex = new object();

		
		public static Dictionary<int, ChargeItemData> _ChargeItemDict = null;

		
		public static Dictionary<int, int> LingYuMaterialZuanshiDict = new Dictionary<int, int>();

		
		public static Dictionary<int, int> FuBenNeedDict = new Dictionary<int, int>();

		
		public static long CombatForceLogMinValue = 1800000L;

		
		public static double CombatForceLogPercent = 0.25;

		
		private static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();

		
		public static HashSet<string> UseridWhiteList = new HashSet<string>();

		
		public static HashSet<int> DaTianShiGoodsIdList = new HashSet<int>();

		
		public static HashSet<int> CanTeleportMapHashSet = new HashSet<int>
		{
			5,
			6,
			7,
			12,
			13,
			24,
			21,
			20,
			14,
			15,
			25,
			26,
			27,
			31,
			35,
			36,
			38,
			39,
			40,
			43,
			50,
			51
		};

		
		public static long FightStateTime = 6000L;

		
		public static int NotifyLiXianAwardMin = 0;

		
		public static int OfflineRW_ItemLimit = 0;

		
		public static bool CheckTimeBoost = true;

		
		public static bool CheckPositionCheat = true;

		
		public static bool CheckPositionCheatSpeed = true;

		
		public static long SyncTimeByClient = 0L;

		
		public static bool IgnoreClientPos = false;

		
		public static long MaxServerClientTimeDiff;

		
		public static int RoleOccupationMaxCount = 2;

		
		public static int OpChangeLifeCount = 100;

		
		public static SystemOpenData OpenData = new SystemOpenData();

		
		public static int ZhuTiID;

		
		public static int ThemeActivityState;

		
		public static TemplateLoader<Dictionary<int, MapSettingItem>> SettingsDict = new TemplateLoader<Dictionary<int, MapSettingItem>>();

		
		public static int MinLianZhanNum;

		
		public static int MaxLianZhanNum;

		
		private static TemplateLoader<List<ChannelName>> _ChannelNameConfigList = new TemplateLoader<List<ChannelName>>();

		
		private static TemplateLoader<List<LianZhanConfig>> _LianZhanConfigList = new TemplateLoader<List<LianZhanConfig>>();

		
		public static List<List<int>> LianZhanTimes = null;

		
		public static int[] LianZhanMaps = null;

		
		private static HashSet<int> KuaFuWorldCmds = new HashSet<int>();

		
		public static bool LoadEquipDelay = true;

		
		public static long LuoLanKingGongGaoCD = 120L;

		
		public static LongCollection NextBroadCastTickDict = new LongCollection();

		
		private static Dictionary<int, int> MiniBufferDataIds = new Dictionary<int, int>
		{
			{
				81,
				0
			},
			{
				83,
				0
			},
			{
				84,
				0
			},
			{
				102,
				0
			},
			{
				39,
				0
			},
			{
				103,
				0
			},
			{
				111,
				0
			},
			{
				101,
				0
			},
			{
				2080011,
				0
			},
			{
				2080010,
				0
			},
			{
				2080001,
				0
			},
			{
				2080007,
				0
			},
			{
				2080008,
				0
			},
			{
				2080009,
				0
			},
			{
				2080002,
				0
			},
			{
				116,
				0
			},
			{
				121,
				0
			},
			{
				2000853,
				0
			},
			{
				2000854,
				0
			},
			{
				2000855,
				0
			},
			{
				2000856,
				0
			},
			{
				2000857,
				0
			},
			{
				10013,
				0
			},
			{
				10020,
				0
			},
			{
				10022,
				0
			},
			{
				10023,
				0
			},
			{
				10012,
				0
			},
			{
				10011,
				0
			},
			{
				10010,
				0
			},
			{
				10009,
				0
			},
			{
				10008,
				0
			},
			{
				10007,
				0
			},
			{
				10001,
				0
			},
			{
				10002,
				0
			},
			{
				10003,
				0
			},
			{
				10004,
				0
			},
			{
				9000,
				0
			},
			{
				9001,
				0
			},
			{
				9002,
				0
			},
			{
				9003,
				0
			},
			{
				9004,
				0
			},
			{
				9005,
				0
			},
			{
				9006,
				0
			},
			{
				9007,
				0
			},
			{
				9008,
				0
			},
			{
				9009,
				0
			},
			{
				9010,
				0
			},
			{
				9011,
				0
			},
			{
				9012,
				0
			},
			{
				9051,
				0
			},
			{
				9052,
				0
			}
		};

		
		public static Tuple<double, double>[] ExtPropThreshold = new Tuple<double, double>[177];

		
		private static HashSet<int> DisabledKuaFuWorldCmds = new HashSet<int>();
	}
}
