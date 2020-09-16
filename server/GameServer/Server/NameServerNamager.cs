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
	
	public static class NameServerNamager
	{
		
		public static void Init(XElement xml)
		{
			Global.Flag_NameServer = false;
		}

		
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

		
		public static string EscapeName(string name)
		{
			for (int i = 0; i < NameServerNamager.ReplaceCharacters.Length; i++)
			{
				name = name.Replace(NameServerNamager.ReplaceCharacters[i], NameServerNamager.ReplaceToCharacters[i]);
			}
			return name;
		}

		
		public static int RegisterNameToNameServer(int zoneID, string userID, string[] nameAndPingTaiID, int type, int roleID = 0)
		{
			bool flag = 1 == 0;
			return 1;
		}

		
		public static string NameServerIP;

		
		public static int NameServerPort;

		
		public static int NameServerConfig;

		
		public static string ServerPingTaiID;

		
		public static string PingTaiID;

		
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

		
		private static string[] InvalidSqlStrings = new string[]
		{
			"--"
		};

		
		private static NameServerData DefaultServerData = new NameServerData();

		
		private static Dictionary<RangeKey, NameServerData> ZoneID2NameServerDict = new Dictionary<RangeKey, NameServerData>(RangeKey.Comparer);

		
		public static class NameErrorCodes
		{
			
			public const int ErrorSuccess = 1;

			
			public const int ErrorServerDisabled = -2;

			
			public const int ErrorInvalidCharacter = -3;

			
			public const int ErrorNameHasBeUsed = -4;
		}
	}
}
