using System;
using System.Net.Sockets;
using LogDBServer.Server;
using Server.Tools;

namespace Server.Protocol
{
	
	internal class TCPInPacket : IDisposable
	{
		
		public TCPInPacket(int recvBufferSize = 131072)
		{
			this.PacketBytes = new byte[recvBufferSize];
		}

		
		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		
		
		
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

		
		public void Dispose()
		{
			this.Reset();
		}

		
		// (add) Token: 0x060000B4 RID: 180 RVA: 0x000057F8 File Offset: 0x000039F8
		// (remove) Token: 0x060000B5 RID: 181 RVA: 0x00005834 File Offset: 0x00003A34
		public event TCPCmdPacketEventHandler TCPCmdPacketEvent;

		
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

		
		private byte[] PacketBytes = null;

		
		private Socket _Socket = null;

		
		private ushort _PacketCmdID = 0;

		
		private int _PacketDataSize = 0;

		
		private int PacketDataHaveSize = 0;

		
		private bool IsWaitingData = false;

		
		private byte[] CmdHeaderBuffer = new byte[6];

		
		private int CmdHeaderSize = 0;
	}
}
