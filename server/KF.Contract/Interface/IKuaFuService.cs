using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace KF.Contract.Interface
{
	// Token: 0x0200002A RID: 42
	public interface IKuaFuService
	{
		// Token: 0x060000E9 RID: 233
		int InitializeClient(IKuaFuClient callback, KuaFuClientContext clientInfo);

		// Token: 0x060000EA RID: 234
		int PushFuBenSeqId(int serverId, List<int> list);

		// Token: 0x060000EB RID: 235
		int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int groupIndex, IGameData gameData);

		// Token: 0x060000EC RID: 236
		int RoleChangeState(int serverId, int roleId, int state);

		// Token: 0x060000ED RID: 237
		int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state);

		// Token: 0x060000EE RID: 238
		int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time);

		// Token: 0x060000EF RID: 239
		KuaFuRoleData GetKuaFuRoleData(int serverId, int roleId);

		// Token: 0x060000F0 RID: 240
		IKuaFuFuBenData GetFuBenData(int gameId);

		// Token: 0x060000F1 RID: 241
		object GetRoleExtendData(int serverId, int roleId, int dataType);

		// Token: 0x060000F2 RID: 242
		AsyncDataItem[] GetClientCacheItems(int serverId);

		// Token: 0x060000F3 RID: 243
		List<KuaFuServerInfo> GetKuaFuServerInfoData(int age);
	}
}
