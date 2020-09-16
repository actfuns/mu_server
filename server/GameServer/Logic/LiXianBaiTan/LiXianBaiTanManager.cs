using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic.LiXianBaiTan
{
	
	public class LiXianBaiTanManager : ScheduleTask, IManager
	{
		
		
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		
		public static LiXianBaiTanManager getInstance()
		{
			return LiXianBaiTanManager._Instance;
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

		
		public void ProcessQueue()
		{
			long nowTicks = TimeUtil.NOW();
			List<LiXianSaleRoleItem> liXianSaleRoleItems;
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				liXianSaleRoleItems = LiXianBaiTanManager._LiXianRoleInfoDict.Values.ToList<LiXianSaleRoleItem>();
			}
			for (int i = 0; i < liXianSaleRoleItems.Count; i++)
			{
				if (nowTicks - liXianSaleRoleItems[i].StartTicks >= (long)liXianSaleRoleItems[i].LiXianBaiTanMaxTicks)
				{
					LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(liXianSaleRoleItems[i].RoleID);
					if (liXianSaleRoleItems[i].LiXianBaiTanMaxTicks >= 43200000)
					{
					}
				}
			}
		}

		
		public static void AddLiXianSaleGoodsItem(LiXianSaleGoodsItem liXianSaleGoodsItem)
		{
			SaleManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				LiXianBaiTanManager._LiXianSaleGoodsDict[liXianSaleGoodsItem.GoodsDbID] = liXianSaleGoodsItem;
			}
		}

		
		public static void AddLiXianSaleGoodsItems(GameClient client, int fakeRoleID)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			List<GoodsData> goodsDataList = client.ClientData.SaleGoodsDataList;
			if (null != goodsDataList)
			{
				lock (goodsDataList)
				{
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						LiXianSaleGoodsItem liXianSaleGoodsItem = new LiXianSaleGoodsItem
						{
							GoodsDbID = goodsDataList[i].Id,
							SalingGoodsData = goodsDataList[i],
							ZoneID = client.ClientData.ZoneID,
							UserID = userID,
							RoleID = client.ClientData.RoleID,
							RoleName = client.ClientData.RoleName,
							RoleLevel = client.ClientData.Level
						};
						LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
					}
				}
			}
			int maxTicks = (int)SaleManager.MaxSaleGoodsTime;
			maxTicks = Math.Min(43200000, maxTicks);
			GameManager.ClientMgr.ModifyLiXianBaiTanTicksValue(client, -maxTicks, true);
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				LiXianBaiTanManager._LiXianRoleInfoDict[client.ClientData.RoleID] = new LiXianSaleRoleItem
				{
					ZoneID = client.ClientData.ZoneID,
					UserID = userID,
					RoleID = client.ClientData.RoleID,
					RoleName = client.ClientData.RoleName,
					RoleLevel = client.ClientData.Level,
					CurrentGrid = client.CurrentGrid,
					LiXianBaiTanMaxTicks = maxTicks,
					StartTicks = TimeUtil.NOW(),
					FakeRoleID = fakeRoleID
				};
			}
		}

		
		public static LiXianSaleGoodsItem RemoveLiXianSaleGoodsItem(int goodsDbID)
		{
			SaleManager.RemoveSaleGoodsItem(goodsDbID);
			LiXianSaleGoodsItem result;
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				LiXianSaleGoodsItem liXianSaleGoodsItem = null;
				if (LiXianBaiTanManager._LiXianSaleGoodsDict.TryGetValue(goodsDbID, out liXianSaleGoodsItem))
				{
					LiXianBaiTanManager._LiXianSaleGoodsDict.Remove(goodsDbID);
				}
				result = liXianSaleGoodsItem;
			}
			return result;
		}

		
		public static void RemoveLiXianSaleGoodsItems(GameClient client)
		{
			LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(client.ClientData.RoleID);
		}

		
		public static void RemoveLiXianSaleGoodsItems(int roleID)
		{
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				List<LiXianSaleGoodsItem> liXianSaleGoodsItemList = new List<LiXianSaleGoodsItem>();
				foreach (LiXianSaleGoodsItem liXianSaleGoodsItem in LiXianBaiTanManager._LiXianSaleGoodsDict.Values)
				{
					if (liXianSaleGoodsItem.RoleID == roleID)
					{
						liXianSaleGoodsItemList.Add(liXianSaleGoodsItem);
					}
				}
				for (int i = 0; i < liXianSaleGoodsItemList.Count; i++)
				{
					LiXianBaiTanManager._LiXianSaleGoodsDict.Remove(liXianSaleGoodsItemList[i].GoodsDbID);
					SaleManager.RemoveSaleGoodsItem(liXianSaleGoodsItemList[i].GoodsDbID);
				}
			}
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				LiXianBaiTanManager._LiXianRoleInfoDict.Remove(roleID);
			}
		}

		
		public static void GetBackLiXianSaleLeftTicks(GameClient client)
		{
			LiXianSaleRoleItem liXianSaleRoleItem = null;
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				if (LiXianBaiTanManager._LiXianRoleInfoDict.TryGetValue(client.ClientData.RoleID, out liXianSaleRoleItem))
				{
					long nowTicks = TimeUtil.NOW();
					long leftTicks = nowTicks - liXianSaleRoleItem.StartTicks;
					if (leftTicks < (long)liXianSaleRoleItem.LiXianBaiTanMaxTicks)
					{
						leftTicks = Math.Max(0L, (long)liXianSaleRoleItem.LiXianBaiTanMaxTicks - leftTicks);
						GameManager.ClientMgr.ModifyLiXianBaiTanTicksValue(client, (int)leftTicks, true);
					}
				}
			}
		}

		
		public static List<GoodsData> GetLiXianSaleGoodsList(int roleID)
		{
			List<GoodsData> saleGoodsDataList = new List<GoodsData>();
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				List<LiXianSaleGoodsItem> liXianSaleGoodsItemList = new List<LiXianSaleGoodsItem>();
				foreach (LiXianSaleGoodsItem liXianSaleGoodsItem in LiXianBaiTanManager._LiXianSaleGoodsDict.Values)
				{
					if (liXianSaleGoodsItem.RoleID == roleID)
					{
						saleGoodsDataList.Add(liXianSaleGoodsItem.SalingGoodsData);
					}
				}
			}
			return saleGoodsDataList;
		}

		
		public static void DelFakeRoleByClient(GameClient client)
		{
			int fakeRoleID = -1;
			LiXianSaleRoleItem liXianSaleRoleItem = null;
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				if (!LiXianBaiTanManager._LiXianRoleInfoDict.TryGetValue(client.ClientData.RoleID, out liXianSaleRoleItem))
				{
					return;
				}
				fakeRoleID = liXianSaleRoleItem.FakeRoleID;
			}
			if (fakeRoleID > 0)
			{
				FakeRoleManager.ProcessDelFakeRole(fakeRoleID, false);
			}
		}

		
		public static int GetLiXianRoleCountByPoint(Point grid)
		{
			int roleCount = 0;
			lock (LiXianBaiTanManager._LiXianRoleInfoDict)
			{
				foreach (LiXianSaleRoleItem liXianSaleRoleItem in LiXianBaiTanManager._LiXianRoleInfoDict.Values)
				{
					if (liXianSaleRoleItem.CurrentGrid.X == grid.X && liXianSaleRoleItem.CurrentGrid.Y == grid.Y)
					{
						roleCount++;
					}
				}
			}
			return roleCount;
		}

		
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		
		private static LiXianBaiTanManager _Instance = new LiXianBaiTanManager();

		
		private static Dictionary<int, LiXianSaleGoodsItem> _LiXianSaleGoodsDict = new Dictionary<int, LiXianSaleGoodsItem>();

		
		private static Dictionary<int, LiXianSaleRoleItem> _LiXianRoleInfoDict = new Dictionary<int, LiXianSaleRoleItem>();
	}
}
