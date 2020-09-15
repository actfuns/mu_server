using System;
using System.Collections.Generic;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB.DBController;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.MerlinMagicBook
{
	// Token: 0x02000154 RID: 340
	public class MerlinRankManager : IManager
	{
		// Token: 0x060005BD RID: 1469 RVA: 0x000327E8 File Offset: 0x000309E8
		public static MerlinRankManager getInstance()
		{
			return MerlinRankManager.instance;
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x00032800 File Offset: 0x00030A00
		public bool initialize()
		{
			this.initCmdProcessor();
			this.initData();
			this.initListener();
			return true;
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00032828 File Offset: 0x00030A28
		private void initCmdProcessor()
		{
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x0003282C File Offset: 0x00030A2C
		private void initData()
		{
			List<MerlinRankingInfo> playerMerlinDataList = MerlinRankDBController.getInstance().getPlayerMerlinDataList();
			if (null != playerMerlinDataList)
			{
				foreach (MerlinRankingInfo data in playerMerlinDataList)
				{
					this.playerMerlinDatas.Add(data.nRoleID, data);
					this.rankingDatas.Add(data.getPlayerMerlinRankingData());
				}
			}
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x000328B8 File Offset: 0x00030AB8
		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, MerlinPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, MerlinPlayerLogoutEventListener.getInstnace());
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x000328DD File Offset: 0x00030ADD
		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, MerlinPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, MerlinPlayerLogoutEventListener.getInstnace());
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x00032902 File Offset: 0x00030B02
		private void removeData()
		{
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00032908 File Offset: 0x00030B08
		public bool startup()
		{
			return true;
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x0003291C File Offset: 0x00030B1C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x00032930 File Offset: 0x00030B30
		public bool destroy()
		{
			this.removeListener();
			this.removeData();
			return true;
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x00032954 File Offset: 0x00030B54
		public List<PaiHangItemData> getRankingList(int pageIndex, int pageShowNum = -1)
		{
			int maxIndex = Math.Max(pageShowNum, MerlinRankManager.RankingList_PageShowNum);
			if (maxIndex > this.rankingDatas.Count)
			{
				maxIndex = this.rankingDatas.Count;
			}
			List<PaiHangItemData> _rankingDatas = new List<PaiHangItemData>();
			lock (this.rankingDatas)
			{
				for (int i = 0; i < maxIndex; i++)
				{
					_rankingDatas.Add(this.rankingDatas[i].getPaiHangItemData());
				}
			}
			return _rankingDatas;
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x00032A38 File Offset: 0x00030C38
		public void ModifyMerlinRankData(MerlinRankingInfo data, bool bIsLogin)
		{
			if (null != data)
			{
				lock (this.rankingDatas)
				{
					PlayerMerlinRankingData result = this.rankingDatas.Find((PlayerMerlinRankingData paiHang) => paiHang.roleId == data.nRoleID);
					if (null == result)
					{
						if (this.rankingDatas.Count < MerlinRankManager.RankingList_Max_Num)
						{
							this.rankingDatas.Add(data.getPlayerMerlinRankingData());
						}
						else if (data.nLevel * 20 + data.nStarNum > this.rankingDatas[this.rankingDatas.Count - 1].Level * 20 + this.rankingDatas[this.rankingDatas.Count - 1].StarNum)
						{
							this.rankingDatas.RemoveAt(this.rankingDatas.Count - 1);
							this.rankingDatas.Add(data.getPlayerMerlinRankingData());
						}
						try
						{
							this.rankingDatas.Sort();
						}
						catch (Exception ex)
						{
							DataHelper.WriteFormatExceptionLog(ex, "", false, false);
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

		// Token: 0x060005C9 RID: 1481 RVA: 0x00032C50 File Offset: 0x00030E50
		public int createMerlinData(int nRoleID)
		{
			MerlinRankingInfo data = null;
			lock (this.playerMerlinDatas)
			{
				if (this.playerMerlinDatas.ContainsKey(nRoleID))
				{
					return 0;
				}
				data = this.getMerlinData(nRoleID);
				if (null != data)
				{
					this.playerMerlinDatas.Add(data.nRoleID, data);
				}
			}
			if (null != data)
			{
				this.ModifyMerlinRankData(data, false);
			}
			return 1;
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00032CF4 File Offset: 0x00030EF4
		public MerlinRankingInfo getMerlinData(int nRoleID)
		{
			MerlinRankingInfo data = null;
			lock (this.playerMerlinDatas)
			{
				if (this.playerMerlinDatas.TryGetValue(nRoleID, out data))
				{
					return data;
				}
			}
			return MerlinRankDBController.getInstance().getMerlinDataByRoleID(nRoleID);
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x00032D6C File Offset: 0x00030F6C
		public void onPlayerLogin(int roleId, string strRoleName)
		{
			MerlinRankingInfo data = null;
			lock (this.playerMerlinDatas)
			{
				if (this.playerMerlinDatas.TryGetValue(roleId, out data))
				{
					return;
				}
			}
			if (null == data)
			{
				data = MerlinRankDBController.getInstance().getMerlinDataByRoleID(roleId);
				if (null != data)
				{
					this.ModifyMerlinRankData(data, true);
					lock (this.playerMerlinDatas)
					{
						this.playerMerlinDatas.Add(data.nRoleID, data);
					}
				}
			}
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x00032E4C File Offset: 0x0003104C
		public void onPlayerLogout(int roleId)
		{
			MerlinRankingInfo data = null;
			lock (this.playerMerlinDatas)
			{
				this.playerMerlinDatas.TryGetValue(roleId, out data);
				if (null != data)
				{
					this.playerMerlinDatas.Remove(roleId);
				}
			}
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00032EBC File Offset: 0x000310BC
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				lock (this.playerMerlinDatas)
				{
					MerlinRankingInfo data = null;
					if (this.playerMerlinDatas.TryGetValue(roleId, out data))
					{
						data.strRoleName = newName;
					}
				}
			}
		}

		// Token: 0x0400084D RID: 2125
		private static MerlinRankManager instance = new MerlinRankManager();

		// Token: 0x0400084E RID: 2126
		public static readonly int RankingList_Max_Num = 100;

		// Token: 0x0400084F RID: 2127
		public static readonly int RankingList_PageShowNum = 30;

		// Token: 0x04000850 RID: 2128
		private List<PlayerMerlinRankingData> rankingDatas = new List<PlayerMerlinRankingData>();

		// Token: 0x04000851 RID: 2129
		private Dictionary<int, MerlinRankingInfo> playerMerlinDatas = new Dictionary<int, MerlinRankingInfo>();
	}
}
