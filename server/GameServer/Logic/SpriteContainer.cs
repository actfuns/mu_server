using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020007D6 RID: 2006
	public class SpriteContainer
	{
		// Token: 0x06003894 RID: 14484 RVA: 0x00303AA8 File Offset: 0x00301CA8
		public void initialize(IEnumerable<XElement> mapItems)
		{
			foreach (XElement mapItem in mapItems)
			{
				int mapCode = (int)Global.GetSafeAttributeLong(mapItem, "Code");
				List<object> objList = new List<object>(100);
				this._MapObjectDict.Add(mapCode, objList);
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06003895 RID: 14485 RVA: 0x00303B20 File Offset: 0x00301D20
		public Dictionary<int, object> ObjectDict
		{
			get
			{
				return this._ObjectDict;
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06003896 RID: 14486 RVA: 0x00303B38 File Offset: 0x00301D38
		public Dictionary<int, List<object>> MapObjectDict
		{
			get
			{
				return this._MapObjectDict;
			}
		}

		// Token: 0x06003897 RID: 14487 RVA: 0x00303B50 File Offset: 0x00301D50
		public void AddObject(int id, int mapCode, object obj)
		{
			lock (this._ObjectDict)
			{
				this._ObjectDict.Add(id, obj);
			}
			List<object> objList = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out objList))
			{
				lock (objList)
				{
					objList.Add(obj);
				}
			}
		}

		// Token: 0x06003898 RID: 14488 RVA: 0x00303BFC File Offset: 0x00301DFC
		public bool RemoveObject(int id, int mapCode, object obj)
		{
			bool removed = false;
			lock (this._ObjectDict)
			{
				try
				{
					if (this._ObjectDict.ContainsKey(id))
					{
						this._ObjectDict.Remove(id);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			List<object> objList = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out objList))
			{
				try
				{
					lock (objList)
					{
						removed = objList.Remove(obj);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return removed;
		}

		// Token: 0x06003899 RID: 14489 RVA: 0x00303D10 File Offset: 0x00301F10
		public List<object> GetObjectsByMap(int mapCode)
		{
			List<object> newObjList = null;
			List<object> objList = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out objList))
			{
				lock (objList)
				{
					newObjList = objList.GetRange(0, objList.Count);
				}
			}
			return newObjList;
		}

		// Token: 0x0600389A RID: 14490 RVA: 0x00303D84 File Offset: 0x00301F84
		public int GetObjectsCountByMap(int mapCode)
		{
			int count = 0;
			List<object> objList = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out objList))
			{
				lock (objList)
				{
					count = objList.Count;
				}
			}
			return count;
		}

		// Token: 0x0600389B RID: 14491 RVA: 0x00303DF4 File Offset: 0x00301FF4
		public object FindObject(int id)
		{
			object obj = null;
			lock (this._ObjectDict)
			{
				this._ObjectDict.TryGetValue(id, out obj);
			}
			return obj;
		}

		// Token: 0x0600389C RID: 14492 RVA: 0x00303E54 File Offset: 0x00302054
		public string GetAllMapRoleNumStr()
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<int, List<object>> kv in this._MapObjectDict)
			{
				lock (kv.Value)
				{
					if (kv.Value.Count > 0)
					{
						sb.AppendFormat("{0}:{1}\n", kv.Key, kv.Value.Count);
					}
				}
			}
			return sb.ToString();
		}

		// Token: 0x04004164 RID: 16740
		private Dictionary<int, object> _ObjectDict = new Dictionary<int, object>(1000);

		// Token: 0x04004165 RID: 16741
		private Dictionary<int, List<object>> _MapObjectDict = new Dictionary<int, List<object>>(1000);
	}
}
