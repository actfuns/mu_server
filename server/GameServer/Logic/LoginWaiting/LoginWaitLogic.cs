using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic.Name;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.LoginWaiting
{
	
	public class LoginWaitLogic
	{
		
		public void LoadConfig()
		{
			string strCfg = GameManager.PlatConfigMgr.GetGameConfigItemStr("userwaitconfig", "700,1000,400,30000,180000,20000");
			this.m_IntConfig[0] = Global.String2IntArray(strCfg, ',');
			strCfg = GameManager.PlatConfigMgr.GetGameConfigItemStr("vipwaitconfig", "900,1000,400,30000,180000,20000");
			this.m_IntConfig[1] = Global.String2IntArray(strCfg, ',');
		}

		
		public int GetConfig(LoginWaitLogic.UserType userType, LoginWaitLogic.ConfigType type)
		{
			int result;
			if (userType < LoginWaitLogic.UserType.Normal || userType >= LoginWaitLogic.UserType.Max_Type)
			{
				result = 0;
			}
			else if (type < LoginWaitLogic.ConfigType.NeedWaitNum || type >= (LoginWaitLogic.ConfigType)this.m_IntConfig[(int)userType].Length)
			{
				result = 0;
			}
			else
			{
				result = this.m_IntConfig[(int)userType][(int)type];
			}
			return result;
		}

		
		public LoginWaitLogic()
		{
			for (LoginWaitLogic.UserType i = LoginWaitLogic.UserType.Normal; i < LoginWaitLogic.UserType.Max_Type; i++)
			{
				this.m_UserList[(int)i] = new List<LoginWaitLogic.UserInfo>();
			}
		}

		
		public int GetTotalWaitingCount()
		{
			int result;
			lock (this.m_Mutex)
			{
				int nCount = 0;
				foreach (List<LoginWaitLogic.UserInfo> list in this.m_UserList)
				{
					nCount += list.Count;
				}
				result = nCount;
			}
			return result;
		}

		
		public int GetWaitingCount(LoginWaitLogic.UserType userType)
		{
			int count;
			lock (this.m_Mutex)
			{
				count = this.m_UserList[(int)userType].Count;
			}
			return count;
		}

		
		public bool IsInWait(string userID)
		{
			bool result;
			try
			{
				if (string.IsNullOrEmpty(userID))
				{
					result = false;
				}
				else
				{
					lock (this.m_Mutex)
					{
						result = this.m_User2SocketDict.ContainsKey(userID);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::IsInWait userID={0}", userID));
				result = false;
			}
			return result;
		}

		
		public bool AddToWait(string userID, int zoneID, LoginWaitLogic.UserType userType, TMSKSocket socket)
		{
			try
			{
				lock (this.m_Mutex)
				{
					if (this.IsInWait(userID))
					{
						return false;
					}
					if (this.GetWaitingCount(userType) >= this.GetConfig(userType, LoginWaitLogic.ConfigType.MaxQueueNum))
					{
						return false;
					}
					this.m_UserList[(int)userType].Add(new LoginWaitLogic.UserInfo
					{
						userID = userID,
						zoneID = zoneID,
						socket = socket,
						startTick = TimeUtil.NOW(),
						updateTick = 0L
					});
					this.m_User2SocketDict.Add(userID, socket);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::AddToWait userID={0}", userID));
				return false;
			}
			return true;
		}

		
		public void RemoveWait(string userID)
		{
			try
			{
				lock (this.m_Mutex)
				{
					if (this.IsInWait(userID))
					{
						foreach (List<LoginWaitLogic.UserInfo> list in this.m_UserList)
						{
							list.RemoveAll((LoginWaitLogic.UserInfo x) => x.userID == userID);
						}
						this.m_User2SocketDict.Remove(userID);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::RemoveWait userID={0}", userID));
			}
		}

		
		public LoginWaitLogic.UserInfo TopWaiting(LoginWaitLogic.UserType userType)
		{
			LoginWaitLogic.UserInfo userInfo = null;
			try
			{
				lock (this.m_Mutex)
				{
					if (this.GetWaitingCount(userType) <= 0)
					{
						return null;
					}
					userInfo = this.m_UserList[(int)userType][0];
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::TopWaiting", new object[0]));
			}
			return userInfo;
		}

		
		public LoginWaitLogic.UserInfo PopTopWaiting(LoginWaitLogic.UserType userType)
		{
			LoginWaitLogic.UserInfo userInfo = null;
			try
			{
				lock (this.m_Mutex)
				{
					if (this.GetWaitingCount(userType) <= 0)
					{
						return null;
					}
					userInfo = this.m_UserList[(int)userType][0];
					this.m_UserList[(int)userType].RemoveAt(0);
					this.m_User2SocketDict.Remove(userInfo.userID);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::PopTopWaiting", new object[0]));
			}
			return userInfo;
		}

		
		public void OutWaitInfo(LoginWaitLogic.UserType userType, int index)
		{
			try
			{
				lock (this.m_Mutex)
				{
					if (index < 0 || index >= this.GetWaitingCount(userType))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("OutWaitInfo Index Was Outside ", new object[0]), null, true);
					}
					else
					{
						LoginWaitLogic.UserInfo userInfo = this.m_UserList[(int)userType][index];
						LogManager.WriteLog(LogTypes.Error, string.Format("OutWaitInfo:userID={0} zoneID={1} startTick={2} updateTick={3} firstTick={4} overTick={5}", new object[]
						{
							userInfo.userID,
							userInfo.zoneID,
							userInfo.startTick,
							userInfo.updateTick,
							userInfo.firstTick,
							userInfo.overTick
						}), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::PopTopWaiting", new object[0]));
			}
		}

		
		public int GetAllowCount()
		{
			int count;
			lock (this.m_AllowUserDict)
			{
				count = this.m_AllowUserDict.Count;
			}
			return count;
		}

		
		public bool AddToAllow(string userID, int mSeconds)
		{
			try
			{
				if (string.IsNullOrEmpty(userID))
				{
					return false;
				}
				lock (this.m_AllowUserDict)
				{
					if (this.IsInAllowDict(userID))
					{
						this.m_AllowUserDict[userID] = TimeUtil.NOW() + (long)mSeconds;
						return true;
					}
					if (this.GetAllowCount() < this.GetConfig(LoginWaitLogic.UserType.Normal, LoginWaitLogic.ConfigType.MaxServerNum))
					{
						this.m_AllowUserDict[userID] = TimeUtil.NOW() + (long)mSeconds;
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::AddToAllow userID={0}", userID));
				return false;
			}
			return false;
		}

		
		public void RemoveAllow(string userID)
		{
			try
			{
				if (!string.IsNullOrEmpty(userID))
				{
					lock (this.m_AllowUserDict)
					{
						this.m_AllowUserDict.Remove(userID);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::RemoveAllow userID={0}", userID));
			}
		}

		
		public bool IsInAllowDict(string userID)
		{
			bool result;
			try
			{
				if (string.IsNullOrEmpty(userID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("LoginWaitLogic::IsInAllowDict userID={0}", (userID == null) ? "null " : userID), null, true);
					result = false;
				}
				else
				{
					lock (this.m_AllowUserDict)
					{
						result = this.m_AllowUserDict.ContainsKey(userID);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::IsInAllowDict userID={0}", userID));
				result = false;
			}
			return result;
		}

		
		public void NotifyWaitingInfo(LoginWaitLogic.UserInfo userInfo, int count, long seconds)
		{
			try
			{
				if (null != userInfo)
				{
					if (userInfo.socket != null && userInfo.socket.Connected)
					{
						string strcmd = string.Format("{0}:{1}", count, seconds);
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 971);
						Global._TCPManager.MySocketListener.SendData(userInfo.socket, tcpOutPacket, true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::NotifyWaitingInfo userID={0} zoneID={1}", userInfo.userID, userInfo.zoneID));
			}
		}

		
		public bool NotifyUserEnter(LoginWaitLogic.UserInfo userInfo)
		{
			try
			{
				if (null == userInfo)
				{
					return true;
				}
				if (userInfo.socket == null || !userInfo.socket.Connected)
				{
					return true;
				}
				this.AddToAllow(userInfo.userID, this.GetConfig(LoginWaitLogic.UserType.Normal, LoginWaitLogic.ConfigType.AllowMSeconds));
				if (!userInfo.socket.IsKuaFuLogin)
				{
					ChangeNameInfo info = SingletonTemplate<NameManager>.Instance().GetChangeNameInfo(userInfo.userID, userInfo.zoneID, userInfo.socket.ServerId);
					if (info != null)
					{
						Global._TCPManager.MySocketListener.SendData(userInfo.socket, DataHelper.ObjectToTCPOutPacket<ChangeNameInfo>(info, Global._TCPManager.TcpOutPacketPool, 14002), true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::NotifyUserEnter userID={0} zoneID={1}", userInfo.userID, userInfo.zoneID));
				return false;
			}
			string strData = "";
			try
			{
				string strcmd = string.Format("{0}:{1}", userInfo.userID, userInfo.zoneID);
				byte[] bytesData = Global.SendAndRecvData<string>(101, strcmd, userInfo.socket.ServerId, 0);
				int length = BitConverter.ToInt32(bytesData, 0);
				strData = new UTF8Encoding().GetString(bytesData, 6, length - 2);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::NotifyUserEnter 向db请求角色列表 faild！ userID={0} zoneID={1}", userInfo.userID, userInfo.zoneID));
				strData = "-1:";
			}
			try
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strData, 101);
				Global._TCPManager.MySocketListener.SendData(userInfo.socket, tcpOutPacket, true);
				this.m_LastEnterSecs = (TimeUtil.NOW() - userInfo.startTick) / 1000L;
				this.m_LastEnterFromFirstSecs = (TimeUtil.NOW() - userInfo.firstTick) / 1000L;
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::NotifyUserEnter 发送角色列表Faild userID={0} zoneID={1}", userInfo.userID, userInfo.zoneID));
				return false;
			}
			return true;
		}

		
		public LoginWaitLogic.UserType GetUserType(string userID)
		{
			LoginWaitLogic.UserType userType = LoginWaitLogic.UserType.Normal;
			try
			{
				if (VIPEumValue.VIP_MIN_NEED_REALMONEY <= 0)
				{
					int moneyToYuanBao = GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
					if (moneyToYuanBao > 0)
					{
						VIPEumValue.VIP_MIN_NEED_REALMONEY = VIPEumValue.VIP_MIN_NEED_EXP / moneyToYuanBao;
					}
				}
				if (GameManager.ClientMgr.QueryTotaoChongZhiMoney(userID, -1, -1) >= VIPEumValue.VIP_MIN_NEED_REALMONEY)
				{
					userType = LoginWaitLogic.UserType.Vip;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "LoginWaitLogic::GetUserType Exception!!!");
				return userType;
			}
			return userType;
		}

		
		public int GetUserCount()
		{
			return GameManager.ClientMgr.GetClientCount() + this.GetAllowCount();
		}

		
		public void UpdateWaitingList()
		{
			long currTick = TimeUtil.NOW();
			try
			{
				lock (this.m_Mutex)
				{
					foreach (List<LoginWaitLogic.UserInfo> list in this.m_UserList)
					{
						list.RemoveAll((LoginWaitLogic.UserInfo x) => x.socket == null || !x.socket.Connected);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::ProcessWaitingList 1", new object[0]));
			}
			try
			{
				lock (this.m_AllowUserDict)
				{
					List<string> removeList = new List<string>();
					foreach (KeyValuePair<string, long> userInfo in this.m_AllowUserDict)
					{
						if (currTick > userInfo.Value)
						{
							if (null == removeList)
							{
								removeList = new List<string>();
							}
							removeList.Add(userInfo.Key);
						}
					}
					if (removeList != null && removeList.Count > 0)
					{
						foreach (string userID in removeList)
						{
							this.m_AllowUserDict.Remove(userID);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::ProcessWaitingList 2", new object[0]));
			}
		}

		
		public void Tick()
		{
			long currTick = TimeUtil.NOW();
			if (currTick - this.m_UpdateAllowTick >= 1000L)
			{
				this.m_UpdateAllowTick = currTick;
				this.UpdateWaitingList();
				this.ProcessWaitingList(LoginWaitLogic.UserType.Vip);
				this.ProcessWaitingList(LoginWaitLogic.UserType.Normal);
			}
		}

		
		public void ProcessWaitingList(LoginWaitLogic.UserType userType)
		{
			try
			{
				long currTick = TimeUtil.NOW();
				if (this.GetWaitingCount(userType) > 0)
				{
					int currClientCount = this.GetUserCount();
					lock (this.m_Mutex)
					{
						int i = 0;
						long lastOverTick = 0L;
						long firstWaitSeconds = 0L;
						foreach (LoginWaitLogic.UserInfo userInfo in this.m_UserList[(int)userType])
						{
							i++;
							if (1 == i && 0L == userInfo.firstTick)
							{
								userInfo.firstTick = TimeUtil.NOW();
							}
							long leftSeconds;
							if (currClientCount + i <= this.GetConfig(userType, LoginWaitLogic.ConfigType.MaxServerNum))
							{
								if (0L == userInfo.overTick)
								{
									userInfo.overTick = ((0L == lastOverTick) ? currTick : lastOverTick) + (long)this.GetConfig(userType, LoginWaitLogic.ConfigType.WaitUpdateInt);
								}
								leftSeconds = Global.GMax(1L, (userInfo.overTick - currTick) / 1000L);
								leftSeconds = Global.GMin(leftSeconds, (long)(this.GetConfig(userType, LoginWaitLogic.ConfigType.WaitUpdateInt) / 1000 * i));
								lastOverTick = userInfo.overTick;
							}
							else if (1 == i)
							{
								leftSeconds = Global.GMax(this.m_LastEnterFromFirstSecs, (TimeUtil.NOW() - userInfo.firstTick) / 1000L);
								leftSeconds = Global.GMax(1L, leftSeconds);
								firstWaitSeconds = leftSeconds;
							}
							else
							{
								leftSeconds = (long)i * firstWaitSeconds;
							}
							if (currTick - userInfo.updateTick <= (long)this.m_UserUpdateInt)
							{
								lastOverTick = userInfo.overTick;
							}
							else
							{
								userInfo.updateTick = currTick;
								this.NotifyWaitingInfo(userInfo, i, leftSeconds);
							}
						}
					}
					if (currClientCount < this.GetConfig(userType, LoginWaitLogic.ConfigType.NeedWaitNum))
					{
						for (int i = 0; i < 5; i++)
						{
							LoginWaitLogic.UserInfo userInfo = this.PopTopWaiting(userType);
							if (null != userInfo)
							{
								this.NotifyUserEnter(userInfo);
							}
						}
					}
					else if (currClientCount < this.GetConfig(userType, LoginWaitLogic.ConfigType.MaxServerNum))
					{
						LoginWaitLogic.UserInfo userInfo = this.TopWaiting(userType);
						if (null != userInfo)
						{
							if (userInfo.overTick > 0L)
							{
								if (currTick >= userInfo.overTick)
								{
									userInfo = this.PopTopWaiting(userType);
									this.NotifyUserEnter(userInfo);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::Tick", new object[0]));
			}
		}

		
		private int[][] m_IntConfig = new int[2][];

		
		private int m_UserUpdateInt = 5000;

		
		private List<LoginWaitLogic.UserInfo>[] m_UserList = new List<LoginWaitLogic.UserInfo>[2];

		
		private Dictionary<string, TMSKSocket> m_User2SocketDict = new Dictionary<string, TMSKSocket>();

		
		private object m_Mutex = new object();

		
		private long m_UpdateTick = 0L;

		
		private long m_UpdateAllowTick = 0L;

		
		private long m_LastEnterSecs = 30L;

		
		private long m_LastEnterFromFirstSecs = 30L;

		
		private Dictionary<string, long> m_AllowUserDict = new Dictionary<string, long>();

		
		public enum ConfigType
		{
			
			NeedWaitNum,
			
			MaxServerNum,
			
			MaxQueueNum,
			
			WaitUpdateInt,
			
			AllowMSeconds,
			
			LogouAllowMSeconds
		}

		
		public enum UserType
		{
			
			Normal,
			
			Vip,
			
			Max_Type
		}

		
		public class UserInfo
		{
			
			public TMSKSocket socket = null;

			
			public string userID = "";

			
			public int zoneID = 0;

			
			public long startTick = 0L;

			
			public long updateTick = 0L;

			
			public long firstTick = 0L;

			
			public long overTick = 0L;
		}
	}
}
