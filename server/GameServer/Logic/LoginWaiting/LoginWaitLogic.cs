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
	// Token: 0x02000351 RID: 849
	public class LoginWaitLogic
	{
		// Token: 0x06000E8A RID: 3722 RVA: 0x000E4C30 File Offset: 0x000E2E30
		public void LoadConfig()
		{
			string strCfg = GameManager.PlatConfigMgr.GetGameConfigItemStr("userwaitconfig", "700,1000,400,30000,180000,20000");
			this.m_IntConfig[0] = Global.String2IntArray(strCfg, ',');
			strCfg = GameManager.PlatConfigMgr.GetGameConfigItemStr("vipwaitconfig", "900,1000,400,30000,180000,20000");
			this.m_IntConfig[1] = Global.String2IntArray(strCfg, ',');
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x000E4C88 File Offset: 0x000E2E88
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

		// Token: 0x06000E8C RID: 3724 RVA: 0x000E4CD8 File Offset: 0x000E2ED8
		public LoginWaitLogic()
		{
			for (LoginWaitLogic.UserType i = LoginWaitLogic.UserType.Normal; i < LoginWaitLogic.UserType.Max_Type; i++)
			{
				this.m_UserList[(int)i] = new List<LoginWaitLogic.UserInfo>();
			}
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x000E4D74 File Offset: 0x000E2F74
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

		// Token: 0x06000E8E RID: 3726 RVA: 0x000E4DF4 File Offset: 0x000E2FF4
		public int GetWaitingCount(LoginWaitLogic.UserType userType)
		{
			int count;
			lock (this.m_Mutex)
			{
				count = this.m_UserList[(int)userType].Count;
			}
			return count;
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x000E4E48 File Offset: 0x000E3048
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

		// Token: 0x06000E90 RID: 3728 RVA: 0x000E4ED4 File Offset: 0x000E30D4
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

		// Token: 0x06000E91 RID: 3729 RVA: 0x000E4FFC File Offset: 0x000E31FC
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

		// Token: 0x06000E92 RID: 3730 RVA: 0x000E50F8 File Offset: 0x000E32F8
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

		// Token: 0x06000E93 RID: 3731 RVA: 0x000E519C File Offset: 0x000E339C
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

		// Token: 0x06000E94 RID: 3732 RVA: 0x000E5260 File Offset: 0x000E3460
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

		// Token: 0x06000E95 RID: 3733 RVA: 0x000E5390 File Offset: 0x000E3590
		public int GetAllowCount()
		{
			int count;
			lock (this.m_AllowUserDict)
			{
				count = this.m_AllowUserDict.Count;
			}
			return count;
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x000E53E4 File Offset: 0x000E35E4
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

		// Token: 0x06000E97 RID: 3735 RVA: 0x000E54CC File Offset: 0x000E36CC
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

		// Token: 0x06000E98 RID: 3736 RVA: 0x000E5558 File Offset: 0x000E3758
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

		// Token: 0x06000E99 RID: 3737 RVA: 0x000E5604 File Offset: 0x000E3804
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

		// Token: 0x06000E9A RID: 3738 RVA: 0x000E56C4 File Offset: 0x000E38C4
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

		// Token: 0x06000E9B RID: 3739 RVA: 0x000E5908 File Offset: 0x000E3B08
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

		// Token: 0x06000E9C RID: 3740 RVA: 0x000E59A0 File Offset: 0x000E3BA0
		public int GetUserCount()
		{
			return GameManager.ClientMgr.GetClientCount() + this.GetAllowCount();
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x000E59F0 File Offset: 0x000E3BF0
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

		// Token: 0x06000E9E RID: 3742 RVA: 0x000E5C5C File Offset: 0x000E3E5C
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

		// Token: 0x06000E9F RID: 3743 RVA: 0x000E5CA8 File Offset: 0x000E3EA8
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

		// Token: 0x0400166B RID: 5739
		private int[][] m_IntConfig = new int[2][];

		// Token: 0x0400166C RID: 5740
		private int m_UserUpdateInt = 5000;

		// Token: 0x0400166D RID: 5741
		private List<LoginWaitLogic.UserInfo>[] m_UserList = new List<LoginWaitLogic.UserInfo>[2];

		// Token: 0x0400166E RID: 5742
		private Dictionary<string, TMSKSocket> m_User2SocketDict = new Dictionary<string, TMSKSocket>();

		// Token: 0x0400166F RID: 5743
		private object m_Mutex = new object();

		// Token: 0x04001670 RID: 5744
		private long m_UpdateTick = 0L;

		// Token: 0x04001671 RID: 5745
		private long m_UpdateAllowTick = 0L;

		// Token: 0x04001672 RID: 5746
		private long m_LastEnterSecs = 30L;

		// Token: 0x04001673 RID: 5747
		private long m_LastEnterFromFirstSecs = 30L;

		// Token: 0x04001674 RID: 5748
		private Dictionary<string, long> m_AllowUserDict = new Dictionary<string, long>();

		// Token: 0x02000352 RID: 850
		public enum ConfigType
		{
			// Token: 0x04001677 RID: 5751
			NeedWaitNum,
			// Token: 0x04001678 RID: 5752
			MaxServerNum,
			// Token: 0x04001679 RID: 5753
			MaxQueueNum,
			// Token: 0x0400167A RID: 5754
			WaitUpdateInt,
			// Token: 0x0400167B RID: 5755
			AllowMSeconds,
			// Token: 0x0400167C RID: 5756
			LogouAllowMSeconds
		}

		// Token: 0x02000353 RID: 851
		public enum UserType
		{
			// Token: 0x0400167E RID: 5758
			Normal,
			// Token: 0x0400167F RID: 5759
			Vip,
			// Token: 0x04001680 RID: 5760
			Max_Type
		}

		// Token: 0x02000354 RID: 852
		public class UserInfo
		{
			// Token: 0x04001681 RID: 5761
			public TMSKSocket socket = null;

			// Token: 0x04001682 RID: 5762
			public string userID = "";

			// Token: 0x04001683 RID: 5763
			public int zoneID = 0;

			// Token: 0x04001684 RID: 5764
			public long startTick = 0L;

			// Token: 0x04001685 RID: 5765
			public long updateTick = 0L;

			// Token: 0x04001686 RID: 5766
			public long firstTick = 0L;

			// Token: 0x04001687 RID: 5767
			public long overTick = 0L;
		}
	}
}
