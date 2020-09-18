using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.DB
{
	
	public class DBUserMgr
	{
		
		public int GetUserInfoCount()
		{
			int count;
			lock (this.DictUserInfos)
			{
				count = this.DictUserInfos.Count;
			}
			return count;
		}

		
		public DBUserInfo FindDBUserInfo(string userID)
		{
			DBUserInfo dbUserInfo = null;
			MyWeakReference weakRef = null;
			lock (this.DictUserInfos)
			{
				if (this.DictUserInfos.Count > 0)
				{
					if (this.DictUserInfos.TryGetValue(userID, out weakRef))
					{
						if (weakRef.IsAlive)
						{
							dbUserInfo = (weakRef.Target as DBUserInfo);
						}
					}
				}
			}
			if (null != dbUserInfo)
			{
				lock (dbUserInfo)
				{
					dbUserInfo.LastReferenceTicks = DateTime.Now.Ticks / 10000L;
				}
			}
			return dbUserInfo;
		}

		
		public DBUserInfo AddDBUserInfo(DBUserInfo dbUserInfo)
		{
			MyWeakReference weakRef = null;
			lock (this.DictUserInfos)
			{
				if (this.DictUserInfos.TryGetValue(dbUserInfo.UserID, out weakRef))
				{
					DBUserInfo old = weakRef.Target as DBUserInfo;
					if (null != old)
					{
						return old;
					}
					weakRef.Target = dbUserInfo;
				}
				else
				{
					this.DictUserInfos.Add(dbUserInfo.UserID, new MyWeakReference(dbUserInfo));
				}
			}
			return dbUserInfo;
		}

		
		public void RemoveDBUserInfo(string userID)
		{
			MyWeakReference weakRef = null;
			lock (this.DictUserInfos)
			{
				if (this.DictUserInfos.TryGetValue(userID, out weakRef))
				{
					weakRef.Target = null;
				}
			}
		}

		
		public void ReleaseIdleDBUserInfos(int ticksSlot)
		{
			long nowTicks = DateTime.Now.Ticks / 10000L;
			List<string> idleUserIDList = new List<string>();
			lock (this.DictUserInfos)
			{
				foreach (MyWeakReference weakRef in this.DictUserInfos.Values)
				{
					if (weakRef.IsAlive)
					{
						DBUserInfo dbUserInfo = weakRef.Target as DBUserInfo;
						if (null != dbUserInfo)
						{
							if (nowTicks - dbUserInfo.LastReferenceTicks >= (long)ticksSlot)
							{
								idleUserIDList.Add(dbUserInfo.UserID);
							}
						}
					}
				}
			}
			for (int i = 0; i < idleUserIDList.Count; i++)
			{
				this.RemoveDBUserInfo(idleUserIDList[i]);
				LogManager.WriteLog(LogTypes.Info, string.Format("释放空闲的用户数据: {0}", idleUserIDList[i]), null, true);
			}
		}

		
		private Dictionary<string, MyWeakReference> DictUserInfos = new Dictionary<string, MyWeakReference>(10000);
	}
}
