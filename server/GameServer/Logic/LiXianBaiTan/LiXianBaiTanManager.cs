using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic.LiXianBaiTan
{
	// Token: 0x0200073E RID: 1854
	public class LiXianBaiTanManager : ScheduleTask, IManager
	{
		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06002EA2 RID: 11938 RVA: 0x0028B240 File Offset: 0x00289440
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		// Token: 0x06002EA3 RID: 11939 RVA: 0x0028B258 File Offset: 0x00289458
		public static LiXianBaiTanManager getInstance()
		{
			return LiXianBaiTanManager._Instance;
		}

		// Token: 0x06002EA4 RID: 11940 RVA: 0x0028B270 File Offset: 0x00289470
		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(this, 0, 30000);
			return true;
		}

		// Token: 0x06002EA5 RID: 11941 RVA: 0x0028B298 File Offset: 0x00289498
		public bool startup()
		{
			return true;
		}

		// Token: 0x06002EA6 RID: 11942 RVA: 0x0028B2AC File Offset: 0x002894AC
		public bool showdown()
		{
			ScheduleExecutor2.Instance.scheduleCancle(this);
			return true;
		}

		// Token: 0x06002EA7 RID: 11943 RVA: 0x0028B2CC File Offset: 0x002894CC
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06002EA8 RID: 11944 RVA: 0x0028B2DF File Offset: 0x002894DF
		public void run()
		{
			this.ProcessQueue();
		}

		// Token: 0x06002EA9 RID: 11945 RVA: 0x0028B2EC File Offset: 0x002894EC
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

		// Token: 0x06002EAA RID: 11946 RVA: 0x0028B3BC File Offset: 0x002895BC
		public static void AddLiXianSaleGoodsItem(LiXianSaleGoodsItem liXianSaleGoodsItem)
		{
			SaleManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
			lock (LiXianBaiTanManager._LiXianSaleGoodsDict)
			{
				LiXianBaiTanManager._LiXianSaleGoodsDict[liXianSaleGoodsItem.GoodsDbID] = liXianSaleGoodsItem;
			}
		}

		// Token: 0x06002EAB RID: 11947 RVA: 0x0028B41C File Offset: 0x0028961C
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

		// Token: 0x06002EAC RID: 11948 RVA: 0x0028B630 File Offset: 0x00289830
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

		// Token: 0x06002EAD RID: 11949 RVA: 0x0028B6A8 File Offset: 0x002898A8
		public static void RemoveLiXianSaleGoodsItems(GameClient client)
		{
			LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(client.ClientData.RoleID);
		}

		// Token: 0x06002EAE RID: 11950 RVA: 0x0028B6BC File Offset: 0x002898BC
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

		// Token: 0x06002EAF RID: 11951 RVA: 0x0028B7F8 File Offset: 0x002899F8
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

		// Token: 0x06002EB0 RID: 11952 RVA: 0x0028B8A8 File Offset: 0x00289AA8
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

		// Token: 0x06002EB1 RID: 11953 RVA: 0x0028B970 File Offset: 0x00289B70
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

		// Token: 0x06002EB2 RID: 11954 RVA: 0x0028B9FC File Offset: 0x00289BFC
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

		// Token: 0x04003C54 RID: 15444
		private TaskInternalLock _InternalLock = new TaskInternalLock();

		// Token: 0x04003C55 RID: 15445
		private static LiXianBaiTanManager _Instance = new LiXianBaiTanManager();

		// Token: 0x04003C56 RID: 15446
		private static Dictionary<int, LiXianSaleGoodsItem> _LiXianSaleGoodsDict = new Dictionary<int, LiXianSaleGoodsItem>();

		// Token: 0x04003C57 RID: 15447
		private static Dictionary<int, LiXianSaleRoleItem> _LiXianRoleInfoDict = new Dictionary<int, LiXianSaleRoleItem>();
	}
}
