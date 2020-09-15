using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200060C RID: 1548
	public class GoldCopySceneData
	{
		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06001EED RID: 7917 RVA: 0x001AD820 File Offset: 0x001ABA20
		// (set) Token: 0x06001EEE RID: 7918 RVA: 0x001AD837 File Offset: 0x001ABA37
		public List<int[]> m_MonsterPatorlPathList { get; set; }

		// Token: 0x04002BD3 RID: 11219
		public Dictionary<int, GoldCopySceneMonster> GoldCopySceneMonsterData = new Dictionary<int, GoldCopySceneMonster>();
	}
}
