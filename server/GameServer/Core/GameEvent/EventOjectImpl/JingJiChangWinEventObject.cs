using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x02000008 RID: 8
	public class JingJiChangWinEventObject : EventObject
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00005A28 File Offset: 0x00003C28
		public JingJiChangWinEventObject(GameClient player, Robot robot, int type) : base(53)
		{
			this.player = player;
			this.robot = robot;
			this.type = type;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00005A4C File Offset: 0x00003C4C
		public int getType()
		{
			return this.type;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00005A64 File Offset: 0x00003C64
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00005A7C File Offset: 0x00003C7C
		public Robot getRobot()
		{
			return this.robot;
		}

		// Token: 0x04000039 RID: 57
		private GameClient player;

		// Token: 0x0400003A RID: 58
		private Robot robot;

		// Token: 0x0400003B RID: 59
		private int type;
	}
}
