using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace KF.Contract.Interface
{
	// Token: 0x0200002B RID: 43
	public interface IMoRiJudgeService
	{
		// Token: 0x060000F4 RID: 244
		int InitializeClient(IKuaFuClient callback, KuaFuClientContext clientInfo);

		// Token: 0x060000F5 RID: 245
		int PushFuBenSeqId(int serverId, List<int> list);

		// Token: 0x060000F6 RID: 246
		int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int combat);

		// Token: 0x060000F7 RID: 247
		int RoleChangeState(int serverId, int roleId, int state);

		// Token: 0x060000F8 RID: 248
		int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state);

		// Token: 0x060000F9 RID: 249
		int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time);

		// Token: 0x060000FA RID: 250
		KuaFuRoleData GetKuaFuRoleData(int serverId, int roleId);

		// Token: 0x060000FB RID: 251
		IKuaFuFuBenData GetFuBenData(int gameId);

		// Token: 0x060000FC RID: 252
		object GetRoleExtendData(int serverId, int roleId, int dataType);

		// Token: 0x060000FD RID: 253
		AsyncDataItem[] GetClientCacheItems(int serverId);

		// Token: 0x060000FE RID: 254
		List<KuaFuServerInfo> GetKuaFuServerInfoData(int age);

		// Token: 0x060000FF RID: 255
		void UpdateCopyPassEvent(int gameId, bool passed, DateTime startTime, DateTime endTime, int limitKillCnt, int roleCountWhenFinish, int combatAvg);
	}
}
