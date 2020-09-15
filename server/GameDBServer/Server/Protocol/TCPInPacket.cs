using System;
using System.Net.Sockets;
using GameDBServer.Server;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x020001E3 RID: 483
	internal class TCPInPacket : IDisposable
	{
		// Token: 0x06000A0D RID: 2573 RVA: 0x00060B88 File Offset: 0x0005ED88
		public TCPInPacket(int recvBufferSize = 131072)
		{
			this.PacketBytes = new byte[recvBufferSize];
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00060BE8 File Offset: 0x0005EDE8
		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000A0F RID: 2575 RVA: 0x00060C00 File Offset: 0x0005EE00
		// (set) Token: 0x06000A10 RID: 2576 RVA: 0x00060C18 File Offset: 0x0005EE18
		public Socket CurrentSocket
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

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000A11 RID: 2577 RVA: 0x00060C24 File Offset: 0x0005EE24
		public ushort PacketCmdID
		{
			get
			{
				ushort ret = 0;
				lock (this)
				{
					ret = this._PacketCmdID;
				}
				return ret;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000A12 RID: 2578 RVA: 0x00060C74 File Offset: 0x0005EE74
		public int PacketDataSize
		{
			get
			{
				int ret = 0;
				lock (this)
				{
					ret = this._PacketDataSize;
				}
				return ret;
			}
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x00060CC4 File Offset: 0x0005EEC4
		public void Dispose()
		{
			this.Reset();
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000A14 RID: 2580 RVA: 0x00060CD0 File Offset: 0x0005EED0
		// (remove) Token: 0x06000A15 RID: 2581 RVA: 0x00060D0C File Offset: 0x0005EF0C
		public event TCPCmdPacketEventHandler TCPCmdPacketEvent;

		// Token: 0x06000A16 RID: 2582 RVA: 0x00060D48 File Offset: 0x0005EF48
		public bool WriteData(byte[] buffer, int offset, int count)
		{
			bool result;
			lock (this)
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
							eventReturn = this.TCPCmdPacketEvent(this);
						}
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
						if (this._PacketDataSize <= 0 || this._PacketDataSize >= 131072)
						{
							if (this._PacketDataSize <= 0 || this._PacketDataSize >= 1048576)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("接收到的非法数据长度的tcp命令, Cmd={0}, Length={1}, offset={2}, count={3}", new object[]
								{
									(TCPGameServerCmds)this._PacketCmdID,
									this._PacketDataSize,
									offset,
									count
								}), null, true);
								return false;
							}
							LogManager.WriteLog(LogTypes.Error, string.Format("接收到的数据长度过长的tcp命令, Cmd={0}, Length={1}, offset={2}, count={3}", new object[]
							{
								(TCPGameServerCmds)this._PacketCmdID,
								this._PacketDataSize,
								offset,
								count
							}), null, true);
						}
						offset += copyLeftSize;
						count -= copyLeftSize;
						this.IsWaitingData = true;
						this.PacketDataHaveSize = 0;
						this._PacketDataSize -= 2;
						result = this.WriteData(buffer, offset, count);
					}
				}
			}
			return result;
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00061034 File Offset: 0x0005F234
		public void Reset()
		{
			lock (this)
			{
				this._Socket = null;
				this._PacketCmdID = 0;
				this._PacketDataSize = 0;
				this.PacketDataHaveSize = 0;
				this.IsWaitingData = false;
				this.CmdHeaderSize = 0;
			}
		}

		// Token: 0x04000C36 RID: 3126
		private byte[] PacketBytes = null;

		// Token: 0x04000C37 RID: 3127
		private Socket _Socket = null;

		// Token: 0x04000C38 RID: 3128
		private ushort _PacketCmdID = 0;

		// Token: 0x04000C39 RID: 3129
		private int _PacketDataSize = 0;

		// Token: 0x04000C3A RID: 3130
		private int PacketDataHaveSize = 0;

		// Token: 0x04000C3B RID: 3131
		private bool IsWaitingData = false;

		// Token: 0x04000C3D RID: 3133
		private byte[] CmdHeaderBuffer = new byte[6];

		// Token: 0x04000C3E RID: 3134
		private int CmdHeaderSize = 0;
	}
}
