using System;
using System.Collections.Generic;
using GameDBServer.DB.DBController;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000168 RID: 360
	public class RingPaiHangManager : IManager
	{
		// Token: 0x06000637 RID: 1591 RVA: 0x00038BC0 File Offset: 0x00036DC0
		public static RingPaiHangManager getInstance()
		{
			return RingPaiHangManager.instance;
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00038BD8 File Offset: 0x00036DD8
		public bool initialize()
		{
			this.initData();
			return true;
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00038BF4 File Offset: 0x00036DF4
		private void initData()
		{
			List<RingRankingInfo> playerRingDataList = RingPaiHangDBController.getInstance().getPlayerRingDataList();
			if (null != playerRingDataList)
			{
				foreach (RingRankingInfo data in playerRingDataList)
				{
					this.playerRingDatas.Add(data.nRoleID, data);
					this.rankingDatas.Add(data.getPlayerRingRankingData());
				}
			}
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x00038C80 File Offset: 0x00036E80
		public bool startup()
		{
			return true;
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00038C94 File Offset: 0x00036E94
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00038CA8 File Offset: 0x00036EA8
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00038CBC File Offset: 0x00036EBC
		public List<PaiHangItemData> getRankingList(int pageIndex, int pageShowNum = -1)
		{
			int maxIndex = Math.Max(pageShowNum, RingPaiHangManager.RankingList_PageShowNum);
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

		// Token: 0x0600063E RID: 1598 RVA: 0x00038D64 File Offset: 0x00036F64
		public void ModifyRingPaihangData(RingRankingInfo data)
		{
			if (null != data)
			{
				lock (this.rankingDatas)
				{
					PlayerRingRankingData result = this.rankingDatas.Find((PlayerRingRankingData paiHang) => paiHang.roleId == data.nRoleID);
					if (null == result)
					{
						if (this.rankingDatas.Count < RingPaiHangManager.RankingList_Max_Num)
						{
							this.rankingDatas.Add(data.getPlayerRingRankingData());
						}
						else if (this.CompareTo(data.getPlayerRingRankingData(), this.rankingDatas[this.rankingDatas.Count - 1]) < 1)
						{
							this.rankingDatas.RemoveAt(this.rankingDatas.Count - 1);
							this.rankingDatas.Add(data.getPlayerRingRankingData());
						}
					}
					else
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

		// Token: 0x0600063F RID: 1599 RVA: 0x00038F08 File Offset: 0x00037108
		public int createRingData(int nRoleID, RingRankingInfo data = null)
		{
			lock (this.playerRingDatas)
			{
				if (data == null)
				{
					data = this.getRingData(nRoleID);
				}
				if (data != null && !this.playerRingDatas.ContainsKey(nRoleID))
				{
					this.playerRingDatas.Add(data.nRoleID, data);
				}
			}
			if (null != data)
			{
				this.ModifyRingPaihangData(data);
			}
			return 1;
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00038FA4 File Offset: 0x000371A4
		public RingRankingInfo getRingData(int nRoleID)
		{
			RingRankingInfo data = null;
			lock (this.playerRingDatas)
			{
				if (this.playerRingDatas.TryGetValue(nRoleID, out data))
				{
					return data;
				}
			}
			return RingPaiHangDBController.getInstance().getRingDataById(nRoleID);
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x0003901C File Offset: 0x0003721C
		private int CompareTo(PlayerRingRankingData A, PlayerRingRankingData B)
		{
			int result;
			if (A.GoodWillLevel == B.GoodWillLevel)
			{
				if (A.GoodWillStar == B.GoodWillStar)
				{
					int nRet = string.Compare(A.RingAddTime, B.RingAddTime);
					result = ((nRet < 0) ? -1 : ((nRet == 0) ? 0 : 1));
				}
				else
				{
					result = ((A.GoodWillStar < B.GoodWillStar) ? 1 : -1);
				}
			}
			else
			{
				result = ((A.GoodWillLevel < B.GoodWillLevel) ? 1 : -1);
			}
			return result;
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x000390AC File Offset: 0x000372AC
		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			RingRankingInfo data = null;
			lock (this.playerRingDatas)
			{
				this.playerRingDatas.TryGetValue(roleid, out data);
			}
			if (data != null)
			{
				data.strRoleName = newName;
			}
		}

		// Token: 0x04000880 RID: 2176
		private static RingPaiHangManager instance = new RingPaiHangManager();

		// Token: 0x04000881 RID: 2177
		public static readonly int RankingList_Max_Num = 100;

		// Token: 0x04000882 RID: 2178
		public static readonly int RankingList_PageShowNum = 30;

		// Token: 0x04000883 RID: 2179
		private List<PlayerRingRankingData> rankingDatas = new List<PlayerRingRankingData>();

		// Token: 0x04000884 RID: 2180
		private Dictionary<int, RingRankingInfo> playerRingDatas = new Dictionary<int, RingRankingInfo>();
	}
}
