using System;

namespace KF.Remoting
{
	// Token: 0x0200006C RID: 108
	public class KuaFuLueDuoStateMachine
	{
		// Token: 0x06000557 RID: 1367 RVA: 0x0004778C File Offset: 0x0004598C
		public KuaFuLueDuoStateMachine.StateType GetCurrState()
		{
			return this._CurrState;
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x000477A4 File Offset: 0x000459A4
		public void SetCurrState(KuaFuLueDuoStateMachine.StateType state, DateTime now, int param)
		{
			KuaFuLueDuoStateMachine.StateHandler oldHandler = this.Handlers[(int)this._CurrState];
			if (oldHandler != null)
			{
				oldHandler.Leave(now, param);
			}
			this._CurrState = state;
			KuaFuLueDuoStateMachine.StateHandler newHandler = this.Handlers[(int)this._CurrState];
			this._CurrStateEnterTicks = now.Ticks;
			if (newHandler != null)
			{
				newHandler.Enter(now, param);
			}
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x00047804 File Offset: 0x00045A04
		public long ContinueTicks(DateTime now)
		{
			return now.Ticks - this._CurrStateEnterTicks;
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x00047824 File Offset: 0x00045A24
		public void Install(KuaFuLueDuoStateMachine.StateHandler handler)
		{
			this.Handlers[(int)handler.State] = handler;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x00047838 File Offset: 0x00045A38
		public void Tick(DateTime now, int param)
		{
			KuaFuLueDuoStateMachine.StateHandler handler = this.Handlers[(int)this._CurrState];
			if (handler != null)
			{
				handler.Update(now, param);
			}
		}

		// Token: 0x040002DD RID: 733
		private KuaFuLueDuoStateMachine.StateHandler[] Handlers = new KuaFuLueDuoStateMachine.StateHandler[7];

		// Token: 0x040002DE RID: 734
		private KuaFuLueDuoStateMachine.StateType _CurrState = KuaFuLueDuoStateMachine.StateType.None;

		// Token: 0x040002DF RID: 735
		private long _CurrStateEnterTicks;

		// Token: 0x040002E0 RID: 736
		public long Tag1;

		// Token: 0x040002E1 RID: 737
		public TimeSpan Tag2;

		// Token: 0x0200006D RID: 109
		public enum StateType
		{
			// Token: 0x040002E3 RID: 739
			None,
			// Token: 0x040002E4 RID: 740
			Init,
			// Token: 0x040002E5 RID: 741
			SignUp,
			// Token: 0x040002E6 RID: 742
			PrepareGame,
			// Token: 0x040002E7 RID: 743
			NotifyEnter,
			// Token: 0x040002E8 RID: 744
			GameStart,
			// Token: 0x040002E9 RID: 745
			RankAnalyse,
			// Token: 0x040002EA RID: 746
			Max
		}

		// Token: 0x0200006E RID: 110
		public class StateHandler
		{
			// Token: 0x1700002D RID: 45
			// (get) Token: 0x0600055D RID: 1373 RVA: 0x00047884 File Offset: 0x00045A84
			// (set) Token: 0x0600055E RID: 1374 RVA: 0x0004789B File Offset: 0x00045A9B
			public KuaFuLueDuoStateMachine.StateType State { get; private set; }

			// Token: 0x0600055F RID: 1375 RVA: 0x000478A4 File Offset: 0x00045AA4
			public void Enter(DateTime now, int param)
			{
				if (this.enterAction != null)
				{
					this.enterAction(now, param);
				}
			}

			// Token: 0x06000560 RID: 1376 RVA: 0x000478D0 File Offset: 0x00045AD0
			public void Update(DateTime now, int param)
			{
				if (this.updateAction != null)
				{
					this.updateAction(now, param);
				}
			}

			// Token: 0x06000561 RID: 1377 RVA: 0x000478FC File Offset: 0x00045AFC
			public void Leave(DateTime now, int param)
			{
				if (this.leaveAction != null)
				{
					this.leaveAction(now, param);
				}
			}

			// Token: 0x06000562 RID: 1378 RVA: 0x00047927 File Offset: 0x00045B27
			public StateHandler(KuaFuLueDuoStateMachine.StateType state, Action<DateTime, int> enter, Action<DateTime, int> updater, Action<DateTime, int> leaver)
			{
				this.State = state;
				this.enterAction = enter;
				this.updateAction = updater;
				this.leaveAction = leaver;
			}

			// Token: 0x040002EB RID: 747
			private Action<DateTime, int> enterAction = null;

			// Token: 0x040002EC RID: 748
			private Action<DateTime, int> updateAction = null;

			// Token: 0x040002ED RID: 749
			private Action<DateTime, int> leaveAction = null;
		}
	}
}
