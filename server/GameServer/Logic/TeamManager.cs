using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;

namespace GameServer.Logic
{
	// Token: 0x020007E3 RID: 2019
	public class TeamManager
	{
		// Token: 0x06003921 RID: 14625 RVA: 0x00309548 File Offset: 0x00307748
		public int GetNextAutoID()
		{
			return (int)(Interlocked.Increment(ref this.BaseAutoID) & 2147483647L);
		}

		// Token: 0x06003922 RID: 14626 RVA: 0x00309570 File Offset: 0x00307770
		public void AddRoleID2TeamID(int roleID, int teamID)
		{
			lock (this._RoleID2TeamIDDict)
			{
				this._RoleID2TeamIDDict[roleID] = teamID;
			}
		}

		// Token: 0x06003923 RID: 14627 RVA: 0x003095C4 File Offset: 0x003077C4
		public void RemoveRoleID2TeamID(int roleID)
		{
			lock (this._RoleID2TeamIDDict)
			{
				if (this._RoleID2TeamIDDict.ContainsKey(roleID))
				{
					this._RoleID2TeamIDDict.Remove(roleID);
				}
			}
		}

		// Token: 0x06003924 RID: 14628 RVA: 0x0030962C File Offset: 0x0030782C
		public int FindRoleID2TeamID(int roleID)
		{
			int teamID = -1;
			lock (this._RoleID2TeamIDDict)
			{
				if (this._RoleID2TeamIDDict.TryGetValue(roleID, out teamID))
				{
					return teamID;
				}
			}
			return teamID;
		}

		// Token: 0x06003925 RID: 14629 RVA: 0x00309698 File Offset: 0x00307898
		public void CreateTeam(TCPManager tcpMgr, TCPOutPacketPool pool, GameClient client)
		{
			int teamID = GameManager.TeamMgr.GetNextAutoID();
			client.ClientData.TeamID = teamID;
			this.AddRoleID2TeamID(client.ClientData.RoleID, client.ClientData.TeamID);
			TeamData td = new TeamData
			{
				TeamID = teamID,
				LeaderRoleID = client.ClientData.RoleID,
				AddDateTime = TimeUtil.NOW(),
				GetThingOpt = 1
			};
			if (null == td.TeamRoles)
			{
				td.TeamRoles = new List<TeamMemberData>();
			}
			td.TeamRoles.Add(Global.ClientDataToTeamMemberData(client.ClientData));
			this.AddData(teamID, td);
			GameManager.ClientMgr.NotifyTeamData(tcpMgr.MySocketListener, pool, td);
			GameManager.ClientMgr.NotifyOthersTeamIDChanged(tcpMgr.MySocketListener, pool, client);
		}

		// Token: 0x06003926 RID: 14630 RVA: 0x00309770 File Offset: 0x00307970
		public void AddData(int teamID, TeamData td)
		{
			lock (this._TeamDataDict)
			{
				this._TeamDataDict[teamID] = td;
			}
		}

		// Token: 0x06003927 RID: 14631 RVA: 0x003097C4 File Offset: 0x003079C4
		public void RemoveData(int teamID)
		{
			lock (this._TeamDataDict)
			{
				if (this._TeamDataDict.ContainsKey(teamID))
				{
					this._TeamDataDict.Remove(teamID);
				}
			}
		}

		// Token: 0x06003928 RID: 14632 RVA: 0x0030982C File Offset: 0x00307A2C
		public TeamData FindData(int teamID)
		{
			TeamData td = null;
			lock (this._TeamDataDict)
			{
				this._TeamDataDict.TryGetValue(teamID, out td);
			}
			return td;
		}

		// Token: 0x06003929 RID: 14633 RVA: 0x0030988C File Offset: 0x00307A8C
		public int GetTotalDataCount()
		{
			int count = 0;
			lock (this._TeamDataDict)
			{
				count = this._TeamDataDict.Count;
			}
			return count;
		}

		// Token: 0x0600392A RID: 14634 RVA: 0x003098E8 File Offset: 0x00307AE8
		public List<TeamData> GetTeamDataList(int startIndex, int count)
		{
			int index = 0;
			List<TeamData> teamDataList = new List<TeamData>();
			lock (this._TeamDataDict)
			{
				foreach (TeamData teamData in this._TeamDataDict.Values)
				{
					if (index < startIndex)
					{
						index++;
					}
					else
					{
						teamDataList.Add(teamData);
						if (teamDataList.Count >= count)
						{
							break;
						}
						index++;
					}
				}
			}
			return teamDataList;
		}

		// Token: 0x0600392B RID: 14635 RVA: 0x003099BC File Offset: 0x00307BBC
		public bool CanAddToTeam(int roleID, int teamID, int requestType)
		{
			TeamRequestItem teamRequestItem = null;
			string key = string.Format("{0}_{1}_{2}", roleID, teamID, requestType);
			lock (this._TeamRequestDict)
			{
				if (!this._TeamRequestDict.TryGetValue(key, out teamRequestItem))
				{
					return true;
				}
			}
			long ticks = TimeUtil.NOW();
			return ticks - teamRequestItem.RequestTicks >= 35000L;
		}

		// Token: 0x0600392C RID: 14636 RVA: 0x00309A68 File Offset: 0x00307C68
		public void AddTeamRequestItem(int roleID, int teamID, int requestType)
		{
			string key = string.Format("{0}_{1}_{2}", roleID, teamID, requestType);
			TeamRequestItem teamRequestItem = new TeamRequestItem
			{
				ToRoleID = roleID,
				RequestTicks = TimeUtil.NOW()
			};
			lock (this._TeamRequestDict)
			{
				this._TeamRequestDict[key] = teamRequestItem;
			}
		}

		// Token: 0x0600392D RID: 14637 RVA: 0x00309AF8 File Offset: 0x00307CF8
		public void RemoveTeamRequestItem(int roleID, int teamID, int requestType)
		{
			string key = string.Format("{0}_{1}_{2}", roleID, teamID, requestType);
			lock (this._TeamRequestDict)
			{
				this._TeamRequestDict.Remove(key);
			}
		}

		// Token: 0x04004320 RID: 17184
		private long BaseAutoID = 0L;

		// Token: 0x04004321 RID: 17185
		private Dictionary<int, int> _RoleID2TeamIDDict = new Dictionary<int, int>();

		// Token: 0x04004322 RID: 17186
		private Dictionary<int, TeamData> _TeamDataDict = new Dictionary<int, TeamData>();

		// Token: 0x04004323 RID: 17187
		private Dictionary<string, TeamRequestItem> _TeamRequestDict = new Dictionary<string, TeamRequestItem>();
	}
}
