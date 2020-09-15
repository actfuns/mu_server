using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Logic;
using GameServer.Server;
using Server.TCP;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x02000868 RID: 2152
	public class TCPInPacket : IDisposable
	{
		// Token: 0x06003CA9 RID: 15529 RVA: 0x0034179C File Offset: 0x0033F99C
		public TCPInPacket(int recvBufferSize = 6144)
		{
			this.PacketBytes = new byte[recvBufferSize];
			TCPInPacket.IncInstanceCount();
		}

		// Token: 0x06003CAA RID: 15530 RVA: 0x00341828 File Offset: 0x0033FA28
		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x06003CAB RID: 15531 RVA: 0x00341840 File Offset: 0x0033FA40
		// (set) Token: 0x06003CAC RID: 15532 RVA: 0x00341858 File Offset: 0x0033FA58
		public int LastCheckTicks
		{
			get
			{
				return this._LastCheckTicks;
			}
			set
			{
				this._LastCheckTicks = value;
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06003CAD RID: 15533 RVA: 0x00341864 File Offset: 0x0033FA64
		// (set) Token: 0x06003CAE RID: 15534 RVA: 0x0034187C File Offset: 0x0033FA7C
		public TMSKSocket CurrentSocket
		{
			get
			{
				return this._Socket;
			}
			set
			{
				this._Socket = value;
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06003CAF RID: 15535 RVA: 0x00341888 File Offset: 0x0033FA88
		public ushort PacketCmdID
		{
			get
			{
				return this._PacketCmdID;
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06003CB0 RID: 15536 RVA: 0x003418A8 File Offset: 0x0033FAA8
		// (set) Token: 0x06003CB1 RID: 15537 RVA: 0x003418BF File Offset: 0x0033FABF
		public ushort LastPacketCmdID { get; set; }

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06003CB2 RID: 15538 RVA: 0x003418C8 File Offset: 0x0033FAC8
		public int PacketDataSize
		{
			get
			{
				return this._PacketDataSize;
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06003CB3 RID: 15539 RVA: 0x003418E8 File Offset: 0x0033FAE8
		public Queue<CmdPacket> CmdPacketPool
		{
			get
			{
				return this._cmdPacketPool;
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x06003CB4 RID: 15540 RVA: 0x00341900 File Offset: 0x0033FB00
		// (set) Token: 0x06003CB5 RID: 15541 RVA: 0x00341918 File Offset: 0x0033FB18
		public bool IsDealingByWorkerThread
		{
			get
			{
				return this._isDealingByWorkerThread;
			}
			set
			{
				this._isDealingByWorkerThread = value;
			}
		}

		// Token: 0x06003CB6 RID: 15542 RVA: 0x00341924 File Offset: 0x0033FB24
		public bool CacheCmdPacketData(int nID, byte[] data, int count)
		{
			bool result;
			if (this._cmdPacketPool.Count > 100)
			{
				result = false;
			}
			else
			{
				lock (this._cmdPacketPool)
				{
					this._cmdPacketPool.Enqueue(new CmdPacket(nID, data, count));
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06003CB7 RID: 15543 RVA: 0x003419A0 File Offset: 0x0033FBA0
		public bool PopCmdPackets(Queue<CmdPacket> ls)
		{
			ls.Clear();
			bool result;
			lock (this._cmdPacketPool)
			{
				if (this._isDealingByWorkerThread || this._cmdPacketPool.Count <= 0)
				{
					result = false;
				}
				else
				{
					this._isDealingByWorkerThread = true;
					for (int i = 0; i < 6; i++)
					{
						if (this._cmdPacketPool.Count <= 0)
						{
							break;
						}
						ls.Enqueue(this._cmdPacketPool.Dequeue());
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06003CB8 RID: 15544 RVA: 0x00341A5C File Offset: 0x0033FC5C
		public void OnThreadDealingComplete()
		{
			lock (this._cmdPacketPool)
			{
				this._isDealingByWorkerThread = false;
			}
		}

		// Token: 0x06003CB9 RID: 15545 RVA: 0x00341AA8 File Offset: 0x0033FCA8
		public int GetCacheCmdPacketCount()
		{
			int result;
			lock (this._cmdPacketPool)
			{
				result = this._cmdPacketPool.Count<CmdPacket>();
			}
			return result;
		}

		// Token: 0x06003CBA RID: 15546 RVA: 0x00341AFC File Offset: 0x0033FCFC
		public void Dispose()
		{
			this.Reset();
			TCPInPacket.DecInstanceCount();
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06003CBB RID: 15547 RVA: 0x00341B0C File Offset: 0x0033FD0C
		// (remove) Token: 0x06003CBC RID: 15548 RVA: 0x00341B48 File Offset: 0x0033FD48
		public event TCPCmdPacketEventHandler TCPCmdPacketEvent;

		// Token: 0x06003CBD RID: 15549 RVA: 0x00341B84 File Offset: 0x0033FD84
		public bool WriteData(byte[] buffer, int offset, int count)
		{
			bool result;
			lock (this.mutex)
			{
				if (this.IsWaitingData)
				{
					int copyCount = (count >= this._PacketDataSize - this.PacketDataHaveSize) ? (this._PacketDataSize - this.PacketDataHaveSize) : count;
					if (copyCount > 0)
					{
						DataHelper.CopyBytes(this.PacketBytes, this.PacketDataHaveSize, buffer, offset, copyCount);
						this.PacketDataHaveSize += copyCount;
					}
					if (this.PacketDataHaveSize >= this._PacketDataSize)
					{
						bool eventReturn = true;
						if (null != this.TCPCmdPacketEvent)
						{
							eventReturn = this.TCPCmdPacketEvent(this, 0);
						}
						this.LastPacketCmdID = this._PacketCmdID;
						this._PacketCmdID = 0;
						this._PacketDataSize = 0;
						this.PacketDataHaveSize = 0;
						this.IsWaitingData = false;
						this.CmdHeaderSize = 0;
						if (!eventReturn)
						{
							return false;
						}
						if (count > copyCount)
						{
							offset += copyCount;
							count -= copyCount;
							return this.WriteData(buffer, offset, count);
						}
					}
					result = true;
				}
				else
				{
					int copyLeftSize = (count > 6 - this.CmdHeaderSize) ? (6 - this.CmdHeaderSize) : count;
					DataHelper.CopyBytes(this.CmdHeaderBuffer, this.CmdHeaderSize, buffer, offset, copyLeftSize);
					this.CmdHeaderSize += copyLeftSize;
					if (this.CmdHeaderSize < 6)
					{
						result = true;
					}
					else
					{
						this._PacketDataSize = BitConverter.ToInt32(this.CmdHeaderBuffer, 0);
						this._PacketCmdID = BitConverter.ToUInt16(this.CmdHeaderBuffer, 4);
						if (this._Socket == null)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("TcpInPacket.WriteData 检查到socket为null, 可能是别的线程关闭了该socket后重置TcpInPacket, 当前消息(极有可能不准):{0}[{1}]", (TCPGameServerCmds)this._PacketCmdID, this._PacketCmdID), null, true);
						}
						else if (this._Socket.magic > 0)
						{
							if (this._PacketCmdID > 100)
							{
								if (this._PacketCmdID <= 30767)
								{
									string uid = GameManager.OnlineUserSession.FindUserID(this._Socket);
									LogManager.WriteLog(LogTypes.Fatal, string.Format("客户端UID={0}, IP={1} 消息偏移后，消息({2})处于(CMD_LOGIN_ON, CMD_DB_ERR_RETURN]范围内", uid, this._Socket.RemoteEndPoint, (TCPGameServerCmds)this._PacketCmdID), null, false);
									return false;
								}
								this._PacketCmdID -= this._Socket.magic;
							}
						}
						if (this._PacketDataSize <= 0 || this._PacketDataSize >= 6144)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("接收到的非法数据长度的tcp命令, Cmd={0}, Length={1}, offset={2}, count={3}", new object[]
							{
								(TCPGameServerCmds)this._PacketCmdID,
								this._PacketDataSize,
								offset,
								count
							}), null, true);
							result = false;
						}
						else
						{
							offset += copyLeftSize;
							count -= copyLeftSize;
							this.IsWaitingData = true;
							this.PacketDataHaveSize = 0;
							this._PacketDataSize -= 2;
							result = this.WriteData(buffer, offset, count);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06003CBE RID: 15550 RVA: 0x00341EF8 File Offset: 0x003400F8
		public void Reset()
		{
			lock (this.mutex)
			{
				this._Socket = null;
				this._PacketCmdID = 0;
				this.LastPacketCmdID = 0;
				this._PacketDataSize = 0;
				this.PacketDataHaveSize = 0;
				this.IsWaitingData = false;
				this.CmdHeaderSize = 0;
				this._cmdPacketPool.Clear();
				this._LastCheckTicks = 0;
			}
		}

		// Token: 0x06003CBF RID: 15551 RVA: 0x00341F84 File Offset: 0x00340184
		private bool HandlePolicyFileRequest(byte[] buffer, int offset, int count)
		{
			this.TCPCmdPacketEvent(this, 1);
			this._PacketCmdID = 0;
			this._PacketDataSize = 0;
			this.PacketDataHaveSize = 0;
			this.IsWaitingData = false;
			this.CmdHeaderSize = 0;
			this._LastCheckTicks = 0;
			return true;
		}

		// Token: 0x06003CC0 RID: 15552 RVA: 0x00341FD0 File Offset: 0x003401D0
		public static void IncInstanceCount()
		{
			lock (TCPInPacket.CountLock)
			{
				TCPInPacket.TotalInstanceCount++;
			}
		}

		// Token: 0x06003CC1 RID: 15553 RVA: 0x00342020 File Offset: 0x00340220
		public static void DecInstanceCount()
		{
			lock (TCPInPacket.CountLock)
			{
				TCPInPacket.TotalInstanceCount--;
			}
		}

		// Token: 0x06003CC2 RID: 15554 RVA: 0x00342070 File Offset: 0x00340270
		public static int GetInstanceCount()
		{
			int count = 0;
			lock (TCPInPacket.CountLock)
			{
				count = TCPInPacket.TotalInstanceCount;
			}
			return count;
		}

		// Token: 0x04004720 RID: 18208
		public const string POLICY_STRING = "<policy-file-request/>\0";

		// Token: 0x04004721 RID: 18209
		private object mutex = new object();

		// Token: 0x04004722 RID: 18210
		private byte[] PacketBytes = null;

		// Token: 0x04004723 RID: 18211
		private int _LastCheckTicks = 0;

		// Token: 0x04004724 RID: 18212
		private TMSKSocket _Socket = null;

		// Token: 0x04004725 RID: 18213
		private ushort _PacketCmdID = 0;

		// Token: 0x04004726 RID: 18214
		private int _PacketDataSize = 0;

		// Token: 0x04004727 RID: 18215
		private Queue<CmdPacket> _cmdPacketPool = new Queue<CmdPacket>();

		// Token: 0x04004728 RID: 18216
		private bool _isDealingByWorkerThread = false;

		// Token: 0x04004729 RID: 18217
		private int PacketDataHaveSize = 0;

		// Token: 0x0400472A RID: 18218
		private bool IsWaitingData = false;

		// Token: 0x0400472C RID: 18220
		private byte[] CmdHeaderBuffer = new byte[6];

		// Token: 0x0400472D RID: 18221
		private int CmdHeaderSize = 0;

		// Token: 0x0400472E RID: 18222
		private static object CountLock = new object();

		// Token: 0x0400472F RID: 18223
		private static int TotalInstanceCount = 0;
	}
}
