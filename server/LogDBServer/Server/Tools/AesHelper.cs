using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Server.Tools
{
	// Token: 0x02000038 RID: 56
	internal class AesHelper
	{
		// Token: 0x0600013F RID: 319 RVA: 0x00007C80 File Offset: 0x00005E80
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

		// Token: 0x06000140 RID: 320 RVA: 0x00007D38 File Offset: 0x00005F38
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
