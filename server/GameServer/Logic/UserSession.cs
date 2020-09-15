using System;
using System.Collections.Generic;
using System.Linq;
using Server.TCP;

namespace GameServer.Logic
{
	// Token: 0x020007E7 RID: 2023
	public class UserSession
	{
		// Token: 0x0600393A RID: 14650 RVA: 0x00309D60 File Offset: 0x00307F60
		public List<TMSKSocket> GetSocketList()
		{
			List<TMSKSocket> result;
			lock (this)
			{
				result = this._S2UDict.Keys.ToList<TMSKSocket>();
			}
			return result;
		}

		// Token: 0x0600393B RID: 14651 RVA: 0x00309DB4 File Offset: 0x00307FB4
		public bool AddSession(TMSKSocket clientSocket, string userID)
		{
			lock (this)
			{
				string oldUserID = "";
				if (this._S2UDict.TryGetValue(clientSocket, out oldUserID))
				{
					return false;
				}
				TMSKSocket oldClientSocket = null;
				if (this._U2SDict.TryGetValue(userID, out oldClientSocket))
				{
					return false;
				}
				this._S2UDict[clientSocket] = userID;
				this._U2SDict[userID] = clientSocket;
			}
			return true;
		}

		// Token: 0x0600393C RID: 14652 RVA: 0x00309E5C File Offset: 0x0030805C
		public void RemoveSession(TMSKSocket clientSocket)
		{
			if (null != clientSocket)
			{
				string userID = "";
				lock (this)
				{
					if (this._S2UDict.TryGetValue(clientSocket, out userID))
					{
						this._S2UDict.Remove(clientSocket);
						this._U2SDict.Remove(userID);
					}
				}
			}
		}

		// Token: 0x0600393D RID: 14653 RVA: 0x00309EE0 File Offset: 0x003080E0
		public string FindUserID(TMSKSocket clientSocket)
		{
			string userID = "";
			lock (this)
			{
				this._S2UDict.TryGetValue(clientSocket, out userID);
			}
			return userID;
		}

		// Token: 0x0600393E RID: 14654 RVA: 0x00309F3C File Offset: 0x0030813C
		public TMSKSocket FindSocketByUserID(string userID)
		{
			TMSKSocket clientSocket = null;
			lock (this)
			{
				this._U2SDict.TryGetValue(userID, out clientSocket);
			}
			return clientSocket;
		}

		// Token: 0x0600393F RID: 14655 RVA: 0x00309F94 File Offset: 0x00308194
		public void AddUserName(TMSKSocket clientSocket, string userName)
		{
			lock (this)
			{
				this._S2UNameDict[clientSocket] = userName;
				this._UName2SDict[userName] = clientSocket;
			}
		}

		// Token: 0x06003940 RID: 14656 RVA: 0x00309FF0 File Offset: 0x003081F0
		public void RemoveUserName(TMSKSocket clientSocket)
		{
			if (null != clientSocket)
			{
				lock (this)
				{
					string userName = null;
					if (this._S2UNameDict.TryGetValue(clientSocket, out userName))
					{
						this._S2UNameDict.Remove(clientSocket);
						this._UName2SDict.Remove(userName);
					}
				}
			}
		}

		// Token: 0x06003941 RID: 14657 RVA: 0x0030A070 File Offset: 0x00308270
		public string FindUserName(TMSKSocket clientSocket)
		{
			string userName = null;
			lock (this)
			{
				this._S2UNameDict.TryGetValue(clientSocket, out userName);
			}
			return userName;
		}

		// Token: 0x06003942 RID: 14658 RVA: 0x0030A0C8 File Offset: 0x003082C8
		public TMSKSocket FindSocketByUserName(string userName)
		{
			TMSKSocket clientSocket = null;
			lock (this)
			{
				this._UName2SDict.TryGetValue(userName, out clientSocket);
			}
			return clientSocket;
		}

		// Token: 0x06003943 RID: 14659 RVA: 0x0030A120 File Offset: 0x00308320
		public void AddUserAdult(TMSKSocket clientSocket, int isAdult)
		{
			lock (this)
			{
				this._S2UAdultDict[clientSocket] = isAdult;
			}
		}

		// Token: 0x06003944 RID: 14660 RVA: 0x0030A170 File Offset: 0x00308370
		public void RemoveUserAdult(TMSKSocket clientSocket)
		{
			if (null != clientSocket)
			{
				lock (this)
				{
					this._S2UAdultDict.Remove(clientSocket);
				}
			}
		}

		// Token: 0x06003945 RID: 14661 RVA: 0x0030A1CC File Offset: 0x003083CC
		public int FindUserAdult(TMSKSocket clientSocket)
		{
			int isAdult = 0;
			lock (this)
			{
				this._S2UAdultDict.TryGetValue(clientSocket, out isAdult);
			}
			return isAdult;
		}

		// Token: 0x04004329 RID: 17193
		private Dictionary<TMSKSocket, string> _S2UDict = new Dictionary<TMSKSocket, string>(1000);

		// Token: 0x0400432A RID: 17194
		private Dictionary<string, TMSKSocket> _U2SDict = new Dictionary<string, TMSKSocket>(1000);

		// Token: 0x0400432B RID: 17195
		private Dictionary<TMSKSocket, string> _S2UNameDict = new Dictionary<TMSKSocket, string>(1000);

		// Token: 0x0400432C RID: 17196
		private Dictionary<string, TMSKSocket> _UName2SDict = new Dictionary<string, TMSKSocket>(1000);

		// Token: 0x0400432D RID: 17197
		private Dictionary<TMSKSocket, int> _S2UAdultDict = new Dictionary<TMSKSocket, int>(1000);
	}
}
