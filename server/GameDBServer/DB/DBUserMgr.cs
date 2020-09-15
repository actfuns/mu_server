using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.DB
{
	// Token: 0x020001A0 RID: 416
	public class DBUserMgr
	{
		// Token: 0x060007C2 RID: 1986 RVA: 0x000479C4 File Offset: 0x00045BC4
		public int GetUserInfoCount()
		{
			int count;
			lock (this.DictUserInfos)
			{
				count = this.DictUserInfos.Count;
			}
			return count;
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x00047A18 File Offset: 0x00045C18
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

		// Token: 0x060007C4 RID: 1988 RVA: 0x00047B18 File Offset: 0x00045D18
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

		// Token: 0x060007C5 RID: 1989 RVA: 0x00047BC8 File Offset: 0x00045DC8
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

		// Token: 0x060007C6 RID: 1990 RVA: 0x00047C30 File Offset: 0x00045E30
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

		// Token: 0x04000989 RID: 2441
		private Dictionary<string, MyWeakReference> DictUserInfos = new Dictionary<string, MyWeakReference>(10000);
	}
}
