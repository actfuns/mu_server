using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Linq;

namespace LogDBServer.Logic
{
	
	internal class Global
	{
		
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

		
		public static XElement GetXElement(XElement XML, string newroot)
		{
			return XML.DescendantsAndSelf(newroot).Single<XElement>();
		}

		
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

		
		public static XElement GetXElement(XElement XML, string newroot, string attribute, string value)
		{
			return XML.DescendantsAndSelf(newroot).Single((XElement X) => X.Attribute(attribute).Value == value);
		}

		
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

		
		public static string GetSafeAttributeStr(XElement XML, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, attribute);
			return (string)attrib;
		}

		
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

		
		public static string GetSafeAttributeStr(XElement XML, string root, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, root, attribute);
			return (string)attrib;
		}

		
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

		
		public static int GetBitValue(int whichOne)
		{
			return (int)Math.Pow(2.0, (double)(whichOne - 1));
		}

		
		public static DateTime GetAddDaysDataTime(DateTime dateTime, int addDays, bool roundDay = true)
		{
			if (roundDay)
			{
				dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
			}
			return new DateTime(dateTime.Ticks + (long)addDays * 10000L * 1000L * 24L * 60L * 60L);
		}

		
		public static double GetOffsetSecond(DateTime date)
		{
			double temp = (date - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return temp / 1000.0;
		}

		
		public static int GetOffsetDay(DateTime now)
		{
			double temp = (now - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return (int)(temp / 1000.0 / 60.0 / 60.0 / 24.0);
		}

		
		public static int GetOffsetDayNow()
		{
			return Global.GetOffsetDay(DateTime.Now);
		}

		
		public static DateTime GetRealDate(int day)
		{
			DateTime startDay = DateTime.Parse("2011-11-11");
			return Global.GetAddDaysDataTime(startDay, day, true);
		}

		
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

		
		private static Dictionary<string, string> LangDict = null;
	}
}
