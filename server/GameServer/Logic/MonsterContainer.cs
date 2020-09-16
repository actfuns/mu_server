using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	
	internal class MonsterContainer
	{
		
		public void initialize(IEnumerable<XElement> mapItems)
		{
			foreach (XElement mapItem in mapItems)
			{
				int mapCode = (int)Global.GetSafeAttributeLong(mapItem, "Code");
				List<object> objList = new List<object>(100);
				this._MapObjectDict.Add(mapCode, objList);
				Dictionary<int, object> objDict = new Dictionary<int, object>(100);
				this._ObjectDict.Add(mapCode, objDict);
				if (mapCode == 6090)
				{
					for (int i = 0; i < 25; i++)
					{
						Dictionary<int, object> freshPlayerObjDict = new Dictionary<int, object>(2000);
						this._FreshPlayerObjectDict.Add(i, freshPlayerObjDict);
						List<object> freshPlayerObjList = new List<object>(100);
						this._FreshPlayerMapObjectDict.Add(i, freshPlayerObjList);
					}
				}
			}
		}

		
		
		public List<object> ObjectList
		{
			get
			{
				return this._ObjectList;
			}
		}

		
		
		public Dictionary<int, Dictionary<int, object>> ObjectDict
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

		
		
		public Dictionary<int, List<object>> CopyMapIDObjectDict
		{
			get
			{
				return this._CopyMapIDObjectDict;
			}
		}

		
		public void AddObject(int id, int mapCode, int copyMapID, Monster obj)
		{
			lock (this._ObjectList)
			{
				this._ObjectList.Add(obj);
			}
			Dictionary<int, object> objDict = null;
			if (this._ObjectDict.TryGetValue(mapCode, out objDict))
			{
				lock (objDict)
				{
					objDict.Add(id, obj);
				}
			}
			List<object> objList = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out objList))
			{
				lock (objList)
				{
					objList.Add(obj);
				}
			}
			if (mapCode == 6090)
			{
				int subMapCode = Global.GetRandomNumber(0, 24);
				obj.SubMapCode = subMapCode;
				List<object> list = null;
				if (this._FreshPlayerMapObjectDict.TryGetValue(subMapCode, out list))
				{
					lock (list)
					{
						list.Add(obj);
					}
				}
				Dictionary<int, object> dict = null;
				if (this._FreshPlayerObjectDict.TryGetValue(subMapCode, out dict))
				{
					lock (dict)
					{
						dict.Add(id, obj);
					}
				}
			}
			lock (this._CopyMapIDObjectDict)
			{
				List<object> _objList = null;
				if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out _objList))
				{
					_objList.Add(obj);
				}
				else
				{
					_objList = new List<object>(100);
					_objList.Add(obj);
					this._CopyMapIDObjectDict.Add(copyMapID, _objList);
				}
			}
		}

		
		public void RemoveObject(int id, int mapCode, int copyMapID, Monster obj)
		{
			lock (this._ObjectList)
			{
				this._ObjectList.Remove(obj);
			}
			Dictionary<int, object> objDict = null;
			if (this._ObjectDict.TryGetValue(mapCode, out objDict))
			{
				lock (objDict)
				{
					objDict.Remove(id);
				}
			}
			List<object> objList = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out objList))
			{
				try
				{
					lock (objList)
					{
						objList.Remove(obj);
					}
				}
				catch (Exception)
				{
				}
			}
			if (mapCode == 6090)
			{
				int subMapCode = obj.SubMapCode;
				List<object> list = null;
				if (this._FreshPlayerMapObjectDict.TryGetValue(subMapCode, out list))
				{
					try
					{
						lock (list)
						{
							list.Remove(obj);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
				Dictionary<int, object> dict = null;
				if (this._FreshPlayerObjectDict.TryGetValue(subMapCode, out dict))
				{
					try
					{
						lock (dict)
						{
							dict.Remove(id);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}
			List<object> _objList = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out _objList))
			{
				try
				{
					lock (_objList)
					{
						_objList.Remove(obj);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		
		public List<object> GetObjectsByMap(int mapCode, int subMapCode = -1)
		{
			List<object> newObjList = null;
			List<object> objList = null;
			if (mapCode == 6090 && subMapCode != -1)
			{
				if (this._FreshPlayerMapObjectDict.TryGetValue(subMapCode, out objList))
				{
					lock (objList)
					{
						newObjList = objList.GetRange(0, objList.Count);
					}
				}
			}
			else if (this._MapObjectDict.TryGetValue(mapCode, out objList))
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

		
		public List<object> GetObjectsByCopyMapID(int copyMapID)
		{
			List<object> newObjList = null;
			List<object> objList = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out objList))
			{
				lock (objList)
				{
					newObjList = objList.GetRange(0, objList.Count);
				}
			}
			return newObjList;
		}

		
		public int GetObjectsCountByCopyMapID(int copyMapID, int aliveType = -1)
		{
			int count = 0;
			List<object> objList = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out objList))
			{
				if (null != objList)
				{
					if (-1 == aliveType)
					{
						lock (objList)
						{
							count = objList.Count;
						}
					}
					else if (0 == aliveType)
					{
						lock (objList)
						{
							for (int i = 0; i < objList.Count; i++)
							{
								if ((objList[i] as Monster).VLife > 0.0 && (objList[i] as Monster).Alive && (objList[i] as Monster).MonsterType != 1001)
								{
									count++;
								}
							}
						}
					}
					else
					{
						lock (objList)
						{
							for (int i = 0; i < objList.Count; i++)
							{
								if (!(objList[i] as Monster).Alive)
								{
									count++;
								}
							}
						}
					}
				}
			}
			return count;
		}

		
		public bool IsAnyMonsterAliveByCopyMapID(int copyMapID)
		{
			List<object> objList = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out objList))
			{
				if (null != objList)
				{
					lock (objList)
					{
						for (int i = 0; i < objList.Count; i++)
						{
							if ((objList[i] as Monster).Alive && (objList[i] as Monster).MonsterType != 1001)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		
		public object FindObject(int id, int mapCode)
		{
			object obj = null;
			Dictionary<int, object> objDict = null;
			if (this._ObjectDict.TryGetValue(mapCode, out objDict))
			{
				lock (objDict)
				{
					objDict.TryGetValue(id, out obj);
				}
			}
			return obj;
		}

		
		public List<object> FindObjectAll(int mapCode)
		{
			List<object> ret = new List<object>();
			Dictionary<int, object> objDict = null;
			if (this._ObjectDict.TryGetValue(mapCode, out objDict))
			{
				lock (objDict)
				{
					foreach (KeyValuePair<int, object> intem in objDict)
					{
						ret.Add(intem.Value);
					}
				}
			}
			return ret;
		}

		
		public List<object> FindObjectsByExtensionID(int extensionID, int copyMapID)
		{
			List<object> findObjsList = new List<object>();
			List<object> objList = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out objList))
			{
				if (null != objList)
				{
					lock (objList)
					{
						for (int i = 0; i < objList.Count; i++)
						{
							if ((objList[i] as Monster).VLife > 0.0 && (objList[i] as Monster).Alive && (objList[i] as Monster).MonsterInfo.ExtensionID == extensionID)
							{
								findObjsList.Add(objList[i]);
							}
						}
					}
				}
			}
			return findObjsList;
		}

		
		public List<object> _ObjectList = new List<object>(20000);

		
		private Dictionary<int, Dictionary<int, object>> _ObjectDict = new Dictionary<int, Dictionary<int, object>>(10000);

		
		private Dictionary<int, Dictionary<int, object>> _FreshPlayerObjectDict = new Dictionary<int, Dictionary<int, object>>(50);

		
		private Dictionary<int, List<object>> _MapObjectDict = new Dictionary<int, List<object>>(10000);

		
		private Dictionary<int, List<object>> _FreshPlayerMapObjectDict = new Dictionary<int, List<object>>(50);

		
		private Dictionary<int, List<object>> _CopyMapIDObjectDict = new Dictionary<int, List<object>>(10000);
	}
}
