using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tmsk.Tools.Tools;

namespace KF.Remoting.Data
{
	
	public class SystemParamsListKF
	{
		
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

		
		private Dictionary<string, string> _ParamsDict = null;
	}
}
