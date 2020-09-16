using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class SpriteContainer
	{
		
		public void initialize(IEnumerable<XElement> mapItems)
		{
			foreach (XElement mapItem in mapItems)
			{
				int mapCode = (int)Global.GetSafeAttributeLong(mapItem, "Code");
				List<object> objList = new List<object>(100);
				this._MapObjectDict.Add(mapCode, objList);
			}
		}

		
		
		public Dictionary<int, object> ObjectDict
		{
			get
			{
				return this._ObjectDict;
			}
		}

		
		
		public Dictionary<int, List<object>> MapObjectDict
		{
			get
			{
				return this._MapObjectDict;
			}
		}

		
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

		
		public object FindObject(int id)
		{
			object obj = null;
			lock (this._ObjectDict)
			{
				this._ObjectDict.TryGetValue(id, out obj);
			}
			return obj;
		}

		
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

		
		private Dictionary<int, object> _ObjectDict = new Dictionary<int, object>(1000);

		
		private Dictionary<int, List<object>> _MapObjectDict = new Dictionary<int, List<object>>(1000);
	}
}
