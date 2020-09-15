using System;
using System.Net.Sockets;
using LogDBServer.Server;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x02000021 RID: 33
	internal class TCPInPacket : IDisposable
	{
		// Token: 0x060000AD RID: 173 RVA: 0x000056B0 File Offset: 0x000038B0
		public TCPInPacket(int recvBufferSize = 131072)
		{
			this.PacketBytes = new byte[recvBufferSize];
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005710 File Offset: 0x00003910
		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00005728 File Offset: 0x00003928
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00005740 File Offset: 0x00003940
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x0000574C File Offset: 0x0000394C
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

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x0000579C File Offset: 0x0000399C
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

		// Token: 0x060000B3 RID: 179 RVA: 0x000057EC File Offset: 0x000039EC
		public void Dispose()
		{
			this.Reset();
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000B4 RID: 180 RVA: 0x000057F8 File Offset: 0x000039F8
		// (remove) Token: 0x060000B5 RID: 181 RVA: 0x00005834 File Offset: 0x00003A34
		public event TCPCmdPacketEventHandler TCPCmdPacketEvent;

		// Token: 0x060000B6 RID: 182 RVA: 0x00005870 File Offset: 0x00003A70
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
							LogManager.WriteLog(LogTypes.Error, string.Format("接收到的非法数据长度的tcp命令, Cmd={0}, Length={1}, offset={2}, count={3}", new object[]
							{
								(TCPGameServerCmds)this._PacketCmdID,
								this._PacketDataSize,
								offset,
								count
							}));
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

		// Token: 0x060000B7 RID: 183 RVA: 0x00005AE0 File Offset: 0x00003CE0
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

		// Token: 0x0400004F RID: 79
		private byte[] PacketBytes = null;

		// Token: 0x04000050 RID: 80
		private Socket _Socket = null;

		// Token: 0x04000051 RID: 81
		private ushort _PacketCmdID = 0;

		// Token: 0x04000052 RID: 82
		private int _PacketDataSize = 0;

		// Token: 0x04000053 RID: 83
		private int PacketDataHaveSize = 0;

		// Token: 0x04000054 RID: 84
		private bool IsWaitingData = false;

		// Token: 0x04000056 RID: 86
		private byte[] CmdHeaderBuffer = new byte[6];

		// Token: 0x04000057 RID: 87
		private int CmdHeaderSize = 0;
	}
}
