using System;

namespace Server.Tools
{
	// Token: 0x0200021D RID: 541
	public class MD5Helper
	{
		// Token: 0x06000CC4 RID: 3268 RVA: 0x000A29FC File Offset: 0x000A0BFC
		public static string get_md5_string(string str)
		{
			byte[] bytes_md5_out = MD5Core.GetHash(str);
			return DataHelper.Bytes2HexString(bytes_md5_out);
		}

		// Token: 0x06000CC5 RID: 3269 RVA: 0x000A2A1C File Offset: 0x000A0C1C
		public static string get_md5_string(byte[] data)
		{
			byte[] bytes_md5_out = MD5Core.GetHash(data);
			return DataHelper.Bytes2HexString(bytes_md5_out);
		}

		// Token: 0x06000CC6 RID: 3270 RVA: 0x000A2A3C File Offset: 0x000A0C3C
		public static byte[] get_md5_bytes(string str)
		{
			return MD5Core.GetHash(str);
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x000A2A58 File Offset: 0x000A0C58
		public static byte[] get_md5_bytes(byte[] data)
		{
			return MD5Core.GetHash(data);
		}
	}
}
