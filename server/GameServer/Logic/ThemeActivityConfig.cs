using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000700 RID: 1792
	public class ThemeActivityConfig
	{
		// Token: 0x06002B16 RID: 11030 RVA: 0x00266A8C File Offset: 0x00264C8C
		public bool InList(int type)
		{
			return this.ConfigDict.ContainsKey(type);
		}

		// Token: 0x06002B17 RID: 11031 RVA: 0x00266AB8 File Offset: 0x00264CB8
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

		// Token: 0x06002B18 RID: 11032 RVA: 0x00266AF0 File Offset: 0x00264CF0
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

		// Token: 0x06002B19 RID: 11033 RVA: 0x00266B28 File Offset: 0x00264D28
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

		// Token: 0x06002B1A RID: 11034 RVA: 0x00266B68 File Offset: 0x00264D68
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

		// Token: 0x04003A1F RID: 14879
		public Dictionary<int, string> ConfigDict = new Dictionary<int, string>();

		// Token: 0x04003A20 RID: 14880
		public Dictionary<int, string> ActivityNameDict = new Dictionary<int, string>();

		// Token: 0x04003A21 RID: 14881
		public Dictionary<int, int> EndDataDict = new Dictionary<int, int>();

		// Token: 0x04003A22 RID: 14882
		public int ActivityOpenVavle;

		// Token: 0x04003A23 RID: 14883
		public List<int> openList = new List<int>();
	}
}
