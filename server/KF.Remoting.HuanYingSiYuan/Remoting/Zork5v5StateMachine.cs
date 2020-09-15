using System;

namespace Remoting
{
	// Token: 0x0200007B RID: 123
	internal class Zork5v5StateMachine
	{
		// Token: 0x06000604 RID: 1540 RVA: 0x00052624 File Offset: 0x00050824
		public Zork5v5StateMachine.StateType GetCurrState()
		{
			return this._CurrState;
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0005263C File Offset: 0x0005083C
		public void SetCurrState(Zork5v5StateMachine.StateType state, DateTime now, int param)
		{
			Zork5v5StateMachine.StateHandler oldHandler = this.Handlers[(int)this._CurrState];
			if (oldHandler != null)
			{
				oldHandler.Leave(now, param);
			}
			this._CurrState = state;
			Zork5v5StateMachine.StateHandler newHandler = this.Handlers[(int)this._CurrState];
			this._CurrStateEnterTicks = now.Ticks;
			if (newHandler != null)
			{
				newHandler.Enter(now, param);
			}
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0005269C File Offset: 0x0005089C
		public long ContinueTicks(DateTime now)
		{
			return now.Ticks - this._CurrStateEnterTicks;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x000526BC File Offset: 0x000508BC
		public void Install(Zork5v5StateMachine.StateHandler handler)
		{
			this.Handlers[(int)handler.State] = handler;
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x000526D0 File Offset: 0x000508D0
		public void Tick(DateTime now, int param)
		{
			Zork5v5StateMachine.StateHandler handler = this.Handlers[(int)this._CurrState];
			if (handler != null)
			{
				handler.Update(now, param);
			}
		}

		// Token: 0x0400034B RID: 843
		private Zork5v5StateMachine.StateHandler[] Handlers = new Zork5v5StateMachine.StateHandler[7];

		// Token: 0x0400034C RID: 844
		private Zork5v5StateMachine.StateType _CurrState = Zork5v5StateMachine.StateType.None;

		// Token: 0x0400034D RID: 845
		private long _CurrStateEnterTicks;

		// Token: 0x0200007C RID: 124
		public enum StateType
		{
			// Token: 0x0400034F RID: 847
			None,
			// Token: 0x04000350 RID: 848
			Init,
			// Token: 0x04000351 RID: 849
			SignUp,
			// Token: 0x04000352 RID: 850
			PrepareGame,
			// Token: 0x04000353 RID: 851
			NotifyEnter,
			// Token: 0x04000354 RID: 852
			GameStart,
			// Token: 0x04000355 RID: 853
			RankAnalyse,
			// Token: 0x04000356 RID: 854
			Max
		}

		// Token: 0x0200007D RID: 125
		public class StateHandler
		{
			// Token: 0x17000035 RID: 53
			// (get) Token: 0x0600060A RID: 1546 RVA: 0x0005271C File Offset: 0x0005091C
			// (set) Token: 0x0600060B RID: 1547 RVA: 0x00052733 File Offset: 0x00050933
			public Zork5v5StateMachine.StateType State { get; private set; }

			// Token: 0x0600060C RID: 1548 RVA: 0x0005273C File Offset: 0x0005093C
			public void Enter(DateTime now, int param)
			{
				if (this.enterAction != null)
				{
					this.enterAction(now, param);
				}
			}

			// Token: 0x0600060D RID: 1549 RVA: 0x00052768 File Offset: 0x00050968
			public void Update(DateTime now, int param)
			{
				if (this.updateAction != null)
				{
					this.updateAction(now, param);
				}
			}

			// Token: 0x0600060E RID: 1550 RVA: 0x00052794 File Offset: 0x00050994
			public void Leave(DateTime now, int param)
			{
				if (this.leaveAction != null)
				{
					this.leaveAction(now, param);
				}
			}

			// Token: 0x0600060F RID: 1551 RVA: 0x000527BF File Offset: 0x000509BF
			public StateHandler(Zork5v5StateMachine.StateType state, Action<DateTime, int> enter, Action<DateTime, int> updater, Action<DateTime, int> leaver)
			{
				this.State = state;
				this.enterAction = enter;
				this.updateAction = updater;
				this.leaveAction = leaver;
			}

			// Token: 0x04000357 RID: 855
			private Action<DateTime, int> enterAction = null;

			// Token: 0x04000358 RID: 856
			private Action<DateTime, int> updateAction = null;

			// Token: 0x04000359 RID: 857
			private Action<DateTime, int> leaveAction = null;
		}
	}
}
