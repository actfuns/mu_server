using System;

namespace Remoting
{
	
	internal class Zork5v5StateMachine
	{
		
		public Zork5v5StateMachine.StateType GetCurrState()
		{
			return this._CurrState;
		}

		
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

		
		public long ContinueTicks(DateTime now)
		{
			return now.Ticks - this._CurrStateEnterTicks;
		}

		
		public void Install(Zork5v5StateMachine.StateHandler handler)
		{
			this.Handlers[(int)handler.State] = handler;
		}

		
		public void Tick(DateTime now, int param)
		{
			Zork5v5StateMachine.StateHandler handler = this.Handlers[(int)this._CurrState];
			if (handler != null)
			{
				handler.Update(now, param);
			}
		}

		
		private Zork5v5StateMachine.StateHandler[] Handlers = new Zork5v5StateMachine.StateHandler[7];

		
		private Zork5v5StateMachine.StateType _CurrState = Zork5v5StateMachine.StateType.None;

		
		private long _CurrStateEnterTicks;

		
		public enum StateType
		{
			
			None,
			
			Init,
			
			SignUp,
			
			PrepareGame,
			
			NotifyEnter,
			
			GameStart,
			
			RankAnalyse,
			
			Max
		}

		
		public class StateHandler
		{
			
			
			
			public Zork5v5StateMachine.StateType State { get; private set; }

			
			public void Enter(DateTime now, int param)
			{
				if (this.enterAction != null)
				{
					this.enterAction(now, param);
				}
			}

			
			public void Update(DateTime now, int param)
			{
				if (this.updateAction != null)
				{
					this.updateAction(now, param);
				}
			}

			
			public void Leave(DateTime now, int param)
			{
				if (this.leaveAction != null)
				{
					this.leaveAction(now, param);
				}
			}

			
			public StateHandler(Zork5v5StateMachine.StateType state, Action<DateTime, int> enter, Action<DateTime, int> updater, Action<DateTime, int> leaver)
			{
				this.State = state;
				this.enterAction = enter;
				this.updateAction = updater;
				this.leaveAction = leaver;
			}

			
			private Action<DateTime, int> enterAction = null;

			
			private Action<DateTime, int> updateAction = null;

			
			private Action<DateTime, int> leaveAction = null;
		}
	}
}
