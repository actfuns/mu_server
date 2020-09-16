using System;
using System.Collections.Generic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	
	public class KuaFuWorldService : MarshalByRefObject, IKuaFuWorld, IExecCommand
	{
		
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().InitializeClient(clientInfo);
		}

		
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().GetClientCacheItems(serverId);
		}

		
		public void UpdateKuaFuMapClientCount(int serverId, Dictionary<int, int> mapClientCountDict)
		{
			TSingleton<KuaFuWorldManager>.getInstance().UpdateKuaFuMapClientCount(serverId, mapClientCountDict);
		}

		
		public int ExecuteCommand(string cmd)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().ExecuteCommand(cmd);
		}

		
		public int ExecCommand(string[] args)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().ExecCommand(args);
		}

		
		public AsyncDataItem GetKuaFuLineDataList(int mapCode)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().GetKuaFuLineDataList(mapCode);
		}

		
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().GetKuaFuServerInfoData(age);
		}

		
		public int RegPTKuaFuRoleData(ref KuaFuWorldRoleData data)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().RegPTKuaFuRoleData(ref data);
		}

		
		public int EnterPTKuaFuMap(int serverID, int roleId, int ptid, int mapCode, int kuaFuLine, out string signToken, out string signKey, out int kuaFuServerID, out string[] ips, out int[] ports)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().EnterPTKuaFuMap(serverID, roleId, ptid, mapCode, kuaFuLine, out signToken, out signKey, out kuaFuServerID, out ips, out ports);
		}

		
		public int CheckEnterWorldKuaFuSign(string worldRoleID, string token, out string signKey, out string[] ips, out int[] ports)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().CheckEnterWorldKuaFuSign(worldRoleID, token, out signKey, out ips, out ports);
		}

		
		public void Reborn_SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			TSingleton<KuaFuWorldManager>.getInstance().Reborn_SetRoleData4Selector(ptId, roleId, bytes);
		}

		
		public int Reborn_RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().Reborn_RoleReborn(ptId, roleId, roleName, level);
		}

		
		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().Reborn_SyncData(ageRank, ageBoss);
		}

		
		public void Reborn_RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			TSingleton<KuaFuWorldManager>.getInstance().Reborn_RebornOpt(ptid, rid, optType, param1, param2, param3);
		}

		
		public KuaFuCmdData Reborn_GetRebornRoleData(int ptId, int roleId, long dataAge)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().Reborn_GetRebornRoleData(ptId, roleId, dataAge);
		}

		
		public void Reborn_ChangeName(int ptId, int roleId, string roleName)
		{
			TSingleton<KuaFuWorldManager>.getInstance().Reborn_ChangeName(ptId, roleId, roleName);
		}

		
		public void Reborn_PlatFormChat(int serverId, byte[] bytes)
		{
			TSingleton<KuaFuWorldManager>.getInstance().Reborn_PlatFormChat(serverId, bytes);
		}
	}
}
