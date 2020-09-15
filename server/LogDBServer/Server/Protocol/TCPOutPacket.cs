using System;
using System.Text;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x02000023 RID: 35
	public class TCPOutPacket : IDisposable
	{
		// Token: 0x060000BD RID: 189 RVA: 0x00005CE8 File Offset: 0x00003EE8
		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00005D00 File Offset: 0x00003F00
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00005D18 File Offset: 0x00003F18
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00005D24 File Offset: 0x00003F24
		public int PacketDataSize
		{
			get
			{
				return this._PacketDataSize + 6;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00005D40 File Offset: 0x00003F40
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00005D57 File Offset: 0x00003F57
		public object Tag { get; set; }

		// Token: 0x060000C3 RID: 195 RVA: 0x00005D60 File Offset: 0x00003F60
		public bool FinalWriteData(byte[] buffer, int offset, int count)
		{
			bool result;
			if (null != this.PacketBytes)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("TCP发出命令包不能被重复写入数据, 命令ID: {0}", this.PacketCmdID));
				result = false;
			}
			else if (6 + count >= 131072)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("TCP命令包长度:{0}, 最大不能超过: {1}, 命令ID: {2}", count, 131072, this.PacketCmdID));
				result = false;
			}
			else
			{
				this.PacketBytes = new byte[count + 6];
				int offsetTo = 6;
				DataHelper.CopyBytes(this.PacketBytes, offsetTo, buffer, offset, count);
				this._PacketDataSize = count;
				this.Final();
				result = true;
			}
			return result;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00005E10 File Offset: 0x00004010
		private void Final()
		{
			int length = this._PacketDataSize + 2;
			DataHelper.CopyBytes(this.PacketBytes, 0, BitConverter.GetBytes(length), 0, 4);
			DataHelper.CopyBytes(this.PacketBytes, 4, BitConverter.GetBytes(this._PacketCmdID), 0, 2);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00005E56 File Offset: 0x00004056
		public void Reset()
		{
			this.PacketBytes = null;
			this.PacketCmdID = 0;
			this._PacketDataSize = 0;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00005E6F File Offset: 0x0000406F
		public void Dispose()
		{
			this.Tag = null;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00005E7C File Offset: 0x0000407C
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, string data, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			byte[] bytesCmd = new UTF8Encoding().GetBytes(data);
			tcpOutPacket.FinalWriteData(bytesCmd, 0, bytesCmd.Length);
			return tcpOutPacket;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00005EB8 File Offset: 0x000040B8
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, byte[] data, int offset, int length, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			tcpOutPacket.FinalWriteData(data, offset, length);
			return tcpOutPacket;
		}

		// Token: 0x04000059 RID: 89
		private byte[] PacketBytes = null;

		// Token: 0x0400005A RID: 90
		private ushort _PacketCmdID = 0;

		// Token: 0x0400005B RID: 91
		private int _PacketDataSize = 0;
	}
}
