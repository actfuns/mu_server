using System;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020007DF RID: 2015
	public class SystemXmlItem
	{
		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x060038F8 RID: 14584 RVA: 0x00307510 File Offset: 0x00305710
		// (set) Token: 0x060038F9 RID: 14585 RVA: 0x00307527 File Offset: 0x00305727
		public XElement XMLNode { get; set; }

		// Token: 0x060038FA RID: 14586 RVA: 0x00307530 File Offset: 0x00305730
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

		// Token: 0x060038FB RID: 14587 RVA: 0x0030759C File Offset: 0x0030579C
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

		// Token: 0x060038FC RID: 14588 RVA: 0x00307628 File Offset: 0x00305828
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

		// Token: 0x060038FD RID: 14589 RVA: 0x003076B4 File Offset: 0x003058B4
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

		// Token: 0x060038FE RID: 14590 RVA: 0x00307748 File Offset: 0x00305948
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

		// Token: 0x060038FF RID: 14591 RVA: 0x00307824 File Offset: 0x00305A24
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
