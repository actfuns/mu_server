using System;
using System.Security.Cryptography;
using System.Text;

namespace Server.Tools
{
	// Token: 0x02000220 RID: 544
	public class SHA1Helper
	{
		// Token: 0x06000CD2 RID: 3282 RVA: 0x000A2C88 File Offset: 0x000A0E88
		public static string get_sha1_string(string str)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_sha1_bytes(str);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x000A2CA8 File Offset: 0x000A0EA8
		public static string get_sha1_string(byte[] data)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_sha1_bytes(data);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x000A2CC8 File Offset: 0x000A0EC8
		public static byte[] get_sha1_bytes(string str)
		{
			SHA1 sha = new SHA1Managed();
			byte[] bytes_sha1_in = new UTF8Encoding().GetBytes(str);
			return sha.ComputeHash(bytes_sha1_in);
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x000A2CF8 File Offset: 0x000A0EF8
		public static byte[] get_sha1_bytes(byte[] data)
		{
			SHA1 sha = new SHA1Managed();
			return sha.ComputeHash(data);
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x000A2D1C File Offset: 0x000A0F1C
		public static string get_macksha1_string(string str, string key)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_macsha1_bytes(str, key);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x000A2D3C File Offset: 0x000A0F3C
		public static string get_macsha1_string(byte[] data, string key)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_macsha1_bytes(data, key);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x000A2D5C File Offset: 0x000A0F5C
		public static byte[] get_macsha1_bytes(string str, string key)
		{
			byte[] keyBytes = new UTF8Encoding().GetBytes(key);
			HMACSHA1 hmacsha = new HMACSHA1(keyBytes);
			byte[] bytes_sha1_in = new UTF8Encoding().GetBytes(str);
			return hmacsha.ComputeHash(bytes_sha1_in);
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x000A2D9C File Offset: 0x000A0F9C
		public static byte[] get_macsha1_bytes(byte[] data, string key)
		{
			byte[] keyBytes = new UTF8Encoding().GetBytes(key);
			HMACSHA1 hmacsha = new HMACSHA1(keyBytes);
			return hmacsha.ComputeHash(data);
		}
	}
}
