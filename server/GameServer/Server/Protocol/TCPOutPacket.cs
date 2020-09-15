using System;
using System.Text;
using System.Threading;
using GameServer.Logic;
using Server.TCP;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x0200086A RID: 2154
	public class TCPOutPacket : IDisposable
	{
		// Token: 0x06003CC8 RID: 15560 RVA: 0x00342254 File Offset: 0x00340454
		public TCPOutPacket()
		{
			TCPOutPacket.IncInstanceCount();
		}

		// Token: 0x06003CC9 RID: 15561 RVA: 0x00342284 File Offset: 0x00340484
		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06003CCA RID: 15562 RVA: 0x0034229C File Offset: 0x0034049C
		// (set) Token: 0x06003CCB RID: 15563 RVA: 0x003422B4 File Offset: 0x003404B4
		public ushort PacketCmdID
		{
			get
			{
				return this._PacketCmdID;
			}
			set
			{
				this._PacketCmdID = value;
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06003CCC RID: 15564 RVA: 0x003422C0 File Offset: 0x003404C0
		public int PacketDataSize
		{
			get
			{
				return this._PacketDataSize + 6;
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x06003CCD RID: 15565 RVA: 0x003422DC File Offset: 0x003404DC
		// (set) Token: 0x06003CCE RID: 15566 RVA: 0x003422F3 File Offset: 0x003404F3
		public object Tag { get; set; }

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06003CCF RID: 15567 RVA: 0x003422FC File Offset: 0x003404FC
		// (set) Token: 0x06003CD0 RID: 15568 RVA: 0x00342314 File Offset: 0x00340514
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

		// Token: 0x06003CD1 RID: 15569 RVA: 0x00342320 File Offset: 0x00340520
		public bool FinalWriteData(byte[] buffer, int offset, int count)
		{
			bool result;
			if (null != this.PacketBytes)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("TCP发出命令包不能被重复写入数据, 命令ID: {0}", this.PacketCmdID), null, true);
				result = false;
			}
			else
			{
				if (6 + count >= 131072)
				{
					if (count >= 1048576)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("TCP命令包长度:{0}, 最大不能超过: {1}, 命令ID: {2}", count, 1048576, this.PacketCmdID), null, true);
						return false;
					}
					LogManager.WriteLog(LogTypes.Error, string.Format("TCP命令包长度:{0}, 警告长度: {1}, 命令ID: {2}", count, 131072, this.PacketCmdID), null, true);
				}
				this._MemoryBlock = Global._MemoryManager.Pop(count + 6);
				this.PacketBytes = this._MemoryBlock.Buffer;
				int offsetTo = 6;
				DataHelper.CopyBytes(this.PacketBytes, offsetTo, buffer, offset, count);
				this._PacketDataSize = count;
				this.Final();
				result = true;
			}
			return result;
		}

		// Token: 0x06003CD2 RID: 15570 RVA: 0x00342430 File Offset: 0x00340630
		public int GetPacketCmdData(out string cmddata)
		{
			int result;
			if (this.PacketBytes == null || 0 >= this._PacketDataSize)
			{
				cmddata = null;
				result = 0;
			}
			else
			{
				cmddata = new UTF8Encoding().GetString(this.PacketBytes, 6, this._PacketDataSize);
				result = this._PacketDataSize;
			}
			return result;
		}

		// Token: 0x06003CD3 RID: 15571 RVA: 0x00342484 File Offset: 0x00340684
		private void Final()
		{
			int length = this._PacketDataSize + 2;
			DataHelper.CopyBytes(this.PacketBytes, 0, BitConverter.GetBytes(length), 0, 4);
			DataHelper.CopyBytes(this.PacketBytes, 4, BitConverter.GetBytes(this._PacketCmdID), 0, 2);
		}

		// Token: 0x06003CD4 RID: 15572 RVA: 0x003424CC File Offset: 0x003406CC
		public void Reset()
		{
			this.PacketBytes = null;
			this.PacketCmdID = 0;
			this._PacketDataSize = 0;
			if (null != this._MemoryBlock)
			{
				Global._MemoryManager.Push(this._MemoryBlock);
				this._MemoryBlock = null;
			}
		}

		// Token: 0x06003CD5 RID: 15573 RVA: 0x0034251D File Offset: 0x0034071D
		public void Dispose()
		{
			TCPOutPacket.DecInstanceCount();
			this.Tag = null;
		}

		// Token: 0x06003CD6 RID: 15574 RVA: 0x00342530 File Offset: 0x00340730
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, string data, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			byte[] bytesCmd = new UTF8Encoding().GetBytes(data);
			tcpOutPacket.FinalWriteData(bytesCmd, 0, bytesCmd.Length);
			return tcpOutPacket;
		}

		// Token: 0x06003CD7 RID: 15575 RVA: 0x0034256C File Offset: 0x0034076C
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, byte[] data, int offset, int length, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			tcpOutPacket.FinalWriteData(data, offset, length);
			return tcpOutPacket;
		}

		// Token: 0x06003CD8 RID: 15576 RVA: 0x0034259C File Offset: 0x0034079C
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, byte[] data, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			tcpOutPacket.FinalWriteData(data, 0, data.Length);
			return tcpOutPacket;
		}

		// Token: 0x06003CD9 RID: 15577 RVA: 0x003425CB File Offset: 0x003407CB
		public static void IncInstanceCount()
		{
			Interlocked.Increment(ref TCPOutPacket.TotalInstanceCount);
		}

		// Token: 0x06003CDA RID: 15578 RVA: 0x003425D9 File Offset: 0x003407D9
		public static void DecInstanceCount()
		{
			Interlocked.Decrement(ref TCPOutPacket.TotalInstanceCount);
		}

		// Token: 0x06003CDB RID: 15579 RVA: 0x003425E8 File Offset: 0x003407E8
		public static int GetInstanceCount()
		{
			return TCPOutPacket.TotalInstanceCount;
		}

		// Token: 0x04004732 RID: 18226
		private byte[] PacketBytes = null;

		// Token: 0x04004733 RID: 18227
		private ushort _PacketCmdID = 0;

		// Token: 0x04004734 RID: 18228
		private int _PacketDataSize = 0;

		// Token: 0x04004735 RID: 18229
		private MemoryBlock _MemoryBlock = null;

		// Token: 0x04004736 RID: 18230
		private static object CountLock = new object();

		// Token: 0x04004737 RID: 18231
		private static int TotalInstanceCount = 0;
	}
}
