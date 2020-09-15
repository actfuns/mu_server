using System;

namespace Server.Tools
{
	// Token: 0x02000217 RID: 535
	public class CRC32
	{
		// Token: 0x06000C84 RID: 3204 RVA: 0x000A0D3C File Offset: 0x0009EF3C
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

		// Token: 0x06000C85 RID: 3205 RVA: 0x000A0DB0 File Offset: 0x0009EFB0
		public uint getValue()
		{
			return this.crc & uint.MaxValue;
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x000A0DCA File Offset: 0x0009EFCA
		public void reset()
		{
			this.crc = 0U;
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x000A0DD4 File Offset: 0x0009EFD4
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

		// Token: 0x06000C88 RID: 3208 RVA: 0x000A0E28 File Offset: 0x0009F028
		public void update(byte[] buf, int off, int len)
		{
			uint c = ~this.crc;
			while (--len >= 0)
			{
				c = (CRC32.crcTable[(int)((UIntPtr)((c ^ (uint)buf[off++]) & 255U))] ^ c >> 8);
			}
			this.crc = ~c;
		}

		// Token: 0x04001228 RID: 4648
		private uint crc = 0U;

		// Token: 0x04001229 RID: 4649
		private static uint[] crcTable = CRC32.makeCrcTable();
	}
}
