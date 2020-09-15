using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004BF RID: 1215
	public class VersionSystemOpenManager
	{
		// Token: 0x0600166D RID: 5741 RVA: 0x0015F16C File Offset: 0x0015D36C
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

		// Token: 0x0600166E RID: 5742 RVA: 0x0015F31C File Offset: 0x0015D51C
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

		// Token: 0x0600166F RID: 5743 RVA: 0x0015F39C File Offset: 0x0015D59C
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

		// Token: 0x04002043 RID: 8259
		private object _VersionSystemOpenMutex = new object();

		// Token: 0x04002044 RID: 8260
		private Dictionary<string, int> VersionSystemOpenDict = new Dictionary<string, int>();

		// Token: 0x04002045 RID: 8261
		private Dictionary<int, bool> SystemOpenDict = new Dictionary<int, bool>();
	}
}
