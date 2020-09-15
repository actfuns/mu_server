using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000DD RID: 221
	public class OnCreateMonsterEventObject : EventObjectEx
	{
		// Token: 0x060003A6 RID: 934 RVA: 0x0003D118 File Offset: 0x0003B318
		public OnCreateMonsterEventObject(Monster monster) : base(30)
		{
			this.Monster = monster;
		}

		// Token: 0x0400050F RID: 1295
		public Monster Monster;
	}
}
