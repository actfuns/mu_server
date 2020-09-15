using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x0200014A RID: 330
	public class MarryPartyDataCache
	{
		// Token: 0x0600059C RID: 1436 RVA: 0x0002FC14 File Offset: 0x0002DE14
		public void LoadPartyList(DBManager dbMgr)
		{
			lock (this.MarryPartyList)
			{
				DBQuery.QueryMarryPartyList(dbMgr, this.MarryPartyList);
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0002FC68 File Offset: 0x0002DE68
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

		// Token: 0x0600059E RID: 1438 RVA: 0x0002FD20 File Offset: 0x0002DF20
		public bool RemoveParty(int roleid)
		{
			bool result;
			lock (this.MarryPartyList)
			{
				result = this.MarryPartyList.Remove(roleid);
			}
			return result;
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0002FD74 File Offset: 0x0002DF74
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

		// Token: 0x060005A0 RID: 1440 RVA: 0x0002FDEC File Offset: 0x0002DFEC
		public TCPOutPacket GetPartyList(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket result;
			lock (this.MarryPartyList)
			{
				result = DataHelper.ObjectToTCPOutPacket<Dictionary<int, MarryPartyData>>(this.MarryPartyList, pool, cmdID);
			}
			return result;
		}

		// Token: 0x04000828 RID: 2088
		private Dictionary<int, MarryPartyData> MarryPartyList = new Dictionary<int, MarryPartyData>();
	}
}
