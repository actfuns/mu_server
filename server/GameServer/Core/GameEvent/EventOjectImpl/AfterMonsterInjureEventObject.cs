using System;
using GameServer.Interface;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000D4 RID: 212
	public class AfterMonsterInjureEventObject : EventObjectEx
	{
		// Token: 0x0600039D RID: 925 RVA: 0x0003CFC5 File Offset: 0x0003B1C5
		public AfterMonsterInjureEventObject(IObject attacker, Monster monster, int sceneType, int injure, int merlininjure) : base(34)
		{
			this.Attacker = attacker;
			this.Monster = monster;
			this.SceneType = sceneType;
			this.Injure = injure;
			this.MerlinInjure = merlininjure;
		}

		// Token: 0x040004F4 RID: 1268
		public int SceneType;

		// Token: 0x040004F5 RID: 1269
		public IObject Attacker;

		// Token: 0x040004F6 RID: 1270
		public Monster Monster;

		// Token: 0x040004F7 RID: 1271
		public int Injure;

		// Token: 0x040004F8 RID: 1272
		public int MerlinInjure;
	}
}
