using System;
using System.Collections.Generic;
using KF.Client;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class KuaFuWorldManager : SingletonTemplate<KuaFuWorldManager>
	{
		
		public bool IsTempRoleID(int roleID)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.TempRoleIDs.Contains(roleID);
			}
			return result;
		}

		
		public bool CheckPTKuaFuLoginSign(KuaFuServerLoginData data)
		{
			string signKey = null;
			string worldRoleID = ConstData.FormatWorldRoleID(data.RoleId, data.PTID);
			string[] ips;
			int[] ports;
			int result = KuaFuWorldClient.getInstance().CheckEnterWorldKuaFuSign(worldRoleID, data.SignToken, out signKey, out ips, out ports);
			bool result2;
			if (result < 0)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("CheckEnterWorldKuaFuSign faild,roleid={0},ptid={1},result={2}", data.RoleId, data.PTID, result), null, true);
				result2 = false;
			}
			else
			{
				string kfsign = MD5Helper.get_md5_string(data.SignDataString() + signKey).ToLower();
				if (kfsign != data.SignCode)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("CheckEnterWorldKuaFuSign SignCode Error,roleid={0},ptid={1},SignCode={2}", data.RoleId, data.PTID, data.SignCode), null, true);
					result2 = false;
				}
				else
				{
					data.ips = ips;
					data.ports = ports;
					result2 = true;
				}
			}
			return result2;
		}

		
		public KuaFuWorldRoleData GetWorldRoleData(int roleID, int serverID, string userID, int tempRoleID)
		{
			string channel = Data.GetChannelNameByUserID(userID);
			KuaFuWorldRoleData data = null;
			lock (this.Mutex)
			{
				if (roleID != tempRoleID)
				{
					this.TempRoleIDs.Add(tempRoleID);
				}
				if (!this.WorldRoleDataDict.TryGetValue(roleID, out data))
				{
					data = new KuaFuWorldRoleData();
					data.LocalRoleID = roleID;
					data.TempRoleID = roleID;
					data.UserID = userID;
					data.ServerID = serverID;
					data.Channel = channel;
					this.WorldRoleDataDict[roleID] = data;
				}
				else
				{
					if (data.TempRoleID == tempRoleID)
					{
						data.ServerID = serverID;
						return data;
					}
					data = new KuaFuWorldRoleData();
					data.LocalRoleID = roleID;
					data.TempRoleID = tempRoleID;
					data.UserID = userID;
					data.ServerID = serverID;
					data.Channel = channel;
					data.UseTempRoleID = true;
				}
			}
			return data;
		}

		
		private object Mutex = new object();

		
		private Dictionary<int, KuaFuWorldRoleData> WorldRoleDataDict = new Dictionary<int, KuaFuWorldRoleData>();

		
		private HashSet<int> TempRoleIDs = new HashSet<int>();
	}
}
