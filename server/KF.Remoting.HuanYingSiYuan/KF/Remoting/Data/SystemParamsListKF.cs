using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tmsk.Tools.Tools;

namespace KF.Remoting.Data
{
	// Token: 0x0200001A RID: 26
	public class SystemParamsListKF
	{
		// Token: 0x060000C4 RID: 196 RVA: 0x0000A898 File Offset: 0x00008A98
		public string GetParamValueByName(string name)
		{
			string result;
			if (null == this._ParamsDict)
			{
				result = "";
			}
			else
			{
				string value = null;
				Dictionary<string, string> paramsDict = this._ParamsDict;
				paramsDict.TryGetValue(name, out value);
				result = value;
			}
			return result;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000A8D8 File Offset: 0x00008AD8
		public long GetParamValueIntByName(string name, int defvalue = -1)
		{
			string value = this.GetParamValueByName(name);
			long result;
			if (string.IsNullOrEmpty(value))
			{
				result = (long)defvalue;
			}
			else
			{
				try
				{
					return Convert.ToInt64(value);
				}
				catch (Exception)
				{
				}
				result = (long)defvalue;
			}
			return result;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000A928 File Offset: 0x00008B28
		public long GetParamValueIntByName(string name, long defvalue)
		{
			string value = this.GetParamValueByName(name);
			long result;
			if (string.IsNullOrEmpty(value))
			{
				result = defvalue;
			}
			else
			{
				try
				{
					return Convert.ToInt64(value);
				}
				catch (Exception)
				{
				}
				result = defvalue;
			}
			return result;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000A978 File Offset: 0x00008B78
		public int[] GetParamValueIntArrayByName(string name)
		{
			string value = this.GetParamValueByName(name);
			int[] result;
			if (string.IsNullOrEmpty(value))
			{
				result = null;
			}
			else
			{
				try
				{
					return ConfigHelper.String2IntArray(value, ',');
				}
				catch (Exception)
				{
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000A9C8 File Offset: 0x00008BC8
		public double GetParamValueDoubleByName(string name)
		{
			string value = this.GetParamValueByName(name);
			double result;
			if (string.IsNullOrEmpty(value))
			{
				result = 0.0;
			}
			else
			{
				try
				{
					return Convert.ToDouble(value);
				}
				catch (Exception)
				{
				}
				result = 0.0;
			}
			return result;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000AA28 File Offset: 0x00008C28
		public void LoadParamsList()
		{
			string fileName = string.Format("Config/SystemParams.xml", new object[0]);
			XElement xml = XElement.Load(KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes));
			if (null == xml)
			{
				throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			IEnumerable<XElement> paramNodes = xml.Elements("Params").Elements<XElement>();
			if (null != paramNodes)
			{
				Dictionary<string, string> paramsDict = new Dictionary<string, string>();
				foreach (XElement xmlNode in paramNodes)
				{
					string paramName = ConfigHelper.GetElementAttributeValue(xmlNode, "Name", "");
					string paramValue = ConfigHelper.GetElementAttributeValue(xmlNode, "Value", "");
					paramsDict[paramName] = paramValue;
				}
				this._ParamsDict = paramsDict;
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000AB20 File Offset: 0x00008D20
		public int ReloadLoadParamsList()
		{
			try
			{
				this.LoadParamsList();
			}
			catch (Exception)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x040000A5 RID: 165
		private Dictionary<string, string> _ParamsDict = null;
	}
}
