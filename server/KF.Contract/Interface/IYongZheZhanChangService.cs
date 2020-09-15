using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace KF.Contract.Interface
{
	// Token: 0x0200002D RID: 45
	public interface IYongZheZhanChangService
	{
		// Token: 0x0600010D RID: 269
		int InitializeClient(IKuaFuClient callback, KuaFuClientContext clientInfo);

		// Token: 0x0600010E RID: 270
		int PushFuBenSeqId(int serverId, List<int> list);

		// Token: 0x0600010F RID: 271
		int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int groupIndex, IGameData gameData);

		// Token: 0x06000110 RID: 272
		int RoleChangeState(int serverId, int roleId, int state);

		// Token: 0x06000111 RID: 273
		int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state);

		// Token: 0x06000112 RID: 274
		int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time);

		// Token: 0x06000113 RID: 275
		KuaFuRoleData GetKuaFuRoleData(int serverId, int roleId);

		// Token: 0x06000114 RID: 276
		IKuaFuFuBenData GetFuBenData(int gameId);

		// Token: 0x06000115 RID: 277
		object GetRoleExtendData(int serverId, int roleId, int dataType);

		// Token: 0x06000116 RID: 278
		AsyncDataItem[] GetClientCacheItems(int serverId);

		// Token: 0x06000117 RID: 279
		List<KuaFuServerInfo> GetKuaFuServerInfoData(int age);

		// Token: 0x06000118 RID: 280
		int ExecuteCommand(string cmd);

		// Token: 0x06000119 RID: 281
		void UpdateStatisticalData(YongZheZhanChangStatisticalData data);
	}
}
