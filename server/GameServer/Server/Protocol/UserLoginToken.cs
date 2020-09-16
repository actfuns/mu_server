using System;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Tools;

namespace Server.Protocol
{
	
	internal class UserLoginToken
	{
		
		
		
		public string UserID { get; set; }

		
		
		
		public int RandomPwd { get; set; }

		
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

		
		public string GetEncryptString(string keySHA1, string keyData)
		{
			byte[] data = this.GetEncryptBytes(keySHA1, keyData);
			return Convert.ToBase64String(data);
		}

		
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

		
		public const byte NormalTokenHeader = 85;
	}
}
