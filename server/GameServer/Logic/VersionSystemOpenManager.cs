using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class VersionSystemOpenManager
	{
		
		public void LoadVersionSystemOpenData()
		{
			lock (this._VersionSystemOpenMutex)
			{
				string fileName = "Config/VersionSystemOpen.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件异常", fileName), null, true);
				}
				else
				{
					IEnumerable<XElement> xmlItems = xml.Elements();
					this.VersionSystemOpenDict.Clear();
					this.SystemOpenDict.Clear();
					foreach (XElement xmlItem in xmlItems)
					{
						string key = Global.GetSafeAttributeStr(xmlItem, "SystemName");
						int nValue = (int)Global.GetSafeAttributeLong(xmlItem, "IsOpen");
						this.VersionSystemOpenDict[key] = nValue;
						int gongNengId = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						if (gongNengId >= 100000)
						{
							if (gongNengId >= 100000 && gongNengId < 120000)
							{
								this.SystemOpenDict[gongNengId - 100000] = (nValue > 0);
							}
							else
							{
								this.SystemOpenDict[gongNengId] = (nValue > 0);
							}
						}
					}
				}
			}
		}

		
		public bool IsVersionSystemOpen(string key)
		{
			int nValue = 0;
			bool bRes = false;
			lock (this._VersionSystemOpenMutex)
			{
				if (this.VersionSystemOpenDict.TryGetValue(key, out nValue))
				{
					bRes = (nValue == 1);
				}
			}
			return bRes;
		}

		
		public bool IsVersionSystemOpen(int id)
		{
			bool bRes = false;
			lock (this._VersionSystemOpenMutex)
			{
				if (!this.SystemOpenDict.TryGetValue(id, out bRes))
				{
					return true;
				}
			}
			return bRes;
		}

		
		private object _VersionSystemOpenMutex = new object();

		
		private Dictionary<string, int> VersionSystemOpenDict = new Dictionary<string, int>();

		
		private Dictionary<int, bool> SystemOpenDict = new Dictionary<int, bool>();
	}
}
