using System;
using System.Security.Cryptography;
using System.Text;

namespace Server.Tools
{
	
	public class SHA1Helper
	{
		
		public static string get_sha1_string(string str)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_sha1_bytes(str);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		
		public static string get_sha1_string(byte[] data)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_sha1_bytes(data);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		
		public static byte[] get_sha1_bytes(string str)
		{
			SHA1 sha = new SHA1Managed();
			byte[] bytes_sha1_in = new UTF8Encoding().GetBytes(str);
			return sha.ComputeHash(bytes_sha1_in);
		}

		
		public static byte[] get_sha1_bytes(byte[] data)
		{
			SHA1 sha = new SHA1Managed();
			return sha.ComputeHash(data);
		}

		
		public static string get_macksha1_string(string str, string key)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_macsha1_bytes(str, key);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		
		public static string get_macsha1_string(byte[] data, string key)
		{
			byte[] bytes_sha1_out = SHA1Helper.get_macsha1_bytes(data, key);
			return DataHelper.Bytes2HexString(bytes_sha1_out);
		}

		
		public static byte[] get_macsha1_bytes(string str, string key)
		{
			byte[] keyBytes = new UTF8Encoding().GetBytes(key);
			HMACSHA1 hmacsha = new HMACSHA1(keyBytes);
			byte[] bytes_sha1_in = new UTF8Encoding().GetBytes(str);
			return hmacsha.ComputeHash(bytes_sha1_in);
		}

		
		public static byte[] get_macsha1_bytes(byte[] data, string key)
		{
			byte[] keyBytes = new UTF8Encoding().GetBytes(key);
			HMACSHA1 hmacsha = new HMACSHA1(keyBytes);
			return hmacsha.ComputeHash(data);
		}
	}
}
