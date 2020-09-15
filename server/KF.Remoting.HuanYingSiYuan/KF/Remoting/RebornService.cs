using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x02000071 RID: 113
	public class RebornService
	{
		// Token: 0x06000598 RID: 1432 RVA: 0x0004B584 File Offset: 0x00049784
		public static RebornService Instance()
		{
			return RebornService._instance;
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x0004B59C File Offset: 0x0004979C
		// (set) Token: 0x0600059A RID: 1434 RVA: 0x0004B5B9 File Offset: 0x000497B9
		private Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>> RebornRoleDataDict
		{
			get
			{
				return this.Persistence.RebornRoleDataDict;
			}
			set
			{
				this.Persistence.RebornRoleDataDict = value;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x0004B5C8 File Offset: 0x000497C8
		// (set) Token: 0x0600059C RID: 1436 RVA: 0x0004B5E5 File Offset: 0x000497E5
		public KuaFuData<Dictionary<int, List<KFRebornRankInfo>>> RebornRankDict
		{
			get
			{
				return this.Persistence.RebornRankDict;
			}
			set
			{
				this.Persistence.RebornRankDict = value;
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0004B63C File Offset: 0x0004983C
		public void InitConfig()
		{
			try
			{
				lock (this.Mutex)
				{
					string fileName = "Config/RebornBoss.xml";
					string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.RebornBossConfigDict.Clear();
					XElement xml = ConfigHelper.Load(fullPathFileName);
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						int MapCodeID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MapID", 0L);
						List<RebornBossConfig> bossList;
						if (!this.RebornBossConfigDict.TryGetValue(MapCodeID, out bossList))
						{
							bossList = new List<RebornBossConfig>();
							this.RebornBossConfigDict[MapCodeID] = bossList;
						}
						RebornBossConfig myData = new RebornBossConfig();
						myData.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
						myData.MapID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MapID", 0L);
						myData.MonstersID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MonstersID", 0L);
						bossList.Add(myData);
					}
					foreach (List<RebornBossConfig> value in this.RebornBossConfigDict.Values)
					{
						value.Sort(delegate(RebornBossConfig left, RebornBossConfig right)
						{
							int result;
							if (left.ID < right.ID)
							{
								result = -1;
							}
							else if (left.ID > right.ID)
							{
								result = 1;
							}
							else
							{
								result = 0;
							}
							return result;
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x0004B860 File Offset: 0x00049A60
		public void LoadDatabase(DateTime now)
		{
			try
			{
				this.Persistence.LoadDatabase();
				this.LastUpdateDayID = TimeUtil.GetOffsetDay(now);
				this.LastUpdateHour = now.Hour;
				this.RebornDataDayID = this.Persistence.GetRebornDayID();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "RebornService.LoadDatabase failed!", ex, true);
			}
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0004B8CC File Offset: 0x00049ACC
		public void OnStopServer()
		{
			this.Persistence.DelayWriteDataProc();
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0004B8DC File Offset: 0x00049ADC
		public void Update(DateTime now)
		{
			try
			{
				this.Persistence.DelayWriteDataProc();
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
				LogManager.WriteLog(LogTypes.Error, "RebornService.Update failed!", ex, true);
			}
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0004B96C File Offset: 0x00049B6C
		private void HandleChangeHour(DateTime now)
		{
			lock (this.Mutex)
			{
				this.Persistence.LoadRebornRankInfo(0, this.RebornRankDict);
			}
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x0004B9C4 File Offset: 0x00049BC4
		private void HandleChangeDay(DateTime now)
		{
			int curDayId = TimeUtil.GetOffsetDay(now);
			if (0 == this.RebornDataDayID)
			{
				this.RebornDataDayID = curDayId;
				this.Persistence.SaveRebornDayID(this.RebornDataDayID);
			}
			else
			{
				lock (this.Mutex)
				{
					if (curDayId != this.RebornDataDayID)
					{
						foreach (KuaFuData<KFRebornRoleData> item in this.RebornRoleDataDict.Values)
						{
							item.V.RarityLast = item.V.Rarity;
							item.V.Rarity = 0;
							item.V.BossLast = item.V.Boss;
							item.V.Boss = 0;
							item.V.LianShaLast = item.V.LianSha;
							item.V.LianSha = 0;
							this.Persistence.UpdateRebornRoleData(item.V, 84, false);
						}
						for (int rankType = 0; rankType <= 3; rankType++)
						{
							if (rankType != 0)
							{
								this.Persistence.LoadRebornRankInfo(rankType, this.RebornRankDict);
							}
						}
						this.RebornDataDayID = curDayId;
						this.Persistence.SaveRebornDayID(this.RebornDataDayID);
					}
				}
			}
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0004BB90 File Offset: 0x00049D90
		public void SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			lock (this.Mutex)
			{
				KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
				if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptId, roleId), out kfRebornRoleData))
				{
					kfRebornRoleData.V.RoleData4Selector = bytes;
					TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
					this.Persistence.UpdateRebornRoleData4Selector(kfRebornRoleData.V);
				}
			}
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x0004BC1C File Offset: 0x00049E1C
		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			try
			{
				RebornSyncData SyncData = new RebornSyncData();
				lock (this.Mutex)
				{
					SyncData.RebornRankDict.Age = this.RebornRankDict.Age;
					SyncData.BossRefreshDict.Age = this.BossRefreshDict.Age;
					if (ageRank != this.RebornRankDict.Age)
					{
						SyncData.BytesRebornRankDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<int, List<KFRebornRankInfo>>>>(this.RebornRankDict);
					}
					if (ageBoss != this.BossRefreshDict.Age)
					{
						SyncData.BytesRebornBossRefreshDict = DataHelper2.ObjectToBytes<KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>>>(this.BossRefreshDict);
					}
				}
				return SyncData;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0004BD10 File Offset: 0x00049F10
		public KuaFuCmdData GetRebornRoleData(int ptId, int roleId, long dataAge)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
					if (!this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptId, roleId), out kfRebornRoleData))
					{
						return null;
					}
					if (dataAge != kfRebornRoleData.Age)
					{
						return new KuaFuCmdData
						{
							Age = kfRebornRoleData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KFRebornRoleData>(kfRebornRoleData.V)
						};
					}
					return new KuaFuCmdData
					{
						Age = kfRebornRoleData.Age
					};
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x0004BDF0 File Offset: 0x00049FF0
		public int RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			int ret = 0;
			try
			{
				KeyValuePair<int, int> key = new KeyValuePair<int, int>(ptId, roleId);
				KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
				if (!this.RebornRoleDataDict.TryGetValue(key, out kfRebornRoleData))
				{
					kfRebornRoleData = new KuaFuData<KFRebornRoleData>();
					kfRebornRoleData.V.PtID = ptId;
					kfRebornRoleData.V.RoleID = roleId;
					kfRebornRoleData.V.RoleName = roleName;
					kfRebornRoleData.V.Lev = level;
					this.RebornRoleDataDict[key] = kfRebornRoleData;
					TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
					this.Persistence.InsertRebornRoleData(kfRebornRoleData.V);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return ret;
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x0004BEF0 File Offset: 0x0004A0F0
		public void ChangeName(int ptId, int roleId, string roleName)
		{
			try
			{
				lock (this.Mutex)
				{
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(ptId, roleId);
					KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
					if (this.RebornRoleDataDict.TryGetValue(key, out kfRebornRoleData))
					{
						kfRebornRoleData.V.RoleName = roleName;
						TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
						this.Persistence.UpdateRebornRoleDataRoleName(kfRebornRoleData.V);
						bool refreshRank = false;
						foreach (KeyValuePair<int, List<KFRebornRankInfo>> kvp in this.RebornRankDict.V)
						{
							KFRebornRankInfo item = kvp.Value.Find((KFRebornRankInfo x) => x.PtID == ptId && x.Key == roleId);
							if (null != item)
							{
								string worldRoleID = ConstData.FormatWorldRoleID(roleId, ptId);
								KuaFuWorldRoleData worldRoleData = TSingleton<KuaFuWorldManager>.getInstance().LoadKuaFuWorldRoleData(roleId, ptId, worldRoleID);
								if (null != worldRoleData)
								{
									item.Param1 = KuaFuServerManager.FormatName(item.tagInfo.V.RoleName, worldRoleData.ZoneID);
									refreshRank = true;
								}
							}
						}
						if (refreshRank)
						{
							TimeUtil.AgeByNow(ref this.RebornRankDict.Age);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0004C194 File Offset: 0x0004A394
		public void RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			try
			{
				lock (this.Mutex)
				{
					switch (optType)
					{
					case 0:
					{
						KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kfRebornRoleData))
						{
							kfRebornRoleData.V.Lev = param1;
							TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
							this.Persistence.UpdateRebornRoleData(kfRebornRoleData.V, 1, true);
						}
						break;
					}
					case 1:
					{
						KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kfRebornRoleData))
						{
							kfRebornRoleData.V.Rarity += param1;
							TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
							this.Persistence.UpdateRebornRoleData(kfRebornRoleData.V, 2, true);
						}
						break;
					}
					case 2:
					{
						KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kfRebornRoleData))
						{
							kfRebornRoleData.V.Boss += param1;
							TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
							this.Persistence.UpdateRebornRoleData(kfRebornRoleData.V, 8, true);
						}
						break;
					}
					case 3:
					{
						KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kfRebornRoleData))
						{
							if (param1 > kfRebornRoleData.V.LianSha)
							{
								kfRebornRoleData.V.LianSha = param1;
								TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
								this.Persistence.UpdateRebornRoleData(kfRebornRoleData.V, 32, true);
							}
						}
						break;
					}
					case 4:
					{
						KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kfRebornRoleData))
						{
							KFRebornBossAwardData myData = new KFRebornBossAwardData();
							myData.MapCodeID = param1;
							myData.LineID = param2;
							string[] fields = param3.Split(new char[]
							{
								','
							});
							if (fields.Length == 2)
							{
								myData.ExtensionID = Global.SafeConvertToInt32(fields[0]);
								myData.RankNum = Global.SafeConvertToInt32(fields[1]);
							}
							KFRebornBossAwardData awardData = kfRebornRoleData.V.BossAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == myData.MapCodeID && x.LineID == myData.LineID);
							if (null != awardData)
							{
								awardData.MapCodeID = myData.MapCodeID;
								awardData.LineID = myData.LineID;
								awardData.ExtensionID = myData.ExtensionID;
								awardData.RankNum = myData.RankNum;
							}
							else
							{
								kfRebornRoleData.V.BossAwardList.Add(myData);
							}
							TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
							this.Persistence.UpdateRebornRoleDataBossAward(kfRebornRoleData.V);
						}
						break;
					}
					case 5:
					{
						KuaFuData<KFRebornRoleData> kfRebornRoleData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(ptid, rid), out kfRebornRoleData))
						{
							kfRebornRoleData.V.BossAwardList.RemoveAll((KFRebornBossAwardData x) => x.MapCodeID == param1 && x.LineID == param2 && x.ExtensionID == Global.SafeConvertToInt32(param3));
							TimeUtil.AgeByNow(ref kfRebornRoleData.Age);
							this.Persistence.UpdateRebornRoleDataBossAward(kfRebornRoleData.V);
						}
						break;
					}
					case 6:
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(param1, param2);
						KFRebornBossRefreshData myData2;
						if (!this.BossRefreshDict.V.TryGetValue(key, out myData2))
						{
							myData2 = new KFRebornBossRefreshData();
							this.BossRefreshDict.V[key] = myData2;
						}
						myData2.MapCodeID = param1;
						myData2.LineID = param2;
						string[] fields = param3.Split(new char[]
						{
							','
						});
						if (fields.Length == 2)
						{
							myData2.ExtensionID = Global.SafeConvertToInt32(fields[0]);
							myData2.NextTime = fields[1];
						}
						TimeUtil.AgeByNow(ref this.BossRefreshDict.Age);
						break;
					}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0004C698 File Offset: 0x0004A898
		public void PlatFormChat(int serverId, byte[] bytes)
		{
			try
			{
				AsyncDataItem evItem = new AsyncDataItem(KuaFuEventTypes.PlatFormChat, new object[]
				{
					bytes
				});
				HashSet<int> SpecialLineSet = new HashSet<int>();
				lock (this.Mutex)
				{
					foreach (KeyValuePair<int, List<RebornBossConfig>> item in this.RebornBossConfigDict)
					{
						List<KuaFuLineData> lineList = KuaFuServerManager.GetKuaFuLineDataList(item.Key);
						if (null != lineList)
						{
							foreach (KuaFuLineData line in lineList)
							{
								SpecialLineSet.Add(line.ServerId);
							}
						}
					}
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

		// Token: 0x04000307 RID: 775
		private static RebornService _instance = new RebornService();

		// Token: 0x04000308 RID: 776
		public readonly GameTypes EvItemGameType = GameTypes.KuaFuWorld;

		// Token: 0x04000309 RID: 777
		private object Mutex = new object();

		// Token: 0x0400030A RID: 778
		private int LastUpdateDayID;

		// Token: 0x0400030B RID: 779
		private int LastUpdateHour;

		// Token: 0x0400030C RID: 780
		private int RebornDataDayID;

		// Token: 0x0400030D RID: 781
		public RebornPersistence Persistence = RebornPersistence.Instance;

		// Token: 0x0400030E RID: 782
		public KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>> BossRefreshDict = new KuaFuData<Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData>>();

		// Token: 0x0400030F RID: 783
		public Dictionary<int, List<RebornBossConfig>> RebornBossConfigDict = new Dictionary<int, List<RebornBossConfig>>();
	}
}
