using System;
using System.Collections.Generic;
using System.ServiceModel;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class JunTuanClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		
		public static JunTuanClient getInstance()
		{
			return JunTuanClient.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 21;
			this.ClientInfo.Token = this.CoreInterface.GetLocalAddressIPs();
			return true;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public void ExecuteEventCallBackAsync(object state)
		{
			AsyncDataItem[] items = state as AsyncDataItem[];
			if (items != null && items.Length > 0)
			{
				foreach (AsyncDataItem item in items)
				{
					this.EventCallBackHandler(item);
				}
			}
		}

		
		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				if (this.NextClearFuBenTime < now)
				{
					this.NextClearFuBenTime = now.AddHours(1.0);
					this.ClearOverTimeFuBen(now);
					this.ClearOverTimePrisonData(now);
				}
				string JunTuanUri = this.CoreInterface.GetRuntimeVariable("JunTuanUri", null);
				if (this.RemoteServiceUri != JunTuanUri)
				{
					this.RemoteServiceUri = JunTuanUri;
				}
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						AsyncDataItem[] items = kuaFuService.GetClientCacheItems(this.ClientInfo.ServerId, this.AsyncDataItemAge);
						if (items != null && items.Length > 0)
						{
							this.ExecuteEventCallBackAsync(items);
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
			}
		}

		
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("JunTuanUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
			}
		}

		
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		
		private IJunTuanService GetKuaFuService(bool noWait = false)
		{
			try
			{
				if (KuaFuManager.KuaFuWorldKuaFuGameServer)
				{
					return null;
				}
				lock (this.Mutex)
				{
					if (string.IsNullOrEmpty(this.RemoteServiceUri))
					{
						return null;
					}
					if (this.KuaFuService == null && noWait)
					{
						return null;
					}
				}
				lock (this.RemotingMutex)
				{
					IJunTuanService kuaFuService;
					if (this.KuaFuService == null)
					{
						kuaFuService = (IJunTuanService)Activator.GetObject(typeof(IJunTuanService), this.RemoteServiceUri);
						if (null == kuaFuService)
						{
							return null;
						}
					}
					else
					{
						kuaFuService = this.KuaFuService;
					}
					int clientId = this.ClientInfo.ClientId;
					long nowTicks = TimeUtil.NOW();
					if (clientId <= 0 || Math.Abs(nowTicks - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = nowTicks;
						clientId = kuaFuService.InitializeClient(this.ClientInfo);
					}
					if (kuaFuService != null && (clientId != this.ClientInfo.ClientId || this.KuaFuService != kuaFuService))
					{
						lock (this.Mutex)
						{
							if (clientId > 0)
							{
								this.KuaFuService = kuaFuService;
							}
							else
							{
								this.KuaFuService = null;
							}
							this.ClientInfo.ClientId = clientId;
							return this.KuaFuService;
						}
					}
					return this.KuaFuService;
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		
		public int GetNewFuBenSeqId()
		{
			int result;
			if (null != this.CoreInterface)
			{
				result = this.CoreInterface.GetNewFuBenSeqId();
			}
			else
			{
				result = -11;
			}
			return result;
		}

		
		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = (int)item.EventType;
				object[] args = item.Args;
				int num = eventType;
				switch (num)
				{
				case 21:
				{
					KuaFuData<JunTuanData> data = args[0] as KuaFuData<JunTuanData>;
					if (null != data)
					{
						JunTuanManager.getInstance().UpdateJunTuanData(data);
					}
					break;
				}
				case 22:
				{
					JunTuanBangHuiMiniData data2 = args[0] as JunTuanBangHuiMiniData;
					if (null != data2)
					{
						JunTuanManager.getInstance().UpdateBhJunTuan(data2);
					}
					break;
				}
				case 23:
				case 24:
					break;
				case 25:
				{
					int bhid = (int)args[0];
					JunTuanManager.getInstance().DelayUpdateJunTuanRoleList(bhid);
					break;
				}
				case 26:
					JunTuanManager.getInstance().OnChatListData(args[0] as byte[]);
					break;
				case 27:
				{
					int junTuanId = (int)args[0];
					bool icon = (int)args[1] > 0;
					JunTuanManager.getInstance().NotifyJunTuanRequest(junTuanId, icon);
					break;
				}
				case 28:
					if (args.Length == 1)
					{
						LingDiCaiJiManager.getInstance().NotifyJunTuanRequest(args[0] as LingDiData, 28);
					}
					break;
				case 29:
					if (args.Length == 1)
					{
						LingDiCaiJiManager.getInstance().NotifyJunTuanRequest(args[0] as LingDiData, 29);
					}
					break;
				case 30:
					if (args.Length == 1)
					{
						HongBaoManager.getInstance().UpdateChongZhiHongBaoDataList(args[0] as KuaFuCmdData);
					}
					break;
				case 31:
					if (args.Length == 1)
					{
						int roleID = (int)args[0];
						YaoSaiJianYuManager.getInstance().UpdateYaoSaiLogData(roleID);
					}
					break;
				case 32:
					if (args.Length == 1)
					{
						int junTuanId = (int)args[0];
						this.GetJunTuanEraData(junTuanId, false);
						EraManager.getInstance().CheckAllJunTuanEraIcon(junTuanId);
					}
					break;
				case 33:
					if (args.Length == 1)
					{
						this.JunTuanEraID = (int)args[0];
						EraManager.getInstance().OnChangeEraID(this.JunTuanEraID);
					}
					break;
				default:
					if (num == 9996)
					{
						if (args.Length == 1)
						{
							GMCmdData data3 = args[0] as GMCmdData;
							GVoiceManager.getInstance().UpdateGVoicePriority(data3, true);
						}
					}
					break;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public int CreateJunTuan(JunTuanRequestData data)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.CreateJunTuan(DataHelper.ObjectToBytes<JunTuanRequestData>(data));
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		
		public int ChangeJunTuanBulltin(int bhid, int junTuanId, string bulltin)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.ChangeJunTuanBulltin(bhid, junTuanId, bulltin);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		
		public int ChangeJunTuanGVoicePrioritys(int bhid, string prioritys)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.ChangeJunTuanGVoicePrioritys(bhid, prioritys);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		
		public void BroadcastGMCmdData(GMCmdData data, int serverFlag)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						kuaFuService.BroadcastGMCmdData(data, serverFlag);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public string GetJunTuanGVoicePrioritys(int bhid)
		{
			string result = null;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.GetJunTuanGVoicePrioritys(bhid);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		
		public int UpdateJunTuanLingDi(int junTuanId, int lingdi)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.UpdateJunTuanLingDi(junTuanId, lingdi);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		
		public int QuitJunTuan(int bhid, int junTuanId, int otherBhId)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.QuitJunTuan(bhid, junTuanId, otherBhId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		
		public int DestroyJunTuan(int bhid, int junTuanId)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.DestroyJunTuan(bhid, junTuanId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		
		public void JunTuanChat(List<KFChat> chatList)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						byte[] sendBytes = DataHelper.ObjectToBytes<List<KFChat>>(chatList);
						kuaFuService.JunTuanChat(this.ClientInfo.ServerId, sendBytes);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public int JoinJunTuan(JunTuanRequestData data)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11000;
				}
				try
				{
					return kuaFuService.JoinJunTuan(DataHelper.ObjectToBytes<JunTuanRequestData>(data));
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11000;
		}

		
		public List<JunTuanMiniData> GetJunTuanList(int bhid)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						KuaFuCmdData result = kuaFuService.GetJunTuanList(this.ClientInfo.ServerId, this.JunTuanList.Age);
						lock (this.Mutex)
						{
							if (null != result)
							{
								if (result == null || result.Age < 0L)
								{
									return null;
								}
								if (result.Age == this.JunTuanList.Age)
								{
									return this.JunTuanList.V;
								}
								this.JunTuanList.Age = result.Age;
								if (result.Bytes0 != null)
								{
									this.JunTuanList.V = DataHelper2.BytesToObject<List<JunTuanMiniData>>(result.Bytes0, 0, result.Bytes0.Length);
								}
								if (this.JunTuanList.V == null)
								{
									this.JunTuanList.V = new List<JunTuanMiniData>();
								}
							}
							return this.JunTuanList.V;
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public JunTuanData GetJunTuanData(int bhid, int junTuanId = 0, bool wait = true)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					long age = 0L;
					JunTuanDetailData detailData = this.GetJunTuanDetailData(bhid, junTuanId);
					if (null != detailData)
					{
						age = detailData.JunTuanData.Age;
					}
					if (!wait)
					{
						return (detailData == null) ? null : detailData.JunTuanData.V;
					}
					KuaFuCmdData result = kuaFuService.GetJunTuanData(bhid, age);
					lock (this.Mutex)
					{
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result.Age == age)
						{
							return detailData.JunTuanData.V;
						}
						JunTuanData data = null;
						if (null != result.Bytes0)
						{
							data = DataHelper.BytesToObject<JunTuanData>(result.Bytes0, 0, result.Bytes0.Length);
						}
						detailData = this.AddJunTuanDetailData(bhid, junTuanId);
						if (data != null && null != detailData)
						{
							return detailData.UpdateJunTuanData(data, result.Age);
						}
						return null;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public int GameFuBenRoleChangeState(int serverId, int rid, int gameId, int side, int state)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GameFuBenRoleChangeState(serverId, rid, gameId, side, state);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		
		public void UpdateKuaFuMapClientCount(int gameId, List<int> mapClientCountList)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.UpdateKuaFuMapClientCount(this.ClientInfo.ServerId, gameId, mapClientCountList);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		
		private void ClearOverTimePrisonData(DateTime now)
		{
			lock (this.Mutex)
			{
				List<int> removeKeyList = new List<int>();
				foreach (KeyValuePair<int, KFPrisonRoleAllData> roleItem in this.YaoSaiPrisonRoleDataDict)
				{
					if (roleItem.Value.RoleDataEndTime < now)
					{
						removeKeyList.Add(roleItem.Key);
					}
				}
				foreach (int key in removeKeyList)
				{
					this.YaoSaiPrisonRoleDataDict.Remove(key);
				}
				removeKeyList.Clear();
				foreach (KeyValuePair<int, KFPrisonFuLuAllData> fuluItem in this.YaoSaiOwnerVsFuLuDict)
				{
					if (fuluItem.Value.DataEndTime < now)
					{
						removeKeyList.Add(fuluItem.Key);
					}
				}
				foreach (int key in removeKeyList)
				{
					this.YaoSaiOwnerVsFuLuDict.Remove(key);
				}
				removeKeyList.Clear();
				foreach (KeyValuePair<int, KFPrisonJingJiAllData> kv in this.YaoSaiPrisonJingJiDataDict)
				{
					if (kv.Value.JingJiDataEndTime < now)
					{
						removeKeyList.Add(kv.Key);
					}
				}
				foreach (int key in removeKeyList)
				{
					this.YaoSaiPrisonJingJiDataDict.Remove(key);
				}
				removeKeyList.Clear();
				foreach (KeyValuePair<int, KFPrisonLogAllData> logItem in this.YaoSaiPrisonLogDataDict)
				{
					if (logItem.Value.LogDataEndTime < now)
					{
						removeKeyList.Add(logItem.Key);
					}
				}
				foreach (int key in removeKeyList)
				{
					this.YaoSaiPrisonLogDataDict.Remove(key);
				}
			}
		}

		
		private void ClearOverTimeFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, KuaFuData<KarenFuBenData>> kv in this.KarenFuBenDataDict)
				{
					if (kv.Value.V.EndTime < now)
					{
						list.Add(kv.Key);
					}
				}
				foreach (int key in list)
				{
					this.KarenFuBenDataDict.Remove(key);
				}
			}
		}

		
		public KarenFuBenRoleData GetKarenFuBenRoleData(int gameid, int roleId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetKarenFuBenRoleData(gameid, roleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public KarenFuBenData GetKarenKuaFuFuBenData(int mapCode)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KarenFuBenData> KFuBenData = null;
						if (!this.KarenFuBenDataDict.TryGetValue(mapCode, out KFuBenData))
						{
							KFuBenData = new KuaFuData<KarenFuBenData>();
							this.KarenFuBenDataDict[mapCode] = KFuBenData;
						}
						SceneUIClasses sceneType = Global.GetMapSceneType(mapCode);
						int gameType;
						if (sceneType == SceneUIClasses.KarenWest)
						{
							gameType = 19;
						}
						else
						{
							gameType = 20;
						}
						KuaFuCmdData result = kuaFuService.GetKarenKuaFuFuBenData(gameType, mapCode, KFuBenData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > KFuBenData.Age)
						{
							KFuBenData.Age = result.Age;
							if (null != result.Bytes0)
							{
								KFuBenData.V = DataHelper2.BytesToObject<KarenFuBenData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null == KFuBenData.V)
							{
								KFuBenData.V = new KarenFuBenData();
							}
						}
						return KFuBenData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public List<JunTuanBaseData> GetJunTuanBaseDataList(bool wait = true)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					KuaFuCmdData result = kuaFuService.GetJunTuanBaseDataList(this.JunTuanBaseDataList.Age);
					lock (this.Mutex)
					{
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result.Age > this.JunTuanBaseDataList.Age)
						{
							this.JunTuanBaseDataList.Age = result.Age;
							if (null != result.Bytes0)
							{
								this.JunTuanBaseDataList.V = DataHelper.BytesToObject<List<JunTuanBaseData>>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null == this.JunTuanBaseDataList.V)
							{
								this.JunTuanBaseDataList.V = new List<JunTuanBaseData>();
							}
						}
						return this.JunTuanBaseDataList.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public int GetJunTuanTaskAllData(int bhid, int junTuanId, out JunTuanTaskAllData taskAllData)
		{
			taskAllData = null;
			try
			{
				KuaFuCmdData result = null;
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					long age = 0L;
					JunTuanDetailData detailData = this.GetJunTuanDetailData(bhid, junTuanId);
					try
					{
						if (null != detailData)
						{
							age = detailData.JunTuanTaskAllData.Age;
						}
						result = kuaFuService.GetJunTuanTaskAllData(bhid, junTuanId, age);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
						return -11000;
					}
					lock (this.Mutex)
					{
						if (result == null || result.Age < 0L)
						{
							return -11003;
						}
						if (result.Age == age)
						{
							taskAllData = detailData.JunTuanTaskAllData.V;
							return 0;
						}
						JunTuanTaskAllData data = null;
						if (result.Bytes0 != null)
						{
							data = DataHelper.BytesToObject<JunTuanTaskAllData>(result.Bytes0, 0, result.Bytes0.Length);
							if (data != null)
							{
								detailData = this.AddJunTuanDetailData(bhid, junTuanId);
								if (detailData.JunTuanTaskAllData.Age < result.Age)
								{
									detailData.JunTuanTaskAllData.Age = result.Age;
									detailData.JunTuanTaskAllData.V = data;
								}
							}
						}
						taskAllData = data;
						return (taskAllData != null) ? 0 : -11000;
					}
				}
				return -11000;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11003;
		}

		
		public bool IsTaskComplete(int bhid, int junTuanId, int taskId)
		{
			try
			{
				JunTuanTaskAllData taskAllData;
				if (this.GetJunTuanTaskAllData(bhid, junTuanId, out taskAllData) < 0 || taskAllData.TaskList == null)
				{
					return false;
				}
				JunTuanTaskData taskData = taskAllData.TaskList.Find((JunTuanTaskData x) => x.TaskId == taskId);
				if (taskData != null && taskData.TaskState == 1L)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return false;
		}

		
		public int JoinJunTuanResponse(int bhid, int junTuanId, int otherBhid, bool accept)
		{
			int result = -11;
			int serverId = this.ClientInfo.ServerId;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.JoinJunTuanResponse(bhid, junTuanId, otherBhid, accept);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		public int RemoveBangHui(int bhid)
		{
			int result = -11;
			int serverId = this.ClientInfo.ServerId;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RemoveBangHui(bhid);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		public int ChangeBangHuiName(int bhid, string bhName)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.ChangeBangHuiName(bhid, bhName);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		public void PushGameResultData(object data)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		
		public int ExecuteCommand(string cmd)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		
		public List<JunTuanRequestData> GetJunTuanRequestList(int bhid)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						long age = 0L;
						KuaFuData<List<JunTuanRequestData>> data;
						lock (this.Mutex)
						{
							if (this.JunTuanRequestListDict.TryGetValue(bhid, out data))
							{
								age = data.Age;
							}
						}
						KuaFuCmdData result = kuaFuService.GetJunTuanRequestList(bhid, age);
						lock (this.Mutex)
						{
							if (result == null || result.Age < 0L)
							{
								return null;
							}
							if (age == result.Age)
							{
								return data.V;
							}
							if (!this.JunTuanRequestListDict.TryGetValue(bhid, out data))
							{
								data = new KuaFuData<List<JunTuanRequestData>>();
								this.JunTuanRequestListDict[bhid] = data;
							}
							if (null != result)
							{
								data.Age = result.Age;
								if (result.Bytes0 != null)
								{
									data.V = DataHelper2.BytesToObject<List<JunTuanRequestData>>(result.Bytes0, 0, result.Bytes0.Length);
								}
								else
								{
									data.V = new List<JunTuanRequestData>();
								}
							}
							return data.V;
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		private JunTuanDetailData GetJunTuanDetailData(int bhid, int junTuanId)
		{
			JunTuanDetailData data = null;
			lock (this.Mutex)
			{
				int oldJunTuanId;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out oldJunTuanId) || oldJunTuanId == 0 || oldJunTuanId != junTuanId)
				{
					return null;
				}
				if (!this.JunTuanDetailDataDict.TryGetValue(junTuanId, out data) || data.JunTuanBaseData.V.BhList == null || !data.JunTuanBaseData.V.BhList.Contains(bhid))
				{
					return null;
				}
			}
			return data;
		}

		
		private JunTuanDetailData AddJunTuanDetailData(int bhid, int junTuanId)
		{
			JunTuanDetailData data = null;
			JunTuanDetailData result;
			lock (this.Mutex)
			{
				this.BangHuiJunTuanIdDict[bhid] = junTuanId;
				if (!this.JunTuanDetailDataDict.TryGetValue(junTuanId, out data))
				{
					data = new JunTuanDetailData
					{
						JunTuanId = junTuanId
					};
					this.JunTuanDetailDataDict[junTuanId] = data;
				}
				result = data;
			}
			return result;
		}

		
		public List<JunTuanBangHuiData> GetJunTuanBangHuiList(int bhid, int junTuanId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					long age = 0L;
					JunTuanDetailData data = this.GetJunTuanDetailData(bhid, junTuanId);
					if (null != data)
					{
						age = data.JunTuanBangHuiList.Age;
					}
					KuaFuCmdData result = kuaFuService.GetJunTuanBangHuiList(bhid, junTuanId, age);
					lock (this.Mutex)
					{
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result.Age == age)
						{
							return data.JunTuanBangHuiList.V;
						}
						if (result.Bytes0 != null)
						{
							data = this.AddJunTuanDetailData(bhid, junTuanId);
							if (data.JunTuanBangHuiList.Age < result.Age)
							{
								data.JunTuanBangHuiList.Age = result.Age;
								if (null != result.Bytes0)
								{
									data.JunTuanBangHuiList.V = DataHelper2.BytesToObject<List<JunTuanBangHuiData>>(result.Bytes0, 0, result.Bytes0.Length);
								}
								else
								{
									data.JunTuanBangHuiList.V = new List<JunTuanBangHuiData>();
								}
							}
							return data.JunTuanBangHuiList.V;
						}
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public List<JunTuanBangHuiData> GetJunTuanBangHuiList(int junTuanId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					KuaFuCmdData result = kuaFuService.GetJunTuanBangHuiList(0, junTuanId, 0L);
					if (result == null || result.Age < 0L)
					{
						return null;
					}
					if (result.Bytes0 != null)
					{
						return DataHelper2.BytesToObject<List<JunTuanBangHuiData>>(result.Bytes0, 0, result.Bytes0.Length);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public int JunTuanChangeBangHuiZhiWu(int bhid, int junTuanId, int otherBhid, int zhiWu)
		{
			int result = -11000;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.JunTuanChangeBangHuiZhiWu(bhid, junTuanId, otherBhid, zhiWu);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		
		public List<JunTuanRoleData> GetJunTuanRoleList(int bhid, int junTuanId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					long age = 0L;
					JunTuanDetailData data = this.GetJunTuanDetailData(bhid, junTuanId);
					if (null != data)
					{
						age = data.JunTuanRoleDataList.Age;
					}
					KuaFuCmdData result = kuaFuService.GetJunTuanRoleList(bhid, age);
					lock (this.Mutex)
					{
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result.Age == age)
						{
							return data.JunTuanRoleDataList.V;
						}
						if (result != null)
						{
							data = this.AddJunTuanDetailData(bhid, junTuanId);
							if (data.JunTuanRoleDataList.Age < result.Age)
							{
								data.JunTuanRoleDataList.Age = result.Age;
								if (null != result.Bytes0)
								{
									data.JunTuanRoleDataList.V = DataHelper2.BytesToObject<List<JunTuanRoleData>>(result.Bytes0, 0, result.Bytes0.Length);
								}
								else
								{
									data.JunTuanRoleDataList.V = new List<JunTuanRoleData>();
								}
							}
							return data.JunTuanRoleDataList.V;
						}
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public int UpdateRoleDataList(int bhid, List<JunTuanRoleData> list)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.UpdateRoleDataList(bhid, new KuaFuCmdData
					{
						Bytes0 = DataHelper.ObjectToBytes<List<JunTuanRoleData>>(list)
					});
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		
		public int JunTuanChangeTaskValue(int bhid, int junTuanId, int taskId, int addValue, long ticks)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.JunTuanChangeTaskValue(bhid, junTuanId, taskId, addValue, ticks);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		
		public List<JunTuanRankData> GetJunTuanRankingData()
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					long age = this.JunTuanRankDataList.Age;
					KuaFuCmdData result = kuaFuService.GetJunTuanRankingData(age);
					lock (this.Mutex)
					{
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result.Age == age)
						{
							return this.JunTuanRankDataList.V;
						}
						if (result != null)
						{
							this.JunTuanRankDataList.Age = result.Age;
							if (null != result.Bytes0)
							{
								this.JunTuanRankDataList.V = DataHelper2.BytesToObject<List<JunTuanRankData>>(result.Bytes0, 0, result.Bytes0.Length);
							}
							else
							{
								this.JunTuanRankDataList.V = new List<JunTuanRankData>();
							}
						}
						return this.JunTuanRankDataList.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public List<JunTuanEventLog> GetJunTuanLogList(int bhid)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					KuaFuCmdData result = kuaFuService.GetJunTuanLogList(bhid, this.JunTuanEventLogList.Age);
					lock (this.Mutex)
					{
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result.Age == this.JunTuanEventLogList.Age)
						{
							return this.JunTuanEventLogList.V;
						}
						if (result != null)
						{
							this.JunTuanEventLogList.Age = result.Age;
							if (null != result.Bytes0)
							{
								this.JunTuanEventLogList.V = DataHelper2.BytesToObject<List<JunTuanEventLog>>(result.Bytes0, 0, result.Bytes0.Length);
							}
							else
							{
								this.JunTuanEventLogList.V = new List<JunTuanEventLog>();
							}
						}
						return this.JunTuanEventLogList.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public int GetJunTuanPoint(int bhid, int junTuanId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetJunTuanPoint(bhid, junTuanId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		
		public int SetDoubleOpenTime(int roleId, int linDiType, DateTime openTime, int openSeconds)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SetDoubleOpenTime(roleId, linDiType, openTime, openSeconds);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -1;
		}

		
		public int SetShouWeiTime(int roleId, int bhid, int linDiType, DateTime openTime, int index, int junTuanPointCost)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SetShouWeiTime(roleId, bhid, linDiType, openTime, index, junTuanPointCost);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -1;
		}

		
		public int CanEnterKuaFuMap(int roleId, int lingDiType)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					int result = kuaFuService.CanEnterKuaFuMap(roleId, lingDiType);
					if (result == 0)
					{
						return -1;
					}
					return result;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -1;
		}

		
		public int UpdateMapRoleNum(int lingDiType, int roleNum, int serverId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.UpdateMapRoleNum(lingDiType, roleNum, serverId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -1;
		}

		
		public List<LingDiData> GetLingDiData()
		{
			List<LingDiData> ret = new List<LingDiData>();
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					ret = kuaFuService.GetLingDiData();
					return ret;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return ret;
		}

		
		public int SetLingZhu(int roleId, int lingDiType, int junTuanId, string junTuanName, int zhiWu, byte[] roledata)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			int ret = 0;
			if (null != kuaFuService)
			{
				try
				{
					ret = kuaFuService.SetLingZhu(roleId, lingDiType, junTuanId, junTuanName, zhiWu, roledata);
					return ret;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return ret;
		}

		
		public int SetShouWei(int lingDiType, List<LingDiShouWei> shouWeiList)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			int ret = 0;
			if (null != kuaFuService)
			{
				try
				{
					ret = kuaFuService.SetShouWei(lingDiType, shouWeiList);
					return ret;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return ret;
		}

		
		public int GetLingDiRoleNum(int lingDiType)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			int ret = 0;
			if (null != kuaFuService)
			{
				try
				{
					ret = kuaFuService.GetLingDiRoleNum(lingDiType);
					return ret;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return ret;
		}

		
		public bool GetClientCacheItems(int serverId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			bool ret = false;
			if (null != kuaFuService)
			{
				try
				{
					ret = kuaFuService.GetClientCacheItems(serverId);
					return ret;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return ret;
		}

		
		public AsyncDataItem GetHongBaoDataList(long dataAge)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetHongBaoDataList(dataAge);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		
		public int OpenHongBao(int hongBaoId, int rid, int zoneid, string userid, string rname)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.OpenHongBao(hongBaoId, rid, zoneid, userid, rname);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		
		public bool AddChargeValue(string keyStr, long addCharge)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.AddServerTotalCharge(keyStr, addCharge);
					return true;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return false;
		}

		
		public void ClearYaoSaiPrisonData(int roleID)
		{
			lock (this.Mutex)
			{
			}
		}

		
		public List<KFPrisonLogData> GetYaoSaiPrisonLogData(int roleID)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KFPrisonLogAllData KYaoSaiLogData = null;
						if (!this.YaoSaiPrisonLogDataDict.TryGetValue(roleID, out KYaoSaiLogData))
						{
							KYaoSaiLogData = new KFPrisonLogAllData();
							this.YaoSaiPrisonLogDataDict[roleID] = KYaoSaiLogData;
						}
						KuaFuCmdData result = kuaFuService.GetYaoSaiPrisonLogData(roleID, KYaoSaiLogData.LogListData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > KYaoSaiLogData.LogListData.Age)
						{
							KYaoSaiLogData.LogListData.Age = result.Age;
							if (null != result.Bytes0)
							{
								KYaoSaiLogData.LogListData.V = DataHelper2.BytesToObject<List<KFPrisonLogData>>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null == KYaoSaiLogData.LogListData.V)
							{
								KYaoSaiLogData.LogListData.V = new List<KFPrisonLogData>();
							}
						}
						KYaoSaiLogData.LogDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						return KYaoSaiLogData.LogListData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public List<KFPrisonRoleData> GetYaoSaiPrisonFuLuData(int roleID)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KFPrisonFuLuAllData KYaoSaiFuLuData = null;
						if (!this.YaoSaiOwnerVsFuLuDict.TryGetValue(roleID, out KYaoSaiFuLuData))
						{
							KYaoSaiFuLuData = new KFPrisonFuLuAllData();
							this.YaoSaiOwnerVsFuLuDict[roleID] = KYaoSaiFuLuData;
						}
						KuaFuCmdData result = kuaFuService.GetYaoSaiFuLuListData(roleID, KYaoSaiFuLuData.fuluData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > KYaoSaiFuLuData.fuluData.Age)
						{
							KYaoSaiFuLuData.fuluData.Age = result.Age;
							if (null != result.Bytes0)
							{
								KYaoSaiFuLuData.fuluData.V = DataHelper2.BytesToObject<List<KFPrisonRoleData>>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null == KYaoSaiFuLuData.fuluData.V)
							{
								KYaoSaiFuLuData.fuluData.V = new List<KFPrisonRoleData>();
							}
						}
						KYaoSaiFuLuData.DataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						return KYaoSaiFuLuData.fuluData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public KFPrisonRoleData GetYaoSaiPrisonRoleData(int roleID)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KFPrisonRoleAllData KYaoSaiRoleData = null;
						if (!this.YaoSaiPrisonRoleDataDict.TryGetValue(roleID, out KYaoSaiRoleData))
						{
							KYaoSaiRoleData = new KFPrisonRoleAllData();
							this.YaoSaiPrisonRoleDataDict[roleID] = KYaoSaiRoleData;
						}
						KuaFuCmdData result = kuaFuService.GetYaoSaiPrisonRoleData(roleID, KYaoSaiRoleData.kfRoleData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > KYaoSaiRoleData.kfRoleData.Age)
						{
							KYaoSaiRoleData.kfRoleData.Age = result.Age;
							if (null != result.Bytes0)
							{
								KYaoSaiRoleData.kfRoleData.V = DataHelper2.BytesToObject<KFPrisonRoleData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null == KYaoSaiRoleData.kfRoleData.V)
							{
								KYaoSaiRoleData.kfRoleData.V = new KFPrisonRoleData();
							}
						}
						KYaoSaiRoleData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						return KYaoSaiRoleData.kfRoleData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public KFPrisonJingJiData GetYaoSaiPrisonJingJiData(int roleID)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KFPrisonJingJiAllData KYaoSaiJingJiData = null;
						if (!this.YaoSaiPrisonJingJiDataDict.TryGetValue(roleID, out KYaoSaiJingJiData))
						{
							KYaoSaiJingJiData = new KFPrisonJingJiAllData();
							this.YaoSaiPrisonJingJiDataDict[roleID] = KYaoSaiJingJiData;
						}
						KuaFuCmdData result = kuaFuService.GetYaoSaiPrisonJingJiData(roleID, KYaoSaiJingJiData.JingJiData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > KYaoSaiJingJiData.JingJiData.Age)
						{
							KYaoSaiJingJiData.JingJiData.Age = result.Age;
							if (null != result.Bytes0)
							{
								KYaoSaiJingJiData.JingJiData.V = DataHelper2.BytesToObject<KFPrisonJingJiData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null == KYaoSaiJingJiData.JingJiData.V)
							{
								KYaoSaiJingJiData.JingJiData.V = new KFPrisonJingJiData();
							}
						}
						KYaoSaiJingJiData.JingJiDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						return KYaoSaiJingJiData.JingJiData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public KFPrisonRoleData SearchYaoSaiFuLu(int rid, int unionlev, int faction, HashSet<int> frindSet)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuCmdData result = kuaFuService.SearchYaoSaiFuLu(rid, unionlev, faction, frindSet);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (null != result)
						{
							KFPrisonRoleAllData KYaoSaiRoleData = new KFPrisonRoleAllData();
							if (null != result.Bytes0)
							{
								KYaoSaiRoleData.kfRoleData.V = DataHelper2.BytesToObject<KFPrisonRoleData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null != KYaoSaiRoleData.kfRoleData.V)
							{
								this.YaoSaiPrisonRoleDataDict[KYaoSaiRoleData.kfRoleData.V.RoleID] = KYaoSaiRoleData;
							}
							return KYaoSaiRoleData.kfRoleData.V;
						}
						return null;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public int YaoSaiPrisonOpt(int srcrid, int otherrid, int type, bool success)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.YaoSaiPrisonOpt(srcrid, otherrid, type, success);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		
		public int YaoSaiPrisonHuDong(int ownerid, int fuluid, int type, int param0, int param1, int param2)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.YaoSaiPrisonHuDong(ownerid, fuluid, type, param0, param1, param2);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		
		public int UpdateYaoSaiPrisonRoleData(KFUpdatePrisonRole data)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.UpdateYaoSaiPrisonRoleData(data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		
		public int UpdateYaoSaiPrisonLogData(int rid, long id, int state)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.UpdateYaoSaiPrisonLogData(rid, id, state);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		
		public bool EraDonate(int juntuanid, int taskid, int var1, int var2, int var3)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.EraDonate(juntuanid, taskid, var1, var2, var3);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return false;
		}

		
		public int GetCurrentEraID()
		{
			int junTuanEraID;
			if (-1 != this.JunTuanEraID)
			{
				junTuanEraID = this.JunTuanEraID;
			}
			else
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					junTuanEraID = this.JunTuanEraID;
				}
				else
				{
					try
					{
						lock (this.Mutex)
						{
							KuaFuCmdData result = kuaFuService.GetEraData(0, 0L);
							if (result == null || result.Age < 0L)
							{
								return this.JunTuanEraID;
							}
							if (null != result.Bytes0)
							{
								KFEraData KEraData = DataHelper2.BytesToObject<KFEraData>(result.Bytes0, 0, result.Bytes0.Length);
								return this.JunTuanEraID = KEraData.EraID;
							}
							return this.JunTuanEraID;
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
						LogManager.WriteLog(LogTypes.Error, "GetCurrentEraID Error{0}", ex, true);
					}
					junTuanEraID = this.JunTuanEraID;
				}
			}
			return junTuanEraID;
		}

		
		public KFEraData GetJunTuanEraData(int JunTuanID, bool noWait = false)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(noWait);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KFEraData> KEraData = null;
						if (!this.EraDataDict.TryGetValue(JunTuanID, out KEraData))
						{
							KEraData = new KuaFuData<KFEraData>();
							this.EraDataDict[JunTuanID] = KEraData;
						}
						else if (noWait)
						{
							return KEraData.V;
						}
						KuaFuCmdData result = kuaFuService.GetEraData(JunTuanID, KEraData.Age);
						if (result == null || result.Age < 0L)
						{
							return null;
						}
						if (result != null && result.Age > KEraData.Age)
						{
							KEraData.Age = result.Age;
							if (null != result.Bytes0)
							{
								KEraData.V = DataHelper2.BytesToObject<KFEraData>(result.Bytes0, 0, result.Bytes0.Length);
							}
							if (null == KEraData.V)
							{
								KEraData.V = new KFEraData();
							}
						}
						if (KEraData.V.EraID != this.JunTuanEraID)
						{
							this.JunTuanEraID = KEraData.V.EraID;
						}
						return KEraData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		public List<KFEraRankData> GetJunTuanEraRankData(bool noWait = false)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(noWait);
				if (null != kuaFuService)
				{
					try
					{
						lock (this.Mutex)
						{
							if (noWait)
							{
								return this.EraRankList.V;
							}
							KuaFuCmdData result = kuaFuService.GetEraRankData(this.EraRankList.Age);
							if (result == null || result.Age < 0L)
							{
								return null;
							}
							if (result != null && result.Age > this.EraRankList.Age)
							{
								this.EraRankList.Age = result.Age;
								if (null != result.Bytes0)
								{
									this.EraRankList.V = DataHelper2.BytesToObject<List<KFEraRankData>>(result.Bytes0, 0, result.Bytes0.Length);
								}
								if (null == this.EraRankList.V)
								{
									this.EraRankList.V = new List<KFEraRankData>();
								}
							}
							return this.EraRankList.V;
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		
		private static JunTuanClient instance = new JunTuanClient();

		
		public static SafeLock LockRoot = new SafeLock(null, 0, false);

		
		private SafeLock Mutex = new SafeLock(JunTuanClient.LockRoot, 2, false);

		
		private SafeLock RemotingMutex = new SafeLock(JunTuanClient.LockRoot, 1, false);

		
		private ICoreInterface CoreInterface = null;

		
		private IJunTuanService KuaFuService = null;

		
		private bool ClientInitialized = false;

		
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		
		private int CurrentRequestCount = 0;

		
		private int MaxRequestCount = 50;

		
		private Dictionary<int, JunTuanDetailData> JunTuanDetailDataDict = new Dictionary<int, JunTuanDetailData>();

		
		private Dictionary<int, JunTuanBaseData> JunTuanBaseDataDict = new Dictionary<int, JunTuanBaseData>();

		
		private KuaFuData<List<JunTuanBaseData>> JunTuanBaseDataList = new KuaFuData<List<JunTuanBaseData>>();

		
		private KuaFuData<List<JunTuanRankData>> JunTuanRankDataList = new KuaFuData<List<JunTuanRankData>>();

		
		private KuaFuData<List<JunTuanEventLog>> JunTuanEventLogList = new KuaFuData<List<JunTuanEventLog>>();

		
		private Dictionary<int, int> BangHuiJunTuanIdDict = new Dictionary<int, int>();

		
		private Dictionary<int, KuaFuData<KarenFuBenData>> KarenFuBenDataDict = new Dictionary<int, KuaFuData<KarenFuBenData>>();

		
		private Dictionary<int, KFPrisonRoleAllData> YaoSaiPrisonRoleDataDict = new Dictionary<int, KFPrisonRoleAllData>();

		
		private Dictionary<int, KFPrisonFuLuAllData> YaoSaiOwnerVsFuLuDict = new Dictionary<int, KFPrisonFuLuAllData>();

		
		private Dictionary<int, KFPrisonJingJiAllData> YaoSaiPrisonJingJiDataDict = new Dictionary<int, KFPrisonJingJiAllData>();

		
		private Dictionary<int, KFPrisonLogAllData> YaoSaiPrisonLogDataDict = new Dictionary<int, KFPrisonLogAllData>();

		
		private KuaFuData<List<KFEraRankData>> EraRankList = new KuaFuData<List<KFEraRankData>>();

		
		private Dictionary<int, KuaFuData<KFEraData>> EraDataDict = new Dictionary<int, KuaFuData<KFEraData>>();

		
		private volatile int JunTuanEraID = -1;

		
		private DateTime NextClearFuBenTime;

		
		private string RemoteServiceUri = null;

		
		private long AsyncDataItemAge;

		
		private DuplexChannelFactory<IJunTuanService> channelFactory;

		
		private KuaFuData<List<JunTuanMiniData>> JunTuanList = new KuaFuData<List<JunTuanMiniData>>();

		
		private Dictionary<int, KuaFuData<JunTuanTaskAllData>> JunTuanTaskAllDataDict = new Dictionary<int, KuaFuData<JunTuanTaskAllData>>();

		
		private Dictionary<int, KuaFuData<List<JunTuanRequestData>>> JunTuanRequestListDict = new Dictionary<int, KuaFuData<List<JunTuanRequestData>>>();
	}
}
