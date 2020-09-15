using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020007DD RID: 2013
	public class SystemMagicManager
	{
		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x060038EA RID: 14570 RVA: 0x00307030 File Offset: 0x00305230
		public SystemMagicManager MagicItemsDict
		{
			get
			{
				return this;
			}
		}

		// Token: 0x060038EB RID: 14571 RVA: 0x00307044 File Offset: 0x00305244
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

		// Token: 0x060038EC RID: 14572 RVA: 0x00307100 File Offset: 0x00305300
		public bool TryGetValue(int intKey, out SystemXmlItem systemMagic)
		{
			bool result;
			lock (this._MagicItemsDict)
			{
				result = this._MagicItemsDict.TryGetValue(intKey, out systemMagic);
			}
			return result;
		}

		// Token: 0x04004302 RID: 17154
		private Dictionary<int, SystemXmlItem> _MagicItemsDict = new Dictionary<int, SystemXmlItem>();
	}
}
