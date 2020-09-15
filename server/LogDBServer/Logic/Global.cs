using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Linq;

namespace LogDBServer.Logic
{
	// Token: 0x0200001A RID: 26
	internal class Global
	{
		// Token: 0x06000083 RID: 131 RVA: 0x000048E0 File Offset: 0x00002AE0
		public static string GetXElementNodePath(XElement element)
		{
			string result;
			try
			{
				string path = element.Name.ToString();
				element = element.Parent;
				while (null != element)
				{
					path = element.Name.ToString() + "/" + path;
					element = element.Parent;
				}
				result = path;
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00004954 File Offset: 0x00002B54
		public static XElement GetXElement(XElement XML, string newroot)
		{
			return XML.DescendantsAndSelf(newroot).Single<XElement>();
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00004978 File Offset: 0x00002B78
		public static XElement GetSafeXElement(XElement XML, string newroot)
		{
			XElement result;
			try
			{
				result = XML.DescendantsAndSelf(newroot).Single<XElement>();
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取: {0} 失败, xml节点名: {1}", newroot, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004A04 File Offset: 0x00002C04
		public static XElement GetXElement(XElement XML, string newroot, string attribute, string value)
		{
			return XML.DescendantsAndSelf(newroot).Single((XElement X) => X.Attribute(attribute).Value == value);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004A84 File Offset: 0x00002C84
		public static XElement GetSafeXElement(XElement XML, string newroot, string attribute, string value)
		{
			XElement result;
			try
			{
				result = XML.DescendantsAndSelf(newroot).Single((XElement X) => X.Attribute(attribute).Value == value);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取: {0}/{1}={2} 失败, xml节点名: {3}", new object[]
				{
					newroot,
					attribute,
					value,
					Global.GetXElementNodePath(XML)
				}));
			}
			return result;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004B1C File Offset: 0x00002D1C
		public static XAttribute GetSafeAttribute(XElement XML, string attribute)
		{
			XAttribute result;
			try
			{
				XAttribute attrib = XML.Attribute(attribute);
				if (null == attrib)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
				result = attrib;
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004B8C File Offset: 0x00002D8C
		public static string GetSafeAttributeStr(XElement XML, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, attribute);
			return (string)attrib;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004BAC File Offset: 0x00002DAC
		public static long GetSafeAttributeLong(XElement XML, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, attribute);
			string str = (string)attrib;
			long result;
			if (str == null || str == "")
			{
				result = -1L;
			}
			else
			{
				try
				{
					result = (long)Convert.ToDouble(str);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004C20 File Offset: 0x00002E20
		public static double GetSafeAttributeDouble(XElement XML, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, attribute);
			string str = (string)attrib;
			double result;
			if (str == null || str == "")
			{
				result = 0.0;
			}
			else
			{
				try
				{
					result = Convert.ToDouble(str);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004C98 File Offset: 0x00002E98
		public static XAttribute GetSafeAttribute(XElement XML, string root, string attribute)
		{
			XAttribute result;
			try
			{
				XAttribute attrib = XML.Element(root).Attribute(attribute);
				if (null == attrib)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
				result = attrib;
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004D14 File Offset: 0x00002F14
		public static string GetSafeAttributeStr(XElement XML, string root, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, root, attribute);
			return (string)attrib;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004D38 File Offset: 0x00002F38
		public static long GetSafeAttributeLong(XElement XML, string root, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, root, attribute);
			string str = (string)attrib;
			long result;
			if (str == null || str == "")
			{
				result = -1L;
			}
			else
			{
				try
				{
					result = (long)Convert.ToDouble(str);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004DAC File Offset: 0x00002FAC
		public static double GetSafeAttributeDouble(XElement XML, string root, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, root, attribute);
			string str = (string)attrib;
			double result;
			if (str == null || str == "")
			{
				result = -1.0;
			}
			else
			{
				try
				{
					result = Convert.ToDouble(str);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004E28 File Offset: 0x00003028
		public static int SafeConvertToInt32(string str)
		{
			str = str.Trim();
			int result;
			if (string.IsNullOrEmpty(str))
			{
				result = 0;
			}
			else
			{
				try
				{
					return Convert.ToInt32(str);
				}
				catch (Exception)
				{
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004E78 File Offset: 0x00003078
		public static void LoadLangDict()
		{
			XElement xml = null;
			try
			{
				xml = XElement.Load("Language.xml");
			}
			catch (Exception)
			{
				return;
			}
			try
			{
				if (null != xml)
				{
					Dictionary<string, string> langDict = new Dictionary<string, string>();
					IEnumerable<XElement> langItems = xml.Elements();
					foreach (XElement langItem in langItems)
					{
						langDict[Global.GetSafeAttributeStr(langItem, "ChineseText")] = Global.GetSafeAttributeStr(langItem, "OtherLangText");
					}
					Global.LangDict = langDict;
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00004F4C File Offset: 0x0000314C
		public static string GetLang(string chineseText)
		{
			string result;
			if (null == Global.LangDict)
			{
				result = chineseText;
			}
			else
			{
				string otherLangText = "";
				if (!Global.LangDict.TryGetValue(chineseText, out otherLangText))
				{
					result = chineseText;
				}
				else if (string.IsNullOrEmpty(otherLangText))
				{
					result = chineseText;
				}
				else
				{
					result = otherLangText;
				}
			}
			return result;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00004FA4 File Offset: 0x000031A4
		public static string GetSocketRemoteEndPoint(Socket s)
		{
			try
			{
				return string.Format("{0} ", s.RemoteEndPoint);
			}
			catch (Exception)
			{
			}
			return "";
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004FE8 File Offset: 0x000031E8
		public static string GetDebugHelperInfo(Socket socket)
		{
			string result;
			if (null == socket)
			{
				result = "socket为null, 无法打印错误信息";
			}
			else
			{
				string ret = "";
				try
				{
					ret += string.Format("IP={0} ", Global.GetSocketRemoteEndPoint(socket));
				}
				catch (Exception)
				{
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005048 File Offset: 0x00003248
		public static int GetBitValue(int whichOne)
		{
			return (int)Math.Pow(2.0, (double)(whichOne - 1));
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005074 File Offset: 0x00003274
		public static DateTime GetAddDaysDataTime(DateTime dateTime, int addDays, bool roundDay = true)
		{
			if (roundDay)
			{
				dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
			}
			return new DateTime(dateTime.Ticks + (long)addDays * 10000L * 1000L * 24L * 60L * 60L);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000050DC File Offset: 0x000032DC
		public static double GetOffsetSecond(DateTime date)
		{
			double temp = (date - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return temp / 1000.0;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005114 File Offset: 0x00003314
		public static int GetOffsetDay(DateTime now)
		{
			double temp = (now - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return (int)(temp / 1000.0 / 60.0 / 60.0 / 24.0);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000516C File Offset: 0x0000336C
		public static int GetOffsetDayNow()
		{
			return Global.GetOffsetDay(DateTime.Now);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005188 File Offset: 0x00003388
		public static DateTime GetRealDate(int day)
		{
			DateTime startDay = DateTime.Parse("2011-11-11");
			return Global.GetAddDaysDataTime(startDay, day, true);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000051B0 File Offset: 0x000033B0
		public static int BeginOfWeek(DateTime date)
		{
			int dayofweek = (int)date.DayOfWeek;
			if (dayofweek == 0)
			{
				dayofweek = 7;
			}
			dayofweek--;
			int currday = Global.GetOffsetDay(date);
			return currday - dayofweek;
		}

		// Token: 0x0400003A RID: 58
		private static Dictionary<string, string> LangDict = null;
	}
}
