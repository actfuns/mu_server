using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class ThemeActivityConfig
	{
		
		public bool InList(int type)
		{
			return this.ConfigDict.ContainsKey(type);
		}

		
		public string GetFileName(int type)
		{
			string result;
			if (this.ConfigDict.ContainsKey(type))
			{
				result = this.ConfigDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public string GetActivityName(int type)
		{
			string result;
			if (this.ConfigDict.ContainsKey(type))
			{
				result = this.ActivityNameDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public int GetEndData(int type)
		{
			int result;
			if (this.EndDataDict.ContainsKey(type))
			{
				result = this.EndDataDict[type];
			}
			else
			{
				result = -1;
			}
			return result;
		}

		
		public XElement GetFilterXElement()
		{
			string fileName = "Config/ThemeActivityType.xml";
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			XElement result;
			if (null == xml)
			{
				result = null;
			}
			else
			{
				XElement filterxml = new XElement(xml);
				List<XElement> removeList = new List<XElement>();
				IEnumerable<XElement> xmlItems = filterxml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					int activityid = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Type"));
					int endData = this.GetEndData(activityid);
					if (endData > 0 && TimeUtil.NowDateTime() > Global.GetKaiFuTime().AddDays((double)endData))
					{
						removeList.Add(xmlItem);
					}
				}
				removeList.ForEach(delegate(XElement x)
				{
					x.Remove();
				});
				result = filterxml;
			}
			return result;
		}

		
		public Dictionary<int, string> ConfigDict = new Dictionary<int, string>();

		
		public Dictionary<int, string> ActivityNameDict = new Dictionary<int, string>();

		
		public Dictionary<int, int> EndDataDict = new Dictionary<int, int>();

		
		public int ActivityOpenVavle;

		
		public List<int> openList = new List<int>();
	}
}
