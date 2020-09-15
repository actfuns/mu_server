using System;
using GameServer.Logic;
using GameServer.Logic.ActivityNew.SevenDay;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x020000FB RID: 251
	public class SevenDayGoalEventObject : EventObject
	{
		// Token: 0x060003E0 RID: 992 RVA: 0x0003D698 File Offset: 0x0003B898
		public SevenDayGoalEventObject() : base(32)
		{
			this.Reset();
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0003D6AC File Offset: 0x0003B8AC
		public void Reset()
		{
			this.Client = null;
			this.FuncType = ESevenDayGoalFuncType.Unknown;
			this.Arg1 = 0;
			this.Arg2 = 0;
			this.Arg3 = 0;
			this.Arg4 = 0;
		}

		// Token: 0x04000535 RID: 1333
		public GameClient Client;

		// Token: 0x04000536 RID: 1334
		public ESevenDayGoalFuncType FuncType;

		// Token: 0x04000537 RID: 1335
		public int Arg1;

		// Token: 0x04000538 RID: 1336
		public int Arg2;

		// Token: 0x04000539 RID: 1337
		public int Arg3;

		// Token: 0x0400053A RID: 1338
		public int Arg4;
	}
}
