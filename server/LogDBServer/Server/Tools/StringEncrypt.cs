using System;
using System.Text;

namespace Server.Tools
{
	// Token: 0x0200003F RID: 63
	public class StringEncrypt
	{
		// Token: 0x0600017E RID: 382 RVA: 0x0000916C File Offset: 0x0000736C
		public static string Encrypt(string plainText, string passwd, string saltValue)
		{
			string result;
			if (string.IsNullOrEmpty(plainText))
			{
				result = null;
			}
			else
			{
				byte[] bytesData = null;
				try
				{
					bytesData = new UTF8Encoding().GetBytes(plainText);
				}
				catch (Exception)
				{
					return null;
				}
				byte[] bytesResult = null;
				try
				{
					bytesResult = AesHelper.AesEncryptBytes(bytesData, passwd, saltValue);
				}
				catch (Exception)
				{
					return null;
				}
				result = DataHelper.Bytes2HexString(bytesResult);
			}
			return result;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x000091E4 File Offset: 0x000073E4
		public static string Decrypt(string encryptText, string passwd, string saltValue)
		{
			string result;
			if (string.IsNullOrEmpty(encryptText))
			{
				result = null;
			}
			else
			{
				byte[] bytesData = DataHelper.HexString2Bytes(encryptText);
				if (null == bytesData)
				{
					result = null;
				}
				else
				{
					byte[] bytesResult = null;
					try
					{
						bytesResult = AesHelper.AesDecryptBytes(bytesData, passwd, saltValue);
					}
					catch (Exception)
					{
						return null;
					}
					string strResult = null;
					try
					{
						strResult = new UTF8Encoding().GetString(bytesResult, 0, bytesResult.Length);
					}
					catch (Exception)
					{
						return null;
					}
					result = strResult;
				}
			}
			return result;
		}
	}
}
