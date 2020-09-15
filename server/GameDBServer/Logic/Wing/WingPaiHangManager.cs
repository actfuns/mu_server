using System;
using System.Collections.Generic;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB.DBController;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Wing
{
	// Token: 0x0200018E RID: 398
	public class WingPaiHangManager : IManager
	{
		// Token: 0x06000707 RID: 1799 RVA: 0x000415C8 File Offset: 0x0003F7C8
		public static WingPaiHangManager getInstance()
		{
			return WingPaiHangManager.instance;
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x000415E0 File Offset: 0x0003F7E0
		public bool initialize()
		{
			this.initCmdProcessor();
			this.initData();
			this.initListener();
			return true;
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00041608 File Offset: 0x0003F808
		private void initCmdProcessor()
		{
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x0004160C File Offset: 0x0003F80C
		private void initData()
		{
			List<WingRankingInfo> playerWingDataList = WingPaiHangDBController.getInstance().getPlayerWingDataList();
			if (null != playerWingDataList)
			{
				foreach (WingRankingInfo data in playerWingDataList)
				{
					this.playerWingDatas.Add(data.nRoleID, data);
					this.rankingDatas.Add(data.getPlayerWingRankingData());
				}
			}
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x00041698 File Offset: 0x0003F898
		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, WingPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, WingPlayerLogoutEventListener.getInstnace());
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x000416BD File Offset: 0x0003F8BD
		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, WingPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, WingPlayerLogoutEventListener.getInstnace());
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x000416E2 File Offset: 0x0003F8E2
		private void removeData()
		{
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x000416E8 File Offset: 0x0003F8E8
		public bool startup()
		{
			return true;
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x000416FC File Offset: 0x0003F8FC
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x00041710 File Offset: 0x0003F910
		public bool destroy()
		{
			this.removeListener();
			this.removeData();
			return true;
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x00041734 File Offset: 0x0003F934
		public List<PaiHangItemData> getRankingList(int pageIndex, int pageShowNum = -1)
		{
			int maxIndex = Math.Max(pageShowNum, WingPaiHangManager.RankingList_PageShowNum);
			if (maxIndex > this.rankingDatas.Count)
			{
				maxIndex = this.rankingDatas.Count;
			}
			List<PaiHangItemData> _rankingDatas = new List<PaiHangItemData>();
			for (int i = 0; i < maxIndex; i++)
			{
				_rankingDatas.Add(this.rankingDatas[i].getPaiHangItemData());
			}
			return _rankingDatas;
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x000417DC File Offset: 0x0003F9DC
		public void ModifyWingPaihangData(WingRankingInfo data, bool bIsLogin)
		{
			if (null != data)
			{
				lock (this.rankingDatas)
				{
					PlayerWingRankingData result = this.rankingDatas.Find((PlayerWingRankingData paiHang) => paiHang.roleId == data.nRoleID);
					if (null == result)
					{
						if (this.rankingDatas.Count < WingPaiHangManager.RankingList_Max_Num)
						{
							this.rankingDatas.Add(data.getPlayerWingRankingData());
						}
						else if (data.nWingID * 20 + data.nStarNum > this.rankingDatas[this.rankingDatas.Count - 1].WingID * 20 + this.rankingDatas[this.rankingDatas.Count - 1].WingStarNum)
						{
							this.rankingDatas.RemoveAt(this.rankingDatas.Count - 1);
							this.rankingDatas.Add(data.getPlayerWingRankingData());
						}
					}
					else if (!bIsLogin)
					{
						try
						{
							result.UpdateData(data);
							this.rankingDatas.Sort();
						}
						catch (Exception ex)
						{
							DataHelper.WriteFormatExceptionLog(ex, "", false, false);
						}
					}
				}
			}
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x000419B8 File Offset: 0x0003FBB8
		public int createWingData(int nRoleID)
		{
			WingRankingInfo data = null;
			lock (this.playerWingDatas)
			{
				if (this.playerWingDatas.ContainsKey(nRoleID))
				{
					return 0;
				}
				data = this.getWingData(nRoleID);
				if (null != data)
				{
					this.playerWingDatas.Add(data.nRoleID, data);
				}
			}
			if (null != data)
			{
				this.ModifyWingPaihangData(data, false);
			}
			return 1;
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x00041A5C File Offset: 0x0003FC5C
		public WingRankingInfo getWingData(int nRoleID)
		{
			WingRankingInfo data = null;
			lock (this.playerWingDatas)
			{
				if (this.playerWingDatas.TryGetValue(nRoleID, out data))
				{
					return data;
				}
			}
			return WingPaiHangDBController.getInstance().getWingDataById(nRoleID);
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x00041AD4 File Offset: 0x0003FCD4
		public void onPlayerLogin(int roleId, string strRoleName)
		{
			WingRankingInfo data = null;
			lock (this.playerWingDatas)
			{
				if (this.playerWingDatas.TryGetValue(roleId, out data))
				{
					return;
				}
			}
			if (null == data)
			{
				data = WingPaiHangDBController.getInstance().getWingDataById(roleId);
				if (null != data)
				{
					this.ModifyWingPaihangData(data, true);
					lock (this.playerWingDatas)
					{
						this.playerWingDatas.Add(data.nRoleID, data);
					}
				}
			}
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x00041BB4 File Offset: 0x0003FDB4
		public void onPlayerLogout(int roleId)
		{
			WingRankingInfo data = null;
			lock (this.playerWingDatas)
			{
				this.playerWingDatas.TryGetValue(roleId, out data);
				if (null != data)
				{
					this.playerWingDatas.Remove(roleId);
				}
			}
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x00041C24 File Offset: 0x0003FE24
		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			WingRankingInfo info = this.getWingData(roleid);
			if (info != null)
			{
				info.strRoleName = newName;
			}
			WingPaiHangDBController.getInstance().OnChangeName(roleid, oldName, newName);
		}

		// Token: 0x04000926 RID: 2342
		private static WingPaiHangManager instance = new WingPaiHangManager();

		// Token: 0x04000927 RID: 2343
		public static readonly int RankingList_Max_Num = 100;

		// Token: 0x04000928 RID: 2344
		public static readonly int RankingList_PageShowNum = 30;

		// Token: 0x04000929 RID: 2345
		private List<PlayerWingRankingData> rankingDatas = new List<PlayerWingRankingData>();

		// Token: 0x0400092A RID: 2346
		private Dictionary<int, WingRankingInfo> playerWingDatas = new Dictionary<int, WingRankingInfo>();
	}
}
