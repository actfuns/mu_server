using System;
using System.Text;
using Server.Tools;

namespace Server.Data
{
	// Token: 0x0200017B RID: 379
	public static class SecondPasswordRC4
	{
		// Token: 0x060004A0 RID: 1184 RVA: 0x00040F1C File Offset: 0x0003F11C
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

		// Token: 0x060004A1 RID: 1185 RVA: 0x00040F60 File Offset: 0x0003F160
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

		// Token: 0x04000878 RID: 2168
		private static string _Key = "SecPwd";
	}
}
