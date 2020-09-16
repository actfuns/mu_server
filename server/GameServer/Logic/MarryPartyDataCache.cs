using System;
using System.Collections.Generic;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class MarryPartyDataCache
	{
		
		
		
		public Dictionary<int, MarryPartyData> MarryPartyList { private get; set; }

		
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

		
		public void SetPartyTime(MarryPartyData data, long startTime)
		{
			lock (this.MarryPartyList)
			{
				data.StartTime = startTime;
			}
		}

		
		public bool RemoveParty(int roleid)
		{
			bool result;
			lock (this.MarryPartyList)
			{
				result = this.MarryPartyList.Remove(roleid);
			}
			return result;
		}

		
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

		
		public int GetPartyCount()
		{
			int count;
			lock (this.MarryPartyList)
			{
				count = this.MarryPartyList.Count;
			}
			return count;
		}

		
		public TCPOutPacket GetPartyList(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket result;
			lock (this.MarryPartyList)
			{
				result = DataHelper.ObjectToTCPOutPacket<Dictionary<int, MarryPartyData>>(this.MarryPartyList, pool, cmdID);
			}
			return result;
		}

		
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
