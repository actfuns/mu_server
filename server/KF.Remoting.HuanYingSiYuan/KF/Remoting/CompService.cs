using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	
	public class CompService
	{
		
		public static CompService Instance()
		{
			return CompService._instance;
		}

		
		
		
		private KuaFuData<Dictionary<int, KFCompData>> CompDataDict
		{
			get
			{
				return this.Persistence.CompDataDict;
			}
			set
			{
				this.Persistence.CompDataDict = value;
			}
		}

		
		
		
		private KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankJunXianDict
		{
			get
			{
				return this.Persistence.CompRankJunXianDict;
			}
			set
			{
				this.Persistence.CompRankJunXianDict = value;
			}
		}

		
		
		
		private KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankJunXianLastDict
		{
			get
			{
				return this.Persistence.CompRankJunXianLastDict;
			}
			set
			{
				this.Persistence.CompRankJunXianLastDict = value;
			}
		}

		
		
		
		private KuaFuData<List<KFCompRankInfo>> CompRankBossDamageList
		{
			get
			{
				return this.Persistence.CompRankBossDamageList;
			}
			set
			{
				this.Persistence.CompRankBossDamageList = value;
			}
		}

		
		
		
		private KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankBattleJiFenDict
		{
			get
			{
				return this.Persistence.CompRankBattleJiFenDict;
			}
			set
			{
				this.Persistence.CompRankBattleJiFenDict = value;
			}
		}

		
		
		
		private KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankMineJiFenDict
		{
			get
			{
				return this.Persistence.CompRankMineJiFenDict;
			}
			set
			{
				this.Persistence.CompRankMineJiFenDict = value;
			}
		}

		
		
		
		private Dictionary<int, KuaFuData<KFCompRoleData>> CompRoleDataDict
		{
			get
			{
				return this.Persistence.CompRoleDataDict;
			}
			set
			{
				this.Persistence.CompRoleDataDict = value;
			}
		}

		
		public void InitConfig()
		{
			try
			{
				string strCompReplaceAmerce = KuaFuServerManager.systemParamsList.GetParamValueByName("CompReplaceAmerce");
				this.CompReplaceAmerce = Global.SafeConvertToDouble(strCompReplaceAmerce);
				string strCompNumEveryDay = KuaFuServerManager.systemParamsList.GetParamValueByName("CompNumEveryDay");
				string[] strFields = strCompNumEveryDay.Split(new char[]
				{
					','
				});
				if (strFields.Length == 2)
				{
					this.CompBoomValueReduce = Global.SafeConvertToInt32(strFields[0]);
					this.CompBoomValueMin = Global.SafeConvertToInt32(strFields[1]);
				}
				lock (this.Mutex)
				{
					string fileName = "Config/Comp.xml";
					string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.CompConfigDict.Clear();
					XElement xml = ConfigHelper.Load(fullPathFileName);
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						CompConfig item = new CompConfig();
						item.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "CompID", 0L);
						item.MapCode = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MapCode", 0L);
						item.BossID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MonstersID", 0L);
						item.MaxPlayer = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MaxPlayer", 0L);
						this.CompConfigDict[item.ID] = item;
					}
					fileName = "Config/ForceCraft.xml";
					fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.CompBattleConfigDict.Clear();
					xml = ConfigHelper.Load(fullPathFileName);
					xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						CompBattleConfig item2 = new CompBattleConfig();
						item2.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
						item2.MapCode = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MapCode", 0L);
						item2.MaxEnterNum = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MaxEnterNum", 0L);
						item2.EnterCD = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "EnterCD", 0L);
						item2.PrepareSecs = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "PrepareSecs", 0L);
						item2.FightingSecs = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "FightingSecs", 0L);
						item2.ClearRolesSecs = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ClearRolesSecs", 0L);
						string[] fields = xmlItem.Attribute("TimePoints").Value.Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (int i = 0; i < fields.Length; i += 3)
						{
							TimeSpan dayPart = new TimeSpan(Convert.ToInt32(fields[i]), 0, 0, 0);
							TimeSpan time = DateTime.Parse(fields[i + 1]).TimeOfDay.Add(dayPart);
							TimeSpan time2 = DateTime.Parse(fields[i + 2]).TimeOfDay.Add(dayPart);
							item2.TimePoints.Add(time);
							item2.TimePoints.Add(time2);
						}
						for (int i = 0; i < item2.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(item2.TimePoints[i].Hours, item2.TimePoints[i].Minutes, item2.TimePoints[i].Seconds);
							item2.SecondsOfDay.Add(ts.TotalSeconds);
						}
						this.CompBattleConfigDict[item2.ID] = item2;
					}
					fileName = "Config/CompMineWar.xml";
					fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.CompMineConfigDict.Clear();
					xml = ConfigHelper.Load(fullPathFileName);
					xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						CompMineConfig item3 = new CompMineConfig();
						item3.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
						item3.MapCode = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MapCode", 0L);
						item3.MaxEnterNum = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MaxEnterNum", 0L);
						item3.EnterCD = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "EnterCD", 0L);
						item3.PrepareSecs = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "PrepareSecs", 0L);
						item3.FightingSecs = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "FightingSecs", 0L);
						item3.ClearRolesSecs = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ClearRolesSecs", 0L);
						string[] fields = xmlItem.Attribute("TimePoints").Value.Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (int i = 0; i < fields.Length; i += 3)
						{
							TimeSpan dayPart = new TimeSpan(Convert.ToInt32(fields[i]), 0, 0, 0);
							TimeSpan time = DateTime.Parse(fields[i + 1]).TimeOfDay.Add(dayPart);
							TimeSpan time2 = DateTime.Parse(fields[i + 2]).TimeOfDay.Add(dayPart);
							item3.TimePoints.Add(time);
							item3.TimePoints.Add(time2);
						}
						for (int i = 0; i < item3.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(item3.TimePoints[i].Hours, item3.TimePoints[i].Minutes, item3.TimePoints[i].Seconds);
							item3.SecondsOfDay.Add(ts.TotalSeconds);
						}
						this.CompMineConfigDict[item3.ID] = item3;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public void LoadDatabase(DateTime now)
		{
			try
			{
				this.Persistence.LoadDatabase();
				this.InitServerCompData();
				this.UpdateCompRankBattleJiFen(now);
				this.UpdateCompRankMineJiFen(now);
				this.LastUpdateDayID = TimeUtil.GetOffsetDay(now);
				this.LastUpdateHour = now.Hour;
				this.CompDataDayID = this.Persistence.GetCompDayID();
				this.CompDataWeekDayID = this.Persistence.GetCompWeekDayID();
				this.CompBattleWeekDayID = this.Persistence.GetCompBattleWeekDayID();
				this.CompMineWeekDayID = this.Persistence.GetCompMineWeekDayID();
				this.HandleChangeDay(now);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "CompService.LoadDatabase failed!", ex, true);
			}
		}

		
		public void OnStopServer()
		{
			this.Persistence.DelayWriteDataProc();
		}

		
		public void Update(DateTime now)
		{
			try
			{
				this.Persistence.DelayWriteDataProc();
				if (now > this.CheckTime10)
				{
					this.CheckTime10 = now.AddSeconds(10.0);
					this.CheckRoleTimerProc(now);
					this.CheckGameFuBenTimerProc(now);
					this.HandleCompMineLogicSomething(now);
					this.HandleCompBattleLogicSomething(now);
				}
				int CurrentHour = now.Hour;
				if (CurrentHour != this.LastUpdateHour)
				{
					this.HandleChangeHour(now);
					this.LastUpdateHour = CurrentHour;
				}
				int CurrentDayID = TimeUtil.GetOffsetDay(now);
				if (CurrentDayID != this.LastUpdateDayID)
				{
					this.HandleChangeDay(now);
					this.LastUpdateDayID = CurrentDayID;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "CompService.Update failed!", ex, true);
			}
		}

		
		private void InitServerCompData()
		{
			for (int compLoop = 1; compLoop <= 3; compLoop++)
			{
				KFCompData kfCompData = null;
				if (!this.CompDataDict.V.TryGetValue(compLoop, out kfCompData))
				{
					kfCompData = new KFCompData();
					kfCompData.InitPlunderResList();
					kfCompData.CompType = compLoop;
					this.CompDataDict.V[compLoop] = kfCompData;
					TimeUtil.AgeByNow(ref this.CompDataDict.Age);
					this.Persistence.SaveCompData(kfCompData, false);
				}
			}
		}

		
		private void HandleChangeHour(DateTime now)
		{
			lock (this.Mutex)
			{
				for (int compLoop = 1; compLoop <= 3; compLoop++)
				{
					this.Persistence.LoadCompRankInfo(1, compLoop, this.CompRankJunXianDict, null);
				}
			}
		}

		
		private void HandleChangeDay(DateTime now)
		{
			int curDayId = TimeUtil.GetOffsetDay(now);
			int curWeekDayId = TimeUtil.GetWeekStartDayIdNow();
			if (this.CompDataDayID == 0 || 0 == this.CompDataWeekDayID)
			{
				this.CompDataDayID = curDayId;
				this.CompDataWeekDayID = curWeekDayId;
				this.Persistence.SaveCompDayID(this.CompDataDayID);
				this.Persistence.SaveCompWeekDayID(this.CompDataWeekDayID);
			}
			else
			{
				lock (this.Mutex)
				{
					if (curDayId != this.CompDataDayID)
					{
						foreach (KFCompData item in this.CompDataDict.V.Values)
						{
							if (0 != item.EnemyCompTypeSet)
							{
								item.EnemyCompType = item.EnemyCompTypeSet;
								item.EnemyCompTypeSet = 0;
							}
							item.YestdCrystal = item.Crystal;
							item.Crystal = 0;
							item.YestdBoss = item.Boss;
							item.Boss = 0;
							item.YestdBossKillCompType = item.BossKillCompType;
							item.BossKillCompType = 0;
							for (int i = 1; i <= 3; i++)
							{
								item.YestdPlunderResList[i - 1] = item.PlunderResList[i - 1];
								item.PlunderResList[i - 1] = 0;
							}
							if (item.BoomValue > this.CompBoomValueMin)
							{
								item.BoomValue -= Math.Min(this.CompBoomValueReduce, item.BoomValue - this.CompBoomValueMin);
							}
							item.YestdBoomValue = item.BoomValue;
							this.Persistence.SaveCompData(item, true);
						}
						TimeUtil.AgeByNow(ref this.CompDataDict.Age);
						this.CompDataDayID = curDayId;
						this.Persistence.SaveCompDayID(this.CompDataDayID);
					}
					if (curWeekDayId != this.CompDataWeekDayID)
					{
						for (int compLoop = 1; compLoop <= 3; compLoop++)
						{
							this.Persistence.LoadCompRankInfo(1, compLoop, this.CompRankJunXianDict, null);
						}
						foreach (KuaFuData<KFCompRoleData> item2 in this.CompRoleDataDict.Values)
						{
							if (item2.V.JunXianLast != 0 || item2.V.JunXian != 0 || item2.V.CompTypeLast != item2.V.CompType)
							{
								item2.V.JunXianLast = item2.V.JunXian;
								item2.V.JunXian = 0;
								item2.V.CompTypeLast = item2.V.CompType;
								TimeUtil.AgeByNow(ref item2.Age);
								this.Persistence.SaveCompRoleData(item2.V, false, true, false, false);
							}
						}
						for (int compLoop = 1; compLoop <= 3; compLoop++)
						{
							List<KFCompRankInfo> kfRankList = null;
							this.CompRankJunXianDict.V.TryGetValue(compLoop, out kfRankList);
							this.CompRankJunXianLastDict.V[compLoop] = new List<KFCompRankInfo>(kfRankList);
							kfRankList.Clear();
						}
						TimeUtil.AgeByNow(ref this.CompRankJunXianDict.Age);
						TimeUtil.AgeByNow(ref this.CompRankJunXianLastDict.Age);
						this.CompDataWeekDayID = curWeekDayId;
						this.Persistence.SaveCompWeekDayID(this.CompDataWeekDayID);
					}
				}
			}
		}

		
		public CompSyncData Comp_SyncData(long ageComp, long ageRankJX, long ageRankJXL, long ageRankBD, long ageRankBJF, long ageRankMJF)
		{
			try
			{
				CompSyncData SyncData = new CompSyncData();
				SyncData.ServerLineList.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_1));
				SyncData.ServerLineList.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_2));
				SyncData.ServerLineList.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_3));
				lock (this.Mutex)
				{
					SyncData.BytesCompMapDataDict = DataHelper2.ObjectToBytes<Dictionary<int, CompMapData>>(this.CompMapDataDict);
					SyncData.CompDataDict.Age = this.CompDataDict.Age;
					SyncData.CompRankJunXianDict.Age = this.CompRankJunXianDict.Age;
					SyncData.CompRankJunXianLastDict.Age = this.CompRankJunXianLastDict.Age;
					SyncData.CompRankBossDamageList.Age = this.CompRankBossDamageList.Age;
					SyncData.CompRankBattleJiFenDict.Age = this.CompRankBattleJiFenDict.Age;
					SyncData.CompRankMineJiFenDict.Age = this.CompRankMineJiFenDict.Age;
					if (ageComp != this.CompDataDict.Age)
					{
						SyncData.BytesCompDataDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, KFCompData>>>(this.CompDataDict);
					}
					if (ageRankJX != this.CompRankJunXianDict.Age)
					{
						SyncData.BytesCompRankJunXianDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(this.CompRankJunXianDict);
					}
					if (ageRankJXL != this.CompRankJunXianLastDict.Age)
					{
						SyncData.BytesCompRankJunXianLastDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(this.CompRankJunXianLastDict);
					}
					if (ageRankBD != this.CompRankBossDamageList.Age)
					{
						SyncData.BytesCompRankBossDamageList = DataHelper2.ObjectToBytes<KuaFuData<List<KFCompRankInfo>>>(this.CompRankBossDamageList);
					}
					if (ageRankBJF != this.CompRankBattleJiFenDict.Age)
					{
						SyncData.CompBattleJoinRoleNum = this.CompBattleJoinRoleNum;
						SyncData.BytesCompRankBattleJiFenDict = this.BytesCompRankBattleJiFenDict;
					}
					if (ageRankBJF != this.CompRankMineJiFenDict.Age)
					{
						SyncData.CompMineJoinRoleNum = this.CompMineJoinRoleNum;
						SyncData.BytesCompRankMineJiFenDict = this.BytesCompRankMineJiFenDict;
					}
					return SyncData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		private int GetZhiWuByRankJunXianLast(int compType, int rid)
		{
			List<KFCompRankInfo> rankInfoList = null;
			this.CompRankJunXianLastDict.V.TryGetValue(compType, out rankInfoList);
			int result;
			if (rankInfoList == null || rankInfoList.Count == 0)
			{
				result = 0;
			}
			else
			{
				int zhiwuIdx = rankInfoList.FindIndex((KFCompRankInfo x) => x.Key == rid) + 1;
				result = ((zhiwuIdx > 5) ? 0 : zhiwuIdx);
			}
			return result;
		}

		
		public KuaFuCmdData GetCompRoleData(int roleId, long dataAge)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<KFCompRoleData> kfCompRoleData = null;
					if (!this.CompRoleDataDict.TryGetValue(roleId, out kfCompRoleData))
					{
						return null;
					}
					if (dataAge != kfCompRoleData.Age)
					{
						kfCompRoleData.V.ZhiWu = this.GetZhiWuByRankJunXianLast(kfCompRoleData.V.CompType, roleId);
						return new KuaFuCmdData
						{
							Age = kfCompRoleData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KFCompRoleData>(kfCompRoleData.V)
						};
					}
					return new KuaFuCmdData
					{
						Age = kfCompRoleData.Age
					};
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public void ChangeName(int roleId, string roleName)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<KFCompRoleData> kfCompRoleData = null;
					if (this.CompRoleDataDict.TryGetValue(roleId, out kfCompRoleData))
					{
						kfCompRoleData.V.RoleName = roleName;
						TimeUtil.AgeByNow(ref kfCompRoleData.Age);
						this.Persistence.SaveCompRoleData(kfCompRoleData.V, false, false, false, false);
						for (int compLoop = 1; compLoop <= 3; compLoop++)
						{
							List<KFCompRankInfo> rankList = null;
							if (this.CompRankJunXianDict.V.TryGetValue(compLoop, out rankList))
							{
								foreach (KFCompRankInfo item in rankList)
								{
									if (item.Key == roleId)
									{
										item.Param1 = KuaFuServerManager.FormatName(roleName, item.tagInfo.V.ZoneID);
										TimeUtil.AgeByNow(ref this.CompRankJunXianDict.Age);
									}
								}
							}
						}
						for (int compLoop = 1; compLoop <= 3; compLoop++)
						{
							List<KFCompRankInfo> rankList = null;
							if (this.CompRankJunXianLastDict.V.TryGetValue(compLoop, out rankList))
							{
								foreach (KFCompRankInfo item in rankList)
								{
									if (item.Key == roleId)
									{
										item.Param1 = KuaFuServerManager.FormatName(roleName, item.tagInfo.V.ZoneID);
										TimeUtil.AgeByNow(ref this.CompRankJunXianLastDict.Age);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public int Comp_JoinComp_Repair(int roleId, int zoneId, string roleName, int compType, int battleJiFen)
		{
			int ret = 0;
			int result;
			if (compType < 1 || compType > 3)
			{
				ret = -12;
				result = ret;
			}
			else
			{
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KFCompRoleData> kfCompRoleData = null;
						if (this.CompRoleDataDict.TryGetValue(roleId, out kfCompRoleData))
						{
							ret = -12;
							return ret;
						}
						kfCompRoleData = new KuaFuData<KFCompRoleData>();
						kfCompRoleData.V.RoleID = roleId;
						kfCompRoleData.V.ZoneID = zoneId;
						kfCompRoleData.V.RoleName = roleName;
						kfCompRoleData.V.CompType = compType;
						kfCompRoleData.V.CompTypeBattle = compType;
						kfCompRoleData.V.BattleJiFen = battleJiFen;
						this.CompRoleDataDict[roleId] = kfCompRoleData;
						TimeUtil.AgeByNow(ref kfCompRoleData.Age);
						this.Persistence.SaveCompRoleData(kfCompRoleData.V, true, true, false, false);
						LogManager.WriteLog(LogTypes.Analysis, string.Format("Comp_JoinComp_Repair roleId={0} zoneId={1} roleName={2} compType={3} battleJiFen={4}", new object[]
						{
							roleId,
							zoneId,
							roleName,
							compType,
							battleJiFen
						}), null, true);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = ret;
			}
			return result;
		}

		
		public int JoinComp(int roleId, int zoneId, string roleName, int compType)
		{
			int ret = 0;
			int result;
			if (compType < 1 || compType > 3)
			{
				ret = -12;
				result = ret;
			}
			else
			{
				try
				{
					lock (this.Mutex)
					{
						KFCompData kfCompData = null;
						if (!this.CompDataDict.V.TryGetValue(compType, out kfCompData))
						{
							kfCompData = new KFCompData();
							kfCompData.InitPlunderResList();
							kfCompData.CompType = compType;
							this.CompDataDict.V[compType] = kfCompData;
							TimeUtil.AgeByNow(ref this.CompDataDict.Age);
							this.Persistence.SaveCompData(kfCompData, true);
						}
						KuaFuData<KFCompRoleData> kfCompRoleData = null;
						if (!this.CompRoleDataDict.TryGetValue(roleId, out kfCompRoleData))
						{
							kfCompRoleData = new KuaFuData<KFCompRoleData>();
							kfCompRoleData.V.RoleID = roleId;
							kfCompRoleData.V.ZoneID = zoneId;
							kfCompRoleData.V.RoleName = roleName;
							kfCompRoleData.V.CompType = compType;
							this.CompRoleDataDict[roleId] = kfCompRoleData;
							TimeUtil.AgeByNow(ref kfCompRoleData.Age);
							this.Persistence.SaveCompRoleData(kfCompRoleData.V, true, true, false, false);
						}
						else
						{
							CompBattleGameStates state = this.GetCompBattleGameStates(TimeUtil.NowDateTime());
							if (state != CompBattleGameStates.None)
							{
								ret = -12;
								return ret;
							}
							state = this.GetCompMineGameStates(TimeUtil.NowDateTime());
							if (state != CompBattleGameStates.None)
							{
								ret = -12;
								return ret;
							}
							kfCompRoleData.V.RoleName = roleName;
							kfCompRoleData.V.CompType = compType;
							kfCompRoleData.V.JunXian = (int)((double)kfCompRoleData.V.JunXian * this.CompReplaceAmerce);
							kfCompRoleData.V.BattleJiFen = 0;
							TimeUtil.AgeByNow(ref kfCompRoleData.Age);
							this.Persistence.SaveCompRoleData(kfCompRoleData.V, true, false, false, false);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = ret;
			}
			return result;
		}

		
		public void CompOpt(int compType, int optType, int param1, int param2)
		{
			if (compType >= 1 && compType <= 3)
			{
				try
				{
					lock (this.Mutex)
					{
						KFCompData kfCompData = null;
						if (this.CompDataDict.V.TryGetValue(compType, out kfCompData))
						{
							switch (optType)
							{
							case 0:
								kfCompData.BoomValue += param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfCompData, true);
								break;
							case 1:
							{
								KuaFuData<KFCompRoleData> kfCompRoleData = null;
								if (this.CompRoleDataDict.TryGetValue(param1, out kfCompRoleData))
								{
									kfCompRoleData.V.JunXian += param2;
									TimeUtil.AgeByNow(ref kfCompRoleData.Age);
									this.Persistence.SaveCompRoleData(kfCompRoleData.V, true, false, false, false);
								}
								break;
							}
							case 2:
							{
								List<int> plunderResList;
								int index;
								(plunderResList = kfCompData.PlunderResList)[index = param1 - 1] = plunderResList[index] + param2;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfCompData, true);
								break;
							}
							case 3:
								kfCompData.EnemyCompTypeSet = param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfCompData, true);
								break;
							case 4:
								kfCompData.BossKillCompType = param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfCompData, true);
								break;
							case 5:
								kfCompData.Boss += param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfCompData, true);
								break;
							case 6:
								kfCompData.Crystal += param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfCompData, true);
								break;
							case 7:
							{
								kfCompData.BossDamageTop = param1;
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								this.Persistence.SaveCompData(kfCompData, true);
								List<KFCompRankInfo> rankInfo = this.CompRankBossDamageList.V;
								rankInfo[compType - 1].Value = param1;
								TimeUtil.AgeByNow(ref this.CompRankBossDamageList.Age);
								break;
							}
							case 8:
							{
								KuaFuData<KFCompRoleData> kfCompRoleData = null;
								if (this.CompRoleDataDict.TryGetValue(param1, out kfCompRoleData))
								{
									kfCompRoleData.V.BattleJiFen += param2;
									kfCompRoleData.V.RankTmBJF = TimeUtil.NowDateTime();
									kfCompRoleData.V.CompTypeBattle = kfCompRoleData.V.CompType;
									if (!this.CompBattleJiFenRoleSet.Contains(param1))
									{
										this.CompBattleJiFenRoleSet.Add(param1);
										List<KFCompRankInfo> rankBattleJiFenList = null;
										if (this.CompRankBattleJiFenDict.V.TryGetValue(kfCompRoleData.V.CompType, out rankBattleJiFenList))
										{
											rankBattleJiFenList.Add(new KFCompRankInfo
											{
												Key = param1,
												Param1 = KuaFuServerManager.FormatName(kfCompRoleData.V.RoleName, kfCompRoleData.V.ZoneID),
												tagInfo = kfCompRoleData
											});
										}
									}
									TimeUtil.AgeByNow(ref kfCompRoleData.Age);
									this.Persistence.SaveCompRoleData(kfCompRoleData.V, false, false, true, false);
								}
								break;
							}
							case 9:
							{
								kfCompData.MineRes += param1;
								List<KFCompData> ResJiFenList = this.CompDataDict.V.Values.ToList<KFCompData>();
								ResJiFenList.Sort(delegate(KFCompData left, KFCompData right)
								{
									int result;
									if (left.MineRes > right.MineRes)
									{
										result = -1;
									}
									else if (left.MineRes < right.MineRes)
									{
										result = 1;
									}
									else if (left.MineRank < right.MineRank)
									{
										result = -1;
									}
									else if (left.MineRank > right.MineRank)
									{
										result = 1;
									}
									else if (left.CompType < right.CompType)
									{
										result = -1;
									}
									else if (left.CompType > right.CompType)
									{
										result = 1;
									}
									else
									{
										result = 0;
									}
									return result;
								});
								for (int loop = 0; loop < ResJiFenList.Count; loop++)
								{
									KFCompData ssData = ResJiFenList[loop];
									ssData.MineRank = loop + 1;
									this.Persistence.SaveCompData(ssData, true);
								}
								TimeUtil.AgeByNow(ref this.CompDataDict.Age);
								break;
							}
							case 10:
							{
								KuaFuData<KFCompRoleData> kfCompRoleData = null;
								if (this.CompRoleDataDict.TryGetValue(param1, out kfCompRoleData))
								{
									kfCompRoleData.V.MineJiFen += param2;
									kfCompRoleData.V.RankTmMJF = TimeUtil.NowDateTime();
									kfCompRoleData.V.CompTypeMine = kfCompRoleData.V.CompType;
									if (!this.CompMineJiFenRoleSet.Contains(param1))
									{
										this.CompMineJiFenRoleSet.Add(param1);
										List<KFCompRankInfo> rankMineJiFenList = null;
										if (this.CompRankMineJiFenDict.V.TryGetValue(kfCompRoleData.V.CompType, out rankMineJiFenList))
										{
											rankMineJiFenList.Add(new KFCompRankInfo
											{
												Key = param1,
												Param1 = string.Format("S{0}·{1}", kfCompRoleData.V.ZoneID, kfCompRoleData.V.RoleName),
												tagInfo = kfCompRoleData
											});
										}
									}
									TimeUtil.AgeByNow(ref kfCompRoleData.Age);
									this.Persistence.SaveCompRoleData(kfCompRoleData.V, false, false, true, false);
								}
								break;
							}
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
		}

		
		public void SetBulletin(int compType, string bulletin)
		{
			if (compType >= 1 && compType <= 3)
			{
				try
				{
					lock (this.Mutex)
					{
						KFCompData kfCompData = null;
						if (this.CompDataDict.V.TryGetValue(compType, out kfCompData))
						{
							kfCompData.Bulletin = bulletin;
							TimeUtil.AgeByNow(ref this.CompDataDict.Age);
							this.Persistence.SaveCompData(kfCompData, true);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
		}

		
		public void BroadCastCompNotice(int serverId, byte[] bytes)
		{
			try
			{
				ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, new AsyncDataItem(KuaFuEventTypes.CompNotice, new object[]
				{
					bytes
				}), serverId);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public void CompChat(int serverId, byte[] bytes)
		{
			try
			{
				AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.CompChat, new object[]
				{
					bytes
				});
				HashSet<int> SpecialLineSet = new HashSet<int>();
				SpecialLineSet.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_1));
				SpecialLineSet.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_2));
				SpecialLineSet.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_3));
				foreach (KuaFuData<CompFuBenData> item in this.CompFuBenDataDict.Values)
				{
					SpecialLineSet.Add(item.V.ServerId);
				}
				foreach (int lineServerId in SpecialLineSet)
				{
					if (serverId != lineServerId)
					{
						ClientAgentManager.Instance().PostAsyncEvent(lineServerId, this.EvItemGameType, evItem);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public void SetRoleData4Selector(int roleId, byte[] bytes)
		{
			HashSet<int> SpecialLineSet = new HashSet<int>();
			SpecialLineSet.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_1));
			SpecialLineSet.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_2));
			SpecialLineSet.Add(KuaFuServerManager.GetSpecialLineId(GameTypes.Comp_3));
			lock (this.Mutex)
			{
				KuaFuData<KFCompRoleData> kfCompRoleData = null;
				if (this.CompRoleDataDict.TryGetValue(roleId, out kfCompRoleData))
				{
					kfCompRoleData.V.RoleData4Selector = bytes;
					TimeUtil.AgeByNow(ref kfCompRoleData.Age);
					this.Persistence.SaveCompRoleData(kfCompRoleData.V, false, false, false, false);
					AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.CompRoleDataSet, new object[]
					{
						roleId
					});
					foreach (int lineServerId in SpecialLineSet)
					{
						ClientAgentManager.Instance().PostAsyncEvent(lineServerId, this.EvItemGameType, evItem);
					}
				}
			}
		}

		
		public void UpdateMapRoleNum(int mapCode, int roleNum)
		{
			GameTypes gameTypes = GameTypes.None;
			CompConfig compConfig = null;
			CompMapData mapData = null;
			lock (this.Mutex)
			{
				compConfig = this.CompConfigDict.Values.ToList<CompConfig>().Find((CompConfig x) => x.MapCode == mapCode);
				if (null == compConfig)
				{
					return;
				}
				if (!this.CompMapDataDict.TryGetValue(mapCode, out mapData))
				{
					mapData = new CompMapData();
					this.CompMapDataDict[mapCode] = mapData;
				}
				if (compConfig.ID == 1)
				{
					gameTypes = GameTypes.Comp_1;
				}
				if (compConfig.ID == 2)
				{
					gameTypes = GameTypes.Comp_2;
				}
				if (compConfig.ID == 3)
				{
					gameTypes = GameTypes.Comp_3;
				}
			}
			int serverID = KuaFuServerManager.GetSpecialLineId(gameTypes);
			if (serverID != mapData.ServerId)
			{
				if (0 != mapData.ServerId)
				{
					ClientAgentManager.Instance().RemoveKfFuben(gameTypes, mapData.ServerId, mapData.GameId);
				}
				int toServerId = 0;
				int gameId = TianTiPersistence.Instance.GetNextGameId();
				if (ClientAgentManager.Instance().AssginKfFuben(gameTypes, (long)gameId, compConfig.MaxPlayer, out toServerId))
				{
					mapData.GameId = (long)gameId;
					mapData.Type = (byte)compConfig.ID;
					mapData.ServerId = toServerId;
				}
			}
			mapData.roleNum = roleNum;
		}

		
		private CompBattleGameStates GetCompMineGameStates(DateTime now)
		{
			CompBattleGameStates state = CompBattleGameStates.None;
			CompMineConfig sceneItem = this.CompMineConfigDict.Values.FirstOrDefault<CompMineConfig>();
			CompBattleGameStates result;
			if (null == sceneItem)
			{
				result = state;
			}
			else
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.PrepareSecs && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] + (double)(sceneItem.PrepareSecs / 2))
						{
							state = CompBattleGameStates.Prepare;
						}
						else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] + (double)sceneItem.PrepareSecs)
						{
							state = CompBattleGameStates.RankReset;
						}
						else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1] - (double)sceneItem.ClearRolesSecs)
						{
							state = CompBattleGameStates.Start;
						}
						else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
						{
							state = CompBattleGameStates.Analysis;
						}
					}
				}
				result = state;
			}
			return result;
		}

		
		private CompBattleGameStates GetCompBattleGameStates(DateTime now)
		{
			CompBattleGameStates state = CompBattleGameStates.None;
			CompBattleConfig sceneItem = this.CompBattleConfigDict.Values.FirstOrDefault<CompBattleConfig>();
			CompBattleGameStates result;
			if (null == sceneItem)
			{
				result = state;
			}
			else
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.PrepareSecs && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] + (double)(sceneItem.PrepareSecs / 2))
						{
							state = CompBattleGameStates.Prepare;
						}
						else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] + (double)sceneItem.PrepareSecs)
						{
							state = CompBattleGameStates.RankReset;
						}
						else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1] - (double)sceneItem.ClearRolesSecs)
						{
							state = CompBattleGameStates.Start;
						}
						else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
						{
							state = CompBattleGameStates.Analysis;
						}
					}
				}
				result = state;
			}
			return result;
		}

		
		private void HandleCompMineLogicSomething(DateTime now)
		{
			CompBattleGameStates state = this.GetCompMineGameStates(now);
			if (CompBattleGameStates.None != state)
			{
				int curWeekDayId = TimeUtil.GetWeekStartDayIdNow();
				if (CompBattleGameStates.RankReset == state && this.CompMineWeekDayID != curWeekDayId)
				{
					foreach (KuaFuData<KFCompRoleData> compRole in this.CompRoleDataDict.Values)
					{
						if (compRole.V.MineJiFen > 0 || compRole.V.CompTypeMine != compRole.V.CompType)
						{
							compRole.V.MineRankNum = 0;
							compRole.V.MineJiFen = 0;
							compRole.V.CompTypeMine = compRole.V.CompType;
							TimeUtil.AgeByNow(ref compRole.Age);
							this.Persistence.SaveCompRoleData(compRole.V, false, false, false, false);
						}
					}
					for (int compLoop = 1; compLoop <= 3; compLoop++)
					{
						this.CompMineJoinRoleNum[compLoop - 1] = 0;
						List<KFCompRankInfo> rankMineJiFenList = null;
						if (this.CompRankMineJiFenDict.V.TryGetValue(compLoop, out rankMineJiFenList))
						{
							rankMineJiFenList.Clear();
						}
					}
					this.CompMineJiFenRoleSet.Clear();
					foreach (KFCompData item in this.CompDataDict.V.Values)
					{
						item.MineRes = 0;
					}
					TimeUtil.AgeByNow(ref this.CompDataDict.Age);
					this.CompMineWeekDayID = curWeekDayId;
					this.Persistence.SaveCompMineWeekDayID(this.CompMineWeekDayID);
					AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.CompMineReset, new object[0]);
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, evItem, 0);
				}
				this.UpdateCompRankMineJiFen(now);
			}
		}

		
		private void HandleCompBattleLogicSomething(DateTime now)
		{
			CompBattleGameStates state = this.GetCompBattleGameStates(now);
			if (CompBattleGameStates.None != state)
			{
				int curWeekDayId = TimeUtil.GetWeekStartDayIdNow();
				if (CompBattleGameStates.RankReset == state && this.CompBattleWeekDayID != curWeekDayId)
				{
					foreach (KuaFuData<KFCompRoleData> compRole in this.CompRoleDataDict.Values)
					{
						if (compRole.V.BattleJiFen > 0 || compRole.V.CompTypeBattle != compRole.V.CompType)
						{
							compRole.V.BattleRankNum = 0;
							compRole.V.BattleJiFen = 0;
							compRole.V.CompTypeBattle = compRole.V.CompType;
							TimeUtil.AgeByNow(ref compRole.Age);
							this.Persistence.SaveCompRoleData(compRole.V, false, false, false, false);
						}
					}
					for (int compLoop = 1; compLoop <= 3; compLoop++)
					{
						this.CompBattleJoinRoleNum[compLoop - 1] = 0;
						List<KFCompRankInfo> rankBattleJiFenList = null;
						if (this.CompRankBattleJiFenDict.V.TryGetValue(compLoop, out rankBattleJiFenList))
						{
							rankBattleJiFenList.Clear();
						}
					}
					this.CompBattleJiFenRoleSet.Clear();
					this.CompBattleWeekDayID = curWeekDayId;
					this.Persistence.SaveCompBattleWeekDayID(this.CompBattleWeekDayID);
					AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.CompBattleReset, new object[0]);
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, evItem, 0);
				}
				this.UpdateCompRankBattleJiFen(now);
			}
		}

		
		private void UpdateCompRankMineJiFen(DateTime now)
		{
			KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankMineJiFenTop50 = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();
			for (int compLoop = 1; compLoop <= 3; compLoop++)
			{
				this.CompMineJoinRoleNum[compLoop - 1] = 0;
				List<KFCompRankInfo> rankMineJiFenList = null;
				if (this.CompRankMineJiFenDict.V.TryGetValue(compLoop, out rankMineJiFenList))
				{
					rankMineJiFenList.Sort(delegate(KFCompRankInfo left, KFCompRankInfo right)
					{
						int result;
						if (left.tagInfo.V.MineJiFen > right.tagInfo.V.MineJiFen)
						{
							result = -1;
						}
						else if (left.tagInfo.V.MineJiFen < right.tagInfo.V.MineJiFen)
						{
							result = 1;
						}
						else if (left.tagInfo.V.RankTmBJF < right.tagInfo.V.RankTmBJF)
						{
							result = -1;
						}
						else if (left.tagInfo.V.RankTmBJF > right.tagInfo.V.RankTmBJF)
						{
							result = 1;
						}
						else if (left.Key > right.Key)
						{
							result = -1;
						}
						else if (left.Key < right.Key)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					for (int loop = 0; loop < rankMineJiFenList.Count; loop++)
					{
						KFCompRankInfo rankInfo = rankMineJiFenList[loop];
						KuaFuData<KFCompRoleData> compRoleData = null;
						if (this.CompRoleDataDict.TryGetValue(rankInfo.Key, out compRoleData))
						{
							rankInfo.Value = compRoleData.V.MineJiFen;
							this.CompMineJiFenRoleSet.Add(rankInfo.Key);
							if (compRoleData.V.MineRankNum != loop + 1)
							{
								compRoleData.V.MineRankNum = loop + 1;
								TimeUtil.AgeByNow(ref compRoleData.Age);
							}
						}
					}
					this.CompMineJoinRoleNum[compLoop - 1] = rankMineJiFenList.Count;
					CompRankMineJiFenTop50.V[compLoop] = rankMineJiFenList.GetRange(0, Math.Min(rankMineJiFenList.Count, 50));
				}
			}
			TimeUtil.AgeByNow(ref this.CompRankMineJiFenDict.Age);
			CompRankMineJiFenTop50.Age = this.CompRankMineJiFenDict.Age;
			this.BytesCompRankMineJiFenDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(CompRankMineJiFenTop50);
		}

		
		private void UpdateCompRankBattleJiFen(DateTime now)
		{
			KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankBattleJiFenTop50 = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();
			for (int compLoop = 1; compLoop <= 3; compLoop++)
			{
				this.CompBattleJoinRoleNum[compLoop - 1] = 0;
				List<KFCompRankInfo> rankBattleJiFenList = null;
				if (this.CompRankBattleJiFenDict.V.TryGetValue(compLoop, out rankBattleJiFenList))
				{
					rankBattleJiFenList.Sort(delegate(KFCompRankInfo left, KFCompRankInfo right)
					{
						int result;
						if (left.tagInfo.V.BattleJiFen > right.tagInfo.V.BattleJiFen)
						{
							result = -1;
						}
						else if (left.tagInfo.V.BattleJiFen < right.tagInfo.V.BattleJiFen)
						{
							result = 1;
						}
						else if (left.tagInfo.V.RankTmBJF < right.tagInfo.V.RankTmBJF)
						{
							result = -1;
						}
						else if (left.tagInfo.V.RankTmBJF > right.tagInfo.V.RankTmBJF)
						{
							result = 1;
						}
						else if (left.Key > right.Key)
						{
							result = -1;
						}
						else if (left.Key < right.Key)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					for (int loop = 0; loop < rankBattleJiFenList.Count; loop++)
					{
						KFCompRankInfo rankInfo = rankBattleJiFenList[loop];
						KuaFuData<KFCompRoleData> compRoleData = null;
						if (this.CompRoleDataDict.TryGetValue(rankInfo.Key, out compRoleData))
						{
							rankInfo.Value = compRoleData.V.BattleJiFen;
							this.CompBattleJiFenRoleSet.Add(rankInfo.Key);
							if (compRoleData.V.BattleRankNum != loop + 1)
							{
								compRoleData.V.BattleRankNum = loop + 1;
								TimeUtil.AgeByNow(ref compRoleData.Age);
							}
						}
					}
					this.CompBattleJoinRoleNum[compLoop - 1] = rankBattleJiFenList.Count;
					CompRankBattleJiFenTop50.V[compLoop] = rankBattleJiFenList.GetRange(0, Math.Min(rankBattleJiFenList.Count, 50));
				}
			}
			TimeUtil.AgeByNow(ref this.CompRankBattleJiFenDict.Age);
			CompRankBattleJiFenTop50.Age = this.CompRankBattleJiFenDict.Age;
			this.BytesCompRankBattleJiFenDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(CompRankBattleJiFenTop50);
		}

		
		private void CheckRoleTimerProc(DateTime now)
		{
			if (this.CompFuBenDataDict != null && this.CompFuBenDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					foreach (KuaFuData<CompFuBenData> fuBenData in this.CompFuBenDataDict.Values)
					{
						List<int> callist = new List<int>(new int[fuBenData.V.EnterGameRoleCount.Count]);
						List<int> list = new List<int>();
						foreach (CompFuBenRoleData roleData in fuBenData.V.RoleDict.Values)
						{
							if (roleData.State == KuaFuRoleStates.EnterGame)
							{
								if (roleData.StateEndTime < now)
								{
									roleData.State = KuaFuRoleStates.None;
									list.Add(roleData.RoleId);
								}
								else
								{
									List<int> list2;
									int index;
									(list2 = callist)[index = roleData.Side - 1] = list2[index] + 1;
								}
							}
						}
						fuBenData.V.EnterGameRoleCount = callist;
						foreach (int key in list)
						{
							fuBenData.V.RoleDict.Remove(key);
						}
						TimeUtil.AgeByNow(ref fuBenData.Age);
					}
				}
			}
		}

		
		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.CompFuBenDataDict != null && this.CompFuBenDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
					foreach (KeyValuePair<KeyValuePair<int, int>, KuaFuData<CompFuBenData>> fuBenData in this.CompFuBenDataDict)
					{
						if (fuBenData.Value.V.EndTime < now)
						{
							list.Add(fuBenData.Key);
							LogManager.WriteLog(LogTypes.Error, string.Format("势力战场副本数据清除 gameType={0} gameId={1} state={2} endtm={3}", new object[]
							{
								fuBenData.Key.Key,
								fuBenData.Value.V.GameId,
								fuBenData.Value.V.State,
								fuBenData.Value.V.EndTime
							}), null, true);
							GameTypes gameType = (GameTypes)fuBenData.Key.Key;
							ClientAgentManager.Instance().RemoveKfFuben(gameType, fuBenData.Value.V.ServerId, (long)fuBenData.Value.V.GameId);
						}
					}
					foreach (KeyValuePair<int, int> key in list)
					{
						this.CompFuBenDataDict.Remove(key);
					}
				}
			}
		}

		
		public void UpdateFuBenMapRoleNum(int gameType, CompFuBenData fubenItem)
		{
			try
			{
				lock (this.Mutex)
				{
					if (fubenItem.RoleCountSideList != null && fubenItem.RoleCountSideList.Count > 0)
					{
						KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(gameType, fubenItem.GameId);
						KuaFuData<CompFuBenData> fubenData = null;
						if (this.CompFuBenDataDict.TryGetValue(kvpKey, out fubenData))
						{
							fubenData.V.RoleCountSideList = fubenItem.RoleCountSideList;
							fubenData.V.ZhuJiangRoleDict = fubenItem.ZhuJiangRoleDict;
							fubenData.V.MineTruckGo = fubenItem.MineTruckGo;
							fubenData.V.MineSafeArrived = fubenItem.MineSafeArrived;
							TimeUtil.AgeByNow(ref fubenData.Age);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public void UpdateStrongholdData(int cityID, List<CompStrongholdData> shDataList)
		{
			try
			{
				lock (this.Mutex)
				{
					if (shDataList.Count == 3)
					{
						for (int compLoop = 1; compLoop <= 3; compLoop++)
						{
							KFCompData kfCompData = null;
							if (this.CompDataDict.V.TryGetValue(compLoop, out kfCompData))
							{
								kfCompData.StrongholdDict[cityID] = shDataList[compLoop - 1];
								this.Persistence.SaveCompData(kfCompData, true);
							}
						}
						TimeUtil.AgeByNow(ref this.CompDataDict.Age);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public int GameFuBenRoleChangeState(int gameType, int serverId, int cityID, int roleId, int zhiwu, int state)
		{
			lock (this.Mutex)
			{
				CompBattleConfig battleSceneInfo = null;
				if (!this.CompBattleConfigDict.TryGetValue(cityID, out battleSceneInfo))
				{
					return -11003;
				}
				KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(gameType, cityID);
				KuaFuData<CompFuBenData> fubenData = null;
				if (this.CompFuBenDataDict == null || !this.CompFuBenDataDict.TryGetValue(kvpKey, out fubenData))
				{
					return -11003;
				}
				if (roleId < 0 && 6 == state)
				{
					fubenData.V.State = GameFuBenState.End;
					TimeUtil.AgeByNow(ref fubenData.Age);
					return state;
				}
				KuaFuData<KFCompRoleData> compRoleData = null;
				if (!this.CompRoleDataDict.TryGetValue(roleId, out compRoleData))
				{
					return -11003;
				}
				CompFuBenRoleData kroleData = null;
				if (!fubenData.V.RoleDict.TryGetValue(roleId, out kroleData))
				{
					kroleData = new CompFuBenRoleData
					{
						ServerId = serverId,
						RoleId = roleId,
						KuaFuServerId = fubenData.V.ServerId,
						KuaFuMapCode = fubenData.V.GameId,
						Side = compRoleData.V.CompType,
						State = KuaFuRoleStates.None,
						ZhiWu = zhiwu
					};
					fubenData.V.RoleDict[roleId] = kroleData;
				}
				if (4 == state)
				{
					if (fubenData.V.GetRoleCountWithEnter(kroleData.Side) >= battleSceneInfo.MaxEnterNum)
					{
						return -22;
					}
					if (kroleData.State == KuaFuRoleStates.None || kroleData.State == KuaFuRoleStates.Offline)
					{
						List<int> list;
						int index;
						(list = fubenData.V.EnterGameRoleCount)[index = kroleData.Side - 1] = list[index] + 1;
						kroleData.StateEndTime = Global.NowTime.AddMinutes(1.0);
					}
					fubenData.V.State = GameFuBenState.Start;
				}
				else if (5 == state)
				{
					List<int> list;
					int index;
					if (kroleData.State == KuaFuRoleStates.EnterGame)
					{
						(list = fubenData.V.EnterGameRoleCount)[index = kroleData.Side - 1] = list[index] - 1;
					}
					(list = fubenData.V.RoleCountSideList)[index = kroleData.Side - 1] = list[index] + 1;
					if (zhiwu > 0)
					{
						fubenData.V.ZhuJiangRoleDict[kroleData.Side].Add(roleId);
					}
				}
				else if (7 == state)
				{
					List<int> list;
					int index;
					(list = fubenData.V.RoleCountSideList)[index = kroleData.Side - 1] = list[index] - 1;
					if (zhiwu > 0)
					{
						fubenData.V.ZhuJiangRoleDict[kroleData.Side].Remove(roleId);
					}
				}
				kroleData.State = (KuaFuRoleStates)state;
				TimeUtil.AgeByNow(ref fubenData.Age);
			}
			return state;
		}

		
		public KuaFuCmdData GetKuaFuFuBenData(int gameType, int cityID, long dataAge)
		{
			KuaFuCmdData result;
			if (30 != gameType && 31 != gameType)
			{
				result = null;
			}
			else
			{
				try
				{
					lock (this.Mutex)
					{
						DateTime now = TimeUtil.NowDateTime();
						int TotalSecs = 0;
						int MaxEnterNum = 0;
						int MapCode = 0;
						if (30 == gameType)
						{
							CompBattleGameStates state = this.GetCompBattleGameStates(now);
							if (CompBattleGameStates.None == state)
							{
								return null;
							}
							CompBattleConfig battleSceneInfo = null;
							if (!this.CompBattleConfigDict.TryGetValue(cityID, out battleSceneInfo))
							{
								return null;
							}
							TotalSecs = battleSceneInfo.TotalSecs;
							MaxEnterNum = battleSceneInfo.MaxEnterNum;
							MapCode = battleSceneInfo.MapCode;
						}
						else if (31 == gameType)
						{
							CompBattleGameStates state = this.GetCompMineGameStates(now);
							if (CompBattleGameStates.None == state)
							{
								return null;
							}
							CompMineConfig mineSceneInfo = null;
							if (!this.CompMineConfigDict.TryGetValue(cityID, out mineSceneInfo))
							{
								return null;
							}
							TotalSecs = mineSceneInfo.TotalSecs;
							MaxEnterNum = mineSceneInfo.MaxEnterNum;
							MapCode = mineSceneInfo.MapCode;
						}
						KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(gameType, cityID);
						KuaFuData<CompFuBenData> KFuBenData = null;
						if (!this.CompFuBenDataDict.TryGetValue(kvpKey, out KFuBenData))
						{
							KFuBenData = new KuaFuData<CompFuBenData>();
							KFuBenData.V.Init();
							KFuBenData.V.GameId = cityID;
							KFuBenData.V.State = GameFuBenState.Wait;
							KFuBenData.V.EndTime = Global.NowTime.AddMinutes((double)TotalSecs);
							if (!ClientAgentManager.Instance().AssginKfFuben((GameTypes)gameType, (long)cityID, MaxEnterNum * 3, out KFuBenData.V.ServerId))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("势力战分配游戏服务器失败 gameType={0}, mapCode={1}", gameType, MapCode), null, true);
								return null;
							}
							this.CompFuBenDataDict[kvpKey] = KFuBenData;
							TimeUtil.AgeByNow(ref KFuBenData.Age);
						}
						if (dataAge != KFuBenData.Age)
						{
							return new KuaFuCmdData
							{
								Age = KFuBenData.Age,
								Bytes0 = DataHelper2.ObjectToBytes<CompFuBenData>(KFuBenData.V)
							};
						}
						return new KuaFuCmdData
						{
							Age = KFuBenData.Age
						};
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = null;
			}
			return result;
		}

		
		private static CompService _instance = new CompService();

		
		public readonly GameTypes EvItemGameType = GameTypes.TianTi;

		
		private object Mutex = new object();

		
		private int LastUpdateDayID;

		
		private int LastUpdateHour;

		
		private int CompDataDayID;

		
		private int CompDataWeekDayID;

		
		private int CompBattleWeekDayID;

		
		private int CompMineWeekDayID;

		
		private DateTime CheckTime10;

		
		public CompPersistence Persistence = CompPersistence.Instance;

		
		private Dictionary<int, CompMapData> CompMapDataDict = new Dictionary<int, CompMapData>();

		
		private Dictionary<KeyValuePair<int, int>, KuaFuData<CompFuBenData>> CompFuBenDataDict = new Dictionary<KeyValuePair<int, int>, KuaFuData<CompFuBenData>>();

		
		private HashSet<int> CompBattleJiFenRoleSet = new HashSet<int>();

		
		private HashSet<int> CompMineJiFenRoleSet = new HashSet<int>();

		
		private int[] CompBattleJoinRoleNum = new int[3];

		
		private int[] CompMineJoinRoleNum = new int[3];

		
		public byte[] BytesCompRankBattleJiFenDict = null;

		
		public byte[] BytesCompRankMineJiFenDict = null;

		
		public Dictionary<int, CompConfig> CompConfigDict = new Dictionary<int, CompConfig>();

		
		public Dictionary<int, CompBattleConfig> CompBattleConfigDict = new Dictionary<int, CompBattleConfig>();

		
		public Dictionary<int, CompMineConfig> CompMineConfigDict = new Dictionary<int, CompMineConfig>();

		
		private double CompReplaceAmerce;

		
		private int CompBoomValueReduce;

		
		private int CompBoomValueMin;
	}
}
