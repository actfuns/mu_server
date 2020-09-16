using System;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic.JingJiChang.FSM
{
	
	internal class ReturnState : IFSMState
	{
		
		public ReturnState(Robot owner, FinishStateMachine FSM)
		{
			this.owner = owner;
			this.FSM = FSM;
		}

		
		public void onBegin()
		{
		}

		
		public void onEnd()
		{
		}

		
		public void onUpdate(long now)
		{
		}

		
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

		
		public static readonly AIState state = AIState.RETURN;

		
		private Robot owner = null;

		
		private FinishStateMachine FSM = null;
	}
}
