using System;
using GameServer.Logic;
using GameServer.Logic.Ornament;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x02000009 RID: 9
	public class OrnamentGoalEventObject : EventObject
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00005A94 File Offset: 0x00003C94
		public OrnamentGoalEventObject(GameClient Client, OrnamentGoalType FuncType, params int[] args) : base(37)
		{
			this.Reset();
			this.Client = Client;
			this.FuncType = FuncType;
			if (args.Length > 0)
			{
				this.Arg1 = args[0];
			}
			if (args.Length > 1)
			{
				this.Arg1 = args[1];
			}
			if (args.Length > 2)
			{
				this.Arg1 = args[2];
			}
			if (args.Length > 3)
			{
				this.Arg1 = args[3];
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00005B19 File Offset: 0x00003D19
		public void Reset()
		{
			this.Client = null;
			this.FuncType = OrnamentGoalType.OGT_UseGoods;
			this.Arg1 = 0;
			this.Arg2 = 0;
			this.Arg3 = 0;
			this.Arg4 = 0;
		}

		// Token: 0x0400003C RID: 60
		public GameClient Client;

		// Token: 0x0400003D RID: 61
		public OrnamentGoalType FuncType;

		// Token: 0x0400003E RID: 62
		public int Arg1;

		// Token: 0x0400003F RID: 63
		public int Arg2;

		// Token: 0x04000040 RID: 64
		public int Arg3;

		// Token: 0x04000041 RID: 65
		public int Arg4;
	}
}
