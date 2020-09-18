using System;
using System.Collections.Generic;
using GameDBServer.DB.DBController;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class RingPaiHangManager : IManager
	{
		
		public static RingPaiHangManager getInstance()
		{
			return RingPaiHangManager.instance;
		}

		
		public bool initialize()
		{
			this.initData();
			return true;
		}

		
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

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
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

		
		private static RingPaiHangManager instance = new RingPaiHangManager();

		
		public static readonly int RankingList_Max_Num = 100;

		
		public static readonly int RankingList_PageShowNum = 30;

		
		private List<PlayerRingRankingData> rankingDatas = new List<PlayerRingRankingData>();

		
		private Dictionary<int, RingRankingInfo> playerRingDatas = new Dictionary<int, RingRankingInfo>();
	}
}
