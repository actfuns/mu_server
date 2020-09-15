using System;
using System.Text;

namespace Server.Tools
{
	// Token: 0x0200021F RID: 543
	public class RC4Helper
	{
		// Token: 0x06000CCD RID: 3277 RVA: 0x000A2B2C File Offset: 0x000A0D2C
		public static void RC4(byte[] bytesData, int offset, int count, string key)
		{
			byte[] keyBytes = Encoding.UTF8.GetBytes(key);
			RC4Helper.RC4(bytesData, offset, count, keyBytes);
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x000A2B50 File Offset: 0x000A0D50
		public static void RC4(byte[] bytesData, int offset, int count, byte[] key)
		{
			byte[] s = new byte[256];
			byte[] i = new byte[256];
			int j;
			for (j = 0; j < 256; j++)
			{
				s[j] = (byte)j;
				i[j] = key[j % key.GetLength(0)];
			}
			int k = 0;
			for (j = 0; j < 256; j++)
			{
				k = (k + (int)s[j] + (int)i[j]) % 256;
				byte temp = s[j];
				s[j] = s[k];
				s[k] = temp;
			}
			k = (j = 0);
			for (int x = offset; x < offset + count; x++)
			{
				j = (j + 1) % 256;
				k = (k + (int)s[j]) % 256;
				byte temp = s[j];
				s[j] = s[k];
				s[k] = temp;
				int t = (int)(s[j] + s[k]) % 256;
				int num = x;
				bytesData[num] ^= s[t];
			}
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x000A2C4C File Offset: 0x000A0E4C
		public static void RC4(byte[] bytesData, byte[] key)
		{
			RC4Helper.RC4(bytesData, 0, bytesData.Length, key);
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x000A2C5C File Offset: 0x000A0E5C
		public static void RC4(byte[] bytesData, string key)
		{
			byte[] keyBytes = Encoding.UTF8.GetBytes(key);
			RC4Helper.RC4(bytesData, keyBytes);
		}
	}
}
