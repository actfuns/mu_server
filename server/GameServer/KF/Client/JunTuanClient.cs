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
	// Token: 0x02000313 RID: 787
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class JunTuanClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		// Token: 0x06000C6D RID: 3181 RVA: 0x000C2030 File Offset: 0x000C0230
		public static JunTuanClient getInstance()
		{
			return JunTuanClient.instance;
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x000C2048 File Offset: 0x000C0248
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x000C205C File Offset: 0x000C025C
		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 21;
			this.ClientInfo.Token = this.CoreInterface.GetLocalAddressIPs();
			return true;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x000C20BC File Offset: 0x000C02BC
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x000C20D0 File Offset: 0x000C02D0
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x000C20E4 File Offset: 0x000C02E4
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x000C20F8 File Offset: 0x000C02F8
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

		// Token: 0x06000C74 RID: 3188 RVA: 0x000C214C File Offset: 0x000C034C
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

		// Token: 0x06000C75 RID: 3189 RVA: 0x000C2254 File Offset: 0x000C0454
		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("JunTuanUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
			}
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x000C22C4 File Offset: 0x000C04C4
		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x000C22CE File Offset: 0x000C04CE
		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x000C22D8 File Offset: 0x000C04D8
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

		// Token: 0x06000C79 RID: 3193 RVA: 0x000C2554 File Offset: 0x000C0754
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

		// Token: 0x06000C7A RID: 3194 RVA: 0x000C2584 File Offset: 0x000C0784
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

		// Token: 0x06000C7B RID: 3195 RVA: 0x000C282C File Offset: 0x000C0A2C
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

		// Token: 0x06000C7C RID: 3196 RVA: 0x000C28B8 File Offset: 0x000C0AB8
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

		// Token: 0x06000C7D RID: 3197 RVA: 0x000C2940 File Offset: 0x000C0B40
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

		// Token: 0x06000C7E RID: 3198 RVA: 0x000C29C8 File Offset: 0x000C0BC8
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

		// Token: 0x06000C7F RID: 3199 RVA: 0x000C2A3C File Offset: 0x000C0C3C
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

		// Token: 0x06000C80 RID: 3200 RVA: 0x000C2AB0 File Offset: 0x000C0CB0
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

		// Token: 0x06000C81 RID: 3201 RVA: 0x000C2B38 File Offset: 0x000C0D38
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

		// Token: 0x06000C82 RID: 3202 RVA: 0x000C2BC0 File Offset: 0x000C0DC0
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

		// Token: 0x06000C83 RID: 3203 RVA: 0x000C2C48 File Offset: 0x000C0E48
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

		// Token: 0x06000C84 RID: 3204 RVA: 0x000C2CC8 File Offset: 0x000C0EC8
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

		// Token: 0x06000C85 RID: 3205 RVA: 0x000C2D4C File Offset: 0x000C0F4C
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

		// Token: 0x06000C86 RID: 3206 RVA: 0x000C2F08 File Offset: 0x000C1108
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

		// Token: 0x06000C87 RID: 3207 RVA: 0x000C30D0 File Offset: 0x000C12D0
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

		// Token: 0x06000C88 RID: 3208 RVA: 0x000C312C File Offset: 0x000C132C
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

		// Token: 0x06000C89 RID: 3209 RVA: 0x000C3184 File Offset: 0x000C1384
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

		// Token: 0x06000C8A RID: 3210 RVA: 0x000C354C File Offset: 0x000C174C
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

		// Token: 0x06000C8B RID: 3211 RVA: 0x000C3660 File Offset: 0x000C1860
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

		// Token: 0x06000C8C RID: 3212 RVA: 0x000C36B0 File Offset: 0x000C18B0
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

		// Token: 0x06000C8D RID: 3213 RVA: 0x000C388C File Offset: 0x000C1A8C
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

		// Token: 0x06000C8E RID: 3214 RVA: 0x000C3A24 File Offset: 0x000C1C24
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

		// Token: 0x06000C8F RID: 3215 RVA: 0x000C3C40 File Offset: 0x000C1E40
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

		// Token: 0x06000C90 RID: 3216 RVA: 0x000C3CF8 File Offset: 0x000C1EF8
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

		// Token: 0x06000C91 RID: 3217 RVA: 0x000C3D64 File Offset: 0x000C1F64
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

		// Token: 0x06000C92 RID: 3218 RVA: 0x000C3DC8 File Offset: 0x000C1FC8
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

		// Token: 0x06000C93 RID: 3219 RVA: 0x000C3E1C File Offset: 0x000C201C
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

		// Token: 0x06000C94 RID: 3220 RVA: 0x000C3E60 File Offset: 0x000C2060
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

		// Token: 0x06000C95 RID: 3221 RVA: 0x000C3EAC File Offset: 0x000C20AC
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

		// Token: 0x06000C96 RID: 3222 RVA: 0x000C40AC File Offset: 0x000C22AC
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

		// Token: 0x06000C97 RID: 3223 RVA: 0x000C416C File Offset: 0x000C236C
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

		// Token: 0x06000C98 RID: 3224 RVA: 0x000C41FC File Offset: 0x000C23FC
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

		// Token: 0x06000C99 RID: 3225 RVA: 0x000C43B4 File Offset: 0x000C25B4
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

		// Token: 0x06000C9A RID: 3226 RVA: 0x000C4450 File Offset: 0x000C2650
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

		// Token: 0x06000C9B RID: 3227 RVA: 0x000C44AC File Offset: 0x000C26AC
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

		// Token: 0x06000C9C RID: 3228 RVA: 0x000C4660 File Offset: 0x000C2860
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

		// Token: 0x06000C9D RID: 3229 RVA: 0x000C46C8 File Offset: 0x000C28C8
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

		// Token: 0x06000C9E RID: 3230 RVA: 0x000C4720 File Offset: 0x000C2920
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

		// Token: 0x06000C9F RID: 3231 RVA: 0x000C486C File Offset: 0x000C2A6C
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

		// Token: 0x06000CA0 RID: 3232 RVA: 0x000C49C4 File Offset: 0x000C2BC4
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

		// Token: 0x06000CA1 RID: 3233 RVA: 0x000C4A18 File Offset: 0x000C2C18
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

		// Token: 0x06000CA2 RID: 3234 RVA: 0x000C4A6C File Offset: 0x000C2C6C
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

		// Token: 0x06000CA3 RID: 3235 RVA: 0x000C4AC4 File Offset: 0x000C2CC4
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

		// Token: 0x06000CA4 RID: 3236 RVA: 0x000C4B28 File Offset: 0x000C2D28
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

		// Token: 0x06000CA5 RID: 3237 RVA: 0x000C4B7C File Offset: 0x000C2D7C
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

		// Token: 0x06000CA6 RID: 3238 RVA: 0x000C4BD4 File Offset: 0x000C2DD4
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

		// Token: 0x06000CA7 RID: 3239 RVA: 0x000C4C30 File Offset: 0x000C2E30
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

		// Token: 0x06000CA8 RID: 3240 RVA: 0x000C4C88 File Offset: 0x000C2E88
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

		// Token: 0x06000CA9 RID: 3241 RVA: 0x000C4CDC File Offset: 0x000C2EDC
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

		// Token: 0x06000CAA RID: 3242 RVA: 0x000C4D30 File Offset: 0x000C2F30
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

		// Token: 0x06000CAB RID: 3243 RVA: 0x000C4D80 File Offset: 0x000C2F80
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

		// Token: 0x06000CAC RID: 3244 RVA: 0x000C4DD8 File Offset: 0x000C2FD8
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

		// Token: 0x06000CAD RID: 3245 RVA: 0x000C4E2C File Offset: 0x000C302C
		public void ClearYaoSaiPrisonData(int roleID)
		{
			lock (this.Mutex)
			{
			}
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x000C4E74 File Offset: 0x000C3074
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

		// Token: 0x06000CAF RID: 3247 RVA: 0x000C5060 File Offset: 0x000C3260
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

		// Token: 0x06000CB0 RID: 3248 RVA: 0x000C524C File Offset: 0x000C344C
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

		// Token: 0x06000CB1 RID: 3249 RVA: 0x000C5438 File Offset: 0x000C3638
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

		// Token: 0x06000CB2 RID: 3250 RVA: 0x000C5624 File Offset: 0x000C3824
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

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000C57AC File Offset: 0x000C39AC
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

		// Token: 0x06000CB4 RID: 3252 RVA: 0x000C5804 File Offset: 0x000C3A04
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

		// Token: 0x06000CB5 RID: 3253 RVA: 0x000C5860 File Offset: 0x000C3A60
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

		// Token: 0x06000CB6 RID: 3254 RVA: 0x000C58B4 File Offset: 0x000C3AB4
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

		// Token: 0x06000CB7 RID: 3255 RVA: 0x000C590C File Offset: 0x000C3B0C
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

		// Token: 0x06000CB8 RID: 3256 RVA: 0x000C5960 File Offset: 0x000C3B60
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

		// Token: 0x06000CB9 RID: 3257 RVA: 0x000C5AA0 File Offset: 0x000C3CA0
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

		// Token: 0x06000CBA RID: 3258 RVA: 0x000C5C94 File Offset: 0x000C3E94
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

		// Token: 0x04001428 RID: 5160
		private static JunTuanClient instance = new JunTuanClient();

		// Token: 0x04001429 RID: 5161
		public static SafeLock LockRoot = new SafeLock(null, 0, false);

		// Token: 0x0400142A RID: 5162
		private SafeLock Mutex = new SafeLock(JunTuanClient.LockRoot, 2, false);

		// Token: 0x0400142B RID: 5163
		private SafeLock RemotingMutex = new SafeLock(JunTuanClient.LockRoot, 1, false);

		// Token: 0x0400142C RID: 5164
		private ICoreInterface CoreInterface = null;

		// Token: 0x0400142D RID: 5165
		private IJunTuanService KuaFuService = null;

		// Token: 0x0400142E RID: 5166
		private bool ClientInitialized = false;

		// Token: 0x0400142F RID: 5167
		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		// Token: 0x04001430 RID: 5168
		private int CurrentRequestCount = 0;

		// Token: 0x04001431 RID: 5169
		private int MaxRequestCount = 50;

		// Token: 0x04001432 RID: 5170
		private Dictionary<int, JunTuanDetailData> JunTuanDetailDataDict = new Dictionary<int, JunTuanDetailData>();

		// Token: 0x04001433 RID: 5171
		private Dictionary<int, JunTuanBaseData> JunTuanBaseDataDict = new Dictionary<int, JunTuanBaseData>();

		// Token: 0x04001434 RID: 5172
		private KuaFuData<List<JunTuanBaseData>> JunTuanBaseDataList = new KuaFuData<List<JunTuanBaseData>>();

		// Token: 0x04001435 RID: 5173
		private KuaFuData<List<JunTuanRankData>> JunTuanRankDataList = new KuaFuData<List<JunTuanRankData>>();

		// Token: 0x04001436 RID: 5174
		private KuaFuData<List<JunTuanEventLog>> JunTuanEventLogList = new KuaFuData<List<JunTuanEventLog>>();

		// Token: 0x04001437 RID: 5175
		private Dictionary<int, int> BangHuiJunTuanIdDict = new Dictionary<int, int>();

		// Token: 0x04001438 RID: 5176
		private Dictionary<int, KuaFuData<KarenFuBenData>> KarenFuBenDataDict = new Dictionary<int, KuaFuData<KarenFuBenData>>();

		// Token: 0x04001439 RID: 5177
		private Dictionary<int, KFPrisonRoleAllData> YaoSaiPrisonRoleDataDict = new Dictionary<int, KFPrisonRoleAllData>();

		// Token: 0x0400143A RID: 5178
		private Dictionary<int, KFPrisonFuLuAllData> YaoSaiOwnerVsFuLuDict = new Dictionary<int, KFPrisonFuLuAllData>();

		// Token: 0x0400143B RID: 5179
		private Dictionary<int, KFPrisonJingJiAllData> YaoSaiPrisonJingJiDataDict = new Dictionary<int, KFPrisonJingJiAllData>();

		// Token: 0x0400143C RID: 5180
		private Dictionary<int, KFPrisonLogAllData> YaoSaiPrisonLogDataDict = new Dictionary<int, KFPrisonLogAllData>();

		// Token: 0x0400143D RID: 5181
		private KuaFuData<List<KFEraRankData>> EraRankList = new KuaFuData<List<KFEraRankData>>();

		// Token: 0x0400143E RID: 5182
		private Dictionary<int, KuaFuData<KFEraData>> EraDataDict = new Dictionary<int, KuaFuData<KFEraData>>();

		// Token: 0x0400143F RID: 5183
		private volatile int JunTuanEraID = -1;

		// Token: 0x04001440 RID: 5184
		private DateTime NextClearFuBenTime;

		// Token: 0x04001441 RID: 5185
		private string RemoteServiceUri = null;

		// Token: 0x04001442 RID: 5186
		private long AsyncDataItemAge;

		// Token: 0x04001443 RID: 5187
		private DuplexChannelFactory<IJunTuanService> channelFactory;

		// Token: 0x04001444 RID: 5188
		private KuaFuData<List<JunTuanMiniData>> JunTuanList = new KuaFuData<List<JunTuanMiniData>>();

		// Token: 0x04001445 RID: 5189
		private Dictionary<int, KuaFuData<JunTuanTaskAllData>> JunTuanTaskAllDataDict = new Dictionary<int, KuaFuData<JunTuanTaskAllData>>();

		// Token: 0x04001446 RID: 5190
		private Dictionary<int, KuaFuData<List<JunTuanRequestData>>> JunTuanRequestListDict = new Dictionary<int, KuaFuData<List<JunTuanRequestData>>>();
	}
}
