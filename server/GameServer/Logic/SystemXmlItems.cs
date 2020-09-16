using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	
	public class SystemXmlItems
	{
		
		
		public Dictionary<int, SystemXmlItem> SystemXmlItemDict
		{
			get
			{
				return this._SystemXmlItemDict;
			}
		}

		
		
		
		public int MaxKey { get; private set; }

		
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

		
		public void LoadFromXMlFile(string fileName, string rootName, string keyName, int resType = 0)
		{
			this._SystemXmlItemDict = this._LoadFromXMlFile(fileName, rootName, keyName, resType);
		}

		
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

		
		private Dictionary<int, SystemXmlItem> _SystemXmlItemDict = null;

		
		private bool FirstLoadOK = false;

		
		private string FileName = "";

		
		private string RootName = "";

		
		private string KeyName = "";

		
		private int ResType = 0;
	}
}
