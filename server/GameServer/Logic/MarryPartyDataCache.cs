using System;
using System.Collections.Generic;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200052D RID: 1325
	public class MarryPartyDataCache
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06001935 RID: 6453 RVA: 0x00189930 File Offset: 0x00187B30
		// (set) Token: 0x06001936 RID: 6454 RVA: 0x00189947 File Offset: 0x00187B47
		public Dictionary<int, MarryPartyData> MarryPartyList { private get; set; }

		// Token: 0x06001937 RID: 6455 RVA: 0x00189950 File Offset: 0x00187B50
		public MarryPartyData AddParty(int roleID, int partyType, long startTime, int husbandRoleID, int wifeRoleID, string husbandName, string wifeName)
		{
			MarryPartyData data = null;
			lock (this.MarryPartyList)
			{
				if (!this.MarryPartyList.ContainsKey(husbandRoleID) && !this.MarryPartyList.ContainsKey(wifeRoleID))
				{
					data = new MarryPartyData
					{
						RoleID = roleID,
						PartyType = partyType,
						JoinCount = 0,
						StartTime = startTime,
						HusbandRoleID = husbandRoleID,
						WifeRoleID = wifeRoleID,
						HusbandName = husbandName,
						WifeName = wifeName
					};
					this.MarryPartyList.Add(roleID, data);
				}
			}
			return data;
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x00189A1C File Offset: 0x00187C1C
		public void SetPartyTime(MarryPartyData data, long startTime)
		{
			lock (this.MarryPartyList)
			{
				data.StartTime = startTime;
			}
		}

		// Token: 0x06001939 RID: 6457 RVA: 0x00189A68 File Offset: 0x00187C68
		public bool RemoveParty(int roleid)
		{
			bool result;
			lock (this.MarryPartyList)
			{
				result = this.MarryPartyList.Remove(roleid);
			}
			return result;
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x00189ABC File Offset: 0x00187CBC
		public void RemovePartyCancel(MarryPartyData partyData)
		{
			lock (this.MarryPartyList)
			{
				try
				{
					this.MarryPartyList.Add(partyData.RoleID, partyData);
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x00189B2C File Offset: 0x00187D2C
		public bool IncPartyJoin(int roleid, int maxJoin, out bool remove)
		{
			remove = false;
			MarryPartyData data = null;
			bool result;
			lock (this.MarryPartyList)
			{
				bool ret = this.MarryPartyList.TryGetValue(roleid, out data);
				if (ret)
				{
					if (data.JoinCount < maxJoin)
					{
						data.JoinCount++;
						if (data.JoinCount == maxJoin)
						{
							remove = true;
						}
					}
					else
					{
						ret = false;
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x00189BD8 File Offset: 0x00187DD8
		public void IncPartyJoinCancel(int roleid)
		{
			MarryPartyData data = null;
			lock (this.MarryPartyList)
			{
				if (this.MarryPartyList.TryGetValue(roleid, out data))
				{
					data.JoinCount--;
				}
			}
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x00189C44 File Offset: 0x00187E44
		public MarryPartyData GetParty(int roleid)
		{
			MarryPartyData data = null;
			MarryPartyData result;
			lock (this.MarryPartyList)
			{
				this.MarryPartyList.TryGetValue(roleid, out data);
				result = data;
			}
			return result;
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x00189CA0 File Offset: 0x00187EA0
		public int GetPartyCount()
		{
			int count;
			lock (this.MarryPartyList)
			{
				count = this.MarryPartyList.Count;
			}
			return count;
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x00189CF4 File Offset: 0x00187EF4
		public TCPOutPacket GetPartyList(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket result;
			lock (this.MarryPartyList)
			{
				result = DataHelper.ObjectToTCPOutPacket<Dictionary<int, MarryPartyData>>(this.MarryPartyList, pool, cmdID);
			}
			return result;
		}

		// Token: 0x06001940 RID: 6464 RVA: 0x00189D48 File Offset: 0x00187F48
		public bool HasPartyStarted(long ticks)
		{
			bool showNPC = false;
			lock (this.MarryPartyList)
			{
				foreach (KeyValuePair<int, MarryPartyData> kv in this.MarryPartyList)
				{
					if (ticks > kv.Value.StartTime)
					{
						showNPC = true;
						break;
					}
				}
			}
			return showNPC;
		}

		// Token: 0x06001941 RID: 6465 RVA: 0x00189DFC File Offset: 0x00187FFC
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				SafeClientData clientData = Global.GetSafeClientDataFromLocalOrDB(roleId);
				if (clientData != null && clientData.MyMarriageData != null && clientData.MyMarriageData.nSpouseID != -1)
				{
					lock (this.MarryPartyList)
					{
						MarryPartyData data = null;
						this.MarryPartyList.TryGetValue(clientData.RoleID, out data);
						if (data != null)
						{
							if (!string.IsNullOrEmpty(data.HusbandName) && data.HusbandName == oldName)
							{
								data.HusbandName = newName;
							}
							else if (!string.IsNullOrEmpty(data.WifeName) && data.WifeName == oldName)
							{
								data.WifeName = newName;
							}
						}
						data = null;
						this.MarryPartyList.TryGetValue(clientData.MyMarriageData.nSpouseID, out data);
						if (data != null)
						{
							if (!string.IsNullOrEmpty(data.HusbandName) && data.HusbandName == oldName)
							{
								data.HusbandName = newName;
							}
							else if (!string.IsNullOrEmpty(data.WifeName) && data.WifeName == oldName)
							{
								data.WifeName = newName;
							}
						}
					}
				}
			}
		}
	}
}
