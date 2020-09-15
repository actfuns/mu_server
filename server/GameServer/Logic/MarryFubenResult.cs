using System;

namespace GameServer.Logic
{
	// Token: 0x02000521 RID: 1313
	public enum MarryFubenResult
	{
		// Token: 0x040022EE RID: 8942
		Error = -1,
		// Token: 0x040022EF RID: 8943
		Success,
		// Token: 0x040022F0 RID: 8944
		ResultRoomInfo,
		// Token: 0x040022F1 RID: 8945
		NotMarriaged,
		// Token: 0x040022F2 RID: 8946
		InFuben,
		// Token: 0x040022F3 RID: 8947
		SelfOrOtherLimit,
		// Token: 0x040022F4 RID: 8948
		IsReaday,
		// Token: 0x040022F5 RID: 8949
		NotOpen,
		// Token: 0x040022F6 RID: 8950
		Error_Denied_For_Minor_Occupation = -35
	}
}
