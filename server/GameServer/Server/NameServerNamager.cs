using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using GameServer.Logic;
using Server.Tools;

namespace GameServer.Server
{
	// Token: 0x020008B3 RID: 2227
	public static class NameServerNamager
	{
		// Token: 0x06003DB9 RID: 15801 RVA: 0x0034BF60 File Offset: 0x0034A160
		public static void Init(XElement xml)
		{
			Global.Flag_NameServer = false;
		}

		// Token: 0x06003DBA RID: 15802 RVA: 0x0034BF7C File Offset: 0x0034A17C
		public static string GetIPV4IP(NameServerData serverData)
		{
			try
			{
				IPAddress ip;
				if (IPAddress.TryParse(serverData.Host, out ip))
				{
					return ip.ToString();
				}
				IPHostEntry hostEntry = Dns.GetHostEntry(serverData.Host);
				if (hostEntry.AddressList.Length >= 0)
				{
					for (int i = 0; i < hostEntry.AddressList.Length; i++)
					{
						if (hostEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
						{
							serverData.ResolvedHost = serverData.Host;
							serverData.ResolvedIP = hostEntry.AddressList[i].ToString();
							return serverData.ResolvedIP;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException("解析名字服务器域名异常: " + ex.ToString());
			}
			string result;
			if (serverData.ResolvedHost == serverData.Host)
			{
				result = serverData.ResolvedIP;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06003DBB RID: 15803 RVA: 0x0034C088 File Offset: 0x0034A288
		public static int CheckInvalidCharacters(string param, bool use_utf8mb4 = false)
		{
			int result;
			if (string.IsNullOrEmpty(param))
			{
				result = -40;
			}
			else if (param.IndexOfAny(NameServerNamager.InvalidCharacters) >= 0)
			{
				result = -40;
			}
			else
			{
				for (int i = 0; i < NameServerNamager.InvalidSqlStrings.Length; i++)
				{
					if (param.IndexOf(NameServerNamager.InvalidSqlStrings[i]) >= 0)
					{
						return -40;
					}
				}
				if (!use_utf8mb4 && GameManager.PlatConfigMgr.GetGameConfigItemStr("use_utf8mb4", "0") != "1")
				{
					if (NameServerNamager.Check_utf8mb4(param))
					{
						return -40;
					}
				}
				result = 1;
			}
			return result;
		}

		// Token: 0x06003DBC RID: 15804 RVA: 0x0034C13C File Offset: 0x0034A33C
		public static bool Check_utf8mb4(string param)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(param);
			int i = 0;
			while (i < bytes.Length)
			{
				int bs = 0;
				BitArray ba = new BitArray(new byte[]
				{
					bytes[i]
				});
				for (int j = 7; j >= 0; j--)
				{
					if (!ba[j])
					{
						bs = 7 - j;
						break;
					}
				}
				if (bs == 0)
				{
					i++;
				}
				else
				{
					i += bs;
					if (bs >= 4)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003DBD RID: 15805 RVA: 0x0034C1E4 File Offset: 0x0034A3E4
		public static string EscapeName(string name)
		{
			for (int i = 0; i < NameServerNamager.ReplaceCharacters.Length; i++)
			{
				name = name.Replace(NameServerNamager.ReplaceCharacters[i], NameServerNamager.ReplaceToCharacters[i]);
			}
			return name;
		}

		// Token: 0x06003DBE RID: 15806 RVA: 0x0034C228 File Offset: 0x0034A428
		public static int RegisterNameToNameServer(int zoneID, string userID, string[] nameAndPingTaiID, int type, int roleID = 0)
		{
			bool flag = 1 == 0;
			return 1;
		}

		// Token: 0x040047C8 RID: 18376
		public static string NameServerIP;

		// Token: 0x040047C9 RID: 18377
		public static int NameServerPort;

		// Token: 0x040047CA RID: 18378
		public static int NameServerConfig;

		// Token: 0x040047CB RID: 18379
		public static string ServerPingTaiID;

		// Token: 0x040047CC RID: 18380
		public static string PingTaiID;

		// Token: 0x040047CD RID: 18381
		private static char[] InvalidCharacters = new char[]
		{
			'<',
			'>',
			'\\',
			'\'',
			'"',
			'=',
			'%',
			'\t',
			'\b',
			'\r',
			'\n',
			'○',
			'●',
			'|',
			'$',
			'{',
			'}',
			'&',
			'?',
			' ',
			':'
		};

		// Token: 0x040047CE RID: 18382
		private static char[] ReplaceCharacters = new char[]
		{
			'\t',
			'\b',
			'\r',
			'\n',
			'|',
			' ',
			':'
		};

		// Token: 0x040047CF RID: 18383
		private static char[] ReplaceToCharacters = new char[]
		{
			'\u3000',
			'\u3000',
			'\u3000',
			'\u3000',
			'｜',
			'\u3000',
			'：'
		};

		// Token: 0x040047D0 RID: 18384
		private static string[] InvalidSqlStrings = new string[]
		{
			"--"
		};

		// Token: 0x040047D1 RID: 18385
		private static NameServerData DefaultServerData = new NameServerData();

		// Token: 0x040047D2 RID: 18386
		private static Dictionary<RangeKey, NameServerData> ZoneID2NameServerDict = new Dictionary<RangeKey, NameServerData>(RangeKey.Comparer);

		// Token: 0x020008B4 RID: 2228
		public static class NameErrorCodes
		{
			// Token: 0x040047D3 RID: 18387
			public const int ErrorSuccess = 1;

			// Token: 0x040047D4 RID: 18388
			public const int ErrorServerDisabled = -2;

			// Token: 0x040047D5 RID: 18389
			public const int ErrorInvalidCharacter = -3;

			// Token: 0x040047D6 RID: 18390
			public const int ErrorNameHasBeUsed = -4;
		}
	}
}
