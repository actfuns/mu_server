using System;

namespace KF.Remoting
{
	// Token: 0x02000076 RID: 118
	internal class ZhengBaStateMachine
	{
		// Token: 0x060005DD RID: 1501 RVA: 0x0005031C File Offset: 0x0004E51C
		public ZhengBaStateMachine.StateType GetCurrState()
		{
			return this._CurrState;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00050334 File Offset: 0x0004E534
		public void SetCurrState(ZhengBaStateMachine.StateType state, DateTime now)
		{
			ZhengBaStateMachine.StateHandler oldHandler = this.Handlers[(int)this._CurrState];
			if (oldHandler != null)
			{
				oldHandler.Leave(now);
			}
			this._CurrState = state;
			ZhengBaStateMachine.StateHandler newHandler = this.Handlers[(int)this._CurrState];
			this._CurrStateEnterTicks = now.Ticks;
			if (newHandler != null)
			{
				newHandler.Enter(now);
			}
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x00050394 File Offset: 0x0004E594
		public long ContinueTicks(DateTime now)
		{
			return now.Ticks - this._CurrStateEnterTicks;
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x000503B4 File Offset: 0x0004E5B4
		public void Install(ZhengBaStateMachine.StateHandler handler)
		{
			this.Handlers[(int)handler.State] = handler;
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x000503C8 File Offset: 0x0004E5C8
		public void Tick(DateTime now)
		{
			ZhengBaStateMachine.StateHandler handler = this.Handlers[(int)this._CurrState];
			if (handler != null)
			{
				handler.Update(now);
			}
		}

		// Token: 0x0400032C RID: 812
		private ZhengBaStateMachine.StateHandler[] Handlers = new ZhengBaStateMachine.StateHandler[8];

		// Token: 0x0400032D RID: 813
		private ZhengBaStateMachine.StateType _CurrState = ZhengBaStateMachine.StateType.None;

		// Token: 0x0400032E RID: 814
		private long _CurrStateEnterTicks;

		// Token: 0x02000077 RID: 119
		public enum StateType
		{
			// Token: 0x04000330 RID: 816
			None,
			// Token: 0x04000331 RID: 817
			Idle,
			// Token: 0x04000332 RID: 818
			TodayPkStart,
			// Token: 0x04000333 RID: 819
			TodayPkEnd,
			// Token: 0x04000334 RID: 820
			InitPkLoop,
			// Token: 0x04000335 RID: 821
			NotifyEnter,
			// Token: 0x04000336 RID: 822
			PkLoopStart,
			// Token: 0x04000337 RID: 823
			PkLoopEnd,
			// Token: 0x04000338 RID: 824
			Max
		}

		// Token: 0x02000078 RID: 120
		public class StateHandler
		{
			// Token: 0x17000034 RID: 52
			// (get) Token: 0x060005E3 RID: 1507 RVA: 0x00050410 File Offset: 0x0004E610
			// (set) Token: 0x060005E4 RID: 1508 RVA: 0x00050427 File Offset: 0x0004E627
			public ZhengBaStateMachine.StateType State { get; private set; }

			// Token: 0x060005E5 RID: 1509 RVA: 0x00050430 File Offset: 0x0004E630
			public void Enter(DateTime now)
			{
				if (this.enterAction != null)
				{
					this.enterAction(now);
				}
			}

			// Token: 0x060005E6 RID: 1510 RVA: 0x0005045C File Offset: 0x0004E65C
			public void Update(DateTime now)
			{
				if (this.updateAction != null)
				{
					this.updateAction(now);
				}
			}

			// Token: 0x060005E7 RID: 1511 RVA: 0x00050488 File Offset: 0x0004E688
			public void Leave(DateTime now)
			{
				if (this.leaveAction != null)
				{
					this.leaveAction(now);
				}
			}

			// Token: 0x060005E8 RID: 1512 RVA: 0x000504B2 File Offset: 0x0004E6B2
			public StateHandler(ZhengBaStateMachine.StateType state, Action<DateTime> enter, Action<DateTime> updater, Action<DateTime> leaver)
			{
				this.State = state;
				this.enterAction = enter;
				this.updateAction = updater;
				this.leaveAction = leaver;
			}

			// Token: 0x04000339 RID: 825
			private Action<DateTime> enterAction = null;

			// Token: 0x0400033A RID: 826
			private Action<DateTime> updateAction = null;

			// Token: 0x0400033B RID: 827
			private Action<DateTime> leaveAction = null;
		}
	}
}
