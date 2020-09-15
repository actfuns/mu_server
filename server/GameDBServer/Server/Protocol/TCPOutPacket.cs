using System;
using System.Text;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x020001E5 RID: 485
	public class TCPOutPacket : IDisposable
	{
		// Token: 0x06000A1D RID: 2589 RVA: 0x0006123C File Offset: 0x0005F43C
		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000A1E RID: 2590 RVA: 0x00061254 File Offset: 0x0005F454
		// (set) Token: 0x06000A1F RID: 2591 RVA: 0x0006126C File Offset: 0x0005F46C
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

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000A20 RID: 2592 RVA: 0x00061278 File Offset: 0x0005F478
		public int PacketDataSize
		{
			get
			{
				return this._PacketDataSize + 6;
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000A21 RID: 2593 RVA: 0x00061294 File Offset: 0x0005F494
		// (set) Token: 0x06000A22 RID: 2594 RVA: 0x000612AB File Offset: 0x0005F4AB
		public object Tag { get; set; }

		// Token: 0x06000A23 RID: 2595 RVA: 0x000612B4 File Offset: 0x0005F4B4
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
				this.PacketBytes = new byte[count + 6];
				int offsetTo = 6;
				DataHelper.CopyBytes(this.PacketBytes, offsetTo, buffer, offset, count);
				this._PacketDataSize = count;
				this.Final();
				result = true;
			}
			return result;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x000613A8 File Offset: 0x0005F5A8
		private void Final()
		{
			int length = this._PacketDataSize + 2;
			DataHelper.CopyBytes(this.PacketBytes, 0, BitConverter.GetBytes(length), 0, 4);
			DataHelper.CopyBytes(this.PacketBytes, 4, BitConverter.GetBytes(this._PacketCmdID), 0, 2);
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x000613EE File Offset: 0x0005F5EE
		public void Reset()
		{
			this.PacketBytes = null;
			this.PacketCmdID = 0;
			this._PacketDataSize = 0;
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00061407 File Offset: 0x0005F607
		public void Dispose()
		{
			this.Tag = null;
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00061414 File Offset: 0x0005F614
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, string data, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			byte[] bytesCmd = new UTF8Encoding().GetBytes(data);
			tcpOutPacket.FinalWriteData(bytesCmd, 0, bytesCmd.Length);
			return tcpOutPacket;
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00061450 File Offset: 0x0005F650
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, byte[] data, int offset, int length, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			tcpOutPacket.FinalWriteData(data, offset, length);
			return tcpOutPacket;
		}

		// Token: 0x04000C40 RID: 3136
		private byte[] PacketBytes = null;

		// Token: 0x04000C41 RID: 3137
		private ushort _PacketCmdID = 0;

		// Token: 0x04000C42 RID: 3138
		private int _PacketDataSize = 0;
	}
}
