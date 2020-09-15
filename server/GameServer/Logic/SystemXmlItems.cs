using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	// Token: 0x020007E0 RID: 2016
	public class SystemXmlItems
	{
		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x06003901 RID: 14593 RVA: 0x00307908 File Offset: 0x00305B08
		public Dictionary<int, SystemXmlItem> SystemXmlItemDict
		{
			get
			{
				return this._SystemXmlItemDict;
			}
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06003902 RID: 14594 RVA: 0x00307920 File Offset: 0x00305B20
		// (set) Token: 0x06003903 RID: 14595 RVA: 0x00307937 File Offset: 0x00305B37
		public int MaxKey { get; private set; }

		// Token: 0x06003904 RID: 14596 RVA: 0x00307940 File Offset: 0x00305B40
		private Dictionary<int, SystemXmlItem> _LoadFromXMlFile(string fileName, string rootName, string keyName, int resType)
		{
			XElement xml = null;
			try
			{
				string fullPathFileName = "";
				if (0 == resType)
				{
					fullPathFileName = Global.GameResPath(fileName);
				}
				else if (1 == resType)
				{
					fullPathFileName = Global.IsolateResPath(fileName);
				}
				fullPathFileName = WorldLevelManager.getInstance().GetJieRiConfigFileName(fullPathFileName);
				xml = XElement.Load(fullPathFileName);
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fullPathFileName));
				}
				this.FileName = fileName;
				this.RootName = rootName;
				this.KeyName = keyName;
				this.ResType = resType;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。\r\n{1}", fileName, ex.ToString()));
			}
			IEnumerable<XElement> nodes;
			if ("" == rootName)
			{
				nodes = xml.Elements();
			}
			else
			{
				nodes = xml.Elements(rootName).Elements<XElement>();
			}
			Dictionary<int, SystemXmlItem> systemXmlItemDict = new Dictionary<int, SystemXmlItem>();
			foreach (XElement node in nodes)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = node
				};
				int key = (int)Global.GetSafeAttributeLong(node, keyName);
				systemXmlItemDict[key] = systemXmlItem;
				if (key > this.MaxKey)
				{
					this.MaxKey = key;
				}
			}
			this.FirstLoadOK = true;
			return systemXmlItemDict;
		}

		// Token: 0x06003905 RID: 14597 RVA: 0x00307AE4 File Offset: 0x00305CE4
		public void LoadFromXMlFile(string fileName, string rootName, string keyName, int resType = 0)
		{
			this._SystemXmlItemDict = this._LoadFromXMlFile(fileName, rootName, keyName, resType);
		}

		// Token: 0x06003906 RID: 14598 RVA: 0x00307AF8 File Offset: 0x00305CF8
		public int ReloadLoadFromXMlFile()
		{
			int result;
			if (!this.FirstLoadOK)
			{
				result = -2;
			}
			else
			{
				try
				{
					this._SystemXmlItemDict = this._LoadFromXMlFile(this.FileName, this.RootName, this.KeyName, this.ResType);
				}
				catch (Exception)
				{
					return -1;
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x04004305 RID: 17157
		private Dictionary<int, SystemXmlItem> _SystemXmlItemDict = null;

		// Token: 0x04004306 RID: 17158
		private bool FirstLoadOK = false;

		// Token: 0x04004307 RID: 17159
		private string FileName = "";

		// Token: 0x04004308 RID: 17160
		private string RootName = "";

		// Token: 0x04004309 RID: 17161
		private string KeyName = "";

		// Token: 0x0400430A RID: 17162
		private int ResType = 0;
	}
}
