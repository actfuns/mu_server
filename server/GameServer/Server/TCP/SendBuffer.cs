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
	// Token: 0x020008C7 RID: 2247
	public class SendBuffer
	{
		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x0600400B RID: 16395 RVA: 0x003BA714 File Offset: 0x003B8914
		// (set) Token: 0x0600400C RID: 16396 RVA: 0x003BA72B File Offset: 0x003B892B
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

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x0600400D RID: 16397 RVA: 0x003BA734 File Offset: 0x003B8934
		// (set) Token: 0x0600400E RID: 16398 RVA: 0x003BA74B File Offset: 0x003B894B
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

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x0600400F RID: 16399 RVA: 0x003BA754 File Offset: 0x003B8954
		// (set) Token: 0x06004010 RID: 16400 RVA: 0x003BA76B File Offset: 0x003B896B
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

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x06004011 RID: 16401 RVA: 0x003BA774 File Offset: 0x003B8974
		// (set) Token: 0x06004012 RID: 16402 RVA: 0x003BA78C File Offset: 0x003B898C
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

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x06004013 RID: 16403 RVA: 0x003BA798 File Offset: 0x003B8998
		// (set) Token: 0x06004014 RID: 16404 RVA: 0x003BA7B0 File Offset: 0x003B89B0
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

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06004015 RID: 16405 RVA: 0x003BA7BC File Offset: 0x003B89BC
		// (set) Token: 0x06004016 RID: 16406 RVA: 0x003BA7D4 File Offset: 0x003B89D4
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

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06004017 RID: 16407 RVA: 0x003BA7E0 File Offset: 0x003B89E0
		// (set) Token: 0x06004018 RID: 16408 RVA: 0x003BA7F8 File Offset: 0x003B89F8
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

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06004019 RID: 16409 RVA: 0x003BA804 File Offset: 0x003B8A04
		// (set) Token: 0x0600401A RID: 16410 RVA: 0x003BA81C File Offset: 0x003B8A1C
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

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x0600401B RID: 16411 RVA: 0x003BA828 File Offset: 0x003B8A28
		// (set) Token: 0x0600401C RID: 16412 RVA: 0x003BA840 File Offset: 0x003B8A40
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

		// Token: 0x0600401D RID: 16413 RVA: 0x003BA84C File Offset: 0x003B8A4C
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

		// Token: 0x0600401E RID: 16414 RVA: 0x003BA90C File Offset: 0x003B8B0C
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

		// Token: 0x0600401F RID: 16415 RVA: 0x003BA9EC File Offset: 0x003B8BEC
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

		// Token: 0x06004020 RID: 16416 RVA: 0x003BAA74 File Offset: 0x003B8C74
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

		// Token: 0x06004021 RID: 16417 RVA: 0x003BAB50 File Offset: 0x003B8D50
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

		// Token: 0x06004022 RID: 16418 RVA: 0x003BABF4 File Offset: 0x003B8DF4
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

		// Token: 0x06004023 RID: 16419 RVA: 0x003BACE4 File Offset: 0x003B8EE4
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

		// Token: 0x06004024 RID: 16420 RVA: 0x003BAD68 File Offset: 0x003B8F68
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

		// Token: 0x06004025 RID: 16421 RVA: 0x003BADC0 File Offset: 0x003B8FC0
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

		// Token: 0x06004026 RID: 16422 RVA: 0x003BAE58 File Offset: 0x003B9058
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

		// Token: 0x06004027 RID: 16423 RVA: 0x003BAF20 File Offset: 0x003B9120
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

		// Token: 0x06004028 RID: 16424 RVA: 0x003BB0DC File Offset: 0x003B92DC
		public void OnSendOK()
		{
			Interlocked.Exchange(ref this._SendTimeoutTickCount, long.MaxValue);
			this.HasLogTimeOutError = false;
			this.HasLogBufferFull = false;
			this.HasLogDiscardBigPacket = false;
			Thread.MemoryBarrier();
			this._IsSendding = false;
		}

		// Token: 0x06004029 RID: 16425 RVA: 0x003BB128 File Offset: 0x003B9328
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

		// Token: 0x04004F1C RID: 20252
		public object BufferLock = new object();

		// Token: 0x04004F1D RID: 20253
		private object SendLock = new object();

		// Token: 0x04004F1E RID: 20254
		public static int ConstMinSendSize = 32768;

		// Token: 0x04004F1F RID: 20255
		private static long _SendDataIntervalTicks = 80L;

		// Token: 0x04004F20 RID: 20256
		private static int _MaxSingleSocketSendBufferSize = 0;

		// Token: 0x04004F21 RID: 20257
		private static long _SendDataTimeOutTicks = 5000L;

		// Token: 0x04004F22 RID: 20258
		private long _AddFirstPacketTicks = 0L;

		// Token: 0x04004F23 RID: 20259
		private long _LastSendDataTicks = 0L;

		// Token: 0x04004F24 RID: 20260
		private long _SendTimeoutTickCount = long.MaxValue;

		// Token: 0x04004F25 RID: 20261
		private bool _IsSendding = false;

		// Token: 0x04004F26 RID: 20262
		private byte[] _Buffer = null;

		// Token: 0x04004F27 RID: 20263
		public Queue<MemoryBlock> QueueMemoryBlock = new Queue<MemoryBlock>();

		// Token: 0x04004F28 RID: 20264
		private int _MaxBufferSize = 0;

		// Token: 0x04004F29 RID: 20265
		private int _CurrentBufferSize = 0;

		// Token: 0x04004F2A RID: 20266
		private int _UsedBufferSize = 0;

		// Token: 0x04004F2B RID: 20267
		public static int MaxBufferSizeForLargePackge = 65536;

		// Token: 0x04004F2C RID: 20268
		private MemoryBlock _MemoryBlock = null;

		// Token: 0x04004F2D RID: 20269
		public bool HasLogTimeOutError = false;

		// Token: 0x04004F2E RID: 20270
		public bool HasLogBufferFull = false;

		// Token: 0x04004F2F RID: 20271
		public bool HasLogDiscardBigPacket = false;
	}
}
