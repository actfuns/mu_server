using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nhiredis;

namespace GameDBServer.Logic
{
	
	public static class CityNameManager
	{
		
		public static string unicode_js_1(string str)
		{
			Regex reg = new Regex("(?i)\\\\u([0-9a-f]{4})");
			return reg.Replace(str, (Match m1) => ((char)Convert.ToInt32(m1.Groups[1].Value, 16)).ToString());
		}

		
		private static void ParseIPInfo(string text, out string region, out string cityName)
		{
			region = "";
			cityName = "";
			if (!string.IsNullOrEmpty(text))
			{
				int startIndex = text.IndexOf("\"region\":\"");
				if (startIndex >= 0)
				{
					int endIndex = text.IndexOf("\",", startIndex + 10);
					if (endIndex >= startIndex)
					{
						region = text.Substring(startIndex + 10, endIndex - startIndex - 10);
						startIndex = text.IndexOf("\"city\":\"");
						if (startIndex >= 0)
						{
							endIndex = text.IndexOf("\",", startIndex + 8);
							if (endIndex >= startIndex)
							{
								cityName = text.Substring(startIndex + 8, endIndex - startIndex - 8);
							}
						}
					}
				}
			}
		}

		
		public static IPInfo ParseIP(string ip)
		{
			string country = null;
			string area = null;
			IPInfo info = null;
			if (IpLibrary.findIpAddrInfo(ip, out country, out area))
			{
				info = new IPInfo();
				info.RegionName = country.Substring(0, Math.Min(20, country.Length));
				info.CityName = area.Substring(0, Math.Min(20, area.Length));
			}
			return info;
		}

		
		public static Dictionary<string, IPInfo> CachingIPInfoDict = new Dictionary<string, IPInfo>();
	}
}
