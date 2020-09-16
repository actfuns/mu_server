using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic;
using GameServer.Server;
using Server.Protocol;
using Server.Tools;

namespace Server.TCP
{
	
	public class SendBuffer
	{
		
		
		
		public static long SendDataIntervalTicks
		{
			get
			{
				return SendBuffer._SendDataIntervalTicks;
			}
			set
			{
				SendBuffer._SendDataIntervalTicks = value;
			}
		}

		
		
		
		public static int MaxSingleSocketSendBufferSize
		{
			get
			{
				return SendBuffer._MaxSingleSocketSendBufferSize;
			}
			set
			{
				SendBuffer._MaxSingleSocketSendBufferSize = value;
			}
		}

		
		
		
		public static long SendDataTimeOutTicks
		{
			get
			{
				return SendBuffer._SendDataTimeOutTicks;
			}
			set
			{
				SendBuffer._SendDataTimeOutTicks = value;
			}
		}

		
		
		
		public long AddFirstPacketTicks
		{
			get
			{
				return this._AddFirstPacketTicks;
			}
			set
			{
				this._AddFirstPacketTicks = value;
			}
		}

		
		
		
		public long LastSendDataTicks
		{
			get
			{
				return this._LastSendDataTicks;
			}
			set
			{
				this._LastSendDataTicks = value;
			}
		}

		
		
		
		public byte[] Buffer
		{
			get
			{
				return this._Buffer;
			}
			set
			{
				this._Buffer = value;
			}
		}

		
		
		
		public int MaxBufferSize
		{
			get
			{
				return this._MaxBufferSize;
			}
			set
			{
				this._MaxBufferSize = value;
			}
		}

		
		
		
		public int CurrentBufferSize
		{
			get
			{
				return this._CurrentBufferSize;
			}
			set
			{
				this._CurrentBufferSize = value;
			}
		}

		
		
		
		public MemoryBlock MyMemoryBlock
		{
			get
			{
				return this._MemoryBlock;
			}
			set
			{
				this._MemoryBlock = value;
			}
		}

		
		public SendBuffer(int maxBufferSize = 0)
		{
			if (maxBufferSize <= 0)
			{
				this._MaxBufferSize = SendBuffer._MaxSingleSocketSendBufferSize;
			}
			else
			{
				this._MaxBufferSize = maxBufferSize;
			}
			this.Reset(true);
		}

		
		protected void Reset(bool init = false)
		{
			if (init)
			{
				this._AddFirstPacketTicks = 0L;
				this._CurrentBufferSize = 0;
			}
			if (this._MaxBufferSize > 0 && this._MaxBufferSize <= 4194304)
			{
				bool needMemoryBlock = true;
				if (null != this._MemoryBlock)
				{
					if (this._MemoryBlock.BlockSize < this._MaxBufferSize)
					{
						Global._MemoryManager.Push(this._MemoryBlock);
						this._MemoryBlock = null;
						this._Buffer = null;
					}
					else
					{
						needMemoryBlock = false;
					}
				}
				if (needMemoryBlock)
				{
					this._MemoryBlock = Global._MemoryManager.Pop(this._MaxBufferSize);
					this._Buffer = this._MemoryBlock.Buffer;
				}
			}
		}

		
		public void Reset2()
		{
			lock (this.BufferLock)
			{
				int remainSize = this._CurrentBufferSize - this._UsedBufferSize;
				if (remainSize > 0)
				{
					Array.Copy(this._Buffer, this._UsedBufferSize, this._Buffer, 0, remainSize);
				}
				this._CurrentBufferSize = remainSize;
				this._UsedBufferSize = 0;
			}
		}

		
		public bool AddBuffer(byte[] buffer, int offset, int count, TMSKSocket s)
		{
			bool result;
			if (buffer == null || buffer.Length < offset + count)
			{
				result = false;
			}
			else
			{
				lock (this.BufferLock)
				{
					if (this._MaxBufferSize - this._CurrentBufferSize <= count)
					{
						return false;
					}
					if (0 == this._CurrentBufferSize)
					{
						this._AddFirstPacketTicks = TimeUtil.NOW();
					}
					DataHelper.CopyBytes(this._Buffer, this._CurrentBufferSize, buffer, offset, count);
					this._CurrentBufferSize += count;
					this.TrySend(s, this._CurrentBufferSize > SendBuffer.ConstMinSendSize);
				}
				result = true;
			}
			return result;
		}

		
		private bool TrySend(TMSKSocket s, bool force = false)
		{
			long ticks = TimeUtil.NOW();
			bool result;
			if (!this._IsSendding && this._CurrentBufferSize > 0 && (force || ticks - this.AddFirstPacketTicks >= SendBuffer._SendDataIntervalTicks))
			{
				this._IsSendding = true;
				Interlocked.Exchange(ref this._SendTimeoutTickCount, ticks + SendBuffer._SendDataTimeOutTicks);
				Global._TCPManager.MySocketListener.SendData(s, this._Buffer, 0, this._CurrentBufferSize, this._MemoryBlock);
				this.Reset(false);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		private bool TrySend2(TMSKSocket s, long ticks, bool force = false)
		{
			int sendSize = 0;
			lock (this.BufferLock)
			{
				if (!this._IsSendding && this._CurrentBufferSize > 0 && (force || ticks - this.AddFirstPacketTicks >= SendBuffer._SendDataIntervalTicks))
				{
					this._IsSendding = true;
					sendSize = (this._UsedBufferSize = this._CurrentBufferSize);
					Interlocked.Exchange(ref this._SendTimeoutTickCount, ticks + SendBuffer._SendDataTimeOutTicks);
					this._AddFirstPacketTicks = 0L;
				}
			}
			bool result;
			if (sendSize > 0)
			{
				Global._TCPManager.MySocketListener.SendData(s, this._Buffer, 0, sendSize, this._MemoryBlock, this);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool ExternalTrySend(TMSKSocket s, bool force = false, int milliseconds = 0)
		{
			long ticks = TimeUtil.NOW();
			if (ticks - this.AddFirstPacketTicks >= SendBuffer._SendDataIntervalTicks || ticks < this.AddFirstPacketTicks)
			{
				if (Monitor.TryEnter(this.SendLock))
				{
					try
					{
						return this.TrySend2(s, ticks, force);
					}
					finally
					{
						Monitor.Exit(this.SendLock);
					}
				}
			}
			return false;
		}

		
		protected bool IsSendTimeOut()
		{
			bool result;
			if (this._LastSendDataTicks <= 0L)
			{
				result = false;
			}
			else
			{
				long ticks = TimeUtil.NOW();
				result = (ticks - this._LastSendDataTicks >= SendBuffer._SendDataTimeOutTicks && this._LastSendDataTicks > 0L);
			}
			return result;
		}

		
		protected bool CanDiscardCmd(int cmdID)
		{
			if (cmdID <= 127)
			{
				if (cmdID <= 106)
				{
					if (cmdID != 21)
					{
						switch (cmdID)
						{
						case 104:
						case 106:
							break;
						case 105:
							goto IL_85;
						default:
							goto IL_85;
						}
					}
				}
				else
				{
					switch (cmdID)
					{
					case 119:
					case 120:
					case 123:
						break;
					case 121:
					case 122:
						goto IL_85;
					default:
						if (cmdID != 127)
						{
							goto IL_85;
						}
						break;
					}
				}
			}
			else if (cmdID <= 181)
			{
				if (cmdID != 160 && cmdID != 181)
				{
					goto IL_85;
				}
			}
			else if (cmdID != 209 && cmdID != 614 && cmdID != 30000)
			{
				goto IL_85;
			}
			return false;
			IL_85:
			return true;
		}

		
		public bool CanSend(int bytesCount, int cmdID, out int canNotSendReason, byte[] buffer, TMSKSocket s)
		{
			canNotSendReason = -1;
			long ticks = TimeUtil.NOW();
			bool result;
			lock (this.BufferLock)
			{
				if (ticks > Interlocked.Read(ref this._SendTimeoutTickCount) && this.CanDiscardCmd(cmdID))
				{
					canNotSendReason = 0;
					result = false;
				}
				else
				{
					if (110 == cmdID)
					{
						if (this._CurrentBufferSize >= this._MaxBufferSize - this._CurrentBufferSize << 2)
						{
							canNotSendReason = 2;
							return false;
						}
					}
					result = this.AddBuffer(buffer, 0, bytesCount, s);
				}
			}
			return result;
		}

		
		public bool CanSend2(TMSKSocket s, TCPOutPacket tcpOutPacket, ref int canNotSendReason)
		{
			int cmdID = (int)tcpOutPacket.PacketCmdID;
			long ticks = TimeUtil.NOW();
			byte[] buffer = tcpOutPacket.GetPacketBytes();
			int count = tcpOutPacket.PacketDataSize;
			bool result;
			if (buffer == null || count > buffer.Length)
			{
				result = false;
			}
			else
			{
				TCPManager.RecordCmdOutputDataSize(cmdID, (long)count);
				int needRemainSize = this._MaxBufferSize - count;
				bool isLargePackge = 110 == cmdID;
				lock (this.BufferLock)
				{
					if (this._CurrentBufferSize >= needRemainSize)
					{
						canNotSendReason = 1;
						return false;
					}
					if (0 == this._CurrentBufferSize)
					{
						this._AddFirstPacketTicks = ticks;
					}
					if (ticks > Interlocked.Read(ref this._SendTimeoutTickCount) && this.CanDiscardCmd(cmdID))
					{
						canNotSendReason = 0;
						return false;
					}
					if (isLargePackge)
					{
						if (this._CurrentBufferSize >= SendBuffer.MaxBufferSizeForLargePackge)
						{
							canNotSendReason = 2;
							return false;
						}
					}
					DataHelper.CopyBytes(this._Buffer, this._CurrentBufferSize, buffer, 0, count);
					this._CurrentBufferSize += count;
				}
				if (Monitor.TryEnter(this.SendLock))
				{
					bool force = this._CurrentBufferSize > SendBuffer.ConstMinSendSize;
					try
					{
						this.TrySend2(s, ticks, force);
					}
					finally
					{
						Monitor.Exit(this.SendLock);
					}
				}
				result = true;
			}
			return result;
		}

		
		public void OnSendOK()
		{
			Interlocked.Exchange(ref this._SendTimeoutTickCount, long.MaxValue);
			this.HasLogTimeOutError = false;
			this.HasLogBufferFull = false;
			this.HasLogDiscardBigPacket = false;
			Thread.MemoryBarrier();
			this._IsSendding = false;
		}

		
		public bool CanLog(int canNotSendReason)
		{
			bool bCanLog = false;
			switch (canNotSendReason)
			{
			case 0:
				if (!this.HasLogTimeOutError)
				{
					this.HasLogTimeOutError = true;
					bCanLog = true;
				}
				break;
			case 1:
				if (!this.HasLogBufferFull)
				{
					this.HasLogBufferFull = true;
					bCanLog = true;
				}
				break;
			case 2:
				if (!this.HasLogDiscardBigPacket)
				{
					this.HasLogDiscardBigPacket = true;
					bCanLog = true;
				}
				break;
			}
			return bCanLog;
		}

		
		public object BufferLock = new object();

		
		private object SendLock = new object();

		
		public static int ConstMinSendSize = 32768;

		
		private static long _SendDataIntervalTicks = 80L;

		
		private static int _MaxSingleSocketSendBufferSize = 0;

		
		private static long _SendDataTimeOutTicks = 5000L;

		
		private long _AddFirstPacketTicks = 0L;

		
		private long _LastSendDataTicks = 0L;

		
		private long _SendTimeoutTickCount = long.MaxValue;

		
		private bool _IsSendding = false;

		
		private byte[] _Buffer = null;

		
		public Queue<MemoryBlock> QueueMemoryBlock = new Queue<MemoryBlock>();

		
		private int _MaxBufferSize = 0;

		
		private int _CurrentBufferSize = 0;

		
		private int _UsedBufferSize = 0;

		
		public static int MaxBufferSizeForLargePackge = 65536;

		
		private MemoryBlock _MemoryBlock = null;

		
		public bool HasLogTimeOutError = false;

		
		public bool HasLogBufferFull = false;

		
		public bool HasLogDiscardBigPacket = false;
	}
}
