using System;

namespace Server.Tools
{
	
	public class CRC32
	{
		
		private static uint[] makeCrcTable()
		{
			uint[] crcTable = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint c = (uint)i;
				int j = 8;
				while (--j >= 0)
				{
					if ((c & 1U) != 0U)
					{
						c = (3988292384U ^ c >> 1);
					}
					else
					{
						c >>= 1;
					}
				}
				crcTable[i] = c;
			}
			return crcTable;
		}

		
		public uint getValue()
		{
			return this.crc & uint.MaxValue;
		}

		
		public void reset()
		{
			this.crc = 0U;
		}

		
		public void update(byte[] buf)
		{
			uint off = 0U;
			int len = buf.Length;
			uint c = ~this.crc;
			while (--len >= 0)
			{
				c = (CRC32.crcTable[(int)((UIntPtr)((c ^ (uint)buf[(int)((UIntPtr)(off++))]) & 255U))] ^ c >> 8);
			}
			this.crc = ~c;
		}

		
		public void update(byte[] buf, int off, int len)
		{
			uint c = ~this.crc;
			while (--len >= 0)
			{
				c = (CRC32.crcTable[(int)((UIntPtr)((c ^ (uint)buf[off++]) & 255U))] ^ c >> 8);
			}
			this.crc = ~c;
		}

		
		private uint crc = 0U;

		
		private static uint[] crcTable = CRC32.makeCrcTable();
	}
}
