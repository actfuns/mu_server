using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class SystemMagicManager
	{
		
		
		public SystemMagicManager MagicItemsDict
		{
			get
			{
				return this;
			}
		}

		
		public void LoadMagicItemsDict(SystemXmlItems systemMagicMgr)
		{
			lock (this._MagicItemsDict)
			{
				foreach (int key in systemMagicMgr.SystemXmlItemDict.Keys)
				{
					SystemXmlItem systemMagic = systemMagicMgr.SystemXmlItemDict[key];
					int intKey = systemMagic.GetIntValue("ID", -1);
					this._MagicItemsDict[intKey] = systemMagic;
				}
			}
		}

		
		public bool TryGetValue(int intKey, out SystemXmlItem systemMagic)
		{
			bool result;
			lock (this._MagicItemsDict)
			{
				result = this._MagicItemsDict.TryGetValue(intKey, out systemMagic);
			}
			return result;
		}

		
		private Dictionary<int, SystemXmlItem> _MagicItemsDict = new Dictionary<int, SystemXmlItem>();
	}
}
