using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic.LiXianGuaJi
{
	
	public class LiXianGuaJiManager : ScheduleTask, IManager
	{
		
		
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		
		public static LiXianGuaJiManager getInstance()
		{
			return LiXianGuaJiManager._Instance;
		}

		
		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(this, 0, 30000);
			return true;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			ScheduleExecutor2.Instance.scheduleCancle(this);
			LiXianGuaJiManager.SaveGuaJiTimeForAll();
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public void run()
		{
			this.ProcessQueue();
		}

		
		public static List<LiXianGuaJiRoleItem> GetLiXianGuaJiRoleItemList()
		{
			List<LiXianGuaJiRoleItem> LiXianGuaJiRoleItems;
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				LiXianGuaJiRoleItems = LiXianGuaJiManager._LiXianRoleInfoDict.Values.ToList<LiXianGuaJiRoleItem>();
			}
			return LiXianGuaJiRoleItems;
		}

		
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

		
		public static void RemoveLiXianGuaJiRole(GameClient client)
		{
			LiXianGuaJiManager.RemoveLiXianGuaJiRole(client.ClientData.RoleID);
		}

		
		public static void RemoveLiXianGuaJiRole(int roleID)
		{
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				LiXianGuaJiManager._LiXianRoleInfoDict.Remove(roleID);
			}
		}

		
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

		
		public static void SaveGuaJiTimeForAll()
		{
			long nowTicks = TimeUtil.NOW();
			List<LiXianGuaJiRoleItem> LiXianGuaJiRoleItems = LiXianGuaJiManager.GetLiXianGuaJiRoleItemList();
			for (int i = 0; i < LiXianGuaJiRoleItems.Count; i++)
			{
				LiXianGuaJiManager.SaveDBLiXianGuaJiTimeForRole(LiXianGuaJiRoleItems[i]);
			}
		}

		
		public static void SaveDBLiXianGuaJiTimeForRole(LiXianGuaJiRoleItem liXianGuaJiRoleItem)
		{
			GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", liXianGuaJiRoleItem.RoleID, "MeditateTime", liXianGuaJiRoleItem.MeditateTime), null, 0);
			GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", liXianGuaJiRoleItem.RoleID, "NotSafeMeditateTime", liXianGuaJiRoleItem.NotSafeMeditateTime), null, 0);
		}

		
		public const int MaxMingXiangTicks = 43200000;

		
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		
		private static LiXianGuaJiManager _Instance = new LiXianGuaJiManager();

		
		private static Dictionary<int, LiXianGuaJiRoleItem> _LiXianRoleInfoDict = new Dictionary<int, LiXianGuaJiRoleItem>();
	}
}
