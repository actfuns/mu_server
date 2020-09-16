using System;
using System.Text;
using System.Threading;
using GameServer.Logic;
using Server.TCP;
using Server.Tools;

namespace Server.Protocol
{
	
	public class TCPOutPacket : IDisposable
	{
		
		public TCPOutPacket()
		{
			TCPOutPacket.IncInstanceCount();
		}

		
		public byte[] GetPacketBytes()
		{
			return this.PacketBytes;
		}

		
		
		
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

		
		
		public int PacketDataSize
		{
			get
			{
				return this._PacketDataSize + 6;
			}
		}

		
		
		
		public object Tag { get; set; }

		
		
		
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

		
		private void Final()
		{
			int length = this._PacketDataSize + 2;
			DataHelper.CopyBytes(this.PacketBytes, 0, BitConverter.GetBytes(length), 0, 4);
			DataHelper.CopyBytes(this.PacketBytes, 4, BitConverter.GetBytes(this._PacketCmdID), 0, 2);
		}

		
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

		
		public void Dispose()
		{
			TCPOutPacket.DecInstanceCount();
			this.Tag = null;
		}

		
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, string data, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			byte[] bytesCmd = new UTF8Encoding().GetBytes(data);
			tcpOutPacket.FinalWriteData(bytesCmd, 0, bytesCmd.Length);
			return tcpOutPacket;
		}

		
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, byte[] data, int offset, int length, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			tcpOutPacket.FinalWriteData(data, offset, length);
			return tcpOutPacket;
		}

		
		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, byte[] data, int cmd)
		{
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = (ushort)cmd;
			tcpOutPacket.FinalWriteData(data, 0, data.Length);
			return tcpOutPacket;
		}

		
		public static void IncInstanceCount()
		{
			Interlocked.Increment(ref TCPOutPacket.TotalInstanceCount);
		}

		
		public static void DecInstanceCount()
		{
			Interlocked.Decrement(ref TCPOutPacket.TotalInstanceCount);
		}

		
		public static int GetInstanceCount()
		{
			return TCPOutPacket.TotalInstanceCount;
		}

		
		private byte[] PacketBytes = null;

		
		private ushort _PacketCmdID = 0;

		
		private int _PacketDataSize = 0;

		
		private MemoryBlock _MemoryBlock = null;

		
		private static object CountLock = new object();

		
		private static int TotalInstanceCount = 0;
	}
}
