using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic.JingJiChang.FSM
{
	// Token: 0x02000727 RID: 1831
	internal class ReturnState : IFSMState
	{
		// Token: 0x06002C84 RID: 11396 RVA: 0x0027C5E9 File Offset: 0x0027A7E9
		public ReturnState(Robot owner, FinishStateMachine FSM)
		{
			this.owner = owner;
			this.FSM = FSM;
		}

		// Token: 0x06002C85 RID: 11397 RVA: 0x0027C610 File Offset: 0x0027A810
		public void onBegin()
		{
		}

		// Token: 0x06002C86 RID: 11398 RVA: 0x0027C613 File Offset: 0x0027A813
		public void onEnd()
		{
		}

		// Token: 0x06002C87 RID: 11399 RVA: 0x0027C616 File Offset: 0x0027A816
		public void onUpdate(long now)
		{
		}

		// Token: 0x06002C88 RID: 11400 RVA: 0x0027C61C File Offset: 0x0027A81C
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

		// Token: 0x04003B18 RID: 15128
		public static readonly AIState state = AIState.RETURN;

		// Token: 0x04003B19 RID: 15129
		private Robot owner = null;

		// Token: 0x04003B1A RID: 15130
		private FinishStateMachine FSM = null;
	}
}
