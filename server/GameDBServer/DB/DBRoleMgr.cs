using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	
	public class DBRoleMgr
	{
		
		public int GetRoleInfoCount()
		{
			int count;
			lock (this.DictRoleInfos)
			{
				count = this.DictRoleInfos.Count;
			}
			return count;
		}

		
		public DBRoleInfo FindDBRoleInfo(ref int roleID)
		{
			if (roleID < 200000)
			{
				int tempRoleID = roleID;
				roleID = SingletonTemplate<RoleMapper>.Instance().GetLocalRoleIDByTempID(tempRoleID);
			}
			DBRoleInfo result;
			if (roleID <= 0)
			{
				result = null;
			}
			else
			{
				DBRoleInfo dbRoleInfo = null;
				MyWeakReference weakRef = null;
				lock (this.DictRoleInfos)
				{
					if (this.DictRoleInfos.Count > 0)
					{
						if (this.DictRoleInfos.TryGetValue(roleID, out weakRef))
						{
							if (weakRef.IsAlive)
							{
								dbRoleInfo = (weakRef.Target as DBRoleInfo);
							}
						}
					}
				}
				if (null != dbRoleInfo)
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.LastReferenceTicks = DateTime.Now.Ticks / 10000L;
					}
				}
				result = dbRoleInfo;
			}
			return result;
		}

		
		public int FindDBRoleID(string roleName)
		{
			int roleID = -1;
			int result;
			if (!this.DictRoleName2ID.TryGetValue(roleName, out roleID))
			{
				result = -1;
			}
			else
			{
				result = roleID;
			}
			return result;
		}

		
		public DBRoleInfo AddDBRoleInfo(DBRoleInfo dbRoleInfo)
		{
			MyWeakReference weakRef = null;
			lock (this.DictRoleInfos)
			{
				if (this.DictRoleInfos.TryGetValue(dbRoleInfo.RoleID, out weakRef))
				{
					DBRoleInfo old = weakRef.Target as DBRoleInfo;
					if (null != old)
					{
						return old;
					}
					weakRef.Target = dbRoleInfo;
				}
				else
				{
					this.DictRoleInfos.Add(dbRoleInfo.RoleID, new MyWeakReference(dbRoleInfo));
				}
			}
			lock (this.DictRoleName2ID)
			{
				string formatedRoleName = Global.FormatRoleName(dbRoleInfo);
				this.DictRoleName2ID[formatedRoleName] = dbRoleInfo.RoleID;
			}
			return dbRoleInfo;
		}

		
		public void RemoveDBRoleInfo(int roleID)
		{
			string formatedRoleName = null;
			MyWeakReference weakRef = null;
			lock (this.DictRoleInfos)
			{
				if (this.DictRoleInfos.TryGetValue(roleID, out weakRef))
				{
					formatedRoleName = Global.FormatRoleName(weakRef.Target as DBRoleInfo);
					weakRef.Target = null;
				}
			}
			lock (this.DictRoleName2ID)
			{
				if (null != formatedRoleName)
				{
					this.DictRoleName2ID.Remove(formatedRoleName);
				}
			}
		}

		
		public void ClearAllDBroleInfo()
		{
			lock (this.DictRoleInfos)
			{
				this.DictRoleInfos.Clear();
			}
			lock (this.DictRoleName2ID)
			{
				this.DictRoleName2ID.Clear();
			}
		}

		
		public List<DBRoleInfo> GetCachingDBRoleInfoListByFaction(int faction)
		{
			List<DBRoleInfo> dbRoleInfoList = new List<DBRoleInfo>();
			lock (this.DictRoleInfos)
			{
				foreach (int key in this.DictRoleInfos.Keys)
				{
					MyWeakReference weakRef = this.DictRoleInfos[key];
					DBRoleInfo dbRoleInfo = weakRef.Target as DBRoleInfo;
					if (dbRoleInfo != null && dbRoleInfo.Faction == faction)
					{
						dbRoleInfoList.Add(dbRoleInfo);
					}
				}
			}
			return dbRoleInfoList;
		}

		
		public void ReleaseIdleDBRoleInfos(int ticksSlot)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			long needUpdateTicks = TimeUtil.NOW() - (long)ticksSlot;
			Dictionary<DBRoleInfo, List<RoleParamsData>> dict = new Dictionary<DBRoleInfo, List<RoleParamsData>>();
			List<int> idleRoleIDList = new List<int>();
			lock (this.DictRoleInfos)
			{
				foreach (MyWeakReference weakRef in this.DictRoleInfos.Values)
				{
					if (weakRef.IsAlive)
					{
						DBRoleInfo dbRoleInfo = weakRef.Target as DBRoleInfo;
						if (null != dbRoleInfo)
						{
							List<RoleParamsData> updateList = null;
							lock (dbRoleInfo)
							{
								if (null != dbRoleInfo.RoleParamsDict)
								{
									foreach (RoleParamsData roleParamData in dbRoleInfo.RoleParamsDict.Values)
									{
										if (roleParamData.UpdateFaildTicks > 0L && needUpdateTicks > roleParamData.UpdateFaildTicks)
										{
											if (null == updateList)
											{
												if (!dict.TryGetValue(dbRoleInfo, out updateList))
												{
													updateList = new List<RoleParamsData>();
													dict.Add(dbRoleInfo, updateList);
												}
											}
											updateList.Add(roleParamData);
										}
									}
								}
							}
							if (null == updateList)
							{
								if (dbRoleInfo.ServerLineID <= 0 && nowTicks - dbRoleInfo.LastReferenceTicks >= (long)ticksSlot)
								{
									idleRoleIDList.Add(dbRoleInfo.RoleID);
								}
							}
						}
					}
				}
			}
			DBManager dbMgr = DBManager.getInstance();
			foreach (KeyValuePair<DBRoleInfo, List<RoleParamsData>> kv in dict)
			{
				foreach (RoleParamsData paramData in kv.Value)
				{
					Global.UpdateRoleParamByName(dbMgr, kv.Key, paramData.ParamName, paramData.ParamValue, paramData.ParamType);
				}
			}
			for (int i = 0; i < idleRoleIDList.Count; i++)
			{
				this.RemoveDBRoleInfo(idleRoleIDList[i]);
				LogManager.WriteLog(LogTypes.Info, string.Format("释放空闲的角色数据: {0}", idleRoleIDList[i]), null, true);
			}
		}

		
		public void ReleaseDBRoleInfoByID(int roleID)
		{
			DBRoleInfo dbRoleInfo = this.FindDBRoleInfo(ref roleID);
			if (null != dbRoleInfo)
			{
				GlobalEventSource.getInstance().fireEvent(new PlayerLogoutEventObject(dbRoleInfo));
				this.RemoveDBRoleInfo(dbRoleInfo.RoleID);
				LogManager.WriteLog(LogTypes.SQL, string.Format("释放指定角色的数据: {0}", dbRoleInfo.RoleID), null, true);
			}
		}

		
		public void LoadDBRoleInfos(DBManager dbMgr, MySQLConnection conn)
		{
		}

		
		public void OnChangeName(int roleId, int zoneId, string oldName, string newName)
		{
			lock (this.DictRoleName2ID)
			{
				string fmtOldName = Global.FormatRoleName(zoneId, oldName);
				int _roleId = 0;
				if (this.DictRoleName2ID.TryGetValue(fmtOldName, out _roleId))
				{
					this.DictRoleName2ID.Remove(fmtOldName);
					this.DictRoleName2ID[Global.FormatRoleName(zoneId, newName)] = _roleId;
				}
			}
		}

		
		private Dictionary<int, MyWeakReference> DictRoleInfos = new Dictionary<int, MyWeakReference>(10000);

		
		private Dictionary<string, int> DictRoleName2ID = new Dictionary<string, int>(10000);
	}
}
