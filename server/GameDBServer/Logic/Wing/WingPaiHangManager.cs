using System;
using System.Collections.Generic;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB.DBController;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Wing
{
	
	public class WingPaiHangManager : IManager
	{
		
		public static WingPaiHangManager getInstance()
		{
			return WingPaiHangManager.instance;
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

		
		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, WingPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, WingPlayerLogoutEventListener.getInstnace());
		}

		
		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, WingPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, WingPlayerLogoutEventListener.getInstnace());
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

		
		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			WingRankingInfo info = this.getWingData(roleid);
			if (info != null)
			{
				info.strRoleName = newName;
			}
			WingPaiHangDBController.getInstance().OnChangeName(roleid, oldName, newName);
		}

		
		private static WingPaiHangManager instance = new WingPaiHangManager();

		
		public static readonly int RankingList_Max_Num = 100;

		
		public static readonly int RankingList_PageShowNum = 30;

		
		private List<PlayerWingRankingData> rankingDatas = new List<PlayerWingRankingData>();

		
		private Dictionary<int, WingRankingInfo> playerWingDatas = new Dictionary<int, WingRankingInfo>();
	}
}
