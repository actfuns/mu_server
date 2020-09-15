using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic.LiXianGuaJi
{
	// Token: 0x02000740 RID: 1856
	public class LiXianGuaJiManager : ScheduleTask, IManager
	{
		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06002EB6 RID: 11958 RVA: 0x0028BB80 File Offset: 0x00289D80
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		// Token: 0x06002EB7 RID: 11959 RVA: 0x0028BB98 File Offset: 0x00289D98
		public static LiXianGuaJiManager getInstance()
		{
			return LiXianGuaJiManager._Instance;
		}

		// Token: 0x06002EB8 RID: 11960 RVA: 0x0028BBB0 File Offset: 0x00289DB0
		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(this, 0, 30000);
			return true;
		}

		// Token: 0x06002EB9 RID: 11961 RVA: 0x0028BBD8 File Offset: 0x00289DD8
		public bool startup()
		{
			return true;
		}

		// Token: 0x06002EBA RID: 11962 RVA: 0x0028BBEC File Offset: 0x00289DEC
		public bool showdown()
		{
			ScheduleExecutor2.Instance.scheduleCancle(this);
			LiXianGuaJiManager.SaveGuaJiTimeForAll();
			return true;
		}

		// Token: 0x06002EBB RID: 11963 RVA: 0x0028BC14 File Offset: 0x00289E14
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06002EBC RID: 11964 RVA: 0x0028BC27 File Offset: 0x00289E27
		public void run()
		{
			this.ProcessQueue();
		}

		// Token: 0x06002EBD RID: 11965 RVA: 0x0028BC34 File Offset: 0x00289E34
		public static List<LiXianGuaJiRoleItem> GetLiXianGuaJiRoleItemList()
		{
			List<LiXianGuaJiRoleItem> LiXianGuaJiRoleItems;
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				LiXianGuaJiRoleItems = LiXianGuaJiManager._LiXianRoleInfoDict.Values.ToList<LiXianGuaJiRoleItem>();
			}
			return LiXianGuaJiRoleItems;
		}

		// Token: 0x06002EBE RID: 11966 RVA: 0x0028BC90 File Offset: 0x00289E90
		public void ProcessQueue()
		{
			long nowTicks = TimeUtil.NOW();
			List<LiXianGuaJiRoleItem> LiXianGuaJiRoleItems = LiXianGuaJiManager.GetLiXianGuaJiRoleItemList();
			for (int i = 0; i < LiXianGuaJiRoleItems.Count; i++)
			{
				this.DoSpriteMeditateTime(LiXianGuaJiRoleItems[i]);
				if (LiXianGuaJiRoleItems[i].MeditateTime + LiXianGuaJiRoleItems[i].NotSafeMeditateTime >= 43200000)
				{
					LiXianGuaJiManager.SaveDBLiXianGuaJiTimeForRole(LiXianGuaJiRoleItems[i]);
					LiXianGuaJiManager.RemoveLiXianGuaJiRole(LiXianGuaJiRoleItems[i].RoleID);
					if (LiXianGuaJiRoleItems[i].FakeRoleID > 0)
					{
						FakeRoleManager.ProcessDelFakeRole(LiXianGuaJiRoleItems[i].FakeRoleID, false);
					}
				}
			}
		}

		// Token: 0x06002EBF RID: 11967 RVA: 0x0028BD48 File Offset: 0x00289F48
		private void DoSpriteMeditateTime(LiXianGuaJiRoleItem c)
		{
			long lCurrticks = TimeUtil.NOW();
			long lTicks = lCurrticks - c.MeditateTicks;
			if (lTicks >= 60000L)
			{
				c.MeditateTicks = lCurrticks;
				bool bIsInsafeArea = false;
				Point currentGrid = c.CurrentGrid;
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(c.MapCode, out gameMap))
				{
					bIsInsafeArea = gameMap.InSafeRegionList(currentGrid);
				}
				if (bIsInsafeArea)
				{
					int nTime = c.MeditateTime;
					int nTime2 = c.NotSafeMeditateTime;
					if (nTime + nTime2 < 43200000)
					{
						long msecs = Math.Max(lCurrticks - c.BiGuanTime, 0L);
						msecs = Math.Min(msecs + (long)nTime, (long)(43200000 - nTime2));
						c.MeditateTime = (int)msecs;
					}
				}
				else
				{
					int nTime = c.MeditateTime;
					int nTime2 = c.NotSafeMeditateTime;
					if (nTime + nTime2 < 43200000)
					{
						long msecs = Math.Max(lCurrticks - c.BiGuanTime, 0L);
						msecs = Math.Min(msecs + (long)nTime2, (long)(43200000 - nTime));
						c.NotSafeMeditateTime = (int)msecs;
					}
				}
				c.BiGuanTime = lCurrticks;
			}
		}

		// Token: 0x06002EC0 RID: 11968 RVA: 0x0028BE84 File Offset: 0x0028A084
		public static void AddLiXianGuaJiRole(GameClient client, int fakeRoleID)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				LiXianGuaJiManager._LiXianRoleInfoDict[client.ClientData.RoleID] = new LiXianGuaJiRoleItem
				{
					ZoneID = client.ClientData.ZoneID,
					UserID = userID,
					RoleID = client.ClientData.RoleID,
					RoleName = client.ClientData.RoleName,
					RoleLevel = client.ClientData.Level,
					CurrentGrid = client.CurrentGrid,
					StartTicks = TimeUtil.NOW(),
					FakeRoleID = fakeRoleID,
					MeditateTime = client.ClientData.MeditateTime,
					NotSafeMeditateTime = client.ClientData.NotSafeMeditateTime,
					MapCode = client.ClientData.MapCode
				};
			}
		}

		// Token: 0x06002EC1 RID: 11969 RVA: 0x0028BF94 File Offset: 0x0028A194
		public static void RemoveLiXianGuaJiRole(GameClient client)
		{
			LiXianGuaJiManager.RemoveLiXianGuaJiRole(client.ClientData.RoleID);
		}

		// Token: 0x06002EC2 RID: 11970 RVA: 0x0028BFA8 File Offset: 0x0028A1A8
		public static void RemoveLiXianGuaJiRole(int roleID)
		{
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				LiXianGuaJiManager._LiXianRoleInfoDict.Remove(roleID);
			}
		}

		// Token: 0x06002EC3 RID: 11971 RVA: 0x0028BFF8 File Offset: 0x0028A1F8
		public static void GetBackLiXianGuaJiTime(GameClient client)
		{
			LiXianGuaJiRoleItem liXianGuaJiRoleItem = null;
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				if (LiXianGuaJiManager._LiXianRoleInfoDict.TryGetValue(client.ClientData.RoleID, out liXianGuaJiRoleItem))
				{
					client.ClientData.MeditateTime = liXianGuaJiRoleItem.MeditateTime;
					client.ClientData.NotSafeMeditateTime = liXianGuaJiRoleItem.NotSafeMeditateTime;
					Global.SaveRoleParamsInt32ValueToDB(client, "MeditateTime", client.ClientData.MeditateTime, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "NotSafeMeditateTime", client.ClientData.NotSafeMeditateTime, true);
				}
			}
		}

		// Token: 0x06002EC4 RID: 11972 RVA: 0x0028C0B4 File Offset: 0x0028A2B4
		public static bool DelFakeRoleByClient(GameClient client)
		{
			int fakeRoleID = -1;
			LiXianGuaJiRoleItem liXianGuaJiRoleItem = null;
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				if (!LiXianGuaJiManager._LiXianRoleInfoDict.TryGetValue(client.ClientData.RoleID, out liXianGuaJiRoleItem))
				{
					return false;
				}
				fakeRoleID = liXianGuaJiRoleItem.FakeRoleID;
			}
			if (fakeRoleID > 0)
			{
				FakeRoleManager.ProcessDelFakeRole(fakeRoleID, false);
			}
			return true;
		}

		// Token: 0x06002EC5 RID: 11973 RVA: 0x0028C148 File Offset: 0x0028A348
		public static void SaveGuaJiTimeForAll()
		{
			long nowTicks = TimeUtil.NOW();
			List<LiXianGuaJiRoleItem> LiXianGuaJiRoleItems = LiXianGuaJiManager.GetLiXianGuaJiRoleItemList();
			for (int i = 0; i < LiXianGuaJiRoleItems.Count; i++)
			{
				LiXianGuaJiManager.SaveDBLiXianGuaJiTimeForRole(LiXianGuaJiRoleItems[i]);
			}
		}

		// Token: 0x06002EC6 RID: 11974 RVA: 0x0028C188 File Offset: 0x0028A388
		public static void SaveDBLiXianGuaJiTimeForRole(LiXianGuaJiRoleItem liXianGuaJiRoleItem)
		{
			GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", liXianGuaJiRoleItem.RoleID, "MeditateTime", liXianGuaJiRoleItem.MeditateTime), null, 0);
			GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", liXianGuaJiRoleItem.RoleID, "NotSafeMeditateTime", liXianGuaJiRoleItem.NotSafeMeditateTime), null, 0);
		}

		// Token: 0x04003C65 RID: 15461
		public const int MaxMingXiangTicks = 43200000;

		// Token: 0x04003C66 RID: 15462
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		// Token: 0x04003C67 RID: 15463
		private static LiXianGuaJiManager _Instance = new LiXianGuaJiManager();

		// Token: 0x04003C68 RID: 15464
		private static Dictionary<int, LiXianGuaJiRoleItem> _LiXianRoleInfoDict = new Dictionary<int, LiXianGuaJiRoleItem>();
	}
}
