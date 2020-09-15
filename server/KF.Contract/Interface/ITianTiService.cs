using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace KF.Contract.Interface
{
	// Token: 0x0200002C RID: 44
	public interface ITianTiService
	{
		// Token: 0x06000100 RID: 256
		int InitializeClient(IKuaFuClient callback, KuaFuClientContext clientInfo);

		// Token: 0x06000101 RID: 257
		int PushFuBenSeqId(int serverId, List<int> list);

		// Token: 0x06000102 RID: 258
		int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int groupIndex, IGameData gameData);

		// Token: 0x06000103 RID: 259
		int RoleChangeState(int serverId, int roleId, int state);

		// Token: 0x06000104 RID: 260
		int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state);

		// Token: 0x06000105 RID: 261
		int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time);

		// Token: 0x06000106 RID: 262
		KuaFuRoleData GetKuaFuRoleData(int serverId, int roleId);

		// Token: 0x06000107 RID: 263
		IKuaFuFuBenData GetFuBenData(int gameId);

		// Token: 0x06000108 RID: 264
		object GetRoleExtendData(int serverId, int roleId, int dataType);

		// Token: 0x06000109 RID: 265
		AsyncDataItem[] GetClientCacheItems(int serverId);

		// Token: 0x0600010A RID: 266
		List<KuaFuServerInfo> GetKuaFuServerInfoData(int age);

		// Token: 0x0600010B RID: 267
		TianTiRankData GetRankingData(DateTime modifyTime);

		// Token: 0x0600010C RID: 268
		void UpdateRoleInfoData(TianTiRoleInfoData data);
	}
}
