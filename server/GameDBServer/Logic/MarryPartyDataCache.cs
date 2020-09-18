using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class MarryPartyDataCache
	{
		
		public void LoadPartyList(DBManager dbMgr)
		{
			lock (this.MarryPartyList)
			{
				DBQuery.QueryMarryPartyList(dbMgr, this.MarryPartyList);
			}
		}

		
		public MarryPartyData AddParty(int roleID, int partyType, long startTime, int husbandRoleID, int wifeRoleID, string husbandName, string wifeName)
		{
			MarryPartyData data = null;
			lock (this.MarryPartyList)
			{
				if (!this.MarryPartyList.ContainsKey(roleID))
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

		
		public bool RemoveParty(int roleid)
		{
			bool result;
			lock (this.MarryPartyList)
			{
				result = this.MarryPartyList.Remove(roleid);
			}
			return result;
		}

		
		public bool IncPartyJoin(int roleid)
		{
			MarryPartyData data = null;
			bool result;
			lock (this.MarryPartyList)
			{
				bool ret = this.MarryPartyList.TryGetValue(roleid, out data);
				if (ret)
				{
					data.JoinCount++;
				}
				result = ret;
			}
			return result;
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

		
		private Dictionary<int, MarryPartyData> MarryPartyList = new Dictionary<int, MarryPartyData>();
	}
}
