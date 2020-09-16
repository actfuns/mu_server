using System;

namespace Server.Tools
{
	
	public class MD5Helper
	{
		
		public static string get_md5_string(string str)
		{
			byte[] bytes_md5_out = MD5Core.GetHash(str);
			return DataHelper.Bytes2HexString(bytes_md5_out);
		}

		
		public static string get_md5_string(byte[] data)
		{
			byte[] bytes_md5_out = MD5Core.GetHash(data);
			return DataHelper.Bytes2HexString(bytes_md5_out);
		}

		
		public static byte[] get_md5_bytes(string str)
		{
			return MD5Core.GetHash(str);
		}

		
		public static byte[] get_md5_bytes(byte[] data)
		{
			return MD5Core.GetHash(data);
		}
	}
}
