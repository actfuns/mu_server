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
	// Token: 0x02000616 RID: 1558
	public static class Data
	{
		// Token: 0x06001F93 RID: 8083 RVA: 0x001B5BAC File Offset: 0x001B3DAC
		public static int GetTotalLoginInfoNum()
		{
			int count;
			lock (Data.TotalLoginDataInfoListLock)
			{
				count = Data.TotalLoginDataInfoList.Count;
			}
			return count;
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x001B5BFC File Offset: 0x001B3DFC
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

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06001F95 RID: 8085 RVA: 0x001B5C68 File Offset: 0x001B3E68
		// (set) Token: 0x06001F96 RID: 8086 RVA: 0x001B5CB4 File Offset: 0x001B3EB4
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

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06001F97 RID: 8087 RVA: 0x001B5D00 File Offset: 0x001B3F00
		// (set) Token: 0x06001F98 RID: 8088 RVA: 0x001B5D4C File Offset: 0x001B3F4C
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

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06001F99 RID: 8089 RVA: 0x001B5D98 File Offset: 0x001B3F98
		public static List<ChannelName> ChannelNameConfigList
		{
			get
			{
				return Data._ChannelNameConfigList.Value;
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06001F9A RID: 8090 RVA: 0x001B5DB4 File Offset: 0x001B3FB4
		public static List<LianZhanConfig> LianZhanConfigList
		{
			get
			{
				return Data._LianZhanConfigList.Value;
			}
		}

		// Token: 0x06001F9B RID: 8091 RVA: 0x001B5DD0 File Offset: 0x001B3FD0
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

		// Token: 0x06001F9C RID: 8092 RVA: 0x001B62F4 File Offset: 0x001B44F4
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

		// Token: 0x06001F9D RID: 8093 RVA: 0x001B6394 File Offset: 0x001B4594
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

		// Token: 0x06001F9E RID: 8094 RVA: 0x001B64DC File Offset: 0x001B46DC
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

		// Token: 0x06001F9F RID: 8095 RVA: 0x001B662C File Offset: 0x001B482C
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

		// Token: 0x06001FA0 RID: 8096 RVA: 0x001B66D0 File Offset: 0x001B48D0
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

		// Token: 0x06001FA1 RID: 8097 RVA: 0x001B6794 File Offset: 0x001B4994
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

		// Token: 0x06001FA2 RID: 8098 RVA: 0x001B6828 File Offset: 0x001B4A28
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

		// Token: 0x06001FA3 RID: 8099 RVA: 0x001B68AC File Offset: 0x001B4AAC
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

		// Token: 0x06001FA4 RID: 8100 RVA: 0x001B693C File Offset: 0x001B4B3C
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

		// Token: 0x06001FA5 RID: 8101 RVA: 0x001B69F0 File Offset: 0x001B4BF0
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

		// Token: 0x06001FA6 RID: 8102 RVA: 0x001B6A8C File Offset: 0x001B4C8C
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

		// Token: 0x06001FA7 RID: 8103 RVA: 0x001B6B98 File Offset: 0x001B4D98
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

		// Token: 0x06001FA8 RID: 8104 RVA: 0x001B6BE4 File Offset: 0x001B4DE4
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

		// Token: 0x06001FA9 RID: 8105 RVA: 0x001B6C74 File Offset: 0x001B4E74
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

		// Token: 0x06001FAA RID: 8106 RVA: 0x001B6CB0 File Offset: 0x001B4EB0
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

		// Token: 0x06001FAB RID: 8107 RVA: 0x001B6D68 File Offset: 0x001B4F68
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

		// Token: 0x06001FAC RID: 8108 RVA: 0x001B6DF0 File Offset: 0x001B4FF0
		public static bool IsMiniBufferDataId(int id)
		{
			bool result;
			lock (Data.MiniBufferDataIds)
			{
				result = Data.MiniBufferDataIds.ContainsKey(id);
			}
			return result;
		}

		// Token: 0x06001FAD RID: 8109 RVA: 0x001B6E40 File Offset: 0x001B5040
		public static bool KuaFuWorldCmdEnabled(int cmd)
		{
			HashSet<int> hashset = Data.KuaFuWorldCmds;
			return hashset == null || !hashset.Contains(cmd);
		}

		// Token: 0x06001FAE RID: 8110 RVA: 0x001B6E74 File Offset: 0x001B5074
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

		// Token: 0x04002C2A RID: 11306
		public const int ConstSubPKPointPerMin = 10;

		// Token: 0x04002C2B RID: 11307
		public const string GAME_CONFIG_USERWHITELIST_FILE = "Config/LoginUserWhiteList.xml";

		// Token: 0x04002C2C RID: 11308
		public const int RoleMaxLevel = 100;

		// Token: 0x04002C2D RID: 11309
		public const int GlobalData = 0;

		// Token: 0x04002C2E RID: 11310
		public static int WalkUnitCost;

		// Token: 0x04002C2F RID: 11311
		public static int RunUnitCost;

		// Token: 0x04002C30 RID: 11312
		public static int[] SpeedTickList;

		// Token: 0x04002C31 RID: 11313
		public static int WalkStepWidth;

		// Token: 0x04002C32 RID: 11314
		public static int RunStepWidth;

		// Token: 0x04002C33 RID: 11315
		public static int MaxAttackDistance;

		// Token: 0x04002C34 RID: 11316
		public static int MinAttackDistance;

		// Token: 0x04002C35 RID: 11317
		public static int MaxMagicDistance;

		// Token: 0x04002C36 RID: 11318
		public static int MaxAttackSlotTick;

		// Token: 0x04002C37 RID: 11319
		public static int LifeTotalWidth;

		// Token: 0x04002C38 RID: 11320
		public static int HoldWidth;

		// Token: 0x04002C39 RID: 11321
		public static int HoldHeight;

		// Token: 0x04002C3A RID: 11322
		public static int GoodsPackOvertimeTick = 90;

		// Token: 0x04002C3B RID: 11323
		public static int PackDestroyTimeTick = 90;

		// Token: 0x04002C3C RID: 11324
		public static int TaskMaxFocusCount = 400;

		// Token: 0x04002C3D RID: 11325
		public static int AliveGoodsID = -1;

		// Token: 0x04002C3E RID: 11326
		public static int AliveMaxLevel = 10;

		// Token: 0x04002C3F RID: 11327
		public static int AutoGetThing = 0;

		// Token: 0x04002C40 RID: 11328
		public static long[] LevelUpExperienceList = null;

		// Token: 0x04002C41 RID: 11329
		public static RoleSitExpItem[] RoleSitExpList = null;

		// Token: 0x04002C42 RID: 11330
		public static List<RoleBasePropItem[]> RoleBasePropList = new List<RoleBasePropItem[]>();

		// Token: 0x04002C43 RID: 11331
		public static List<MapStallItem> MapStallList = new List<MapStallItem>(10);

		// Token: 0x04002C44 RID: 11332
		public static Dictionary<int, string> MapNamesDict = new Dictionary<int, string>(100);

		// Token: 0x04002C45 RID: 11333
		public static Dictionary<int, ChangeOccupInfo> ChangeOccupInfoList = new Dictionary<int, ChangeOccupInfo>();

		// Token: 0x04002C46 RID: 11334
		public static Dictionary<int, double> ChangeLifeEverydayExpRate = new Dictionary<int, double>();

		// Token: 0x04002C47 RID: 11335
		public static Dictionary<int, OccupationAddPointInfo> OccupationAddPointInfoList = new Dictionary<int, OccupationAddPointInfo>();

		// Token: 0x04002C48 RID: 11336
		public static Dictionary<int, ChangeLifeAddPointInfo> ChangeLifeAddPointInfoList = new Dictionary<int, ChangeLifeAddPointInfo>();

		// Token: 0x04002C49 RID: 11337
		public static Dictionary<int, BloodCastleDataInfo> BloodCastleDataInfoList = new Dictionary<int, BloodCastleDataInfo>();

		// Token: 0x04002C4A RID: 11338
		public static Dictionary<int, MoBaiData> MoBaiDataInfoList = new Dictionary<int, MoBaiData>();

		// Token: 0x04002C4B RID: 11339
		public static Dictionary<int, List<CopyScoreDataInfo>> CopyScoreDataInfoList = new Dictionary<int, List<CopyScoreDataInfo>>();

		// Token: 0x04002C4C RID: 11340
		public static FreshPlayerCopySceneInfo FreshPlayerSceneInfo = new FreshPlayerCopySceneInfo();

		// Token: 0x04002C4D RID: 11341
		public static List<TaskStarDataInfo> TaskStarInfo = new List<TaskStarDataInfo>();

		// Token: 0x04002C4E RID: 11342
		public static List<DailyCircleTaskAwardInfo> DailyCircleTaskAward = new List<DailyCircleTaskAwardInfo>();

		// Token: 0x04002C4F RID: 11343
		public static TaofaTaskAwardInfo TaofaTaskExAward = new TaofaTaskAwardInfo();

		// Token: 0x04002C50 RID: 11344
		public static Dictionary<int, CombatForceInfo> CombatForceDataInfo = new Dictionary<int, CombatForceInfo>();

		// Token: 0x04002C51 RID: 11345
		public static Dictionary<int, DaimonSquareDataInfo> DaimonSquareDataInfoList = new Dictionary<int, DaimonSquareDataInfo>();

		// Token: 0x04002C52 RID: 11346
		public static double[] WingForgeLevelAddShangHaiJiaCheng = null;

		// Token: 0x04002C53 RID: 11347
		public static double[] WingForgeLevelAddDefenseRates = null;

		// Token: 0x04002C54 RID: 11348
		public static double[] WingForgeLevelAddShangHaiXiShou = null;

		// Token: 0x04002C55 RID: 11349
		public static double[] WingZhuiJiaLevelAddDefenseRates = null;

		// Token: 0x04002C56 RID: 11350
		public static double[] ForgeLevelAddAttackRates = null;

		// Token: 0x04002C57 RID: 11351
		public static double[] ForgeLevelAddDefenseRates = null;

		// Token: 0x04002C58 RID: 11352
		public static double[] ZhuiJiaLevelAddAttackRates = null;

		// Token: 0x04002C59 RID: 11353
		public static double[] ZhuiJiaLevelAddDefenseRates = null;

		// Token: 0x04002C5A RID: 11354
		public static double[] ForgeLevelAddMaxLifeVRates = null;

		// Token: 0x04002C5B RID: 11355
		public static double[] ZhuoYueAddAttackRates = null;

		// Token: 0x04002C5C RID: 11356
		public static double[] ZhuoYueAddDefenseRates = null;

		// Token: 0x04002C5D RID: 11357
		public static double[] RebornZhuoYueAddRates = null;

		// Token: 0x04002C5E RID: 11358
		public static int[] ForgeProtectStoneGoodsID = null;

		// Token: 0x04002C5F RID: 11359
		public static int[] ForgeProtectStoneGoodsNum = null;

		// Token: 0x04002C60 RID: 11360
		public static int DiamondToVipExpValue = 0;

		// Token: 0x04002C61 RID: 11361
		public static double[] RedNameDebuffInfo = null;

		// Token: 0x04002C62 RID: 11362
		public static string[] ForgeNeedGoodsID = new string[21];

		// Token: 0x04002C63 RID: 11363
		public static string[] ForgeNeedGoodsNum = new string[21];

		// Token: 0x04002C64 RID: 11364
		public static Dictionary<int, int> MapTransNeedMoneyDict = new Dictionary<int, int>();

		// Token: 0x04002C65 RID: 11365
		public static double[] EquipChangeLifeAddAttackRates = null;

		// Token: 0x04002C66 RID: 11366
		public static double[] EquipChangeLifeAddDefenseRates = null;

		// Token: 0x04002C67 RID: 11367
		public static int[] KillBossCountForChengJiu = null;

		// Token: 0x04002C68 RID: 11368
		public static int InsertAwardtPortableBagTaskID = 0;

		// Token: 0x04002C69 RID: 11369
		public static string InsertAwardtPortableBagGoodsInfo = null;

		// Token: 0x04002C6A RID: 11370
		public static int PaihangbangAdration = 0;

		// Token: 0x04002C6B RID: 11371
		public static int[] StoryCopyMapID = null;

		// Token: 0x04002C6C RID: 11372
		public static int FreeImpetrateIntervalTime = 0;

		// Token: 0x04002C6D RID: 11373
		public static Dictionary<int, TotalLoginDataInfo> TotalLoginDataInfoList = new Dictionary<int, TotalLoginDataInfo>();

		// Token: 0x04002C6E RID: 11374
		public static object TotalLoginDataInfoListLock = new object();

		// Token: 0x04002C6F RID: 11375
		public static Dictionary<int, VIPDataInfo> VIPDataInfoList = new Dictionary<int, VIPDataInfo>();

		// Token: 0x04002C70 RID: 11376
		public static Dictionary<int, VIPLevAwardAndExpInfo> VIPLevAwardAndExpInfoList = new Dictionary<int, VIPLevAwardAndExpInfo>();

		// Token: 0x04002C71 RID: 11377
		public static Dictionary<int, MeditateData> MeditateInfoList = new Dictionary<int, MeditateData>();

		// Token: 0x04002C72 RID: 11378
		public static Dictionary<int, ExperienceCopyMapDataInfo> ExperienceCopyMapDataInfoList = new Dictionary<int, ExperienceCopyMapDataInfo>();

		// Token: 0x04002C73 RID: 11379
		public static PKKingAdrationData PKkingadrationData = new PKKingAdrationData();

		// Token: 0x04002C74 RID: 11380
		public static PKKingAdrationData LLCZadrationData = new PKKingAdrationData();

		// Token: 0x04002C75 RID: 11381
		public static BossHomeData BosshomeData = new BossHomeData();

		// Token: 0x04002C76 RID: 11382
		public static GoldTempleData GoldtempleData = new GoldTempleData();

		// Token: 0x04002C77 RID: 11383
		public static Dictionary<int, PictureJudgeData> PicturejudgeData = new Dictionary<int, PictureJudgeData>();

		// Token: 0x04002C78 RID: 11384
		public static Dictionary<int, PictureJudgeTypeData> PicturejudgeTypeData = new Dictionary<int, PictureJudgeTypeData>();

		// Token: 0x04002C79 RID: 11385
		public static Dictionary<int, Dictionary<int, MuEquipUpgradeData>> EquipUpgradeData = new Dictionary<int, Dictionary<int, MuEquipUpgradeData>>();

		// Token: 0x04002C7A RID: 11386
		public static GoldCopySceneData Goldcopyscenedata = new GoldCopySceneData();

		// Token: 0x04002C7B RID: 11387
		public static Dictionary<int, EquipJuHunXmlData> EquipJuHunDataDict = new Dictionary<int, EquipJuHunXmlData>();

		// Token: 0x04002C7C RID: 11388
		public static Dictionary<int, BagTypeXmlData> BagTypeDict = new Dictionary<int, BagTypeXmlData>();

		// Token: 0x04002C7D RID: 11389
		private static SingleChargeData _ChargeData = null;

		// Token: 0x04002C7E RID: 11390
		private static object SingleChargeDataMutex = new object();

		// Token: 0x04002C7F RID: 11391
		private static object ChargeItemDataMutex = new object();

		// Token: 0x04002C80 RID: 11392
		public static Dictionary<int, ChargeItemData> _ChargeItemDict = null;

		// Token: 0x04002C81 RID: 11393
		public static Dictionary<int, int> LingYuMaterialZuanshiDict = new Dictionary<int, int>();

		// Token: 0x04002C82 RID: 11394
		public static Dictionary<int, int> FuBenNeedDict = new Dictionary<int, int>();

		// Token: 0x04002C83 RID: 11395
		public static long CombatForceLogMinValue = 1800000L;

		// Token: 0x04002C84 RID: 11396
		public static double CombatForceLogPercent = 0.25;

		// Token: 0x04002C85 RID: 11397
		private static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();

		// Token: 0x04002C86 RID: 11398
		public static HashSet<string> UseridWhiteList = new HashSet<string>();

		// Token: 0x04002C87 RID: 11399
		public static HashSet<int> DaTianShiGoodsIdList = new HashSet<int>();

		// Token: 0x04002C88 RID: 11400
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

		// Token: 0x04002C89 RID: 11401
		public static long FightStateTime = 6000L;

		// Token: 0x04002C8A RID: 11402
		public static int NotifyLiXianAwardMin = 0;

		// Token: 0x04002C8B RID: 11403
		public static int OfflineRW_ItemLimit = 0;

		// Token: 0x04002C8C RID: 11404
		public static bool CheckTimeBoost = true;

		// Token: 0x04002C8D RID: 11405
		public static bool CheckPositionCheat = true;

		// Token: 0x04002C8E RID: 11406
		public static bool CheckPositionCheatSpeed = true;

		// Token: 0x04002C8F RID: 11407
		public static long SyncTimeByClient = 0L;

		// Token: 0x04002C90 RID: 11408
		public static bool IgnoreClientPos = false;

		// Token: 0x04002C91 RID: 11409
		public static long MaxServerClientTimeDiff;

		// Token: 0x04002C92 RID: 11410
		public static int RoleOccupationMaxCount = 2;

		// Token: 0x04002C93 RID: 11411
		public static int OpChangeLifeCount = 100;

		// Token: 0x04002C94 RID: 11412
		public static SystemOpenData OpenData = new SystemOpenData();

		// Token: 0x04002C95 RID: 11413
		public static int ZhuTiID;

		// Token: 0x04002C96 RID: 11414
		public static int ThemeActivityState;

		// Token: 0x04002C97 RID: 11415
		public static TemplateLoader<Dictionary<int, MapSettingItem>> SettingsDict = new TemplateLoader<Dictionary<int, MapSettingItem>>();

		// Token: 0x04002C98 RID: 11416
		public static int MinLianZhanNum;

		// Token: 0x04002C99 RID: 11417
		public static int MaxLianZhanNum;

		// Token: 0x04002C9A RID: 11418
		private static TemplateLoader<List<ChannelName>> _ChannelNameConfigList = new TemplateLoader<List<ChannelName>>();

		// Token: 0x04002C9B RID: 11419
		private static TemplateLoader<List<LianZhanConfig>> _LianZhanConfigList = new TemplateLoader<List<LianZhanConfig>>();

		// Token: 0x04002C9C RID: 11420
		public static List<List<int>> LianZhanTimes = null;

		// Token: 0x04002C9D RID: 11421
		public static int[] LianZhanMaps = null;

		// Token: 0x04002C9E RID: 11422
		private static HashSet<int> KuaFuWorldCmds = new HashSet<int>();

		// Token: 0x04002C9F RID: 11423
		public static bool LoadEquipDelay = true;

		// Token: 0x04002CA0 RID: 11424
		public static long LuoLanKingGongGaoCD = 120L;

		// Token: 0x04002CA1 RID: 11425
		public static LongCollection NextBroadCastTickDict = new LongCollection();

		// Token: 0x04002CA2 RID: 11426
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

		// Token: 0x04002CA3 RID: 11427
		public static Tuple<double, double>[] ExtPropThreshold = new Tuple<double, double>[177];

		// Token: 0x04002CA4 RID: 11428
		private static HashSet<int> DisabledKuaFuWorldCmds = new HashSet<int>();
	}
}
