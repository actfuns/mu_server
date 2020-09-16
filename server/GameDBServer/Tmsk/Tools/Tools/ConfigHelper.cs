using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Server.Tools;

namespace Tmsk.Tools.Tools
{
	
	public static class ConfigHelper
	{
		
		public static XElement Load(string fileName)
		{
			XElement xml = null;
			if (File.Exists(fileName))
			{
				xml = XElement.Load(fileName);
			}
			return xml;
		}

		
		public static IEnumerable<XElement> GetXElements(XElement xml, string name)
		{
			try
			{
				return xml.DescendantsAndSelf(name);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		
		public static XElement GetXElement(XElement xml, string name)
		{
			try
			{
				return xml.DescendantsAndSelf(name).SingleOrDefault<XElement>();
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		
		public static XElement GetXElement(XElement xml, string name, string attrName, string attrValue)
		{
			try
			{
				return xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		
		public static string GetElementAttributeValue(XElement xml, string name, string attrName, string attrValue, string attribute, string defVal = "")
		{
			string val = defVal;
			try
			{
				XElement node = xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
				if (null != node)
				{
					XAttribute attrib = node.Attribute(attribute);
					if (null != attrib)
					{
						val = attrib.Value;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return val;
		}

		
		public static long GetElementAttributeValueLong(XElement xml, string name, string attrName, string attrValue, string attribute, long defVal = 0L)
		{
			long val = defVal;
			try
			{
				XElement node = xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
				if (null != node)
				{
					XAttribute attrib = node.Attribute(attribute);
					if (null != attrib)
					{
						if (!long.TryParse(attrib.Value, out val))
						{
							val = defVal;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return val;
		}

		
		public static int[] GetElementAttributeValueIntArray(XElement xml, string name, string attrName, string attrValue, string attribute, int[] defArr = null)
		{
			int[] arr = defArr;
			try
			{
				XElement node = xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
				if (null != node)
				{
					XAttribute attrib = node.Attribute(attribute);
					if (null == attrib)
					{
						return defArr;
					}
					arr = ConfigHelper.String2IntArray(attrib.Value, ',');
					if (arr == null)
					{
						return defArr;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return arr;
		}

		
		public static string[] GetElementAttributeValueStrArray(XElement xml, string name, string attrName, string attrValue, string attribute, string[] defArr = null)
		{
			string[] arr = defArr;
			try
			{
				XElement node = xml.DescendantsAndSelf(name).SingleOrDefault((XElement X) => X.Attribute(attrName).Value == attrValue);
				if (null != node)
				{
					XAttribute attrib = node.Attribute(attribute);
					if (null == attrib)
					{
						return defArr;
					}
					arr = attrib.Value.Split(new char[]
					{
						','
					});
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return arr;
		}

		
		public static string GetElementAttributeValue(XElement xml, string attribute, string defVal = "")
		{
			string val = defVal;
			try
			{
				XAttribute attrib = xml.Attribute(attribute);
				if (null != attrib)
				{
					val = attrib.Value;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return val;
		}

		
		public static long GetElementAttributeValueLong(XElement xml, string attribute, long defVal = 0L)
		{
			long val = defVal;
			try
			{
				XAttribute attrib = xml.Attribute(attribute);
				if (null != attrib)
				{
					if (!long.TryParse(attrib.Value, out val))
					{
						val = defVal;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return val;
		}

		
		public static double GetElementAttributeValueDouble(XElement xml, string attribute, double defVal = 0.0)
		{
			double val = defVal;
			try
			{
				XAttribute attrib = xml.Attribute(attribute);
				if (null != attrib)
				{
					if (!double.TryParse(attrib.Value, out val))
					{
						val = defVal;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return val;
		}

		
		public static int[] String2IntArray(string str, char spliter = ',')
		{
			int[] result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				string[] sa = str.Split(new char[]
				{
					spliter
				});
				result = ConfigHelper.StringArray2IntArray(sa, 0, sa.Length);
			}
			return result;
		}

		
		public static List<int> String2IntList(string str, char spliter = ',')
		{
			List<int> list = new List<int>();
			List<int> result;
			if (string.IsNullOrEmpty(str))
			{
				result = list;
			}
			else
			{
				string[] sa = str.Split(new char[]
				{
					spliter
				});
				foreach (string s in sa)
				{
					int v;
					if (int.TryParse(s, out v))
					{
						list.Add(v);
					}
				}
				result = list;
			}
			return result;
		}

		
		private static int[] StringArray2IntArray(string[] sa, int start, int count)
		{
			int[] result2;
			if (sa == null)
			{
				result2 = null;
			}
			else if (start < 0 || start >= sa.Length)
			{
				result2 = null;
			}
			else if (count <= 0)
			{
				result2 = null;
			}
			else if (sa.Length - start < count)
			{
				result2 = null;
			}
			else
			{
				int[] result = new int[count];
				for (int i = 0; i < count; i++)
				{
					string str = sa[start + i].Trim();
					str = (string.IsNullOrEmpty(str) ? "0" : str);
					result[i] = Convert.ToInt32(str);
				}
				result2 = result;
			}
			return result2;
		}

		
		public static bool ParseStrInt2(string str, ref int v1, ref int v2, char splitChar = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				string[] strArray = str.Split(new char[]
				{
					splitChar
				});
				int t;
				int t2;
				if (strArray.Length < 2 || !int.TryParse(strArray[0], out t) || !int.TryParse(strArray[1], out t2))
				{
					result = false;
				}
				else
				{
					v1 = t;
					v2 = t2;
					result = true;
				}
			}
			return result;
		}

		
		public static bool ParseStrInt3(string str, ref int v1, ref int v2, ref int v3, char splitChar = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				string[] strArray = str.Split(new char[]
				{
					splitChar
				});
				int t;
				int t2;
				int t3;
				if (strArray.Length < 3 || !int.TryParse(strArray[0], out t) || !int.TryParse(strArray[1], out t2) || !int.TryParse(strArray[2], out t3))
				{
					result = false;
				}
				else
				{
					v1 = t;
					v2 = t2;
					v3 = t3;
					result = true;
				}
			}
			return result;
		}

		
		public static bool ParserTimeRangeList(List<TimeSpan> list, string str, bool clear = true, char splitChar1 = '|', char splitChar2 = '-')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				if (clear)
				{
					list.Clear();
				}
				string[] rangeStr = str.Split(new char[]
				{
					splitChar1
				});
				foreach (string rangeItem in rangeStr)
				{
					string[] timeStr = rangeItem.Split(new char[]
					{
						splitChar2
					});
					if (timeStr.Length != 2)
					{
						return false;
					}
					TimeSpan time;
					TimeSpan time2;
					if (!TimeSpan.TryParse(timeStr[0], out time) || !TimeSpan.TryParse(timeStr[1], out time2))
					{
						return false;
					}
					list.Add(time);
					list.Add(time2);
				}
				result = (list.Count > 0);
			}
			return result;
		}

		
		public static bool ParserTimeRangeListWithDay(List<TimeSpan> list, string str, bool clear = true, char splitChar1 = '|', char splitChar2 = '-', char splitChar3 = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				if (clear)
				{
					list.Clear();
				}
				string[] rangeStr = str.Split(new char[]
				{
					splitChar1
				});
				foreach (string rangeItem in rangeStr)
				{
					string[] dayAndTimeStr = rangeItem.Split(new char[]
					{
						splitChar3
					});
					if (dayAndTimeStr.Length != 2)
					{
						return false;
					}
					int day;
					if (!int.TryParse(dayAndTimeStr[0], out day))
					{
						return false;
					}
					string[] timeStr = dayAndTimeStr[1].Split(new char[]
					{
						splitChar2
					});
					if (timeStr.Length != 2)
					{
						return false;
					}
					TimeSpan time;
					TimeSpan time2;
					if (!TimeSpan.TryParse(timeStr[0], out time) || !TimeSpan.TryParse(timeStr[1], out time2))
					{
						return false;
					}
					TimeSpan dayPart = new TimeSpan(day, 0, 0, 0);
					time = time.Add(dayPart);
					time2 = time2.Add(dayPart);
					list.Add(time);
					list.Add(time2);
				}
				result = (list.Count > 0);
			}
			return result;
		}

		
		public static List<List<int>> ParserIntArrayList(string str, bool verifyColumn = true, char splitChar1 = '|', char splitChar2 = ',')
		{
			List<List<int>> list = new List<List<int>>();
			List<List<int>> result;
			if (string.IsNullOrEmpty(str))
			{
				result = list;
			}
			else
			{
				string[] rangeStr = str.Split(new char[]
				{
					splitChar1
				});
				int maxColumnCount = -1;
				foreach (string rangeItem in rangeStr)
				{
					List<int> ls = new List<int>();
					if (!string.IsNullOrEmpty(rangeItem))
					{
						string[] arr = rangeItem.Split(new char[]
						{
							splitChar2
						});
						foreach (string s in arr)
						{
							int v;
							if (int.TryParse(s, out v))
							{
								ls.Add(v);
							}
						}
					}
					list.Add(ls);
					if (verifyColumn)
					{
						if (maxColumnCount < 0)
						{
							maxColumnCount = ls.Count;
							if (maxColumnCount == 0)
							{
								break;
							}
						}
						else if (maxColumnCount != ls.Count)
						{
							list.Clear();
							break;
						}
					}
				}
				result = list;
			}
			return result;
		}
	}
}
