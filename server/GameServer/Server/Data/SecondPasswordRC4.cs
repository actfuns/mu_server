using System;
using System.Text;
using Server.Tools;

namespace Server.Data
{
	
	public static class SecondPasswordRC4
	{
		
		public static string Encrypt(string input)
		{
			string result;
			if (string.IsNullOrEmpty(input))
			{
				result = null;
			}
			else
			{
				byte[] b = new UTF8Encoding().GetBytes(input);
				RC4Helper.RC4(b, SecondPasswordRC4._Key);
				result = Convert.ToBase64String(b);
			}
			return result;
		}

		
		public static string Decrypt(string input)
		{
			string result;
			if (string.IsNullOrEmpty(input))
			{
				result = null;
			}
			else
			{
				byte[] b = Convert.FromBase64String(input);
				RC4Helper.RC4(b, SecondPasswordRC4._Key);
				result = new UTF8Encoding().GetString(b);
			}
			return result;
		}

		
		private static string _Key = "SecPwd";
	}
}
