using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Server
{
	
	public class TCPManager
	{
		
		private TCPManager()
		{
		}

		
		public static TCPManager getInstance()
		{
			return TCPManager.instance;
		}

		
		public void initialize(int capacity)
		{
			this.MaxConnectedClientLimit = capacity;
			this.socketListener = new SocketListener(capacity, 6144);
			this.socketListener.SocketClosed += this.SocketClosed;
			this.socketListener.SocketConnected += this.SocketConnected;
			this.socketListener.SocketReceived += this.SocketReceived;
			this.socketListener.SocketSended += this.SocketSended;
			this._tcpClientPool = TCPClientPool.getInstance();
			this._tcpClientPool.initialize(100);
			this._tcpLogClientPool = TCPClientPool.getLogInstance();
			this._tcpLogClientPool.initialize(100);
			this.tcpInPacketPool = new TCPInPacketPool(capacity);
			TCPOutPacketPool.getInstance().initialize(capacity);
			this.tcpOutPacketPool = TCPOutPacketPool.getInstance();
			this.dictInPackets = new Dictionary<TMSKSocket, TCPInPacket>(capacity);
			this.tcpSessions = new Dictionary<TMSKSocket, TCPSession>();
			TCPCmdDispatcher.getInstance().initialize();
			this.taskExecutor = new ScheduleExecutor(0);
			this.taskExecutor.start();
		}

		
		
		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		
		
		public TCPInPacketPool TcpInPacketPool
		{
			get
			{
				return this.tcpInPacketPool;
			}
		}

		
		
		public TCPOutPacketPool TcpOutPacketPool
		{
			get
			{
				return this.tcpOutPacketPool;
			}
		}

		
		
		
		public Program RootWindow { get; set; }

		
		
		public TCPClientPool tcpClientPool
		{
			get
			{
				return this._tcpClientPool;
			}
		}

		
		
		public TCPClientPool tcpLogClientPool
		{
			get
			{
				return this._tcpLogClientPool;
			}
		}

		
		
		public TCPRandKey tcpRandKey
		{
			get
			{
				return this._tcpRandKey;
			}
		}

		
		public void Start(string ip, int port)
		{
			TCPManager.ServerPort = port;
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		
		public void Stop()
		{
			this.socketListener.Stop();
			this.taskExecutor.stop();
		}

		
		public void ForceCloseSocket(TMSKSocket s)
		{
			DelayForceClosingMgr.RemoveDelaySocket(s);
			lock (this.dictInPackets)
			{
				if (this.dictInPackets.ContainsKey(s))
				{
					TCPInPacket tcpInPacket = this.dictInPackets[s];
					this.tcpInPacketPool.Push(tcpInPacket);
					this.dictInPackets.Remove(s);
				}
			}
			bool bIsExistClient = false;
			GameClient gameClient = GameManager.ClientMgr.FindClient(s);
			if (null != gameClient)
			{
				GameManager.ClientMgr.Logout(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
				bIsExistClient = true;
			}
			string userID = GameManager.OnlineUserSession.FindUserID(s);
			GameManager.OnlineUserSession.RemoveSession(s);
			GameManager.OnlineUserSession.RemoveUserName(s);
			GameManager.OnlineUserSession.RemoveUserAdult(s);
			if (!string.IsNullOrEmpty(userID))
			{
				Global.RegisterUserIDToDBServer(userID, 0, s.ServerId, ref s.session.LastLogoutServerTicks);
			}
			GameManager.loginWaitLogic.RemoveWait(userID);
			if (!bIsExistClient)
			{
				GameManager.loginWaitLogic.RemoveAllow(userID);
			}
			Global._SendBufferManager.Remove(s);
		}

		
		public string GetAllCacheCmdPacketInfo()
		{
			int nTotal = 0;
			int nMaxNum = 0;
			lock (this.dictInPackets)
			{
				for (int i = 0; i < this.dictInPackets.Values.Count; i++)
				{
					TCPInPacket tcpInPacket = this.dictInPackets.Values.ElementAt(i);
					if (tcpInPacket.GetCacheCmdPacketCount() > nMaxNum)
					{
						nMaxNum = tcpInPacket.GetCacheCmdPacketCount();
					}
					nTotal += tcpInPacket.GetCacheCmdPacketCount();
				}
			}
			return string.Format("总共缓存命令包{0}个,单个连接最大缓存{1}个", nTotal, nMaxNum);
		}

		
		private TCPInPacket GetNextTcpInPacket(int index)
		{
			lock (this.dictInPackets)
			{
				if (this.dictInPackets.Values.Count > index && index >= 0)
				{
					return this.dictInPackets.Values.ElementAt(index);
				}
			}
			return null;
		}

		
		public void ProcessCmdPackets(Queue<CmdPacket> ls)
		{
			int maxCount = this.dictInPackets.Values.Count + 20;
			for (int i = 0; i < maxCount; i++)
			{
				TCPInPacket tcpInPacket = this.GetNextTcpInPacket(i);
				if (null == tcpInPacket)
				{
					break;
				}
				ls.Clear();
				if (tcpInPacket.PopCmdPackets(ls))
				{
					try
					{
						while (ls.Count > 0)
						{
							CmdPacket cmdPacket = ls.Dequeue();
							TCPOutPacket tcpOutPacket = null;
							TCPProcessCmdResults result = TCPCmdHandler.ProcessCmd(this, tcpInPacket.CurrentSocket, this.tcpClientPool, this.tcpRandKey, this.tcpOutPacketPool, cmdPacket.CmdID, cmdPacket.Data, cmdPacket.Data.Length, out tcpOutPacket);
							if (result == TCPProcessCmdResults.RESULT_DATA && null != tcpOutPacket)
							{
								this.socketListener.SendData(tcpInPacket.CurrentSocket, tcpOutPacket, true);
							}
							else
							{
								if (result == TCPProcessCmdResults.RESULT_FAILED)
								{
									if (cmdPacket.CmdID != 22)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false)), null, true);
									}
									this.socketListener.CloseSocket(tcpInPacket.CurrentSocket, "");
									break;
								}
								if (result == TCPProcessCmdResults.RESULT_DATA_CLOSE && null != tcpOutPacket)
								{
									this.socketListener.SendData(tcpInPacket.CurrentSocket, tcpOutPacket, true);
									tcpInPacket.CurrentSocket.DelayClose = 250;
								}
							}
						}
					}
					finally
					{
						tcpInPacket.OnThreadDealingComplete();
					}
				}
			}
		}

		
		public static void RecordCmdDetail(int cmdId, long processTime, long dataSize, TCPProcessCmdResults result)
		{
			PorcessCmdMoniter moniter = null;
			if (!ProcessSessionTask.cmdMoniter.TryGetValue(cmdId, out moniter))
			{
				moniter = new PorcessCmdMoniter(cmdId, processTime);
				moniter = ProcessSessionTask.cmdMoniter.GetOrAdd(cmdId, moniter);
			}
			moniter.onProcessNoWait(processTime, dataSize, result);
		}

		
		public static void RecordCmdDetail2(int cmdId, long processTime, long waitTime)
		{
			PorcessCmdMoniter moniter = null;
			if (!ProcessSessionTask.cmdMoniter.TryGetValue(cmdId, out moniter))
			{
				moniter = new PorcessCmdMoniter(cmdId, processTime);
				moniter = ProcessSessionTask.cmdMoniter.GetOrAdd(cmdId, moniter);
			}
			moniter.onProcess(processTime, waitTime);
		}

		
		public static void RecordCmdOutputDataSize(int cmdId, long dataSize)
		{
			PorcessCmdMoniter moniter = null;
			if (!ProcessSessionTask.cmdMoniter.TryGetValue(cmdId, out moniter))
			{
				moniter = new PorcessCmdMoniter(cmdId, 0L);
				moniter = ProcessSessionTask.cmdMoniter.GetOrAdd(cmdId, moniter);
			}
			moniter.OnOutputData(dataSize);
		}

		
		private byte[] CheckClientDataValid(int packetCmdID, byte[] bytesData, int dataSize, int lastClientCheckTicks, out int clientCheckTicks, out int errorCode)
		{
			errorCode = 0;
			clientCheckTicks = 0;
			byte[] result;
			if (dataSize < 5)
			{
				errorCode = 1;
				result = null;
			}
			else
			{
				int crc32Num_client = (int)bytesData[0];
				clientCheckTicks = BitConverter.ToInt32(bytesData, 1);
				if (clientCheckTicks < lastClientCheckTicks)
				{
					errorCode = 2;
					result = null;
				}
				else
				{
					CRC32 crc32 = new CRC32();
					crc32.update(bytesData, 1, dataSize - 1);
					uint cc = crc32.getValue() % 255U;
					uint cc2 = (uint)(packetCmdID % 255);
					int cc3 = (int)(cc ^ cc2);
					if (cc3 != crc32Num_client)
					{
						errorCode = 3;
						result = null;
					}
					else
					{
						byte[] newByteData = new byte[dataSize - 1 - 4];
						DataHelper.CopyBytes(newByteData, 0, bytesData, 5, dataSize - 1 - 4);
						result = newByteData;
					}
				}
			}
			return result;
		}

		
		private bool TCPCmdPacketEvent(object sender, int doType)
		{
			TCPInPacket tcpInPacket = sender as TCPInPacket;
			if (0 == doType)
			{
				int thisTimeCheckTicks = 0;
				int checkErrorCode = 0;
				byte[] bytesData = this.CheckClientDataValid((int)tcpInPacket.PacketCmdID, tcpInPacket.GetPacketBytes(), tcpInPacket.PacketDataSize, tcpInPacket.LastCheckTicks, out thisTimeCheckTicks, out checkErrorCode);
				if (null == bytesData)
				{
					TMSKSocket _s = tcpInPacket.CurrentSocket;
					string uid = (_s != null) ? GameManager.OnlineUserSession.FindUserID(_s) : "socket is nil";
					LogManager.WriteLog(LogTypes.Error, string.Format("校验客户端发送的指令数据完整性失败: {0},{1}, 错误码:{2}, uid={3}, 关闭连接", new object[]
					{
						(TCPGameServerCmds)tcpInPacket.PacketCmdID,
						Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false),
						checkErrorCode,
						uid
					}), null, true);
					return false;
				}
				tcpInPacket.LastCheckTicks = thisTimeCheckTicks;
				tcpInPacket.CurrentSocket.ClientCmdSecs = thisTimeCheckTicks;
				TCPSession session = null;
				if (null != tcpInPacket.CurrentSocket)
				{
					session = tcpInPacket.CurrentSocket.session;
				}
				if (null == session)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("未与客户端建立会话: {0},{1}, 错误码:{2}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false), checkErrorCode), null, true);
					return false;
				}
				int posCmdNum = 0;
				session.CheckCmdNum((int)tcpInPacket.PacketCmdID, (long)thisTimeCheckTicks, out posCmdNum);
				if (posCmdNum > 0)
				{
					int banSpeedUpMinutes = GameManager.PlatConfigMgr.GetGameConfigItemInt("ban-speed-up-minutes2", 10);
					GameClient client = GameManager.ClientMgr.FindClient(tcpInPacket.CurrentSocket);
					if (null != client)
					{
						if (GameManager.PlatConfigMgr.GetGameConfigItemInt("ban_speed_up_delay", 0) == 0 || client.CheckCheatData.ProcessBooster)
						{
							GameManager.ClientMgr.NotifyImportantMsg(this.MySocketListener, this.tcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(663, new object[0]), new object[]
							{
								banSpeedUpMinutes
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
							BanManager.BanRoleName(Global.FormatRoleName(client, client.ClientData.RoleName), banSpeedUpMinutes, 1);
							LogManager.WriteLog(LogTypes.Error, string.Format("通过POSITION指令判断客户端加速: {0}, 指令个数:{1}, 断开连接", Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false), posCmdNum), null, true);
							return false;
						}
						if (client.CheckCheatData.ProcessBoosterTicks == 0L)
						{
							client.CheckCheatData.ProcessBoosterTicks = TimeUtil.NOW();
						}
					}
				}
				TCPOutPacket tcpOutPacket = null;
				long processBeginTime = TimeUtil.NowEx();
				TCPProcessCmdResults result = TCPCmdHandler.ProcessCmd(this, tcpInPacket.CurrentSocket, this.tcpClientPool, this.tcpRandKey, this.tcpOutPacketPool, (int)tcpInPacket.PacketCmdID, bytesData, tcpInPacket.PacketDataSize - 1 - 4, out tcpOutPacket);
				long processTime = TimeUtil.NowEx() - processBeginTime;
				if (GameManager.StatisticsMode > 0 || processTime > 50L || result == TCPProcessCmdResults.RESULT_FAILED)
				{
					TCPManager.RecordCmdDetail((int)tcpInPacket.PacketCmdID, processTime, (long)tcpInPacket.PacketDataSize, result);
				}
				if (result == TCPProcessCmdResults.RESULT_DATA && null != tcpOutPacket)
				{
					this.socketListener.SendData(tcpInPacket.CurrentSocket, tcpOutPacket, true);
				}
				else
				{
					if (result == TCPProcessCmdResults.RESULT_FAILED)
					{
						if (tcpInPacket.PacketCmdID != 22)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false)), null, true);
						}
						return false;
					}
					if (result == TCPProcessCmdResults.RESULT_DATA_CLOSE && null != tcpOutPacket)
					{
						this.socketListener.SendData(tcpInPacket.CurrentSocket, tcpOutPacket, true);
						tcpInPacket.CurrentSocket.DelayClose = 250;
					}
				}
			}
			else
			{
				if (1 != doType)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析并执行命令时类型未知: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpInPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false)), null, true);
					return false;
				}
				this.DirectSendPolicyFileData(tcpInPacket);
			}
			return true;
		}

		
		public ushort LastPacketCmdID(TMSKSocket s)
		{
			ushort cmd = 0;
			if (s != null && this.dictInPackets != null)
			{
				lock (this.dictInPackets)
				{
					TCPInPacket inPacket = null;
					if (this.dictInPackets.TryGetValue(s, out inPacket))
					{
						cmd = inPacket.LastPacketCmdID;
					}
				}
			}
			return cmd;
		}

		
		public Dictionary<TMSKSocket, TCPSession> GetTCPSessions()
		{
			throw new NotSupportedException("因为未被用到,优化tcpSessions的锁后,这个函数未兼容实现");
		}

		
		private void SocketConnected(object sender, SocketAsyncEventArgs e)
		{
			SocketListener sl = sender as SocketListener;
			TMSKSocket s = (e.UserToken as AsyncUserToken).CurrentSocket;
			if (null == s.session)
			{
				s.session = new TCPSession(s);
			}
			s.SortKey64 = DataHelper.SortKey64;
		}

		
		private void SocketClosed(object sender, TMSKSocket s)
		{
			SocketListener sl = sender as SocketListener;
			this.ExternalClearSocket(s);
		}

		
		public void ExternalClearSocket(TMSKSocket s)
		{
			this.ForceCloseSocket(s);
			if (null != s.session)
			{
				s.session.release();
			}
			s.MyDispose();
		}

		
		private bool SocketReceived(object sender, SocketAsyncEventArgs e)
		{
			SocketListener sl = sender as SocketListener;
			TCPInPacket tcpInPacket = null;
			AsyncUserToken userToken = e.UserToken as AsyncUserToken;
			TMSKSocket s = userToken.CurrentSocket;
			tcpInPacket = s._TcpInPacket;
			if (null == tcpInPacket)
			{
				lock (this.dictInPackets)
				{
					if (!this.dictInPackets.TryGetValue(s, out tcpInPacket))
					{
						tcpInPacket = this.tcpInPacketPool.Pop(s, new TCPCmdPacketEventHandler(this.TCPCmdPacketEvent));
						this.dictInPackets[s] = tcpInPacket;
					}
				}
				s._TcpInPacket = tcpInPacket;
			}
			return tcpInPacket.WriteData(e.Buffer, e.Offset, e.BytesTransferred);
		}

		
		private void SocketSended(object sender, SocketAsyncEventArgs e)
		{
			AsyncUserToken userToken = e.UserToken as AsyncUserToken;
			SendBuffer sendBuffer = userToken._SendBuffer;
			if (null != sendBuffer)
			{
				sendBuffer.Reset2();
			}
			TMSKSocket s = userToken.CurrentSocket;
			Global._SendBufferManager.OnSendBufferOK(s);
		}

		
		private void DirectSendPolicyFileData(TCPInPacket tcpInPacket)
		{
			TMSKSocket s = tcpInPacket.CurrentSocket;
			try
			{
				s.Send(TCPPolicy.PolicyServerFileContent, TCPPolicy.PolicyServerFileContent.Length, SocketFlags.None);
				LogManager.WriteLog(LogTypes.Info, string.Format("向客户端返回策略文件数据: {0}", Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false)), null, true);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Info, string.Format("向客户端返回策略文件时，socket出现异常，对方已经关闭: {0}", Global.GetSocketRemoteEndPoint(tcpInPacket.CurrentSocket, false)), null, true);
			}
		}

		
		public const bool UseWorkerPool = false;

		
		private static TCPManager instance = new TCPManager();

		
		public static int ServerPort = 0;

		
		public ScheduleExecutor taskExecutor = null;

		
		public int MaxConnectedClientLimit = 0;

		
		private SocketListener socketListener = null;

		
		private TCPInPacketPool tcpInPacketPool = null;

		
		private TCPOutPacketPool tcpOutPacketPool = null;

		
		private TCPClientPool _tcpClientPool = null;

		
		private TCPClientPool _tcpLogClientPool = null;

		
		private TCPRandKey _tcpRandKey = new TCPRandKey(10000);

		
		private Dictionary<TMSKSocket, TCPInPacket> dictInPackets = null;

		
		private Dictionary<TMSKSocket, TCPSession> tcpSessions = null;
	}
}
