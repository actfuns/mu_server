using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Logic;

namespace GameServer.Interface
{
	// Token: 0x020005B2 RID: 1458
	public interface IObject
	{
		// Token: 0x17000073 RID: 115
		
		ObjectTypes ObjectType { get; }

		// Token: 0x06001A5F RID: 6751
		int GetObjectID();

		// Token: 0x17000074 RID: 116
		
		
		long LastLifeMagicTick { get; set; }

		// Token: 0x17000075 RID: 117
		
		
		Point CurrentGrid { get; set; }

		// Token: 0x17000076 RID: 118
		
		
		Point CurrentPos { get; set; }

		// Token: 0x17000077 RID: 119
		
		int CurrentMapCode { get; }

		// Token: 0x17000078 RID: 120
		
		int CurrentCopyMapID { get; }

		// Token: 0x17000079 RID: 121
		
		
		Dircetions CurrentDir { get; set; }

		// Token: 0x1700007A RID: 122
		
		
		List<int> PassiveEffectList { get; set; }

		// Token: 0x06001A6C RID: 6764
		T GetExtComponent<T>(ExtComponentTypes type) where T : class;
	}
}
