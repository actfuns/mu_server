using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace KF.Contract.Interface
{
	// Token: 0x02000027 RID: 39
	public interface IElementWarService
	{
		// Token: 0x060000D8 RID: 216
		int InitializeClient(IKuaFuClient callback, KuaFuClientContext clientInfo);

		// Token: 0x060000D9 RID: 217
		int PushFuBenSeqId(int serverId, List<int> list);

		// Token: 0x060000DA RID: 218
		int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int zhanDouLi);

		// Token: 0x060000DB RID: 219
		int RoleChangeState(int serverId, int roleId, int state);

		// Token: 0x060000DC RID: 220
		int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state);

		// Token: 0x060000DD RID: 221
		int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time);

		// Token: 0x060000DE RID: 222
		KuaFuRoleData GetKuaFuRoleData(int serverId, int roleId);

		// Token: 0x060000DF RID: 223
		IKuaFuFuBenData GetFuBenData(int gameId);

		// Token: 0x060000E0 RID: 224
		AsyncDataItem[] GetClientCacheItems(int serverId);

		// Token: 0x060000E1 RID: 225
		List<KuaFuServerInfo> GetKuaFuServerInfoData(int age);

		// Token: 0x060000E2 RID: 226
		void UpdateCopyPassEvent(int seqId, int roleCount, int wave, int zhanLi);
	}
}
