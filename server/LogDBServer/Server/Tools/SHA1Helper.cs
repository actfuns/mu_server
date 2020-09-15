using System;
using System.Security.Cryptography;
using System.Text;

namespace Server.Tools
{
	// Token: 0x0200003E RID: 62
	public class SHA1Helper
	{
		// Token: 0x06000175 RID: 373 RVA: 0x00009020 File Offset: 0x00007220
		public static string get_sha1_string(string str)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_sha1_bytes(str);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00009040 File Offset: 0x00007240
		public static string get_sha1_string(byte[] data)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_sha1_bytes(data);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00009060 File Offset: 0x00007260
		public static byte[] get_sha1_bytes(string str)
		{
			SHA1 sha = new SHA1Managed();
			byte[] bytes_sha1_in = new UTF8Encoding().GetBytes(str);
			return sha.ComputeHash(bytes_sha1_in);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00009090 File Offset: 0x00007290
		public static byte[] get_sha1_bytes(byte[] data)
		{
			SHA1 sha = new SHA1Managed();
			return sha.ComputeHash(data);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x000090B4 File Offset: 0x000072B4
		public static string get_macksha1_string(string str, string key)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_macsha1_bytes(str, key);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000090D4 File Offset: 0x000072D4
		public static string get_macsha1_string(byte[] data, string key)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_macsha1_bytes(data, key);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000090F4 File Offset: 0x000072F4
		public static byte[] get_macsha1_bytes(string str, string key)
		{
			byte[] keyBytes = new UTF8Encoding().GetBytes(key);
			HMACSHA1 hmacsha = new HMACSHA1(keyBytes);
			byte[] bytes_sha1_in = new UTF8Encoding().GetBytes(str);
			return hmacsha.ComputeHash(bytes_sha1_in);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00009134 File Offset: 0x00007334
		public static byte[] get_macsha1_bytes(byte[] data, string key)
		{
			byte[] keyBytes = new UTF8Encoding().GetBytes(key);
			HMACSHA1 hmacsha = new HMACSHA1(keyBytes);
			return hmacsha.ComputeHash(data);
		}
	}
}
