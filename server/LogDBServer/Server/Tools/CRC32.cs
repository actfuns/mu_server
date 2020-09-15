using System;

namespace Server.Tools
{
	// Token: 0x02000039 RID: 57
	public class CRC32
	{
		// Token: 0x06000142 RID: 322 RVA: 0x00007E14 File Offset: 0x00006014
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

		// Token: 0x06000143 RID: 323 RVA: 0x00007E88 File Offset: 0x00006088
		public uint getValue()
		{
			return this.crc & uint.MaxValue;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00007EA2 File Offset: 0x000060A2
		public void reset()
		{
			this.crc = 0U;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00007EAC File Offset: 0x000060AC
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

		// Token: 0x06000146 RID: 326 RVA: 0x00007F00 File Offset: 0x00006100
		public void update(byte[] buf, int off, int len)
		{
			uint c = ~this.crc;
			while (--len >= 0)
			{
				c = (CRC32.crcTable[(int)((UIntPtr)((c ^ (uint)buf[off++]) & 255U))] ^ c >> 8);
			}
			this.crc = ~c;
		}

		// Token: 0x04000428 RID: 1064
		private uint crc = 0U;

		// Token: 0x04000429 RID: 1065
		private static uint[] crcTable = CRC32.makeCrcTable();
	}
}
