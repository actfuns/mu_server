using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020006CF RID: 1743
	public class GeneralCachingXmlMgr
	{
		// Token: 0x0600241B RID: 9243 RVA: 0x001E9888 File Offset: 0x001E7A88
		private static XElement CachingXml(string xmlFileName)
		{
			XElement xml = null;
			try
			{
				string jieRiFileName = WorldLevelManager.getInstance().GetJieRiConfigFileName(xmlFileName);
				xml = XElement.Load(jieRiFileName);
				lock (GeneralCachingXmlMgr.CachingXmlDict)
				{
					GeneralCachingXmlMgr.CachingXmlDict[xmlFileName] = xml;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString() + "xmlFileName=" + xmlFileName);
				return null;
			}
			return xml;
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x001E992C File Offset: 0x001E7B2C
		public static XElement GetXElement(string xmlFileName)
		{
			XElement xml = null;
			lock (GeneralCachingXmlMgr.CachingXmlDict)
			{
				if (GeneralCachingXmlMgr.CachingXmlDict.TryGetValue(xmlFileName, out xml))
				{
					return xml;
				}
			}
			return GeneralCachingXmlMgr.CachingXml(xmlFileName);
		}

		// Token: 0x0600241D RID: 9245 RVA: 0x001E999C File Offset: 0x001E7B9C
		public static XElement Reload(string xmlFileName)
		{
			return GeneralCachingXmlMgr.CachingXml(xmlFileName);
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x001E99B4 File Offset: 0x001E7BB4
		public static void RemoveCachingXml(string xmlFileName)
		{
			lock (GeneralCachingXmlMgr.CachingXmlDict)
			{
				GeneralCachingXmlMgr.CachingXmlDict.Remove(xmlFileName);
			}
		}

		// Token: 0x04003854 RID: 14420
		private static Dictionary<string, XElement> CachingXmlDict = new Dictionary<string, XElement>();
	}
}
