using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Maticsoft.DBUtility
{
	
	public class DESEncrypt
	{
		
		public static string Encrypt(string Text)
		{
			return DESEncrypt.Encrypt(Text, "litianping");
		}

		
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

		
		public static string Decrypt(string Text)
		{
			return DESEncrypt.Decrypt(Text, "litianping");
		}

		
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
