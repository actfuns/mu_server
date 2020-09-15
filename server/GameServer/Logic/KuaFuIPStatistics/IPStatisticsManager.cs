using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using KF.Client;
using KF.Contract.Data;
using Server.TCP;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic.KuaFuIPStatistics
{
	// Token: 0x0200034C RID: 844
	public class IPStatisticsManager : IManager, IEventListener
	{
		// Token: 0x06000E3D RID: 3645 RVA: 0x000E1170 File Offset: 0x000DF370
		public static IPStatisticsManager getInstance()
		{
			return IPStatisticsManager.instance;
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x000E1188 File Offset: 0x000DF388
		public bool initialize()
		{
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.PlayerCreateRoleBeBan, IPInfoType.createRoleFailByBan);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.LoginFailByDataCheck, IPInfoType.LoginFailByDataCheckCnt);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.LoginFailByUserBan, IPInfoType.LoginFailByBanCnt);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.LoginFailByTimeout, IPInfoType.LoginFailByLoginTimeOutCnt);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.LoginSuccess, IPInfoType.LoginSuccessCnt);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.PlayerInitGameAsync, IPInfoType.InitGameSuccessCnt);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.PlayerInitGameBeBan, IPInfoType.InitGameFailByBanCnt);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.PlayerCreateRoleLimit, IPInfoType.createRoleFailByCnt);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.SocketConnect, IPInfoType.ConnectCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.LoginFailByDataCheck, UserParamType.Begin);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.LoginFailByUserBan, UserParamType.LoginFailByBanCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.LoginFailByTimeout, UserParamType.LoginFailByLoginTimeOutCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.PlayerInitGameBeBan, UserParamType.InitGameFailByBanCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.PlayerCreateRoleLimit, UserParamType.createRoleFailByCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.PlayerCreateRoleBeBan, UserParamType.createRoleFailByBan);
			GlobalEventSource.getInstance().registerListener(43, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(46, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(47, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(48, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(45, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(15, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(42, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(44, IPStatisticsManager.getInstance());
			this.LoadConfig();
			return true;
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x000E1308 File Offset: 0x000DF508
		public void LoadConfig()
		{
			try
			{
				XElement xml = ConfigHelper.Load(Global.GameResPath("Config/IPPassList.xml"));
				if (null != xml)
				{
					List<IPPassList> NewIPPassList = new List<IPPassList>();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						IPPassList value = new IPPassList();
						value.ID = Convert.ToInt32(xmlItem.Attribute("ID").Value.ToString());
						string MinIP = xmlItem.Attribute("MinIP").Value.ToString();
						value.MinIP = IpHelper.IpToInt(MinIP);
						string MaxIP = xmlItem.Attribute("MaxIP").Value.ToString();
						value.MaxIP = IpHelper.IpToInt(MaxIP);
						NewIPPassList.Add(value);
					}
					List<StatisticsControl> MyUserIDControlList = new List<StatisticsControl>();
					xml = XElement.Load(Global.GameResPath("Config/UserIDStaristicsConfig.xml"));
					if (null != xml)
					{
						foreach (XElement xmlItem in xml.Elements())
						{
							MyUserIDControlList.Add(new StatisticsControl
							{
								ID = Convert.ToInt32(xmlItem.Attribute("ID").Value.ToString()),
								ParamType = Convert.ToInt32(xmlItem.Attribute("ParamType").Value.ToString()),
								ParamLimit = Convert.ToInt32(xmlItem.Attribute("ParamLimit").Value.ToString()),
								ComParamType = Convert.ToInt32(xmlItem.Attribute("ComParamType").Value.ToString()),
								ComParamLimit = Convert.ToDouble(xmlItem.Attribute("ComParamLimit").Value.ToString()),
								OperaType = Convert.ToInt32(xmlItem.Attribute("OperaType").Value.ToString()),
								OperaParam = Convert.ToInt32(xmlItem.Attribute("OperaParam").Value.ToString()),
								Local = (xmlItem.Attribute("OperaParam").Value == "1")
							});
						}
						this._IPPassList = NewIPPassList;
						IPStatisticsManager._UserIDControlList = MyUserIDControlList;
						IPStatisticsManager.bBeReload = true;
						HashSet<string> UserIDPass = new HashSet<string>();
						xml = XElement.Load(Global.GameResPath("Config/UserIDPassList.xml"));
						if (null != xml)
						{
							foreach (XElement xmlItem in xml.Elements())
							{
								string strParams = xmlItem.Attribute("UserID").Value.ToString();
								string[] strFields = strParams.Split(new char[]
								{
									','
								});
								foreach (string field in strFields)
								{
									try
									{
										UserIDPass.Add(field);
									}
									catch
									{
									}
								}
							}
							IPStatisticsManager._UserIDPass = UserIDPass;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString() + "xmlFileName=IPPassList.xml");
			}
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x000E1754 File Offset: 0x000DF954
		private bool isCanPassIP(long ipAsInt)
		{
			bool result;
			lock (this._IPPassList)
			{
				if (this._IPPassList == null || this._IPPassList.Count == 0)
				{
					result = false;
				}
				else
				{
					foreach (IPPassList data in this._IPPassList)
					{
						if (data.MinIP <= ipAsInt && data.MaxIP >= ipAsInt)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x000E1830 File Offset: 0x000DFA30
		private bool isCanPassUserID(string userID)
		{
			return IPStatisticsManager._UserIDPass.Contains(userID);
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x000E1850 File Offset: 0x000DFA50
		private bool checkUserID(int[] Count, StatisticsControl config)
		{
			bool bPass = true;
			if (config.ParamLimit > 0)
			{
				if (Count[config.ParamType] >= config.ParamLimit)
				{
					bPass = false;
				}
			}
			if (!bPass)
			{
				if (config.ComParamType >= 0)
				{
					double coe = double.MaxValue;
					if (Count[config.ComParamType] > 0)
					{
						coe = (double)Count[config.ParamType] * 1.0 / (double)Count[config.ComParamType];
					}
					bPass = ((config.ComParamLimit > 0.0) ? (coe > config.ComParamLimit) : (coe < -config.ComParamLimit));
				}
			}
			return bPass;
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x000E1918 File Offset: 0x000DFB18
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x000E192C File Offset: 0x000DFB2C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x000E1940 File Offset: 0x000DFB40
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x000E1954 File Offset: 0x000DFB54
		public void processEvent(EventObject eventObject)
		{
			if (!GameManager.IsKuaFuServer)
			{
				IpEventBase e = eventObject as IpEventBase;
				IPStatisticsData ipData = null;
				if (eventObject.getEventType() == 15)
				{
					PlayerInitGameAsyncEventObject e2 = eventObject as PlayerInitGameAsyncEventObject;
					if (null != e2)
					{
						GameClient client = e2.getPlayer();
						this.SetUserIdValue(client.strUserID, (int)client.ClientData.VipExp, Global.GetUnionLevel(client, false));
					}
				}
				lock (IPStatisticsManager.dictIPStatisticsData)
				{
					if (e.getIpAsInt() > 0L)
					{
						if (!IPStatisticsManager.dictIPStatisticsData.TryGetValue(e.getIpAsInt(), out ipData))
						{
							ipData = new IPStatisticsData
							{
								ipAsInt = e.getIpAsInt()
							};
							IPStatisticsManager.dictIPStatisticsData.Add(e.getIpAsInt(), ipData);
						}
						if (null == ipData)
						{
							return;
						}
						IPInfoType ipInfoType = IPInfoType.Begin;
						if (IPStatisticsManager.event2IPTypeDict.TryGetValue((EventTypes)eventObject.getEventType(), out ipInfoType))
						{
							ipData.IPInfoParams[(int)ipInfoType]++;
						}
					}
				}
				UserParamType userInfoType = UserParamType.Begin;
				if (IPStatisticsManager.event2UserTypeDict.TryGetValue((EventTypes)eventObject.getEventType(), out userInfoType))
				{
					if (!string.IsNullOrEmpty(e.getUserID()))
					{
						UserIDState userData = null;
						lock (this.dictUserStateData)
						{
							if (!this.dictUserStateData.TryGetValue(e.getUserID(), out userData))
							{
								userData = new UserIDState();
								this.dictUserStateData.Add(e.getUserID(), userData);
							}
							if (null != userData)
							{
								userData.Count[(int)userInfoType]++;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x000E1B8C File Offset: 0x000DFD8C
		public List<IPStatisticsData> getCurrDataList()
		{
			List<IPStatisticsData> result;
			lock (IPStatisticsManager.dictIPStatisticsData)
			{
				Dictionary<long, Tuple<int, int, int>> tmpDict = Global._TCPManager.MySocketListener.GetSocketCnt();
				foreach (KeyValuePair<long, Tuple<int, int, int>> item in tmpDict)
				{
					IPStatisticsData IPData = null;
					if (!IPStatisticsManager.dictIPStatisticsData.TryGetValue(item.Key, out IPData))
					{
						IPData = new IPStatisticsData
						{
							ipAsInt = item.Key
						};
						IPStatisticsManager.dictIPStatisticsData[item.Key] = IPData;
					}
					IPData.IPInfoParams[0] = item.Value.Item1;
					IPData.IPInfoParams[1] = item.Value.Item2;
					IPData.IPInfoParams[2] = item.Value.Item3;
				}
				List<IPStatisticsData> tempList = IPStatisticsManager.dictIPStatisticsData.Values.ToList<IPStatisticsData>();
				IPStatisticsManager.dictIPStatisticsData.Clear();
				result = tempList;
			}
			return result;
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x000E1CD0 File Offset: 0x000DFED0
		public void TimerProcForIP()
		{
			if (!GameManager.IsKuaFuServer)
			{
				long currTicks = TimeUtil.NOW();
				if (currTicks - IPStatisticsManager.updateTicks >= 30000L)
				{
					IPStatisticsManager.updateTicks = currTicks;
					int centerMinite = IPStatisticsClient.getInstance().RequestMinite();
					if (centerMinite != IPStatisticsManager.lastMinite)
					{
						IPStatisticsManager.lastMinite = centerMinite;
						this.RequestResult();
						this.ReportProc();
					}
				}
			}
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x000E1D44 File Offset: 0x000DFF44
		public void TimerProcForUserID()
		{
			int currMinite = Global.GetOffsetMiniteNow();
			if (currMinite != IPStatisticsManager.lastUserIDMinite)
			{
				IPStatisticsManager.lastUserIDMinite = currMinite;
				long currTicks = TimeUtil.NOW();
				List<UserIDState> UserIDStateList = null;
				lock (this.dictUserStateData)
				{
					UserIDStateList = this.dictUserStateData.Values.ToList<UserIDState>();
					this.dictUserStateData.Clear();
				}
				if (IPStatisticsManager.bBeReload)
				{
					IPStatisticsManager.bBeReload = false;
					lock (this.dictOperaUserID)
					{
						foreach (KeyValuePair<string, UserOperaData> operaData in this.dictOperaUserID)
						{
							for (int i = 0; i < 4; i++)
							{
								operaData.Value.OperaTime[i] = 0;
								operaData.Value.OperaCount[i] = 0;
							}
							foreach (StatisticsControl config in IPStatisticsManager._UserIDControlList)
							{
								if (!this.checkUserID(operaData.Value.AllCount, config))
								{
									if (config.OperaType >= 0)
									{
										operaData.Value.OperaTime[config.OperaType] = config.OperaParam;
										operaData.Value.OperaCount[config.OperaType] = operaData.Value.AllCount[config.ParamType];
									}
								}
							}
						}
					}
				}
				foreach (UserIDState userIDState in UserIDStateList)
				{
					UserOperaData operaData2 = null;
					foreach (StatisticsControl config in IPStatisticsManager._UserIDControlList)
					{
						if (!this.checkUserID(userIDState.Count, config))
						{
							lock (this.dictOperaUserID)
							{
								if (!this.dictOperaUserID.TryGetValue(userIDState.UserID, out operaData2))
								{
									operaData2 = new UserOperaData();
									operaData2.UserID = userIDState.UserID;
									this.dictOperaUserID.Add(userIDState.UserID, operaData2);
								}
							}
							operaData2.IPAsInt = userIDState.IPAsInt;
							if (config.OperaType >= 0)
							{
								if ((long)config.OperaParam + currTicks > operaData2.createTicks + (long)operaData2.OperaTime[config.OperaType])
								{
									operaData2.createTicks = currTicks;
									operaData2.OperaTime[config.OperaType] = config.OperaParam;
								}
								if (userIDState.Count[config.ParamType] > operaData2.OperaCount[config.OperaType])
								{
									operaData2.OperaCount[config.OperaType] = userIDState.Count[config.ParamType];
								}
								for (int i = 0; i < 6; i++)
								{
									operaData2.AllCount[i] = userIDState.Count[i];
								}
							}
							string logmsg = string.Format("cant pass userid={0}:{1} ruleid={2} paramValue={3}", new object[]
							{
								operaData2.IPAsInt,
								IpHelper.IntToIp(operaData2.IPAsInt),
								config.ID,
								userIDState.Count[config.ParamType]
							});
							if (config.ComParamType >= 0)
							{
								logmsg += string.Format(" comParamValue={0}", userIDState.Count[config.ComParamType]);
							}
							LogManager.WriteLog(LogTypes.UserIDStatistics, logmsg, null, true);
						}
					}
				}
				List<string> needDelUserIDList = new List<string>();
				lock (this.dictOperaUserID)
				{
					foreach (KeyValuePair<string, UserOperaData> userIDData in this.dictOperaUserID)
					{
						bool needDel = true;
						if (this.isCanPassUserID(userIDData.Key))
						{
							needDel = true;
						}
						else
						{
							for (int i = 0; i < 4; i++)
							{
								if (userIDData.Value.createTicks + (long)(userIDData.Value.OperaTime[i] * 1000) > currTicks)
								{
									needDel = false;
									break;
								}
							}
						}
						if (needDel)
						{
							needDelUserIDList.Add(userIDData.Key);
						}
					}
					foreach (string ip in needDelUserIDList)
					{
						this.dictOperaUserID.Remove(ip);
					}
				}
			}
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x000E23FC File Offset: 0x000E05FC
		public void RequestResult()
		{
			long currTicks = TimeUtil.NOW();
			List<long> needDelIPList = new List<long>();
			lock (IPStatisticsManager.dictOperaMothod)
			{
				foreach (KeyValuePair<long, IPOperaData> ipData in IPStatisticsManager.dictOperaMothod)
				{
					bool needDel = true;
					if (this.isCanPassIP(ipData.Key))
					{
						needDel = true;
					}
					else
					{
						for (int i = 0; i < 4; i++)
						{
							if (ipData.Value.recvTicks + (long)(ipData.Value.OperaTime[i] * 1000) > currTicks)
							{
								needDel = false;
								break;
							}
						}
					}
					if (needDel)
					{
						needDelIPList.Add(ipData.Key);
					}
				}
				foreach (long ip in needDelIPList)
				{
					IPStatisticsManager.dictOperaMothod.Remove(ip);
				}
			}
			List<IPOperaData> resultList = IPStatisticsClient.getInstance().GetIPStatisticsResult();
			LogManager.WriteLog(LogTypes.IPStatistics, string.Format("request ip data minite={0} count={1}", IPStatisticsManager.lastMinite, (resultList == null) ? 0 : resultList.Count), null, true);
			if (resultList != null && resultList.Count > 0)
			{
				lock (IPStatisticsManager.dictOperaMothod)
				{
					foreach (IPOperaData ipData2 in resultList)
					{
						if (!this.isCanPassIP(ipData2.ipAsInt))
						{
							IPOperaData oldData = null;
							if (IPStatisticsManager.dictOperaMothod.TryGetValue(ipData2.ipAsInt, out oldData))
							{
								for (int i = 0; i < 4; i++)
								{
									if (currTicks + (long)(ipData2.OperaTime[i] * 1000) > oldData.recvTicks + (long)(oldData.OperaTime[i] * 1000))
									{
										oldData.recvTicks = currTicks;
										oldData.OperaTime[i] = ipData2.OperaTime[i];
									}
								}
							}
							else
							{
								ipData2.recvTicks = currTicks;
								IPStatisticsManager.dictOperaMothod.Add(ipData2.ipAsInt, ipData2);
							}
							LogManager.WriteLog(LogTypes.IPStatistics, string.Format("recv need ip minite={0} ip={1}:{2} ", IPStatisticsManager.lastMinite, ipData2.ipAsInt, IpHelper.IntToIp(ipData2.ipAsInt)), null, true);
						}
					}
				}
			}
		}

		// Token: 0x06000E4B RID: 3659 RVA: 0x000E278C File Offset: 0x000E098C
		public void ReportProc()
		{
			List<IPStatisticsData> ipDataList = this.getCurrDataList();
			if (ipDataList != null && ipDataList.Count > 0)
			{
				int result = IPStatisticsClient.getInstance().IPStatisticsDataReport(IPStatisticsManager.lastMinite, ipDataList);
				LogManager.WriteLog(LogTypes.IPStatistics, string.Format("report ip data minite={0} count={1} result={2}", IPStatisticsManager.lastMinite, ipDataList.Count, result), null, true);
			}
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x000E27F8 File Offset: 0x000E09F8
		public bool GetIPInBeOperation(TMSKSocket socket, IPOperaType type)
		{
			if (type == IPOperaType.BanConnect)
			{
				this.processEvent(new IpEventBase(49, socket.AcceptIpAsInt, socket.UserID));
			}
			long ipAsInt = Global.GetIpAsIntSafe(socket);
			lock (IPStatisticsManager.dictOperaMothod)
			{
				IPOperaData oldData = null;
				if (!IPStatisticsManager.dictOperaMothod.TryGetValue(ipAsInt, out oldData))
				{
					return false;
				}
				if (oldData.recvTicks + (long)(oldData.OperaTime[(int)type] * 1000) > TimeUtil.NOW())
				{
					if (type != IPOperaType.BanConnect)
					{
						string userID = socket.UserID;
						if (!string.IsNullOrEmpty(userID))
						{
							if (this.CheckUserIdValue(userID, "ThisIPPass"))
							{
								return false;
							}
						}
					}
					LogManager.WriteLog(LogTypes.IPStatistics, string.Format("Operation {0} ip={1}", type.ToString(), Global.GetIPAddress(socket)), null, true);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x000E2920 File Offset: 0x000E0B20
		public bool GetUserIDInBeOperation(string userid, IPOperaType type)
		{
			lock (this.dictOperaUserID)
			{
				UserOperaData oldData = null;
				if (!this.dictOperaUserID.TryGetValue(userid, out oldData))
				{
					return false;
				}
				if (oldData.createTicks + (long)(oldData.OperaTime[(int)type] * 1000) > TimeUtil.NOW())
				{
					if (this.CheckUserIdValue(userid, "ThisUserIDPass"))
					{
						return false;
					}
					LogManager.WriteLog(LogTypes.UserIDStatistics, string.Format("Operation {0} ip={1}", type.ToString(), userid), null, true);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x000E29F0 File Offset: 0x000E0BF0
		public void SetUserIdValue(string userID, int vipExp, int unionLevel)
		{
			lock (IPStatisticsManager._UserIdValueDict)
			{
				int[] arr2;
				if (IPStatisticsManager._UserIdValueDict.TryGetValue(userID, out arr2) && null != arr2)
				{
					if (vipExp > arr2[0])
					{
						arr2[0] = vipExp;
					}
					if (unionLevel > arr2[1])
					{
						arr2[1] = unionLevel;
					}
				}
				else
				{
					IPStatisticsManager._UserIdValueDict[userID] = new int[]
					{
						vipExp,
						unionLevel
					};
				}
			}
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x000E2A9C File Offset: 0x000E0C9C
		public int[] GetUserIdValue(string userID)
		{
			try
			{
				int[] arr;
				lock (IPStatisticsManager._UserIdValueDict)
				{
					if (IPStatisticsManager._UserIdValueDict.TryGetValue(userID, out arr))
					{
						return arr;
					}
				}
				arr = GameManager.ClientMgr.QueryUserIdValue(userID, 0);
				lock (IPStatisticsManager._UserIdValueDict)
				{
					int[] arr2;
					if (IPStatisticsManager._UserIdValueDict.TryGetValue(userID, out arr2))
					{
						return arr2;
					}
					IPStatisticsManager._UserIdValueDict[userID] = arr;
				}
				return arr;
			}
			catch (Exception ex)
			{
			}
			return null;
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x000E2B90 File Offset: 0x000E0D90
		public bool CheckUserIdValue(string userID, string configName)
		{
			if (!string.IsNullOrEmpty(userID))
			{
				int[] ids = GameManager.systemParamsList.GetParamValueIntArrayByName(configName, ',');
				if (ids != null && ids.Length == 3)
				{
					int[] arr = this.GetUserIdValue(userID);
					if (null != arr)
					{
						if (arr[0] >= ids[0])
						{
							return true;
						}
						if (arr[1] >= Global.GetUnionLevel2(ids[1], ids[2]))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0400163E RID: 5694
		private static IPStatisticsManager instance = new IPStatisticsManager();

		// Token: 0x0400163F RID: 5695
		private static Dictionary<long, IPStatisticsData> dictIPStatisticsData = new Dictionary<long, IPStatisticsData>();

		// Token: 0x04001640 RID: 5696
		private static int lastMinite = Global.GetOffsetMiniteNow();

		// Token: 0x04001641 RID: 5697
		private static long updateTicks = TimeUtil.NOW();

		// Token: 0x04001642 RID: 5698
		private static Dictionary<EventTypes, IPInfoType> event2IPTypeDict = new Dictionary<EventTypes, IPInfoType>();

		// Token: 0x04001643 RID: 5699
		private static Dictionary<long, IPOperaData> dictOperaMothod = new Dictionary<long, IPOperaData>();

		// Token: 0x04001644 RID: 5700
		private List<IPPassList> _IPPassList = new List<IPPassList>();

		// Token: 0x04001645 RID: 5701
		private static Dictionary<EventTypes, UserParamType> event2UserTypeDict = new Dictionary<EventTypes, UserParamType>();

		// Token: 0x04001646 RID: 5702
		private Dictionary<string, UserIDState> dictUserStateData = new Dictionary<string, UserIDState>();

		// Token: 0x04001647 RID: 5703
		private Dictionary<string, UserOperaData> dictOperaUserID = new Dictionary<string, UserOperaData>();

		// Token: 0x04001648 RID: 5704
		private static int lastUserIDMinite = Global.GetOffsetMiniteNow();

		// Token: 0x04001649 RID: 5705
		private static List<StatisticsControl> _UserIDControlList = new List<StatisticsControl>();

		// Token: 0x0400164A RID: 5706
		private static HashSet<string> _UserIDPass = new HashSet<string>();

		// Token: 0x0400164B RID: 5707
		private static Dictionary<string, int[]> _UserIdValueDict = new Dictionary<string, int[]>();

		// Token: 0x0400164C RID: 5708
		private static bool bBeReload = false;
	}
}
