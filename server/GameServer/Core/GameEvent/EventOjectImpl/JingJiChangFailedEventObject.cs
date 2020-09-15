using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	// Token: 0x02000007 RID: 7
	public class JingJiChangFailedEventObject : EventObject
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000059BC File Offset: 0x00003BBC
		public JingJiChangFailedEventObject(GameClient player, Robot robot, int type) : base(54)
		{
			this.player = player;
			this.robot = robot;
			this.type = type;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000059E0 File Offset: 0x00003BE0
		public int getType()
		{
			return this.type;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000059F8 File Offset: 0x00003BF8
		public GameClient getPlayer()
		{
			return this.player;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00005A10 File Offset: 0x00003C10
		public Robot getRobot()
		{
			return this.robot;
		}

		// Token: 0x04000036 RID: 54
		private GameClient player;

		// Token: 0x04000037 RID: 55
		private Robot robot;

		// Token: 0x04000038 RID: 56
		private int type;
	}
}
