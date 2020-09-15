using System;
using GameServer.Interface;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D3 RID: 211
	public class PreMonsterInjureEventObject : EventObjectEx
	{
		// Token: 0x0600039C RID: 924 RVA: 0x0003CFA3 File Offset: 0x0003B1A3
		public PreMonsterInjureEventObject(IObject attacker, Monster monster, int sceneType) : base(33)
		{
			this.Attacker = attacker;
			this.Monster = monster;
			this.SceneType = sceneType;
		}

		// Token: 0x040004F0 RID: 1264
		public int SceneType;

		// Token: 0x040004F1 RID: 1265
		public IObject Attacker;

		// Token: 0x040004F2 RID: 1266
		public Monster Monster;

		// Token: 0x040004F3 RID: 1267
		public int Injure;
	}
}
