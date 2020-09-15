using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Server.Tools
{
	// Token: 0x02000214 RID: 532
	internal class AesHelper
	{
		// Token: 0x06000C6B RID: 3179 RVA: 0x0009FDD8 File Offset: 0x0009DFD8
		public static byte[] AesEncryptBytes(byte[] data, string passwd, string saltValue)
		{
			byte[] saltBytes = Encoding.UTF8.GetBytes(saltValue);
			AesManaged aes = new AesManaged();
			Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(passwd, saltBytes);
			aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
			aes.KeySize = aes.LegalKeySizes[0].MaxSize;
			aes.Key = rfc.GetBytes(aes.KeySize / 8);
			aes.IV = rfc.GetBytes(aes.BlockSize / 8);
			ICryptoTransform encryptTransform = aes.CreateEncryptor();
			MemoryStream encryptStream = new MemoryStream();
			CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransform, CryptoStreamMode.Write);
			encryptor.Write(data, 0, data.Length);
			encryptor.Close();
			return encryptStream.ToArray();
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x0009FE90 File Offset: 0x0009E090
		public static byte[] AesDecryptBytes(byte[] encryptData, string passwd, string saltValue)
		{
			byte[] saltBytes = Encoding.UTF8.GetBytes(saltValue);
			AesManaged aes = new AesManaged();
			Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(passwd, saltBytes);
			aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
			aes.KeySize = aes.LegalKeySizes[0].MaxSize;
			aes.Key = rfc.GetBytes(aes.KeySize / 8);
			aes.IV = rfc.GetBytes(aes.BlockSize / 8);
			ICryptoTransform decryptTransform = aes.CreateDecryptor();
			MemoryStream decryptStream = new MemoryStream();
			CryptoStream decryptor = new CryptoStream(decryptStream, decryptTransform, CryptoStreamMode.Write);
			try
			{
				decryptor.Write(encryptData, 0, encryptData.Length);
				decryptor.Close();
			}
			catch (Exception)
			{
				return null;
			}
			return decryptStream.ToArray();
		}
	}
}
