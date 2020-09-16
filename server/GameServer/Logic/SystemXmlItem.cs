using System;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class SystemXmlItem
	{
		
		
		
		public XElement XMLNode { get; set; }

		
		public string GetStringValue(string name)
		{
			string ret = "";
			try
			{
				ret = (string)this.XMLNode.Attribute(name);
			}
			catch (Exception)
			{
				string path = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(LogTypes.Warning, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1}", name, path), null, true);
			}
			return ret;
		}

		
		public int GetIntValue(string name, int defaultValue = -1)
		{
			int ret = defaultValue;
			try
			{
				string str = (string)this.XMLNode.Attribute(name);
				if (str != null && str != "")
				{
					ret = (int)Convert.ToDouble(str);
				}
			}
			catch (Exception)
			{
				string path = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(LogTypes.Warning, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1}", name, path), null, true);
			}
			return ret;
		}

		
		public long GetLongValue(string name)
		{
			long ret = -1L;
			try
			{
				string str = (string)this.XMLNode.Attribute(name);
				if (str != null && str != "")
				{
					ret = Convert.ToInt64(str);
				}
			}
			catch (Exception)
			{
				string path = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(LogTypes.Warning, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1}", name, path), null, true);
			}
			return ret;
		}

		
		public double GetDoubleValue(string name)
		{
			double ret = 0.0;
			try
			{
				string str = (string)this.XMLNode.Attribute(name);
				if (str != null && str != "")
				{
					ret = Convert.ToDouble(str);
				}
			}
			catch (Exception)
			{
				string path = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(LogTypes.Warning, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1}", name, path), null, true);
			}
			return ret;
		}

		
		public int[] GetIntArrayValue(string name, char split = ',')
		{
			int[] ret = null;
			try
			{
				string str = (string)this.XMLNode.Attribute(name);
				if (str != null && str != "")
				{
					string[] strArr = str.Split(new char[]
					{
						split
					});
					if (strArr.Length > 0)
					{
						ret = new int[strArr.Length];
						for (int i = 0; i < strArr.Length; i++)
						{
							ret[i] = Convert.ToInt32(strArr[i]);
						}
					}
				}
			}
			catch (Exception)
			{
				string path = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(LogTypes.Warning, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1},采用整形数组返回", name, path), null, true);
			}
			return ret;
		}

		
		public double[] GetDoubleArrayValue(string name, char split = ',')
		{
			double[] ret = null;
			try
			{
				string str = (string)this.XMLNode.Attribute(name);
				if (str != null && str != "")
				{
					string[] strArr = str.Split(new char[]
					{
						split
					});
					if (strArr.Length > 0)
					{
						ret = new double[strArr.Length];
						for (int i = 0; i < strArr.Length; i++)
						{
							ret[i] = Convert.ToDouble(strArr[i]);
						}
					}
				}
			}
			catch (Exception)
			{
				string path = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(LogTypes.Warning, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1},采用整形数组返回", name, path), null, true);
			}
			return ret;
		}
	}
}
