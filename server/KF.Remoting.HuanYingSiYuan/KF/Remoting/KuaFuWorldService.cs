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
	// Token: 0x02000058 RID: 88
	public class KuaFuWorldService : MarshalByRefObject, IKuaFuWorld, IExecCommand
	{
		// Token: 0x060003FF RID: 1023 RVA: 0x00034104 File Offset: 0x00032304
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().InitializeClient(clientInfo);
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00034124 File Offset: 0x00032324
		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().GetClientCacheItems(serverId);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00034141 File Offset: 0x00032341
		public void UpdateKuaFuMapClientCount(int serverId, Dictionary<int, int> mapClientCountDict)
		{
			TSingleton<KuaFuWorldManager>.getInstance().UpdateKuaFuMapClientCount(serverId, mapClientCountDict);
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00034154 File Offset: 0x00032354
		public int ExecuteCommand(string cmd)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().ExecuteCommand(cmd);
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00034174 File Offset: 0x00032374
		public int ExecCommand(string[] args)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().ExecCommand(args);
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00034194 File Offset: 0x00032394
		public AsyncDataItem GetKuaFuLineDataList(int mapCode)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().GetKuaFuLineDataList(mapCode);
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x000341B4 File Offset: 0x000323B4
		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().GetKuaFuServerInfoData(age);
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x000341D4 File Offset: 0x000323D4
		public int RegPTKuaFuRoleData(ref KuaFuWorldRoleData data)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().RegPTKuaFuRoleData(ref data);
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x000341F4 File Offset: 0x000323F4
		public int EnterPTKuaFuMap(int serverID, int roleId, int ptid, int mapCode, int kuaFuLine, out string signToken, out string signKey, out int kuaFuServerID, out string[] ips, out int[] ports)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().EnterPTKuaFuMap(serverID, roleId, ptid, mapCode, kuaFuLine, out signToken, out signKey, out kuaFuServerID, out ips, out ports);
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00034224 File Offset: 0x00032424
		public int CheckEnterWorldKuaFuSign(string worldRoleID, string token, out string signKey, out string[] ips, out int[] ports)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().CheckEnterWorldKuaFuSign(worldRoleID, token, out signKey, out ips, out ports);
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00034247 File Offset: 0x00032447
		public void Reborn_SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			TSingleton<KuaFuWorldManager>.getInstance().Reborn_SetRoleData4Selector(ptId, roleId, bytes);
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00034258 File Offset: 0x00032458
		public int Reborn_RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().Reborn_RoleReborn(ptId, roleId, roleName, level);
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x0003427C File Offset: 0x0003247C
		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().Reborn_SyncData(ageRank, ageBoss);
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0003429A File Offset: 0x0003249A
		public void Reborn_RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			TSingleton<KuaFuWorldManager>.getInstance().Reborn_RebornOpt(ptid, rid, optType, param1, param2, param3);
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x000342B4 File Offset: 0x000324B4
		public KuaFuCmdData Reborn_GetRebornRoleData(int ptId, int roleId, long dataAge)
		{
			return TSingleton<KuaFuWorldManager>.getInstance().Reborn_GetRebornRoleData(ptId, roleId, dataAge);
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x000342D3 File Offset: 0x000324D3
		public void Reborn_ChangeName(int ptId, int roleId, string roleName)
		{
			TSingleton<KuaFuWorldManager>.getInstance().Reborn_ChangeName(ptId, roleId, roleName);
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x000342E4 File Offset: 0x000324E4
		public void Reborn_PlatFormChat(int serverId, byte[] bytes)
		{
			TSingleton<KuaFuWorldManager>.getInstance().Reborn_PlatFormChat(serverId, bytes);
		}
	}
}
