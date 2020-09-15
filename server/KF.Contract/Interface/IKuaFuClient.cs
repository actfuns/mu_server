using System;
using KF.Contract.Data;

namespace KF.Contract.Interface
{
	// Token: 0x02000029 RID: 41
	public interface IKuaFuClient
	{
		// Token: 0x060000E4 RID: 228
		void EventCallBackHandler(int eventType, params object[] args);

		// Token: 0x060000E5 RID: 229
		object GetDataFromClientServer(int dataType, params object[] args);

		// Token: 0x060000E6 RID: 230
		int GetNewFuBenSeqId();

		// Token: 0x060000E7 RID: 231
		int UpdateRoleData(KuaFuRoleData kuaFuRoleData, int roleId = 0);

		// Token: 0x060000E8 RID: 232
		int OnRoleChangeState(int roleId, int state, int age);
	}
}
