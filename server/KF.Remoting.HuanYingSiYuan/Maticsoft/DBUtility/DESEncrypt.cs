using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Maticsoft.DBUtility
{
	// Token: 0x0200004E RID: 78
	public class DESEncrypt
	{
		// Token: 0x0600038B RID: 907 RVA: 0x0002FDC0 File Offset: 0x0002DFC0
		public static string Encrypt(string Text)
		{
			return DESEncrypt.Encrypt(Text, "litianping");
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0002FDE0 File Offset: 0x0002DFE0
		public static string Encrypt(string Text, string sKey)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			byte[] inputByteArray = Encoding.Default.GetBytes(Text);
			des.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
			des.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
			cs.Write(inputByteArray, 0, inputByteArray.Length);
			cs.FlushFinalBlock();
			StringBuilder ret = new StringBuilder();
			foreach (byte b in ms.ToArray())
			{
				ret.AppendFormat("{0:X2}", b);
			}
			return ret.ToString();
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0002FEC0 File Offset: 0x0002E0C0
		public static string Decrypt(string Text)
		{
			return DESEncrypt.Decrypt(Text, "litianping");
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0002FEE0 File Offset: 0x0002E0E0
		public static string Decrypt(string Text, string sKey)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			int len = Text.Length / 2;
			byte[] inputByteArray = new byte[len];
			for (int x = 0; x < len; x++)
			{
				int i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
				inputByteArray[x] = (byte)i;
			}
			des.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
			des.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
			cs.Write(inputByteArray, 0, inputByteArray.Length);
			cs.FlushFinalBlock();
			return Encoding.Default.GetString(ms.ToArray());
		}
	}
}
