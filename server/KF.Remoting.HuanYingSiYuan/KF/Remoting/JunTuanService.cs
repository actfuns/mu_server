using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	// Token: 0x02000025 RID: 37
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, UseSynchronizationContext = true)]
	public class JunTuanService : MarshalByRefObject, IJunTuanService, IExecCommand
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00012724 File Offset: 0x00010924
		private object Mutex
		{
			get
			{
				return this.Persistence.Mutex;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00012744 File Offset: 0x00010944
		// (set) Token: 0x06000159 RID: 345 RVA: 0x00012761 File Offset: 0x00010961
		private KuaFuCmdData JunTuanBaseDataListCmdData
		{
			get
			{
				return this.Persistence.JunTuanBaseDataListCmdData;
			}
			set
			{
				this.Persistence.JunTuanBaseDataListCmdData = value;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00012770 File Offset: 0x00010970
		// (set) Token: 0x0600015B RID: 347 RVA: 0x0001278D File Offset: 0x0001098D
		private KuaFuCmdData JunTuanMiniDataListCmdData
		{
			get
			{
				return this.Persistence.JunTuanMiniDataListCmdData;
			}
			set
			{
				this.Persistence.JunTuanMiniDataListCmdData = value;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600015C RID: 348 RVA: 0x0001279C File Offset: 0x0001099C
		// (set) Token: 0x0600015D RID: 349 RVA: 0x000127B9 File Offset: 0x000109B9
		private KuaFuCmdData JunTuanRankDataListCmdData
		{
			get
			{
				return this.Persistence.JunTuanRankDataListCmdData;
			}
			set
			{
				this.Persistence.JunTuanRankDataListCmdData = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600015E RID: 350 RVA: 0x000127C8 File Offset: 0x000109C8
		// (set) Token: 0x0600015F RID: 351 RVA: 0x000127E5 File Offset: 0x000109E5
		private Dictionary<int, JunTuanDetailData> JunTuanAllDataDict
		{
			get
			{
				return this.Persistence.JunTuanAllDataDict;
			}
			set
			{
				this.Persistence.JunTuanAllDataDict = value;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000160 RID: 352 RVA: 0x000127F4 File Offset: 0x000109F4
		// (set) Token: 0x06000161 RID: 353 RVA: 0x00012811 File Offset: 0x00010A11
		private Dictionary<int, int> BangHuiJunTuanIdDict
		{
			get
			{
				return this.Persistence.BangHuiJunTuanIdDict;
			}
			set
			{
				this.Persistence.BangHuiJunTuanIdDict = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00012820 File Offset: 0x00010A20
		// (set) Token: 0x06000163 RID: 355 RVA: 0x0001283D File Offset: 0x00010A3D
		private Dictionary<int, JunTuanBangHuiData> JunTuanBangHuiDataDict
		{
			get
			{
				return this.Persistence.JunTuanBangHuiDataDict;
			}
			set
			{
				this.Persistence.JunTuanBangHuiDataDict = value;
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0001284C File Offset: 0x00010A4C
		public override object InitializeLifetimeService()
		{
			return null;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00012860 File Offset: 0x00010A60
		public JunTuanService()
		{
			JunTuanService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		// Token: 0x06000166 RID: 358 RVA: 0x000128E4 File Offset: 0x00010AE4
		~JunTuanService()
		{
			this.BackgroundThread.Abort();
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0001291C File Offset: 0x00010B1C
		public void ThreadProc(object state)
		{
			do
			{
				Thread.Sleep(1000);
			}
			while (!this.Persistence.Initialized);
			for (;;)
			{
				try
				{
					DateTime now = TimeUtil.NowDateTime();
					Global.UpdateNowTime(now);
					if (now.TimeOfDay.TotalMinutes < 5.0)
					{
						this.Persistence.CheckJunTuanPoint();
					}
					if (now > this.CheckTime20)
					{
						this.CheckTime20 = now.AddSeconds(20.0);
						this.Persistence.UpdateJunTuanTaskList();
						this.CheckRoleTimerProc(now);
						JunTuanEraService.Instance().HandleChangeEraID(now, true);
					}
					if (now > this.CheckTimer120)
					{
						this.CheckTimer120 = now.AddSeconds(120.0);
						this.Persistence.CheckJunTuanBangHuiList();
						int count = 12;
						lock (this.Mutex)
						{
							foreach (KeyValuePair<int, int> kv in this.BangHuiJunTuanIdDict)
							{
								if (kv.Value > 0)
								{
									int dayId;
									if (!this.BangHuiId2RoleDataUpdateDayIdDict.TryGetValue(kv.Key, out dayId) || dayId != now.Day)
									{
										this.BangHuiId2RoleDataUpdateDayIdDict[kv.Key] = now.Day;
										ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.UpdateJunTuanRoleDataList, new object[]
										{
											kv.Key
										}), 0);
										if (count-- <= 0)
										{
											break;
										}
									}
								}
							}
							this.CheckGameFuBenTimerProc(now);
						}
					}
					if (this.ExecPaiHang)
					{
						this.ExecPaiHang = false;
						this.Persistence.UpdateJunTuanRankDataList();
					}
					if (now > this.CheckTimer3600)
					{
						this.CheckTimer3600 = now.AddSeconds(3600.0);
						this.Persistence.UpdateJunTuanRankDataList();
						this.Persistence.UpdateJunTuanBaseDataList();
						YaoSaiService.Instance().CheckYaoSaiPrisonTimerProc(now);
					}
					this.Persistence.DelayWriteDataProc();
					int sleepMS = (int)(TimeUtil.NowDateTime() - now).TotalMilliseconds;
					this.Persistence.SaveCostTime(sleepMS);
					sleepMS = 1000 - sleepMS;
					if (sleepMS < 50)
					{
						sleepMS = 50;
					}
					Thread.Sleep(sleepMS);
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00012C68 File Offset: 0x00010E68
		public AsyncDataItem[] GetClientCacheItems(int serverId, long dataAge)
		{
			AsyncDataItem[] array = ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
			AsyncDataItem item = HongBaoManager_K.getInstance().GetHongBaoDataList(dataAge);
			AsyncDataItem[] result2;
			if (item != null)
			{
				if (null != array)
				{
					AsyncDataItem[] result = new AsyncDataItem[array.Length + 1];
					array.CopyTo(result, 1);
					result[0] = item;
					result2 = result;
				}
				else
				{
					result2 = new AsyncDataItem[]
					{
						item
					};
				}
			}
			else
			{
				result2 = array;
			}
			return result2;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00012CE0 File Offset: 0x00010EE0
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == 21 && clientInfo.ServerId != 0)
				{
					result = ClientAgentManager.Instance().InitializeClient(clientInfo);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1}", clientInfo.ServerId, clientInfo.ClientId));
				result = -11003;
			}
			return result;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00012D90 File Offset: 0x00010F90
		public int CreateJunTuan(byte[] cmdBytes)
		{
			try
			{
				JunTuanRequestData data = DataHelper2.BytesToObject<JunTuanRequestData>(cmdBytes, 0, cmdBytes.Length);
				string junTuanName = data.JunTuanName;
				lock (this.Mutex)
				{
					int junTuanId;
					if (this.BangHuiJunTuanIdDict.TryGetValue(data.BhId, out junTuanId) && junTuanId > 0)
					{
						return -1020;
					}
					JunTuanBangHuiData bhData;
					if (!this.JunTuanBangHuiDataDict.TryGetValue(data.BhId, out bhData))
					{
						bhData = new JunTuanBangHuiData
						{
							BhId = data.BhId
						};
						this.JunTuanBangHuiDataDict[bhData.BhId] = bhData;
					}
					bhData.BhId = data.BhId;
					bhData.BhName = data.BhName;
					bhData.BhZoneId = data.BhZoneId;
					bhData.LeaderName = data.LeaderName;
					bhData.LeaderZoneId = data.LeaderZoneId;
					bhData.RoleNum = data.RoleNum;
					bhData.ZhanLi = data.ZhanLi;
					bhData.LeaderOccupation = data.Occupation;
					bhData.LeaderRoleId = data.LeaderRoleId;
					long nowTicks = TimeUtil.NOW();
					if (nowTicks - bhData.LastCreateTicks < (long)(this.Persistence.RuntimeData.LegionsCreateCD * 3600000))
					{
						return -2007;
					}
					int initPoint = this.Persistence.RuntimeData.LegionProsperityCost[0];
					junTuanId = (int)this.Persistence.CreateJunTuan(junTuanName, "", data.BhZoneId, data.LeaderName, initPoint, TimeUtil.GetOffsetDay2(TimeUtil.NowDateTime()));
					if (junTuanId <= 0)
					{
						return -1023;
					}
					bhData.JuTuanZhiWu = 1;
					if (this.Persistence.UpdateJunTuanBangHuiData(bhData, junTuanId) < 0)
					{
						return -15;
					}
					bhData.LastCreateTicks = nowTicks;
					JunTuanDetailData detailData = this.JuntuanAdd(new JunTuanData
					{
						JunTuanId = junTuanId,
						JunTuanName = junTuanName,
						Point = initPoint,
						LeaderName = data.LeaderName,
						LeaderZoneId = data.LeaderZoneId,
						BangHuiNum = 1
					}, bhData);
					string message = "";
					JunTuanEventLog log = new JunTuanEventLog
					{
						EventType = 0,
						Message = message,
						Time = TimeUtil.NowDateTime()
					};
					this.Persistence.AddJuntuanEventLog(detailData, log);
					JunTuanBangHuiMiniData bhjtData = new JunTuanBangHuiMiniData
					{
						BhId = data.BhId,
						JunTuanId = junTuanId,
						JunTuanName = junTuanName,
						JunTuanZhiWu = 1,
						JunTuanChanged = 1
					};
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.UpdateBhJunTuan, new object[]
					{
						bhjtData
					}), 0);
					return junTuanId;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11003;
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000130F4 File Offset: 0x000112F4
		public int ChangeJunTuanBulltin(int bhid, int junTuanId, string bulltin)
		{
			lock (this.Mutex)
			{
				int realJunTuanId;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out realJunTuanId) || realJunTuanId != junTuanId)
				{
					return -1020;
				}
				JunTuanDetailData detailData;
				bool flag2;
				if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
				{
					flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
				}
				else
				{
					flag2 = false;
				}
				if (!flag2)
				{
					return -1024;
				}
				detailData.JunTuanData.V.Bulletin = bulltin;
				TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
				DbHelperMySQL.ExecuteSql(string.Format("update t_juntuan set bulletin='{1}' where juntuanid={0}", junTuanId, bulltin));
			}
			return 0;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00013234 File Offset: 0x00011434
		public int ChangeJunTuanGVoicePrioritys(int bhid, string prioritys)
		{
			lock (this.Mutex)
			{
				int junTuanId;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out junTuanId))
				{
					return -1025;
				}
				JunTuanDetailData detailData;
				bool flag2;
				if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
				{
					flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
				}
				else
				{
					flag2 = false;
				}
				if (!flag2)
				{
					return -1024;
				}
				string leader = detailData.JunTuanData.V.LeaderRoleId.ToString();
				HashSet<string> rids = new HashSet<string>();
				if (!string.IsNullOrEmpty(prioritys))
				{
					foreach (string str in prioritys.Split(new char[]
					{
						','
					}))
					{
						if (str != leader)
						{
							rids.Add(str);
						}
					}
				}
				prioritys = string.Join(",", rids);
				detailData.JunTuanData.V.GVoicePrioritys = prioritys;
				this.Persistence.AddDelayWriteSql(string.Format("update t_juntuan set voice='{0}';", prioritys));
				GMCmdData gmCmdData = new GMCmdData
				{
					Fields = new string[]
					{
						"-gvoicepriority",
						2.ToString(),
						junTuanId.ToString(),
						prioritys
					}
				};
				this.BroadcastGMCmdData(gmCmdData, 2);
			}
			return 0;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00013448 File Offset: 0x00011648
		public string GetJunTuanGVoicePrioritys(int bhid)
		{
			string result;
			lock (this.Mutex)
			{
				int junTuanId;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out junTuanId))
				{
					result = null;
				}
				else
				{
					JunTuanDetailData detailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
					{
						flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = null;
					}
					else
					{
						result = detailData.JunTuanData.V.GVoicePrioritys;
					}
				}
			}
			return result;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0001351C File Offset: 0x0001171C
		public void BroadcastGMCmdData(GMCmdData data, int serverFlag)
		{
			lock (this.Mutex)
			{
				if (serverFlag == 1)
				{
					ClientAgentManager.Instance().KFBroadCastAsyncEvent(GameTypes.JunTuan, new AsyncDataItem(KuaFuEventTypes.GMCmd, new object[]
					{
						data
					}));
				}
				else
				{
					ClientAgentManager.Instance().BroadCastAsyncEvent(GameTypes.JunTuan, new AsyncDataItem(KuaFuEventTypes.GMCmd, new object[]
					{
						data
					}), 0);
				}
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x000135BC File Offset: 0x000117BC
		public int UpdateJunTuanLingDi(int junTuanId, int lingdi)
		{
			lock (this.Mutex)
			{
				foreach (JunTuanDetailData detailData in this.JunTuanAllDataDict.Values)
				{
					if (detailData.JunTuanId == junTuanId)
					{
						detailData.JunTuanData.V.LingDi |= lingdi;
						TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
						this.Persistence.AddJuntuanEventLog(detailData, new JunTuanEventLog
						{
							EventType = 5,
							Time = TimeUtil.NowDateTime(),
							Message = lingdi.ToString()
						});
					}
					else if ((detailData.JunTuanData.V.LingDi & lingdi) != 0)
					{
						detailData.JunTuanData.V.LingDi &= ~lingdi;
						TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
					}
				}
				this.Persistence.UpdateJunTuanLingDi(junTuanId, lingdi);
				this.Persistence.UpdateJunTuanMiniDataList();
			}
			return 0;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x000137B0 File Offset: 0x000119B0
		public int QuitJunTuan(int bhid, int junTuanId, int otherBhid)
		{
			try
			{
				lock (this.Mutex)
				{
					int realJunTuanId;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out realJunTuanId) || realJunTuanId != junTuanId)
					{
						return -1020;
					}
					JunTuanDetailData detailData;
					if (!this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
					{
						return -1024;
					}
					int otherJunTuanId;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(otherBhid, out otherJunTuanId) && otherJunTuanId != junTuanId)
					{
						return -1025;
					}
					int num = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0) ? 1 : 2;
					if (otherBhid != bhid && this.Persistence.RuntimeData.RolePermissionDict.Value[1].Manager == 0)
					{
						return -1024;
					}
					JunTuanBangHuiData bhData = detailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == otherBhid);
					bhData.JuTuanZhiWu = 0;
					this.Persistence.UpdateJunTuanBangHuiData(bhData, 0);
					string message = KuaFuServerManager.FormatName(bhData.BhZoneId, bhData.BhName);
					JunTuanEventLog log = new JunTuanEventLog
					{
						EventType = 3,
						Message = message,
						Time = TimeUtil.NowDateTime()
					};
					this.Persistence.AddJuntuanEventLog(detailData, log);
					int zhiwu2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == otherBhid) == 0) ? 1 : 0;
					if (zhiwu2 == 1)
					{
						this.JunTuanRemoveBangHui(detailData, otherBhid);
						JunTuanBangHuiMiniData bhjtData = new JunTuanBangHuiMiniData
						{
							BhId = detailData.LeaderBhId,
							JunTuanId = junTuanId,
							JunTuanName = detailData.JunTuanData.V.JunTuanName,
							JunTuanZhiWu = 1
						};
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.UpdateBhJunTuan, new object[]
						{
							bhjtData
						}), 0);
					}
					else
					{
						this.JunTuanRemoveBangHui(detailData, otherBhid);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return 0;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00013AE0 File Offset: 0x00011CE0
		private void JunTuanAddBangHui(JunTuanDetailData detailData, JunTuanBangHuiData data)
		{
			this.BangHuiJunTuanIdDict[data.BhId] = detailData.JunTuanId;
			int index = detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == data.BhId);
			if (index >= 0)
			{
				detailData.JunTuanBangHuiList.V[index] = data;
			}
			else
			{
				detailData.JunTuanBangHuiList.V.Add(data);
			}
			TimeUtil.AgeByNow(ref detailData.JunTuanBangHuiList.Age);
			this.Persistence.JunTuanUpdateBhList(detailData, true, false);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00013BBC File Offset: 0x00011DBC
		public int DestroyJunTuan(int bhid, int junTuanId)
		{
			try
			{
				lock (this.Mutex)
				{
					int realJunTuanId;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out realJunTuanId) || realJunTuanId != junTuanId)
					{
						return -1020;
					}
					JunTuanDetailData detailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
					{
						flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						return -1024;
					}
					if (detailData.JunTuanData.V.LingDi > 0)
					{
						return -1032;
					}
					this.Persistence.DestroyJunTuan(detailData);
					this.Persistence.AddJunTuanEventLog(detailData.JunTuanId, new JunTuanEventLog
					{
						EventType = 6,
						Message = "Destroy",
						Time = TimeUtil.NowDateTime()
					});
					JunTuanEraService.Instance().OnJunTuanDestroy(detailData.JunTuanId);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return 0;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00013D5C File Offset: 0x00011F5C
		public void JunTuanChat(int serverId, byte[] bytes)
		{
			try
			{
				ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.JunTuanChat, new object[]
				{
					bytes
				}), serverId);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00013DE4 File Offset: 0x00011FE4
		public int JoinJunTuan(byte[] cmdBytes)
		{
			try
			{
				JunTuanRequestData data = DataHelper2.BytesToObject<JunTuanRequestData>(cmdBytes, 0, cmdBytes.Length);
				lock (this.Mutex)
				{
					int junTuanId;
					if (this.BangHuiJunTuanIdDict.TryGetValue(data.BhId, out junTuanId) && junTuanId > 0)
					{
						return -1020;
					}
					JunTuanBangHuiData bhData;
					if (!this.JunTuanBangHuiDataDict.TryGetValue(data.BhId, out bhData))
					{
						bhData = new JunTuanBangHuiData();
						bhData.BhId = data.BhId;
						this.JunTuanBangHuiDataDict[bhData.BhId] = bhData;
					}
					bhData.BhName = data.BhName;
					bhData.BhZoneId = data.BhZoneId;
					bhData.LeaderName = data.LeaderName;
					bhData.LeaderZoneId = data.LeaderZoneId;
					bhData.RoleNum = data.RoleNum;
					bhData.ZhanLi = data.ZhanLi;
					bhData.LeaderOccupation = data.Occupation;
					bhData.LeaderRoleId = data.LeaderRoleId;
					long nowTicks = TimeUtil.NOW();
					if (nowTicks - bhData.LastRequestJoinTicks < (long)(this.Persistence.RuntimeData.LegionsJionCD * 60000))
					{
						return -2007;
					}
					JunTuanDetailData detailData;
					if (!this.JunTuanAllDataDict.TryGetValue(data.JunTuanId, out detailData))
					{
						return -1022;
					}
					if (detailData.JunTuanData.V.BangHuiNum >= 4)
					{
						return -1031;
					}
					if (detailData.JoinDataList.V.Exists((JunTuanRequestData x) => x.BhId == data.BhId))
					{
						return -1021;
					}
					if (this.Persistence.AddJunTuanJoinData(data) < 0L)
					{
						return -15;
					}
					bhData.LastRequestJoinTicks = nowTicks;
					if (detailData.JoinDataList == null)
					{
						detailData.JoinDataList = new KuaFuData<List<JunTuanRequestData>>();
						detailData.JoinDataList.Age += 1L;
						detailData.JoinDataList.V = new List<JunTuanRequestData>();
					}
					detailData.JoinDataList.V.Add(data);
					this.Persistence.UpdateRequestDataListCmdData(detailData);
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.JunTuanRequest, new object[]
					{
						data.JunTuanId,
						1
					}), 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return 0;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0001414C File Offset: 0x0001234C
		public KuaFuCmdData GetJunTuanList(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (this.JunTuanMiniDataListCmdData.Age != dataAge)
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanMiniDataListCmdData.Age,
						Bytes0 = this.JunTuanMiniDataListCmdData.Bytes0
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanMiniDataListCmdData.Age
					};
				}
			}
			return result;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x000141F0 File Offset: 0x000123F0
		public KuaFuCmdData GetJunTuanData(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int junTuanId;
				JunTuanDetailData detailData;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out junTuanId))
				{
					result = null;
				}
				else if (!this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
				{
					result = null;
				}
				else if (dataAge != detailData.JunTuanData.Age)
				{
					result = new KuaFuCmdData
					{
						Age = detailData.JunTuanData.Age,
						Bytes0 = DataHelper2.ObjectToBytes<JunTuanData>(detailData.JunTuanData.V)
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = detailData.JunTuanData.Age
					};
				}
			}
			return result;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x000142D8 File Offset: 0x000124D8
		public KuaFuCmdData GetJunTuanBaseData(int bhid, long dataAge)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000142E0 File Offset: 0x000124E0
		public KuaFuCmdData GetJunTuanBaseDataList(long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (this.JunTuanBaseDataListCmdData.Age != dataAge)
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanBaseDataListCmdData.Age,
						Bytes0 = this.JunTuanBaseDataListCmdData.Bytes0
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanBaseDataListCmdData.Age
					};
				}
			}
			return result;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00014384 File Offset: 0x00012584
		public KuaFuCmdData GetJunTuanRequestList(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int junTuanId;
				JunTuanDetailData detailData;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out junTuanId))
				{
					result = null;
				}
				else if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
				{
					if (dataAge == detailData.RequestDataListCmdData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = detailData.RequestDataListCmdData.Age
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = detailData.RequestDataListCmdData.Age,
							Bytes0 = detailData.RequestDataListCmdData.Bytes0
						};
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000144F4 File Offset: 0x000126F4
		public int JoinJunTuanResponse(int bhid, int junTuanId, int otherBhid, bool accept)
		{
			try
			{
				lock (this.Mutex)
				{
					int selfJunTuanId;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId) || selfJunTuanId != junTuanId)
					{
						return -1026;
					}
					JunTuanDetailData detailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
					{
						flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						return -1024;
					}
					JunTuanRequestData requestData = detailData.JoinDataList.V.Find((JunTuanRequestData x) => x.BhId == otherBhid);
					if (requestData == null)
					{
						return -1001;
					}
					if (accept)
					{
						if (detailData.JunTuanData.V.BangHuiNum >= 4)
						{
							return -1031;
						}
					}
					foreach (JunTuanDetailData d in this.JunTuanAllDataDict.Values)
					{
						JunTuanRequestData v = d.JoinDataList.V.FirstOrDefault((JunTuanRequestData x) => x.BhId == otherBhid);
						if (null != v)
						{
							this.Persistence.DeleteJunTuanRequestData(d, v);
							this.Persistence.UpdateRequestDataListCmdData(d);
						}
					}
					if (accept)
					{
						int otherJunTuanId;
						if (this.BangHuiJunTuanIdDict.TryGetValue(otherBhid, out otherJunTuanId) && otherJunTuanId != 0)
						{
							return -1020;
						}
						JunTuanBangHuiData jtbhdata;
						if (!this.JunTuanBangHuiDataDict.TryGetValue(otherBhid, out jtbhdata))
						{
							return -1001;
						}
						this.JunTuanAddBangHui(detailData, jtbhdata);
						this.Persistence.UpdateJunTuanBangHuiData(jtbhdata, junTuanId);
						this.Persistence.ReloadJunTuanRoleDataList(detailData);
						JunTuanBangHuiData bhData = detailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == otherBhid);
						string message = KuaFuServerManager.FormatName(bhData.BhZoneId, bhData.BhName);
						JunTuanEventLog log = new JunTuanEventLog
						{
							EventType = 1,
							Message = message,
							Time = TimeUtil.NowDateTime()
						};
						this.Persistence.AddJuntuanEventLog(detailData, log);
						JunTuanBangHuiMiniData bhjtData = new JunTuanBangHuiMiniData
						{
							BhId = otherBhid,
							JunTuanId = junTuanId,
							JunTuanName = detailData.JunTuanData.V.JunTuanName,
							JunTuanChanged = 1
						};
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.UpdateBhJunTuan, new object[]
						{
							bhjtData
						}), 0);
					}
					if (detailData.JoinDataList.V.Count == 0)
					{
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.JunTuanRequest, new object[]
						{
							junTuanId,
							0
						}), 0);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -11003;
			}
			return 0;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00014978 File Offset: 0x00012B78
		public int RemoveBangHui(int otherBhid)
		{
			try
			{
				lock (this.Mutex)
				{
					int selfJunTuanId;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(otherBhid, out selfJunTuanId) || selfJunTuanId <= 0)
					{
						Dictionary<JunTuanDetailData, JunTuanRequestData> dict = new Dictionary<JunTuanDetailData, JunTuanRequestData>();
						foreach (JunTuanDetailData dt in this.JunTuanAllDataDict.Values)
						{
							JunTuanRequestData rd = dt.JoinDataList.V.FirstOrDefault((JunTuanRequestData x) => x.BhId == otherBhid);
							if (null != rd)
							{
								dict[dt] = rd;
							}
						}
						foreach (KeyValuePair<JunTuanDetailData, JunTuanRequestData> kv in dict)
						{
							this.Persistence.DeleteJunTuanRequestData(kv.Key, kv.Value);
						}
						return 1;
					}
					JunTuanDetailData detailData;
					if (!this.JunTuanAllDataDict.TryGetValue(selfJunTuanId, out detailData))
					{
						return 1;
					}
					JunTuanBangHuiData bhData = detailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == otherBhid);
					if (null == bhData)
					{
						return 1;
					}
					bhData.JuTuanZhiWu = 0;
					this.Persistence.UpdateJunTuanBangHuiData(bhData, 0);
					string message = KuaFuServerManager.FormatName(bhData.BhZoneId, bhData.BhName);
					JunTuanEventLog log = new JunTuanEventLog
					{
						EventType = 3,
						Message = message,
						Time = TimeUtil.NowDateTime()
					};
					this.Persistence.AddJuntuanEventLog(detailData, log);
					int zhiwu2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == otherBhid) == 0) ? 1 : 0;
					if (zhiwu2 == 1)
					{
						this.JunTuanRemoveBangHui(detailData, otherBhid);
						JunTuanBangHuiMiniData bhjtData = new JunTuanBangHuiMiniData
						{
							BhId = detailData.LeaderBhId,
							JunTuanId = selfJunTuanId,
							JunTuanName = detailData.JunTuanData.V.JunTuanName,
							JunTuanZhiWu = 1
						};
						ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.UpdateBhJunTuan, new object[]
						{
							bhjtData
						}), 0);
					}
					else
					{
						this.JunTuanRemoveBangHui(detailData, otherBhid);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -11003;
			}
			return 0;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00014D10 File Offset: 0x00012F10
		public int ChangeBangHuiName(int bhid, string bhName)
		{
			try
			{
				lock (this.Mutex)
				{
					JunTuanBangHuiData bhData;
					if (!this.JunTuanBangHuiDataDict.TryGetValue(bhid, out bhData))
					{
						return 3;
					}
					bhData.BhName = bhName;
					int selfJunTuanId;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId) || selfJunTuanId <= 0)
					{
						this.Persistence.UpdateJunTuanBangHuiData(bhData, 0);
						return 1;
					}
					JunTuanDetailData detailData;
					if (!this.JunTuanAllDataDict.TryGetValue(selfJunTuanId, out detailData))
					{
						return 1;
					}
					bhData = detailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == bhid);
					if (null == bhData)
					{
						return 1;
					}
					this.Persistence.UpdateJunTuanBangHuiData(bhData, selfJunTuanId);
					this.Persistence.JunTuanUpdateBhList(detailData, true, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -11003;
			}
			return 0;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00014E90 File Offset: 0x00013090
		public KuaFuCmdData GetJunTuanBangHuiList(int bhid, int junTuanId, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int selfJunTuanId;
				JunTuanDetailData detailData;
				if (bhid > 0 && (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId) || junTuanId != selfJunTuanId))
				{
					result = null;
				}
				else if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
				{
					if (dataAge != detailData.JunTuanBangHuiListCmdData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = detailData.JunTuanBangHuiListCmdData.Age,
							Bytes0 = detailData.JunTuanBangHuiListCmdData.Bytes0
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = detailData.JunTuanBangHuiListCmdData.Age
						};
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00014FD0 File Offset: 0x000131D0
		public int JunTuanChangeBangHuiZhiWu(int bhid, int junTuanId, int otherBhid, int zhiWu)
		{
			try
			{
				lock (this.Mutex)
				{
					int selfJunTuanId;
					if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId) || selfJunTuanId != junTuanId)
					{
						return -1026;
					}
					JunTuanDetailData detailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
					{
						flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						return -1024;
					}
					int otherJunTuanId;
					if (this.BangHuiJunTuanIdDict.TryGetValue(otherBhid, out otherJunTuanId) && otherJunTuanId != junTuanId)
					{
						return -1025;
					}
					JunTuanBangHuiData jtbhdata;
					if (!this.JunTuanBangHuiDataDict.TryGetValue(otherBhid, out jtbhdata))
					{
						return -4;
					}
					if (detailData.JunTuanData.V.LingDi > 0)
					{
						return -1032;
					}
					this.JunTuanChangeBangHui(detailData, bhid, otherBhid);
					JunTuanBangHuiData bhData = detailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == otherBhid);
					string message = KuaFuServerManager.FormatName(bhData.BhZoneId, bhData.BhName);
					JunTuanEventLog log = new JunTuanEventLog
					{
						EventType = 2,
						Message = message,
						Time = TimeUtil.NowDateTime()
					};
					this.Persistence.AddJuntuanEventLog(detailData, log);
					JunTuanBangHuiMiniData bhjtData = new JunTuanBangHuiMiniData
					{
						BhId = otherBhid,
						JunTuanId = junTuanId,
						JunTuanName = detailData.JunTuanData.V.JunTuanName,
						JunTuanZhiWu = zhiWu
					};
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.UpdateBhJunTuan, new object[]
					{
						bhjtData
					}), 0);
					JunTuanBangHuiMiniData bhjtData2 = new JunTuanBangHuiMiniData
					{
						BhId = bhid,
						JunTuanId = junTuanId,
						JunTuanName = detailData.JunTuanData.V.JunTuanName,
						JunTuanZhiWu = 0
					};
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, new AsyncDataItem(KuaFuEventTypes.UpdateBhJunTuan, new object[]
					{
						bhjtData2
					}), 0);
					return 0;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11003;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x000152DC File Offset: 0x000134DC
		public KuaFuCmdData GetJunTuanRoleList(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int selfJunTuanId;
				JunTuanDetailData detailData;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId) || selfJunTuanId <= 0)
				{
					result = null;
				}
				else if (this.JunTuanAllDataDict.TryGetValue(selfJunTuanId, out detailData))
				{
					if (dataAge != detailData.JunTuanRoleDataListCmdData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = detailData.JunTuanRoleDataListCmdData.Age,
							Bytes0 = detailData.JunTuanRoleDataListCmdData.Bytes0
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = detailData.JunTuanRoleDataListCmdData.Age
						};
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00015414 File Offset: 0x00013614
		public int UpdateRoleDataList(int bhid, KuaFuCmdData listCmdData)
		{
			try
			{
				List<JunTuanRoleData> list = null;
				int roleNum = 0;
				long zhanLi = 0L;
				Dictionary<int, JunTuanRoleData> roleDict = new Dictionary<int, JunTuanRoleData>();
				if (listCmdData != null && listCmdData.Bytes0 != null && listCmdData.Bytes0.Length > 0)
				{
					list = DataHelper2.BytesToObject<List<JunTuanRoleData>>(listCmdData.Bytes0, 0, listCmdData.Bytes0.Length);
					if (null != list)
					{
						foreach (JunTuanRoleData data in list)
						{
							roleNum++;
							zhanLi += (long)data.ZhanLi;
							roleDict[data.RoleId] = data;
						}
					}
				}
				long nowTicks = TimeUtil.NOW();
				bool updateJunTuanMiniDataList = false;
				lock (this.Mutex)
				{
					int junTuanId;
					if (this.BangHuiJunTuanIdDict.TryGetValue(bhid, out junTuanId))
					{
						JunTuanDetailData detailData;
						if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
						{
							JunTuanBangHuiData bhData = detailData.JunTuanBangHuiList.V.Find((JunTuanBangHuiData x) => x.BhId == bhid);
							if (null != bhData)
							{
								bool updateBangHuiListData = false;
								if (bhData.RoleNum != roleNum || bhData.ZhanLi != zhanLi)
								{
									if (bhData.NextUpdateTicks < nowTicks)
									{
										updateBangHuiListData = true;
										bhData.NextUpdateTicks = nowTicks + 300000L;
									}
								}
								bhData.RoleNum = roleNum;
								bhData.ZhanLi = zhanLi;
								this.Persistence.UpdateJunTuanBangHuiData(bhData, junTuanId);
								foreach (JunTuanRoleData roleData in roleDict.Values)
								{
									if (roleData.JuTuanZhiWu == 2 || roleData.JuTuanZhiWu == 1)
									{
										if (roleData.BhId == detailData.LeaderBhId)
										{
											if (detailData.JunTuanData.V.LeaderName != roleData.RoleName || detailData.JunTuanData.V.LeaderZoneId != roleData.ZoneId)
											{
												updateJunTuanMiniDataList = true;
											}
											roleData.JuTuanZhiWu = 1;
											detailData.JunTuanData.V.LeaderZoneId = roleData.ZoneId;
											detailData.JunTuanData.V.LeaderName = roleData.RoleName;
											detailData.JunTuanData.V.LeaderRoleId = roleData.RoleId;
											TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
										}
										else
										{
											roleData.JuTuanZhiWu = 2;
										}
										if (bhData.LeaderName != roleData.RoleName || bhData.LeaderZoneId != roleData.ZoneId || bhData.LeaderOccupation != roleData.Occu)
										{
											updateBangHuiListData = true;
											bhData.LeaderName = roleData.RoleName;
											bhData.LeaderZoneId = roleData.ZoneId;
											bhData.LeaderOccupation = roleData.Occu;
										}
									}
								}
								detailData.JunTuanRoleDataList.Age = TimeUtil.AgeByNow(detailData.JunTuanRoleDataList.Age);
								detailData.JunTuanRoleDataList.V.RemoveAll((JunTuanRoleData x) => x.BhId == bhid);
								detailData.JunTuanRoleDataList.V.AddRange(list);
								detailData.JunTuanRoleDataListCmdData.Age = detailData.JunTuanRoleDataList.Age;
								detailData.JunTuanRoleDataListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanRoleData>>(detailData.JunTuanRoleDataList.V);
								if (updateBangHuiListData)
								{
									TimeUtil.AgeByNow(ref detailData.JunTuanBangHuiListCmdData.Age);
									detailData.JunTuanBangHuiListCmdData.Bytes0 = DataHelper2.ObjectToBytes<List<JunTuanBangHuiData>>(detailData.JunTuanBangHuiList.V);
								}
							}
						}
					}
				}
				this.Persistence.UpdateJuntuanRoleDataList(bhid, roleDict);
				if (updateJunTuanMiniDataList)
				{
					this.Persistence.UpdateJunTuanMiniDataList();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return 0;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00015944 File Offset: 0x00013B44
		public KuaFuCmdData GetJunTuanTaskAllData(int bhid, int junTuanId, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int selfJunTuanId;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId) || selfJunTuanId == 0)
				{
					result = new KuaFuCmdData
					{
						Age = -1025L
					};
				}
				else
				{
					JunTuanDetailData detailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(selfJunTuanId, out detailData))
					{
						flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) >= 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = new KuaFuCmdData
						{
							Age = -1025L
						};
					}
					else if (dataAge != detailData.JunTuanTaskAllData.Age)
					{
						detailData.JunTuanTaskAllDataCmdData.Age = detailData.JunTuanTaskAllData.Age;
						detailData.JunTuanTaskAllDataCmdData.Bytes0 = DataHelper2.ObjectToBytes<JunTuanTaskAllData>(detailData.JunTuanTaskAllData.V);
						result = new KuaFuCmdData
						{
							Age = detailData.JunTuanTaskAllDataCmdData.Age,
							Bytes0 = detailData.JunTuanTaskAllDataCmdData.Bytes0
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = detailData.JunTuanTaskAllData.Age
						};
					}
				}
			}
			return result;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00015B30 File Offset: 0x00013D30
		public int JunTuanChangeTaskValue(int bhid, int junTuanId, int taskId, int addValue, long ticks)
		{
			DateTime taskTime = new DateTime(ticks);
			int result;
			lock (this.Mutex)
			{
				int selfJunTuanId;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId))
				{
					result = -1025;
				}
				else
				{
					JunTuanDetailData detailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(selfJunTuanId, out detailData))
					{
						flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) >= 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = -1025;
					}
					else
					{
						JunTuanTaskData taskData = detailData.JunTuanTaskAllData.V.TaskList.Find((JunTuanTaskData x) => x.TaskId == taskId);
						JunTuanTaskInfo taskInfo;
						if (null == taskData)
						{
							result = -1029;
						}
						else if (taskData.TaskState != 0L)
						{
							result = 3;
						}
						else if (taskData.WeekDay != TimeUtil.GetWeekStartDayIdNow())
						{
							result = 3;
						}
						else if (!this.Persistence.RuntimeData.TaskList.Value.TryGetValue(taskId, out taskInfo))
						{
							result = -1029;
						}
						else
						{
							DateTime now = TimeUtil.NowDateTime();
							taskData.TaskValue += Math.Min(addValue, taskInfo.NumInterval - taskData.TaskValue);
							TimeUtil.AgeByNow(ref detailData.JunTuanTaskAllData.Age);
							if (taskData.TaskValue >= taskInfo.NumInterval)
							{
								taskData.TaskState = 1L;
								detailData.JunTuanTaskAllData.V.TaskLastTime = now;
								this.Persistence.AddJunTuanPoint(detailData, taskInfo.Score, false);
								this.Persistence.AddJuntuanEventLog(detailData, new JunTuanEventLog
								{
									EventType = 4,
									Message = taskInfo.Name,
									Time = taskTime
								});
								this.Persistence.UpdateJunTuanTaskData(junTuanId, taskData, taskTime, taskInfo.Score);
							}
							else
							{
								this.Persistence.UpdateJunTuanTaskData(junTuanId, taskData);
							}
							result = taskData.TaskValue;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00015DC0 File Offset: 0x00013FC0
		public KuaFuCmdData GetJunTuanRankingData(long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				if (dataAge != this.JunTuanRankDataListCmdData.Age)
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanRankDataListCmdData.Age,
						Bytes0 = this.JunTuanRankDataListCmdData.Bytes0
					};
				}
				else
				{
					result = new KuaFuCmdData
					{
						Age = this.JunTuanRankDataListCmdData.Age
					};
				}
			}
			return result;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00015E8C File Offset: 0x0001408C
		public KuaFuCmdData GetJunTuanLogList(int bhid, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				int selfJunTuanId;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId))
				{
					result = null;
				}
				else
				{
					JunTuanDetailData detailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(selfJunTuanId, out detailData))
					{
						flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) >= 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = null;
					}
					else if (dataAge != detailData.EventLogListCmdData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = detailData.EventLogListCmdData.Age,
							Bytes0 = detailData.EventLogListCmdData.Bytes0
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = detailData.EventLogListCmdData.Age
						};
					}
				}
			}
			return result;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00015FB8 File Offset: 0x000141B8
		private void CheckRoleTimerProc(DateTime now)
		{
			if (this.KarenFuBenDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					foreach (KuaFuData<KarenFuBenData> fuBenData in this.KarenFuBenDataDict.Values)
					{
						List<int> callist = new List<int>(new int[fuBenData.V.EnterGameRoleCount.Count]);
						List<int> list = new List<int>();
						foreach (KarenFuBenRoleData roleData in fuBenData.V.RoleDict.Values)
						{
							if (roleData.State == KuaFuRoleStates.EnterGame)
							{
								if (roleData.StateEndTime < now)
								{
									roleData.State = KuaFuRoleStates.None;
									list.Add(roleData.RoleId);
									LogManager.WriteLog(LogTypes.Error, string.Format("阿卡伦战场角色状态数据清除 rid={0} endtm={1} state={2}", roleData.RoleId, roleData.StateEndTime, roleData.State), null, true);
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
						fuBenData.Age += 1L;
					}
				}
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0001622C File Offset: 0x0001442C
		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.KarenFuBenDataDict.Count != 0)
			{
				lock (this.Mutex)
				{
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, KuaFuData<KarenFuBenData>> fuBenData in this.KarenFuBenDataDict)
					{
						if (fuBenData.Value.V.EndTime < now)
						{
							list.Add(fuBenData.Key);
							LogManager.WriteLog(LogTypes.Error, string.Format("阿卡伦战场副本数据清除 gameId={0} state={1} endtm={2}", fuBenData.Value.V.GameId, fuBenData.Value.V.State, fuBenData.Value.V.EndTime), null, true);
							ClientAgentManager.Instance().RemoveKfFuben((GameTypes)fuBenData.Value.V.GameType, fuBenData.Value.V.ServerId, (long)fuBenData.Value.V.GameId);
						}
					}
					foreach (int key in list)
					{
						this.KarenFuBenDataDict.Remove(key);
					}
				}
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00016420 File Offset: 0x00014620
		public void UpdateKuaFuMapClientCount(int serverId, int gameId, List<int> mapClientCountList)
		{
			lock (this.Mutex)
			{
				if (mapClientCountList != null && mapClientCountList.Count > 0)
				{
					ClientAgent agent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
					if (null != agent)
					{
						KuaFuData<KarenFuBenData> fubenData = null;
						if (this.KarenFuBenDataDict.TryGetValue(gameId, out fubenData))
						{
							fubenData.V.RoleCountSideList = mapClientCountList;
							fubenData.Age += 1L;
						}
					}
				}
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x000164F8 File Offset: 0x000146F8
		public KuaFuCmdData GetKarenKuaFuFuBenData(int gameType, int mapCode, long dataAge)
		{
			KuaFuCmdData result;
			lock (this.Mutex)
			{
				JunTuanBattleMiniInfo battleMiniInfo = JunTuanPersistence.Instance.RuntimeData.KarenBattleMapList.Value.Find((JunTuanBattleMiniInfo x) => x.MapCode == mapCode);
				if (null == battleMiniInfo)
				{
					result = null;
				}
				else
				{
					KuaFuData<KarenFuBenData> KFuBenData = null;
					if (!this.KarenFuBenDataDict.TryGetValue(mapCode, out KFuBenData))
					{
						KFuBenData = new KuaFuData<KarenFuBenData>();
						KFuBenData.V.GameId = mapCode;
						KFuBenData.V.State = GameFuBenState.Wait;
						KFuBenData.V.EndTime = Global.NowTime.AddMinutes(65.0);
						KFuBenData.V.RoleCountSideList = new List<int>(new int[battleMiniInfo.LegionsMax]);
						KFuBenData.V.EnterGameRoleCount = new List<int>(new int[battleMiniInfo.LegionsMax]);
						KFuBenData.V.RoleDict = new Dictionary<int, KarenFuBenRoleData>();
						KFuBenData.V.GameType = gameType;
						if (!ClientAgentManager.Instance().SpecialKfFuben((GameTypes)gameType, (long)mapCode, battleMiniInfo.MaxEnterNum * battleMiniInfo.LegionsMax, out KFuBenData.V.ServerId))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("阿卡伦分配游戏服务器失败 gameType={0}, mapCode={1}", gameType, mapCode), null, true);
							return null;
						}
						this.KarenFuBenDataDict[mapCode] = KFuBenData;
						KFuBenData.Age += 1L;
					}
					if (dataAge != KFuBenData.Age)
					{
						result = new KuaFuCmdData
						{
							Age = KFuBenData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KarenFuBenData>(KFuBenData.V)
						};
					}
					else
					{
						result = new KuaFuCmdData
						{
							Age = KFuBenData.Age
						};
					}
				}
			}
			return result;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00016734 File Offset: 0x00014934
		public KarenFuBenRoleData GetKarenFuBenRoleData(int gameId, int roleId)
		{
			KarenFuBenRoleData result;
			lock (this.Mutex)
			{
				KuaFuData<KarenFuBenData> fubenData = null;
				if (this.KarenFuBenDataDict == null || !this.KarenFuBenDataDict.TryGetValue(gameId, out fubenData))
				{
					result = null;
				}
				else
				{
					KarenFuBenRoleData kroleData = null;
					if (!fubenData.V.RoleDict.TryGetValue(roleId, out kroleData))
					{
						result = null;
					}
					else
					{
						result = kroleData;
					}
				}
			}
			return result;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000167EC File Offset: 0x000149EC
		public int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int side, int state)
		{
			lock (this.Mutex)
			{
				JunTuanBattleMiniInfo battleMiniInfo = JunTuanPersistence.Instance.RuntimeData.KarenBattleMapList.Value.Find((JunTuanBattleMiniInfo x) => x.MapCode == gameId);
				if (null == battleMiniInfo)
				{
					return -11003;
				}
				KuaFuData<KarenFuBenData> fubenData = null;
				if (this.KarenFuBenDataDict == null || !this.KarenFuBenDataDict.TryGetValue(gameId, out fubenData))
				{
					return -11003;
				}
				KarenFuBenRoleData kroleData = null;
				if (!fubenData.V.RoleDict.TryGetValue(roleId, out kroleData))
				{
					kroleData = new KarenFuBenRoleData
					{
						ServerId = serverId,
						RoleId = roleId,
						KuaFuServerId = fubenData.V.ServerId,
						KuaFuMapCode = fubenData.V.GameId,
						Side = side,
						State = KuaFuRoleStates.None
					};
					fubenData.V.RoleDict[roleId] = kroleData;
				}
				if (4 == state)
				{
					if (fubenData.V.GetRoleCountWithEnter(side) >= battleMiniInfo.MaxEnterNum)
					{
						return -22;
					}
					if (kroleData.State == KuaFuRoleStates.None || kroleData.State == KuaFuRoleStates.Offline)
					{
						List<int> list;
						int index;
						(list = fubenData.V.EnterGameRoleCount)[index = side - 1] = list[index] + 1;
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
						(list = fubenData.V.EnterGameRoleCount)[index = side - 1] = list[index] - 1;
					}
					(list = fubenData.V.RoleCountSideList)[index = side - 1] = list[index] + 1;
				}
				else if (7 == state)
				{
					List<int> list;
					int index;
					(list = fubenData.V.RoleCountSideList)[index = side - 1] = list[index] - 1;
				}
				else if (6 == state)
				{
					fubenData.V.State = GameFuBenState.End;
				}
				kroleData.Side = side;
				kroleData.State = (KuaFuRoleStates)state;
				fubenData.Age += 1L;
			}
			return state;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00016AE4 File Offset: 0x00014CE4
		private JunTuanDetailData JuntuanAdd(JunTuanData data, JunTuanBangHuiData bhData)
		{
			JunTuanDetailData result;
			lock (this.Mutex)
			{
				int junTuanId = data.JunTuanId;
				JunTuanDetailData detailData;
				if (!this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
				{
					detailData = new JunTuanDetailData
					{
						JunTuanId = data.JunTuanId
					};
					this.JunTuanAllDataDict[junTuanId] = detailData;
				}
				detailData.JunTuanData.V = data;
				this.JunTuanAddBangHui(detailData, bhData);
				result = detailData;
			}
			return result;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00016BD0 File Offset: 0x00014DD0
		private void JunTuanChangeBangHui(JunTuanDetailData detailData, int bhid1, int bhid2)
		{
			int index = detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid1);
			int index2 = detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid2);
			if (index >= 0 && index2 >= 0)
			{
				JunTuanBangHuiData data = detailData.JunTuanBangHuiList.V[index];
				detailData.JunTuanBangHuiList.V[index] = detailData.JunTuanBangHuiList.V[index2];
				detailData.JunTuanBangHuiList.V[index2] = data;
				for (int i = 0; i < detailData.JunTuanBangHuiList.V.Count; i++)
				{
					if (i == 0)
					{
						detailData.JunTuanBangHuiList.V[i].JuTuanZhiWu = 1;
					}
					else
					{
						detailData.JunTuanBangHuiList.V[i].JuTuanZhiWu = 0;
					}
					this.Persistence.UpdateJunTuanBangHuiData(detailData.JunTuanBangHuiList.V[i], detailData.JunTuanId);
				}
				TimeUtil.AgeByNow(ref detailData.JunTuanBangHuiList.Age);
				this.Persistence.JunTuanUpdateBhList(detailData, true, true);
				foreach (JunTuanRoleData roleData in detailData.JunTuanRoleDataList.V)
				{
					if (roleData.JuTuanZhiWu == 1 || roleData.JuTuanZhiWu == 2)
					{
						if (roleData.BhId == bhid2)
						{
							roleData.JuTuanZhiWu = 1;
							detailData.JunTuanData.V.LeaderZoneId = roleData.ZoneId;
							detailData.JunTuanData.V.LeaderName = roleData.RoleName;
							detailData.JunTuanData.V.LeaderRoleId = roleData.RoleId;
						}
						else
						{
							roleData.JuTuanZhiWu = 2;
						}
						this.Persistence.UpdateJuntuanRoleData(roleData);
					}
				}
				TimeUtil.AgeByNow(ref detailData.JunTuanData.Age);
				this.Persistence.UpdateRoleDataListCmdData(detailData);
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00016E78 File Offset: 0x00015078
		private void JunTuanRemoveBangHui(JunTuanDetailData detailData, int bhid)
		{
			this.BangHuiJunTuanIdDict[bhid] = 0;
			if (null != detailData.JunTuanBangHuiList.V)
			{
				detailData.JunTuanBangHuiList.V.RemoveAll((JunTuanBangHuiData x) => x.BhId == bhid);
				detailData.JunTuanBangHuiList.Age += 1L;
			}
			if (null != detailData.JunTuanBaseData.V)
			{
				detailData.JunTuanBaseData.V.BhList.Remove(bhid);
				detailData.JunTuanBaseData.Age += 1L;
			}
			this.Persistence.JunTuanUpdateBhList(detailData, true, false);
			if (detailData.JunTuanBangHuiList.V.Count == 0)
			{
				this.Persistence.DestroyJunTuan(detailData);
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00016F9C File Offset: 0x0001519C
		public int GetJunTuanPoint(int bhid, int junTuanId)
		{
			int result;
			lock (this.Mutex)
			{
				int realJunTuanId;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out realJunTuanId) || realJunTuanId != junTuanId)
				{
					result = -1020;
				}
				else
				{
					JunTuanDetailData detailData;
					bool flag2;
					if (this.JunTuanAllDataDict.TryGetValue(junTuanId, out detailData))
					{
						flag2 = (detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) == 0);
					}
					else
					{
						flag2 = false;
					}
					if (!flag2)
					{
						result = -1024;
					}
					else
					{
						result = detailData.JunTuanData.V.Point;
					}
				}
			}
			return result;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00017084 File Offset: 0x00015284
		public int ExecCommand(string[] args)
		{
			int result = -1;
			try
			{
				if (string.Compare(args[0], "reload") == 0 && 0 == string.Compare(args[1], "paihang"))
				{
					this.ExecPaiHang = true;
				}
				else if (0 == string.Compare(args[0], "load"))
				{
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00017110 File Offset: 0x00015310
		public KuaFuCmdData GetYaoSaiPrisonRoleData(int rid, long dataAge)
		{
			return YaoSaiService.Instance().GetYaoSaiPrisonRoleData(rid, dataAge);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00017130 File Offset: 0x00015330
		public KuaFuCmdData GetYaoSaiFuLuListData(int rid, long dataAge)
		{
			return YaoSaiService.Instance().GetYaoSaiFuLuListData(rid, dataAge);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00017150 File Offset: 0x00015350
		public KuaFuCmdData GetYaoSaiPrisonLogData(int rid, long dataAge)
		{
			return YaoSaiService.Instance().GetYaoSaiPrisonLogData(rid, dataAge);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00017170 File Offset: 0x00015370
		public KuaFuCmdData GetYaoSaiPrisonJingJiData(int rid, long dataAge)
		{
			return YaoSaiService.Instance().GetYaoSaiPrisonJingJiData(rid, dataAge);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00017190 File Offset: 0x00015390
		public KuaFuCmdData SearchYaoSaiFuLu(int rid, int unionlev, int faction, HashSet<int> frindSet)
		{
			return YaoSaiService.Instance().SearchYaoSaiFuLu(rid, unionlev, faction, frindSet);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x000171B4 File Offset: 0x000153B4
		public int YaoSaiPrisonOpt(int srcrid, int otherrid, int type, bool success)
		{
			return YaoSaiService.Instance().YaoSaiPrisonOpt(srcrid, otherrid, type, success);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x000171D8 File Offset: 0x000153D8
		public int UpdateYaoSaiPrisonRoleData(KFUpdatePrisonRole data)
		{
			return YaoSaiService.Instance().UpdateYaoSaiPrisonRoleData(data);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x000171F8 File Offset: 0x000153F8
		public int YaoSaiPrisonHuDong(int ownerid, int fuluid, int type, int param0, int param1, int param2)
		{
			return YaoSaiService.Instance().YaoSaiPrisonHuDong(ownerid, fuluid, type, param0, param1, param2);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00017220 File Offset: 0x00015420
		public int UpdateYaoSaiPrisonLogData(int rid, long id, int state)
		{
			return YaoSaiService.Instance().UpdateYaoSaiPrisonLogData(rid, id, state);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00017240 File Offset: 0x00015440
		public KuaFuCmdData GetEraRankData(long dataAge)
		{
			return JunTuanEraService.Instance().GetEraRankData(dataAge);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00017260 File Offset: 0x00015460
		public KuaFuCmdData GetEraData(int juntuanid, long dataAge)
		{
			return JunTuanEraService.Instance().GetEraData(juntuanid, dataAge);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00017280 File Offset: 0x00015480
		public bool EraDonate(int juntuanid, int taskid, int var1, int var2, int var3)
		{
			return JunTuanEraService.Instance().EraDonate(juntuanid, taskid, var1, var2, var3);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x000172A4 File Offset: 0x000154A4
		public int SetDoubleOpenTime(int roleId, int lingDiType, DateTime openTime, int openSeconds)
		{
			return LingDiCaiJiService.Instance().SetDoubleOpenTime(roleId, lingDiType, openTime, openSeconds);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x000172F0 File Offset: 0x000154F0
		public int SetShouWeiTime(int roleId, int bhid, int lingDiType, DateTime openTime, int index, int junTuanPointCost)
		{
			int selfJunTuanId;
			int result;
			if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out selfJunTuanId))
			{
				result = -1025;
			}
			else
			{
				int junTuanPoint = this.GetJunTuanPoint(bhid, selfJunTuanId);
				JunTuanDetailData detailData;
				if (junTuanPoint < junTuanPointCost)
				{
					result = -1030;
				}
				else if (!this.JunTuanAllDataDict.TryGetValue(selfJunTuanId, out detailData) || detailData.JunTuanBangHuiList.V.FindIndex((JunTuanBangHuiData x) => x.BhId == bhid) < 0)
				{
					result = -1025;
				}
				else
				{
					this.Persistence.AddJunTuanPoint(detailData, -junTuanPointCost, false);
					result = LingDiCaiJiService.Instance().SetShouWeiTime(roleId, lingDiType, openTime, index);
				}
			}
			return result;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000173C4 File Offset: 0x000155C4
		public int CanEnterKuaFuMap(int roleId, int lingDiType)
		{
			return LingDiCaiJiService.Instance().CanEnterKuaFuMap(roleId, lingDiType);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000173E4 File Offset: 0x000155E4
		public List<LingDiData> GetLingDiData()
		{
			return LingDiCaiJiService.Instance().GetLingDiData();
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00017400 File Offset: 0x00015600
		public int SetLingZhu(int roleId, int lingDiType, int junTuanId, string junTuanName, int zhiWu, byte[] roledata)
		{
			return LingDiCaiJiService.Instance().SetLingZhu(roleId, lingDiType, junTuanId, junTuanName, zhiWu, roledata);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00017428 File Offset: 0x00015628
		public int SetShouWei(int lingDiType, List<LingDiShouWei> shouWeiList)
		{
			return LingDiCaiJiService.Instance().SetShouWei(lingDiType, shouWeiList);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00017448 File Offset: 0x00015648
		public int UpdateMapRoleNum(int lingDiType, int roleNum, int serverId)
		{
			return LingDiCaiJiService.Instance().UpdateMapRoleNum(lingDiType, roleNum, serverId);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00017468 File Offset: 0x00015668
		public int GetLingDiRoleNum(int lingDiType)
		{
			return LingDiCaiJiService.Instance().GetLingDiRoleNum(lingDiType);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00017488 File Offset: 0x00015688
		public bool GetClientCacheItems(int serverId)
		{
			return LingDiCaiJiService.Instance().GetClientCacheItems(serverId);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x000174A8 File Offset: 0x000156A8
		public AsyncDataItem GetHongBaoDataList(long dataAge)
		{
			return HongBaoManager_K.getInstance().GetHongBaoDataList(dataAge);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x000174C8 File Offset: 0x000156C8
		public int OpenHongBao(int hongBaoId, int rid, int zoneid, string userid, string rname)
		{
			return HongBaoManager_K.getInstance().OpenHongBao(hongBaoId, rid, zoneid, userid, rname);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x000174EB File Offset: 0x000156EB
		public void AddServerTotalCharge(string keyStr, long addCharge)
		{
			HongBaoManager_K.getInstance().AddServerTotalCharge(keyStr, addCharge);
		}

		// Token: 0x040000E0 RID: 224
		private const double SaveServerStateProcInterval = 30.0;

		// Token: 0x040000E1 RID: 225
		public static JunTuanService Instance = null;

		// Token: 0x040000E2 RID: 226
		public readonly GameTypes GameType = GameTypes.JunTuan;

		// Token: 0x040000E3 RID: 227
		private DateTime CheckTime20;

		// Token: 0x040000E4 RID: 228
		private DateTime CheckTimer120;

		// Token: 0x040000E5 RID: 229
		private DateTime CheckTimer3600;

		// Token: 0x040000E6 RID: 230
		private bool ExecPaiHang = false;

		// Token: 0x040000E7 RID: 231
		private int LastUpdateRankHour = -1;

		// Token: 0x040000E8 RID: 232
		public JunTuanPersistence Persistence = JunTuanPersistence.Instance;

		// Token: 0x040000E9 RID: 233
		private Dictionary<int, int> BangHuiId2RoleDataUpdateDayIdDict = new Dictionary<int, int>();

		// Token: 0x040000EA RID: 234
		private Dictionary<int, KuaFuData<KarenFuBenData>> KarenFuBenDataDict = new Dictionary<int, KuaFuData<KarenFuBenData>>();

		// Token: 0x040000EB RID: 235
		public Thread BackgroundThread;
	}
}
