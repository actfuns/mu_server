using System;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x0200086D RID: 2157
	internal class UserLoginToken
	{
		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06003CE6 RID: 15590 RVA: 0x0034274C File Offset: 0x0034094C
		// (set) Token: 0x06003CE7 RID: 15591 RVA: 0x00342763 File Offset: 0x00340963
		public string UserID { get; set; }

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06003CE8 RID: 15592 RVA: 0x0034276C File Offset: 0x0034096C
		// (set) Token: 0x06003CE9 RID: 15593 RVA: 0x00342783 File Offset: 0x00340983
		public int RandomPwd { get; set; }

		// Token: 0x06003CEA RID: 15594 RVA: 0x0034278C File Offset: 0x0034098C
		public byte[] GetEncryptBytes(string keySHA1, string keyData)
		{
			string userToken = string.Format("U:{0}:{1}:{2}:T", this.UserID, this.RandomPwd, TimeUtil.NowRealTime() * 10000L);
			byte[] dataToken = new UTF8Encoding().GetBytes(userToken);
			byte[] macSHA = SHA1Helper.get_macsha1_bytes(dataToken, keySHA1);
			byte[] encryptToken = new byte[macSHA.Length + dataToken.Length];
			DataHelper.CopyBytes(encryptToken, 0, macSHA, 0, macSHA.Length);
			DataHelper.CopyBytes(encryptToken, macSHA.Length, dataToken, 0, dataToken.Length);
			RC4Helper.RC4(encryptToken, keyData);
			return encryptToken;
		}

		// Token: 0x06003CEB RID: 15595 RVA: 0x00342814 File Offset: 0x00340A14
		public int SetEncryptBytes(byte[] buffer, string keySHA1, string keyData, long maxTicks)
		{
			RC4Helper.RC4(buffer, keyData);
			byte[] macSHA = new byte[20];
			DataHelper.CopyBytes(macSHA, 0, buffer, 0, macSHA.Length);
			byte[] dataToken = new byte[buffer.Length - 20];
			DataHelper.CopyBytes(dataToken, 0, buffer, 20, dataToken.Length);
			byte[] verifyMacSHA = SHA1Helper.get_macsha1_bytes(dataToken, keySHA1);
			int result;
			if (!DataHelper.CompBytes(verifyMacSHA, macSHA))
			{
				result = -1;
			}
			else if (dataToken[0] == 85)
			{
				string strToken = new UTF8Encoding().GetString(dataToken);
				string[] parseTokens = strToken.Split(new char[]
				{
					':'
				});
				if (parseTokens.Length != 5)
				{
					result = -2;
				}
				else if (parseTokens[0] != "U" || parseTokens[4] != "T")
				{
					result = -3;
				}
				else
				{
					long ticks = (long)Convert.ToUInt64(parseTokens[3]);
					if (TimeUtil.NowRealTime() * 10000L - ticks >= maxTicks && GameManager.GM_NoCheckTokenTimeRemainMS <= 0L)
					{
						result = -4;
					}
					else
					{
						this.UserID = parseTokens[1];
						this.RandomPwd = (int)Convert.ToUInt32(parseTokens[2]);
						result = 0;
					}
				}
			}
			else
			{
				result = -3;
			}
			return result;
		}

		// Token: 0x06003CEC RID: 15596 RVA: 0x0034295C File Offset: 0x00340B5C
		public string GetEncryptString(string keySHA1, string keyData)
		{
			byte[] data = this.GetEncryptBytes(keySHA1, keyData);
			return Convert.ToBase64String(data);
		}

		// Token: 0x06003CED RID: 15597 RVA: 0x00342980 File Offset: 0x00340B80
		public int SetEncryptString(string s, string keySHA1, string keyData, long maxTicks)
		{
			byte[] buffer = null;
			try
			{
				buffer = Convert.FromBase64String(s);
			}
			catch (FormatException)
			{
				return -1000;
			}
			int result;
			if (null == buffer)
			{
				result = -1001;
			}
			else
			{
				if (buffer.Length < 20)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("SetEncryptString输入数据长度不足#data={0}", s), null, true);
				}
				result = this.SetEncryptBytes(buffer, keySHA1, keyData, maxTicks);
			}
			return result;
		}

		// Token: 0x0400473D RID: 18237
		public const byte NormalTokenHeader = 85;
	}
}
