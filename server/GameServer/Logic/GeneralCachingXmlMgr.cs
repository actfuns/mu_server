using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class GeneralCachingXmlMgr
	{
		
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

		
		public static XElement Reload(string xmlFileName)
		{
			return GeneralCachingXmlMgr.CachingXml(xmlFileName);
		}

		
		public static void RemoveCachingXml(string xmlFileName)
		{
			lock (GeneralCachingXmlMgr.CachingXmlDict)
			{
				GeneralCachingXmlMgr.CachingXmlDict.Remove(xmlFileName);
			}
		}

		
		private static Dictionary<string, XElement> CachingXmlDict = new Dictionary<string, XElement>();
	}
}
