using System;

namespace KF.Remoting
{
	// Token: 0x02000066 RID: 102
	internal class BHMatchStateMachine
	{
		// Token: 0x060004E7 RID: 1255 RVA: 0x00041144 File Offset: 0x0003F344
		public BHMatchStateMachine.StateType GetCurrState()
		{
			return this._CurrState;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0004115C File Offset: 0x0003F35C
		public void SetCurrState(BHMatchStateMachine.StateType state, DateTime now, int param)
		{
			BHMatchStateMachine.StateHandler oldHandler = this.Handlers[(int)this._CurrState];
			if (oldHandler != null)
			{
				oldHandler.Leave(now, param);
			}
			this._CurrState = state;
			BHMatchStateMachine.StateHandler newHandler = this.Handlers[(int)this._CurrState];
			this._CurrStateEnterTicks = now.Ticks;
			if (newHandler != null)
			{
				newHandler.Enter(now, param);
			}
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x000411BC File Offset: 0x0003F3BC
		public long ContinueTicks(DateTime now)
		{
			return now.Ticks - this._CurrStateEnterTicks;
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x000411DC File Offset: 0x0003F3DC
		public void Install(BHMatchStateMachine.StateHandler handler)
		{
			this.Handlers[(int)handler.State] = handler;
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x000411F0 File Offset: 0x0003F3F0
		public void Tick(DateTime now, int param)
		{
			BHMatchStateMachine.StateHandler handler = this.Handlers[(int)this._CurrState];
			if (handler != null)
			{
				handler.Update(now, param);
			}
		}

		// Token: 0x0400029D RID: 669
		private BHMatchStateMachine.StateHandler[] Handlers = new BHMatchStateMachine.StateHandler[7];

		// Token: 0x0400029E RID: 670
		private BHMatchStateMachine.StateType _CurrState = BHMatchStateMachine.StateType.None;

		// Token: 0x0400029F RID: 671
		private long _CurrStateEnterTicks;

		// Token: 0x02000067 RID: 103
		public enum StateType
		{
			// Token: 0x040002A1 RID: 673
			None,
			// Token: 0x040002A2 RID: 674
			Init,
			// Token: 0x040002A3 RID: 675
			SignUp,
			// Token: 0x040002A4 RID: 676
			PrepareGame,
			// Token: 0x040002A5 RID: 677
			NotifyEnter,
			// Token: 0x040002A6 RID: 678
			GameStart,
			// Token: 0x040002A7 RID: 679
			RankAnalyse,
			// Token: 0x040002A8 RID: 680
			Max
		}

		// Token: 0x02000068 RID: 104
		public class StateHandler
		{
			// Token: 0x17000025 RID: 37
			// (get) Token: 0x060004ED RID: 1261 RVA: 0x0004123C File Offset: 0x0003F43C
			// (set) Token: 0x060004EE RID: 1262 RVA: 0x00041253 File Offset: 0x0003F453
			public BHMatchStateMachine.StateType State { get; private set; }

			// Token: 0x060004EF RID: 1263 RVA: 0x0004125C File Offset: 0x0003F45C
			public void Enter(DateTime now, int param)
			{
				if (this.enterAction != null)
				{
					this.enterAction(now, param);
				}
			}

			// Token: 0x060004F0 RID: 1264 RVA: 0x00041288 File Offset: 0x0003F488
			public void Update(DateTime now, int param)
			{
				if (this.updateAction != null)
				{
					this.updateAction(now, param);
				}
			}

			// Token: 0x060004F1 RID: 1265 RVA: 0x000412B4 File Offset: 0x0003F4B4
			public void Leave(DateTime now, int param)
			{
				if (this.leaveAction != null)
				{
					this.leaveAction(now, param);
				}
			}

			// Token: 0x060004F2 RID: 1266 RVA: 0x000412DF File Offset: 0x0003F4DF
			public StateHandler(BHMatchStateMachine.StateType state, Action<DateTime, int> enter, Action<DateTime, int> updater, Action<DateTime, int> leaver)
			{
				this.State = state;
				this.enterAction = enter;
				this.updateAction = updater;
				this.leaveAction = leaver;
			}

			// Token: 0x040002A9 RID: 681
			private Action<DateTime, int> enterAction = null;

			// Token: 0x040002AA RID: 682
			private Action<DateTime, int> updateAction = null;

			// Token: 0x040002AB RID: 683
			private Action<DateTime, int> leaveAction = null;
		}
	}
}
