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
	// Token: 0x0200018B RID: 395
	public class WanMoTaManager : IManager
	{
		// Token: 0x060006EB RID: 1771 RVA: 0x00040DC0 File Offset: 0x0003EFC0
		public static WanMoTaManager getInstance()
		{
			return WanMoTaManager.instance;
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x00040DD8 File Offset: 0x0003EFD8
		public bool initialize()
		{
			this.initCmdProcessor();
			this.initData();
			this.initListener();
			return true;
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x00040E00 File Offset: 0x0003F000
		private void initCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10159, GetWanMoTaoDetailCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10158, ModifyWanMoTaCmdProcessor.getInstance());
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x00040E30 File Offset: 0x0003F030
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

		// Token: 0x060006EF RID: 1775 RVA: 0x00040EBC File Offset: 0x0003F0BC
		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(0, WanMoTaPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().registerListener(1, WanMoTaPlayerLogoutEventListener.getInstnace());
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x00040EE1 File Offset: 0x0003F0E1
		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(0, WanMoTaPlayerLoginEventListener.getInstnace());
			GlobalEventSource.getInstance().removeListener(1, WanMoTaPlayerLogoutEventListener.getInstnace());
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x00040F06 File Offset: 0x0003F106
		private void removeData()
		{
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x00040F0C File Offset: 0x0003F10C
		public bool startup()
		{
			return true;
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x00040F20 File Offset: 0x0003F120
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00040F34 File Offset: 0x0003F134
		public bool destroy()
		{
			this.removeListener();
			this.removeData();
			return true;
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x00040F58 File Offset: 0x0003F158
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

		// Token: 0x060006F6 RID: 1782 RVA: 0x00040FF8 File Offset: 0x0003F1F8
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

		// Token: 0x060006F7 RID: 1783 RVA: 0x000411A0 File Offset: 0x0003F3A0
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

		// Token: 0x060006F8 RID: 1784 RVA: 0x00041238 File Offset: 0x0003F438
		public int updateWanMoTaData(int nRoleID, string[] fields, int startIndex)
		{
			return WanMoTaDBController.updateWanMoTaData(TCPManager.getInstance().DBMgr, nRoleID, fields, 1);
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0004125C File Offset: 0x0003F45C
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

		// Token: 0x060006FA RID: 1786 RVA: 0x000412E0 File Offset: 0x0003F4E0
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

		// Token: 0x060006FB RID: 1787 RVA: 0x00041408 File Offset: 0x0003F608
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

		// Token: 0x060006FC RID: 1788 RVA: 0x00041478 File Offset: 0x0003F678
		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			WanMotaInfo info = this.getWanMoTaData(roleid);
			if (info != null)
			{
				info.strRoleName = newName;
			}
			WanMoTaDBController.getInstance().OnChangeName(roleid, oldName, newName);
		}

		// Token: 0x0400091F RID: 2335
		private static WanMoTaManager instance = new WanMoTaManager();

		// Token: 0x04000920 RID: 2336
		public static readonly int RankingList_Max_Num = 50;

		// Token: 0x04000921 RID: 2337
		public static readonly int RankingList_PageShowNum = 30;

		// Token: 0x04000922 RID: 2338
		private List<PlayerWanMoTaRankingData> rankingDatas = new List<PlayerWanMoTaRankingData>();

		// Token: 0x04000923 RID: 2339
		private Dictionary<int, WanMotaInfo> playerWanMoTaDatas = new Dictionary<int, WanMotaInfo>();
	}
}
