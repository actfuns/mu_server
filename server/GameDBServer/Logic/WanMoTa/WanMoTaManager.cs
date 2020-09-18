using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.GameEvent;
using GameDBServer.DB.DBController;
using GameDBServer.Server;
using GameDBServer.Server.CmdProcessor;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.WanMoTa
{
	
	public class WanMoTaManager : IManager
	{
		
		public static WanMoTaManager getInstance()
		{
			return WanMoTaManager.instance;
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
			TCPCmdDispatcher.getInstance().registerProcessor(10159, GetWanMoTaoDetailCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10158, ModifyWanMoTaCmdProcessor.getInstance());
		}

		
		private void initData()
		{
			List<WanMotaInfo> playerWanMoTaDataList = WanMoTaDBController.getInstance().getPlayerWanMotaDataList();
			if (null != playerWanMoTaDataList)
			{
				foreach (WanMotaInfo data in playerWanMoTaDataList)
				{
					this.playerWanMoTaDatas.Add(data.nRoleID, data);
					this.rankingDatas.Add(data.getPlayerWanMoTaRankingData());
				}
			}
		}

		
		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, WanMoTaPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, WanMoTaPlayerLogoutEventListener.getInstnace());
		}

		
		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, WanMoTaPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, WanMoTaPlayerLogoutEventListener.getInstnace());
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

		
		public List<PaiHangItemData> getRankingList(int pageIndex)
		{
			int maxIndex = WanMoTaManager.RankingList_PageShowNum;
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

		
		public void ModifyWanMoTaPaihangData(WanMotaInfo data, bool bIsLogin)
		{
			if (null != data)
			{
				lock (this.rankingDatas)
				{
					PlayerWanMoTaRankingData result = this.rankingDatas.Find((PlayerWanMoTaRankingData paiHang) => paiHang.roleId == data.nRoleID);
					if (null == result)
					{
						if (this.rankingDatas.Count < WanMoTaManager.RankingList_Max_Num)
						{
							this.rankingDatas.Add(data.getPlayerWanMoTaRankingData());
						}
						else if (data.nPassLayerCount > this.rankingDatas[this.rankingDatas.Count - 1].passLayerCount)
						{
							this.rankingDatas.RemoveAt(this.rankingDatas.Count - 1);
							this.rankingDatas.Add(data.getPlayerWanMoTaRankingData());
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

		
		public int createWanMoTaData(WanMotaInfo data)
		{
			lock (this.playerWanMoTaDatas)
			{
				if (this.playerWanMoTaDatas.ContainsKey(data.nRoleID))
				{
					return 0;
				}
				this.playerWanMoTaDatas.Add(data.nRoleID, data);
			}
			this.ModifyWanMoTaPaihangData(data, true);
			return WanMoTaDBController.getInstance().insertWanMoTaData(TCPManager.getInstance().DBMgr, data);
		}

		
		public int updateWanMoTaData(int nRoleID, string[] fields, int startIndex)
		{
			return WanMoTaDBController.updateWanMoTaData(TCPManager.getInstance().DBMgr, nRoleID, fields, 1);
		}

		
		public WanMotaInfo getWanMoTaData(int nRoleID)
		{
			WanMotaInfo data = null;
			lock (this.playerWanMoTaDatas)
			{
				if (this.playerWanMoTaDatas.TryGetValue(nRoleID, out data))
				{
					data.nTopPassLayerCount = this.rankingDatas[0].passLayerCount;
					return data;
				}
			}
			return data;
		}

		
		public void onPlayerLogin(int roleId, string strRoleName)
		{
			WanMotaInfo data = null;
			lock (this.playerWanMoTaDatas)
			{
				if (this.playerWanMoTaDatas.TryGetValue(roleId, out data))
				{
					if (null != data)
					{
						return;
					}
				}
			}
			if (null == data)
			{
				data = WanMoTaDBController.getInstance().getPlayerWanMoTaDataById(roleId);
				if (null == data)
				{
					this.createWanMoTaData(new WanMotaInfo
					{
						nRoleID = roleId,
						strRoleName = strRoleName,
						lFlushTime = TimeUtil.NOW(),
						nSweepLayer = -1
					});
				}
				else
				{
					this.ModifyWanMoTaPaihangData(data, true);
					lock (this.playerWanMoTaDatas)
					{
						this.playerWanMoTaDatas.Add(data.nRoleID, data);
					}
				}
			}
		}

		
		public void onPlayerLogout(int roleId)
		{
			WanMotaInfo data = null;
			lock (this.playerWanMoTaDatas)
			{
				this.playerWanMoTaDatas.TryGetValue(roleId, out data);
				if (null != data)
				{
					this.playerWanMoTaDatas.Remove(roleId);
				}
			}
		}

		
		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			WanMotaInfo info = this.getWanMoTaData(roleid);
			if (info != null)
			{
				info.strRoleName = newName;
			}
			WanMoTaDBController.getInstance().OnChangeName(roleid, oldName, newName);
		}

		
		private static WanMoTaManager instance = new WanMoTaManager();

		
		public static readonly int RankingList_Max_Num = 50;

		
		public static readonly int RankingList_PageShowNum = 30;

		
		private List<PlayerWanMoTaRankingData> rankingDatas = new List<PlayerWanMoTaRankingData>();

		
		private Dictionary<int, WanMotaInfo> playerWanMoTaDatas = new Dictionary<int, WanMotaInfo>();
	}
}
