using System;
using System.Collections.Generic;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB.DBController;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.MerlinMagicBook
{
	
	public class MerlinRankManager : IManager
	{
		
		public static MerlinRankManager getInstance()
		{
			return MerlinRankManager.instance;
		}

		
		public bool initialize()
		{
			this.initCmdProcessor();
			this.initData();
			this.initListener();
			return true;
		}

		
		private void initCmdProcessor()
		{
		}

		
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

		
		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, MerlinPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, MerlinPlayerLogoutEventListener.getInstnace());
		}

		
		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, MerlinPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, MerlinPlayerLogoutEventListener.getInstnace());
		}

		
		private void removeData()
		{
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
			this.removeListener();
			this.removeData();
			return true;
		}

		
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

		
		private static MerlinRankManager instance = new MerlinRankManager();

		
		public static readonly int RankingList_Max_Num = 100;

		
		public static readonly int RankingList_PageShowNum = 30;

		
		private List<PlayerMerlinRankingData> rankingDatas = new List<PlayerMerlinRankingData>();

		
		private Dictionary<int, MerlinRankingInfo> playerMerlinDatas = new Dictionary<int, MerlinRankingInfo>();
	}
}
