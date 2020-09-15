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
	// Token: 0x020008C2 RID: 2242
	public class TCPManager
	{
		// Token: 0x06003FC6 RID: 16326 RVA: 0x003B8CDC File Offset: 0x003B6EDC
		private TCPManager()
		{
		}

		// Token: 0x06003FC7 RID: 16327 RVA: 0x003B8D44 File Offset: 0x003B6F44
		public static TCPManager getInstance()
		{
			return TCPManager.instance;
		}

		// Token: 0x06003FC8 RID: 16328 RVA: 0x003B8D5C File Offset: 0x003B6F5C
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

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x06003FC9 RID: 16329 RVA: 0x003B8E74 File Offset: 0x003B7074
		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06003FCA RID: 16330 RVA: 0x003B8E8C File Offset: 0x003B708C
		public TCPInPacketPool TcpInPacketPool
		{
			get
			{
				return this.tcpInPacketPool;
			}
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06003FCB RID: 16331 RVA: 0x003B8EA4 File Offset: 0x003B70A4
		public TCPOutPacketPool TcpOutPacketPool
		{
			get
			{
				return this.tcpOutPacketPool;
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06003FCC RID: 16332 RVA: 0x003B8EBC File Offset: 0x003B70BC
		// (set) Token: 0x06003FCD RID: 16333 RVA: 0x003B8ED3 File Offset: 0x003B70D3
		public Program RootWindow { get; set; }

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06003FCE RID: 16334 RVA: 0x003B8EDC File Offset: 0x003B70DC
		public TCPClientPool tcpClientPool
		{
			get
			{
				return this._tcpClientPool;
			}
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06003FCF RID: 16335 RVA: 0x003B8EF4 File Offset: 0x003B70F4
		public TCPClientPool tcpLogClientPool
		{
			get
			{
				return this._tcpLogClientPool;
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06003FD0 RID: 16336 RVA: 0x003B8F0C File Offset: 0x003B710C
		public TCPRandKey tcpRandKey
		{
			get
			{
				return this._tcpRandKey;
			}
		}

		// Token: 0x06003FD1 RID: 16337 RVA: 0x003B8F24 File Offset: 0x003B7124
		public void Start(string ip, int port)
		{
			TCPManager.ServerPort = port;
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		// Token: 0x06003FD2 RID: 16338 RVA: 0x003B8F47 File Offset: 0x003B7147
		public void Stop()
		{
			this.socketListener.Stop();
			this.taskExecutor.stop();
		}

		// Token: 0x06003FD3 RID: 16339 RVA: 0x003B8F64 File Offset: 0x003B7164
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

		// Token: 0x06003FD4 RID: 16340 RVA: 0x003B90B4 File Offset: 0x003B72B4
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

		// Token: 0x06003FD5 RID: 16341 RVA: 0x003B9178 File Offset: 0x003B7378
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

		// Token: 0x06003FD6 RID: 16342 RVA: 0x003B91F8 File Offset: 0x003B73F8
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

		// Token: 0x06003FD7 RID: 16343 RVA: 0x003B93C4 File Offset: 0x003B75C4
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

		// Token: 0x06003FD8 RID: 16344 RVA: 0x003B9408 File Offset: 0x003B7608
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

		// Token: 0x06003FD9 RID: 16345 RVA: 0x003B944C File Offset: 0x003B764C
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

		// Token: 0x06003FDA RID: 16346 RVA: 0x003B9490 File Offset: 0x003B7690
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

		// Token: 0x06003FDB RID: 16347 RVA: 0x003B9550 File Offset: 0x003B7750
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

		// Token: 0x06003FDC RID: 16348 RVA: 0x003B9988 File Offset: 0x003B7B88
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

		// Token: 0x06003FDD RID: 16349 RVA: 0x003B9A14 File Offset: 0x003B7C14
		public Dictionary<TMSKSocket, TCPSession> GetTCPSessions()
		{
			throw new NotSupportedException("因为未被用到,优化tcpSessions的锁后,这个函数未兼容实现");
		}

		// Token: 0x06003FDE RID: 16350 RVA: 0x003B9A24 File Offset: 0x003B7C24
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

		// Token: 0x06003FDF RID: 16351 RVA: 0x003B9A7C File Offset: 0x003B7C7C
		private void SocketClosed(object sender, TMSKSocket s)
		{
			SocketListener sl = sender as SocketListener;
			this.ExternalClearSocket(s);
		}

		// Token: 0x06003FE0 RID: 16352 RVA: 0x003B9A9C File Offset: 0x003B7C9C
		public void ExternalClearSocket(TMSKSocket s)
		{
			this.ForceCloseSocket(s);
			if (null != s.session)
			{
				s.session.release();
			}
			s.MyDispose();
		}

		// Token: 0x06003FE1 RID: 16353 RVA: 0x003B9AE0 File Offset: 0x003B7CE0
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

		// Token: 0x06003FE2 RID: 16354 RVA: 0x003B9BD4 File Offset: 0x003B7DD4
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

		// Token: 0x06003FE3 RID: 16355 RVA: 0x003B9C28 File Offset: 0x003B7E28
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

		// Token: 0x04004EEA RID: 20202
		public const bool UseWorkerPool = false;

		// Token: 0x04004EEB RID: 20203
		private static TCPManager instance = new TCPManager();

		// Token: 0x04004EEC RID: 20204
		public static int ServerPort = 0;

		// Token: 0x04004EED RID: 20205
		public ScheduleExecutor taskExecutor = null;

		// Token: 0x04004EEE RID: 20206
		public int MaxConnectedClientLimit = 0;

		// Token: 0x04004EEF RID: 20207
		private SocketListener socketListener = null;

		// Token: 0x04004EF0 RID: 20208
		private TCPInPacketPool tcpInPacketPool = null;

		// Token: 0x04004EF1 RID: 20209
		private TCPOutPacketPool tcpOutPacketPool = null;

		// Token: 0x04004EF2 RID: 20210
		private TCPClientPool _tcpClientPool = null;

		// Token: 0x04004EF3 RID: 20211
		private TCPClientPool _tcpLogClientPool = null;

		// Token: 0x04004EF4 RID: 20212
		private TCPRandKey _tcpRandKey = new TCPRandKey(10000);

		// Token: 0x04004EF5 RID: 20213
		private Dictionary<TMSKSocket, TCPInPacket> dictInPackets = null;

		// Token: 0x04004EF6 RID: 20214
		private Dictionary<TMSKSocket, TCPSession> tcpSessions = null;
	}
}
