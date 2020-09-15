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
		// (get) Token: 0x06001A5E RID: 6750
		ObjectTypes ObjectType { get; }

		// Token: 0x06001A5F RID: 6751
		int GetObjectID();

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06001A60 RID: 6752
		// (set) Token: 0x06001A61 RID: 6753
		long LastLifeMagicTick { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06001A62 RID: 6754
		// (set) Token: 0x06001A63 RID: 6755
		Point CurrentGrid { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06001A64 RID: 6756
		// (set) Token: 0x06001A65 RID: 6757
		Point CurrentPos { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06001A66 RID: 6758
		int CurrentMapCode { get; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06001A67 RID: 6759
		int CurrentCopyMapID { get; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06001A68 RID: 6760
		// (set) Token: 0x06001A69 RID: 6761
		Dircetions CurrentDir { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06001A6A RID: 6762
		// (set) Token: 0x06001A6B RID: 6763
		List<int> PassiveEffectList { get; set; }

		// Token: 0x06001A6C RID: 6764
		T GetExtComponent<T>(ExtComponentTypes type) where T : class;
	}
}
