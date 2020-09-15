using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic.JingJiChang.FSM
{
	// Token: 0x02000729 RID: 1833
	internal class NormalState : IFSMState
	{
		// Token: 0x06002C8F RID: 11407 RVA: 0x0027C77A File Offset: 0x0027A97A
		public NormalState(Robot owner, FinishStateMachine FSM)
		{
			this.owner = owner;
			this.FSM = FSM;
		}

		// Token: 0x06002C90 RID: 11408 RVA: 0x0027C7A1 File Offset: 0x0027A9A1
		public void onBegin()
		{
			this.changeAction(GActions.Stand);
		}

		// Token: 0x06002C91 RID: 11409 RVA: 0x0027C7AC File Offset: 0x0027A9AC
		public void onEnd()
		{
		}

		// Token: 0x06002C92 RID: 11410 RVA: 0x0027C7AF File Offset: 0x0027A9AF
		public void onUpdate(long now)
		{
		}

		// Token: 0x06002C93 RID: 11411 RVA: 0x0027C7B4 File Offset: 0x0027A9B4
		private void changeAction(GActions action)
		{
			if (this.owner.VLife > 0.0)
			{
				Point enemyPos = this.owner.EnemyTarget;
				double newDirection = this.owner.Direction;
				List<object> listObjs = Global.GetAll9Clients(this.owner);
				GameManager.ClientMgr.NotifyOthersDoAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.owner, this.owner.MonsterZoneNode.MapCode, this.owner.CopyMapID, this.owner.RoleID, (int)newDirection, (int)action, (int)this.owner.SafeCoordinate.X, (int)this.owner.SafeCoordinate.Y, (int)enemyPos.X, (int)enemyPos.Y, 114, listObjs);
				this.owner.DestPoint = new Point(-1.0, -1.0);
				Global.RemoveStoryboard(this.owner.Name);
				this.owner.Action = action;
			}
		}

		// Token: 0x04003B1E RID: 15134
		public static readonly AIState state = AIState.NORMAL;

		// Token: 0x04003B1F RID: 15135
		private Robot owner = null;

		// Token: 0x04003B20 RID: 15136
		private FinishStateMachine FSM = null;
	}
}
