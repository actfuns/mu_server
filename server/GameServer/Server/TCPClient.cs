using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GameServer.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Server
{
	// Token: 0x020008B9 RID: 2233
	public class TCPClient
	{
		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06003DDF RID: 15839 RVA: 0x0034C8D4 File Offset: 0x0034AAD4
		// (set) Token: 0x06003DE0 RID: 15840 RVA: 0x0034C8EB File Offset: 0x0034AAEB
		public IConnectInfoContainer RootWindow { get; set; }

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06003DE1 RID: 15841 RVA: 0x0034C8F4 File Offset: 0x0034AAF4
		// (set) Token: 0x06003DE2 RID: 15842 RVA: 0x0034C90B File Offset: 0x0034AB0B
		public int ListIndex { get; set; }

		// Token: 0x06003DE3 RID: 15843 RVA: 0x0034C914 File Offset: 0x0034AB14
		public bool ValidateIpPort(string ip, int port)
		{
			return !(ip != this.ServerIP) && port == this.ServerPort;
		}

		// Token: 0x06003DE4 RID: 15844 RVA: 0x0034C94C File Offset: 0x0034AB4C
		public void Connect(string ip, int port, string serverName)
		{
			this.ServerName = serverName;
			lock (this.MutexSocket)
			{
				if (null == this._Socket)
				{
					this.ServerIP = ip;
					this.ServerPort = port;
					IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
					this._Socket = new TMSKSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					this._Socket.SendTimeout = 30000;
					this._Socket.ReceiveTimeout = 30000;
					this._Socket.NoDelay = this.NoDelay;
					try
					{
						this._Socket.Connect(remoteEndPoint);
					}
					catch (Exception)
					{
						this.RootWindow.AddDBConnectInfo(this.ListIndex, string.Format("{0}, 与{1}: {2}:{3}建立连接失败", new object[]
						{
							this.ListIndex,
							this.ServerName,
							ip,
							port
						}));
						LogManager.WriteLog(LogTypes.Error, string.Format("{0}, 与{1}: {2}:{3}建立连接失败", new object[]
						{
							this.ListIndex,
							this.ServerName,
							ip,
							port
						}), null, true);
						this._Socket = null;
						throw;
					}
					Global.SendGameServerHeart(this);
					this.RootWindow.AddDBConnectInfo(this.ListIndex, string.Format("{0}, 与{1}: {2}建立连接成功", this.ListIndex, this.ServerName, remoteEndPoint));
				}
			}
		}

		// Token: 0x06003DE5 RID: 15845 RVA: 0x0034CB14 File Offset: 0x0034AD14
		public void Disconnect()
		{
			lock (this.MutexSocket)
			{
				if (null != this._Socket)
				{
					this.RootWindow.AddDBConnectInfo(this.ListIndex, string.Format("{0}, 与{1}: {2}断开连接", this.ListIndex, this.ServerName, Global.GetSocketRemoteEndPoint(this._Socket, false)));
					try
					{
						this._Socket.Shutdown(SocketShutdown.Receive);
						this._Socket.Close(30);
					}
					catch (Exception)
					{
					}
					this._Socket = null;
				}
			}
		}

		// Token: 0x06003DE6 RID: 15846 RVA: 0x0034CBE0 File Offset: 0x0034ADE0
		public bool IsConnected()
		{
			bool ret = false;
			lock (this.MutexSocket)
			{
				ret = (null != this._Socket);
			}
			return ret;
		}

		// Token: 0x06003DE7 RID: 15847 RVA: 0x0034CC3C File Offset: 0x0034AE3C
		public byte[] SendData(TCPOutPacket tcpOutPacket)
		{
			byte[] result;
			lock (this.MutexSocket)
			{
				if (null == this._Socket)
				{
					result = null;
				}
				else
				{
					try
					{
						if (LogManager.LogTypeToWrite >= LogTypes.Info && LogManager.LogTypeToWrite <= LogTypes.Warning)
						{
						}
						this._Socket.Send(tcpOutPacket.GetPacketBytes(), tcpOutPacket.PacketDataSize, SocketFlags.None);
						byte[] data = new byte[4];
						int i = this._Socket.Receive(data, 0, 4, SocketFlags.None);
						if (i != 4)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("{0}, 与{1}: {2}通讯失败, 获取数据包长度失败", this.ListIndex, this.ServerName, Global.GetSocketRemoteEndPoint(this._Socket, false)), null, true);
							return null;
						}
						int length = BitConverter.ToInt32(data, 0);
						byte[] dataTmp = new byte[length + 4];
						DataHelper.CopyBytes(dataTmp, 0, data, 0, 4);
						data = dataTmp;
						int totalReceived;
						for (totalReceived = 0; totalReceived < length; totalReceived += i)
						{
							i = this._Socket.Receive(data, 4 + totalReceived, length - totalReceived, SocketFlags.None);
						}
						if (totalReceived != length)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("{0}, 与{1}: {2}通讯失败, 返回的数据包数据长度:{3}和接收到的数据长度:{4}不匹配", new object[]
							{
								this.ListIndex,
								this.ServerName,
								Global.GetSocketRemoteEndPoint(this._Socket, false),
								length,
								totalReceived
							}), null, true);
							return null;
						}
						this.LastCmdID = (int)tcpOutPacket.PacketCmdID;
						return data;
					}
					catch (Exception ex)
					{
						this.Disconnect();
						try
						{
							string cmdData = new UTF8Encoding().GetString(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
							LogManager.WriteLog(LogTypes.Error, string.Format("{0}, 和{1}:{2}通讯失败, 发送命令{3}, 数据 {4}, 长度{5}, 异常信息:{6}", new object[]
							{
								this.ListIndex,
								this.ServerName,
								Global.GetSocketRemoteEndPoint(this._Socket, false),
								(TCPGameServerCmds)tcpOutPacket.PacketCmdID,
								cmdData,
								tcpOutPacket.PacketDataSize - 6,
								ex.Message
							}), null, true);
							DataHelper.WriteExceptionLogEx(ex, string.Format("和{0}通讯发生异常", this.ServerName));
						}
						catch (Exception)
						{
						}
						this.RootWindow.AddDBConnectInfo(this.ListIndex, string.Format("{0}, 与{1}: {2}通讯失败", this.ListIndex, this.ServerName, Global.GetSocketRemoteEndPoint(this._Socket, false)));
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x040047EA RID: 18410
		private object MutexSocket = new object();

		// Token: 0x040047EB RID: 18411
		private TMSKSocket _Socket = null;

		// Token: 0x040047EC RID: 18412
		private string ServerIP = "";

		// Token: 0x040047ED RID: 18413
		private int ServerPort = 0;

		// Token: 0x040047EE RID: 18414
		public string ServerName = "";

		// Token: 0x040047EF RID: 18415
		public bool NoDelay = false;

		// Token: 0x040047F0 RID: 18416
		public int LastCmdID;
	}
}
